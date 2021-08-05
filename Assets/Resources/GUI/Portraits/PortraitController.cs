using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortraitController : MonoBehaviour {
    public bool change = false;
    bool changing = false;
    [SerializeField] Image image;
    [SerializeField] Image crystal;
    public int characterId;
    //Color redCrystal = new Color32(210, 20, 20, 255);
    Color redCrystal = new Color32(255, 60, 60, 255);
    //Color blueCrystal = new Color32(20, 20, 210, 255);
    Color blueCrystal = new Color32(100, 110, 255, 255);

    public int GetCharacterId() {
        return characterId;
    }

    public void SetCharacterId(int id) {
        characterId = id;
        CharacterController character = CharacterManager.instance.Get(id);
        if (character != null) {
            image.sprite = character.portrait;
            crystal.color = (character.GetPlayer() == 0) ? redCrystal : blueCrystal;
        }
    }

    public void Resize(float startSize, float endSize, float time) {
        StartCoroutine(ResizeAnim(startSize, endSize, time));
    }

    public void Move(float startPosition, float endPosition, float time) {
        StartCoroutine(MoveAnim(startPosition, endPosition, time));
    }

    IEnumerator ResizeAnim(float startSize, float endSize, float time) {
        changing = true;
        float size = startSize;
        for (float f = 0; f <= time; f += Time.deltaTime) {
            size = Mathf.Lerp(startSize, endSize, f / time);
            transform.localScale = new Vector3(size, size, size);
            yield return null;
        }
        transform.localScale = new Vector3(endSize, endSize, endSize);
    }

    IEnumerator MoveAnim(float startPosition, float endPosition, float time) {
        changing = true;
        float position = startPosition;
        for (float f = 0; f <= time; f += Time.deltaTime) {
            position = Mathf.Lerp(startPosition, endPosition, f / time);
            transform.localPosition = new Vector3(position, transform.localPosition.y, transform.localPosition.z);
            yield return null;
        }
        transform.localPosition = new Vector3(endPosition, transform.localPosition.y, transform.localPosition.z);
    }
}
