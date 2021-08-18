using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;

public class CharacterController : NetworkBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] CharacterMove characterMove;
    [SerializeField] CharacterSkill characterSkill;
    [SerializeField] CharacterShove characterShove;

    [SyncVar] public int id;
    [SyncVar] public Vector2Int position;
    [SyncVar] int player;

    [SerializeField] float animationTime;
    [SerializeField] string name;
    [SerializeField] string surname;
    [SerializeField] int movement;
    [SerializeField] int armor;
    [SerializeField] Skill[] skills = new Skill[Const.SKILL_NUMBER];
    [SerializeField] int maxHealth;
    public Sprite portrait;

    [SyncVar] public int health;
    [SyncVar] int energy;

    public Transform leftHand;
    public Transform rightHand;
    public Transform body;

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

    public string GetName() {
        return name;
    }

    public string GetSurname() {
        return surname;
    }

    public void ChangeHealth(int value) {
        health = Mathf.Clamp(health+value, 0, maxHealth);
        if (health <= 0) CharacterManager.instance.Death(id);
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
        List<Vector2Int> path = BoardUtils.GetPath(origin, destiny, player);
        characterMove.StartMove(path);
    }

    /*
    [Server]
    public int Shove(Vector2Int origin) {
        int collisionId;
        Vector2Int destiny = BoardUtils.GetShoveDestiny(position, origin);
        if (destiny.x < 0 || destiny.y < 0 || destiny.x >= Const.BOARD_COLS || destiny.y >= Const.BOARD_ROWS) {
            collisionId = id;
        } else {
            collisionId = CharacterManager.instance.GetId(destiny);
            if (collisionId < 0) {
                SetPosition(destiny);
            }
        }
       
        PrepareShoveAnimation(origin);
        return collisionId;
    }
    */

    [Server]
    public SkillResult UseSkill(int skillIndex, Vector2Int destiny) {
        Skill skill = GetSkill(skillIndex);
        //CharacterController target = CharacterManager.instance.Get(targetId);
        return skill.Play(destiny);
    }

    [ClientRpc]
    public void UseSkillAnimation(int skillIndex, int[] targetIds, bool success, int[] observerIds, string data) {
        Skill skill = GetSkill(skillIndex);
        characterSkill.StartPlay(skill, targetIds, success, observerIds, data);
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

    /*
    public void ReceiveImpact(bool success) {
        Debug.Log("CharacterController - ReceiveImpact");
        RefreshHealth();
        characterSkill.ReceiveImpact(success);
    }
    

    [ClientRpc]
    public void PrepareShoveAnimation(Vector2Int origin) {
        //characterSkill.shove = true;
        //characterSkill.shoveOrigin = origin;
        characterShove.origin = origin;
    }


    [Client]
    public void ShoveAnimation(Vector2Int origin) {
        //Vector2Int destiny = BoardUtils.GetShoveDestiny(position, origin);
        characterShove.StartShove(origin, position);
        //characterSkill.shove = false;
    }
    */

    public void ReceiveImpact(string type = null) {
        characterSkill.ReceiveImpact(this, type);
    }

    public void ReceiveDamage() {
        RefreshHealth();
        characterSkill.Damage();
    }

    public void ReceiveShove(string data) {
        RefreshHealth();
        characterShove.StartShove(data);
    }


    public void Death() {
        RefreshHealth();
        characterSkill.Death();
    }

    public void Dodge() {
        characterSkill.Dodge();
    }

    public void EndAnimation() {
        characterSkill.EndAnimation();
    }

    public void DeathFadeOut() {
        characterSkill.DeathFadeOut();
    }
    #endregion
}
