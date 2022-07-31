using System;
using UnityEngine;

public class TriggerPlane : MonoBehaviour
{
    public Action<Collider> PlaneTriggered;

    [SerializeField]
    private string playerModelGameObjectRootName = "PlayerModel";
    private GameObject playerModel;
    private Player playerScript;

    //private bool isTriggered;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Player is Out of Bounds!");
        //if (isTriggered) 
        //    return;

        //isTriggered = true;
        //Debug.Log("Invoke");
        PlaneTriggered?.Invoke(other);
    }

    private void OnTriggerExit()
    {
        //Debug.Log("Teleported to starting position");
        playerModel = GameObject.Find(playerModelGameObjectRootName);
        Utility.LogErrorIfNull(playerModel, nameof(playerModel));
        playerScript = playerModel.GetComponent<Player>();
        Utility.LogErrorIfNull(playerScript, nameof(playerScript));
        playerScript.StopAnimMotion();
        //isTriggered = false;
    }
}
