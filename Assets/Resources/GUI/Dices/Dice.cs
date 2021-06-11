using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dice : MonoBehaviour {
    public string diceNumber;
    public Animator anim;
    public GameObject lightPoint;
    bool success = false;

    public int Roll(int required) {
        gameObject.SetActive(true);
        int result = Random.Range(1, 7);
        anim.SetTrigger("Result" + result);
        success = (result >= required);
        return result;
    }

    void Success() {
        lightPoint.SetActive(success);
    }

    public void Reset() {
        lightPoint.SetActive(false);
        gameObject.SetActive(false);
        success = false;
        anim.SetTrigger("Reset");
    }
}
