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
    [SerializeField] ButtonsResponseController buttonsResponse;

    // index of current skill selected of the active character
    int skillSelected = -1;
    // id of character under cursor
    int hoverCharacter = -1;
    // id of character in turn
    int turnCharacter = -1; // replace with activeCharacter from GameManager?


    void Awake() {
        instance = this;
        buttons.ShowButtons();
        buttonsResponse.HideButtons();
    }

    void Start() {
        GameManager.instance.OnStartTurn += StartTurnHandler;
        CharacterManager.instance.OnCharacterHoverEnter += CharacterHoverEnterHandler;
        CharacterManager.instance.OnCharacterHoverExit += CharacterHoverExitHandler;
        CharacterManager.instance.OnChangeEnergy += ChangeEnergyHandler;
        GameManager.instance.OnRequestResponseSkill += RequestResponseSkillHandler;
        ClientManager.instance.OnSendResponseSkill += SendResponseSkillHandler;
        GameManager.instance.OnWaitingResponseSkill += WaitingResponseSkillHandler;
    }

    #region GameEvents
    [TargetRpc]
    void WaitingResponseSkillHandler(NetworkConnection userConnection, bool status) {
        if (status) {
            buttons.HideButtons();
            Debug.Log("Waiting for opponent response");
        } else buttons.ShowButtons();
    }

    [TargetRpc]
    void RequestResponseSkillHandler(NetworkConnection userConnection, int casterId, int skillIndex, int targetId) {
        CharacterController caster = CharacterManager.instance.Get(casterId);
        Skill skill = caster.GetSkill(skillIndex);
        CharacterController target = CharacterManager.instance.Get(targetId);
        Debug.Log(caster.name + " has used " + skill.GetTitle() + " against " + target + ". Do you want to response?");
        characterData.SetCharacter(targetId);
        buttons.HideButtons();
        buttonsResponse.ShowButtons();
    }

    void SendResponseSkillHandler(int skillIndex) {
        characterData.SetCharacter(turnCharacter);
        buttons.ShowButtons();
        buttonsResponse.HideButtons();
    }

    [ClientRpc]
    void StartTurnHandler(object source, int characterId) {
        turnCharacter = characterId;
        characterData.SetCharacter(characterId);
        // Unselect all skills
        SelectSkill();
    }
    #endregion

    #region CharacterData
    public int GetSkillSelected() {
        return skillSelected;
    }

    void CharacterHoverEnterHandler(object sender, int characterId) {
        hoverCharacter = characterId;
        ShowCharacter(characterId);
    }

    void CharacterHoverExitHandler(object sender, int characterId) {
        hoverCharacter = -1;
        ShowCharacter();
    }

    void ChangeEnergyHandler(int characterId, int energy) {
        ShowCharacter(hoverCharacter);
    }

    /// <summary>
    /// Handler when click on a skill
    /// </summary>
    /// <param name="skillIndex"></param>
    public void ClickSkill(int skillIndex) {
        int selectedCharacterId = characterData.GetSelectedCharacter();
        if (selectedCharacterId >= 0) {
            Skill skill = CharacterManager.instance.Get(selectedCharacterId).GetSkill(skillIndex);
            if (skill.IsVisible()) {
                if (skillSelected != skillIndex) SelectSkill(skillIndex);
                else SelectSkill();
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
    #endregion

    #region Buttons
    public void ClickSkipButton() {
        if (OnRequestSkip != null) OnRequestSkip(this, EventArgs.Empty);
    }

    public void ClickEndTurnButton() {
        if (OnRequestEndTurn != null) OnRequestEndTurn(this, EventArgs.Empty);
    }

    public void ClickSendResponseButton() {
        ClientManager.instance.SendResponseSkill(skillSelected);
    }

    public void EnableButtons(bool skip = false, bool endTurn = false) {
        if (skip || (!skip && !endTurn)) buttons.EnableSkip();
        if (endTurn || (!skip && !endTurn)) buttons.EnableEndTurn();
    }

    public void DisableButtons(bool skip = false, bool endTurn = false) {
        if (skip || (!skip && !endTurn)) buttons.DisableSkip();
        if (endTurn || (!skip && !endTurn)) buttons.DisableEndTurn();
    }
    #endregion
}
