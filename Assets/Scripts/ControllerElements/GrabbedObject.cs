using UnityEngine;

namespace Drone.Builder.ControllerElements
{
    public class GrabbedObject : MonoBehaviour
    {
        [SerializeField] private Prompt prompt;

        public Rigidbody grabbedObjectRigidbody;


        private GameObject grabberObject;
        private Rigidbody grabberObjectRigidbody;


        private bool isObjectGrabbed;

        private bool isMovePrev;

        private FixedJoint joint;
        public float raycastDistance = 5f;
        private Ray ray;

        private void Update()
        {
            CheckGrab();
            ray = new Ray(transform.position, Vector3.up);
            SetGravityIfMove();
        }

        private void SetGravityIfMove()
        {
            if (isMovePrev == BuilderManager.Instance.isMove)
            {
                isMovePrev = !isMovePrev;
                if(grabbedObjectRigidbody)
                    grabbedObjectRigidbody.isKinematic = !BuilderManager.Instance.isMove;
            }
        }

        private void CheckButton()
        {
            if (!isObjectGrabbed)
                TryGrabObject();
            else
                ReleaseObject();
        }

        private void OnEnable()
        {
            InputManager.Instance.ApplyOpenEvent += CheckButton;
            BuilderManager.Instance.ObjectChangeSceneEvent += FindPrompt;
        }

        private void OnDisable()
        {
            InputManager.Instance.ApplyOpenEvent -= CheckButton;
            BuilderManager.Instance.ObjectChangeSceneEvent -= FindPrompt;
        }

        private void FindPrompt()
        {
            prompt = FindObjectOfType<BuilderUI>().prompt;
        }

        private void CheckGrab()
        {
            if (!isObjectGrabbed)
            {
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, raycastDistance))
                {
                    if (hit.collider.GetComponentInParent<DroneController>()) //if (hit.collider.CompareTag("Player"))//
                        //prompt.ChangePromptText("Для захвата нажмите F");
                        prompt.SetActive(BuilderManager.Instance.isMove);
                }
                else
                {
                    prompt.SetActive(false);
                }
            }
            else
            {
                prompt.SetActive(false);
            }
        }

        private void TryGrabObject()
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, raycastDistance))
                if (hit.collider.GetComponentInParent<DroneController>()) //if (hit.collider.CompareTag("Player"))//
                {
                    grabberObject = hit.collider.gameObject;
                    grabberObjectRigidbody = grabberObject.GetComponent<Rigidbody>();

                    joint = gameObject.AddComponent<FixedJoint>();
                    joint.connectedBody = grabberObjectRigidbody;

                    isObjectGrabbed = true;
                }
        }

        private void ReleaseObject()
        {
            if (joint != null)
            {
                Destroy(joint);
                grabberObject = null;
                grabberObjectRigidbody = null;
                isObjectGrabbed = false;
            }
        }
    }
}