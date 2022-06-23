using TMPro;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

namespace Assets.Scripts.GameManagement
{
    public class DeveloperConsoleBehaviour : MonoBehaviour
    {
        [SerializeField] private string prefix = string.Empty;
        [SerializeField] private BaseDevCommand[] commands = new BaseDevCommand[0];

        [Header("UI")]
        [SerializeField] private GameObject uiCanvas = null;
        [SerializeField] private TMP_InputField inputField = null;

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
            else
            {
                instance = this;
            }
        }

        public void Toggle(CallbackContext context)
        {
            if (!context.action.triggered)
                return;

            uiCanvas.SetActive(!uiCanvas.activeSelf);

            if (uiCanvas.activeSelf)
                inputField.ActivateInputField();
        }

        public void ProcessCommand()
        {
            var commandResult = DeveloperConsole.ProcessCommand(inputField.text);

            if (!commandResult.WasSuccessful)
                Debug.Log(commandResult.Response);

            inputField.text = string.Empty;
        }
    }
}
