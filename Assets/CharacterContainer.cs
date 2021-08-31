using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterContainer : MonoBehaviour {
    [SerializeField] public RectTransform container;
    public List<CharacterData> characters = new List<CharacterData>();
    float CHAR_DATA_HEIGHT = 180;
    float OFFSET_CHAR = 20;


    public void AddCharacter(CharacterData character) {
        character.SetContainer(this);
        character.currentContainer = this;
        characters.Add(character);
        Refresh();
    }

    public void RemoveCharacter(CharacterData character) {
        characters.Remove(character);
        Refresh();
    }

    void Refresh() {
        characters.Sort();
        for (int i = 0; i < characters.Count; i++) {
            PlaceCharacter(characters[i], i);
        }
    }

    void PlaceCharacter(CharacterData character, int index) {
        RectTransform characterRectTransform = character.GetComponent<RectTransform>();
        characterRectTransform.anchoredPosition = new Vector2(0, -(CHAR_DATA_HEIGHT / 2 + OFFSET_CHAR) - ((CHAR_DATA_HEIGHT + OFFSET_CHAR) * index));

    }
}
