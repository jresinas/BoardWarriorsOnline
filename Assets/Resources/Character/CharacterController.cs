using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;

public class CharacterController : NetworkBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] CharacterMove characterMove;

    [SyncVar] public int id;
    [SyncVar] public Vector2Int position;
    [SyncVar] int player;

    int movement = 3;

    [SerializeField] Skill[] skills = new Skill[Const.SKILL_NUMBER];

    public int animationTime = 1;

 /*
    [SyncVar] public int id = -1;
    public void SetId(int id) {
        if (this.id < 0) this.id = id;
    }
*/

    public void SetId(int id) {
        this.id = id;
    }

    public void SetPosition(Vector2Int position) {
        this.position = position;
    }

    public Vector2Int GetPosition() {
        return position;
    }

    public int GetMovement() {
        return movement;
    }

    public void SetPlayer(int player) {
        this.player = player;
    }

    public Skill GetSkill(int index) {
        if (index < Const.SKILL_NUMBER) return skills[index];
        else return null;
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

    public void OnPointerEnter(PointerEventData eventData) {
        CharacterManager.instance.EnterHover(id);
    }

    public void OnPointerExit(PointerEventData eventData) {
        CharacterManager.instance.ExitHover(id);
    }
}
