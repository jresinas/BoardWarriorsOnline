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
    public event EventHandler OnRequestSkip;
    public event EventHandler OnRequestEndTurn;

    [SerializeField] CharacterDataController characterData;
    [SerializeField] ButtonsController buttons;

    bool isTurn = false;
    bool isAvailableMove = false;
    bool isAvailableSkill = false;
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
        StartTurn();
    }

    [ClientRpc]
    void EndTurnHandler(object source, EventArgs args) {
        EndTurn();
    }

    [ClientRpc]
    void StartTurnHandler(object source, int characterId) {
        characterData.SelectCharacter(characterId);
        SelectSkill();
    }

    [ClientRpc]
    void EndActionsHandler(object source, EventArgs args) {
        isAvailableMove = false;
        isAvailableSkill = false;
        buttons.DisableSkip();
        characterData.ShowSelectedCharacter();
//        isTurn = false;
//        characterData.ShowSelectedCharacter();
    }

    void ClickTileHandler(object source, Vector2Int destiny) {
        if (IsAvailableSkill() && IsSkillSelected() && OnRequestUseSkill != null) OnRequestUseSkill(this, destiny);
        else if (IsAvailableMove() && OnRequestMove != null) OnRequestMove(this, destiny);
    }

    void CharacterHoverEnterHandler(object sender, int characterId) {
        int selectedCharacterId = characterData.GetSelectedCharacter();
        if (characterId >= 0 && characterId != selectedCharacterId) characterData.ShowCharacter(characterId);
    }

    void CharacterHoverExitHandler(object sender, int characterId) {
        characterData.ShowSelectedCharacter();
    }

    public void ClickSkipButton() {
        if (OnRequestSkip != null) OnRequestSkip(this, EventArgs.Empty);
    }

    public void ClickEndTurnButton() {
        if (OnRequestEndTurn != null) OnRequestEndTurn(this, EventArgs.Empty);
    }

    /// <summary>
    /// Handler when click on a skill
    /// </summary>
    /// <param name="skillIndex"></param>
    public void ClickSkill(int skillIndex) {
        if (IsAvailableSkill()) {
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
        if (IsAvailableSkill()) {
            if (skillIndex >= 0 && OnSelectSkill != null) OnSelectSkill(characterData.GetSelectedCharacter(), skillIndex);
            else if (OnUnselectSkill != null) OnUnselectSkill(characterData.GetSelectedCharacter());
        }
    }


    void StartTurn() {
        isTurn = true;
        isAvailableMove = true;
        isAvailableSkill = true;
        buttons.Enable();
    }

    void EndTurn() {
        isTurn = false;
        isAvailableMove = false;
        isAvailableSkill = false;
        buttons.Disable();
    }

    public bool IsTurn() {
        return isTurn;
    }

    public bool IsAvailableMove() {
        return IsTurn() && isAvailableMove;
    }

    public bool IsAvailableSkill() {
        return IsTurn() && isAvailableSkill;
    }

    bool IsSkillSelected() {
        return skillSelected >= 0;
    }
}
