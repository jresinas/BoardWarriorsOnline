using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortraitController : MonoBehaviour {
    [SerializeField] Image image;
    [SerializeField] Image crystal;
    public int characterId;
    public int index;

    public int GetCharacterId() {
        return characterId;
    }

    /// <summary>
    /// Initialize portrait with specified character
    /// </summary>
    /// <param name="index">Portrait index</param>
    /// <param name="id">Character id to load</param>
    public void InitializePortrait(int index, int id) {
        this.index = index;
        characterId = id;
        CharacterController character = CharacterManager.instance.Get(id);
        if (character != null) {
            image.sprite = character.portrait;
            crystal.color = (character.GetPlayer() == 0) ? Const.RED_CRYSTAL : Const.BLUE_CRYSTAL;
        }
    }

    /// <summary>
    /// Play portrait resize animation
    /// </summary>
    /// <param name="startSize">Initial portrait size</param>
    /// <param name="endSize">Finish portrait size</param>
    /// <param name="time">Animation time</param>
    public void Resize(float startSize, float endSize, float time) {
        StartCoroutine(ResizeAnim(startSize, endSize, time));
    }

    /// <summary>
    /// Play portrait move animation
    /// </summary>
    /// <param name="startPosition">Initial portrait X position</param>
    /// <param name="endPosition">Finish portrait X position</param>
    /// <param name="time">AnimationTime</param>
    public void Move(float startPosition, float endPosition, float time) {
        StartCoroutine(MoveAnim(startPosition, endPosition, time));
    }

    IEnumerator ResizeAnim(float startSize, float endSize, float time) {
        float size;
        for (float f = 0; f <= time; f += Time.deltaTime) {
            size = Mathf.Lerp(startSize, endSize, f / time);
            transform.localScale = new Vector3(size, size, size);
            yield return null;
        }
        transform.localScale = new Vector3(endSize, endSize, endSize);
    }

    IEnumerator MoveAnim(float startPosition, float endPosition, float time) {
        float position;
        for (float f = 0; f <= time; f += Time.deltaTime) {
            position = Mathf.Lerp(startPosition, endPosition, f / time);
            transform.localPosition = new Vector3(position, transform.localPosition.y, transform.localPosition.z);
            yield return null;
        }
        transform.localPosition = new Vector3(endPosition, transform.localPosition.y, transform.localPosition.z);
    }
}
