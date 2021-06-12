using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkill : MonoBehaviour {
    [SerializeField] Animator anim;
    CharacterController targetCharacter;
    Skill skill;
    bool success;

    public void StartPlay(Skill skill, CharacterController character, bool success) {
        targetCharacter = character;
        this.skill = skill;
        this.success = success;
        StartAnimation();
    }

    void StartAnimation() {
        transform.LookAt(targetCharacter.transform);
        targetCharacter.transform.LookAt(transform);
        Waiting();
        targetCharacter.Waiting();
        StartCoroutine(WaitDiceRoll());
    }

    IEnumerator WaitDiceRoll() {
        yield return new WaitForSeconds(1);
        anim.SetTrigger(skill.GetAnimation());
    }

    // Attacking character 
    void Impact() {
        //skill.Play(targetCharacter);
        if (success) targetCharacter.ReceiveDamage();
        else targetCharacter.DodgeAttack();

        targetCharacter = null;
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
}
