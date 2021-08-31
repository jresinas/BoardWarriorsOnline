using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterList : MonoBehaviour {
    [SerializeField] Transform characterData;
    //[SerializeField] RectTransform content;
    [SerializeField] CharacterContainer content;

    [SerializeField] List<CharacterDataObject> characters = new List<CharacterDataObject>();

    // Start is called before the first frame update
    void Awake() {
        characters.Sort();

        Canvas canvas = GetComponentInParent<Canvas>();
        content.container.sizeDelta = new Vector2(625, 20 + (characters.Count * 200));
        for (int i = 0; i < characters.Count; i++) {
            Transform charTransform = Instantiate(characterData, canvas.transform);
            CharacterData charData = charTransform.GetComponent<CharacterData>();
            if (charData != null) {
                charData.Initialize(characters[i]);
                content.AddCharacter(charData);
            }
        }
    }
}