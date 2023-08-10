using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Drone;
using TMPro;
using UnityEngine;

namespace Drone.Builder
{
    public class TerminalManager : MonoBehaviour
    {
        [SerializeField] private DroneRpgController droneRpgController;
        [SerializeField] private TMP_Text armorText;
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private TMP_Text batteryText;
        [SerializeField] private TMP_Text coinsText;
        [SerializeField] private TMP_Text crystalsText;
        [SerializeField] private SkillManager skillManager;

        private Terminal _currentTerminal;
        private List<Terminal> _terminals = new();

        private void Start()
        {
            skillManager.BuySkillEvent += UpdateTerminalUI;
            skillManager.SellSkillEvent += UpdateTerminalUI;
            BuilderManager.Instance.TestLevelEvent += GetTerminals;
            InputManager.Instance.ExitTerminalEvent += CloseTerminal;
        }

        private void OnDestroy()
        {
            skillManager.BuySkillEvent -= UpdateTerminalUI;
            skillManager.SellSkillEvent -= UpdateTerminalUI;
            BuilderManager.Instance.TestLevelEvent -= GetTerminals;
            InputManager.Instance.ExitTerminalEvent -= CloseTerminal;
        }

        private void GetTerminals()
        {
            if (!droneRpgController)
                droneRpgController = BuilderManager.Instance.droneBuilderController.droneRpgController;
            
            if(!BuilderManager.Instance.isMove)
                return;

            if (_terminals.Count > 0)
            {
                foreach (var port in _terminals)
                {
                    port.TerminalOpenEvent -= FillTerminal;
                }
            }

            _terminals = FindObjectsOfType<Terminal>().ToList();
            foreach (var port in _terminals)
            {
                port.TerminalOpenEvent += FillTerminal;
            }
        }

        private void FillTerminal(Terminal terminal)
        {
            _currentTerminal = terminal;
            UpdateTerminalUI();
        }

        private void UpdateTerminalUI()
        {
            armorText.text = droneRpgController.Armor.ToString("f0", CultureInfo.InvariantCulture);
            healthText.text = droneRpgController.Health.ToString("f0", CultureInfo.InvariantCulture);
            batteryText.text = droneRpgController.Battery.ToString("f0", CultureInfo.InvariantCulture);
            coinsText.text = droneRpgController.Coins.ToString("f0", CultureInfo.InvariantCulture);
            crystalsText.text = droneRpgController.Crystals.ToString("f0", CultureInfo.InvariantCulture);
        }
        
        public void CloseTerminal()
        {
            _currentTerminal.droneRpgController.isEnergyUsage = true;
            InputManager.Instance.TurnCustomActionMap("Player");
            BuilderManager.Instance.builderUI.droneView.SetActive(true);
            BuilderManager.Instance.builderUI.terminalUI.SetActive(false);
        }

        public void SaveGame()
        {
            droneRpgController.GetComponent<DroneBuilderCheckNode>().SetRespawn(droneRpgController.transform);
        }

        public void LoadGame()
        {
            
        }

        public void Settings()
        {
            
        }

        public void Help()
        {
            
        }
    }
}