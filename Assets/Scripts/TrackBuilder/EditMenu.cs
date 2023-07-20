using System.Collections.Generic;
using System.Globalization;
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
        [SerializeField] private TMP_Text xyzScaleValue;
        [SerializeField] private TMP_Text windmillRotSpeedValue;
        [SerializeField] private TMP_Text magnetForceValue;
        [SerializeField] private TMP_Text pendulumSpeedValue;
        [SerializeField] private TMP_Text pendulumAngleValue;
        [SerializeField] private TMP_Text windForceValue;
        [SerializeField] private TMP_Text batteryEnergyValue;
        [SerializeField] private TMP_Text boostForceValue;
        [SerializeField] private Toggle freezing;
        [SerializeField] private Toggle lamp;
        [SerializeField] private Toggle gate;
        [SerializeField] private Dropdown color; 
        [SerializeField] private Dropdown code_n1; 
        [SerializeField] private Dropdown code_n2; 
        [SerializeField] private Dropdown code_n3; 
        [SerializeField] private GameObject windmillPanel;
        [SerializeField] private GameObject magnetPanel;
        [SerializeField] private GameObject pendulumPanel;
        [SerializeField] private GameObject windPanel;
        [SerializeField] private GameObject batteryPanel;
        [SerializeField] private GameObject freezingPanel;
        [SerializeField] private GameObject lampPanel;
        [SerializeField] private GameObject boostPanel;
        [SerializeField] private GameObject hintPanel;
        [SerializeField] private GameObject drawPanel; 
        [SerializeField] private GameObject electrogatePanel; 
        [SerializeField] private GameObject controllerPanelPanel; 
        [SerializeField] private GameObject controllerButtonPanel;
        [SerializeField] private List<GameObject> interactivePanels;
        // добавить тооггл 
        //dropdown 

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
                    TurnInteractivePanels(windmillPanel);
                    windmillRotSpeed.value = trackObject.interactiveObject.windMillRotateSpeed;
                    windmillRotSpeedValue.text =
                        trackObject.interactiveObject.windMillRotateSpeed.ToString("f1", CultureInfo.CurrentCulture);
                    break;
                case InteractiveType.Magnet:
                    TurnInteractivePanels(magnetPanel);
                    magnetForce.value = trackObject.interactiveObject.magnetForce;
                    magnetForceValue.text =
                        trackObject.interactiveObject.magnetForce.ToString("f1", CultureInfo.CurrentCulture);
                    break;
                case InteractiveType.Pendulum:
                    TurnInteractivePanels(pendulumPanel);
                    pendulumSpeed.value = trackObject.interactiveObject.pendulumMoveSpeed;
                    pendulumSpeedValue.text =
                        trackObject.interactiveObject.pendulumMoveSpeed.ToString("f1", CultureInfo.CurrentCulture);
                    pendulumAngle.value = trackObject.interactiveObject.rightPendulumAngle;
                    pendulumAngleValue.text =
                        (trackObject.interactiveObject.rightPendulumAngle * 360f).ToString("f1", CultureInfo.CurrentCulture);
                    break;
                case InteractiveType.Wind:
                    TurnInteractivePanels(windPanel);
                    windForce.value = trackObject.interactiveObject.windForce;
                    windForceValue.text =
                        trackObject.interactiveObject.windForce.ToString("f1", CultureInfo.CurrentCulture);
                    break;
                case InteractiveType.Battery:
                    TurnInteractivePanels(batteryPanel);
                    batteryEnergy.value = trackObject.interactiveObject.batteryEnergy;
                    batteryEnergyValue.text = trackObject.interactiveObject.batteryEnergy.ToString(CultureInfo.CurrentCulture);
                    break;
                case InteractiveType.Freezing:
                    TurnInteractivePanels(freezingPanel);
                    freezing.isOn = trackObject.interactiveObject.isFreezing;
                    break;
                case InteractiveType.Boost:
                    TurnInteractivePanels(boostPanel);
                    boostForce.value = trackObject.interactiveObject.boostSpeed;
                    boostForceValue.text =
                        trackObject.interactiveObject.boostSpeed.ToString("f1", CultureInfo.CurrentCulture);
                    break;
                case InteractiveType.Lamp:
                    TurnInteractivePanels(lampPanel);
                    lamp.isOn = trackObject.interactiveObject.isLampTurn;
                    break;
                case InteractiveType.Hint:
                    TurnInteractivePanels(hintPanel);
                    hintInput.text = trackObject.interactiveObject.hintText.text;
                    break;
                case InteractiveType.Draw:
                    TurnInteractivePanels(drawPanel);
                    break;
                case InteractiveType.ElectroGate:
                    TurnInteractivePanels(electrogatePanel);
                    gate.isOn = trackObject.interactiveObject.is_activ;
                    color.value = trackObject.interactiveObject.color_index;
                    break;
                case InteractiveType.Panel:
                    TurnInteractivePanels(controllerPanelPanel);
                    // gate.isOn = trackObject.interactiveObject.is_activ;
                    // color.value = trackObject.interactiveObject.color_index;
                    break;
                case InteractiveType.Button:
                    TurnInteractivePanels(controllerButtonPanel);
                    // gate.isOn = trackObject.interactiveObject.is_activ;
                    // color.value = trackObject.interactiveObject.color_index;
                    break;
                                    
            }
        }

        private void TurnInteractivePanels(GameObject activePanel)
        {
            foreach (var interactivePanel in interactivePanels)
                interactivePanel.SetActive(interactivePanel == activePanel);
        }

        private int ConvertScaleToSliderValue(float originValue)
        {
            if (_sliderValues.TryGetValue(originValue, out var value))
                return value;

            return (int)xyzScale.value;
        }
    }
}