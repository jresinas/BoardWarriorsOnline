using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GUIManager : NetworkBehaviour {
    public static GUIManager instance = null;

    public event EventHandler<Vector2Int> OnRequestMove;
    public event EventHandler<Vector2Int> OnRequestUseSkill;

    bool isTurn = false;
    bool skillSelected = false;

    void Awake() {
        instance = this;
    }

    void Start() {
        GameManager.instance.OnStartTurn += StartTurnHandler;
        GameManager.instance.OnEndTurn += EndTurnHandler;
        BoardManager.instance.OnClickTile += ClickTileHandler;
    }

    [TargetRpc]
    void StartTurnHandler(NetworkConnection userConnection, int characterId) {
        isTurn = true;
    }

    [ClientRpc]
    void EndTurnHandler(object source, EventArgs args) {
        isTurn = false;
    }

    void ClickTileHandler(object source, Vector2Int destiny) {
        if (isTurn) {
            Debug.Log("This is a client sending a request: " + netIdentity);
            if (skillSelected && OnRequestUseSkill != null) OnRequestUseSkill(this, destiny);
            else if (OnRequestMove != null) OnRequestMove(this, destiny);
        }
    }

    public bool IsTurn() {
        return isTurn;
    }
}
