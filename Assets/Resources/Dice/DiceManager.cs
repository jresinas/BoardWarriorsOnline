using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DiceManager : NetworkBehaviour {
    public static DiceManager instance = null;

    // Event triggered on server when a dice roll is made
    // * int: number of successful dices
    // * int[]: results of dice roll
    public event Action<int, int[]> OnRollDices;

    [SerializeField] DicesController dices;

    void Awake() {
        instance = this;
    }

    void Start() {
        GameManager.instance.OnEndOfTurn += EndOfTurnHandler;
    }

    [ClientRpc]
    void EndOfTurnHandler(object sender, EventArgs args) {
        dices.Hide();
    }

    /// <summary>
    /// Make a dice roll
    /// </summary>
    /// <param name="dicesNumber">Number of dices to roll</param>
    /// <param name="minRequired">Min dice value required to success</param>
    /// <returns></returns>
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
