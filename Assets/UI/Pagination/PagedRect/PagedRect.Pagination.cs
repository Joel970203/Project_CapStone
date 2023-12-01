using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UI.Pagination
{
    public partial class PagedRect
    {
        private Dictionary<PaginationButton.eButtonType, List<PaginationButton>> buttonPool = new Dictionary<PaginationButton.eButtonType, List<PaginationButton>>();

        /// <summary>
        /// Updates the page buttons
        /// </summary>
        public void UpdatePagination()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                // If we're not active and in the game, don't do this
                if (!this.gameObject.activeInHierarchy)
                {
                    return;
                }

                // If we're not dirty, then don't do this
                if (!this.isDirty)
                {
                    return;
                }
            }
#endif
            var paginationButtons = GetComponentsInChildren<PaginationButton>(true)
                                    .Where(pb => !pb.DontUpdate)    // don't select buttons that we shouldn't be adjusting (e.g. templates, first/last, next/previous)
                                    .Where(pb => pb != ButtonTemplate_CurrentPage && pb != ButtonTemplate_OtherPages) // just in case we accidentally clear the DontUpdate field on the templates
                                    .Where(pb => pb.transform.parent == Pagination.transform)
                                    .ToList();

            //paginationButtons.ForEach(pb =>
            for (var x = 0; x < paginationButtons.Count; x++)
            {
                FreeButton(paginationButtons[x]);
            };

            if (!ShowPagination || !ShowPageButtons)
            {
                return;
            }

            var visiblePages = Pages.Where(p => p.ShowOnPagination).Select(p => p.PageNumber).OrderBy(p => p).ToList();
            var pagesToShow = visiblePages;

            if (MaximumNumberOfButtonsToShow != 0 && visiblePages.Count > MaximumNumberOfButtonsToShow)
            {
                var currentPageIndex = visiblePages.IndexOf(CurrentPage);

                // essentially, we'd like the current button to be as close to the center as possible
                var desiredCurrentPageIndex = (int)Math.Floor(MaximumNumberOfButtonsToShow / 2f);

                // the current page is always visible
                var tempPages = new List<int>() { CurrentPage };
                var lastStartIndexAdded = 0;

                if (currentPageIndex > 0)
                {
                    // add as many pages before the current until the current page is approximately halfway
                    int maxIterations = desiredCurrentPageIndex;
                    for (int x = currentPageIndex - 1; x <= currentPageIndex && x >= 0; x--)
                    {
                        tempPages.Insert(0, visiblePages[x]);

                        lastStartIndexAdded = x;

                        maxIterations--;

                        if (maxIterations <= 0) break;
                    }
                }

                var buttonsToAdd = MaximumNumberOfButtonsToShow - tempPages.Count;

                // add pages at the end until we reach the desired amount, or run out of pages
                for (int x = 1; x <= buttonsToAdd; x++)
                {
                    var index = currentPageIndex + x;
                    if (index >= visiblePages.Count) break;
                    tempPages.Add(visiblePages[currentPageIndex + x]);
                }

                int currentIndex = lastStartIndexAdded - 1;
                while (currentIndex > 0 && tempPages.Count < MaximumNumberOfButtonsToShow)
                {
                    // if this executes, then we ran out of buttons at the end, so we need to add some more at the beginning
                    tempPages.Insert(0, visiblePages[currentIndex]);
                    currentIndex--;
                }

                pagesToShow = tempPages;
            }

            int i = 0;
            //foreach(var pageNumber in pagesToShow)
            for (var x = 0; x < pagesToShow.Count; x++)
            {
                var pageNumber = pagesToShow[x];

                Page page = GetPageByNumber(pageNumber, false, true);

                if (page == null)
                {
                    // no page was found by this number
                    // this means a page was most likely deleted
                    // renumber pages and start again
                    PageWasDeleted(pageNumber);

                    return;
                }

                if (!page.ShowOnPagination)
                {
                    continue;
                }

                var buttonType = pageNumber == CurrentPage ? PaginationButton.eButtonType.CurrentPage : PaginationButton.eButtonType.OtherPages;
                if (!page.PageEnabled) buttonType = PaginationButton.eButtonType.DisabledPage;

                var button = GetButtonFromPool(buttonType);

                // Activate the button if need be (the template is usually disabled at this point)
                // (Moved from the end of the method to here - in Unity 2017.3, not doing this before making changes was breaking buttons for some reason)
                button.gameObject.SetActive(true);

                button.Button.onClick.RemoveAllListeners();

                if (page.PageEnabled)
                {
                    // Add the onClick listener
                    button.Button.onClick.AddListener(new UnityEngine.Events.UnityAction(() => { this.SetCurrentPage((pageNumber)); }));
                }

                // Position the button
                button.gameObject.transform.SetParent(Pagination.transform, false);

                button.transform.SetSiblingIndex(i);

                var buttonText = "";
                if (ShowNumbersOnButtons)
                {
                    buttonText = pageNumber.ToString();
                    if (ShowPageTitlesOnButtons)
                    {
                        buttonText += ". ";
                    }
                }

                if (ShowPageTitlesOnButtons)
                {
                    buttonText += page.PageTitle;
                }

                button.SetText(buttonText);

                button.Text.gameObject.SetActive(!String.IsNullOrEmpty(button.Text.text));

                if (ShowPageIconsOnButtons)
                {
                    button.Icon.gameObject.SetActive(true);
                    button.SetIcon(page.PageIcon);
                    button.Icon.color = page.PageIconColor;
                }
                else
                {
                    button.Icon.gameObject.SetActive(false);
                }

                button.gameObject.name = String.Format("Button - Page {0} {1}", pageNumber, pageNumber == CurrentPage ? "(Current Page)" : "");

                // DO update this button
                button.DontUpdate = false;

                i++;
            }

            // ensure our other buttons are in the right places
            if (Button_PreviousPage != null) Button_PreviousPage.gameObject.transform.SetAsFirstSibling();
            if (Button_FirstPage != null) Button_FirstPage.gameObject.transform.SetAsFirstSibling();
            if (Button_NextPage != null) Button_NextPage.gameObject.transform.SetAsLastSibling();
            if (Button_LastPage != null) Button_LastPage.gameObject.transform.SetAsLastSibling();

            if (Button_PreviousPage != null) Button_PreviousPage.Button.interactable = CurrentPage != 1 || LoopEndlessly;
            if (Button_NextPage != null) Button_NextPage.Button.interactable = CurrentPage != NumberOfPages || LoopEndlessly;
        }

        void ToggleTemplateButtons(bool show)
        {
            if (ButtonTemplate_CurrentPage != null) ButtonTemplate_CurrentPage.gameObject.SetActive(show);
            if (ButtonTemplate_OtherPages != null) ButtonTemplate_OtherPages.gameObject.SetActive(show);
            if (ButtonTemplate_DisabledPage != null) ButtonTemplate_DisabledPage.gameObject.SetActive(show);
        }

        void ToggleFirstAndLastButtons(bool show)
        {
            if (Button_FirstPage != null) Button_FirstPage.gameObject.SetActive(show);
            if (Button_LastPage != null) Button_LastPage.gameObject.SetActive(show);
        }

        void TogglePreviousAndNextButtons(bool show)
        {
            if (Button_NextPage != null) Button_NextPage.gameObject.SetActive(show);
            if (Button_PreviousPage != null) Button_PreviousPage.gameObject.SetActive(show);
        }

        void OptimizePagination()
        {
            if (Pagination == null) return;

            // a) Ensure that the Pagination object has a 'Canvas' component.
            // This helps reduce the overhead of adjusting components within the Pagination section
            var paginationCanvas = Pagination.GetComponent<Canvas>();
            if (paginationCanvas == null)
            {
                Pagination.AddComponent<Canvas>();
                Pagination.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }
        }

        public void InvalidateButtonPool()
        {
            buttonPool.Clear();

            var paginationButtons = GetComponentsInChildren<PaginationButton>(true)
                                    .Where(pb => !pb.DontUpdate)    // don't select buttons that we shouldn't be adjusting (e.g. templates, first/last, next/previous)
                                    .Where(pb => pb != ButtonTemplate_CurrentPage && pb != ButtonTemplate_OtherPages) // just in case we accidentally clear the DontUpdate field on the templates
                                    .Where(pb => pb.transform.parent == Pagination.transform)
                                    .ToList();

            paginationButtons.ForEach(pb =>
            {
                if(Application.isPlaying)
                {
                    Destroy(pb.gameObject);
                }
                else
                {
                    DestroyImmediate(pb.gameObject);
                }
            });
        }

        PaginationButton GetButtonFromPool(PaginationButton.eButtonType buttonType)
        {
            PaginationButton button = null;

            if (Application.isPlaying)
            {
                if (!buttonPool.ContainsKey(buttonType)) buttonPool.Add(buttonType, new List<PaginationButton>());

                button = buttonPool[buttonType].FirstOrDefault(b => b != null && !b.gameObject.activeSelf);
            }

            if (button == null)
            {
                PaginationButton template = null;

                switch (buttonType)
                {
                    case PaginationButton.eButtonType.CurrentPage:
                        template = ButtonTemplate_CurrentPage;
                        break;
                    case PaginationButton.eButtonType.OtherPages:
                        template = ButtonTemplate_OtherPages;
                        break;
                    case PaginationButton.eButtonType.DisabledPage:
                        template = ButtonTemplate_DisabledPage ?? ButtonTemplate_OtherPages;
                        break;
                }

                button = Instantiate(template) as PaginationButton;

                if(Application.isPlaying) buttonPool[buttonType].Add(button);
            }

            return button;
        }

        void FreeButton(PaginationButton button)
        {
            button.gameObject.SetActive(false);
            button.Button.onClick.RemoveAllListeners();

            // No object pooling at edit-time
            if (!Application.isPlaying)
            {
                DestroyImmediate(button.gameObject);
            }
        }
    }
}
