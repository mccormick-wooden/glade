using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterStrafeBlendTree : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Strafing does not currently use root motion, so disable it when entering the blend tree
        animator.applyRootMotion = false;
    }
}
