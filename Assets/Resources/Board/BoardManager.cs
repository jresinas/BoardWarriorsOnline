using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BoardManager : NetworkBehaviour {
    public static BoardManager instance = null;

    public event EventHandler<Vector2Int> OnClickTile;

    [SerializeField] BoardController board;

    void Awake() {
        instance = this;
    }

    void Start() {
        board.LoadTiles();
    }

    public void ClickTile(Vector2Int position) {
        if (OnClickTile != null) OnClickTile(this, position);
    }

    public TileController GetTile(Vector2Int position) {
        return board.GetTile(position);
    }

    /*
        public BoardController GetBoard() {
            return board;
        }

        public TileController[] GetPath(Vector2Int origin, Vector2Int destiny) {
            return board.GetPath(origin, destiny);
        }
    */
}