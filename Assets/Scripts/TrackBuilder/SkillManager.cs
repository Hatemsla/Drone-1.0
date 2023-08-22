using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Drone.Builder;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Drone
{
    public class SkillManager : MonoBehaviour
    {
        [SerializeField] private GameObject buyPanel;
        [SerializeField] private Image skillImage;
        [SerializeField] private Image buySkillTime;
        [SerializeField] private Image buySkillCoin;
        [SerializeField] private Image buySkillCrystal;
        [SerializeField] private TMP_Text timeCostText;
        [SerializeField] private TMP_Text coinCostText;
        [SerializeField] private TMP_Text crystalCostText;
        [SerializeField] private Sprite enoughMoneySprite;
        [SerializeField] private Sprite noEnoughMoneySprite;
        [SerializeField] private Timer timer;
        [SerializeField] private DroneData droneData;
        [SerializeField] private List<Skill> skills = new();
        [SerializeField] private BoughtSkill[] boughtSkills;
        [SerializeField] private SkillDroneView[] skillDroneViews;

        public event Action BuySkillEvent;
        public event Action SellSkillEvent;

        private DroneRpgController _droneRpgController;
        private Skill _currentSkill;

        private void Start()
        {
            buyPanel.SetActive(false);
            foreach (var skill in skills)
            {
                skill.SelectSkillEvent += OnSelectSkill;
                skill.timeCost = droneData.skillsCosts[skill.skill][0];
                skill.coinCost = droneData.skillsCosts[skill.skill][1];
                skill.crystalCost = droneData.skillsCosts[skill.skill][2];
            }

            foreach (var boughtSkill in boughtSkills)
                boughtSkill.SellSkillEvent += OnSellSkill;    

            BuilderManager.Instance.TestLevelEvent += FindDroneRpg;

            SetupSkills();
        }

        private void OnSellSkill(BoughtSkill boughtSkill)
        {
            if(boughtSkill.skillCount < 1)
                return;
            
            var lastBuyingType = droneData.buyingTypes[boughtSkill.skill][^1];

            switch (lastBuyingType)
            {
                case MoneyType.Time:
                    BuilderManager.Instance.timer.waitForEndGame += droneData.skillsCosts[boughtSkill.skill][0];
                    break;
                case MoneyType.Coins:
                    _droneRpgController.Coins += droneData.skillsCosts[boughtSkill.skill][1];
                    break;
                case MoneyType.Crystals:
                    _droneRpgController.Crystals += droneData.skillsCosts[boughtSkill.skill][2];
                    break;
            }
            
            _droneRpgController.UpdateSkillValue(boughtSkill.skill, _droneRpgController.SkillsCount[boughtSkill.skill] - 1);
            boughtSkill.UpdateSkillCount(_droneRpgController.SkillsCount[boughtSkill.skill]);
            SellSkillEvent?.Invoke();
        }

        private void FindDroneRpg()
        {
            if (!_droneRpgController)
            {
                _droneRpgController = FindObjectOfType<DroneRpgController>();
                _droneRpgController.SkillsCountChangedEvent += UpdateBoughtSkillsUI;
            }
        }

        private void OnDestroy()
        {
            foreach (var skill in skills)
                skill.SelectSkillEvent -= OnSelectSkill;
            
            BuilderManager.Instance.TestLevelEvent -= FindDroneRpg;
        }

        private void OnSelectSkill(Skill skill)
        {
            buyPanel.SetActive(true);
            _currentSkill = skill;
            skillImage.sprite = skill.skillSprite;
            float minutes = Mathf.FloorToInt(skill.timeCost / 60);
            float seconds = Mathf.FloorToInt(skill.timeCost % 60);
            timeCostText.text = $"{minutes:00}:{seconds:00}";
            coinCostText.text = skill.coinCost.ToString(CultureInfo.InvariantCulture);
            crystalCostText.text = skill.crystalCost.ToString(CultureInfo.InvariantCulture);
            UpdateBuyButtonsUI();
        }

        private void UpdateBuyButtonsUI()
        {
            buySkillTime.sprite = timer.waitForEndGame >= _currentSkill.timeCost && boughtSkills.Any(x => x.skill == Skills.None)
                ? enoughMoneySprite
                : noEnoughMoneySprite;
            buySkillCoin.sprite = _droneRpgController.Coins >= _currentSkill.coinCost && boughtSkills.Any(x => x.skill == Skills.None)
                ? enoughMoneySprite
                : noEnoughMoneySprite;
            buySkillCrystal.sprite =
                _droneRpgController.Crystals >= _currentSkill.crystalCost && boughtSkills.Any(x => x.skill == Skills.None)
                    ? enoughMoneySprite
                    : noEnoughMoneySprite;
        }

        public void BuyByTime()
        {
            if (BuilderManager.Instance.timer.waitForEndGame >= _currentSkill.timeCost &&
                boughtSkills.Any(x => x.skill == Skills.None))
            {
                BuilderManager.Instance.timer.waitForEndGame -= _currentSkill.timeCost;
                AddBoughtSkill(_currentSkill.skill, _currentSkill.skillSprite, MoneyType.Time);
                BuySkillEvent?.Invoke();
            }
        }

        public void BuyByCoins()
        {
            if (_droneRpgController.Coins >= _currentSkill.coinCost && boughtSkills.Any(x => x.skill == Skills.None))
            {
                _droneRpgController.Coins -= _currentSkill.coinCost;
                AddBoughtSkill(_currentSkill.skill, _currentSkill.skillSprite, MoneyType.Coins);
                BuySkillEvent?.Invoke();
            }
        }

        public void BuyByCrystals()
        {
            if (_droneRpgController.Crystals >= _currentSkill.coinCost && boughtSkills.Any(x => x.skill == Skills.None))
            {
                _droneRpgController.Crystals -= _currentSkill.crystalCost;
                AddBoughtSkill(_currentSkill.skill, _currentSkill.skillSprite, MoneyType.Crystals);
                BuySkillEvent?.Invoke();
            }
        }

        private void AddBoughtSkill(Skills skill, Sprite currentSkillSkillSprite, MoneyType moneyType)
        {
            var i = 0;

            foreach (var boughtSkill in boughtSkills)
            {
                if (boughtSkill.skill == Skills.None)
                {
                    boughtSkill.SetSkillType(skill, currentSkillSkillSprite);
                    droneData.buyingTypes[skill].Add(moneyType);
                    skillDroneViews[i].GetSkill(currentSkillSkillSprite);
                    _droneRpgController.UpdateSkillValue(skill, boughtSkill.skillCount);
                    return;
                }
                else if(boughtSkill.skill == skill)
                {
                    _droneRpgController.UpdateSkillValue(skill, _droneRpgController.SkillsCount[skill] + 1);
                    droneData.buyingTypes[skill].Add(moneyType);
                    return;
                }

                i++;
            }
        }

        private void SetupSkills()
        {
            foreach (var buyingType in droneData.buyingTypes)
            {
                var i = 0;
                
                foreach (var boughtSkill in boughtSkills)
                {
                    if (buyingType.Value.Count > 0)
                    {
                        var currentSkillSprite = skills.Where(s => s.skill == buyingType.Key)
                            .Select(p => p.skillSprite)
                            .FirstOrDefault();
                        
                        if (boughtSkill.skill == Skills.None)
                        {
                            boughtSkill.SetSkillType(buyingType.Key, currentSkillSprite);
                            boughtSkill.UpdateSkillCount(buyingType.Value.Count);
                            skillDroneViews[i].GetSkill(currentSkillSprite);
                            break;
                        }
                        else if (boughtSkill.skill == buyingType.Key)
                        {
                            boughtSkill.UpdateSkillCount(buyingType.Value.Count);
                            skillDroneViews[i].GetSkill(currentSkillSprite);
                        }
                    }
                    
                    i++;
                }
            }
        }

        private void UpdateBoughtSkillsUI(Skills skill)
        {
            if(!_currentSkill)
                _currentSkill = skills.FirstOrDefault(sk => sk.skill == skill);
            
            UpdateBuyButtonsUI();

            var i = 0;
            foreach (var boughtSkill in boughtSkills)
            {
                if(!_droneRpgController.SkillsCount.ContainsKey(boughtSkill.skill))
                    continue;
                
                if (_droneRpgController.SkillsCount[boughtSkill.skill] == 0)
                {
                    droneData.buyingTypes[boughtSkill.skill].Clear();
                    
                    boughtSkill.ResetBoughtSkill();
                    skillDroneViews[i].ResetSkill();
                    
                }
                else
                {
                    if(boughtSkill.skillCount > _droneRpgController.SkillsCount[boughtSkill.skill])
                        droneData.buyingTypes[boughtSkill.skill].RemoveAt(droneData.buyingTypes[boughtSkill.skill].Count - 1);
                    
                    boughtSkill.UpdateSkillCount(_droneRpgController.SkillsCount[boughtSkill.skill]);
                }

                i++;
            }   
        }
    }
}