using Assets.Scripts.Abstract;
using UnityEngine;

namespace Assets.Scripts.Damageable
{
    public class AnimateDamageable : BaseDamageable
    {
        private Animator animator;

        [SerializeField]
        private string applyDamageAnimTrigger;

        [SerializeField]
        private string dieAnimTrigger;

        protected override void Start()
        {
            base.Start();

            animator = GetComponent<Animator>();
            if (animator == null)
                Debug.Log($"{gameObject.name}.{GetType().Name}.{nameof(animator)} is null.");
        }

        protected override void ApplyDamage(BaseWeapon attackingWeapon)
        {
            animator.SetTrigger(applyDamageAnimTrigger);
            base.ApplyDamage(attackingWeapon);
        }

        protected override void Die()
        {
            animator.SetTrigger(dieAnimTrigger);
            base.Die();
        }
    }
}
