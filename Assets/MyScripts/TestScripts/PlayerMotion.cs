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
    private float SimulationRate = 60f; // todo make it 90fps (adjust the value of SimulationRate?)
    private float FallSpeed = 0.0f;
    private float Acceleration;
    private float Damping;
    private float GravityModifier;

    private Vector3 touchVelocityL;
    private Vector3 touchVelocityR;
    private Vector3 touchAccelerationL;
    private Vector3 touchAccelerationR;
    private float handShakeSpeed;

    const float MIN_WALKSPEED = 1.0f;
    const float MAX_WALKSPEED = 2.5f;


    private void Awake()
    {
        Controller
            = OVRPlayerControllerGameObject.GetComponent<CharacterController>();
        OVRPlayerControllerComponent
            = Controller.GetComponent<OVRPlayerController>();
    }

    private void Start()
    {
        Acceleration = OVRPlayerControllerComponent.Acceleration;
        Damping = OVRPlayerControllerComponent.Damping;
        GravityModifier = OVRPlayerControllerComponent.GravityModifier;
        Controller.Move(Vector3.zero * Time.deltaTime);
    }

    private void UpdateHandShakeController()
    {
        touchVelocityL
            = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);
        touchVelocityR
            = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
        touchAccelerationL
            = OVRInput.GetLocalControllerAcceleration(OVRInput.Controller.LTouch);
        touchAccelerationR
            = OVRInput.GetLocalControllerAcceleration(OVRInput.Controller.RTouch);
        handShakeSpeed = Math.Max(
            Math.Abs(GetMaxElementVector3(touchVelocityL)),
            Math.Abs(GetMaxElementVector3(touchVelocityR)));
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

        float moveInfluence
            = Acceleration * 0.1f * MoveScale * MoveScaleMultiplier;

        Quaternion ort = OVRPlayerControllerGameObject.transform.rotation;
        Vector3 ortEuler = ort.eulerAngles;
        ortEuler.z = ortEuler.x = 0f;
        ort = Quaternion.Euler(ortEuler);

        if (moveForward)
            MoveThrottle += ort
                * (OVRPlayerControllerGameObject.transform.lossyScale.z
                * moveInfluence * Vector3.forward)
                * (1.0f + handShakeSpeed);
    }

    private bool DetectHandShakeWalk()
    {
        if (handShakeSpeed > MIN_WALKSPEED && handShakeSpeed < MAX_WALKSPEED)
            return true;
        else
            return false;
    }

    private void Update()
    {
        UpdateHandShakeController();
        Debug.Log("L-touch velocity: " + touchVelocityL);
        Debug.Log("R-touch velocity: " + touchVelocityR);

        Vector3 moveDirection = Vector3.zero;

        float motorDamp = 1.0f + (Damping * SimulationRate * Time.deltaTime);

        MoveThrottle.x /= motorDamp;
        MoveThrottle.y = (MoveThrottle.y > 0.0f) ? (MoveThrottle.y / motorDamp) : MoveThrottle.y;
        MoveThrottle.z /= motorDamp;

        moveDirection += MoveThrottle * SimulationRate * Time.deltaTime;

        // gravity
        if (Controller.isGrounded && FallSpeed <= 0)
            FallSpeed = Physics.gravity.y * (GravityModifier * 0.002f);
        else
            FallSpeed += Physics.gravity.y
                * (GravityModifier * 0.002f) * SimulationRate * Time.deltaTime;

        moveDirection.y += FallSpeed * SimulationRate * Time.deltaTime;

        if (Controller.isGrounded && MoveThrottle.y <= transform.lossyScale.y * 0.001f)
        {
            // Offset correction for uneven ground
            float bumpUpOffset
                = Mathf.Max(
                    Controller.stepOffset,
                    new Vector3(moveDirection.x, 0, moveDirection.z).magnitude);
            moveDirection -= bumpUpOffset * Vector3.up;
        }

        // todo PreCharacterMove()を使用することで全て代替可能か

        Vector3 predictedXZ = Vector3.Scale((Controller.transform.localPosition + moveDirection), new Vector3(1, 0, 1));

        Controller.Move(moveDirection);

        Vector3 actualXZ = Vector3.Scale(Controller.transform.localPosition, new Vector3(1, 0, 1));

        if (predictedXZ != actualXZ)
            MoveThrottle += (actualXZ - predictedXZ) / (SimulationRate * Time.deltaTime);

        //Debug.Log("R-touch Y-velocity: " + touchVelocityR.y);
        //Debug.Log("ABS(R-touch Y-velocity): " + Math.Abs(touchVelocityR.y));

        //bool isWalkMotion =
        //    (touchVelocityL.y > 1.0f && touchVelocityL.y < 2.5f)
        //    || (touchVelocityR.y > 1.0f && touchVelocityR.y < 2.5f);

        //bool isJumpMotion =
        //    touchVelocityL.y > 2.5f || touchVelocityR.y > 2.5f;

        //Quaternion ort = OVRPlayerControllerGameObject.transform.rotation;
        //Vector3 ortEuler = ort.eulerAngles;
        //ortEuler.z = ortEuler.x = 0f;
        //ort = Quaternion.Euler(ortEuler);

        //if (isWalkMotion) // Input.GetKey(KeyCode.A)
        //{
        //    Debug.Log("walking");
        //    moveDirection += ort * (OVRPlayerControllerGameObject.transform.lossyScale.z * Vector3.forward);
        //}

        //// todo 現状のコードではジャンプできていない。
        //if (isJumpMotion)
        //{
        //    Debug.Log("jumping");
        //    moveDirection.y = 1.0f;
        //}

        //Controller.Move(moveDirection * SimulationRate * Time.deltaTime);
    }

    private float GetMaxElementVector3(Vector3 vec)
    {
        return Math.Max(Math.Max(vec.x, vec.y), vec.z);
    }

}
