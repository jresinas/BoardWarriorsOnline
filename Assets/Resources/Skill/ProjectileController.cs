using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour {
    List<CharacterController> targets;
    bool success;

    void Start() {
        var tm = GetComponentInChildren<AE_PhysicsMotion>();
        if (tm != null) tm.CollisionEnter += CollisionEnter;
    }

    /// <summary>
    /// Initialize projectile
    /// </summary>
    public void StartProjectile(CharacterController caster) {
        CharacterSkill cs = caster.GetComponent<CharacterSkill>();
        this.targets = cs.GetTargets();
        this.success = cs.GetSuccess();
        transform.LookAt(targets[0].transform.position + Vector3.up * Const.PROJ_OFFSET);

        SphereCollider collider = GetComponentInChildren<SphereCollider>();
        collider.isTrigger = true;
        NextTarget();
    }

    // Projectile has passed non target character
    void OnTriggerExit(Collider other) {
        NextTarget();
    }

    // Projectile (with not success) has collide target character
    void OnTriggerEnter(Collider other) {
        if (!success) {
            CharacterController cc = other.GetComponentInParent<CharacterController>();
            if (cc != null && cc == targets[0]) {
                targets[0].Dodge();
            }
        }
    }

    // Determine needs to collision (isTrigger = false) with next character
    void NextTarget() {
        SphereCollider collider = GetComponentInChildren<SphereCollider>();
        if (success && collider != null) {
            RaycastHit cast;
            Physics.Raycast(collider.transform.position, collider.transform.forward, out cast);
            if (cast.transform != null) {
                CharacterController castTarget = cast.transform.GetComponentInParent<CharacterController>();
                if (targets.Count > 0 && castTarget != null && castTarget == targets[0]) {
                    collider.isTrigger = false;
                }
            }
        }
    }

    private void CollisionEnter(object sender, AE_PhysicsMotion.AE_CollisionInfo e) {
        CharacterController targetController = (e.ContactPoint.otherCollider).GetComponentInParent<CharacterController>();
        if (success && targetController != null && targets.Count > 0 && targets.Contains(targetController)) {
            if (targetController.GetHealth() <= 0) targetController.Death();
            else targetController.ReceiveImpact("Damage"); 

            Renderer[] renders = GetComponentsInChildren<MeshRenderer>();
            foreach (Renderer render in renders) StartCoroutine(FadeOut(render));
        }
    }

    IEnumerator FadeOut(Renderer render) {
        float num;
        for (float f = 0; f <= Const.PROJ_FADE_OUT_SECONDS; f += Time.deltaTime) {
            num = Mathf.Lerp(1f, 0f, f/Const.PROJ_FADE_OUT_SECONDS);
            render.material.SetFloat("_InvFade", num);
            yield return null;
        }
    }
}
