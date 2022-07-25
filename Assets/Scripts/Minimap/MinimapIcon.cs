using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapIcon : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;

    public void Disable()
    {
        sprite.enabled = false;
    }
}
