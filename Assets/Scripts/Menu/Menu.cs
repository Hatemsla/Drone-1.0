using UnityEngine;
using UnityEngine.UI;

namespace Drone.Menu
{
    public class Menu : MonoBehaviour
    {
        public string menuName;

        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}