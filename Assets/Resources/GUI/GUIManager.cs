using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GUIManager : NetworkBehaviour {
    public static GUIManager instance = null;

    public event EventHandler<Vector2Int> OnRequestMove;
    public event EventHandler<Vector2Int> OnRequestUseSkill;

    [SerializeField] CharacterDataController characterData;

    bool isTurn = false;
    Skill skillSelected = null;

    void Awake() {
        instance = this;
    }

    void Start() {
        GameManager.instance.OnStartTurn += TargetStartTurnHandler;
        GameManager.instance.OnStartTurn += StartTurnHandler;
        GameManager.instance.OnEndTurn += EndTurnHandler;
        BoardManager.instance.OnClickTile += ClickTileHandler;
        CharacterManager.instance.OnCharacterHoverEnter += CharacterHoverEnterHandler;
        CharacterManager.instance.OnCharacterHoverExit += CharacterHoverExitHandler;
    }

    [TargetRpc]
    void TargetStartTurnHandler(NetworkConnection userConnection, int characterId) {
        isTurn = true;
    }

    [ClientRpc]
    void EndTurnHandler(object source, EventArgs args) {
        isTurn = false;
    }

    [ClientRpc]
    void StartTurnHandler(object source, int characterId) {
        skillSelected = null;
        CharacterController character = CharacterManager.instance.Get(characterId);
        if (character != null) characterData.SelectCharacter(character);
    }

    void ClickTileHandler(object source, Vector2Int destiny) {
        if (IsTurn()) {
            if (IsSkillSelected() && OnRequestUseSkill != null) OnRequestUseSkill(this, destiny);
            else if (OnRequestMove != null) OnRequestMove(this, destiny);
        }
    }

    void CharacterHoverEnterHandler(object sender, int characterId) {
        CharacterController character = CharacterManager.instance.Get(characterId);
        CharacterController selectedCharacter = characterData.GetSelectedCharacter();
        if (character != null && character != selectedCharacter) characterData.ShowCharacter(character);
    }

    void CharacterHoverExitHandler(object sender, int characterId) {
        characterData.ShowSelectedCharacter();
    }

    /// <summary>
    /// Handler when click on a skill
    /// </summary>
    /// <param name="skillIndex"></param>
    public void ClickSkill(int skillIndex) {
        CharacterController character = characterData.GetSelectedCharacter();
        Skill skill = character.GetSkill(skillIndex);
        if (skillSelected != skill) {
            skillSelected = skill;
            characterData.SelectSkill(skillIndex);
        } else {
            skillSelected = null;
            characterData.SelectSkill();
        }
    }

    public bool IsTurn() {
        return isTurn;
    }

    bool IsSkillSelected() {
        return skillSelected != null;
    }
}
