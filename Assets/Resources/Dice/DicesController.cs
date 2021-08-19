using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DicesController : MonoBehaviour {
    public Dice[] dices;
    public GameObject dicesView;

    /// <summary>
    /// Disable all dices
    /// </summary>
    public void Hide() {
        dicesView.SetActive(false);
        foreach (Dice dice in dices) dice.Hide();
    }

    /// <summary>
    /// Simulate a dice roll of a specified number of dices
    /// </summary>
    /// <param name="dicesNumber">Number of dices to roll</param>
    /// <returns>Array with results</returns>
    public int[] Roll(int dicesNumber) {
        int[] results = new int[dicesNumber];
        for (int i = 0; i < dicesNumber; i++) results[i] = dices[i].Roll();
        return results;
    }

    /// <summary>
    /// Returns number of succesful dices for a given result of dice rolls
    /// </summary>
    /// <param name="results">Array of dice results</param>
    /// <param name="minRequired">Min dice value required to success</param>
    /// <returns></returns>
    public int GetResult(int[] results, int minRequired) {
        int success = 0;
        foreach (int result in results)
            if (result >= minRequired) success++;
        return success;
    }

    /// <summary>
    /// Show animation of a dice roll with specified results
    /// </summary>
    /// <param name="results">Array of dice results</param>
    /// <param name="minRequired">Min dice value required to success</param>
    public void Show(int[] results, int minRequired) {
        Hide();
        dicesView.SetActive(true);
        for (int i = 0; i < results.Length; i++)
            dices[i].Show(results[i], minRequired);
    }
}
