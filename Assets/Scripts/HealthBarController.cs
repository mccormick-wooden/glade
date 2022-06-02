using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    public Slider hpSlider;

    // Initializes the HP value.
    public void InitHp(float maxHp)
    {
        hpSlider.maxValue = maxHp;
        hpSlider.value = maxHp;
    }

    // Sets the HP value.
    public void SetHp(float hp)
    {
        hpSlider.value = hp;
    }
}
