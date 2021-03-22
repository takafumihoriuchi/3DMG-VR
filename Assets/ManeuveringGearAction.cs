using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManeuveringGearAction : MonoBehaviour
{
    [SerializeField] private GameObject bullet = null;

    [SerializeField] private Transform leftHandAnchor = null;
    [SerializeField] private Transform rightHandAnchor = null;

    [SerializeField] private AudioClip wireRelease = null;
    [SerializeField] private AudioClip wireRewind = null;
    [SerializeField] private AudioClip gunShot = null;

    [SerializeField] private GameObject fireBulletGionCanvas = null;

    [SerializeField] private float shotSpeed = 4000.0f;
    [SerializeField] private int shotCount = 9999;
    [SerializeField] private float shotInterval = 0.25f;

    private AudioSource wireReleaseAudio;
    private AudioSource wireRewindAudio;

    private bool bulletReadyL = true;
    private bool bulletReadyR = true;
    private const int LEFT = 0;
    private const int RIGHT = 1;
    private bool[] bulletReady = new bool[2] {true, true};

    void Start()
    {
        wireReleaseAudio = gameObject.AddComponent<AudioSource>();
        wireReleaseAudio.clip = wireRelease;
        wireRewindAudio = gameObject.AddComponent<AudioSource>();
        wireRewindAudio.clip = wireRewind;

        bullet.SetActive(false);
        fireBulletGionCanvas.SetActive(false);
    }

    
    void Update()
    {
        MonitorFireBullet(LEFT, leftHandAnchor,
            OVRInput.Get(OVRInput.RawButton.LIndexTrigger), Input.GetKey(KeyCode.L));
        MonitorFireBullet(RIGHT, rightHandAnchor,
            OVRInput.Get(OVRInput.RawButton.RIndexTrigger), Input.GetKey(KeyCode.R));
    }


    private void MonitorFireBullet(int SIDE, Transform handAnchor, bool isTriggered, bool isKeyPressed)
    {
        if (bulletReady[SIDE] && (isTriggered || isKeyPressed))
        {
            GameObject bulletInstance = Instantiate(bullet, handAnchor.position, handAnchor.rotation);
            bulletInstance.SetActive(true);

            Rigidbody bulletRigidBody = bulletInstance.GetComponent<Rigidbody>();
            bulletRigidBody.AddForce(handAnchor.transform.forward * shotSpeed);

            // sound effect
            AudioSource gunShotAudio = bulletInstance.AddComponent<AudioSource>();
            gunShotAudio.clip = gunShot;
            gunShotAudio.Play();

            // visual sound effect (gion-go)
            System.Random rand = new System.Random();
            Vector3 fireGionPos = handAnchor.position + handAnchor.forward;
            fireGionPos.x += rand.Next(-1, 1) / 5.0f;
            fireGionPos.y += rand.Next(-1, 1) / 5.0f;
            fireGionPos.z += rand.Next(-1, 1) / 5.0f;
            Quaternion fireGionRot = handAnchor.rotation;
            fireGionRot.x = 0.0f;
            fireGionRot.z += rand.Next(-1, 1) / 5.0f;
            GameObject fireGionInstance = Instantiate(fireBulletGionCanvas, fireGionPos, fireGionRot);
            fireGionInstance.SetActive(true);

            shotCount--;
            StartCoroutine(SetBulletReloadInterval(SIDE));

            Destroy(bulletInstance, 3.0f);
            Destroy(fireGionInstance, 1.0f);
        }
    }

    IEnumerator SetBulletReloadInterval(int SIDE)
    {
        bulletReady[SIDE] = false;
        yield return new WaitForSecondsRealtime(shotInterval);
        bulletReady[SIDE] = true;
    }

}
