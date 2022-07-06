using Assets.Scripts.Interfaces;
using Assets.Scripts.Abstract;
using UnityEngine;

namespace Assets.Scripts.Damageable
{
    public class AnimateDamageable : BaseDamageable
    {
        [SerializeField]
        private Animator animator = null;

        [SerializeField] private string applyDamageAnimTrigger;

        [SerializeField] private string dieAnimTrigger;

        protected override void Start()
        {
            base.Start();

            // If we haven't defined an animator in the unity editor, try finding one
            if (animator == null)
                animator = GetComponent<Animator>();

            // If still null, call for help
            if (animator == null)
                Debug.LogError($"{gameObject.name}.{GetType().Name}.{nameof(animator)} is null.");
        }

        protected override void ApplyDamage(IWeapon attackingWeapon, float modifier = 1f)
        {
            animator.SetTrigger(applyDamageAnimTrigger);
            base.ApplyDamage(attackingWeapon, modifier);
        }

        protected override void Die()
        {
            animator.SetTrigger(dieAnimTrigger);
            base.Die();
        }
    }
}
