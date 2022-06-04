using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class Beacon : MonoBehaviour
{
    public TextMeshProUGUI beaconCountText;

    void DecrementBeaconCount()
    {
        SetBeaconCountText(false);
    }

    void IncrementBeaconCount()
    {
        SetBeaconCountText(true);
    }

    void OnDisable()
    {
        DecrementBeaconCount();
    }

    void SetBeaconCountText(bool increment)
    {
        int newCount;

        string oldCount = beaconCountText.text.Replace("Beacons: ", null);
        if (int.TryParse(oldCount, out newCount))
        {
            newCount = increment ? ++newCount : --newCount;
            beaconCountText.text = string.Format("Beacons: {0}", newCount >= 0 ? newCount : 0);
        } else {
            Debug.LogError("Could not parse int from beaconCountText", beaconCountText);
        }
    }

    private void OnTriggerEnter(Collider other) {
        // The Beacons probably want to react to any kind of "damage" - but this is hacky and specific to the Sword object.
        if (other.name == "Sword") {
            Sword sword = other.gameObject.GetComponent(typeof(Sword)) as Sword;
            if (sword != null && sword.isSwinging) {
                Debug.Log("Beacon deactivated due to Sword swing hit.");
                gameObject.SetActive(false);
            }
        }
    }

    void Start()
    {
        IncrementBeaconCount();
    }
}
