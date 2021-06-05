using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[Serializable]
public class CharacterPlayer {
    public GameObject prefab;
    public Vector2Int position;
    public int player;
}

public class UserController : NetworkBehaviour {
    //[SerializeField] GameObject[] characters;
    [SerializeField] CharacterPlayer[] characters;
    public bool isTurn = false;

    /*
        public GameObject[] GetCharacters() {
            return characters;
        }

        public void LoadCharacters() {
            foreach (GameObject character in characters) {
                GameObject charObject = Instantiate(character);
                NetworkServer.Spawn(charObject, netIdentity.connectionToClient);
                CharacterController charController = charObject.GetComponent<CharacterController>();
                CharacterManager.instance.AddCharacter(charController);
            }
        }
    */
    public void LoadCharacters() {
        foreach (CharacterPlayer character in characters) {
            GameObject charObject = Instantiate(character.prefab);
            NetworkServer.Spawn(charObject, netIdentity.connectionToClient);
            CharacterController charController = charObject.GetComponent<CharacterController>();
            CharacterManager.instance.AddCharacter(charController, character.position, character.player);
        }
    }
}
