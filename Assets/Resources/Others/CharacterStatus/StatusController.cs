using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusController : MonoBehaviour {
    [SerializeField] GameObject[] healthPoints;
    [SerializeField] GameObject[] energyPoints;

    [SerializeField] CharacterController character;

    void Awake() {
        ChangeHealthHandler(null, character.GetHealth());
        ChangeEnergyHandler(null, character.GetEnergy());
    }

    void Start() {
        character.OnChangeHealth += ChangeHealthHandler;
        character.OnChangeEnergy += ChangeEnergyHandler;
    }

    void Update() {
        Camera camera = ClientManager.instance.GetCamera();
        if (camera != null) transform.rotation = camera.transform.rotation;
    }

    void ChangeHealthHandler(object source, int health) {
        for (int i = 0; i < 10; i++) healthPoints[i].SetActive(i < health);
    }

    void ChangeEnergyHandler(object source, int energy) {
        for (int i = 0; i < 10; i++) energyPoints[i].SetActive(i < energy);
    }
}
