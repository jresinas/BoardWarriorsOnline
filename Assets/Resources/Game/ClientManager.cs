using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ClientManager : NetworkBehaviour {
    public static ClientManager instance = null;

    int playerId;
    [SerializeField] Camera[] cameras = new Camera[2];

    public event Action<Vector2Int> OnRequestMove;
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
        GameManager.instance.OnEndTurn += EndTurnHandler;
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
        GetCamera().gameObject.SetActive(true);
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
    void TargetStartTurnHandler(NetworkConnection userConnection, int characterId) {
        StartTurn();
    }

    [ClientRpc]
    void EndTurnHandler(object source, EventArgs args) {
        EndTurn();
    }

    [ClientRpc]
    void EndActionsHandler(object source, EventArgs args) {
        isAvailableMove = false;
        isAvailableSkill = false;
        GUIManager.instance.DisableButtons(skip: true);
        GUIManager.instance.ShowCharacter();
    }

    void StartTurn() {
        isTurn = true;
        isAvailableMove = true;
        isAvailableSkill = true;
        GUIManager.instance.EnableButtons();
    }

    void EndTurn() {
        isTurn = false;
        isAvailableMove = false;
        isAvailableSkill = false;
        GUIManager.instance.DisableButtons();
    }
    void ClickTileHandler(object source, Vector2Int destiny) {
        if (IsAvailableSkill() && GUIManager.instance.IsSkillSelected() && OnRequestUseSkill != null) OnRequestUseSkill(GUIManager.instance.GetSkillSelected(), destiny);
        else if (IsAvailableMove() && OnRequestMove != null) OnRequestMove(destiny);
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
        return IsTurn() && isAvailableSkill;
    }
}
