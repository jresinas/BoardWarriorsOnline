using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterMove : MonoBehaviour {
    [SerializeField] Animator anim;
    [SerializeField] CharacterController self;
    [SerializeField] float animationTime;

    int player = 0;

    int destPoint;
    List<Vector2Int> path;

    Vector3 originPosition;
    Vector3 targetPosition;
    bool animating = false;
    float time = 0;

    void Start() {
        player = self.GetPlayer();
    }

    void Update() {
        if (animating) Move(Time.deltaTime);
    }

    public void Idle() {
        StartCoroutine(SetIdle());
    }

    IEnumerator SetIdle() {
        yield return new WaitForFixedUpdate();
        transform.position = BoardManager.instance.GetTile(self.position).transform.position + Vector3.up * Const.CHAR_OFFSET;
        transform.rotation = player == 0 ? Quaternion.identity : Quaternion.Euler(0, 180, 0);

    }

    public bool StartMove(List<Vector2Int> path) {
        if (!animating && path.Count > 0) {
                destPoint = 0;
            this.path = path;
            Step();
            return true;
        }
        return false;
    }

    void Step() {
        if (path.Count != 0 && destPoint < path.Count) {
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
        }
    }


    void Move(float t) {
        time += t;

        if (time / animationTime > 1) Step();

        transform.position = Vector3.Lerp(originPosition, targetPosition, time / animationTime);
    }
}