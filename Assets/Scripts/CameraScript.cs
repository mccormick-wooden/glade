using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Player player;
    public GameObject PlayerGameObject;
    public GameObject ObjectToLookAt;
    private float cameraRotationHorizontalAngle = 0f;
    private float cameraRotationVerticalAngle = 0f;

    /*PlayerControls controls;
    Vector2 look;*/


    private void Awake()
    {
        //controls = new PlayerControls();
        //cameraRotationAngle += 0.002f;
    }

    /*
    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }
    */

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerGameObject.GetComponent<Player>();
        cameraRotationHorizontalAngle = 0f;
        cameraRotationVerticalAngle = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        CheckInputs();
        CalculateCameraPosition();
    }

    void CheckInputs()
    {
        // camera rotate
        /*
        if (Input.GetKey(KeyCode.E))
        {
            cameraRotationAngle += 0.002f;
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            cameraRotationAngle -= 0.002f;
        }
        */
    }

    void CalculateCameraPosition()
    {
        Transform smoothedTransform = transform;

        Vector3 position = PlayerGameObject.transform.position;
        //Vector3 forward = playerGameObject.transform.forward;

        //float angle = Mathf.Deg2Rad * player.RotationAngle + cameraRotationAngle;
        //cameraRotationAngle += player.cameraAngleChange / 100f;
        //cameraRotationHorizontalAngle = player.cameraHorizontalAngleChange;
        float horizontalAngle = Mathf.Deg2Rad;// * player.RotationAngle;// + (Mathf.Deg2Rad * cameraRotationHorizontalAngle * 90);

        //cameraRotationVerticalAngle = player.cameraVerticalAngleChange;
        //float verticalAngle = /*Mathf.Deg2Rad * player.RotationAngle +*/ (Mathf.Deg2Rad * cameraRotationVerticalAngle * 90);
        
        float x = Mathf.Sin(horizontalAngle) * -10.5f;
        float z = Mathf.Cos(horizontalAngle) * -10.5f;

        float y = Mathf.Sin(0) * 10 + 5;// + (3 * (2*cameraRotationVerticalAngle+1));

        //Vector3 desiredPosition = position + new Vector3(x, 3, z);
        Vector3 desiredPosition = position + new Vector3(x, y, z);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, 0.0125f);

        transform.position = smoothedPosition;
        transform.LookAt(ObjectToLookAt.transform.position);// + (5 * objectToLookAt.transform.forward));
    }
}
