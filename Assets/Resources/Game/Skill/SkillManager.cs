using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Controls when characters ending their skill animations (use skill, receive impact, etc) and sync them to finish at the same time
/// </summary>
public class SkillManager : MonoBehaviour {
    public static SkillManager instance;

    List<int> charactersInAnimation;
    List<int> charactersWaiting;
    // Dice roll has finished
    public bool diceRoll = false;

    void Awake() {
        instance = this;    
    }

    void Start() {
        DiceManager.instance.OnEndRollDicesAnim += EndRollDicesAnimHandler;
    }

    public void StartAnimation(List<int> characterIds) {
        charactersInAnimation = characterIds.Distinct().ToList();
        charactersWaiting = new List<int>();
    }

    private void EndRollDicesAnimHandler() {
        diceRoll = true;
    }

    public void EndSkillAnimation(int characterId) {
        if (charactersInAnimation.Contains(characterId) && !charactersWaiting.Contains(characterId)) {
            charactersWaiting.Add(characterId);
            if (charactersWaiting.Count == charactersInAnimation.Count) StartCoroutine(EndAnimation(charactersInAnimation));
        }
    }

    IEnumerator EndAnimation(List<int> characters) {
        // Delay after last character end animation
        yield return new WaitForSeconds(Const.WAIT_AFTER_SKILL_ANIM);
        for (int i = 0; i < characters.Count; i++) {
            CharacterController character = CharacterManager.instance.Get(characters[i]);
            character.EndAnimation();
        }
        charactersInAnimation = new List<int>();
        charactersWaiting = new List<int>();
        diceRoll = false;
    }
}
