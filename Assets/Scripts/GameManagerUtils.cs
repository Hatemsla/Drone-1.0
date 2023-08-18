using UnityEngine;

namespace Drone
{
    public static class GameManagerUtils
    {
        public static void BackToMenu(AsyncLoad asyncLoad, GameObject ui, GameObject loadUi)
        {
            ui.SetActive(false);
            loadUi.SetActive(true);
            asyncLoad.LoadScene(1);
            Time.timeScale = 1f;
        }
        
        public static void LoadNewLevel(AsyncLoad asyncLoad, GameObject ui, GameObject loadUi)
        {
            ui.SetActive(false);
            loadUi.SetActive(true);
            asyncLoad.LoadScene(4);
            Time.timeScale = 1f;
        }

        public static void Exit()
        {
            Application.Quit();
        }
    }
}