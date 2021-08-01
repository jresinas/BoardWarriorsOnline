using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkill : MonoBehaviour {
    [SerializeField] Animator anim;
    [SerializeField] CharacterController self;
    List<CharacterController> targetCharacters;
    Skill skill;
    bool success;

    public List<CharacterController> GetTargets() {
        return targetCharacters;
    }

    public bool GetSuccess() {
        return success;
    }

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
            if (targetCharacter != self) targetCharacter.ReceiveImpact(success);
        }
    }

    void Effect(int number) {
        skill.AnimationEffect(number);
    }

    // During attack, waiting dice roll
    public void Waiting() {
        anim.SetTrigger("Waiting");
    }

    public void ReceiveImpact(bool success) {
        Debug.Log("CharacterSkill - ReceiveImpact "+success);
        if (success) {
            if (self.GetHealth() > 0) anim.SetTrigger("Damage");
            else anim.SetBool("Death", true);
        } else anim.SetTrigger("Dodge");
    }

    public void EndAnimation() {
        if (!anim.GetBool("Death")) anim.SetTrigger("EndAnimation");
        targetCharacters = null;
        skill = null;
    }

    public void DeathFadeOut() {
        Renderer[] renders = GetComponentsInChildren<SkinnedMeshRenderer>();
        //Renderer[] renders = GetComponentsInChildren<Renderer>();
        foreach (Renderer render in renders) {
            StartCoroutine(FadeOut(render));
        }
    }
    
    IEnumerator FadeOut(Renderer render) {
        render.material.ToFadeMode();
        Color color = render.material.color;
        for (float f = 0; f <= Const.CHAR_FADE_OUT_SECONDS; f += Time.deltaTime) {
            color.a = Mathf.Lerp(1f, 0f, f/Const.CHAR_FADE_OUT_SECONDS);
            render.material.color = color;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
