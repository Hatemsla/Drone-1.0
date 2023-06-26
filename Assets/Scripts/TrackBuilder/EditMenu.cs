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
        [SerializeField] private GameObject windmillPanel;
        [SerializeField] private GameObject magnetPanel;
        [SerializeField] private GameObject pendulumPanel;
        [SerializeField] private GameObject windPanel;
        [SerializeField] private GameObject batteryPanel;
        [SerializeField] private GameObject freezingPanel;
        [SerializeField] private GameObject lampPanel;
        [SerializeField] private GameObject boostPanel;
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

            if (!trackObject.windmill && !trackObject.magnet && !trackObject.pendulum && !trackObject.battery &&
                !trackObject.windZone && !trackObject.windZone && !trackObject.freezingBall && !trackObject.boost && !trackObject.lamp)
                TurnInteractivePanels(gameObject);
            else if (trackObject.windmill)
            {
                TurnInteractivePanels(windmillPanel);
                windmillRotSpeed.value = trackObject.windmill.rotateSpeed;
                windmillRotSpeedValue.text = trackObject.windmill.rotateSpeed.ToString("f1", CultureInfo.CurrentCulture);
            }
            else if (trackObject.magnet)
            {
                TurnInteractivePanels(magnetPanel);
                magnetForce.value = trackObject.magnet.magnetForce;
                magnetForceValue.text = trackObject.magnet.magnetForce.ToString("f1", CultureInfo.CurrentCulture);
            }
            else if (trackObject.pendulum)
            {
                TurnInteractivePanels(pendulumPanel);
                pendulumSpeed.value = trackObject.pendulum.moveSpeed;
                pendulumSpeedValue.text = trackObject.pendulum.moveSpeed.ToString("f1", CultureInfo.CurrentCulture);
                pendulumAngle.value = trackObject.pendulum.rightAngle;
                pendulumAngleValue.text = (trackObject.pendulum.rightAngle * 360f).ToString("f1", CultureInfo.CurrentCulture);
            }
            else if (trackObject.windZone)
            {
                TurnInteractivePanels(windPanel);
                windForce.value = trackObject.windZone.windForce;
                windForceValue.text = trackObject.windZone.windForce.ToString("f1", CultureInfo.CurrentCulture);
            }
            else if (trackObject.battery)
            {
                TurnInteractivePanels(batteryPanel);
                batteryEnergy.value = trackObject.battery.energy;
                batteryEnergyValue.text = trackObject.battery.energy.ToString(CultureInfo.CurrentCulture);
            }
            else if (trackObject.freezingBall)
            {
                TurnInteractivePanels(freezingPanel);
                freezing.isOn = trackObject.freezingBall.isFreezing;
            }
            else if (trackObject.boost)
            {
                TurnInteractivePanels(boostPanel);
                boostForce.value = trackObject.boost.boost;
                boostForceValue.text = trackObject.boost.boost.ToString("f1", CultureInfo.CurrentCulture);
            }
            else if (trackObject.lamp)
            {
                TurnInteractivePanels(lampPanel);
                lamp.isOn = trackObject.lamp.isTurn;
            }
        }

        private void TurnInteractivePanels(GameObject activePanel)
        {
            foreach (var interactivePanel in interactivePanels)
                interactivePanel.SetActive(interactivePanel == activePanel);
        }

        private int ConvertScaleToSliderValue(float originValue)
        {
            if(_sliderValues.TryGetValue(originValue, out var value))
                return value;

            return (int)xyzScale.value;
        }
    }
}