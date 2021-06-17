using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour {
    public static GameManager instance = null;

    public event EventHandler OnStartGame;
    public event Action<NetworkConnection, int> OnStartTurn;
    public event EventHandler OnEndTurn;
    public event Action<int, Vector2Int> OnMove;
    public event Action<int, int, int> OnUseSkill;
    public event EventHandler OnEndActions;

    int turn;

    [SerializeField] int[] charactersPriority = new int[Const.CHAR_NUMBER];
    public int[] charactersOrder = new int[Const.CHAR_NUMBER * 2];
    int activeCharacter;
    int actions = 1;

    void Awake() {
        instance = this;    
    }

    void Start() {
        ClientManager.instance.OnRequestMove += RequestMoveHandler;
        ClientManager.instance.OnRequestUseSkill += RequestUseSkillHandler;
        GUIManager.instance.OnRequestEndTurn += RequestEndTurnHandler;
        GUIManager.instance.OnRequestSkip += RequestSkipHandler;
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
        for (int i = 0; i < Const.CHAR_NUMBER * 2; i++) charactersOrder[i] = i < Const.CHAR_NUMBER ? charactersPriority[i] : -1;
        turn = 0;
        NextCharacter();
    }
    void NextCharacter() {
        while (turn < Const.CHAR_NUMBER*2 && charactersOrder[turn] < 0) turn++;
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

    void RequestUseSkillHandler(int skillIndex, Vector2Int destiny) {
        CmdUseSkill(skillIndex, destiny);
    }

    [Command(requiresAuthority = false)]
    void CmdUseSkill(int skillIndex, Vector2Int destiny, NetworkConnectionToClient sender = null) {
        Debug.Log("This is a command send by" + netIdentity + " who wants to use a skill");
        int targetId = CharacterManager.instance.GetId(destiny);
        if (IsUserTurn(sender) && actions > 0 && CharacterManager.instance.AllowUseSkill(activeCharacter, skillIndex, targetId)) {
            ChangeActions(-1);
            if (OnUseSkill != null) OnUseSkill(activeCharacter, skillIndex, targetId);
        }
    }

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

    bool IsUserTurn(NetworkConnection conn) {
        CharacterController character = CharacterManager.instance.Get(activeCharacter);
        Debug.Log("Actual character selected is " + activeCharacter + "who is owned by" + character.netIdentity.connectionToClient);
        return character.netIdentity.connectionToClient == conn;
    }
    #endregion

    //* TESTING *//
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
}