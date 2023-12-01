using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UI.Pagination
{
    public partial class PagedRect
    {
        public void ScrollBarValueChanged()
        {
            if (!UsingScrollRect) return;
            if (ScrollRect.ScrollBar == null) return;
            if (NumberOfPages <= 0) return;

            var increment = 1f / (NumberOfPages - 1);
            var desiredPagePosition = Mathf.RoundToInt(ScrollRect.ScrollBar.value / increment);
            var desiredPage = GetPageByNumber(desiredPagePosition + 1);

            if (desiredPage.PageNumber != CurrentPage)
            {
                SetCurrentPage(desiredPage);
            }
        }

        void UpdateScrollBar()
        {
            if (!UsingScrollRect) return;
            if (ScrollRect.ScrollBar == null) return;

            if (ShowScrollBar)
            {
                if (!ScrollRect.ScrollBar.gameObject.activeInHierarchy) ScrollRect.ScrollBar.gameObject.SetActive(true);
            }
            else
            {
                if (ScrollRect.ScrollBar.gameObject.activeInHierarchy) ScrollRect.ScrollBar.gameObject.SetActive(false);
            }

            ScrollRect.ScrollBar.numberOfSteps = NumberOfPages;
            ScrollRect.ScrollBar.size = 1f / NumberOfPages;
        }

        void UpdateScrollBarPosition()
        {
            if (!UsingScrollRect) return;
            if (ScrollRect.ScrollBar == null) return;
            if (!ScrollRect.ScrollBar.gameObject.activeInHierarchy) return;
            if (NumberOfPages <= 1) return;

            var increment = 1f / (NumberOfPages - 1);
            var desiredValue = (CurrentPage - 1) * increment;

            ScrollRect.ScrollBar.value = desiredValue;
        }
    }
}
