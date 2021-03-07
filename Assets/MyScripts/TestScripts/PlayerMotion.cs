using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotion : MonoBehaviour
{
    //[Tooltip("Object representing 3D model of left controller")]
    //[SerializeField] GameObject leftControllerObj = null;

    //[Tooltip("Object representing 3D model of right controller")]
    //[SerializeField] GameObject rightControllerObj = null;

    void Start()
    {
        
    }

    void Update()
    {
        OnControllerButtonPress();
    }

    private void OnControllerButtonPress()
    {
        //// left menu button
        //if (OVRInput.GetDown(OVRInput.RawButton.Start))
        //{
        //    leftControllerObj.SetActive(!leftControllerObj.activeSelf);
        //    rightControllerObj.SetActive(!rightControllerObj.activeSelf);
        //}
    }

}
