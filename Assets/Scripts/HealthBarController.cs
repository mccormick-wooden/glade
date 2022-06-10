using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    public bool SnapToMainCamera = true;

    private GameObject MainCamera = null;

    [SerializeField]
    private Slider hpSlider;

    private void Awake()
    {
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    private void Update()
    {
        if (SnapToMainCamera && MainCamera != null)
        {
            transform.LookAt(MainCamera.transform.position);
        }
    }

    public float MaxHp
    {
        get => hpSlider.maxValue;
        set => hpSlider.maxValue = value;
    }

    public float CurrentHp
    {
        get => hpSlider.value;
        set => hpSlider.value = value;
    }

    // Initializes the HP value.
    public void InitHealthBar(float maxHp)
    {
        MaxHp = maxHp;
        CurrentHp = maxHp;
    }
}
