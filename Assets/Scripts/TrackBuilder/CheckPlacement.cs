using System;
using System.Collections;
using System.Collections.Generic;
using Builder;
using UnityEngine;

namespace Builder
{
    public class CheckPlacement : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("TrackGround"))
            {
                BuilderManager.Instance.canPlace = false;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("TrackGround"))
            {
                BuilderManager.Instance.canPlace = true;
            }
        }
    }
}