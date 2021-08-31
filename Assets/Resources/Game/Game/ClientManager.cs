using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ClientManager : NetworkBehaviour {
    public static ClientManager instance = null;

    int playerId;
    [SerializeField] Camera[] cameras = new Camera[2];

    // Event triggered on client when a player request to move a character
    // * Vector2Int: position destiny
    public event Action<Vector2Int> OnRequestMove;
    // Event triggered on client when a player request to move a character
    // * int: turn character skill index
    // * Vector2Int: skill target position
    public event Action<int, Vector2Int> OnRequestUseSkill;
    public event Action<int> OnSendResponseSkill;

    bool isTurn = false;
    bool isAvailableMove = false;
    bool isAvailableSkill = false;
    bool isResponding = false;
    bool isWaiting = false;

    void Awake() {
        instance = this;
    }

    void Start()     {
        GameManager.instance.OnStartTurn += TargetStartTurnHandler;
        GameManager.instance.OnEndOfTurn += EndOfTurnHandler;
        GameManager.instance.OnEndActions += EndActionsHandler;
        BoardManager.instance.OnClickTile += ClickTileHandler;
        GameManager.instance.OnRequestResponseSkill += RequestResponseSkillHandler;
        GameManager.instance.OnWaitingResponseSkill += WaitingResponseSkillHandler;
    }


    #region SetupGame
    [TargetRpc]
    public void LoadPlayer(NetworkConnection conn, int playerId) {
        this.playerId = playerId;
        LoadCamera();
    }

    void LoadCamera() {
        Camera camera = GetCamera();
        GUIManager.instance.SetUICamera(camera);
        camera.gameObject.SetActive(true);
    }

    public Camera GetCamera() {
        return cameras[playerId];
    }
    #endregion


    #region ResponseSkill
    [TargetRpc]
    void WaitingResponseSkillHandler(NetworkConnection userConnection, bool status) {
        isWaiting = status;
    }

    [TargetRpc]
    void RequestResponseSkillHandler(NetworkConnection userConnection, int casterId, int skillIndex, int targetId) {
        isResponding = true;
        Debug.Log("Response");
    }

    public void SendResponseSkill(int skillIndex = -1) {
        isResponding = false;
        if (OnSendResponseSkill != null) OnSendResponseSkill(skillIndex);
    }
    #endregion


    #region GameEvents
    [TargetRpc]
    void TargetStartTurnHandler(NetworkConnection userConnection, int characterId, bool canSkip) {
        StartTurn(canSkip);
    }

    [ClientRpc]
    void EndOfTurnHandler(object source, EventArgs args) {
        EndTurn();
    }

    [ClientRpc]
    void EndActionsHandler(object source, EventArgs args) {
        isAvailableMove = false;
        isAvailableSkill = false;
        GUIManager.instance.DisableButtons(skip: true);
        GUIManager.instance.ShowCharacter();
    }

    void StartTurn(bool canSkip) {
        isTurn = true;
        isAvailableMove = true;
        isAvailableSkill = true;
        GUIManager.instance.EnableButtons(endTurn: true, skip: canSkip);
    }

    void EndTurn() {
        isTurn = false;
        isAvailableMove = false;
        isAvailableSkill = false;
        GUIManager.instance.DisableButtons();
    }
    void ClickTileHandler(object source, Vector2Int destiny) {
        if (IsAvailableSkill() && GUIManager.instance.IsSkillSelected()) {
            if (OnRequestUseSkill != null) OnRequestUseSkill(GUIManager.instance.GetSkillSelected(), destiny);
        } else if (IsAvailableMove() && OnRequestMove != null) OnRequestMove(destiny);
    }
    #endregion


    public bool IsTurn() {
        return isTurn;
    }

    public bool IsResponding() {
        return isResponding;
    }

    public bool IsAvailableMove() {
        return IsTurn() && isAvailableMove;
    }

    public bool IsAvailableSkill() {
        return IsTurn() && isAvailableSkill && !IsResponding();
    }
}
