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
        /// Add a new page to this PagedRect - this Page should already have been instantiated.
        /// </summary>
        /// <param name="page"></param>
        public void AddPage(Page page)
        {
            if (UsingScrollRect) page.gameObject.SetActive(true);
            page.transform.SetParent(Viewport.transform);
            page.transform.localPosition = Vector3.zero;
            page.transform.localScale = Vector3.one;

            // If we don't set these values, Unity seems to set them to non-zero values and our new page shows up off-screen
            var rectTransform = (RectTransform)page.transform;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;

            page.ShowOnPagination = true;

            this.isDirty = true;

            if (CurrentPage == 0) CurrentPage = 1;

            UpdateDisplay();

            if (UsingScrollRect)
            {
                // attempt to insert the new page in the correct position
                var previousPage = GetPageByNumber(page.PageNumber - 1, false, true);
                if (previousPage != null)
                {
                    page.transform.SetSiblingIndex(previousPage.transform.GetSiblingIndex() + 1);
                    Pages = Pages.OrderBy(p => p.transform.GetSiblingIndex()).ToList();
                }

                CenterScrollRectOnCurrentPage(true);
                PagedRectTimer.DelayedCall(0, () => CenterScrollRectOnCurrentPage(true), this);
            }
        }

        /// <summary>
        /// Add a new page to this PagedRect - this Page will be instantiated and returned by this function. You can then customize it as required.
        /// </summary>
        public Page AddPageUsingTemplate()
        {
            if (NewPageTemplate == null)
            {
                throw new UnityException("Attempted to use PagedRect.AddPageUsingTemplate(), but this PagedRect instance has no NewPageTemplate set!");
            }

            var page = Instantiate(NewPageTemplate) as Page;
            this.AddPage(page);

            // this will always be the last page
            page.name = "Page " + this.NumberOfPages;

            return page;
        }

        private void UpdatePageNumbers()
        {
            var currentPage = GetCurrentPage();
            var pages = Pages.Where(p => p.PageNumber != 0)
                             .OrderBy(p => p.PageNumber)
                             .ToList();

            int pageNumber = 1;

            //foreach (var page in pages)
            for (var x = 0; x <= pages.Count; x++)
            {
                pages[x].PageNumber = pageNumber++;

                if (pages[x] == currentPage) CurrentPage = pages[x].PageNumber;
            }
        }

        /// <summary>
        /// Remove a Page from this PagedRect, and optionally destroy it
        /// </summary>
        /// <param name="page"></param>
        /// <param name="destroyPageObject"></param>
        public void RemovePage(Page page, bool destroyPageObject = false)
        {
            if (Pages.Contains(page))
            {
                page.ShowOnPagination = false;
                Pages.Remove(page);
                page.gameObject.SetActive(false);

                // If we remove the current page, move to the previous page
                if (page.PageNumber == CurrentPage)
                {
                    var pageToMoveTo = Pages.OrderByDescending(p => p.PageNumber)
                                            .Where(p => p.PageNumber < CurrentPage)
                                            .FirstOrDefault(p => p.PageEnabled && p.ShowOnPagination);

                    if (pageToMoveTo != null) CurrentPage = pageToMoveTo.PageNumber;
                    else CurrentPage = NumberOfPages;
                    //if (CurrentPage != 1) PreviousPage();
                }

                if (destroyPageObject)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(page.gameObject);
                    }
                    else
                    {
                        DestroyImmediate(page.gameObject);
                    }
                }
                else if (UsingScrollRect)
                {
                    page.gameObject.SetActive(false);
                }

                //UpdatePageNumbers();

                this.isDirty = true;
                PagedRectTimer.DelayedCall(0, () => this.UpdatePages(true, true, true), this);
            }
        }

        public void RemovePage(int pageNumber)
        {
            RemovePage(pageNumber, false);
        }

        public void RemoveCurrentPage()
        {
            RemovePage(CurrentPage, true);
        }

        public void RemovePage(int pageNumber, bool destroyPageObject)
        {
            RemovePage(GetPageByNumber(pageNumber), destroyPageObject);
        }

        public void RemoveAllPages(bool destroyPageObjects = false)
        {
            SetCurrentPage(1, true);

            var pages = this.Pages.ToList();
            foreach (var page in pages)
            {
                this.RemovePage(page, destroyPageObjects);
            }
        }

        /// <summary>
        /// Used by the Editor to keep track of the selected page in edit mode
        /// </summary>
        /// <param name="page"></param>
        public void SetEditorSelectedPage(int page)
        {
            editorSelectedPage = page;
        }

        public Page GetPageByNumber(int pageNumber, bool secondAttempt = false, bool allowNulls = false)
        {
            var page = Pages.FirstOrDefault(p => p.PageNumber == pageNumber);

            if (page == null && !secondAttempt && !allowNulls)
            {
                UpdatePages();
                return GetPageByNumber(pageNumber, true);
            }

            return page;
        }

        public Page GetCurrentPage()
        {
            return GetPageByNumber(CurrentPage);
        }

        public int GetPageNumber(Page page)
        {
            //return Pages.IndexOf(page) + 1;
            if (page.PageNumber == -1) { UpdatePages(); }

            return page.PageNumber;
        }

        /// <summary>
        /// Return the position of the given page number (as pages are moved around by Infinite Scrolling)
        /// </summary>
        /// <param name="PageNumber"></param>
        /// <returns></returns>
        protected int GetPagePosition(int PageNumber)
        {
            var page = GetPageByNumber(CurrentPage);

            return GetPagePosition(page);
        }

        protected int GetPagePosition(Page page)
        {
            var pageIndex = Pages.IndexOf(page);
            var pagePosition = pageIndex + 1;

            return pagePosition;
        }

        #region Page Collection Monitoring
        private List<Page> pageCollection = new List<Page>();

        void MonitorPageCollection()
        {
            if (!UsingScrollRect) return;

            var tempPageCollection = new List<Page>();
            foreach (RectTransform childRectTransform in ScrollRect.content)
            {
                if (!childRectTransform.gameObject.activeInHierarchy) continue;

                var page = childRectTransform.GetComponent<Page>();
                if (page != null)
                {
                    tempPageCollection.Add(page);
                }
            }

            tempPageCollection = tempPageCollection.OrderBy(p => p.PageNumber).ToList();

            if (!pageCollection.SequenceEqual(tempPageCollection))
            {
                // Force update pages, if we're in edit mode, resequence the page numbers
                UpdatePages(true, !Application.isPlaying);
                UpdateDisplay();

                pageCollection = tempPageCollection;

                if (ShowPagePreviews)
                {
                    HandlePagePreviewScaling();
                }
            }
        }
        #endregion
    }
}
