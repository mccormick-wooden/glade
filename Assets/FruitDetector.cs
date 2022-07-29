using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitDetector : MonoBehaviour
{
    [SerializeField]
    Player player;

    List<Transform> nearbyFruit;

    // Start is called before the first frame update
    void Start()
    {
        nearbyFruit = new List<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Fruit")
            return;

        Debug.Log("Found fruit!");

        nearbyFruit.Add(other.transform);

        /*
        // if the player doesn't already have a nearby fruit location, just assign it this
        if (!player.nearbyFruit)
        {
            player.nearbyFruit = transform;
            return;
        }

        // else determine which one is closer and assign this if it's closer
        float distanceToCurrentFruit = (player.transform.position - player.nearbyFruit.position).magnitude;
        float distanceToDetectedFruit = (player.transform.position - transform.position).magnitude;

        if (distanceToDetectedFruit < distanceToCurrentFruit)
            player.nearbyFruit = transform;
        */
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag != "Fruit")
            return;

        nearbyFruit.Remove(other.transform);
    }

    public bool FruitNearby()
    {
        return nearbyFruit.Count > 0;
    }

    public Transform GetClosestFruit()
    {
        Transform closestFruit = null;
        float closestFruitDistance = float.MaxValue;

        for (int i = 0; i < nearbyFruit.Count; i++)
        {
            float distanceToFruit = (player.transform.position - nearbyFruit[i].position).magnitude;
            if (distanceToFruit < closestFruitDistance)
            {
                closestFruit = nearbyFruit[i];
                closestFruitDistance = distanceToFruit;
            }
        }

        return closestFruit;
    }

    public void RemoveFruit(Transform fruit)
    {
        nearbyFruit.Remove(fruit);
    }
}
