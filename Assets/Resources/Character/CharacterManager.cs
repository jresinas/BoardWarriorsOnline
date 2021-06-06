using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CharacterManager : NetworkBehaviour {
    public static CharacterManager instance = null;
    //public CharacterController[] characters = new CharacterController[Const.CHAR_NUMBER];
    public SyncList<Transform> characters = new SyncList<Transform>();
    int number = 0;

    void Awake() {
        instance = this;
    }

    void Start() {
        GameManager.instance.OnStartGame += StartGameHandler;
        GameManager.instance.OnMove += MoveHandler;
    }

    //public void AddCharacter(CharacterController character, Vector2Int position, int player) {
    public void AddCharacter(Transform character, Vector2Int position, int player) {
        //characters[number] = character;
        //number++;
        //character.SetPosition(position);
        //character.SetPlayer(player);
        characters.Add(character);
        CharacterController characterController = character.GetComponent<CharacterController>();
        characterController.SetPosition(position);
        characterController.SetPlayer(player);
    }

    public CharacterController Get(int id) {
        if (id < Const.CHAR_NUMBER) {
            CharacterController character = characters[id].GetComponent<CharacterController>();
            return character;
        } else return null;
    }

    public NetworkConnection GetOwner(int id) {
        CharacterController character = Get(id);
        if (character != null) {
            NetworkIdentity ni = character.GetComponent<NetworkIdentity>();
            return ni.connectionToClient;
        } else return null;
    }

    public bool AllowMove(int characterId, Vector2Int position) {
        return true;
    }


    void StartGameHandler(object source, EventArgs args) {
        for (int i = 0; i < Const.CHAR_NUMBER; i++) Get(i).LocateCharacter(); //Get(i).SetId(i);
    }


    void MoveHandler(int characterId, Vector2Int position) {
        CharacterController character = Get(characterId);
        character.MoveAnimation(position);
        character.Move(position);
        //Get(characterId).transform.position = BoardManager.instance.GetTile(position).transform.position;
    }
}
