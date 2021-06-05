using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CharacterController : NetworkBehaviour {
    [SerializeField] CharacterMove characterMove;

    [SyncVar] public Vector2Int position;
    [SyncVar] int player;

    public int animationTime = 1;

 /*
    [SyncVar] public int id = -1;
    public void SetId(int id) {
        if (this.id < 0) this.id = id;
    }
*/

    public void SetPosition(Vector2Int position) {
        this.position = position;
    }

    public void SetPlayer(int player) {
        this.player = player;
    }

    [ClientRpc]
    public void LocateCharacter() {
        Debug.Log(this);
        Debug.Log(position);
        Debug.Log(BoardManager.instance.GetTile(position).transform.position);
        transform.position = BoardManager.instance.GetTile(position).transform.position;
        characterMove.StartCharacterMove(player, animationTime);
    }

    [ClientRpc]
    public void MoveAnimation(Vector2Int destiny) {
        //GetComponent<Animator>().SetBool("Walk", true);
        Vector2Int[] path = BoardUtils.GetPath(position, destiny);
        characterMove.StartMove(path);
    }

    [Server]
    public void Move(Vector2Int position) {
        SetPosition(position);
    }
}
