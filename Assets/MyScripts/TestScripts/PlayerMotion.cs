using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotion : MonoBehaviour
{
    //[Tooltip("put help-text here")]
    //[SerializeField] GameObject someGameObject = null;

    [SerializeField] GameObject OVRPlayerControllerGameObject = null;
    CharacterController characterControllerComponent;
    OVRPlayerController OVRPlayerControllerComponent;
    Vector3 moveDirection = Vector3.zero;
    Vector3 playerVelocity = Vector3.zero;

    Vector3 touchVelocityL;
    Vector3 touchVelocityR;
    Vector3 touchAccelerationL;
    Vector3 touchAccelerationR;


    void Start()
    {
        characterControllerComponent = OVRPlayerControllerGameObject.GetComponent<CharacterController>();
        OVRPlayerControllerComponent = OVRPlayerControllerGameObject.GetComponent<OVRPlayerController>();

        touchVelocityL = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);
        touchVelocityR = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
        touchAccelerationL = OVRInput.GetLocalControllerAcceleration(OVRInput.Controller.LTouch);
        touchAccelerationR = OVRInput.GetLocalControllerAcceleration(OVRInput.Controller.RTouch);

        characterControllerComponent.Move(moveDirection * Time.deltaTime);
    }

    void Update()
    {
        moveDirection = Vector3.zero;

        Debug.Log("ABS(R-touch Y-velocity): " + Math.Abs(touchVelocityL.y));

        bool isWalkMotion =
            (1.0f < Math.Abs(touchVelocityL.y) && Math.Abs(touchVelocityL.y) < 2.5f)
            || (1.0f < Math.Abs(touchVelocityR.y) && Math.Abs(touchVelocityR.y) < 2.5f);

        bool isJumpMotion =
            2.5f < Math.Abs(touchVelocityL.y)
            || 2.5f < Math.Abs(touchVelocityR.y);

        if (isWalkMotion)
        {
            Debug.Log("walking");
            moveDirection.z = 1.0f;
        }
        if (isJumpMotion)
        {
            Debug.Log("jumping");
            moveDirection.y = 1.0f;
        }

        characterControllerComponent.Move(moveDirection * Time.deltaTime);

        //if (Input.GetKey(KeyCode.A))
        //{
        //    if (characterControllerComponent.isGrounded) {
        //        moveDirection.y = 1.0f;
        //        characterControllerComponent.Move(moveDirection * Time.deltaTime);
        //        moveDirection.y = -1.0f;
        //    }
        //}
    }

}

// UnityEngine.CharacterController.Move()
// https://docs.unity3d.com/ScriptReference/CharacterController.Move.html

// OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch)
// OVRInput.GetLocalControllerAcceleration(OVRInput.Controller.LTouch)