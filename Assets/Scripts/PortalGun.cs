using System.Collections.Generic;
using Builder;
using UnityEngine;

namespace Drone
{
    public class PortalGun : MonoBehaviour
    {
        [SerializeField] private Portal redPortal;
        [SerializeField] private Portal bluePortal;

        private List<Portal> _portals;
        private int _activePortalIndex;

        private void Start()
        {
            _portals = new List<Portal>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G) && BuilderManager.Instance.isMove && BuilderManager.Instance.cameraController.isFirstView)
            {
                if (Physics.Raycast(transform.position, transform.forward, out var hitInfo, 50f, -1, QueryTriggerInteraction.Ignore))
                {
                    var trackObject = hitInfo.collider.gameObject.GetComponentInParent<TrackObject>();
                    if (!trackObject) return;
                    if (trackObject.hasPortal == HasPortal.Yes)
                    {
                        if (_portals.Count < 2)
                        {
                            if (_portals.Exists(portal => portal.transform.position == hitInfo.point))
                                return;

                            var portalRotation = Quaternion.LookRotation(hitInfo.normal, Vector3.up);
                            var portalSpawnPosition = hitInfo.point + hitInfo.normal * 1.01f;
                            
                            Portal portal;
                            if (_portals.Count == 0)
                            {
                                portal = Instantiate(redPortal, portalSpawnPosition, portalRotation);
                            }
                            else
                            {
                                portal = Instantiate(bluePortal, portalSpawnPosition, portalRotation);
                                _portals[0].otherPortal = portal;
                                _portals[0].gameObject.GetComponent<Teleporter>().otherTeleporter =
                                    portal.gameObject.GetComponent<Teleporter>();
                            }

                            _portals.Add(portal);
                        }
                        else
                        {
                            _portals[_activePortalIndex].transform.position = hitInfo.point + _portals[_activePortalIndex].transform.forward * 0.6f;
                            _portals[_activePortalIndex].transform.rotation = Quaternion.LookRotation(hitInfo.normal);
                            _activePortalIndex = _activePortalIndex >= 1 ? 0 : 1;
                        }
                    }
                }
            }
        }
    }
}