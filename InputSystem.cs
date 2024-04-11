using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
public class InputSystem : MonoBehaviour
{
    Movement movement;
    [System.Serializable]
    public struct InputSettings
    {
        public string ForwardInput;
        public string DirectionInput;
        public string SprintInput;
        public string aim;
        public string fire;

    }
    [SerializeField]
    // Define an instance of the InputSettings struct
    public InputSettings inputSettings;

    [Header("Camera and Character Sync")]
    public float lookDistance;
    public float lookSpeed;

    [Header("Aiming Settings")]
    RaycastHit hit;
    public LayerMask aimLayers;
    Ray ray;

    [Header("Spine Settings")]
    public Transform spine;
    public Vector3 spineOffset;

    [Header("Head Rotation Settings")]
    public float lookAtPoint = 2.8f;


    Transform camCenter;
    Transform mainCam;

    public Bow bowScript;
    bool isAimimg;

    public bool testAim;

    // Start is called before the first frame update
    void Start()
    {
        // Assign the Movement component to the movement variable
        movement = GetComponent<Movement>();
        camCenter = Camera.main.transform.parent;
        mainCam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        // Use the input settings from the struct
        if (Input.GetAxis(inputSettings.ForwardInput) != 0 || Input.GetAxis(inputSettings.DirectionInput) != 0)
        {
            RotateToCamView();
        }

        isAimimg = Input.GetButton(inputSettings.aim);

        if (testAim)
        {
            isAimimg = true;
        }

        movement.AnimateCharacter(Input.GetAxis(inputSettings.ForwardInput), Input.GetAxis(inputSettings.DirectionInput));
        movement.Sprint(Input.GetButton(inputSettings.SprintInput));
        movement.CharacterAim(isAimimg);

        if (isAimimg)
        {
            Aim();
            movement.CharacterPullString(Input.GetButton(inputSettings.fire));
        }

    }

    void LateUpdate()
    {
        if (isAimimg)
        {
            RotateCharacterSpine();
        }
    }

    void RotateToCamView()
    {
        Vector3 camCenterPos = camCenter.position;
        Vector3 lookPoint = camCenterPos + (camCenter.forward * lookDistance);
        Vector3 direction = lookPoint - transform.position;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        lookRotation.x = 0;
        lookRotation.z = 0;

        Quaternion finalRotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookSpeed);
        transform.rotation = finalRotation;
    }

    //Does the aiming and sends a raycast to a target
    void Aim()
    {
        Vector3 camPosition = mainCam.position;
        Vector3 dir = mainCam.forward;

        ray = new Ray(camPosition, dir);
        if (Physics.Raycast(ray, out hit, 500f, aimLayers))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red);
            bowScript.ShowCrosshair(hit.point);
        }else
        {
            bowScript.RemoveCrosshair();
        }
    }

    void RotateCharacterSpine()
    {
        spine.LookAt(ray.GetPoint(50));
        spine.Rotate(spineOffset);
    }
}
