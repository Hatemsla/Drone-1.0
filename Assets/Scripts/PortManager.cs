using System.Collections.Generic;
using System.Linq;
using Builder;
using ObjectsPool;
using UnityEngine;

namespace Drone
{
    public sealed class PortManager : MonoBehaviour
    {
        [SerializeField] private int maxPortControlsCount;
        [SerializeField] private PortControl controlPrefab;
        [SerializeField] private RectTransform controlParent;

        private List<Port> _ports = new();
        private List<PortControl> _portControls = new();
        private PoolBase<PortControl> _portControlsPool;

        private void Start()
        {
            _portControlsPool =
                new PoolBase<PortControl>(PreloadPortControl, GetPortControlAction, ReturnPortControlAction, maxPortControlsCount);
            BuilderManager.Instance.TestLevelEvent += GetPorts;
        }

        private void OnDestroy()
        {
            BuilderManager.Instance.TestLevelEvent -= GetPorts;
        }

        private PortControl PreloadPortControl()
        {
            var newControl = Instantiate(controlPrefab, Vector3.zero, Quaternion.identity, controlParent);
            _portControls.Add(newControl);
            return newControl;
        }

        private void GetPortControlAction(PortControl portControl) => portControl.gameObject.SetActive(true);
        private void ReturnPortControlAction(PortControl portControl) => portControl.gameObject.SetActive(false);

        public void ClosePort()
        {
            InputManager.Instance.TurnCustomActionMap("Player");
            BuilderManager.Instance.builderUI.droneView.SetActive(true);
            BuilderManager.Instance.builderUI.portUI.SetActive(false);
        }
        
        private void GetPorts()
        {
            if(!BuilderManager.Instance.isMove)
                return;

            if (_ports.Count > 0)
            {
                foreach (var port in _ports)
                {
                    port.PortOpenEvent -= CreateControlPanels;
                }
            }

            _ports = FindObjectsOfType<Port>().ToList();
            foreach (var port in _ports)
            {
                port.PortOpenEvent += CreateControlPanels;
            }
        }

        private void CreateControlPanels(Port currentPort)
        {
            _portControlsPool.ReturnAll();
            foreach (var port in _ports)
            {
                if (port == currentPort)
                {
                    var allInteractiveObjects = currentPort.GetAllInteractiveObjects();
                    foreach (var interactive in allInteractiveObjects)
                    {
                        if(interactive.Count == 0)
                            continue;
                        
                        var objectType = interactive;

                        var interactiveType = InteractiveType.None;
                        interactiveType = GetInteractiveType(objectType, interactiveType);
                        
                        var portControl = _portControlsPool.Get();
                        portControl.AddControlObjects(interactive, currentPort, interactiveType);
                    }
                }
            }
        }

        private InteractiveType GetInteractiveType(List<InteractiveObject> objectType, InteractiveType interactiveType)
        {
            switch (objectType[0])
            {
                case SecurityCamera securityCamera:
                    interactiveType = InteractiveType.SecureCamera;
                    break;
                case Lamp lamp:
                    interactiveType = InteractiveType.Lamp;
                    break;
            }

            return interactiveType;
        }
    }
}