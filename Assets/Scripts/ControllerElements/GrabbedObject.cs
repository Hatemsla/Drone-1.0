using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Drone
{
    public class GrabbedObject : MonoBehaviour
    {
        private bool isObjectGrabbed = false;
        private GameObject grabbedObject;
        private Rigidbody grabbedObjectRigidbody;
        private FixedJoint joint;
        public float raycastDistance = 5f;
        private Ray ray;
        // public Transform target;
        

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {

            ray = new Ray(transform.position, Vector3.up);
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.green);


            if (Input.GetKeyDown(KeyCode.F))
            {
                if (!isObjectGrabbed)
                {
                    TryGrabObject();
                }
                else
                {
                    ReleaseObject();
                }
            }
        
        }

        private void TryGrabObject()
        {
            Debug.Log("TryGrab");
            RaycastHit hit;
            Debug.Log(Physics.Raycast(ray, out hit, raycastDistance));            

            if (Physics.Raycast(ray, out hit, raycastDistance))
            {
                if (hit.collider.GetComponentInParent<DroneController>())//if (hit.collider.CompareTag("Player"))//
                {
                    Debug.Log("Grabbed");
                    grabbedObject = hit.collider.gameObject;
                    grabbedObjectRigidbody = grabbedObject.GetComponent<Rigidbody>();

                    joint = gameObject.AddComponent<FixedJoint>();
                    joint.connectedBody = grabbedObjectRigidbody;
                    
                    isObjectGrabbed = true;


                }
            }
        }

        private void ReleaseObject()
        {
            if (joint != null)
            {
                Destroy(joint);
                grabbedObject = null;
                grabbedObjectRigidbody = null;
                isObjectGrabbed = false;
            }
        }
    }
}
