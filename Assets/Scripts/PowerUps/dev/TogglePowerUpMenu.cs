using PowerUps;
using UnityEngine;

public class TogglePowerUpMenu : MonoBehaviour
{
    [SerializeField] private bool isMenuOpen;
    private PowerUpMenu menu;

    private void Start()
    {
        isMenuOpen = false;
        menu = FindObjectOfType<PowerUpMenu>();
        Utility.LogErrorIfNull(menu, "PowerUpMenu menu", "Could not find PowerUpMenu object in scene.");

        menu.gameObject.SetActive(isMenuOpen);
    }

    // Update is called once per frame
    void Update()
    {
        ListenForKeyUp();
    }

    void ListenForKeyUp()
    {
        if (Input.GetKeyUp(KeyCode.Tab) || Input.GetButtonUp("Cancel"))
        {
            isMenuOpen = !isMenuOpen;
            menu.gameObject.SetActive(isMenuOpen);
        }
    }
}
