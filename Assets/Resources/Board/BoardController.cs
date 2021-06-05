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

    /*
    public TileController[] GetPath(Vector2Int origin, Vector2Int destiny) {
        TileController[] path = new TileController[BoardUtils.Distance(origin, destiny)];
        Vector2Int position = origin;
        int i = 0;
        while (position != destiny) {
            if (position.y < destiny.y) position.y++;
            else if (position.y > destiny.y) position.y--;
            else if (position.x < destiny.x) position.x++;
            else if (position.x > destiny.x) position.x--;
            path[i] = GetTile(position);
            i++;
        }

        return path;
    }
    */
    
}
