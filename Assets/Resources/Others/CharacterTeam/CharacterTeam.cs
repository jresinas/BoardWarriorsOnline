using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTeam : MonoBehaviour {
    [SerializeField] CharacterController character;
    //Color COLOR_PLAYER1 = new Color(255, 0, 0);
    //Color COLOR_PLAYER1 = new Color32(255, 30, 30, 100);
    Color COLOR_PLAYER1 = new Color32(255, 30, 30, 150);
    //Color COLOR_PLAYER1 = new Color32(120, 15, 15, 150);
    //Color COLOR_PLAYER2 = new Color(0, 125, 255);
    //Color COLOR_PLAYER2 = new Color32(0, 180, 255, 100);
    //Color COLOR_PLAYER2 = new Color32(0, 255, 255, 150);
    Color COLOR_PLAYER2 = new Color32(0, 100, 255, 150);
    //Color COLOR_PLAYER2 = new Color32(0, 44, 120, 150);

    void Start() {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (character.GetPlayer() == 0) {
            sr.color = COLOR_PLAYER1;
        } else {
            sr.color = COLOR_PLAYER2;
        }
    }
}
