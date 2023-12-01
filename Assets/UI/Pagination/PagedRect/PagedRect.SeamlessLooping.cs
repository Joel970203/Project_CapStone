using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UI.Pagination
{
    public partial class PagedRect
    {
        void UpdateSeamlessPagePositions()
        {
            if (!Application.isPlaying) return;
            if (!LoopSeamlessly) return;
            //if (NumberOfPages <= 1) return; // there's no point in adjusting page positions if we only have one page

            if (NumberOfPages <= 3) return;

            if (ShowPagePreviews)
            {
                UpdateSeamlessPagePositions_PagePreviews();
                return;
            }

            float pageSize = ScrollRect.GetPageSize();

            float totalSize = ScrollRect.GetTotalSize();
            float offset = ScrollRect.GetOffset();

            //Debug.Log(offset + " ~~ " + pageSize);

            //Debug.Log("Distance from start " + offset + ". Distance from end " + (totalSize - offset));

            // let's give it a bit more breathing room (if we have enough pages to do so)
            if (NumberOfPages > 3) pageSize *= 1.5f;

            if (offset <= pageSize)
            {
                MoveLastPageToStart();
            }
            else if (offset >= totalSize - pageSize)
            {
                MoveFirstPageToEnd();
            }
        }

        void MoveFirstPageToEnd()
        {
            var pageDistances = GetPageDistancesFromScrollRectCenter();

            var leftMostPageNumber = pageDistances.First().Key;
            var leftMostPage = GetPageByNumber(leftMostPageNumber);
            leftMostPage.transform.SetAsLastSibling();

            AdjustScrollPositionAfterPageMoved(eDirection.Right);
        }

        void MoveLastPageToStart()
        {
            var pageDistances = GetPageDistancesFromScrollRectCenter();

            var rightMostPageNumber = pageDistances.Last().Key;
            var rightMostPage = GetPageByNumber(rightMostPageNumber);
            rightMostPage.transform.SetAsFirstSibling();

            AdjustScrollPositionAfterPageMoved(eDirection.Left);
        }

        void AdjustScrollPositionAfterPageMoved(eDirection directionMoved)
        {
            var oneOrMinusOne = directionMoved == eDirection.Left ? -1 : 1;
            float pageSize = ScrollRect.GetPageSize();

            var directionVector = ScrollRect.GetDirectionVector();
            var adjustment = (directionVector * (pageSize + SpaceBetweenPages) * oneOrMinusOne);

            ScrollRect.ResetDragOffset = true;
            ScrollRect.content.anchoredPosition += adjustment;

            UpdatePages();

            // if we're already scrolling, stop, recalculate, and scroll from there
            if (scrollCoroutine != null)
            {
                scrollRectAnimation_InitialPosition += adjustment;
                scrollRectAnimation_DesiredPosition += adjustment;
            }
        }

        /// <summary>
        /// Called by UpdateScrollRectPagePositions when ShowPagePreviews is true
        /// </summary>
        void UpdateSeamlessPagePositions_PagePreviews()
        {
            if (!Application.isPlaying) return;
            if (!LoopSeamlessly) return;

            // we need at least 3 pages for our positioning code to work properly,
            // so duplicate pages as necessary
            // This isn't working yet, but will hopefully be available in a later version of PagedRect
            /*if (NumberOfPages <= 3 || Pages.Any(p => p.IsDuplicate))
            {
                SeamlessLooping_HandleDuplicatePages();
            }*/

            // this will work differently to the regular method
            // instead of moving pages as we scroll past certain offsets at the start/end, we will instead move pages whenever we change the current page,
            // provided we're less than two pages from the start/end

            bool pageMoved = false;
            var pagePosition = GetPagePosition(CurrentPage);

            float oneOrMinusOne = 1;
            var pageSize = ScrollRect.horizontal ? m_otherPageSize.x : m_otherPageSize.y;

            // if we have enough pages, then move pages around earlier to make the experience more 'seamless'
            // (and avoid users seeing the actual page move). With fewer pages, this isn't an option
            var minPage = NumberOfPages >= 5 ? 2 : 1;

            if (pagePosition <= minPage)
            {
                var pageToMove = Pages.Last();
                pageToMove.transform.SetAsFirstSibling();

                oneOrMinusOne = -1;

                pageMoved = true;
            }
            else if (NumberOfPages - pagePosition <= minPage)
            {
                var pageToMove = Pages.First();
                pageToMove.transform.SetAsLastSibling();

                pageMoved = true;
            }

            if (pageMoved)
            {
                ScrollRect.ResetDragOffset = true;

                var directionVector = ScrollRect.GetDirectionVector();
                var adjustment = (directionVector * (pageSize + SpaceBetweenPages) * oneOrMinusOne);

                ScrollRect.content.anchoredPosition += adjustment;

                UpdatePages();

                // if we're already scrolling, stop, recalculate, and scroll from there
                if (scrollCoroutine != null)
                {
                    scrollRectAnimation_InitialPosition += adjustment;
                    scrollRectAnimation_DesiredPosition += adjustment;
                }

                PagedRectTimer.DelayedCall(0, () => Viewport.GetComponent<PagedRect_LayoutGroup>().SetLayoutHorizontal(), this);
            }
        }

        void AutomaticallyMoveToNextPage_Seamless()
        {
            var currentPagePosition = GetPagePosition(CurrentPage);
            if (currentPagePosition == NumberOfPages)
            {
                MoveFirstPageToEnd();
            }

            if (CurrentPage >= NumberOfPages)
            {
                if (LoopEndlessly)
                {
                    var firstEnabledPage = Pages.OrderBy(p => p.PageNumber).FirstOrDefault(p => p.PageEnabled && p.ShowOnPagination);
                    SetCurrentPage(firstEnabledPage);
                }
            }
            else
            {
                var nextEnabledPage = Pages.OrderBy(p => p.PageNumber).Where(p => p.PageNumber > CurrentPage).FirstOrDefault(p => p.PageEnabled && p.ShowOnPagination);
                SetCurrentPage(nextEnabledPage);
            }
        }

        /*void SeamlessLooping_HandleDuplicatePages()
        {
            if (!Application.isPlaying) return;

            var numberOfNonDuplicatePages = Pages.Count(p => !p.IsDuplicate);

            if (numberOfNonDuplicatePages == 3)
            {
                int pageToDuplicate = -1;

                switch (CurrentPage)
                {
                    case 1:
                        pageToDuplicate = 3;
                        break;
                    case 3:
                        pageToDuplicate = 1;
                        break;
                }

                if (pageToDuplicate != -1)
                {
                    var existingDuplicate = Pages.FirstOrDefault(p => p.IsDuplicate);

                    if (existingDuplicate != null && existingDuplicate.OriginalPage.PageNumber == pageToDuplicate)
                    {
                        // we already have a duplicate of the correct page, do nothing
                    }
                    else
                    {
                        // we either don't have a duplicate page yet, or we have one, but it's the wrong one
                        SeamlessLooping_RemoveDuplicatePages();
                        SeamlessLooping_DuplicatePage(GetPageByNumber(pageToDuplicate));
                    }
                }
                else
                {
                    SeamlessLooping_RemoveDuplicatePages();
                    SetCurrentPage(2);
                }

            }
            else if (numberOfNonDuplicatePages == 2)
            {
            }

            // we'll never have to duplicate more than two pages
        }*/

        /*void SeamlessLooping_RemoveDuplicatePages()
        {
            // this needs to be changed so that we just disable the page, destroying it and recreating it is too expensive and too slow
            var duplicatePages = Pages.Where(p => p.IsDuplicate).ToList();
            foreach(var duplicatePage in duplicatePages)
            {
                RemovePage(duplicatePage, true);
            }
        }

        Page SeamlessLooping_DuplicatePage(Page sourcePage)
        {
            if (!Application.isPlaying) return null; // if we execute this in edit mode, it'll break everything, so... don't.

            var newPageGameObject = GameObject.Instantiate(sourcePage.gameObject);
            var pageComponent = newPageGameObject.GetComponent<Page>();

            AddPage(pageComponent);

            pageComponent.IsDuplicate = true;
            pageComponent.OriginalPage = sourcePage;

            return pageComponent;
        }*/
    }
}
