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

    [SerializeField] private float shotSpeed = 2500.0f;
    [SerializeField] private int shotCount = 9999;
    [SerializeField] private float shotInterval = 0.35f;

    private AudioSource wireReleaseAudio;
    private AudioSource wireRewindAudio;
    private AudioSource gunShotAudio;

    private bool bulletReadyL = true;
    private bool bulletReadyR = true;

    void Start()
    {
        wireReleaseAudio = gameObject.AddComponent<AudioSource>();
        wireReleaseAudio.clip = wireRelease;
        wireRewindAudio = gameObject.AddComponent<AudioSource>();
        wireRewindAudio.clip = wireRewind;
        gunShotAudio = gameObject.AddComponent<AudioSource>();
        gunShotAudio.clip = gunShot;

        bullet.SetActive(false);
    }

    
    void Update()
    {
        Debug.Log("bulletReadyL: " + bulletReadyL);

        if (bulletReadyL &&
            (OVRInput.Get(OVRInput.RawButton.LIndexTrigger) || Input.GetKey(KeyCode.L)))
        {
            //Quaternion ort = leftHandAnchor.rotation;
            //Vector3 ortEuler = ort.eulerAngles;
            //ortEuler.z = ortEuler.x = 0f;
            //ort = Quaternion.Euler(ortEuler);

            //GameObject bulletInstance = Instantiate(bullet, leftHandAnchor.position, ort);

            GameObject bulletInstance = Instantiate(bullet, leftHandAnchor.position, leftHandAnchor.rotation);

            Rigidbody bulletRigidBody = bulletInstance.GetComponent<Rigidbody>();
            bulletInstance.SetActive(true);
            bulletRigidBody.AddForce(leftHandAnchor.transform.forward * shotSpeed);
            gunShotAudio.Play();

            shotCount--;
            StartCoroutine(SetBulletReloadInterval());

            Destroy(bulletInstance, 3.0f);
        }

    }

    IEnumerator SetBulletReloadInterval()
    {
        bulletReadyL = false;
        yield return new WaitForSecondsRealtime(shotInterval);
        bulletReadyL = true;
    }

}
