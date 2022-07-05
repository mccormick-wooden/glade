using UnityEngine;

namespace Animation
{
    public class ExitCombatLayerAnimation : StateMachineBehaviour
    {
        private static readonly int IsAttacking = Animator.StringToHash("isAttacking");
        private static readonly int CanCombo = Animator.StringToHash("canCombo");

        /*
         * Should be attached to each animation in the "Combat Layer" layer of the animator
         *
         * Resets Animator parameters that are used by PlayerCombat
         */
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(IsAttacking, false);
            animator.SetBool(CanCombo, false);
        }
    }
}
