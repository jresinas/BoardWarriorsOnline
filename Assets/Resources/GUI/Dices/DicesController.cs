using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DicesController : MonoBehaviour {
    public Dice[] dices;
    public GameObject dicesView;

    public void Reset() {
        dicesView.SetActive(false);
        foreach (Dice dice in dices) dice.Reset();
    }

    public int[] Roll(int dicesNumber) {
        Reset();
        int[] results = new int[dicesNumber];
        dicesView.SetActive(true);
        for (int i = 0; i < dicesNumber; i++) results[i] = dices[i].Roll(7);
        return results;
    }

    public int Roll(int dicesNumber, int minRequired) {
        Reset();
        int result = 0;
        dicesView.SetActive(true);
        for (int i = 0; i < dicesNumber; i++) 
            if (dices[i].Roll(minRequired) >= minRequired) result++;
        return result;
    }
}
