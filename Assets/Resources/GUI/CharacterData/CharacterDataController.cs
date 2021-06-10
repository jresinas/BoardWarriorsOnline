using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterDataController : MonoBehaviour {
    public TextMeshProUGUI characterName;
    public SkillDataController[] characterSkills = new SkillDataController[Const.SKILL_NUMBER];
    int selectedCharacterId;

    /// <summary>
    /// Select character who is in turn
    /// </summary>
    /// <param name="character"></param>
    public void SelectCharacter(int characterId) {
        selectedCharacterId = characterId;
        //ShowSelectedCharacter();
        ShowCharacter();
    }

    /// <summary>
    /// Return selected character
    /// </summary>
    /// <returns></returns>
    public int GetSelectedCharacter() {
        return selectedCharacterId;
    }

    /// <summary>
    /// Show Character Info
    /// </summary>
    /// <param name="characterId">Character id to show. If empty, selected character will be shown</param>
    public void ShowCharacter(int characterId = -1) {
        characterId = (characterId < 0) ? selectedCharacterId : characterId;
        CharacterController character = CharacterManager.instance.Get(characterId);
        if (character != null) {
            bool enableSkill = (characterId == selectedCharacterId) && ClientManager.instance.IsAvailableSkill();
            characterName.text = character.name;
            for (int i = 0; i < Const.SKILL_NUMBER; i++) {
                characterSkills[i].ShowSkill(character.GetSkill(i), enableSkill);
            }
        }
    }

    /// <summary>
    /// Mark skill as active and others as inactive
    /// </summary>
    /// <param name="skillIndex">Skill to active. If empty, all skills are unselected</param>
    public void SelectSkill(int skillIndex = -1) {
        for (int i = 0; i < Const.SKILL_NUMBER; i++) {
            if (i == skillIndex) characterSkills[i].SelectSkill();
            else characterSkills[i].UnselectSkill();
        }
    }
}
