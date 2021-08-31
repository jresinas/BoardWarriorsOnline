using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceParticle : MonoBehaviour {
    [SerializeField] EnergySphere sphere;
    ParticleSystem particle;
    bool wasAlive = true;

    void Awake() {
        particle = GetComponent<ParticleSystem>();
    }

    void OnEnable() {
        wasAlive = true;
    }

    void Update() {
        // When particle death (was alive and now isn't alive) call sphere Increase method
        if (!particle.IsAlive() && wasAlive) {
            sphere.Increase();
            wasAlive = false;
        }
        
    }
}
