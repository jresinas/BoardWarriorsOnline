using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkill : MonoBehaviour {
    [SerializeField] Animator anim;
    [SerializeField] CharacterController self;
    List<CharacterController> targetCharacters;
    Skill skill;
    bool success;

    //public void StartPlay(Skill skill, CharacterController targetCharacter, bool success) {
    public void StartPlay(Skill skill, int[] targetIds, bool success) {
        this.targetCharacters = CharacterManager.instance.Get(targetIds);
        this.skill = skill;
        this.success = success;
        List<int> characterIds = new List<int>(targetIds);
        characterIds.Add(self.GetId());
        SkillManager.instance.StartAnimation(characterIds);
        StartAnimation();
    }

    void StartAnimation() {
        if (targetCharacters.Count > 0) transform.LookAt(targetCharacters[0].transform);
        Waiting();
        foreach (CharacterController targetCharacter in targetCharacters) {
            targetCharacter.transform.LookAt(transform);
            targetCharacter.Waiting();
        }
        StartCoroutine(WaitDiceRoll());
    }

    IEnumerator WaitDiceRoll() {
        yield return new WaitForSeconds(1);
        anim.SetTrigger(skill.GetAnimation());
    }

    // Attacking character 
    void Impact() {
        foreach (CharacterController targetCharacter in targetCharacters) {
            if (targetCharacter != self) {
                if (success) targetCharacter.ReceiveDamage();
                else targetCharacter.DodgeAttack();
            }
        }

        targetCharacters = null;
        skill = null;
    }

    // During attack, waiting dice roll
    public void Waiting() {
        anim.SetTrigger("Waiting");
    }

    // Target character has been damaged
    public void ReceiveDamage() {
        anim.SetTrigger("Damage");
    }

    public void DodgeAttack() {
        anim.SetTrigger("Dodge");
    }

    public void EndAnimation() {
        anim.SetTrigger("EndAnimation");
    }
}
