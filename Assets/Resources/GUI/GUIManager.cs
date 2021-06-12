using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GUIManager : NetworkBehaviour {
    public static GUIManager instance = null;

    public event Action<int, int> OnSelectSkill;
    public event Action<int> OnUnselectSkill;
    public event EventHandler OnRequestSkip;
    public event EventHandler OnRequestEndTurn;

    [SerializeField] CharacterDataController characterData;
    [SerializeField] ButtonsController buttons;
    [SerializeField] DicesController dices;
    int skillSelected = -1;

    void Awake() {
        instance = this;
    }

    void Start() {
        GameManager.instance.OnStartTurn += StartTurnHandler;
        CharacterManager.instance.OnCharacterHoverEnter += CharacterHoverEnterHandler;
        CharacterManager.instance.OnCharacterHoverExit += CharacterHoverExitHandler;
    }

    public int GetSkillSelected() {
        return skillSelected;
    }

    [ClientRpc]
    void StartTurnHandler(object source, int characterId) {
        characterData.SelectCharacter(characterId);
        SelectSkill();
    }

    void CharacterHoverEnterHandler(object sender, int characterId) {
        ShowCharacter(characterId);
    }

    void CharacterHoverExitHandler(object sender, int characterId) {
        ShowCharacter();
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
        if (ClientManager.instance.IsAvailableSkill()) {
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
        if (ClientManager.instance.IsAvailableSkill()) {
            if (skillIndex >= 0 && OnSelectSkill != null) OnSelectSkill(characterData.GetSelectedCharacter(), skillIndex);
            else if (OnUnselectSkill != null) OnUnselectSkill(characterData.GetSelectedCharacter());
        }
    }

    public bool IsSkillSelected() {
        return skillSelected >= 0;
    }

    /// <summary>
    /// Show character info in GUI
    /// </summary>
    /// <param name="characterId">Id of character to show. If null, show selected character</param>
    public void ShowCharacter(int characterId = -1) {
        characterData.ShowCharacter(characterId);
    }

    public void EnableButtons(bool skip = false, bool endTurn = false) {
        if (skip || (!skip && !endTurn)) buttons.EnableSkip();
        if (endTurn || (!skip && !endTurn)) buttons.EnableEndTurn();
    }

    public void DisableButtons(bool skip = false, bool endTurn = false) {
        if (skip || (!skip && !endTurn)) buttons.DisableSkip();
        if (endTurn || (!skip && !endTurn)) buttons.DisableEndTurn();
    }

    public int RollDices(int dicesNumber, int minRequired) {
        return dices.Roll(dicesNumber, minRequired);
    }

    public void ResetDices() {
        dices.Reset();
    }
}
