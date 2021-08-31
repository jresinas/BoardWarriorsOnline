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
    public event Action OnEndRollDicesAnim;

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
        RollResult rollResult = dices.Roll(dicesNumber, minRequired);
        ShowDices(rollResult.results, minRequired);
        if (OnRollDices != null) OnRollDices(rollResult.successes, rollResult.results);
        return rollResult.successes;
    }

    [ClientRpc]
    void ShowDices(int[] results, int minRequired) {
        dices.Show(results, minRequired);
    }

    [Client]
    public void EndRollDicesAnim() {
        if (OnEndRollDicesAnim != null) OnEndRollDicesAnim();
    }
}
