using System;
using System.Collections.Generic;
using UnityEngine;

namespace Builder
{
    public sealed class SelectCreateObjectCategories : MonoBehaviour
    {
        public List<SelectCategory> selectCategories;
        public List<Category> categories;

        public event Action<SelectCategory> SelectedCategoryChanged;

        private void Start()
        {
            InvokeSelectedCategoryChanged(selectCategories[0]);
        }

        public void InvokeSelectedCategoryChanged(SelectCategory selectCategory)
        {
            SelectedCategoryChanged?.Invoke(selectCategory);

            var categoryIndex = selectCategories.IndexOf(selectCategory);
            for (var i = 0; i < categories.Count; i++)
                categories[i].gameObject.SetActive(i == categoryIndex);
        }
    }
}