using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WaitingResponse {
    public int casterId;
    public int skillIndex;
    public int targetId;
    public Vector2Int destiny;
    public int result;
    public int[] results;

    //public WaitingResponse(int casterId, int skillIndex, int targetId) {
    public WaitingResponse(int casterId, int skillIndex, Vector2Int destiny, int targetId) {
        this.casterId = casterId;
        this.skillIndex = skillIndex;
        this.targetId = targetId;
        this.destiny = destiny;
    }
}

public class GameManager : NetworkBehaviour {
    public static GameManager instance = null;

    public event EventHandler OnStartGame;
    public event EventHandler OnStartRound;
    public event Action<NetworkConnection, int, bool> OnStartTurn;
    public event EventHandler OnEndTurn;
    public event EventHandler<int> OnSkipTurn;
    public event EventHandler OnEndOfTurn;
    public event Action<int, Vector2Int> OnMove;
    public event Action<int, int, Vector2Int> OnUseSkill;
    public event Action<int, int, int> OnUseResponseSkillPre;
    public event Action<int, int, int, int, int[]> OnUseResponseSkillPost;
    public event Action<NetworkConnection, int, int, int> OnRequestResponseSkill;
    public event Action<NetworkConnection, bool> OnWaitingResponseSkill;
    public event EventHandler OnEndActions;

    //public int turn;

    [SyncVar] public List<int> charactersPriority = new List<int>(); //int[] charactersPriority = new int[Const.CHAR_NUMBER];
    //public int[] charactersOrder = new int[Const.CHAR_NUMBER * 2];
    LinkedList<int> firstRound = new LinkedList<int>();
    LinkedList<int> secondRound = new LinkedList<int>();
    bool isFirstRound;
    int activeCharacter;
    int actions = 1;
    // store data for current action waiting response
    WaitingResponse waitingResponse = null;

    void Awake() {
        instance = this;    
    }

    void Start() {
        ClientManager.instance.OnRequestMove += RequestMoveHandler;
        ClientManager.instance.OnRequestUseSkill += RequestUseSkillHandler;
        ClientManager.instance.OnSendResponseSkill += SendResponseSkillHandler;
        GUIManager.instance.OnRequestEndTurn += RequestEndTurnHandler;
        GUIManager.instance.OnRequestSkip += RequestSkipHandler;
        DiceManager.instance.OnRollDices += RollDicesHandler;
        CharacterManager.instance.OnDeath += DeathHandler;
    }


    #region SetupGame
    public void PlayersReady(GameObject[] players) {
        for (int i = 0; i < Const.PLAYER_NUMBER; i++) {
            UserController playerController = players[i].GetComponent<UserController>();
            playerController.LoadCharacters(i);
            ClientManager.instance.LoadPlayer(playerController.netIdentity.connectionToClient, i);
        }
        StartCoroutine(SetupGame(players));
    }

    IEnumerator SetupGame(GameObject[] players) {
        yield return new WaitForSeconds(2);
        StartGame();
    }
    #endregion


    #region GameFlow
    void StartGame() {
        if (OnStartGame != null) OnStartGame(this, EventArgs.Empty);
        StartRound();
    }

    void StartRound() {
        if (OnStartRound != null) OnStartRound(this, EventArgs.Empty);
        //for (int i = 0; i < charactersOrder.Length ; i++) charactersOrder[i] = i < charactersPriority.Count ? charactersPriority[i] : -1;
        //turn = 0;
        //foreach (int id in charactersPriority) firstRound.Enqueue(id);
        firstRound = new LinkedList<int>(charactersPriority);
        secondRound.Clear();
        NextCharacter();
    }

    void NextCharacter() {
        /*
        while (turn < charactersOrder.Length && charactersOrder[turn] < 0) turn++;

        if (turn >= charactersOrder.Length) StartRound();
        else {
            activeCharacter = charactersOrder[turn];
            NetworkConnection owner = CharacterManager.instance.GetOwner(activeCharacter);
            SetActions(activeCharacter);
            if (OnStartTurn != null) OnStartTurn(owner, activeCharacter, turn < charactersPriority.Count);
        }
        */
        if (firstRound.Count > 0) {
            activeCharacter = firstRound.First.Value;
            firstRound.RemoveFirst();
            isFirstRound = true;
        } else if (secondRound.Count > 0) {
            activeCharacter = secondRound.First.Value;
            secondRound.RemoveFirst();
            isFirstRound = false;
        } else {
            StartRound();
            return;
        }
        NetworkConnection owner = CharacterManager.instance.GetOwner(activeCharacter);
        SetActions(activeCharacter);
        if (OnStartTurn != null) OnStartTurn(owner, activeCharacter, isFirstRound);
    }

    void SetActions(int characterId) {
        actions = 1;
    }

    void ChangeActions(int value) {
        actions += value;
        if (actions <= 0 && OnEndActions != null) OnEndActions(this, EventArgs.Empty);
    }

    void SkipTurn() {
        if (isFirstRound) {
            secondRound.AddFirst(activeCharacter);
            if (OnSkipTurn != null) OnSkipTurn(this, activeCharacter);

            //if (turn < charactersPriority.Count) charactersOrder[GetSecondRound(turn)] = activeCharacter;

            EndOfTurn();
        }
    }

    void EndTurn() {
        if (OnEndTurn != null) OnEndTurn(this, EventArgs.Empty);
        EndOfTurn();
    }

    void EndOfTurn() {
        if (OnEndOfTurn != null) OnEndOfTurn(this, EventArgs.Empty);
        //charactersOrder[turn] = -1;
        NextCharacter();
    }
    #endregion


    #region GameEvents

    #region Move
    void RequestMoveHandler(Vector2Int destiny) {
        CmdMove(destiny);
    }

    [Command(requiresAuthority = false)]
    void CmdMove(Vector2Int destiny, NetworkConnectionToClient sender = null) {
        Debug.Log("This is a command send by" + sender + " who wants to move to " + destiny);
        if (IsUserTurn(sender) && actions > 0 && CharacterManager.instance.AllowMove(activeCharacter, destiny)) {
            ChangeActions(-1);
            if (OnMove != null) OnMove(activeCharacter, destiny);
        }
    }
    #endregion

    #region UseSkill
    void RollDicesHandler(int result, int[] results) {
        if (waitingResponse != null) {
            waitingResponse.result = result;
            waitingResponse.results = results;
        }
    }

    void RequestUseSkillHandler(int skillIndex, Vector2Int destiny) {
            CmdUseSkill(skillIndex, destiny);
    }

    [Command(requiresAuthority = false)]
    void CmdUseSkill(int skillIndex, Vector2Int destiny, NetworkConnectionToClient sender = null) {
        if (skillIndex >= 0) UseSkill(skillIndex, destiny, sender);
    }

    void UseSkill(int skillIndex, Vector2Int destiny, NetworkConnection sender, bool isResponse = false) {
        if (IsUserTurn(sender) && actions > 0 && CharacterManager.instance.AllowUseSkill(activeCharacter, skillIndex, destiny)) {
            if (!isResponse && CharacterManager.instance.CanResponse(activeCharacter, skillIndex, destiny)) {
                WaitingForResponse(skillIndex, destiny, sender);
                return;
            }
            ChangeActions(-1);
            if (OnUseSkill != null) OnUseSkill(activeCharacter, skillIndex, destiny);
        }
    }

    void WaitingForResponse(int skillIndex, Vector2Int destiny, NetworkConnection sender) {
        int targetId = CharacterManager.instance.GetId(destiny);
        NetworkConnection targetOwner = CharacterManager.instance.GetOwner(targetId);
        if (OnRequestResponseSkill != null) OnRequestResponseSkill(targetOwner, activeCharacter, skillIndex, targetId);
        if (OnWaitingResponseSkill != null) OnWaitingResponseSkill(sender, true);
        waitingResponse = new WaitingResponse(activeCharacter, skillIndex, destiny, targetId);
    }
    #endregion

    #region UseSkillResponse
    void SendResponseSkillHandler(int skillIndex) {
        CmdSendResponseSkillHandler(skillIndex);
    }

    [Command(requiresAuthority = false)]
    void CmdSendResponseSkillHandler(int skillIndex, NetworkConnectionToClient sender = null) {
        if (waitingResponse != null && CharacterManager.instance.GetOwner(waitingResponse.targetId) == sender && (skillIndex < 0 || CharacterManager.instance.AllowUseSkill(waitingResponse.targetId, skillIndex, waitingResponse.destiny))) {
            if (OnUseResponseSkillPre != null) OnUseResponseSkillPre(waitingResponse.targetId, skillIndex, waitingResponse.casterId);
            UseSkill(waitingResponse.skillIndex, waitingResponse.destiny, CharacterManager.instance.GetOwner(waitingResponse.casterId), true);
            if (OnWaitingResponseSkill != null) OnWaitingResponseSkill(CharacterManager.instance.GetOwner(waitingResponse.casterId), false);
            if (OnUseResponseSkillPost != null) OnUseResponseSkillPost(waitingResponse.targetId, skillIndex, waitingResponse.casterId, waitingResponse.result, waitingResponse.results);
            waitingResponse = null;
        }
    }
    #endregion

    #region Buttons
    void RequestEndTurnHandler(object source, EventArgs args) {
        CmdEndTurn();
    }

    [Command(requiresAuthority = false)]
    void CmdEndTurn(NetworkConnectionToClient sender = null) {
        Debug.Log("This is a command send by" + netIdentity + " who wants to finish turn");
        if (IsUserTurn(sender)) EndTurn();
    }

    void RequestSkipHandler(object source, EventArgs args) {
        CmdSkip();
    }

    [Command(requiresAuthority = false)]
    void CmdSkip(NetworkConnectionToClient sender = null) {
        Debug.Log("This is a command send by" + netIdentity + " who wants to skip turn");
        if (IsUserTurn(sender) && actions > 0) SkipTurn();
    }
    #endregion

    #region Death
    void DeathHandler(object source, int characterId) {
        charactersPriority.Remove(characterId);
        if (firstRound.Contains(characterId)) firstRound.Remove(characterId);
        if (secondRound.Contains(characterId)) secondRound.Remove(characterId);
        if (activeCharacter == characterId) NextCharacter();

        // fix turn index
        //int characterIndex = charactersPriority.IndexOf(characterId);
        //if (turn > characterIndex) turn--;
        //if (turn > GetSecondRound(characterIndex)) turn--;

        // Fix charactersPriority
        //List<int> newCharactersPriority = new List<int>(charactersPriority);
        //newCharactersPriority.Remove(characterId);

        // Fix charactersOrder
        //int[] newCharactersOrder = new int[newCharactersPriority.Count * 2];
        //int j = 0;
        //Debug.Log(characterIndex);
        //Debug.Log(GetSecondRound(characterIndex));
        //for (int i = 0; i < charactersOrder.Length; i++) {
        //    Debug.Log(i);
        //    if (i != characterIndex && i != GetSecondRound(characterIndex)) {
        //        Debug.Log(j);
        //        newCharactersOrder[j] = charactersOrder[i];
        //        j++;
        //    }
        //}

        /*
        int[] newCharactersOrder = new int[newCharactersPriority.Count * 2]; //new int[charactersOrder.Length-2];
        int j = 0;
        for (int i = 0; i < charactersOrder.Length; i++) {
            if (j < newCharactersOrder.Length && charactersOrder[i] != characterId) {
                newCharactersOrder[j] = charactersOrder[i];
                j++;
            }
        }
        */

        //charactersPriority = newCharactersPriority;
        //charactersOrder = newCharactersOrder;       
    }
    #endregion

    #endregion


    bool IsUserTurn(NetworkConnection conn) {
        CharacterController character = CharacterManager.instance.Get(activeCharacter);
        Debug.Log("Actual character selected is " + activeCharacter + "who is owned by" + character.netIdentity.connectionToClient);
        return character.netIdentity.connectionToClient == conn;
    }


    //* TESTING *//
    /*
    private void Update() {
        if (Input.GetButtonDown("Jump")) {
            //RollDices();
            CharacterManager.instance.Get(activeCharacter).ChangeHealth(-1);
        }

        if (Input.GetButtonDown("Fire1")) {
            CharacterManager.instance.Get(activeCharacter).ChangeEnergy(1);
        }
    }

    [ClientRpc]
    void RollDices() {
        GUIManager.instance.RollDices(3, 2);
    }
    */

    //int GetSecondRound(int index) {
    //    return (charactersPriority.Count * 2) - (index + 1);
    //}
}