using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterDataController : MonoBehaviour {
    public TextMeshProUGUI characterName;
    public SkillDataController[] characterSkills = new SkillDataController[Const.SKILL_NUMBER];
    CharacterController selectedCharacter;

    public void SelectCharacter(CharacterController character) {
        selectedCharacter = character;
        ShowCharacter();
    }


    void ShowCharacter() {
        bool isTurn = GUIManager.instance.IsTurn();
        ShowCharacter(selectedCharacter, isTurn);
    }

    void ShowCharacter(CharacterController character, bool enable = false) {
        if (character != null) {
            characterName.text = character.name;
            for (int i = 0; i < Const.SKILL_NUMBER; i++) {
                characterSkills[i].ShowSkill(character.GetSkill(i), enable);
            }
        }
    }
}
