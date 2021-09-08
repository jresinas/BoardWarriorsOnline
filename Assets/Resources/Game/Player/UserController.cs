using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class UserController : NetworkBehaviour {
    [SerializeField] GameObject[] characters;
    public bool isTurn = false;

    void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadCharacters(int player) {
        int characterIndex = 0;
        foreach (GameObject character in characters) {
            GameObject charObject = Instantiate(character);
            NetworkServer.Spawn(charObject, netIdentity.connectionToClient);
            Vector2Int position = new Vector2Int(characterIndex, player == 0 ? (Const.BOARD_ROWS-1) : 0);
            CharacterManager.instance.AddCharacter(charObject.transform, position, player);
            characterIndex++;
        }
    }
}
