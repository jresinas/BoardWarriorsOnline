using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Called when return to idle status. Set character to default position
public class EndAction : StateMachineBehaviour {
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        CharacterMove cm = animator.gameObject.GetComponent<CharacterMove>();
        if (cm != null) cm.Idle();
    }
}