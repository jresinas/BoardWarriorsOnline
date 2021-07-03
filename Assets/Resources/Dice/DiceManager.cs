using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DiceManager : NetworkBehaviour {
    public static DiceManager instance = null;

    public event Action<int, int[]> OnRollDices;

    [SerializeField] DicesController dices;

    void Awake() {
        instance = this;
    }

    void Start() {
        GameManager.instance.OnEndTurn += EndTurnHandler;
    }

    [ClientRpc]
    void EndTurnHandler(object sender, EventArgs args) {
        dices.Hide();
    }

    [Server]
    public int RollDices(int dicesNumber, int minRequired) {
        int[] results = dices.Roll(dicesNumber);
        ShowDices(results, minRequired);
        int result = dices.GetResult(results, minRequired);
        if (OnRollDices != null) OnRollDices(result, results);
        return result;
    }

    [ClientRpc]
    void ShowDices(int[] results, int minRequired) {
        dices.Show(results, minRequired);
    }
}
