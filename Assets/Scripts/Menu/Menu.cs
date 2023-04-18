using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class Menu : MonoBehaviour
    {
        public string menuName;
        public Image blur;

        public void Open()
        {
            gameObject.SetActive(true);
            if (blur)
                blur.enabled = true;
        }

        public void Close()
        {
            if(blur)
                blur.enabled = false;
            gameObject.SetActive(false);
        }
    }
}