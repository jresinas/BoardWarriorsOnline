using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusController : MonoBehaviour {
    [SerializeField] GameObject[] healthPoints;
    [SerializeField] GameObject[] energyPoints;

    [SerializeField] CharacterController character;

    void Awake() {
        int characterId = character.GetId();
        ChangeHealthHandler(characterId, character.GetHealth());
        ChangeEnergyHandler(characterId, character.GetEnergy());
    }

    void Start() {
        CharacterManager.instance.OnChangeHealth += ChangeHealthHandler;
        CharacterManager.instance.OnChangeEnergy += ChangeEnergyHandler;
    }

    void Update() {
        Camera camera = ClientManager.instance.GetCamera();
        if (camera != null) transform.rotation = camera.transform.rotation;
    }

    void ChangeHealthHandler(int characterId, int health) {
        if (character.GetId() == characterId)
            for (int i = 0; i < 10; i++) healthPoints[i].SetActive(i < health);
    }

    void ChangeEnergyHandler(int characterId, int energy) {
        if (character.GetId() == characterId)
            for (int i = 0; i < 10; i++) energyPoints[i].SetActive(i < energy);
    }
}
