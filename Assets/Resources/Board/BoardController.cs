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
                if (BoardUtils.Distance(position, new Vector2Int(j, i)) <= range && tiles[i][j].GetCharacter() < 0) {
                        tiles[i][j].ShowMoveMark();
                }
            }
        }
    }

    public void ShowTargetMarks(Vector2Int position, int range, List<int> targetIds, bool targetCharacter) {
        for (int i = 0; i < Const.BOARD_ROWS; i++) {
            for (int j = 0; j < Const.BOARD_COLS; j++) {
                int characterId;
                Vector2Int targetPosition = new Vector2Int(j, i);
                // Distance between current position and target position <= range AND
                // skill isn't targeting a character and target position != current position OR there is a character in target position and it's included in targetable ids
                if (BoardUtils.Distance(position, targetPosition) <= range && ((!targetCharacter && position != targetPosition) || (characterId = tiles[i][j].GetCharacter()) >= 0 && targetIds.Contains(characterId))) {
                    tiles[i][j].ShowTargetMark();
                }
            }
        }
    }

}
