using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;

public class CharacterController : NetworkBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] CharacterMove characterMove;
    [SerializeField] CharacterSkill characterSkill;

    [SyncVar] public int id;
    [SyncVar] public Vector2Int position;
    [SyncVar] int player;

    [SerializeField] int movement;
    [SerializeField] Skill[] skills = new Skill[Const.SKILL_NUMBER];
    [SerializeField] float animationTime;

 /*
    [SyncVar] public int id = -1;
    public void SetId(int id) {
        if (this.id < 0) this.id = id;
    }
*/

    public void SetId(int id) {
        this.id = id;
    }

    public int GetId() {
        return id;
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

    public int GetPlayer() {
        return player;
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

    [Server]
    public void Move(Vector2Int position) {
        SetPosition(position);
    }

    [ClientRpc]
    public void MoveAnimation(Vector2Int destiny) {
        //GetComponent<Animator>().SetBool("Walk", true);
        Vector2Int[] path = BoardUtils.GetPath(position, destiny);
        characterMove.StartMove(path);
    }

    [Server]
    public bool UseSkill(int skillIndex, int targetId) {
        Skill skill = GetSkill(skillIndex);
        CharacterController target = CharacterManager.instance.Get(targetId);
        return skill.Play(target);
    }

    [ClientRpc]
    public void UseSkillAnimation(int skillIndex, int targetId, bool success) {
        Skill skill = GetSkill(skillIndex);
        CharacterController target = CharacterManager.instance.Get(targetId);
        characterSkill.StartPlay(skill, target, success);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        CharacterManager.instance.EnterHover(id);
    }

    public void OnPointerExit(PointerEventData eventData) {
        CharacterManager.instance.ExitHover(id);
    }

    #region AnimationCallbacks
    public void Waiting() {
        characterSkill.Waiting();
    }

    public void ReceiveDamage() {
        characterSkill.ReceiveDamage();
    }

    public void DodgeAttack() {
        characterSkill.DodgeAttack();
    }
    #endregion
}
