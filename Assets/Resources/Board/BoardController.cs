using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour {
    TileController[][] tiles;

    public void LoadTiles() {
        Debug.Log("load tiles");
        tiles = new TileController[Const.BOARD_ROWS][];
        for (int i = 0; i < Const.BOARD_ROWS; i++) {
            tiles[i] = new TileController[Const.BOARD_COLS];
            for (int j = 0; j < Const.BOARD_COLS; j++) {
                tiles[i][j] = transform.GetChild(i * Const.BOARD_COLS + j).GetComponent<TileController>();
                tiles[i][j].SetPosition(new Vector2Int(j, i));
            }
        }
    }

    public TileController GetTile(Vector2Int tile) {
        return tiles[tile.y][tile.x];
    }

    public void HideMarks() {
        for (int i = 0; i < Const.BOARD_ROWS; i++) {
            for (int j = 0; j < Const.BOARD_COLS; j++) {
                tiles[i][j].HideMark();
            }
        }
    }

    public void ShowMoveMarks(Vector2Int position, int range) {
        for (int i = 0; i < Const.BOARD_ROWS; i++) {
            for (int j = 0; j < Const.BOARD_COLS; j++) {
                Vector2Int destinyPosition = new Vector2Int(j, i);
                if (BoardUtils.Reach(position, destinyPosition, range) && CharacterManager.instance.GetId(destinyPosition) < 0) {
                        tiles[i][j].ShowMoveMark();
                }
            }
        }
    }

    public void ShowSkillMarks(Skill skill) {
        for (int i = 0; i < Const.BOARD_ROWS; i++) {
            for (int j = 0; j < Const.BOARD_COLS; j++) {
                Vector2Int targetPosition = new Vector2Int(j, i);
                if (skill.AllowTarget(targetPosition)) tiles[i][j].ShowSkillMark();
            }
        }
    }

}
