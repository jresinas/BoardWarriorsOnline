using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsController : MonoBehaviour {
    [SerializeField] Button endTurn;
    [SerializeField] Button skip;

    public void ShowButtons() {
        gameObject.SetActive(true);
    }

    public void HideButtons() {
        gameObject.SetActive(false);
    }

    public void EnableSkip() {
        skip.interactable = true;
    }

    public void EnableEndTurn() {
        endTurn.interactable = true;
    }

    public void DisableSkip() {
        skip.interactable = false;
    }

    public void DisableEndTurn() {
        endTurn.interactable = false;
    }

    public void ClickSkip() {
        GUIManager.instance.ClickSkipButton();
    }

    public void ClickEndTurn() {
        GUIManager.instance.ClickEndTurnButton();
    }
}
