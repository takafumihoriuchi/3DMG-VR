using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWireAction : MonoBehaviour
{
    [SerializeField] private GameObject playerGameObject = null;
    [SerializeField] private Transform leftHandAnchor = null;
    [SerializeField] private Transform rightHandAnchor = null;
    [SerializeField] private GameObject wire = null;

    [SerializeField] private AudioClip wireRelease = null;
    [SerializeField] private AudioClip wireRewind = null;

    [SerializeField] private LayerMask grappleableLayer;
    [SerializeField] private float wireMaxLength = 999.0f;

    [SerializeField] private float spring = 4.5f;
    [SerializeField] private float damper = 7.0f;
    [SerializeField] private float massScale = 4.5f;
    [SerializeField] private float maxDistRate = 0.0f; // 0.8f
    [SerializeField] private float minDistRate = 0.0f; // 0.25f

    private const int LEFT = 0;
    private const int RIGHT = 1;
    private Transform[] handAnchor = new Transform[2];
    private GameObject[] wireInstance = new GameObject[2];
    private LineRenderer[] lineRend = new LineRenderer[2];
    private SpringJoint[] joint = new SpringJoint[2];
    private Vector3[] grapplePoint = new Vector3[2];

    private AudioSource[] wireReleaseAudio = new AudioSource[2];
    private AudioSource[] wireRewindAudio = new AudioSource[2];


    void Start()
    {
        handAnchor[LEFT] = leftHandAnchor;
        handAnchor[RIGHT] = rightHandAnchor;

        wire.SetActive(false);
        WireSetUp(LEFT);
        WireSetUp(RIGHT);
    }


    void Update()
    {
        MonitorWireAction(LEFT, OVRInput.RawButton.LHandTrigger, KeyCode.A);
        MonitorWireAction(RIGHT, OVRInput.RawButton.RHandTrigger, KeyCode.D);
    }


    void LateUpdate()
    {
        RenderWire(LEFT);
        RenderWire(RIGHT);
    }


    private void MonitorWireAction(int SIDE, OVRInput.RawButton rawMask, KeyCode key)
    {
        if (OVRInput.GetDown(rawMask) || Input.GetKeyDown(key))
        {
            if (wireRewindAudio[SIDE].isPlaying) wireRewindAudio[SIDE].Stop();
            wireReleaseAudio[SIDE].Play();
            StartGrapple(SIDE);
        }
        else if (OVRInput.GetUp(rawMask) || Input.GetKeyUp(key))
        {
            if (wireReleaseAudio[SIDE].isPlaying) wireReleaseAudio[SIDE].Stop();
            wireRewindAudio[SIDE].Play();
            StopGrapple(SIDE);
        }
    }


    private void StartGrapple(int SIDE)
    {
        RaycastHit hit;
        if (Physics.Raycast(handAnchor[SIDE].position,
            handAnchor[SIDE].forward, out hit, wireMaxLength, grappleableLayer)) // todo "grappable"ではなく、除外するレイヤーでは？
        {
            wireInstance[SIDE].SetActive(true);

            // todo 無限遠にアタッチされて吹き飛ばされてる？
            Debug.Log("handAnchor[SIDE].position: " + handAnchor[SIDE].position);
            Debug.Log("handAnchor[SIDE].forward: " + handAnchor[SIDE].forward);
            Debug.Log("hit.point: " + hit.point);
            // => hit.point は正しく検知されている
            // todo それでも無限遠（z軸の反対方向に）に飛ばされてしまう理由は何？
            grapplePoint[SIDE] = hit.point;

            joint[SIDE] = playerGameObject.AddComponent<SpringJoint>();
            joint[SIDE].autoConfigureConnectedAnchor = false;
            joint[SIDE].connectedAnchor = grapplePoint[SIDE];
            //joint[SIDE].connectedAnchor = new Vector3(0.0f, 1.0f, 4.0f);

            float distanceToPoint = Vector3.Distance(playerGameObject.transform.position, grapplePoint[SIDE]);

            joint[SIDE].maxDistance = distanceToPoint * maxDistRate;
            joint[SIDE].minDistance = distanceToPoint * minDistRate;

            joint[SIDE].spring = spring;
            joint[SIDE].damper = damper;
            joint[SIDE].massScale = massScale;

            lineRend[SIDE].positionCount = 2;
        }
        else
        {
            Debug.Log("did not hit");
        }
    }


    private void StopGrapple(int SIDE)
    {
        lineRend[SIDE].positionCount = 0;
        Destroy(joint[SIDE]);
        wireInstance[SIDE].SetActive(false);
    }


    private void RenderWire(int SIDE)
    {
        if (!joint[SIDE]) return;
        lineRend[SIDE].SetPosition(0, handAnchor[SIDE].position);
        lineRend[SIDE].SetPosition(1, grapplePoint[SIDE]);
    }


    private void WireSetUp(int SIDE)
    {
        wireInstance[SIDE] = Instantiate(wire, handAnchor[SIDE]);

        wireReleaseAudio[SIDE] = wireInstance[SIDE].AddComponent<AudioSource>();
        wireReleaseAudio[SIDE].clip = wireRelease;

        wireRewindAudio[SIDE] = wireInstance[SIDE].AddComponent<AudioSource>();
        wireRewindAudio[SIDE].clip = wireRewind;

        lineRend[SIDE] = wireInstance[SIDE].GetComponent<LineRenderer>();
    }


}
