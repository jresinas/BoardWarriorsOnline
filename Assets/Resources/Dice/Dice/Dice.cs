using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dice : MonoBehaviour {
    public string diceNumber;
    public Animator anim;
    public GameObject lightPoint;
    bool success = false;

    public int Roll() {
        int result = Random.Range(1, 7);
        return result;
    }

    public void Show(int value, int required) {
        gameObject.SetActive(true);
        anim.SetTrigger("Result" + value);
        success = (value >= required);
    }

    void Success() {
        lightPoint.SetActive(success);
    }

    public void Hide() {
        lightPoint.SetActive(false);
        gameObject.SetActive(false);
        success = false;
        anim.SetTrigger("Reset");
    }
}
