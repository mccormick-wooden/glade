﻿using Assets.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Scripts.Abstract
{
    public abstract class BaseWeapon : MonoBehaviour, IWeapon
    {
        [SerializeField]
        protected float attackDamage;

        public string[] TargetTags;
        
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
