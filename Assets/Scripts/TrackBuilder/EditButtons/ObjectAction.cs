using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Drone.TrackBuilder.EditButtons
{
    public abstract class ObjectAction : MonoBehaviour, IPointerClickHandler
    {
        public abstract event Action<ObjectAction> EventAction;
        public abstract void OnPointerClick(PointerEventData eventData);
    }
}