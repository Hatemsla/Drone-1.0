using System;
using Drone;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Drone.Builder
{
    public sealed class SelectCategory : MonoBehaviour
    {
        [SerializeField] private SelectCreateObjectCategories selectCreateObjectCategories;
        [SerializeField] private Image background;
        [SerializeField] private TMP_Text text;
        [SerializeField] private Sprite backgroundDefault;
        [SerializeField] private Sprite backgroundPressed;

        private void Start()
        {
            selectCreateObjectCategories.SelectedCategoryChanged += SelectObjectCategory;
        }

        private void SelectObjectCategory(SelectCategory select)
        {
            foreach (var category in selectCreateObjectCategories.selectCategories)
            {
                if (category == select)
                    category.TurnCategory();
                else
                    category.OffCategory();
            }
        }

        public void SelectObjectCategory()
        {
            selectCreateObjectCategories.InvokeSelectedCategoryChanged(this);
        }

        private void TurnCategory()
        {
            background.sprite = backgroundPressed;
            text.color = TrackBuilderUtils.HexToColor(Idents.HexColors.White);
        }

        private void OffCategory()
        {
            background.sprite = backgroundDefault;
            text.color = TrackBuilderUtils.HexToColor(Idents.HexColors.Gray8B8B8B);
        }
    }
}