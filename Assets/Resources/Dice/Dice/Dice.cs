using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dice : MonoBehaviour {
    public string diceNumber;
    public Animator anim;
    public GameObject lightPoint;
    // if true, lightPoint will be enabled after dice roll
    bool success = false;

    /// <summary>
    /// Simulate a dice roll
    /// </summary>
    public int Roll() {
        int result = Random.Range(1, 7);
        return result;
    }

    /// <summary>
    /// Show dice roll animation
    /// </summary>
    /// <param name="value">Result of dice roll</param>
    /// <param name="required">Min dice value required to success</param>
    public void Show(int value, int required) {
        gameObject.SetActive(true);
        anim.SetTrigger("Result" + value);
        success = (value >= required);
    }

    /// <summary>
    /// Disable dice and sets its default value
    /// </summary>
    public void Hide() {
        lightPoint.SetActive(false);
        gameObject.SetActive(false);
        success = false;
        anim.SetTrigger("Reset");
    }

    /// <summary>
    /// Callback from AnimEvent when finish dice roll animation. Enable light behind dice if success
    /// </summary>
    void EndRollAnimation() {
        lightPoint.SetActive(success);
    }
}
