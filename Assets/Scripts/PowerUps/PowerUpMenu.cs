using Cinemachine;
using PlayerBehaviors;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PowerUps
{
    public class PowerUpMenu : MonoBehaviour
    {
        private Canvas _canvas;

        public GameObject firstOptionButton;
        public GameObject secondOptionButton;
        public GameObject thirdOptionButton;

        private BasePowerUp optionOne;
        private BasePowerUp optionTwo;
        private BasePowerUp optionThree;

        private Player Player => FindObjectOfType<Player>();
        private CinemachineBrain PlayerCamera => FindObjectOfType<CinemachineBrain>();

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            Utility.LogErrorIfNull(_canvas, "canvas", "PowerUpMenu could not find its canvas");
        }

        private void OnEnable()
        {
            SetPauseState(true);

            // UI Specific
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstOptionButton);

            PrepareOptions();
        }

        private void OnDisable()
        {
            SetPauseState(false);
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
            if (PlayerCamera != null) PlayerCamera.enabled = !areWePausing;

            if (areWePausing && !TimeScaleToggle.IsTimePaused || !areWePausing && TimeScaleToggle.IsTimePaused)
                TimeScaleToggle.Toggle();

            if (areWePausing)
            {
                Utility.DisableAllOf(except: PlayerStats.Instance);
                _canvas.enabled = true;
            }
            else
                Utility.EnableAllOf(except: _canvas);
        }
    }
}
