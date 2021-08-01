using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterMove : MonoBehaviour {
    int player = 0;
    float animationTime = 1f;

    int destPoint;
    Vector2Int[] path;

    Vector3 originPosition;
    Vector3 targetPosition;
    bool animating = false;
    float time = 0;
    [SerializeField] Animator anim;

    public void StartCharacterMove(int player, float animationTime) {
        this.player = player;
        this.animationTime = animationTime;
        Idle();
    }

    // Update is called once per frame
    void Update() {
        if (animating) Move(Time.deltaTime);

    }

    public void Idle() {
        //transform.rotation = player == 0 ? Quaternion.identity : Quaternion.Euler(0, 180, 0);
        StartCoroutine(SetIdle());
    }

    IEnumerator SetIdle() {
        yield return new WaitForFixedUpdate();
        //transform.position = 
        transform.rotation = player == 0 ? Quaternion.identity : Quaternion.Euler(0, 180, 0);

    }

    public bool StartMove(Vector2Int[] path) {
        if (!animating && path.Length > 0) {
            destPoint = 0;
            this.path = path;
            Step();
            return true;
        }
        return false;
    }

    void Step() {
        if (path.Length != 0 && destPoint < path.Length) {
            time = 0;
            originPosition = transform.position;
            targetPosition = BoardManager.instance.GetTile(path[destPoint]).transform.position + Vector3.up * Const.CHAR_OFFSET;

            transform.LookAt(targetPosition);
            anim.SetBool("Walk", true);

            destPoint++;
            animating = true;
        } else {
            anim.SetBool("Walk", false);
            animating = false;
            //Idle();
        }
    }


    void Move(float t) {
        time += t;

        if (time / animationTime > 1) Step();

        transform.position = Vector3.Lerp(originPosition, targetPosition, time / animationTime);
    }
}