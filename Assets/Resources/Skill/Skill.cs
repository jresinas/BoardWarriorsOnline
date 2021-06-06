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
}
