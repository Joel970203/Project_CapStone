using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UI.Pagination
{
    public partial class PagedRect
    {
        /// <summary>
        /// Show or hide the First and Last Buttons
        /// </summary>
        /// <param name="show"></param>
        public void SetShowFirstAndLastButtons(bool show)
        {
            ShowFirstAndLastButtons = show;
            ToggleFirstAndLastButtons(show);
        }

        /// <summary>
        /// Show or Hide the Previous and Next Buttons
        /// </summary>
        /// <param name="show"></param>
        public void SetShowPreviousAndNextButtons(bool show)
        {
            ShowPreviousAndNextButtons = show;
            TogglePreviousAndNextButtons(show);
        }

        /// <summary>
        /// Set the animation speed for this PagedRect
        /// </summary>
        /// <param name="animationSpeed"></param>
        public void SetAnimationSpeed(float animationSpeed)
        {
            AnimationSpeed = animationSpeed;
        }

        /// <summary>
        /// Set the Animation type of this PagedRect
        /// </summary>
        /// <param name="animationType"></param>
        public void SetAnimationType(string animationType)
        {
            AnimationType = (eAnimationType)Enum.Parse(typeof(eAnimationType), animationType);
            Pages.ForEach(p => p.ResetPositionAndAlpha());
        }

        /// <summary>
        /// Set the delay between pages (when automatically scrolling through pages)
        /// </summary>
        /// <param name="delay"></param>
        public void SetDelayBetweenPages(float delay)
        {
            DelayBetweenPages = delay;
        }

        /// <summary>
        /// Enable or Disable Endless Looping
        /// </summary>
        /// <param name="loop"></param>
        public void SetLoopEndlessly(bool loop)
        {
            LoopEndlessly = loop;
        }

        /// <summary>
        /// Enable or Disable automatically moving to the next page
        /// </summary>
        /// <param name="move"></param>
        public void SetAutomaticallyMoveToNextPage(bool move)
        {
            AutomaticallyMoveToNextPage = move;
            _timeSinceLastPage = 0f;
        }

        /// <summary>
        /// Show or hide the pagination
        /// </summary>
        /// <param name="show"></param>
        public void SetShowPagination(bool show)
        {
            ShowPagination = show;
            UpdatePagination();
        }

        /// <summary>
        /// Show or Hide page numbers on Buttons
        /// </summary>
        /// <param name="show"></param>
        public void SetShowPageNumbersOnButtons(bool show)
        {
            ShowNumbersOnButtons = show;
            UpdatePagination();
        }

        /// <summary>
        /// Show or Hide page titles on Buttons (page titles are sourced from Page.PageTitle)
        /// </summary>
        /// <param name="show"></param>
        public void SetShowPageTitlesOnButtons(bool show)
        {
            ShowPageTitlesOnButtons = show;
            UpdatePagination();
        }

        /// <summary>
        /// Set the maximum number of buttons to show at once
        /// </summary>
        /// <param name="maxNumber"></param>
        public void SetMaximumNumberOfButtonsToShow(int maxNumber)
        {
            MaximumNumberOfButtonsToShow = maxNumber;
            UpdatePagination();
        }

        /// <summary>
        /// Float version of SetMaximumNumberOfButtonsToShow so we can accept input from a UI slider.
        /// </summary>
        /// <param name="maxNumber"></param>
        public void SetMaximumNumberOfButtonsToShow(float maxNumber)
        {
            SetMaximumNumberOfButtonsToShow((int)maxNumber);
        }

        /// <summary>
        /// Enable or disable keyboard input
        /// </summary>
        /// <param name="useInput"></param>
        public void SetUseKeyboardInput(bool useInput)
        {
            UseKeyboardInput = useInput;
        }

        /// <summary>
        /// Enable or disable Swipe input
        /// Please be advised that if this PagedRect is using a ScrollRect, this value will be ignored
        /// </summary>
        /// <param name="useInput"></param>
        public void SetUseSwipeInput(bool useInput)
        {
            if (UsingScrollRect)
            {
                UseSwipeInputForScrollRect = useInput;

                // Enable Mobile Input if scrollrect's dragging is disabled
                MobileInput.enabled = useInput && ScrollRect.DisableDragging;
            }
            else
            {
                MobileInput.enabled = useInput;
            }
        }

        public void SetAllowDragging(bool allowDragging)
        {
            if (UsingScrollRect)
            {
                ScrollRect.DisableDragging = !allowDragging;

                SetUseSwipeInput(UseSwipeInputForScrollRect);
            }
        }

        /// <summary>
        /// Enable or disable scroll wheel input
        /// </summary>
        /// <param name="useInput"></param>
        public void SetUseScrollWheelInput(bool useInput)
        {
            UseScrollWheelInput = useInput;
            ScrollWheelInput.enabled = useInput;
        }

        public void SetOnlyUseScrollWheelInputOnlyWhenMouseIsOver(bool onlyWhenMouseIsOver)
        {
            OnlyUseScrollWheelInputWhenMouseIsOver = onlyWhenMouseIsOver;
        }

        public void SetHighlightWhenMouseIsOver(bool highlight)
        {
            HighlightWhenMouseIsOver = highlight;

            if (!highlight)
            {
                ClearHighlight();
            }
            else
            {
                if (mouseIsOverPagedRect)
                {
                    ShowHighlight();
                }
            }
        }

        public void SetSwipeDeltaThreshold(float threshold)
        {
            SwipeDeltaThreshold = threshold;
        }
    }
}
