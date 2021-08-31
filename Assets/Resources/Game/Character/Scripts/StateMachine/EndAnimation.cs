using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Called when finish skill animation. Sync all characters animations endings
public class EndAnimation : StateMachineBehaviour {
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        CharacterController cc = animator.gameObject.GetComponent<CharacterController>();
        if (cc != null) SkillManager.instance.EndSkillAnimation(cc.GetId());
    }
}
