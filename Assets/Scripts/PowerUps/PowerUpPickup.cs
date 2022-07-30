using PowerUps;
using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    public bool PowerUpEnabled = true;

    private PowerUpMenu menu;

    private void Start()
    {
        menu = FindObjectOfType<PowerUpMenu>(true); // Include the inactive menu
        Utility.LogErrorIfNull(menu, "PowerUpMenu menu", "Could not find PowerUpMenu object in scene.");
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.gameObject.GetComponent<Player>();
        if (player != null && PowerUpEnabled)
        {
            Destroy(gameObject);
            menu.gameObject.SetActive(true);
        }
    }
}
