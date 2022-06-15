using TMPro;
using UnityEngine;

public class Beacon : MonoBehaviour
{

    // All this should move out to a manager class
    public TextMeshProUGUI beaconCountText;
    public Boss boss;

    private void Start()
    {
        IncrementBeaconCount();
    }

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
        var oldCount = beaconCountText.text.Replace("Beacons: ", null);
        if (int.TryParse(oldCount, out var newCount))
        {
            newCount = increment ? ++newCount : --newCount;
            beaconCountText.text = string.Format("Beacons: {0}", newCount >= 0 ? newCount : 0);

            // TODO: Move out of this behavior out into a Manager
            if (boss.transform != null && newCount == 0)
            {
                boss.transform.gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.LogError("Could not parse int from beaconCountText", beaconCountText);
        }
    }
}
