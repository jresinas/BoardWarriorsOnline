using System;
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

    [SerializeField] float animationTime;
    [SerializeField] int movement;
    [SerializeField] int armor;
    [SerializeField] Skill[] skills = new Skill[Const.SKILL_NUMBER];
    [SerializeField] int maxHealth;

    [SyncVar] int health;
    [SyncVar] int energy;

    void Awake() {
        health = maxHealth;
        energy = 0;
    }

    #region Get&Set
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

    public int GetArmor() {
        return armor;
    }

    public int GetHealth() {
        return health;
    }

    public int GetEnergy() {
        return energy;
    }

    public void ChangeHealth(int value) {
        health = Mathf.Clamp(health+value, 0, maxHealth);
    }

    public void ChangeEnergy(int value) {
        energy = Mathf.Clamp(energy + value, 0, Const.MAX_ENERGY);
        CharacterManager.instance.RefreshEnergy(id, energy);
    }

    public void RefreshHealth() {
        CharacterManager.instance.RefreshHealth(id, health);
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
    #endregion


    [ClientRpc]
    public void LocateCharacter() {
        transform.position = BoardManager.instance.GetTile(position).transform.position + Vector3.up * Const.CHAR_OFFSET;
        characterMove.StartCharacterMove(player, animationTime);
    }

    [Server]
    public void Move(Vector2Int position) {
        SetPosition(position);
    }

    [ClientRpc]
    public void MoveAnimation(Vector2Int origin, Vector2Int destiny) {
        //GetComponent<Animator>().SetBool("Walk", true);
        Vector2Int[] path = BoardUtils.GetPath(origin, destiny);
        characterMove.StartMove(path);
    }

    [Server]
    public SkillResult UseSkill(int skillIndex, Vector2Int destiny) {
        Skill skill = GetSkill(skillIndex);
        //CharacterController target = CharacterManager.instance.Get(targetId);
        return skill.Play(destiny);
    }

    [ClientRpc]
    public void UseSkillAnimation(int skillIndex, int[] targetIds, bool success) {
        Skill skill = GetSkill(skillIndex);
        characterSkill.StartPlay(skill, targetIds, success);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        CharacterManager.instance.EnterHover(id);
    }

    public void OnPointerExit(PointerEventData eventData) {
        CharacterManager.instance.ExitHover(id);
    }

    #region Animations
    public void Waiting() {
        characterSkill.Waiting();
    }

    public void ReceiveImpact(bool success) {
        characterSkill.ReceiveImpact(success);
    }

    public void EndAnimation() {
        characterSkill.EndAnimation();
    }

    public void DeathFadeOut() {
        characterSkill.DeathFadeOut();
    }
    #endregion
}
