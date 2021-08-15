using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour {
    CharacterController caster;
    List<CharacterController> targets;
    bool success;

    void Start() {
        var tm = GetComponentInChildren<AE_PhysicsMotion>();
        if (tm != null) tm.CollisionEnter += CollisionEnter;
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
                //targets[0].ReceiveImpact(success);
                targets[0].Dodge();
            }
        }
    }

    // Determine need to collision (isTrigger = false) with next character
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

    // Initialize projectile
    public void StartProjectile(CharacterController caster) {
        this.caster = caster;
        CharacterSkill cs = caster.GetComponent<CharacterSkill>();
        this.targets = cs.GetTargets();
        this.success = cs.GetSuccess();
        transform.LookAt(targets[0].transform.position+Vector3.up*0.7f);

        SphereCollider collider = GetComponentInChildren<SphereCollider>();
        collider.isTrigger = true;
        NextTarget();
    }

    private void CollisionEnter(object sender, AE_PhysicsMotion.AE_CollisionInfo e) {
        CharacterController targetController = (e.ContactPoint.otherCollider).GetComponentInParent<CharacterController>();
        if (success && targetController != null && targets.Count > 0 && targets.Contains(targetController)) {
            //targetController.ReceiveImpact(success);
            if (targetController.GetHealth() <= 0) targetController.Death();
            else targetController.ReceiveDamage();

            Renderer[] renders = GetComponentsInChildren<MeshRenderer>();
            foreach (Renderer render in renders) StartCoroutine(FadeOut(render));
        }
    }

    IEnumerator FadeOut(Renderer render) {
        float num = 1f;
        for (float f = 0; f <= Const.PROJ_FADE_OUT_SECONDS; f += Time.deltaTime) {
            num = Mathf.Lerp(1f, 0f, f/Const.PROJ_FADE_OUT_SECONDS);
            render.material.SetFloat("_InvFade", num);
            yield return null;
        }
        //Destroy(render.gameObject);
    }
}
