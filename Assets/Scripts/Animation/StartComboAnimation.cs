using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartComboAnimation : StateMachineBehaviour
{
    private static readonly int CanCombo = Animator.StringToHash("canCombo");

    /* Whenever starting a combo animation we want to suppress further combos by disabling the "canCombo" animator param
     * "canCombo" may be enabled again by leveraging an animation event at the proper key frame of the desired animation
     */
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(CanCombo, false);
    }
}
