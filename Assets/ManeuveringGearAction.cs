using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManeuveringGearAction : MonoBehaviour
{
    [SerializeField] private GameObject bullet = null;
    [SerializeField] private Transform leftHandAnchor;
    [SerializeField] private Transform rightHandAnchor;

    [SerializeField] private float shotSpeed = 2500.0f;
    [SerializeField] private int shotCount = 9999;
    [SerializeField] private float shotInterval = 1.0f;

    private bool bulletReadyL = true;
    private bool bulletReadyR = true;

    void Start()
    {
        bullet.SetActive(false);
    }

    
    void Update()
    {
        Debug.Log("bulletReadyL: " + bulletReadyL);

        if (bulletReadyL &&
            (OVRInput.Get(OVRInput.RawButton.LIndexTrigger) || Input.GetKey(KeyCode.L)))
        {
            Quaternion ort = leftHandAnchor.rotation;
            Vector3 ortEuler = ort.eulerAngles;
            ortEuler.z = ortEuler.x = 0f;
            ort = Quaternion.Euler(ortEuler);

            GameObject bulletInstance = Instantiate(bullet, leftHandAnchor.position, ort);
            Rigidbody bulletRigidBody = bulletInstance.GetComponent<Rigidbody>();
            bulletInstance.SetActive(true);
            bulletRigidBody.AddForce(transform.forward * shotSpeed);

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
