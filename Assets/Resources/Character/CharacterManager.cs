using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CharacterManager : NetworkBehaviour {
    public static CharacterManager instance = null;
    public CharacterController[] characters = new CharacterController[Const.CHAR_NUMBER];
    //readonly SyncList<CharacterController> characters = new SyncList<CharacterController>();
    int number = 0;

    void Awake() {
        instance = this;
    }

    void Start() {
        GameManager.instance.OnStartGame += StartGameHandler;
        GameManager.instance.OnMove += MoveHandler;
    }

    public void AddCharacter(CharacterController character, Vector2Int position, int player) {
        characters[number] = character;
        number++;
        character.SetPosition(position);
        character.SetPlayer(player);
        //character.LocateCharacter();
    }

    public CharacterController Get(int id) {
        if (id < Const.CHAR_NUMBER) return characters[id];
        else return null;
    }

    public NetworkConnection GetOwner(int id) {
        CharacterController cc = Get(id);
        NetworkIdentity ni = cc.GetComponent<NetworkIdentity>();
        return ni.connectionToClient;
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
