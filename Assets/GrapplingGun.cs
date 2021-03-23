using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3 grapplePoint;
    private SpringJoint joint;

    [SerializeField] private GameObject OVRPlayerControllerGameObject = null;
    [SerializeField] private Transform leftHandAnchor = null;
    [SerializeField] private LayerMask grappaleLayer;
    [SerializeField] private float wireMaxLength = 100.0f;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.LHandTrigger) || Input.GetKeyDown(KeyCode.A))
        {
            StartGrapple();
        }
        else if (OVRInput.GetUp(OVRInput.RawButton.LHandTrigger) || Input.GetKeyUp(KeyCode.A))
        {
            StopGrapple();
        }
    }

    private void LateUpdate()
    {
        DrawWire();
    }

    private void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(leftHandAnchor.position, leftHandAnchor.forward, out hit, wireMaxLength, grappaleLayer))
        {
            grapplePoint = hit.point;
            joint = OVRPlayerControllerGameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceToPoint = Vector3.Distance(OVRPlayerControllerGameObject.transform.position, grapplePoint);

            joint.maxDistance = distanceToPoint * 0.8f;
            joint.minDistance = distanceToPoint * 0.25f;

            joint.spring = 4.5f;
            joint.damper = 7.0f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
        }
    }

    private void DrawWire()
    {
        if (!joint) return;
        lr.SetPosition(0, leftHandAnchor.position);
        lr.SetPosition(1, grapplePoint);
    }

    private void StopGrapple()
    {
        lr.positionCount = 0;
        Destroy(joint);
    }

}
