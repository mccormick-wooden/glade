using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitLockedOnLayer : StateMachineBehaviour
{
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Strafing doesn't currently use root motion, so turn it back on when done locking on
        animator.applyRootMotion = true;
    }
}
