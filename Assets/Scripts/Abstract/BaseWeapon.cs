using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abstract
{
    public abstract class BaseWeapon : MonoBehaviour, IWeapon
    {
        [SerializeField] protected float baseAttackDamage;
        [SerializeField] protected float currentAttackDamage;

        [SerializeField] public bool isDPSType { get; protected set; } = false;

        public string[] TargetTags;

        public virtual float AttackDamage
        {
            get => currentAttackDamage;
            set => currentAttackDamage = value;
        }

        public virtual bool InUse { get; set; }

        protected virtual void Start()
        {
            InUse = false;
        }
    }
}
