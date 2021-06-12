using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CharacterManager : NetworkBehaviour {
    public static CharacterManager instance = null;

    public event EventHandler<int> OnCharacterHoverEnter;
    public event EventHandler<int> OnCharacterHoverExit;

    public SyncList<Transform> characters = new SyncList<Transform>();

    void Awake() {
        instance = this;
    }

    void Start() {
        GameManager.instance.OnStartGame += StartGameHandler;
        GameManager.instance.OnMove += MoveHandler;
        GameManager.instance.OnUseSkill += UseSkillHandler;
    }

    //public void AddCharacter(CharacterController character, Vector2Int position, int player) {
    public void AddCharacter(Transform character, Vector2Int position, int player) {
        characters.Add(character);
        CharacterController characterController = character.GetComponent<CharacterController>();
        characterController.SetId(characters.IndexOf(character));
        characterController.SetPosition(position);
        characterController.SetPlayer(player);
    }

    public CharacterController Get(int id) {
        if (id < Const.CHAR_NUMBER) {
            CharacterController character = characters[id].GetComponent<CharacterController>();
            return character;
        } else return null;
    }

    public int GetId(Vector2Int position) {
        for (int i = 0; i < Const.CHAR_NUMBER; i++) {
            CharacterController character = Get(i);
            if (character.GetPosition() == position) return i;
        }
        return -1;
    }

    public List<int> GetPlayerCharacters(int player) {
        List<int> characters = new List<int>();
        for (int i = 0; i < Const.CHAR_NUMBER; i++) {
            CharacterController character = Get(i);
            if (character.GetPlayer() == player) characters.Add(i);
        }
        return characters;
    }

    public NetworkConnection GetOwner(int id) {
        CharacterController character = Get(id);
        if (character != null) {
            NetworkIdentity ni = character.GetComponent<NetworkIdentity>();
            return ni.connectionToClient;
        } else return null;
    }

    public bool AllowMove(int characterId, Vector2Int position) {
        CharacterController character = Get(characterId);
        return BoardUtils.Distance(character.GetPosition(), position) <= character.GetMovement();
    }

    // TODO
    public bool AllowUseSkill(int casterId, int skillIndex, int targetId) {
        CharacterController caster = Get(casterId);
        CharacterController target = Get(targetId);
        if (caster != null && target != null) {
            Skill skill = caster.GetSkill(skillIndex);
            return true;
        } else return false;
    }


    void StartGameHandler(object source, EventArgs args) {
        for (int i = 0; i < Const.CHAR_NUMBER; i++) Get(i).LocateCharacter(); //Get(i).SetId(i);
    }


    void MoveHandler(int characterId, Vector2Int destiny) {
        CharacterController character = Get(characterId);
        Vector2Int currentPosition = character.GetPosition();
        character.MoveAnimation(currentPosition, destiny);
        character.Move(destiny);
        //Get(characterId).transform.position = BoardManager.instance.GetTile(position).transform.position;
    }

    void UseSkillHandler(int casterId, int skillIndex, int targetId) {
        CharacterController caster = Get(casterId);
        bool success = caster.UseSkill(skillIndex, targetId);
        caster.UseSkillAnimation(skillIndex, targetId, success);
    }

    public void EnterHover(int characterId) {
        if (OnCharacterHoverEnter!= null) OnCharacterHoverEnter(this, characterId);
    }

    public void ExitHover(int characterId) {
        if (OnCharacterHoverExit != null) OnCharacterHoverExit(this, characterId);
    }
}
