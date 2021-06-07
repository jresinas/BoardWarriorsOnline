using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour {
    public static GameManager instance = null;

    [SerializeField] GameObject[] cameras = new GameObject[2];

    public event EventHandler OnStartGame;
    public event Action<NetworkConnection, int> OnStartTurn;
    public event EventHandler OnEndTurn;
    public event Action<int, Vector2Int> OnMove;

    int turn;
    //[SerializeField] CharacterController[] charactersPriority = new CharacterController[Const.CHAR_NUMBER];
    //CharacterController[] charactersOrder = new CharacterController[Const.CHAR_NUMBER*2];
    //CharacterController activeCharacter;
    [SerializeField] int[] charactersPriority = new int[Const.CHAR_NUMBER];
    int[] charactersOrder = new int[Const.CHAR_NUMBER * 2];
    int activeCharacter;

    void Awake() {
        instance = this;    
    }

    void Start() {
        GUIManager.instance.OnRequestMove += RequestMoveHandler;
        GUIManager.instance.OnRequestUseSkill += RequestUseSkillHandler;
    }

    #region SetupGame
    public void PlayersReady(GameObject[] players) {
        for (int i = 0; i < 2; i++) {
            UserController playerController = players[i].GetComponent<UserController>();
            playerController.LoadCharacters(i);
            LoadCamera(playerController.netIdentity.connectionToClient, i);
        }
        StartCoroutine(SetupGame(players));
    }

    [TargetRpc]
    void LoadCamera(NetworkConnection conn, int playerNumber) {
        cameras[playerNumber].SetActive(true);
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
            Debug.Log(owner);
            if (OnStartTurn != null) OnStartTurn(owner, activeCharacter);
        }
    }

    void SkipTurn() {
        if (turn < Const.CHAR_NUMBER) charactersOrder[Const.CHAR_NUMBER - 1 - turn] = activeCharacter;
        EndTurn();
    }

    void EndTurn() {
        if (OnEndTurn != null) OnEndTurn(this, EventArgs.Empty);
        charactersOrder[turn] = -1;
        NextCharacter();
    }
    #endregion

    #region GameEvents
    void RequestMoveHandler(object source, Vector2Int destiny) {
        CmdMove(destiny);
    }

    [Command(requiresAuthority = false)]
    void CmdMove(Vector2Int destiny, NetworkConnectionToClient sender = null) {
        if (IsUserTurn(sender) && CharacterManager.instance.AllowMove(activeCharacter, destiny))
            if (OnMove != null) OnMove(activeCharacter, destiny);
 
        Debug.Log("This is a command send by" + sender + " who wants to move to "+destiny);
    }

    void RequestUseSkillHandler(object source, Vector2Int destiny) {
        CmdUseSkill(destiny);
    }

    [Command(requiresAuthority = false)]
    void CmdUseSkill(Vector2Int destiny, NetworkConnectionToClient sender = null) {
        Debug.Log("This is a command send by" + netIdentity + " who wants to use a skill");
    }

    bool IsUserTurn(NetworkConnection conn) {
        CharacterController character = CharacterManager.instance.Get(activeCharacter);
        Debug.Log("Actual character selected is " + activeCharacter + "who is owned by" + character.netIdentity.connectionToClient);
        return character.netIdentity.connectionToClient == conn;
    }
    #endregion


    private void Update() {
        if (Input.GetButtonDown("Jump")) {
            EndTurn();
        }
    }
}