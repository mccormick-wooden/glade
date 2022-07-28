using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThoughtBubble : MonoBehaviour
{
    [SerializeField]
    Texture attack;

    [SerializeField]
    Texture defendBeacon;

    [SerializeField]
    Texture heal;

    [SerializeField]
    Texture locatePlayer;

    RawImage image;
    DateTime displayTime;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<RawImage>();
        image.enabled = false;
        displayTime = DateTime.Now;
    }

    // Update is called once per frame
    void Update()
    {
        if (image.enabled && DateTime.Now > displayTime.AddSeconds(5))
        {
            Hide();
        }
    }

    private void Attack()
    {
        image.texture = attack;
    }

    private void Defend()
    {
        image.texture = defendBeacon;
    }

    private void Heal()
    {
        image.texture = heal;
    }

    private void LocatePlayer()
    {
        image.texture = locatePlayer;
    }

    public void Display()
    {
        image.enabled = true;
        displayTime = DateTime.Now;
    }

    public void Hide()
    {
        image.enabled = false;
    }

    public void ShowPriority(BaseEnemy.Priority priority)
    {
        Display();
        switch(priority)
        {
            case BaseEnemy.Priority.AttackPlayer:
                Attack();
                break;
            case BaseEnemy.Priority.DefendBeacon:
                Defend();
                break;
            case BaseEnemy.Priority.HealAtCrystal:
                Heal();
                break;
            case BaseEnemy.Priority.LocatePlayer:
                LocatePlayer();
                break;
        }
    }
}
