using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PortraitManager : NetworkBehaviour {
    public static PortraitManager instance = null;
    [SerializeField] List<PortraitController> portraits = new List<PortraitController>();
    public int selected = 0;
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
    private void StartGameHandler(object sender, EventArgs e) {
        List<int> charactersPriority = GameManager.instance.charactersPriority;
        for (int i = 0; i < charactersPriority.Count; i++) {
            portraits[i].SetCharacterId(charactersPriority[i]);
        }

        Select(selected, Const.PORTRAIT_ENDTURN_TIME);
    }

    [Client]
    private void ChangeHealthHandler(int characterId, int health) {
        if (health <= 0) Death(characterId);
    }

    [ClientRpc]
    private void SkipTurnHandler(object sender, int characterId) {
        SkipTurn();
    }

    [ClientRpc]
    private void EndTurnHandler(object sender, EventArgs e) {
        EndTurn();
    }

    [ClientRpc]
    private void StartRoundHandler(object sender, EventArgs e) {
        skipIndex = portraits.Count-1;
        selected = 0;
    }

    void EndTurn() {
        Deselect(selected, Const.PORTRAIT_ENDTURN_TIME);
        selected = (selected + 1) % portraits.Count;
        Select(selected, Const.PORTRAIT_ENDTURN_TIME);
    }

    void SkipTurn() {
        if (skipIndex >= 0) {
            Move(selected, skipIndex, Const.PORTRAIT_SKIPTURN_TIME);
            Deselect(skipIndex, Const.PORTRAIT_SKIPTURN_TIME);
            Select(selected, Const.PORTRAIT_SKIPTURN_TIME);
            skipIndex--;
        }
    }

    void Death(int characterId) {
        int portraitId = GetIndex(characterId);
        if (portraitId >= 0) Remove(portraitId, Const.PORTRAIT_DEATH_TIME);
    }

    void Remove(int index, float time) {
        portraits[index].Resize(portraits[index].transform.localScale.x, 0f, time);
        portraits.RemoveAt(index);
        if (selected > index) selected--;
        if (skipIndex >= index) skipIndex--; 
        Refresh(time);
    }

    void Move(int portraitId, int position, float time) {
        PortraitController portrait = portraits[portraitId];
        portrait.Move(portrait.transform.localPosition.x, GetX(position), time);
        for (int i = portraitId + 1; i <= position; i++) {
            portraits[i - 1] = portraits[i];
        }
        portraits[position] = portrait;
    }

    void Select(int index, float time) {
        portraits[index].Resize(portraits[index].transform.localScale.x, Const.PORTRAIT_SELECT_RESIZE, time);
        Refresh(time);
    }

    void Deselect(int index, float time) {
        portraits[index].Resize(portraits[index].transform.localScale.x, 1f, time);
        Refresh(time);
    }

    void Refresh(float time) {
        for (int i = 0; i < portraits.Count; i++) portraits[i].Move(portraits[i].transform.localPosition.x, GetX(i), time);
    }

    float GetX(int position) {
        float offset = 0;
        if (position < selected) offset = -Const.PORTRAIT_RESIZE_OFFSET;
        if (position > selected) offset = Const.PORTRAIT_RESIZE_OFFSET;
        offset += (Const.PORTRAIT_DISTANCE / 2) * (Const.CHAR_NUMBER - portraits.Count);
        return position * Const.PORTRAIT_DISTANCE + offset;
    }

    int GetIndex(int characterId) {
        int index = -1;
        for (int i = 0; i < portraits.Count; i++) {
            if (portraits[i].GetCharacterId() == characterId) return i;
        }
        return index;
    }
}
