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
        /// Set the Current Page of this PagedRect.
        /// </summary>
        /// <param name="newPage"></param>
        /// <param name="initial"></param>
        public virtual void SetCurrentPage(Page newPage, bool initial = false)
        {
            var index = Pages.IndexOf(newPage);
            if (index == -1)
            {
                throw new UnityException("PagedRect.SetCurrentPag(Page newPage) :: The value provided for 'newPage' is not in the collection of pages!");
            }

            SetCurrentPage(newPage.PageNumber, initial);
        }

        public virtual void SetCurrentPage(int newPage)
        {
            SetCurrentPage(newPage, false);
        }

        /// <summary>
        /// Set the Current Page of this PagedRect (by its position in the hierarchy)
        /// </summary>
        /// <param name="newPage"></param>
        /// <param name="initial"></param>
        public virtual void SetCurrentPage(int newPage, bool initial)
        {
            if (NumberOfPages == 0) return;

            if (newPage > NumberOfPages)
            {
                throw new UnityException("PagedRect.SetCurrentPage(int newPage) :: The value provided for 'newPage' is greater than the number of pages.");
            }
            else if (newPage <= 0)
            {
                throw new UnityException("PagedRect.SetCurrentPage(int newPage) :: The value provided for 'newPage' is less than zero.");
            }

            _timeSinceLastPage = 0.0f;

            //if (CurrentPage == newPage && !UsingScrollRect) return;

            UpdatePages(false, false, false);

            var previousPage = CurrentPage;

            _timeSinceLastPage = 0f;

            CurrentPage = newPage;

            var newPageIndex = GetPagePosition(newPage) - 1;

            if (!UsingScrollRect)
            {
                if (initial)
                {
                    Pages.ForEach(p =>
                    {
                        p.LegacyReset();
                    });
                }

                var direction = CurrentPage < previousPage ? eDirection.Left : eDirection.Right;

                for (var i = 0; i < NumberOfPages; i++)
                {
                    var page = Pages[i];
                    if (i == newPageIndex)
                    {
                        PageEnterAnimation(page, direction, initial);
                        if (Application.isPlaying) page.OnShow();
                    }
                    else
                    {
                        if (page.gameObject.activeSelf)
                        {
                            if (Application.isPlaying) page.OnHide();
                            PageExitAnimation(page, direction == eDirection.Left ? eDirection.Right : eDirection.Left);
                        }
                    }
                }
            }
            else
            {
                if (Application.isPlaying)
                {
                    // Using a Scroll Rect means that the ScrollRect itself will handle animation, we just need to trigger OnShow and OnHide events here
                    for (var i = 0; i < NumberOfPages; i++)
                    {
                        var page = Pages[i];
                        if (i == newPageIndex)
                        {
                            page.OnShow();
                        }
                        else
                        {
                            if (page.Visible)
                            {
                                page.OnHide();
                            }
                        }
                    }
                }

                CenterScrollRectOnCurrentPage(initial);
            }

            UpdatePagination();

            if (!initial && PageChangedEvent != null)
            {
                PageChangedEvent.Invoke(GetPageByNumber(CurrentPage), GetPageByNumber(previousPage));
            }

            if (UsingScrollRect && ShowPagePreviews)
            {
                UpdateSeamlessPagePositions_PagePreviews();
            }

            UpdateScrollBarPosition();
        }

        /// <summary>
        /// Show the next enabled Page (if there is one)
        /// </summary>
        public virtual void NextPage()
        {
            /*if (CurrentPage == NumberOfPages)
            {
                if (LoopEndlessly) ShowFirstPage();
                return;
            }*/

            var nextEnabledPage = Pages.OrderBy(p => p.PageNumber)
                                       .Where(p => p.PageNumber > CurrentPage)
                                       .FirstOrDefault(p => p.PageEnabled && p.ShowOnPagination);

            if (nextEnabledPage != null)
            {
                SetCurrentPage(nextEnabledPage);
            }
            else
            {
                if (LoopEndlessly) ShowFirstPage();
            }
        }

        /// <summary>
        /// Show the previous enabled Page (if there is one)
        /// </summary>
        public virtual void PreviousPage()
        {
            /*if (CurrentPage == 1)
            {
                if (LoopEndlessly) ShowLastPage();
                return;
            }*/

            var prevEnablePage = Pages.OrderByDescending(p => p.PageNumber)
                                      .Where(p => p.PageNumber < CurrentPage)
                                      .FirstOrDefault(p => p.PageEnabled && p.ShowOnPagination);

            if (prevEnablePage != null)
            {
                SetCurrentPage(prevEnablePage);
            }
            else
            {
                if (LoopEndlessly) ShowLastPage();
            }
        }

        /// <summary>
        /// Show the first enabled page
        /// </summary>
        public virtual void ShowFirstPage()
        {
            var firstEnabledPage = Pages.OrderBy(p => p.PageNumber).FirstOrDefault(p => p.PageEnabled && p.ShowOnPagination);

            if (firstEnabledPage != null)
            {
                SetCurrentPage(firstEnabledPage);
            }
        }

        /// <summary>
        /// Show the last enabled page
        /// </summary>
        public virtual void ShowLastPage()
        {
            var lastEnabledPage = Pages.OrderByDescending(p => p.PageNumber).FirstOrDefault(p => p.PageEnabled && p.ShowOnPagination);

            if (lastEnabledPage != null)
            {
                SetCurrentPage(lastEnabledPage);
            }
        }
    }
}
