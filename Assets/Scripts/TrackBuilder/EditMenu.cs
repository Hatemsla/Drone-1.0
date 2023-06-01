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
        [SerializeField] private Slider xScale;
        [SerializeField] private Slider yScale;
        [SerializeField] private Slider zScale;

        public int XYZValue
        {
            get => (int)xyzScale.value;
            set => xyzScale.value = value;
        }

        private Dictionary<float, int> _sliderValues = new Dictionary<float, int>()
        {
            { 0.125f, 0 },
            { 0.25f, 1 },
            { 0.5f, 2 },
            { 1f, 3 },
            { 2f, 4 },
            { 4f, 5 },
            { 8f, 6 },
        };

        public void SetEditPanelParams(string objName, string objDesc, float xP, float yP, float zP, float xR, float yR, float zR, float xyzS, float xS, float yS, float zS, ObjectsType type)
        {
            objectName.text = objName;
            objectDescription.text = objDesc;
            xPos.text = xP.ToString(CultureInfo.CurrentCulture);
            yPos.text = yP.ToString(CultureInfo.CurrentCulture);
            zPos.text = zP.ToString(CultureInfo.CurrentCulture);
            xRot.text = xR.ToString(CultureInfo.CurrentCulture);
            yRot.text = yR.ToString(CultureInfo.CurrentCulture);
            zRot.text = zR.ToString(CultureInfo.CurrentCulture);
            
            if(type is ObjectsType.Gate or ObjectsType.Drone)
                return;
            if((int)xyzScale.value != (int)xyzS)
                xyzScale.value = ConvertScaleToSliderValue(xyzS);
            xScale.value = ConvertScaleToSliderValue(xS);
            yScale.value = ConvertScaleToSliderValue(yS);
            zScale.value = ConvertScaleToSliderValue(zS);
        }

        private int ConvertScaleToSliderValue(float originValue)
        {
            return _sliderValues[originValue];
        }
    }
}