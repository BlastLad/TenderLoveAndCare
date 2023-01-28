using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class VineGrapple : MonoBehaviour
{
    [SerializeField]
    public InputAction fireButton;

    private LineRenderer vine;
    private Vector3 grapplePoint;
    public LayerMask mask;

    public Transform firePoint;
    public Transform currentCamera, player;

    private SpringJoint joint;

    float maxRange = 100;

    bool mouseDown = false;

    private void Awake()
    {
        vine = GetComponent<LineRenderer>();


        fireButton.performed += ctx =>
        {
            mouseDown = true;
            Grapple();
        };
        fireButton.canceled += ctx =>
        {
            mouseDown = false;
            EndGrapple();
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Grapple()
    {
        RaycastHit hit;
        Debug.Log("FIRED YES");
        if (Physics.Raycast(currentCamera.position, currentCamera.forward, out hit, maxRange, mask))
        {
            Debug.Log("Reached yes");
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distance = Vector3.Distance(player.position, grapplePoint);

            joint.maxDistance = distance * 0.8f;
            joint.minDistance = distance * .25f;

            joint.spring = 4.5f;//more pul and push
            joint.damper = 7f;
            joint.massScale = 4.5f;
        }
    }

    void EndGrapple()
    {

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
