using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PortraitManager : NetworkBehaviour {
    public static PortraitManager instance = null;

    [SerializeField] List<PortraitController> portraits = new List<PortraitController>();
    public int selected = 0;
    // Portraits index which separates characters who haven't skipped their turn from those who have
    public int skipIndex = Const.CHAR_NUMBER - 1;

    void Awake() {
        instance = this;
    }

    void Start() {
        GameManager.instance.OnStartGame += StartGameHandler;
        GameManager.instance.OnStartRound += StartRoundHandler;
        GameManager.instance.OnEndTurn += EndTurnHandler;
        GameManager.instance.OnSkipTurn += SkipTurnHandler;
        CharacterManager.instance.OnChangeHealth += ChangeHealthHandler;
    }

    [ClientRpc]
    void StartGameHandler(object sender, EventArgs e) {
        List<int> charactersPriority = GameManager.instance.charactersPriority;
        for (int i = 0; i < charactersPriority.Count; i++) {
            portraits[i].InitializePortrait(i, charactersPriority[i]);
        }
        Select(selected, Const.PORTRAIT_ENDTURN_TIME);
    }

    [ClientRpc]
    void StartRoundHandler(object sender, EventArgs e) {
        Deselect(selected, Const.PORTRAIT_STARTROUND_TIME);
        skipIndex = portraits.Count - 1;
        selected = 0;
        RestartPortraits(Const.PORTRAIT_STARTROUND_TIME);
    }

    [ClientRpc]
    void SkipTurnHandler(object sender, int characterId) {
        if (skipIndex >= 0) {
            Move(selected, skipIndex, Const.PORTRAIT_SKIPTURN_TIME);
            Deselect(skipIndex, Const.PORTRAIT_SKIPTURN_TIME);
            Select(selected, Const.PORTRAIT_SKIPTURN_TIME);
            skipIndex--;
        }
    }

    [ClientRpc]
    void EndTurnHandler(object sender, EventArgs e) {
        Deselect(selected, Const.PORTRAIT_ENDTURN_TIME);
        selected = (selected + 1) % portraits.Count;
        Select(selected, Const.PORTRAIT_ENDTURN_TIME);
    }

    [Client]
    void ChangeHealthHandler(int characterId, int health) {
        if (health <= 0) {
            int portraitId = GetIndex(characterId);
            if (portraitId >= 0) Remove(portraitId, Const.PORTRAIT_DEATH_TIME);
        }
    }

    /// <summary>
    /// Set portraits to original position
    /// </summary>
    /// <param name="time">Animation time</param>
    void RestartPortraits(float time) {
        List<PortraitController> portraitsBackup = new List<PortraitController>(portraits);
        for (int i = 0; i < portraits.Count; i++) {
            portraits[portraitsBackup[i].index] = portraitsBackup[i];
        }
        Refresh(time);
        Select(selected, time);
    }

    /// <summary>
    /// Remove a portrait
    /// </summary>
    /// <param name="index">Portrait index to be removed</param>
    /// <param name="time">Animation time</param>
    void Remove(int index, float time) {
        portraits[index].Resize(portraits[index].transform.localScale.x, 0f, time);
        foreach (PortraitController portrait in portraits) if (portrait.index > portraits[index].index) portrait.index--;
        portraits.RemoveAt(index);
        if (selected > index) selected--;
        if (skipIndex >= index) skipIndex--;
        Refresh(time);
    }

    /// <summary>
    /// Logic and animations for move a portrait to another position (others will be pushed)
    /// </summary>
    /// <param name="index">Portrait index to be moved</param>
    /// <param name="position">Position to move</param>
    /// <param name="time">Animation time</param>
    void Move(int index, int position, float time) {
        PortraitController portrait = portraits[index];
        portrait.Move(portrait.transform.localPosition.x, GetX(position), time);
        for (int i = index + 1; i <= position; i++) {
            portraits[i - 1] = portraits[i];
        }
        portraits[position] = portrait;
    }

    /// <summary>
    /// Logic and animations for selecting a portrait
    /// </summary>
    /// <param name="index">Portrait index to be selected</param>
    /// <param name="time">Animation time</param>
    void Select(int index, float time) {
        portraits[index].Resize(portraits[index].transform.localScale.x, Const.PORTRAIT_SELECT_SIZE, time);
        Refresh(time);
    }

    /// <summary>
    /// Logic and animations for unselecting a portrait
    /// </summary>
    /// <param name="index">Portrait index to be selected</param>
    /// <param name="time">Animation time</param>
    void Deselect(int index, float time) {
        portraits[index].Resize(portraits[index].transform.localScale.x, Const.PORTRAIT_UNSELECT_SIZE, time);
        Refresh(time);
    }

    /// <summary>
    /// Update all portraits positions
    /// </summary>
    /// <param name="time">Animation time</param>
    void Refresh(float time) {
        for (int i = 0; i < portraits.Count; i++) portraits[i].Move(portraits[i].transform.localPosition.x, GetX(i), time);
    }

    /// <summary>
    /// Get in game X position for a given index
    /// </summary>
    float GetX(int index) {
        float offset = 0;
        if (index < selected) offset = -Const.PORTRAIT_RESIZE_OFFSET;
        if (index > selected) offset = Const.PORTRAIT_RESIZE_OFFSET;
        offset += (Const.PORTRAIT_DISTANCE / 2) * (Const.CHAR_NUMBER - portraits.Count);
        return index * Const.PORTRAIT_DISTANCE + offset;
    }

    /// <summary>
    /// Get portrait index for a given character id
    /// </summary>
    int GetIndex(int characterId) {
        int index = -1;
        for (int i = 0; i < portraits.Count; i++) {
            if (portraits[i].GetCharacterId() == characterId) return i;
        }
        return index;
    }
}
