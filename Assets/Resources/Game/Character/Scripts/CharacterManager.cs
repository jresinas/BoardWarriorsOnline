using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CharacterManager : NetworkBehaviour {
    public static CharacterManager instance = null;

    // Event triggered pn client when pointer starts hovering over a character
    // * int: character id
    public event EventHandler<int> OnCharacterHoverEnter;
    // Event triggered on client when pointer ends hovering over a character
    // * int: character id
    public event EventHandler<int> OnCharacterHoverExit;
    // Event triggered on client when character's health is modified
    // * int: character id
    // * int: new health value
    public event Action<int, int> OnChangeHealth;
    // Event triggered on client when character's energy is modified
    // * int: character id
    // * int: new energy value
    public event Action<int, int> OnChangeEnergy;
    // Event triggered on server when a character is death (health <= 0)
    // * int: character id
    public event EventHandler<int> OnDeath;

    // List of characters in game
    public SyncList<Transform> characters = new SyncList<Transform>();

    void Awake() {
        instance = this;
    }

    void Start() {
        GameManager.instance.OnStartGame += StartGameHandler;
        GameManager.instance.OnStartRound += StartRoundHandler;
        GameManager.instance.OnEndTurn += EndTurndHandler;
        GameManager.instance.OnMove += MoveHandler;
        GameManager.instance.OnUseSkill += UseSkillHandler;
        GameManager.instance.OnUseResponseSkillPre += UseResponseSkillPreHandler;
        GameManager.instance.OnUseResponseSkillPost += UseResponseSkillPostHandler;
    }

    /// <summary>
    /// Add new character to the game
    /// </summary>
    public void AddCharacter(Transform character, Vector2Int position, int player) {
        characters.Add(character);
        CharacterController characterController = character.GetComponent<CharacterController>();
        characterController.SetId(characters.IndexOf(character));
        characterController.SetPosition(position);
        characterController.SetPlayer(player);
    }

    /// <summary>
    /// Returns the CharacterController for a given id
    /// </summary>
    public CharacterController Get(int id) {
        if (id < Const.CHAR_NUMBER && id >= 0) {
            CharacterController character = characters[id].GetComponent<CharacterController>();
            if (character.gameObject.activeSelf) return character;
        }
        return null;
    }

    /// <summary>
    /// Returns a list of CharacterController for a given array of ids
    /// </summary>
    public List<CharacterController> Get(int[] ids) {
        List<CharacterController> targets = new List<CharacterController>();
        foreach (int id in ids) {
            CharacterController target = Get(id);
            if (target != null) targets.Add(target);
        }
        return targets;
    }

    /// <summary>
    /// Returns the character id in specified position of the board
    /// </summary>
    public int GetId(Vector2Int position) {
        for (int i = 0; i < Const.CHAR_NUMBER; i++) {
            CharacterController character = Get(i);
            if (character != null && character.GetPosition() == position) return i;
        }
        return -1;
    }

    /// <summary>
    /// Returns the list of characters id owned by specified player
    /// </summary>
    public List<int> GetPlayerCharacters(int player) {
        List<int> characters = new List<int>();
        for (int i = 0; i < Const.CHAR_NUMBER; i++) {
            CharacterController character = Get(i);
            if (character != null && character.GetPlayer() == player) characters.Add(i);
        }
        return characters;
    }

    /// <summary>
    /// Returns the player connection for a given character id
    /// </summary>
    public NetworkConnection GetOwner(int characterId) {
        CharacterController character = Get(characterId);
        if (character != null) {
            NetworkIdentity ni = character.GetComponent<NetworkIdentity>();
            return ni.connectionToClient;
        } else return null;
    }

    #region Permissions
    [Server]
    public bool AllowMove(int characterId, Vector2Int position) {
        CharacterController character = Get(characterId);
        return BoardUtils.Reach(character.GetPosition(), position, character.GetMovement());
    }

    [Server]
    public bool AllowUseSkill(int casterId, int skillIndex, Vector2Int destiny) {
        CharacterController caster = Get(casterId);
        if (caster != null && skillIndex >= 0) {
            Skill skill = caster.GetSkill(skillIndex);
            return skill.AllowTarget(destiny);
        } else return false;
    }

    /// <summary>
    /// Check if a skill used could be responded by another skill
    /// </summary>
    /// <param name="casterId">Character id who use the original skill</param>
    /// <param name="skillIndex">Skill (index) used by original caster</param>
    /// <param name="destiny">Position that targets original skill</param>
    /// <returns></returns>
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
    #endregion

    #region EventHandlers
    void StartGameHandler(object source, EventArgs args) {
        CharacterController character;
        for (int i = 0; i < Const.CHAR_NUMBER; i++) if ((character = Get(i)) != null) character.PlaceCharacter();
    }

    void StartRoundHandler(object source, EventArgs args) {
        CharacterController character;
        for (int i = 0; i < Const.CHAR_NUMBER; i++) if ((character = Get(i)) != null) character.ChangeEnergy(1);
    }

    [ClientRpc]
    private void EndTurndHandler(object source, EventArgs args) {
        foreach (Transform character in characters) {
            CharacterController cc = character.GetComponent<CharacterController>();
            if (cc != null) RefreshHealth(cc.id, cc.GetHealth());
        }
    }

    [Server]
    void MoveHandler(int characterId, Vector2Int destiny) {
        CharacterController character = Get(characterId);
        Vector2Int currentPosition = character.GetPosition();
        character.MoveAnimation(currentPosition, destiny);
        character.Move(destiny);
    }

    [Server]
    void UseSkillHandler(int casterId, int skillIndex, Vector2Int destiny) {
        CharacterController caster = Get(casterId);
        SkillResult result = caster.UseSkill(skillIndex, destiny);
        caster.UseSkillAnimation(skillIndex, result.targets, result.success, result.observers, result.data);
    }

    [Server]
    void UseResponseSkillPreHandler(int casterId, int skillIndex, int targetId) {
        Debug.Log("ResponseSkillPre");
    }

    [Server]
    void UseResponseSkillPostHandler(int casterId, int skillIndex, int targetId, int result, int[] results) {
        Debug.Log("ResponseSkillPost");
    }
    #endregion

    [Client]
    public void EnterHover(int characterId) {
        if (OnCharacterHoverEnter!= null) OnCharacterHoverEnter(this, characterId);
    }

    [Client]
    public void ExitHover(int characterId) {
        if (OnCharacterHoverExit != null) OnCharacterHoverExit(this, characterId);
    }

    [ClientRpc]
    public void RefreshEnergy(int characterId, int energy) {
        if (OnChangeEnergy != null) OnChangeEnergy(characterId, energy);
    }

    [Client]
    public void RefreshHealth(int characterId, int health) {
        if (OnChangeHealth != null) OnChangeHealth(characterId, health);
    }

    [Server]
    public void Death(int characterId) {
        if (OnDeath != null) OnDeath(this, characterId);
    }
}
