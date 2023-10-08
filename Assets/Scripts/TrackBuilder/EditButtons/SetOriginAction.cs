using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Drone.TrackBuilder.EditButtons
{
    public sealed class SetOriginAction : ObjectAction
    {
        public Image image;
        public override event Action<ObjectAction> EventAction;

        private void Start()
        {
            image = GetComponent<Image>();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            EventAction?.Invoke(this);
        }
    }
}