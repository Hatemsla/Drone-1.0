using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Builder
{
    public class EditMenu : MonoBehaviour
    {
        [SerializeField] private TMP_Text objectName;
        [SerializeField] private TMP_Text objectDescription;
        [SerializeField] private TMP_InputField xPos;
        [SerializeField] private TMP_InputField yPos;
        [SerializeField] private TMP_InputField zPos;
        [SerializeField] private TMP_InputField xRot;
        [SerializeField] private TMP_InputField yRot;
        [SerializeField] private TMP_InputField zRot;
        [SerializeField] private TMP_InputField hintInput;
        [SerializeField] private Slider xyzScale;
        [SerializeField] private Slider windmillRotSpeed;
        [SerializeField] private Slider magnetForce;
        [SerializeField] private Slider pendulumSpeed;
        [SerializeField] private Slider pendulumAngle;
        [SerializeField] private Slider windForce;
        [SerializeField] private Slider batteryEnergy;
        [SerializeField] private Slider boostForce;
        [SerializeField] private Slider TimeDelaySlider;
        [SerializeField] private TMP_Text xyzScaleValue;
        [SerializeField] private TMP_Text windmillRotSpeedValue;
        [SerializeField] private TMP_Text magnetForceValue;
        [SerializeField] private TMP_Text pendulumSpeedValue;
        [SerializeField] private TMP_Text pendulumAngleValue;
        [SerializeField] private TMP_Text windForceValue;
        [SerializeField] private TMP_Text batteryEnergyValue;
        [SerializeField] private TMP_Text boostForceValue;
        [SerializeField] private Toggle activeToggle;
        [SerializeField] private Toggle is_hacked;
        [SerializeField] private Dropdown color; 
        [SerializeField] private Dropdown color_panel; 
        [SerializeField] private Dropdown color_button; 
        [SerializeField] private Dropdown code_n1; 
        [SerializeField] private Dropdown code_n2; 
        [SerializeField] private Dropdown code_n3; 
        [SerializeField] private GameObject isActivePanel;
        [SerializeField] private GameObject windmillPanel;
        [SerializeField] private GameObject magnetPanel;
        [SerializeField] private GameObject pendulumPanel;
        [SerializeField] private GameObject windPanel;
        [SerializeField] private GameObject batteryPanel;
        [SerializeField] private GameObject boostPanel;
        [SerializeField] private GameObject hintPanel;
        [SerializeField] private GameObject drawPanel; 
        [SerializeField] private GameObject electrogatePanel; 
        [SerializeField] private GameObject controllerPanelPanel; 
        [SerializeField] private GameObject controllerButtonPanel;
        [SerializeField] private List<GameObject> interactivePanels;

        private Dictionary<float, int> _sliderValues = new Dictionary<float, int>()
        {
            { 0.5f, 0 },
            { 1f, 1 },
            { 1.5f, 2 },
            { 2f, 3 },
            { 2.5f, 4 },
            { 3f, 5 },
            { 3.5f, 6 },
            { 4f, 7 },
            { 4.5f, 8 },
            { 5f, 9 },
            { 5.5f, 10 },
            { 6f, 11 },
            { 6.5f, 12 },
            { 7f, 13 },
            { 7.5f, 14 },
            { 8f, 15 },
        };

        public void SetEditPanelParams(string objName, string objDesc, float xP, float yP, float zP, float xR, float yR, float zR, float xyzS, TrackObject trackObject)
        {
            objectName.text = objName;
            objectDescription.text = objDesc;
            xPos.text = xP.ToString("f2", CultureInfo.CurrentCulture);
            yPos.text = yP.ToString("f2", CultureInfo.CurrentCulture);
            zPos.text = zP.ToString("f2", CultureInfo.CurrentCulture);
            xRot.text = xR.ToString("f2", CultureInfo.CurrentCulture);
            yRot.text = yR.ToString("f2", CultureInfo.CurrentCulture);
            zRot.text = zR.ToString("f2", CultureInfo.CurrentCulture);
            xyzScaleValue.text = xyzS.ToString("f1", CultureInfo.CurrentCulture);
            
            if(trackObject.objectType is ObjectsType.Gate or ObjectsType.Drone)
                return;

            xyzScale.value = ConvertScaleToSliderValue(xyzS);

            switch (trackObject.interactiveType)
            {
                case InteractiveType.None:
                    TurnInteractivePanels(gameObject);
                    break;
                case InteractiveType.Windmill:
                    TurnInteractivePanels(windmillPanel, isActivePanel);
                    windmillRotSpeed.value = trackObject.interactiveObject.windMillRotateSpeed;
                    windmillRotSpeedValue.text =
                        trackObject.interactiveObject.windMillRotateSpeed.ToString("f1", CultureInfo.CurrentCulture);
                    activeToggle.isOn = trackObject.interactiveObject.isActive;
                    break;
                case InteractiveType.Magnet:
                    TurnInteractivePanels(magnetPanel, isActivePanel);
                    magnetForce.value = trackObject.interactiveObject.magnetForce;
                    magnetForceValue.text =
                        trackObject.interactiveObject.magnetForce.ToString("f1", CultureInfo.CurrentCulture);
                    activeToggle.isOn = trackObject.interactiveObject.isActive;
                    break;
                case InteractiveType.Pendulum:
                    TurnInteractivePanels(pendulumPanel, isActivePanel);
                    pendulumSpeed.value = trackObject.interactiveObject.pendulumMoveSpeed;
                    pendulumSpeedValue.text =
                        trackObject.interactiveObject.pendulumMoveSpeed.ToString("f1", CultureInfo.CurrentCulture);
                    pendulumAngle.value = trackObject.interactiveObject.rightPendulumAngle;
                    pendulumAngleValue.text =
                        (trackObject.interactiveObject.rightPendulumAngle * 360f).ToString("f1", CultureInfo.CurrentCulture);
                    activeToggle.isOn = trackObject.interactiveObject.isActive;
                    break;
                case InteractiveType.Wind:
                    TurnInteractivePanels(windPanel, isActivePanel);
                    windForce.value = trackObject.interactiveObject.windForce;
                    windForceValue.text =
                        trackObject.interactiveObject.windForce.ToString("f1", CultureInfo.CurrentCulture);
                    activeToggle.isOn = trackObject.interactiveObject.isActive;
                    break;
                case InteractiveType.Battery:
                    TurnInteractivePanels(batteryPanel, isActivePanel);
                    batteryEnergy.value = trackObject.interactiveObject.batteryEnergy;
                    batteryEnergyValue.text = trackObject.interactiveObject.batteryEnergy.ToString(CultureInfo.CurrentCulture);
                    activeToggle.isOn = trackObject.interactiveObject.isActive;
                    break;
                case InteractiveType.Freezing:
                    TurnInteractivePanels(isActivePanel);
                    activeToggle.isOn = trackObject.interactiveObject.isActive;
                    break;
                case InteractiveType.Boost:
                    TurnInteractivePanels(boostPanel, isActivePanel);
                    boostForce.value = trackObject.interactiveObject.boostSpeed;
                    boostForceValue.text =
                        trackObject.interactiveObject.boostSpeed.ToString("f1", CultureInfo.CurrentCulture);
                    activeToggle.isOn = trackObject.interactiveObject.isActive;
                    break;
                case InteractiveType.Lamp:
                    TurnInteractivePanels(isActivePanel);
                    activeToggle.isOn = trackObject.interactiveObject.isLampTurn;
                    break;
                case InteractiveType.Hint:
                    TurnInteractivePanels(hintPanel, isActivePanel);
                    hintInput.text = trackObject.interactiveObject.hintText.text;
                    activeToggle.isOn = trackObject.interactiveObject.isActive;
                    break;
                case InteractiveType.Draw:
                    TurnInteractivePanels(drawPanel, isActivePanel);
                    break;
                case InteractiveType.ElectroGate:
                    TurnInteractivePanels(electrogatePanel, isActivePanel);
                    activeToggle.isOn = trackObject.interactiveObject.isActive;
                    color.value = trackObject.interactiveObject.color_index;
                    break;
                case InteractiveType.Panel:
                    TurnInteractivePanels(controllerPanelPanel, isActivePanel);
                    is_hacked.isOn = trackObject.interactiveObject.ishacked;
                    code_n1.value = trackObject.interactiveObject.n1;
                    code_n2.value = trackObject.interactiveObject.n2;
                    code_n3.value = trackObject.interactiveObject.n3;                    
                    color_panel.value = trackObject.interactiveObject.color_index;
                    break;
                case InteractiveType.Button:
                    TurnInteractivePanels(controllerButtonPanel, isActivePanel);
                    color_button.value = trackObject.interactiveObject.color_index;
                    break;            
            }
        }

        private void TurnInteractivePanels(params GameObject[] activePanels)
        {
            foreach (var interactivePanel in interactivePanels)
                interactivePanel.SetActive(activePanels.Any(x => x == interactivePanel));
        }

        private int ConvertScaleToSliderValue(float originValue)
        {
            if (_sliderValues.TryGetValue(originValue, out var value))
                return value;

            return (int)xyzScale.value;
        }
    }
}