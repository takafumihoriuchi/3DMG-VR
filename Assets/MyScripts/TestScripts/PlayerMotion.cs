using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotion : MonoBehaviour
{
    //[Tooltip("put help-text here")]
    //[SerializeField] GameObject someGameObject = null;

    [SerializeField] private GameObject OVRPlayerControllerGameObject = null;
    private OVRPlayerController OVRPlayerControllerComponent;

    // identical to fields of OVRPlayerController class
    private CharacterController Controller;
    private Vector3 MoveThrottle = Vector3.zero;
    private float MoveScale = 1.0f;
    private float MoveScaleMultiplier = 1.0f;
    private float SimulationRate = 60f;
    // todo 90fpsに変えられないか (adjust the value of SimulationRate?)
    private float Acceleration;

    private Vector3 moveDirection = Vector3.zero;
    private Vector3 playerVelocity = Vector3.zero;

    private Vector3 touchVelocityL;
    private Vector3 touchVelocityR;
    private Vector3 touchAccelerationL;
    private Vector3 touchAccelerationR;

    const float MIN_WALKSPEED = 1.0f;
    const float MAX_WALKSPEED = 2.5f;


    private void Awake()
    {
        Controller = OVRPlayerControllerGameObject.GetComponent<CharacterController>();
        OVRPlayerControllerComponent = Controller.GetComponent<OVRPlayerController>();
    }

    private void Start()
    {
        Acceleration = OVRPlayerControllerComponent.Acceleration;

        Controller.Move(moveDirection * Time.deltaTime);
    }

    private void UpdateHandShakeController()
    {
        touchVelocityL = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);
        touchVelocityR = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
        touchAccelerationL = OVRInput.GetLocalControllerAcceleration(OVRInput.Controller.LTouch);
        touchAccelerationR = OVRInput.GetLocalControllerAcceleration(OVRInput.Controller.RTouch);
        UpdateHandShakeMovement();
    }

    private void UpdateHandShakeMovement()
    {

        bool moveForward = DetectHandShakeWalk();

        if (Controller.isGrounded)
            MoveScale = 0.0f;
        else
            MoveScale = 1.0f;

        MoveScale += SimulationRate * Time.deltaTime;

        float moveInfluence = Acceleration * 0.1f * MoveScale * MoveScaleMultiplier;

        // TODO 次はここの理解から
        Quaternion ort = OVRPlayerControllerGameObject.transform.rotation;
        Vector3 ortEuler = ort.eulerAngles;
        ortEuler.z = ortEuler.x = 0f;
        ort = Quaternion.Euler(ortEuler);


    }

    private bool DetectHandShakeWalk()
    {
        // adjust move amount based on shaking speed
        float handShakeSpeed = Math.Max(touchVelocityL.y, touchVelocityR.y);
        MoveScaleMultiplier = handShakeSpeed;
        if (handShakeSpeed > MIN_WALKSPEED && handShakeSpeed < MAX_WALKSPEED)
            return true;
        else
            return false;
    }

    private void Update()
    {
        moveDirection = Vector3.zero;

        //Debug.Log("R-touch Y-velocity: " + touchVelocityR.y);
        //Debug.Log("ABS(R-touch Y-velocity): " + Math.Abs(touchVelocityR.y));

        bool isWalkMotion =
            (touchVelocityL.y > 1.0f && touchVelocityL.y < 2.5f)
            || (touchVelocityR.y > 1.0f && touchVelocityR.y < 2.5f);

        bool isJumpMotion =
            touchVelocityL.y > 2.5f || touchVelocityR.y > 2.5f;

        Quaternion ort = OVRPlayerControllerGameObject.transform.rotation;
        Vector3 ortEuler = ort.eulerAngles;
        ortEuler.z = ortEuler.x = 0f;
        ort = Quaternion.Euler(ortEuler);

        if (isWalkMotion) // Input.GetKey(KeyCode.A)
        {
            Debug.Log("walking");
            moveDirection += ort * (OVRPlayerControllerGameObject.transform.lossyScale.z * Vector3.forward);
        }

        // todo 現状のコードではジャンプできていない。
        if (isJumpMotion)
        {
            Debug.Log("jumping");
            moveDirection.y = 1.0f;
        }

        Controller.Move(moveDirection * SimulationRate * Time.deltaTime);
    }

}

// UnityEngine.CharacterController.Move()
// https://docs.unity3d.com/ScriptReference/CharacterController.Move.html

// OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch)
// OVRInput.GetLocalControllerAcceleration(OVRInput.Controller.LTouch)