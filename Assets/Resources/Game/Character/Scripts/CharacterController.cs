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

    [SerializeField] string name;
    [SerializeField] string surname;
    [SerializeField] int maxHealth;
    [SerializeField] int movement;
    [SerializeField] int armor;
    [SerializeField] Skill[] skills = new Skill[Const.SKILL_NUMBER];
    public Sprite portrait;

    [SyncVar] public int health;
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

    /// <summary>
    /// Place character on clients on its current tile
    /// </summary>
    [ClientRpc]
    public void PlaceCharacter() {
        transform.position = BoardManager.instance.GetTile(position).transform.position + Vector3.up * Const.CHAR_OFFSET;
        characterMove.Idle();
    }

    /// <summary>
    /// Move character on server side
    /// </summary>
    /// <param name="position">Destiny position</param>
    [Server]
    public void Move(Vector2Int position) {
        SetPosition(position);
    }

    /// <summary>
    /// Starts character move animation on client side
    /// </summary>
    /// <param name="origin">Origin position (needed because Mirror sync reasons)</param>
    /// <param name="destiny">Destiny position</param>
    [ClientRpc]
    public void MoveAnimation(Vector2Int origin, Vector2Int destiny) {
        List<Vector2Int> path = BoardUtils.GetPath(origin, destiny, player);
        characterMove.StartMove(path);
    }

    /// <summary>
    /// Use skill on server side
    /// </summary>
    /// <param name="skillIndex">Character skill index</param>
    /// <param name="destiny">Skill target position</param>
    /// <returns>Results of skill execution</returns>
    [Server]
    public SkillResult UseSkill(int skillIndex, Vector2Int destiny) {
        Skill skill = GetSkill(skillIndex);
        return skill.Play(destiny);
    }

    /// <summary>
    /// Starts skil use animation on client side
    /// </summary>
    /// <param name="skillIndex">Character skill index to use</param>
    /// <param name="targetIds">Array of character ids directly affected by skill animation</param>
    /// <param name="success">True if skill was executed successfully and will impact on targets</param>
    /// <param name="observerIds">Array of character ids indirectly affected by skill</param>
    /// <param name="data">Additional data provided from skill execution (serialized in JSON)</param>
    [ClientRpc]
    public void UseSkillAnimation(int skillIndex, int[] targetIds, bool success, int[] observerIds, string data) {
        Skill skill = GetSkill(skillIndex);
        characterSkill.StartSkill(skill, targetIds, success, observerIds, data);
    }

    #region Animations
    /// <summary>
    /// Starts skill waiting animation (before executing)
    /// </summary>
    public void Waiting() {
        characterSkill.Waiting();
    }

    /// <summary>
    /// Starts skill impact animation
    /// </summary>
    /// <param name="type">Type of impact: Damage (default) or Shove</param>
    public void ReceiveImpact(string type = null) {
        characterSkill.ReceiveImpact(this, type);
    }

    public void ReceiveDamage() {
        CharacterManager.instance.RefreshHealth(id, health);
        characterSkill.Damage();
    }

    public void ReceiveShove(string data) {
        CharacterManager.instance.RefreshHealth(id, health);
        characterShove.StartShove(data);
    }

    public void Death() {
        CharacterManager.instance.RefreshHealth(id, health);
        characterSkill.Death();
    }

    public void Dodge() {
        characterSkill.Dodge();
    }

    public void EndAnimation() {
        characterSkill.EndAnimation();
    }

    /// <summary>
    /// Starts fade out effect
    /// </summary>
    public void DeathFadeOut() {
        characterSkill.DeathFadeOut();
    }
    #endregion

    #region PointerEvent
    public void OnPointerEnter(PointerEventData eventData) {
        CharacterManager.instance.EnterHover(id);
    }

    public void OnPointerExit(PointerEventData eventData) {
        CharacterManager.instance.ExitHover(id);
    }
    #endregion
}
