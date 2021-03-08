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

        characterControllerComponent.Move(moveDirection * Time.deltaTime);
    }

    void Update()
    {
        moveDirection = Vector3.zero;

        touchVelocityL = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);
        touchVelocityR = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
        touchAccelerationL = OVRInput.GetLocalControllerAcceleration(OVRInput.Controller.LTouch);
        touchAccelerationR = OVRInput.GetLocalControllerAcceleration(OVRInput.Controller.RTouch);

        Debug.Log("R-touch Y-velocity: " + touchVelocityR.y);
        Debug.Log("ABS(R-touch Y-velocity): " + Math.Abs(touchVelocityR.y));

        bool isWalkMotion =
            (1.0f < Math.Abs(touchVelocityL.y) && Math.Abs(touchVelocityL.y) < 2.5f)
            || (1.0f < Math.Abs(touchVelocityR.y) && Math.Abs(touchVelocityR.y) < 2.5f);

        bool isJumpMotion =
            2.5f < Math.Abs(touchVelocityL.y) || 2.5f < Math.Abs(touchVelocityR.y);

        // todo 現状のコードではデフォルトのz軸方向に進んでしまう。forward方向に進んで欲しい。
        if (isWalkMotion)
        {
            Debug.Log("walking");
            moveDirection.z = 1.0f;
        }

        // todo 現状のコードではジャンプできていない。
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