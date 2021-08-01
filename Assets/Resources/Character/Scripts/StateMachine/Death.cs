using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Called after death animations. Start corpse fade out effect
public class Death : StateMachineBehaviour {
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        CharacterController cc = animator.gameObject.GetComponent<CharacterController>();
        cc.DeathFadeOut();
    }
}
