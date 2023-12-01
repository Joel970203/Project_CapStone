using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Pagination
{
    public partial class PagedRect
    {
        /// <summary>
        /// Shows/Hides pagination and buttons as required
        /// </summary>
        public void UpdateDisplay()
        {
            if (!ShowPagination)
            {
                ToggleTemplateButtons(false);
                ToggleFirstAndLastButtons(false);
                TogglePreviousAndNextButtons(false);
            }
            else
            {
                // hide our templates if we're in game
                if (Application.isPlaying || !ShowButtonTemplatesInEditor)
                {
                    ToggleTemplateButtons(false);
                }
                else if (!Application.isPlaying && ShowButtonTemplatesInEditor)
                {
                    ToggleTemplateButtons(true);
                }

                ToggleFirstAndLastButtons(ShowFirstAndLastButtons);
                TogglePreviousAndNextButtons(ShowPreviousAndNextButtons);
            }

            // Initialise page counts and pagination
            UpdatePages();

            if (UsingScrollRect)
            {
                if (layoutGroup != null)
                {
                    layoutGroup.spacing = this.SpaceBetweenPages;
                }

                UpdateScrollBar();
            }

            ViewportDimensionsChanged();
        }

        /// <summary>
        /// Call this function at any time to update the page collection.
        /// <param name="forceRenewPageNumbers">
        /// If this is set to true, then all pages will be given new page numbers based on their order - if not, only pages which have not yet been issued a pageNumber will be granted one.
        /// The reasoning behind this is that, if this PagedRect is using Infinite Scrolling, then the pages themselves will be reordered as you scroll through them, but we still want
        /// to preserve the page numbers (in the past the order was always the page number).
        /// </param>
        /// </summary>
        public void UpdatePages(bool force = false, bool forceRenewPageNumbers = false, bool updatePagination = true)
        {
            //if (!AutoDiscoverPages) return;

            if (this == null) return;

            if (force) this.isDirty = true;

            var tempPages = Viewport.GetComponentsInChildren<Page>(!UsingScrollRect)
                                    .Where(p => p != this.NewPageTemplate && p.transform.parent == Viewport.transform && (UsingScrollRect || p.ShowOnPagination))
                                    .ToList();

            // avoid an unnecessary initial update (_pages is empty when we initialise)
            if (!_pages.Any())
            {
                _pages = tempPages;
            }
            else
            {
                this.isDirty = this.isDirty || !tempPages.SequenceEqual<Page>(_pages);
            }

            Pages = _pages = tempPages;
            int pageNumber = 1;

            //            Pages.ForEach(p =>
            for (var x = 0; x < Pages.Count; x++)
            {
                var p = Pages[x];

                if (!p.Initialised) p.Initialise(this);

                if (p.PageNumber == 0 || forceRenewPageNumbers)
                {
                    p.PageNumber = pageNumber;
                }

                if (!ShowPagePreviews)
                {
                    var changed = p.DesiredScale != Vector3.one || (Vector3)p.rectTransform.localScale != Vector3.one;

                    p.DesiredScale = Vector3.one;
                    p.rectTransform.localScale = Vector3.one;

                    if (changed)
                    {
                        LayoutRebuilder.MarkLayoutForRebuild(p.rectTransform);
                    }
                }

                pageNumber++;
            };

            // see if there are any changes
            if (!this.isDirty)
            {
                return;
            }

            /*if (CurrentPage > NumberOfPages && NumberOfPages > 0)
            {
                //SetCurrentPage(Math.Max(NumberOfPages - 1, 1));
                //ShowLastPage();
            }*/

            if (updatePagination) UpdatePagination();
        }

        /// <summary>
        /// A page has been deleted - renumber pages appropriately
        /// </summary>
        /// <param name="deletedPageNumber"></param>
        void PageWasDeleted(int deletedPageNumber)
        {
            Pages.Where(p => p.PageNumber >= deletedPageNumber)
                 .ToList()
                 .ForEach(p => p.PageNumber--);

            UpdatePages();

            // deleting pages within the scrollrect will have moved us around a bit
            if (UsingScrollRect)
            {
                CenterScrollRectOnCurrentPage(true);
            }
        }

        [NonSerialized]
        private int _numberOfPages = 0;

        void CheckForDeletedPages()
        {
            if (_numberOfPages != NumberOfPages)
            {
                // check for a deleted page
                for (var x = 1; x <= _numberOfPages; x++)
                {
                    var page = GetPageByNumber(x, false, true);
                    if (page == null)
                    {
                        PageWasDeleted(x);
                        break;
                    }
                }

                _numberOfPages = NumberOfPages;
            }
        }
    }
}
