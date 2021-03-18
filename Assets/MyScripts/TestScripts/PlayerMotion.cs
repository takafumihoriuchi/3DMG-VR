using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotion : MonoBehaviour
{
    [SerializeField] private GameObject OVRPlayerControllerGameObject = null;
    [SerializeField] private Transform LeftHandAnchorTransform = null;
    [SerializeField] private Transform RightHandAnchorTransform = null;
    private OVRPlayerController OVRPlayerControllerComponent;

    // identical to fields of OVRPlayerController class
    private CharacterController Controller;
    private Vector3 MoveThrottle = Vector3.zero;
    private float MoveScale = 1.0f;
    private float MoveScaleMultiplier = 1.0f;
    private float SimulationRate = 60f;
    private float FallSpeed = 0.0f;
    private float Acceleration;
    private float Damping;
    private float GravityModifier;
    private float JumpForce;

    // original fields for this script
    private Vector3 touchVelocityL;
    private Vector3 touchVelocityR;
    private Vector3 touchAccelerationL;
    private Vector3 touchAccelerationR;

    const float MIN_WALKSPEED = 0.5f;
    const float MAX_WALKSPEED = 1.2f;
    const float MIN_JUMPSPEED = 1.9f;


    private void Awake()
    {
        Controller
            = OVRPlayerControllerGameObject.GetComponent<CharacterController>();
        OVRPlayerControllerComponent
            = Controller.GetComponent<OVRPlayerController>();
    }


    private void Start()
    {
        // store public fields of OVRPlayerController-class to local private fileds
        Acceleration = OVRPlayerControllerComponent.Acceleration;
        Damping = OVRPlayerControllerComponent.Damping;
        GravityModifier = OVRPlayerControllerComponent.GravityModifier;
        JumpForce = OVRPlayerControllerComponent.JumpForce;

        // pre-setting for overriding character-control
        OVRPlayerControllerComponent.PreCharacterMove
            += () => CharacterMoveByHandShake();
        OVRPlayerControllerComponent.EnableLinearMovement = false;

        // necessary for initial grounded-evaluation
        Controller.Move(Vector3.zero * Time.deltaTime);
    }


    private void Update() { }


    private void CharacterMoveByHandShake()
    {
        HandShakeControler();
        UpdateController();

        // display for development purpose
        Debug.Log("L-touch velocity: " + touchVelocityL);
        Debug.Log("R-touch velocity: " + touchVelocityR);
    }


    private void HandShakeControler()
    {
        touchVelocityL
            = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);
        touchVelocityR
            = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
        touchAccelerationL
            = OVRInput.GetLocalControllerAcceleration(OVRInput.Controller.LTouch);
        touchAccelerationR
            = OVRInput.GetLocalControllerAcceleration(OVRInput.Controller.RTouch);

        if (!IsGrounded()) MoveScale = 0.0f;
        else MoveScale = 1.0f;

        MoveScale *= SimulationRate * Time.deltaTime;

        float moveInfluence
            = Acceleration * 0.1f * MoveScale * MoveScaleMultiplier;

        Transform activeHand;
        Vector3 handShakeVel;
        Vector3 handShakeAcc;

        if (Math.Abs(touchVelocityL.y) > Math.Abs(touchVelocityR.y))
        {
            activeHand = LeftHandAnchorTransform;
            handShakeVel = touchVelocityL;
            handShakeAcc = touchAccelerationL;
        }
        else
        {
            activeHand = RightHandAnchorTransform;
            handShakeVel = touchVelocityR;
            handShakeAcc = touchAccelerationR;
        }

        Quaternion ort = activeHand.rotation;
        Vector3 ortEuler = ort.eulerAngles;
        ortEuler.z = ortEuler.x = 0f;
        ort = Quaternion.Euler(ortEuler);

        MoveThrottle += CalculateMoveEffect(moveInfluence, ort, handShakeVel, handShakeAcc);
    }


    private Vector3 CalculateMoveEffect(float moveInfluence,
        Quaternion ort, Vector3 handShakeVel, Vector3 handShakeAcc)
    {
        Vector3 tmpMoveThrottle = Vector3.zero;

        bool isWalk = DetectHandShakeWalk(Math.Abs(handShakeVel.y));
        bool isRun = DetectHandShakeRun(Math.Abs(handShakeVel.y));
        bool isJump = DetectHandShakeJump(handShakeVel.y);

        if (isRun)
        {
            tmpMoveThrottle += ort
                * (OVRPlayerControllerGameObject.transform.lossyScale.z
                * moveInfluence * Vector3.forward)
                * 1.2f;
        }
        else if (isWalk)
        {
            tmpMoveThrottle += ort
                * (OVRPlayerControllerGameObject.transform.lossyScale.z
                * moveInfluence * Vector3.forward)
                * 1.0f;
        }

        if (isJump)
        {
            Vector3 tmpVec3 = handShakeVel;
            tmpVec3.x *= -0.5f;
            tmpVec3.y = handShakeVel.y * JumpForce + 1.0f;
            tmpVec3.z *= -0.5f;
            tmpMoveThrottle += tmpVec3; // todo これで良いか要確認
        }

        return tmpMoveThrottle;
    }


    private bool DetectHandShakeWalk(float speed)
    {
        if (!IsGrounded())
            return false;
        if (speed > MIN_WALKSPEED && speed < MAX_WALKSPEED)
            return true;
        else
            return false;
    }


    private bool DetectHandShakeRun(float speed)
    {
        if (!IsGrounded())
            return false;
        if (speed > MAX_WALKSPEED && speed < MIN_JUMPSPEED)
            return true;
        else
            return false;
    }


    private bool DetectHandShakeJump(float speed)
    {
        if (!IsGrounded())
            return false;
        if (speed > MIN_JUMPSPEED) // only in positive direction
            return true;
        return false;
    }


    private bool IsGrounded()
    {
        if (Controller.isGrounded) return true;

        var pos = OVRPlayerControllerGameObject.transform.position;
        var ray = new Ray(pos + Vector3.up * 0.1f, Vector3.down);
        var tolerance = 0.3f;
        return Physics.Raycast(ray, tolerance);
    }


    private void UpdateController()
    {
        Vector3 moveDirection = Vector3.zero;

        float motorDamp = 1.0f + (Damping * SimulationRate * Time.deltaTime);

        MoveThrottle.x /= motorDamp;
        MoveThrottle.y = (MoveThrottle.y > 0.0f) ?
            (MoveThrottle.y / motorDamp) : MoveThrottle.y;
        MoveThrottle.z /= motorDamp;

        moveDirection += MoveThrottle * SimulationRate * Time.deltaTime;

        // calculate gravity influence
        if (Controller.isGrounded && FallSpeed <= 0)
            FallSpeed = Physics.gravity.y * (GravityModifier * 0.002f);
        else
            FallSpeed += Physics.gravity.y
                * (GravityModifier * 0.002f) * SimulationRate * Time.deltaTime;

        moveDirection.y += FallSpeed * SimulationRate * Time.deltaTime;

        if (Controller.isGrounded && MoveThrottle.y
            <= OVRPlayerControllerGameObject.transform.lossyScale.y * 0.001f)
        {
            // offset correction for uneven ground
            float bumpUpOffset
                = Mathf.Max(
                    Controller.stepOffset,
                    new Vector3(moveDirection.x, 0, moveDirection.z).magnitude);
            moveDirection -= bumpUpOffset * Vector3.up;
        }

        Vector3 predictedXZ
            = Vector3.Scale(
                Controller.transform.localPosition + moveDirection,
                new Vector3(1, 0, 1));

        // update character position
        Controller.Move(moveDirection);

        Vector3 actualXZ
            = Vector3.Scale(
                Controller.transform.localPosition,
                new Vector3(1, 0, 1));

        if (predictedXZ != actualXZ)
            MoveThrottle += (actualXZ - predictedXZ)
                / (SimulationRate * Time.deltaTime);
    }


}