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


    void Start()
    {
        characterControllerComponent = OVRPlayerControllerGameObject.GetComponent<CharacterController>();
        OVRPlayerControllerComponent = OVRPlayerControllerGameObject.GetComponent<OVRPlayerController>();

        //moveDirection.y = 0.2f; // todo これをコントローラーの加速度で調整する
    }

    void Update()
    {
        characterControllerComponent.Move(moveDirection * Time.deltaTime);
        Debug.Log(characterControllerComponent.isGrounded);
        if (Input.GetKey(KeyCode.A))
        {
            if (characterControllerComponent.isGrounded) {
                moveDirection.y = 1.0f;
                characterControllerComponent.Move(moveDirection * Time.deltaTime);
                moveDirection.y = 0.0f;
            }
        }
    }

}

// UnityEngine.CharacterController.Move()
// https://docs.unity3d.com/ScriptReference/CharacterController.Move.html

// OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch)
// OVRInput.GetLocalControllerAcceleration(OVRInput.Controller.LTouch)