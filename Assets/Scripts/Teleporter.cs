using System;
using Builder;
using UnityEngine;

namespace Drone
{
    public class Teleporter : MonoBehaviour
    {
        private Teleporter _otherTeleporter;

        private void Start()
        {
            var otherTeleporters = FindObjectsOfType<Teleporter>();
            foreach (var otherTeleporter in otherTeleporters)
            {
                if (otherTeleporter != this)
                    _otherTeleporter = otherTeleporter;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (_otherTeleporter == this) return;
            
            var zPos = transform.worldToLocalMatrix.MultiplyPoint3x4(other.transform.position).z;
            if (zPos < 0)
                Teleport(other.transform);
        }

        private void Teleport(Transform obj)
        {
            var localPos = transform.worldToLocalMatrix.MultiplyPoint3x4(obj.position);
            localPos = new Vector3(-localPos.x, localPos.y, -localPos.z * 100);
            obj.position = _otherTeleporter.transform.localToWorldMatrix.MultiplyPoint3x4(localPos);
            
            obj.GetComponent<DroneBuilderController>().yaw = 180f;
        }

        private void OnTriggerEnter(Collider other)
        {
            other.gameObject.layer = 12;
        }

        private void OnTriggerExit(Collider other)
        {
            other.gameObject.layer = 8;
        }
    }
}