using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abstract
{
    public abstract class BaseWeapon : MonoBehaviour, IWeapon
    {
        [SerializeField]
        private float attackDamage;

        public virtual float AttackDamage
        {
            get => attackDamage;
            protected set => attackDamage = value;
        }

        public virtual bool InUse { get; set; }

        protected virtual void Start()
        {
            InUse = false;
        }
    }
}
