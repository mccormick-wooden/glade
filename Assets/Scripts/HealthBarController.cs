using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    private GameObject MainCamera = null;

    private bool useHealthText;
    private Text healthText;

    [SerializeField]
    private Slider hpSlider;

    private void Awake()
    {
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        healthText = GetComponentInChildren<Text>();
    }

    public float MaxHp
    {
        get => hpSlider.maxValue;
        set
        {
            hpSlider.maxValue = value;
            UpdateHealthText();
        }
    }

    public float CurrentHp
    {
        get => hpSlider.value;
        set
        {
            hpSlider.value = value;
            UpdateHealthText();
        }
    }

    private void UpdateHealthText()
    {
        if (!useHealthText || healthText == null) return;

        healthText.text = $"{Mathf.RoundToInt(CurrentHp)} / {MaxHp}";
    }

    // Initializes the HP value.
    public void InitHealthBar(float maxHp, bool useText)
    {
        useHealthText = useText;
        MaxHp = maxHp;
        CurrentHp = maxHp;
    }
}
