using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTeam : MonoBehaviour {
    [SerializeField] CharacterController character;
    
    void Start() {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (character.GetPlayer() == 0) {
            sr.color = Const.COLOR_PLAYER1;
        } else {
            sr.color = Const.COLOR_PLAYER2;
        }
    }
}
