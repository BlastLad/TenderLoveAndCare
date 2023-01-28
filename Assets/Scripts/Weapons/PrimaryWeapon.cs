using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PrimaryWeapon : MonoBehaviour
{

    [SerializeField]
    TutorialMyPlayer3 controls;

    [SerializeField]
    public InputAction fireButton;

    [SerializeField]
    GameObject bullet;

    [SerializeField]
    float shootForce = 30;

    public float timeBetweenShots, timeBetweenShooting, spread;

    bool shooting;

    private LineRenderer vine;
    private Vector3 grapplePoint;
    public LayerMask hitMask;

    public Transform firePoint;
    public Transform currentCamera;

    float maxRange = 100;

    bool mouseDown = false;
    bool readyToShoot = true;
    bool allowInvoke = true;

    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (fireButton.triggered)
        {
            Debug.Log("FIRED YES");
            Shoot();
        }
    }

    void Shoot()
    {
        Debug.Log("FIRED YES 2");
        Ray ray = currentCamera.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit, maxRange, hitMask))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(75);
        }

        Vector3 directionDefault = targetPoint - firePoint.position;

        /*GameObject currentBullet = Instantiate(bullet, firePoint.position, Quaternion.identity);
        currentBullet.transform.forward = directionDefault.normalized;

        currentBullet.GetComponent<Rigidbody>().AddForce(directionDefault.normalized * shootForce, ForceMode.Impulse);*/
        //currentBullet.GetComponent<Rigidbody>().AddForce(.normalized * shootForce, ForceMode.Impulse);

        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
            readyToShoot = false;
        }
    }

    public void ResetShot()
    {
        allowInvoke = true;
        readyToShoot = true;
    }

    private void OnEnable()
    {
        fireButton.Enable();
    }

    private void OnDisable()
    {
        fireButton.Disable();
    }
}
