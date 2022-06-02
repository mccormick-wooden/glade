using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class dzPlayerController : MonoBehaviour
{
    public float currHP;
    public float maxHP = 100f;
    public HealthBarController myHpBar;

    public float damage;

    // Start is called before the first frame update
    void Start()
    {
        currHP = maxHP;
        myHpBar.InitHp(maxHP);
    }

    // Uses the 'Fire' action to the InputActions asset.
    // It's mapped to the left mouse button.
    private void OnFire()
    {
        DamagePlayer();
    }

    // Update is called once per frame
    void Update()
    {
        // NOTE: This is the OLD method for reading user inputs.
        // Doesn't need the Input System.
        if (Input.GetKeyDown(KeyCode.N)) // -> 'N' key
        {
            DamagePlayer();
        }

        if (Input.GetKeyDown(KeyCode.M)) // -> 'M' key
        {
            HealPlayer();
        }
    }

    private void DamagePlayer()
    {
        currHP = Mathf.Max((currHP - damage), 0f);
        myHpBar.SetHp(currHP);

        if (0 < currHP)
        {
            Debug.Log("I'm taking damage here!");
        }
        else
        {
            Debug.Log("I'm dead!");
        }
    }

    private void HealPlayer()
    {
        currHP = Mathf.Min((currHP + damage), maxHP);
        myHpBar.SetHp(currHP);

        if (currHP < maxHP)
        {
            Debug.Log("Thanks for the potion!");
        }
        else
        {
            Debug.Log("I'm at full health!");
        }
    }
}
