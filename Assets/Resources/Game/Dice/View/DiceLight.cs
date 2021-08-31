using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceLight : MonoBehaviour {
    public float rotation;

    private void Start() {
        transform.Rotate(0, 0, Random.Range(0, 360));
    }

    void Update() {
        transform.Rotate(0, 0, rotation);
    }
}
