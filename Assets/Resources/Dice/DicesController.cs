using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RollResult {
    public int[] results;
    public int successes;

    public RollResult(int[] results, int successes) {
        this.results = results;
        this.successes = successes;
    }
}

public class DicesController : MonoBehaviour {
    public Dice[] dices;
    public GameObject dicesView;
    [SerializeField] TextMeshProUGUI totalSuccessTMP;
    int totalSuccess;

    /// <summary>
    /// Disable all dices
    /// </summary>
    public void Hide() {
        dicesView.SetActive(false);
        totalSuccess = 0;
        totalSuccessTMP.text = "";
        Restart();
    }

    void Restart() {
        foreach (Dice dice in dices) dice.Hide();
    }

    /// <summary>
    /// Simulate a dice roll of a specified number of dices
    /// </summary>
    /// <param name="dicesNumber">Number of dices to roll</param>
    /// <param name="minRequired">Min dice value required to make a success</param>
    /// <returns>Roll results (dice results and number of succesful dices)</returns>
    public RollResult Roll(int dicesNumber, int minRequired) {
        int[] results = new int[dicesNumber];
        int successes = 0;
        for (int i = 0; i < dicesNumber; i++) {
            results[i] = dices[0].Roll();
            if (results[i] >= minRequired) successes++;
        }
        return new RollResult(results, successes);
    }

    /// <summary>
    /// Show animation of a dice roll with specified results
    /// </summary>
    /// <param name="results">Array of dice results</param>
    /// <param name="minRequired">Min dice value required to success</param>
    public void Show(int[] results, int minRequired) {
        StartCoroutine(RollAnimation(results, minRequired));
    }

    IEnumerator RollAnimation(int[] results, int minRequired) {
        List<int> dices = new List<int>(results);
        while (dices.Count > 0) {
            Restart();
            dicesView.SetActive(true);
            int dicesToGet = Mathf.Min(Const.MAX_DICES, dices.Count);
            List<int> selectedDices = dices.GetRange(0, dicesToGet);
            dices.RemoveRange(0, dicesToGet);
            //StartCoroutine(RollAnimationStep(selectedDices, minRequired));
            RollAnimationStep(selectedDices, minRequired);
            yield return new WaitForSeconds(Const.DICE_ROLL_TIME);
            UpdateSuccess();
        }
        DiceManager.instance.EndRollDicesAnim();
    }

    void RollAnimationStep(List<int> results, int minRequired) {
        for (int i = 0; i < results.Count; i++) {
            dices[i].Show(results[i], minRequired);
            if (results[i] >= minRequired) totalSuccess++;
        }
    }

    void UpdateSuccess() {
        totalSuccessTMP.text = totalSuccess.ToString();
    }

}
