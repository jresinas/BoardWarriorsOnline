using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkill : MonoBehaviour {
    [SerializeField] Animator anim;
    [SerializeField] CharacterController self;

    List<CharacterController> targetCharacters;
    List<CharacterController> observerCharacters;
    Skill skill;
    bool success;
    string data;

    #region Get&Set
    public List<CharacterController> GetTargets() {
        return targetCharacters;
    }

    public bool GetSuccess() {
        return success;
    }
    #endregion

    /// <summary>
    /// Start using skill animation
    /// </summary>
    /// <param name="skill">Skill to use</param>
    /// <param name="targetIds">Array of character ids directly affected by skill animation</param>
    /// <param name="success">True if skill was executed successfully and will impact on targets</param>
    /// <param name="observerIds">Array of character ids indirectly affected by skill</param>
    /// <param name="data">Additional data provided from skill execution (serialized in JSON)</param>
    public void StartSkill(Skill skill, int[] targetIds, bool success, int[] observerIds, string data) {
        this.targetCharacters = CharacterManager.instance.Get(targetIds);
        this.observerCharacters = CharacterManager.instance.Get(observerIds);
        this.skill = skill;
        this.success = success;
        this.data = data;
        List<int> characterIds = new List<int>();
        characterIds.AddRange(targetIds);
        characterIds.AddRange(observerIds);
        characterIds.Add(self.GetId());
        SkillManager.instance.StartAnimation(characterIds);
        PrepareCharacters();
    }

    void PrepareCharacters() {
        if (targetCharacters.Count > 0) transform.LookAt(targetCharacters[0].transform);
        Waiting();
        foreach (CharacterController targetCharacter in targetCharacters) {
            targetCharacter.transform.LookAt(transform);
            targetCharacter.Waiting();
        }
        foreach (CharacterController observerCharacter in observerCharacters) {
            observerCharacter.transform.LookAt(transform);
            observerCharacter.Waiting();
        }

        StartCoroutine(WaitDiceRoll());
    }

    /// <summary>
    /// Characters waiting until dice roll has finished
    /// </summary>
    IEnumerator WaitDiceRoll() {
        yield return new WaitUntil(() => SkillManager.instance.diceRoll);
        anim.SetTrigger(skill.GetAnimation());
    }    

    /// <summary>
    /// Controls impact effect on character and activate appropriate animation
    /// </summary>
    /// <param name="targetCharacter">CharacterController of character impacted</param>
    /// <param name="type">Type of impact: Damage or Shove</param>
    public void ReceiveImpact(CharacterController targetCharacter, string type) {
        if (targetCharacter.GetHealth() <= 0) targetCharacter.Death();
        else {
            switch (type) {
                case "Damage":
                    targetCharacter.ReceiveDamage();
                    break;
                case "Shove":
                    targetCharacter.ReceiveShove(data);
                    break;
                default:
                    Debug.LogError("Invalid type of impact");
                    break;
            }
        }
    }

    #region ActivateAnimations
    public void Damage() {
        anim.SetTrigger("Damage");
    }

    public void Dodge() {
        anim.SetTrigger("Dodge");
    }

    public void Death() {
        anim.SetBool("Death", true);
    }

    public void Waiting() {
        anim.SetTrigger("Waiting");
    }
    #endregion

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
            color.a = Mathf.Lerp(1f, 0f, f / Const.CHAR_FADE_OUT_SECONDS);
            render.material.color = color;
            yield return null;
        }
        gameObject.SetActive(false);
    }

    #region AnimEventCallbacks
    void Impact(string type = "Damage") {
        foreach (CharacterController targetCharacter in targetCharacters) {
            if (targetCharacter != self) {
                if (!success) targetCharacter.Dodge();
                else {
                    ReceiveImpact(targetCharacter, type);
                }
            }
        }
    }

    void Effect(int number) {
        skill.AnimationEffect(number);
    }
    #endregion
}
