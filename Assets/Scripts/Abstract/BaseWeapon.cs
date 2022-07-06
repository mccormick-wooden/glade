using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abstract
{
    public abstract class BaseWeapon : MonoBehaviour, IWeapon
    {
        [SerializeField] protected float attackDamage;

        [SerializeField]
        public bool isDPSType { get; protected set; } = false;

        public string[] TargetTags;

        public virtual float AttackDamage
        {
            get => attackDamage;
            /* Not ideal to be public, should be changed with combat overhaul */
            set => attackDamage = value;
        }

        public virtual bool InUse { get; set; }

        protected virtual void Start()
        {
            InUse = false;
        }
    }
}
