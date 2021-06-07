using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterDataController : MonoBehaviour {
    public TextMeshProUGUI characterName;
    public SkillDataController[] characterSkills = new SkillDataController[Const.SKILL_NUMBER];
    CharacterController selectedCharacter;

    /// <summary>
    /// Select character who is in turn
    /// </summary>
    /// <param name="character"></param>
    public void SelectCharacter(CharacterController character) {
        selectedCharacter = character;
        SelectSkill();
        ShowSelectedCharacter();
    }

    /// <summary>
    /// Return selected character
    /// </summary>
    /// <returns></returns>
    public CharacterController GetSelectedCharacter() {
        return selectedCharacter;
    }

    /// <summary>
    /// Show selected character info
    /// </summary>
    public void ShowSelectedCharacter() {
        bool isTurn = GUIManager.instance.IsTurn();
        ShowCharacter(selectedCharacter, isTurn);
    }

    /// <summary>
    /// Show specified character info
    /// </summary>
    /// <param name="character">Character to show info</param>
    /// <param name="enable">If false, skills are shown as disabled</param>
    public void ShowCharacter(CharacterController character, bool enable = false) {
        if (character != null) {
            characterName.text = character.name;
            for (int i = 0; i < Const.SKILL_NUMBER; i++) {
                characterSkills[i].ShowSkill(character.GetSkill(i), enable);
            }
        }
    }

    /// <summary>
    /// Mark skill as active and others as inactive
    /// </summary>
    /// <param name="skillIndex">Skill to active. If is not specified, all skills are unselected</param>
    public void SelectSkill(int skillIndex = -1) {
        for (int i = 0; i < Const.SKILL_NUMBER; i++) {
            if (i == skillIndex) characterSkills[i].SelectSkill();
            else characterSkills[i].UnselectSkill();
        }
    }
}
