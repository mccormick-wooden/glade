using System;
using Cinemachine;
using PlayerBehaviors;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PowerUps
{
    public class PowerUpMenu : MonoBehaviour
    {
        public Action<BasePowerUp> PowerUpApplied;

        private Canvas _canvas;

        public GameObject firstOptionButton;
        public GameObject secondOptionButton;
        public GameObject thirdOptionButton;
        public GameObject noSelectButton;

        private BasePowerUp optionOne;
        private BasePowerUp optionTwo;
        private BasePowerUp optionThree;

        private Player Player => FindObjectOfType<Player>();
        private CinemachineBrain PlayerCamera => FindObjectOfType<CinemachineBrain>();

        private CharacterPlayerControls controls;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            Utility.LogErrorIfNull(_canvas, "canvas", "PowerUpMenu could not find its canvas");
        }

        private void Update()
        {
            // If we mouse click on canvas, it deselects all buttons and makes
            // keyboard/gamepad navigation broken. Select the invisible button
            // instead.
            if (EventSystem.current.currentSelectedGameObject == null)
                EventSystem.current.SetSelectedGameObject(noSelectButton);
        }

        private void OnEnable()
        {
            SetPauseState(true);

            // UI Specific
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstOptionButton);

            // Heavy-handed way to keep cursor hidden until mouse moves
            controls = new CharacterPlayerControls();
            controls.Gameplay.Disable();
            controls.PauseGame.Enable();
            controls.PauseGame.MouseMove.performed += _ => {
                if (isActiveAndEnabled)
                    Utility.ShowCursor();
            };
            Utility.HideCursor();

            PrepareOptions();
        }

        private void OnDisable()
        {
            SetPauseState(false);
            Utility.HideCursor();
            controls.Dispose();
        }

        public void OnClick(int buttonNumber)
        {
            var powerUpToApply = buttonNumber switch
            {
                1 => optionOne,
                2 => optionTwo,
                3 => optionThree,
                _ => null
            };

            powerUpToApply?.ApplyPowerUp();

            gameObject.SetActive(false);

            PowerUpApplied?.Invoke(powerUpToApply);
        }

        private void PrepareOptions()
        {
            // Once we have more power ups we probably want to be pulling from a pool of power ups
            optionOne = new DamageIncreasePowerUp();
            optionTwo = new DamageResistPowerUp();
            optionThree = new MaxHealthPowerUp();

            firstOptionButton.GetComponentInChildren<Text>().text = optionOne.Description;
            secondOptionButton.GetComponentInChildren<Text>().text = optionTwo.Description;
            thirdOptionButton.GetComponentInChildren<Text>().text = optionThree.Description;
        }

        private void SetPauseState(bool areWePausing)
        {
            if (Player != null) Player.enabled = !areWePausing;

            if (areWePausing && !TimeScaleToggle.IsTimePaused || !areWePausing && TimeScaleToggle.IsTimePaused)
                TimeScaleToggle.Toggle();

            if (areWePausing)
            {
                Utility.DisableAllOf(except: PlayerStats.Instance);
                _canvas.enabled = true;
            }
            else
                Utility.EnableAllOf(except: new Canvas[] { _canvas,
                    GameObject.Find("TreeSpiritDialogueCanvas")?.GetComponentInChildren<Canvas>(),
                    GameObject.Find("EndGameMenu")?.GetComponentInChildren<Canvas>() });
        }
    }
}
