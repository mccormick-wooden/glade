using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PauseMenuManager : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    private Player player => FindObjectOfType<Player>();
    private CinemachineBrain playerCamera => FindObjectOfType<CinemachineBrain>();

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup not found");
        }
        else
        {
            canvasGroup.alpha = 0; // We don't set this in the inspector because then we can't see it in the inspector! And that's annoying.
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TimeScaleToggle.Toggle();
            canvasGroup.alpha = !canvasGroup.interactable ? 1 : 0;
            canvasGroup.blocksRaycasts = !canvasGroup.interactable;
            player.enabled = canvasGroup.interactable;
            playerCamera.enabled = canvasGroup.interactable;
            canvasGroup.interactable = !canvasGroup.interactable;
        }
    }
}
