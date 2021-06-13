using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DicesController : MonoBehaviour {
    public Dice[] dices;
    public GameObject dicesView;

    public void Hide() {
        dicesView.SetActive(false);
        foreach (Dice dice in dices) dice.Hide();
    }

    /*
    public int[] Roll(int dicesNumber) {
        Reset();
        int[] results = new int[dicesNumber];
        dicesView.SetActive(true);
        for (int i = 0; i < dicesNumber; i++) results[i] = dices[i].Roll(7);
        return results;
    }
    */

    public int[] Roll(int dicesNumber, int minRequired) {
        int[] results = new int[dicesNumber];
        for (int i = 0; i < dicesNumber; i++) results[i] = dices[i].Roll(minRequired);
        return results;
    }

    public void Show(int[] results, int minRequired) {
        Hide();
        dicesView.SetActive(true);
        for (int i = 0; i < results.Length; i++)
            dices[i].Show(results[i], minRequired);
    }

    public int GetResult(int[] dices, int minRequired) {
        int result = 0;
        foreach (int dice in dices)
            if (dice >= minRequired) result++;
        return result;
    }
}
