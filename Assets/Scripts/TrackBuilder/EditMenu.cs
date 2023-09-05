using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Drone.Builder.ControllerElements;
using Drone.Builder.Text3D;

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
        [SerializeField] private TMP_Dropdown mapsDropdown;
        [SerializeField] private TMP_Dropdown soundsDropdown;
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
        [SerializeField] private GameObject mapsPanel;
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

        private List<string> _maps;
        private List<string> fileList = new List<string>();

        private void Start()
        {
            _maps = LevelManager.LoadMaps().ToList();
            mapsDropdown.AddOptions(_maps);
            FillDropdownWithOptions();
            
        }

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
                    var windmill = (Windmill)trackObject.interactiveObject;
                    windmillRotSpeed.value = windmill.windMillRotateSpeed;
                    windmillRotSpeedValue.text =
                        windmill.windMillRotateSpeed.ToString("f1", CultureInfo.CurrentCulture);
                    activeToggle.isOn = trackObject.interactiveObject.isActive;
                    color.value = trackObject.interactiveObject.colorIndex;
                    break;
                case InteractiveType.Magnet:
                    TurnInteractivePanels(magnetPanel, isActivePanel, colorPanel);
                    var rigidbodyMagnet = (RigidbodyMagnet)trackObject.interactiveObject;
                    magnetForce.value = rigidbodyMagnet.magnetForce;
                    magnetForceValue.text =
                        rigidbodyMagnet.magnetForce.ToString("f1", CultureInfo.CurrentCulture);
                    activeToggle.isOn = trackObject.interactiveObject.isActive;
                    color.value = trackObject.interactiveObject.colorIndex;
                    break;
                case InteractiveType.MagnetKiller:
                    TurnInteractivePanels(magnetPanel, isActivePanel, colorPanel);
                    var magnetKiller = (MagnetKiller)trackObject.interactiveObject;
                    magnetForce.value = magnetKiller.magnetForce;
                    magnetForceValue.text =
                        magnetKiller.magnetForce.ToString("f1", CultureInfo.CurrentCulture);
                    activeToggle.isOn = trackObject.interactiveObject.isActive;
                    color.value = trackObject.interactiveObject.colorIndex;
                    break;
                case InteractiveType.Pendulum:
                    TurnInteractivePanels(pendulumPanel, isActivePanel, colorPanel);
                    var pendulum = (Pendulum)trackObject.interactiveObject;
                    pendulumSpeed.value = pendulum.pendulumMoveSpeed;
                    pendulumSpeedValue.text =
                        pendulum.pendulumMoveSpeed.ToString("f1", CultureInfo.CurrentCulture);
                    pendulumAngle.value = pendulum.rightPendulumAngle;
                    pendulumAngleValue.text =
                        (pendulum.rightPendulumAngle * 360f).ToString("f1", CultureInfo.CurrentCulture);
                    activeToggle.isOn = trackObject.interactiveObject.isActive;
                    color.value = trackObject.interactiveObject.colorIndex;
                    break;
                case InteractiveType.Wind:
                    TurnInteractivePanels(windPanel, isActivePanel, colorPanel);
                    var windZone = (WindZoneScript)trackObject.interactiveObject;
                    windForce.value = windZone.windForce;
                    windForceValue.text =
                        windZone.windForce.ToString("f1", CultureInfo.CurrentCulture);
                    activeToggle.isOn = trackObject.interactiveObject.isActive;
                    color.value = trackObject.interactiveObject.colorIndex;
                    break;
                case InteractiveType.Battery:
                    TurnInteractivePanels(batteryPanel);
                    var battery = (Battery)trackObject.interactiveObject;
                    batteryEnergy.value = battery.batteryEnergy;
                    batteryEnergyValue.text = battery.batteryEnergy.ToString(CultureInfo.CurrentCulture);
                    break;
                case InteractiveType.Freezing:
                    TurnInteractivePanels(isActivePanel, colorPanel);
                    activeToggle.isOn = trackObject.interactiveObject.isActive;
                    color.value = trackObject.interactiveObject.colorIndex;
                    break;
                case InteractiveType.Boost:
                    TurnInteractivePanels(boostPanel, isActivePanel, colorPanel);
                    var boost = (BoostTrigger)trackObject.interactiveObject;
                    boostForce.value = boost.boostSpeed;
                    boostForceValue.text =
                        boost.boostSpeed.ToString("f1", CultureInfo.CurrentCulture);
                    activeToggle.isOn = trackObject.interactiveObject.isActive;
                    color.value = trackObject.interactiveObject.colorIndex;
                    break;
                case InteractiveType.Lamp:
                    TurnInteractivePanels(isActivePanel, colorPanel);
                    activeToggle.isOn = ((Lamp)trackObject.interactiveObject).isLampTurn;
                    color.value = trackObject.interactiveObject.colorIndex;
                    break;
                case InteractiveType.Hint:
                    TurnInteractivePanels(hintPanel, isActivePanel);
                    hintInput.text = ((Hint)trackObject.interactiveObject).hintText.text;
                    activeToggle.isOn = trackObject.interactiveObject.isActive;
                    break;
                case InteractiveType.Draw:
                    TurnInteractivePanels(drawPanel, isActivePanel);
                    break;
                case InteractiveType.ElectroGate:
                    TurnInteractivePanels(isActivePanel, colorPanel);
                    activeToggle.isOn = trackObject.interactiveObject.isActive;
                    color.value = trackObject.interactiveObject.colorIndex;
                    break;
                case InteractiveType.Panel:
                    TurnInteractivePanels(colorPanel, portPanel);
                    hasPasswordToggle.isOn = ((ControllerPanel)trackObject.interactiveObject).hasPassword;
                    var panelPassword = ((ControllerPanel)trackObject.interactiveObject).password;                   
                    passwordInput.text = panelPassword;
                    color.value = trackObject.interactiveObject.colorIndex;
                    break;
                case InteractiveType.Button:
                    TurnInteractivePanels(colorPanel, controllerButtonPanel);
                    var controllerButton = (ControllerButton)trackObject.interactiveObject;
                    color.value = trackObject.interactiveObject.colorIndex;
                    TimeDelay.value = controllerButton.timeDelay;
                    TimeDelayValue.text =
                        controllerButton.timeDelay.ToString("f1", CultureInfo.CurrentCulture);
                    break;
                case InteractiveType.Port:
                    TurnInteractivePanels(portPanel, isActivePanel);
                    hasPasswordToggle.isOn = ((Port)trackObject.interactiveObject).hasPassword;
                    var portPassword = ((Port)trackObject.interactiveObject).portPassword.Password;
                    passwordInput.text = portPassword;
                    break;
                case InteractiveType.TrMessage:
                    TurnInteractivePanels(colorPanel, hintPanel, triggerMessagePanel);
                    hintInput.text = ((TriggerMessage)trackObject.interactiveObject).triggerText;
                    color.value = trackObject.interactiveObject.colorIndex;
                    soundsDropdown.value = ((TriggerMessage)trackObject.interactiveObject).soundIndex;

                    // soundsDropdown.value = trackObject.interactiveObject.sound_index;
                    break;
                case InteractiveType.Terminal:
                    TurnInteractivePanels(isActivePanel);
                    break;
                case InteractiveType.PitStop:
                    TurnInteractivePanels(isActivePanel);
                    break;
                case InteractiveType.Text3D:
                    TurnInteractivePanels(hintPanel);
                    hintInput.text = ((TextWriter3D)trackObject.interactiveObject).text3D;
                    activeToggle.isOn = trackObject.interactiveObject.isActive;
                    break;
                case InteractiveType.Portal:
                    TurnInteractivePanels(mapsPanel);
                    mapsDropdown.value = _maps.ToList().IndexOf(((PortalObject)trackObject.interactiveObject).GetMap());
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

        public void FillDropdownWithOptions()
        {
            fileList.AddRange(Directory.GetFiles(Application.dataPath + "/SoundsSource/"));
            soundsDropdown.ClearOptions();
            List<TMP_Dropdown.OptionData> dropdownOptions = new List<TMP_Dropdown.OptionData>();
            foreach (string fileName in fileList)
            {
                if (fileName.EndsWith("mp3"))
                {
                    TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(fileName.Substring((Application.dataPath + "/SoundsSource/").Length));

                    dropdownOptions.Add(option);
                }
            }
            soundsDropdown.AddOptions(dropdownOptions);
        }

    }
}