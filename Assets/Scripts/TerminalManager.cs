using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Builder;
using TMPro;
using UnityEngine;

namespace Drone
{
    public class TerminalManager : MonoBehaviour
    {
        [SerializeField] private DroneData droneData;
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
            BuilderManager.Instance.TestLevelEvent += GetTerminals;
            InputManager.Instance.ExitTerminalEvent += CloseTerminal;
        }

        private void OnDestroy()
        {
            skillManager.BuySkillEvent -= UpdateTerminalUI;
            BuilderManager.Instance.TestLevelEvent -= GetTerminals;
            InputManager.Instance.ExitTerminalEvent -= CloseTerminal;
        }

        private void GetTerminals()
        {
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
            armorText.text = droneData.Armor.ToString("f0", CultureInfo.InvariantCulture);
            healthText.text = droneData.Health.ToString("f0", CultureInfo.InvariantCulture);
            batteryText.text = droneData.Battery.ToString("f0", CultureInfo.InvariantCulture);
            coinsText.text = droneData.Coins.ToString("f0", CultureInfo.InvariantCulture);
            crystalsText.text = droneData.Crystals.ToString("f0", CultureInfo.InvariantCulture);
        }
        
        public void CloseTerminal()
        {
            _currentTerminal.droneRpgController.isEnergyUsage = true;
            InputManager.Instance.TurnCustomActionMap("Player");
            BuilderManager.Instance.builderUI.droneView.SetActive(true);
            BuilderManager.Instance.builderUI.terminalUI.SetActive(false);
        }
    }
}