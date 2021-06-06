using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour {
    [SerializeField] Sprite icon;
    [SerializeField] string text;
    int range;

    public int GetRange() {
        return range;
    }

    public Sprite GetIcon() {
        return icon;
    }

    public string GetText() {
        return text;
    }
}
