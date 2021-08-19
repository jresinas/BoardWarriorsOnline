using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : MonoBehaviour {
    [SerializeField] Animator anim;
    [SerializeField] CharacterController self;
    // Character animation time spent on move to an adjacent tile 
    [SerializeField] float animationTime;

    int player;
    List<Vector2Int> path;

    void Start() {
        player = self.GetPlayer();
    }

    /// <summary>
    /// Set character to idle status
    /// </summary>
    public void Idle() {
        StartCoroutine(SetIdle());
    }

    IEnumerator SetIdle() {
        yield return new WaitForFixedUpdate();
        transform.position = BoardManager.instance.GetTile(self.position).transform.position + Vector3.up * Const.CHAR_OFFSET;
        transform.rotation = player == 0 ? Quaternion.identity : Quaternion.Euler(0, 180, 0);

    }

    /// <summary>
    /// Start move animation across specified path
    /// </summary>
    /// <param name="path">List of positions to move through</param>
    public void StartMove(List<Vector2Int> path) {
        if (path.Count > 0) {
            this.path = path;
            Step();
        }
    }

    void Step() {
        if (path.Count > 0) {
            Vector3 originPosition = transform.position;
            Vector3 targetPosition = BoardManager.instance.GetTile(path[0]).transform.position + Vector3.up * Const.CHAR_OFFSET;
            path.RemoveAt(0);

            transform.LookAt(targetPosition);
            anim.SetBool("Walk", true);

            StartCoroutine(Move(originPosition, targetPosition));
        } else anim.SetBool("Walk", false);
    }


    IEnumerator Move(Vector3 origin, Vector3 destiny) {
        for (float f = 0; f <= animationTime; f += Time.deltaTime) {
            transform.position = Vector3.Lerp(origin, destiny, f / animationTime);
            yield return null;
        }
        Step();
    }
}