using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace dz
{
    public class dzPlayerController : MonoBehaviour
    {
        public float currentHp;
        public float maxHp = 100f;
        public HealthBarController myHpBar;

        public float damage;

        // Start is called before the first frame update
        void Start()
        {
            myHpBar.InitHp(maxHp);
            currentHp = myHpBar.CurrentHp;
        }

        // Uses the 'DzDamagePlayer' action added to the InputActions asset.
        // Mapped to the 'N' key.
        private void OnDzDamagePlayer()
        {
            DamagePlayer();
        }

        // Uses the 'DzHealPlayer' action added to the InputActions asset.
        // Mapped to the 'M' key.
        private void OnDzHealPlayer()
        {
            HealPlayer();
        }

        private void DamagePlayer()
        {
            myHpBar.ApplyDamage(damage);
            currentHp = myHpBar.CurrentHp;

            if (0 < currentHp)
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
            myHpBar.ApplyHeal(damage);
            currentHp = myHpBar.CurrentHp;

            if (currentHp < maxHp)
            {
                Debug.Log("Thanks for the potion!");
            }
            else
            {
                Debug.Log("I'm at full health!");
            }
        }

        // Update is called once per frame [unused for now]
        void Update()
        {
            // NOTE: This is the OLD method for reading user inputs. Doesn't need
            // the Input System.
            // Keeping it here for reference.

            //if (Input.GetKeyDown(KeyCode.N)) // -> 'N' key
            //{
            //    DamagePlayer();
            //}

            //if (Input.GetKeyDown(KeyCode.M)) // -> 'M' key
            //{
            //    HealPlayer();
            //}
        }
    }
}

