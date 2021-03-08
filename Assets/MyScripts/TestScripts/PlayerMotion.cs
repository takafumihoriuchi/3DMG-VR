using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotion : MonoBehaviour
{
    //[Tooltip("put help-text here")]
    //[SerializeField] GameObject someGameObject = null;

    [SerializeField] GameObject OVRPlayerController = null;
    CharacterController Controller = null;
    Vector3 moveDirection = Vector3.zero;

    void Start()
    {
        Controller = OVRPlayerController.GetComponent<CharacterController>();
        moveDirection.z = 1.0f;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            Controller.Move(moveDirection);
        }
    }

}
