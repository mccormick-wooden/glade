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

        [SerializeField] private AudioClip takeDamageSoundEffect;

        [SerializeField] private AudioClip dieSoundEffect;

        [SerializeField] private float dieSoundEffectPitch;

        [SerializeField] private float dieSoundEffectVolume;

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
            if (IsDead)
                return;

            //EventManager.TriggerEvent<MonsterTakeDamageEvent, Vector3, AudioClip>(transform.position, takeDamageSoundEffect);

            animator.SetTrigger(applyDamageAnimTrigger);
            base.ApplyDamage(attackingWeapon, modifier);
        }

        protected override void Die()
        {
            EventManager.TriggerEvent<MonsterDieEvent, Vector3, AudioClip, float, float>(transform.position, dieSoundEffect, dieSoundEffectPitch, dieSoundEffectVolume);

            animator.SetTrigger(dieAnimTrigger);
            base.Die();
        }
    }
}
