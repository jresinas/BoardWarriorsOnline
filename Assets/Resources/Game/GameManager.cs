using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class WaitingResponse {
    public int casterId;
    public int skillIndex;
    public int targetId;
    public int result;
    public int[] results;

    public WaitingResponse(int casterId, int skillIndex, int targetId) {
        this.casterId = casterId;
        this.skillIndex = skillIndex;
        this.targetId = targetId;
    }
}

public class GameManager : NetworkBehaviour {
    public static GameManager instance = null;

    public event EventHandler OnStartGame;
    public event EventHandler OnStartRound;
    public event Action<NetworkConnection, int> OnStartTurn;
    public event EventHandler OnEndTurn;
    public event Action<int, Vector2Int> OnMove;
    public event Action<int, int, int> OnUseSkill;
    public event Action<int, int, int> OnUseResponseSkillPre;
    public event Action<int, int, int, int, int[]> OnUseResponseSkillPost;
    public event Action<NetworkConnection, int, int, int> OnRequestResponseSkill;
    public event Action<NetworkConnection, bool> OnWaitingResponseSkill;
    public event EventHandler OnEndActions;

    int turn;

    [SerializeField] int[] charactersPriority = new int[Const.CHAR_NUMBER];
    public int[] charactersOrder = new int[Const.CHAR_NUMBER * 2];
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
        for (int i = 0; i < Const.CHAR_NUMBER * 2; i++) charactersOrder[i] = i < Const.CHAR_NUMBER ? charactersPriority[i] : -1;
        turn = 0;
        NextCharacter();
    }
    void NextCharacter() {
        while (turn < Const.CHAR_NUMBER * 2 && charactersOrder[turn] < 0) turn++;

        if (turn >= Const.CHAR_NUMBER * 2) StartRound();
        else {
            activeCharacter = charactersOrder[turn];
            NetworkConnection owner = CharacterManager.instance.GetOwner(activeCharacter);
            SetActions(activeCharacter);
            if (OnStartTurn != null) OnStartTurn(owner, activeCharacter);
        }
    }

    void SetActions(int characterId) {
        actions = 1;
    }

    void ChangeActions(int value) {
        actions += value;
        if (actions <= 0 && OnEndActions != null) OnEndActions(this, EventArgs.Empty);
    }

    void SkipTurn() {
        if (turn < Const.CHAR_NUMBER) charactersOrder[(Const.CHAR_NUMBER * 2) - 1 - turn] = activeCharacter;
        EndTurn();
    }

    void EndTurn() {
        if (OnEndTurn != null) OnEndTurn(this, EventArgs.Empty);
        charactersOrder[turn] = -1;
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

    void RequestUseSkillHandler(int skillIndex, int targetId) {
        CmdUseSkill(skillIndex, targetId);
    }

    [Command(requiresAuthority = false)]
    void CmdUseSkill(int skillIndex, int targetId, NetworkConnectionToClient sender = null) {
        if (skillIndex >= 0 && targetId >= 0) UseSkill(skillIndex, targetId, sender);
    }

    void UseSkill(int skillIndex, int targetId, NetworkConnection sender, bool isResponse = false) {
        if (IsUserTurn(sender) && actions > 0 && CharacterManager.instance.AllowUseSkill(activeCharacter, skillIndex, targetId)) {
            if (!isResponse && CharacterManager.instance.CanResponse(activeCharacter, skillIndex, targetId)) {
                WaitingForResponse(skillIndex, targetId, sender);
                return;
            }
            ChangeActions(-1);
            if (OnUseSkill != null) OnUseSkill(activeCharacter, skillIndex, targetId);
        }
    }

    void WaitingForResponse(int skillIndex, int targetId, NetworkConnection sender) {
        NetworkConnection targetOwner = CharacterManager.instance.GetOwner(targetId);
        if (OnRequestResponseSkill != null) OnRequestResponseSkill(targetOwner, activeCharacter, skillIndex, targetId);
        if (OnWaitingResponseSkill != null) OnWaitingResponseSkill(sender, true);
        waitingResponse = new WaitingResponse(activeCharacter, skillIndex, targetId);
    }
    #endregion

    #region UseSkillResponse
    void SendResponseSkillHandler(int skillIndex) {
        CmdSendResponseSkillHandler(skillIndex);
    }

    [Command(requiresAuthority = false)]
    void CmdSendResponseSkillHandler(int skillIndex, NetworkConnectionToClient sender = null) {
        if (waitingResponse != null && CharacterManager.instance.GetOwner(waitingResponse.targetId) == sender && (skillIndex < 0 || CharacterManager.instance.AllowUseSkill(waitingResponse.targetId, skillIndex, waitingResponse.targetId))) {
            if (OnUseResponseSkillPre != null) OnUseResponseSkillPre(waitingResponse.targetId, skillIndex, waitingResponse.casterId);
            UseSkill(waitingResponse.skillIndex, waitingResponse.targetId, CharacterManager.instance.GetOwner(waitingResponse.casterId), true);
            if (OnWaitingResponseSkill != null) OnWaitingResponseSkill(CharacterManager.instance.GetOwner(waitingResponse.casterId), false);
            if (OnUseResponseSkillPost != null) OnUseResponseSkillPost(waitingResponse.targetId, skillIndex, waitingResponse.casterId, waitingResponse.result, waitingResponse.results);
            waitingResponse = null;
        }
    }

    void RequestEndTurnHandler(object source, EventArgs args) {
        CmdEndTurn();
    }
    #endregion

    #region Buttons
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
}