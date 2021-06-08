using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GUIManager : NetworkBehaviour {
    public static GUIManager instance = null;

    public event EventHandler<Vector2Int> OnRequestMove;
    public event EventHandler<Vector2Int> OnRequestUseSkill;
    public event Action<int, int> OnSelectSkill;
    public event Action<int> OnUnselectSkill;

    [SerializeField] CharacterDataController characterData;

    bool isTurn = false;
    int skillSelected = -1;

    void Awake() {
        instance = this;
    }

    void Start() {
        GameManager.instance.OnStartTurn += TargetStartTurnHandler;
        GameManager.instance.OnStartTurn += StartTurnHandler;
        GameManager.instance.OnEndTurn += EndTurnHandler;
        GameManager.instance.OnEndActions += EndActionsHandler;
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
        characterData.SelectCharacter(characterId);
        SelectSkill();
    }

    [ClientRpc]
    void EndActionsHandler(object source, EventArgs args) {
        isTurn = false;
        characterData.ShowSelectedCharacter();
    }

    void ClickTileHandler(object source, Vector2Int destiny) {
        if (IsTurn()) {
            if (IsSkillSelected() && OnRequestUseSkill != null) OnRequestUseSkill(this, destiny);
            else if (OnRequestMove != null) OnRequestMove(this, destiny);
        }
    }

    void CharacterHoverEnterHandler(object sender, int characterId) {
        int selectedCharacterId = characterData.GetSelectedCharacter();
        if (characterId >= 0 && characterId != selectedCharacterId) characterData.ShowCharacter(characterId);
    }

    void CharacterHoverExitHandler(object sender, int characterId) {
        characterData.ShowSelectedCharacter();
    }

    /// <summary>
    /// Handler when click on a skill
    /// </summary>
    /// <param name="skillIndex"></param>
    public void ClickSkill(int skillIndex) {
        if (IsTurn()) {
            if (skillSelected != skillIndex) {
                SelectSkill(skillIndex);
            } else {
                SelectSkill();
            }
        }
    }

    void SelectSkill(int skillIndex = -1) {
        skillSelected = skillIndex;
        characterData.SelectSkill(skillIndex);
        if (IsTurn()) {
            if (skillIndex >= 0 && OnSelectSkill != null) OnSelectSkill(characterData.GetSelectedCharacter(), skillIndex);
            else if (OnUnselectSkill != null) OnUnselectSkill(characterData.GetSelectedCharacter());
        }
    }



    public bool IsTurn() {
        return isTurn;
    }

    bool IsSkillSelected() {
        return skillSelected >= 0;
    }
}
