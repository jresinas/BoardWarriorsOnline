using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class UserController : NetworkBehaviour {
    [SerializeField] GameObject[] characters;
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
    public void LoadCharacters(int player) {
        int characterIndex = 0;
        foreach (GameObject character in characters) {
            GameObject charObject = Instantiate(character);
            NetworkServer.Spawn(charObject, netIdentity.connectionToClient);
            //CharacterController charController = charObject.GetComponent<CharacterController>();
            //CharacterManager.instance.AddCharacter(charController, character.position, character.player);
            Vector2Int position = new Vector2Int(characterIndex, player == 0 ? 4 : 0);
            CharacterManager.instance.AddCharacter(charObject.transform, position, player);
            characterIndex++;
        }
    }
}
