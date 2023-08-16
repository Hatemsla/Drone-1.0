using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Drone.Builder
{
    public class EditMenu : MonoBehaviour
    {
        [SerializeField] private TMP_Text objectName;
        [SerializeField] private TMP_InputField xPos;
        [SerializeField] private TMP_InputField yPos;
        [SerializeField] private TMP_InputField zPos;
        [SerializeField] private Slider xRot;
        [SerializeField] private Slider yRot;
        [SerializeField] private Slider zRot;
        [SerializeField] private TMP_Text xRotValue;
        [SerializeField] private TMP_Text yRotValue;
        [SerializeField] private TMP_Text zRotValue;
        [SerializeField] private TMP_InputField hintInput;
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private Slider xyzScale;
        [SerializeField] private Slider windmillRotSpeed;
        [SerializeField] private Slider magnetForce;
        [SerializeField] private Slider pendulumSpeed;
        [SerializeField] private Slider pendulumAngle;
        [SerializeField] private Slider windForce;
        [SerializeField] private Slider batteryEnergy;
        [SerializeField] private Slider boostForce;
        [SerializeField] private Slider TimeDelay;
        [SerializeField] private TMP_Text xyzScaleValue;
        [SerializeField] private TMP_Text windmillRotSpeedValue;
        [SerializeField] private TMP_Text magnetForceValue;
        [SerializeField] private TMP_Text pendulumSpeedValue;
        [SerializeField] private TMP_Text pendulumAngleValue;
        [SerializeField] private TMP_Text windForceValue;
        [SerializeField] private TMP_Text batteryEnergyValue;
        [SerializeField] private TMP_Text boostForceValue;
        [SerializeField] private TMP_Text passwordHint;
        [SerializeField] private TMP_Text TimeDelayValue;
        [SerializeField] private Toggle activeToggle;
        [SerializeField] private Toggle hasPasswordToggle;
        [SerializeField] private Toggle is_hacked;
        [SerializeField] private TMP_Dropdown color; 
        [SerializeField] private TMP_Dropdown color_panel; 
        [SerializeField] private TMP_Dropdown color_button; 
        [SerializeField] private TMP_Dropdown code_n1; 
        [SerializeField] private TMP_Dropdown code_n2; 
        [SerializeField] private TMP_Dropdown code_n3; 
        [SerializeField] private GameObject isActivePanel;
        [SerializeField] private GameObject colorPanel;
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
        [SerializeField] private GameObject portPanel;
        [SerializeField] private GameObject triggerMessagePanel;
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

        public void SetEditPanelParams(string objName, float xP, float yP, float zP, float xR, float yR, float zR, float xyzS, TrackObject trackObject)
        {
            objectName.text = objName;
            xPos.text = xP.ToString("f1", CultureInfo.CurrentCulture);
            yPos.text = yP.ToString("f1", CultureInfo.CurrentCulture);
            zPos.text = zP.ToString("f1", CultureInfo.CurrentCulture);
            xRotValue.text = xR.ToString("f1", CultureInfo.CurrentCulture);
            yRotValue.text = yR.ToString("f1", CultureInfo.CurrentCulture);
            zRotValue.text = zR.ToString("f1", CultureInfo.CurrentCulture);
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
                    TurnInteractivePanels(windmillPanel, isActivePanel, colorPanel);
                    windmillRotSpeed.value = trackObject.interactiveObject.windMillRotateSpeed;
                    windmillRotSpeedValue.text =
                        trackObject.interactiveObject.windMillRotateSpeed.ToString("f1", CultureInfo.CurrentCulture);
                    activeToggle.isOn = trackObject.interactiveObject.isActive;
                    color.value = trackObject.interactiveObject.color_index;
                    break;
                case InteractiveType.Magnet:
                    TurnInteractivePanels(magnetPanel, isActivePanel, colorPanel);
                    magnetForce.value = trackObject.interactiveObject.magnetForce;
                    magnetForceValue.text =
                        trackObject.interactiveObject.magnetForce.ToString("f1", CultureInfo.CurrentCulture);
                    activeToggle.isOn = trackObject.interactiveObject.isActive;
                    color.value = trackObject.interactiveObject.color_index;
                    break;
                case InteractiveType.MagnetKiller:
                    TurnInteractivePanels(magnetPanel, isActivePanel, colorPanel);
                    magnetForce.value = trackObject.interactiveObject.magnetForce;
                    magnetForceValue.text =
                        trackObject.interactiveObject.magnetForce.ToString("f1", CultureInfo.CurrentCulture);
                    activeToggle.isOn = trackObject.interactiveObject.isActive;
                    color.value = trackObject.interactiveObject.color_index;
                    break;
                case InteractiveType.Pendulum:
                    TurnInteractivePanels(pendulumPanel, isActivePanel, colorPanel);
                    pendulumSpeed.value = trackObject.interactiveObject.pendulumMoveSpeed;
                    pendulumSpeedValue.text =
                        trackObject.interactiveObject.pendulumMoveSpeed.ToString("f1", CultureInfo.CurrentCulture);
                    pendulumAngle.value = trackObject.interactiveObject.rightPendulumAngle;
                    pendulumAngleValue.text =
                        (trackObject.interactiveObject.rightPendulumAngle * 360f).ToString("f1", CultureInfo.CurrentCulture);
                    activeToggle.isOn = trackObject.interactiveObject.isActive;
                    color.value = trackObject.interactiveObject.color_index;
                    break;
                case InteractiveType.Wind:
                    TurnInteractivePanels(windPanel, isActivePanel, colorPanel);
                    windForce.value = trackObject.interactiveObject.windForce;
                    windForceValue.text =
                        trackObject.interactiveObject.windForce.ToString("f1", CultureInfo.CurrentCulture);
                    activeToggle.isOn = trackObject.interactiveObject.isActive;
                    color.value = trackObject.interactiveObject.color_index;
                    break;
                case InteractiveType.Battery:
                    TurnInteractivePanels(batteryPanel, isActivePanel, colorPanel);
                    batteryEnergy.value = trackObject.interactiveObject.batteryEnergy;
                    batteryEnergyValue.text = trackObject.interactiveObject.batteryEnergy.ToString(CultureInfo.CurrentCulture);
                    activeToggle.isOn = trackObject.interactiveObject.isActive;
                    color.value = trackObject.interactiveObject.color_index;
                    break;
                case InteractiveType.Freezing:
                    TurnInteractivePanels(isActivePanel, colorPanel);
                    activeToggle.isOn = trackObject.interactiveObject.isActive;
                    color.value = trackObject.interactiveObject.color_index;
                    break;
                case InteractiveType.Boost:
                    TurnInteractivePanels(boostPanel, isActivePanel, colorPanel);
                    boostForce.value = trackObject.interactiveObject.boostSpeed;
                    boostForceValue.text =
                        trackObject.interactiveObject.boostSpeed.ToString("f1", CultureInfo.CurrentCulture);
                    activeToggle.isOn = trackObject.interactiveObject.isActive;
                    color.value = trackObject.interactiveObject.color_index;
                    break;
                case InteractiveType.Lamp:
                    TurnInteractivePanels(isActivePanel, colorPanel);
                    activeToggle.isOn = trackObject.interactiveObject.isLampTurn;
                    color.value = trackObject.interactiveObject.color_index;
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
                    TurnInteractivePanels(isActivePanel, colorPanel);
                    activeToggle.isOn = trackObject.interactiveObject.isActive;
                    color.value = trackObject.interactiveObject.color_index;
                    break;
                case InteractiveType.Panel:
                    TurnInteractivePanels(colorPanel, portPanel);
                    hasPasswordToggle.isOn = trackObject.interactiveObject.hasPassword;
                    var panelPassword = trackObject.interactiveObject.password;                   
                    passwordInput.text = panelPassword;
                    color.value = trackObject.interactiveObject.color_index;
                    break;
                case InteractiveType.Button:
                    TurnInteractivePanels(colorPanel, controllerButtonPanel);
                    color.value = trackObject.interactiveObject.color_index;
                    TimeDelay.value = trackObject.interactiveObject.TimeDelay;
                    TimeDelayValue.text =
                        trackObject.interactiveObject.TimeDelay.ToString("f1", CultureInfo.CurrentCulture);
                    
                    break;
                case InteractiveType.Port:
                    TurnInteractivePanels(portPanel, isActivePanel);
                    hasPasswordToggle.isOn = trackObject.interactiveObject.hasPassword;
                    var portPassword = ((Port)trackObject.interactiveObject).portPassword.Password;
                    passwordInput.text = portPassword;
                    break;
                case InteractiveType.TrMessage:
                    TurnInteractivePanels(colorPanel, hintPanel);
                    hintInput.text = trackObject.interactiveObject.text3D;
                    color.value = trackObject.interactiveObject.color_index;
                    break;
                case InteractiveType.Terminal:
                    TurnInteractivePanels(isActivePanel);
                    break;
                case InteractiveType.PitStop:
                    TurnInteractivePanels(isActivePanel);
                    break;
                case InteractiveType.Text3D:
                    TurnInteractivePanels(hintPanel);
                    hintInput.text = trackObject.interactiveObject.text3D;
                    activeToggle.isOn = trackObject.interactiveObject.isActive;
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

        public void PasswordHintActive(bool active)
        {
            passwordHint.enabled = active;
        }
    }
}