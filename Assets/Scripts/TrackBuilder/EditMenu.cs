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
        public TMP_Text objectName;
        public Image objectImage;
        public TMP_InputField xPos;
        public TMP_InputField yPos;
        public TMP_InputField zPos;
        public TMP_InputField altXPos;
        public TMP_InputField altYPos;
        public TMP_InputField altZPos;
        public TMP_InputField rotationXInput;
        public TMP_InputField rotationYInput;
        public TMP_InputField rotationZInput;
        public TMP_InputField hintInput;
        public TMP_InputField passwordInput;
        public Slider xyzScale;
        public Slider windmillRotSpeed;
        public Slider magnetForce;
        public Slider pendulumSpeed;
        public Slider pendulumAngle;
        public Slider windForce;
        public Slider batteryEnergy;
        public Slider boostForce;
        public Slider TimeDelay;
        public TMP_Text xyzScaleValue;
        public TMP_Text windmillRotSpeedValue;
        public TMP_Text magnetForceValue;
        public TMP_Text magnetKillerRotateSpeedValue;
        public TMP_Text magnetKillerDamageValue;
        public TMP_Text magnetKillerDamageIntervalValue;
        public TMP_Text pendulumSpeedValue;
        public TMP_Text pendulumAngleValue;
        public TMP_Text windForceValue;
        public TMP_Text batteryEnergyValue;
        public TMP_Text boostForceValue;
        public TMP_Text passwordHint;
        public TMP_Text TimeDelayValue;
        public Toggle activeToggle;
        public Toggle hasPasswordToggle;
        public Toggle is_hacked;
        public TMP_Dropdown color;
        public TMP_Dropdown color_panel;
        public TMP_Dropdown color_button;
        public TMP_Dropdown code_n1;
        public TMP_Dropdown code_n2;
        public TMP_Dropdown code_n3;
        public TMP_Dropdown mapsDropdown;
        public TMP_Dropdown soundsDropdown;
        public GameObject isActivePanel;
        public GameObject colorPanel;
        public GameObject windmillPanel;
        public GameObject magnetPanel;
        public GameObject magnetKillerPanel;
        public GameObject pendulumPanel;
        public GameObject windPanel;
        public GameObject batteryPanel;
        public GameObject boostPanel;
        public GameObject hintPanel;
        public GameObject drawPanel; 
        public GameObject electrogatePanel; 
        public GameObject controllerPanelPanel; 
        public GameObject controllerButtonPanel;
        public GameObject portPanel;
        public GameObject triggerMessagePanel;
        public GameObject mapsPanel;
        public List<GameObject> interactivePanels;

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
        private List<string> _sounds;

        private void Awake()
        {
            GetMaps();
            GetSounds();
        }
        
        private void GetMaps()
        {
            _maps = LevelManager.LoadMaps().ToList();
            _maps.Insert(0, "No map");
            mapsDropdown.AddOptions(_maps);
        }

        private void GetSounds()
        {
            _sounds = TrackBuilderUtils.LoadSounds().ToList();
            _sounds.Insert(0, "No sound");
            soundsDropdown.AddOptions(_sounds);
        }

        public void UpdateRotationsView(TrackObject trackObject)
        {
            rotationXInput.text = trackObject.Rotation.eulerAngles.x.ToString(CultureInfo.CurrentUICulture);
            rotationYInput.text = trackObject.Rotation.eulerAngles.y.ToString(CultureInfo.CurrentUICulture);
            rotationZInput.text = trackObject.Rotation.eulerAngles.z.ToString(CultureInfo.CurrentUICulture);
        }

        public void SetEditPanelParams(TrackObject trackObject, float newScale)
        {
            objectName.text = trackObject.objectName;
            if(trackObject.objectSprite)
                objectImage.sprite = trackObject.objectSprite;
            xPos.text = trackObject.Position.x.ToString("f1", CultureInfo.CurrentCulture);
            yPos.text = trackObject.Position.y.ToString("f1", CultureInfo.CurrentCulture);
            zPos.text = trackObject.Position.z.ToString("f1", CultureInfo.CurrentCulture);
            
            altXPos.text = trackObject.Position.x.ToString("f2", CultureInfo.CurrentCulture);
            altYPos.text = trackObject.Position.y.ToString("f2", CultureInfo.CurrentCulture);
            altZPos.text = trackObject.Position.z.ToString("f2", CultureInfo.CurrentCulture);

            xyzScaleValue.text = trackObject.Scale.x.ToString("f1", CultureInfo.CurrentCulture);
            
            if(trackObject.objectType is ObjectsType.Gate or ObjectsType.Drone)
                return;

            xyzScale.value = ConvertScaleToSliderValue(newScale);

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
                    TurnInteractivePanels(magnetPanel, magnetKillerPanel, isActivePanel, colorPanel);
                    var magnetKiller = (MagnetKiller)trackObject.interactiveObject;
                    magnetForce.value = magnetKiller.magnetForce;
                    magnetForceValue.text =
                        magnetKiller.magnetForce.ToString("f1", CultureInfo.CurrentCulture);
                    magnetKillerRotateSpeedValue.text = magnetKiller.rotationSpeed.ToString("f1", CultureInfo.CurrentCulture);
                    magnetKillerDamageValue.text = magnetKiller.baseDamage.ToString("f1", CultureInfo.CurrentCulture);
                    magnetKillerDamageIntervalValue.text = magnetKiller.damageInterval.ToString("f1", CultureInfo.CurrentCulture);
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
                    soundsDropdown.value = _sounds.IndexOf(((TriggerMessage)trackObject.interactiveObject).GetSound());
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
                    mapsDropdown.value = _maps.IndexOf(((PortalObject)trackObject.interactiveObject).GetMap());
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