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

    /*
    public bool shove = false;
    public Vector2Int shoveOrigin;
    */

    public List<CharacterController> GetTargets() {
        return targetCharacters;
    }

    public bool GetSuccess() {
        return success;
    }

    //public void StartPlay(Skill skill, CharacterController targetCharacter, bool success) {
    public void StartPlay(Skill skill, int[] targetIds, bool success, int[] observerIds, string data) {
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
        StartAnimation();
    }

    void StartAnimation() {
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

    IEnumerator WaitDiceRoll() {
        yield return new WaitForSeconds(1);
        anim.SetTrigger(skill.GetAnimation());
    }

    // Attacking character 
    //void Impact() {
    //    foreach (CharacterController targetCharacter in targetCharacters) {
    //        if (targetCharacter != self) targetCharacter.ReceiveImpact(success);
    //    }
    //}

    // During attack, waiting dice roll
    public void Waiting() {
        anim.SetTrigger("Waiting");
    }

    public void Impact(string type = "Damage") {
        foreach (CharacterController targetCharacter in targetCharacters) {
            if (targetCharacter != self) {
                if (!success) targetCharacter.Dodge();
                else {
                    if (targetCharacter.GetHealth() <= 0) targetCharacter.Death();
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
        }
    }   

    /*
    public void ReceiveImpact(bool success) {
        if (success) {
            if (self.GetHealth() <= 0) anim.SetBool("Death", true);
            //else if (shove) self.ShoveAnimation(shoveOrigin);
            else anim.SetTrigger("Damage");
        } else anim.SetTrigger("Dodge");
    }
    */

    public void Damage() {
        anim.SetTrigger("Damage");
    }

    public void Dodge() {
        anim.SetTrigger("Dodge");
    }

    public void Death() {
        anim.SetBool("Death", true);
    }

    public void EndAnimation() {
        if (!anim.GetBool("Death")) anim.SetTrigger("EndAnimation");
        targetCharacters = null;
        skill = null;
    }

    void Effect(int number) {
        skill.AnimationEffect(number);
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
