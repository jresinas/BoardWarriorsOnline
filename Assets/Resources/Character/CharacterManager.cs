using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CharacterManager : NetworkBehaviour {
    public static CharacterManager instance = null;

    public event EventHandler<int> OnCharacterHoverEnter;
    public event EventHandler<int> OnCharacterHoverExit;
    public event Action<int, int> OnChangeHealth;
    public event Action<int, int> OnChangeEnergy;

    public SyncList<Transform> characters = new SyncList<Transform>();
    int characterWaitingAnimation = -1;

    void Awake() {
        instance = this;
    }

    void Start() {
        GameManager.instance.OnStartGame += StartGameHandler;
        GameManager.instance.OnStartRound += StartRoundHandler;
        GameManager.instance.OnMove += MoveHandler;
        GameManager.instance.OnUseSkill += UseSkillHandler;
        GameManager.instance.OnUseResponseSkillPre += UseResponseSkillPreHandler;
        GameManager.instance.OnUseResponseSkillPost += UseResponseSkillPostHandler;
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
        if (id < Const.CHAR_NUMBER && id >= 0) {
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

    [Server]
    public bool AllowMove(int characterId, Vector2Int position) {
        CharacterController character = Get(characterId);
        return BoardUtils.Distance(character.GetPosition(), position) <= character.GetMovement();
    }

    [Server]
    public bool AllowUseSkill(int casterId, int skillIndex, Vector2Int destiny) {
        CharacterController caster = Get(casterId);
        if (caster != null && skillIndex >= 0) {
            Skill skill = caster.GetSkill(skillIndex);
            return skill.AllowTarget(destiny);
        } else return false;
    }

    [Server]
    public bool CanResponse(int casterId, int skillIndex, Vector2Int destiny) {
        bool result = false;
        CharacterController caster = Get(casterId);
        if (caster != null) {
            Skill skill = caster.GetSkill(skillIndex);
            if (skill.TargetCharacter()) {
                int targetId = GetId(destiny);
                CharacterController target = Get(targetId);
                if (target != null) {
                    for (int i = 0; i < Const.SKILL_NUMBER; i++) {
                        if (target.GetSkill(i) is SkillResponse) result = true;
                    }
                }
            }
        }
        return result;
    }

    void StartGameHandler(object source, EventArgs args) {
        for (int i = 0; i < Const.CHAR_NUMBER; i++) Get(i).LocateCharacter();
    }

    void StartRoundHandler(object source, EventArgs args) {
        for (int i = 0; i < Const.CHAR_NUMBER; i++) Get(i).ChangeEnergy(1);
    }

    [Server]
    void MoveHandler(int characterId, Vector2Int destiny) {
        CharacterController character = Get(characterId);
        Vector2Int currentPosition = character.GetPosition();
        character.MoveAnimation(currentPosition, destiny);
        character.Move(destiny);
        //Get(characterId).transform.position = BoardManager.instance.GetTile(position).transform.position;
    }

    [Server]
    void UseSkillHandler(int casterId, int skillIndex, Vector2Int destiny) {
        CharacterController caster = Get(casterId);
        SkillResult result = caster.UseSkill(skillIndex, destiny);
        caster.UseSkillAnimation(skillIndex, result.target, result.success);
    }

    [Server]
    void UseResponseSkillPreHandler(int casterId, int skillIndex, int targetId) {
        Debug.Log("ResponseSkillPre");
    }

    [Server]
    void UseResponseSkillPostHandler(int casterId, int skillIndex, int targetId, int result, int[] results) {
        Debug.Log("ResponseSkillPost");
    }


    public void EnterHover(int characterId) {
        if (OnCharacterHoverEnter!= null) OnCharacterHoverEnter(this, characterId);
    }

    public void ExitHover(int characterId) {
        if (OnCharacterHoverExit != null) OnCharacterHoverExit(this, characterId);
    }

    [ClientRpc]
    public void ChangeEnergy(int characterId, int energy) {
        if (OnChangeEnergy != null) OnChangeEnergy(characterId, energy);
    }

    [ClientRpc]
    public void ChangeHealth(int characterId, int health) {
        if (OnChangeHealth != null) OnChangeHealth(characterId, health);
    }



    public void EndSkillAnimation(int characterId) {
        if (characterWaitingAnimation != characterId) {
            if (characterWaitingAnimation < 0) characterWaitingAnimation = characterId;
            else StartCoroutine(EndAnimation(new int[] { characterWaitingAnimation, characterId }));
        }
    }

    IEnumerator EndAnimation(int[] characters) {
        // Delay after last character end animation
        yield return new WaitForSeconds(Const.WAIT_AFTER_SKILL_ANIM);
        for (int i = 0; i < characters.Length; i++) {
            CharacterController character = Get(characters[i]);
            character.EndAnimation();
        }
        characterWaitingAnimation = -1;
    }
}
