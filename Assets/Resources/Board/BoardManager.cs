using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BoardManager : NetworkBehaviour {
    public static BoardManager instance = null;

    // Event triggered on client when player click on a Tile.
    // * Vector2Int: tile position
    public event EventHandler<Vector2Int> OnClickTile;

    [SerializeField] BoardController board;

    void Awake() {
        instance = this;
    }

    void Start() {
        GUIManager.instance.OnSelectSkill += SelectSkillHandler;
        GUIManager.instance.OnUnselectSkill += UnselectSkillHandler;
        GameManager.instance.OnEndOfTurn += EndOfTurnHandler;
        GameManager.instance.OnMove += MoveHandler;

        board.LoadTiles();
    }

    // Get TileController of specific position
    public TileController GetTile(Vector2Int position) {
        return board.GetTile(position);
    }

    public void ClickTile(Vector2Int position) {
        if (OnClickTile != null) OnClickTile(this, position);
    }

    #region EventHandlers
    [ClientRpc]
    void EndOfTurnHandler(object source, EventArgs args) {
        board.HideMarks();
    }

    [ClientRpc]
    void MoveHandler(int characterId, Vector2Int position) {
        board.HideMarks();
    }

    void SelectSkillHandler(int characterId, int skillId) {
        if (characterId >= 0 && skillId >= 0) {
            board.HideMarks();
            ShowTargetMarks(characterId, skillId);
        }
    }

    void UnselectSkillHandler(int characterId) {
        if (characterId >= 0) {
            board.HideMarks();
            ShowMoveMarks(characterId);
        }
    }
    #endregion

    void ShowMoveMarks(int characterId) {
        CharacterController character = CharacterManager.instance.Get(characterId);
        Vector2Int origin = character.GetPosition();
        int range = character.GetMovement();
        board.ShowMoveMarks(origin, range);
    }

    void ShowTargetMarks(int characterId, int skillId) {
        CharacterController character = CharacterManager.instance.Get(characterId);
        Skill skill = character.GetSkill(skillId);
        board.ShowSkillMarks(skill);
    }   
}