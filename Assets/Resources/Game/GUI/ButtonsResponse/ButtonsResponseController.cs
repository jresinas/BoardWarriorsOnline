using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsResponseController : MonoBehaviour {
    [SerializeField] Button sendResponse;

    public void ShowButtons() {
        gameObject.SetActive(true);
    }

    public void HideButtons() {
        gameObject.SetActive(false);
    }

    public void ClickSendResponse() {
        GUIManager.instance.ClickSendResponseButton();
    }
}
