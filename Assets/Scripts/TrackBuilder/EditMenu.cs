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

        public void SetEditPanelParams(string objName, string objDesc, float xP, float yP, float zP, float xR, float yR, float zR, float xyzS, ObjectsType type)
        {
            objectName.text = objName;
            objectDescription.text = objDesc;
            xPos.text = xP.ToString("f2", CultureInfo.CurrentCulture);
            yPos.text = yP.ToString("f2", CultureInfo.CurrentCulture);
            zPos.text = zP.ToString("f2", CultureInfo.CurrentCulture);
            xRot.text = xR.ToString("f2", CultureInfo.CurrentCulture);
            yRot.text = yR.ToString("f2", CultureInfo.CurrentCulture);
            zRot.text = zR.ToString("f2", CultureInfo.CurrentCulture);
            
            if(type is ObjectsType.Gate or ObjectsType.Drone)
                return;
            
            xyzScale.value = ConvertScaleToSliderValue(xyzS);
        }

        private int ConvertScaleToSliderValue(float originValue)
        {
            return _sliderValues[originValue];
        }
    }
}