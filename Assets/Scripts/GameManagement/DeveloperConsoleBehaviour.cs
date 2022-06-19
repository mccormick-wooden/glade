using TMPro;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

namespace Assets.Scripts.GameManagement
{
    public class DeveloperConsoleBehaviour : MonoBehaviour
    {
        [SerializeField] private string prefix = string.Empty;
        [SerializeField] private IConsoleCommand[] commands = new IConsoleCommand[0];

        [Header("UI")]
        [SerializeField] private GameObject uiCanvas = null;
        [SerializeField] private TMP_InputField inputField = null;

        private float pausedTimeScale;

        private static DeveloperConsoleBehaviour instance;

        private DeveloperConsole developerConsole;
        private DeveloperConsole DeveloperConsole => developerConsole != null ? developerConsole : new DeveloperConsole(prefix, commands);

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            DontDestroyOnLoad(gameObject);
        }

        public void Toggle(CallbackContext context)
        {
            if (!context.action.triggered)
                return;

            if (uiCanvas.activeSelf)
            {
                Time.timeScale = pausedTimeScale;
                uiCanvas.SetActive(false);
            }
            else
            {
                pausedTimeScale = Time.timeScale;
                Time.timeScale = 0;
                uiCanvas.SetActive(true);
                inputField.ActivateInputField();
            }
        }

        public void ProcessCommand(string inputValue)
        {
            DeveloperConsole.ProcessCommand(inputValue);
            inputField.text = string.Empty;
        }
    }
}
