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
    public void SetCharacter(int characterId) {
        selectedCharacterId = characterId;
        CharacterController character = CharacterManager.instance.Get(characterId);
        for (int i = 0; i < Const.SKILL_NUMBER; i++) characterSkills[i].SetSkill(character.GetSkill(i));
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
        bool isSelectedCharacter = (characterId == selectedCharacterId) && ClientManager.instance.IsAvailableSkill();
        CharacterController character = CharacterManager.instance.Get(characterId);
        if (character != null) {
            characterName.text = character.name;
            for (int i = 0; i < Const.SKILL_NUMBER; i++) {
                characterSkills[i].ShowSkill(character.GetSkill(i), isSelectedCharacter);
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
