using Assets.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Abstract
{
    public abstract class BasePlayerControls : MonoBehaviour, IPlayerControls
    {
        private CharacterPlayerControls controls;
        private Vector2 _cameraMovement;
        private Vector2 _movement;
        private bool _isAttackPerformed;
        
        public Vector2 movement
        {
            get => movement;
            protected set => _movement = value;
        }

        public Vector2 cameraMovement
        {
            get => cameraMovement;
            protected set => _cameraMovement = value;
        }

        public bool isAttackPerformed
        {
            get => _isAttackPerformed;
            protected set => _isAttackPerformed = value;
        }

        protected virtual void Awake()
        {
            controls = new CharacterPlayerControls();
            
            // m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
            //controls.Gameplay.Move.performed += ctx => _movement = ctx.ReadValue<Vector2>();
            //controls.Gameplay.Move.canceled += ctx => _movement = Vector2.zero;
            
            //controls.Gameplay.MoveCamera.performed += ctx => _cameraMovement = ctx.ReadValue<Vector2>();
            //controls.Gameplay.MoveCamera.canceled += ctx => _cameraMovement = Vector2.zero;
        }
    }
}
