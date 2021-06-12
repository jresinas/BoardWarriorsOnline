using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ClientManager : NetworkBehaviour {
    public static ClientManager instance = null;

    public event Action<Vector2Int> OnRequestMove;
    public event Action<int, Vector2Int> OnRequestUseSkill;

    bool isTurn = false;
    bool isAvailableMove = false;
    bool isAvailableSkill = false;

    void Awake() {
        instance = this;
    }

    void Start()     {
        GameManager.instance.OnStartTurn += TargetStartTurnHandler;
        GameManager.instance.OnEndTurn += EndTurnHandler;
        GameManager.instance.OnEndActions += EndActionsHandler;
        BoardManager.instance.OnClickTile += ClickTileHandler;
    }

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

    void ClickTileHandler(object source, Vector2Int destiny) {
        if (IsAvailableSkill() && GUIManager.instance.IsSkillSelected() && OnRequestUseSkill != null) OnRequestUseSkill(GUIManager.instance.GetSkillSelected(), destiny);
        else if (IsAvailableMove() && OnRequestMove != null) OnRequestMove(destiny);
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

    public bool IsTurn() {
        return isTurn;
    }

    public bool IsAvailableMove() {
        return IsTurn() && isAvailableMove;
    }

    public bool IsAvailableSkill() {
        return IsTurn() && isAvailableSkill;
    }
}
