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
        protected void RecalculateDesiredPageSizes()
        {
            if (ShowPagePreviews)
            {
                var mainPageScale = 1f - (PagePreviewScale * 2f);

                // ORIGINAL DEPRECATED CODE
                m_currentPageSize = new Vector3(sizingTransform.rect.width * mainPageScale, sizingTransform.rect.height * mainPageScale, 1);

                if (ScrollRect.horizontal)
                {
                    m_currentPageSize.x -= SpaceBetweenPages * 2;
                }
                else
                {
                    m_currentPageSize.y -= SpaceBetweenPages * 2;
                }

                // used for positioning
                m_otherPageSize = new Vector3(sizingTransform.rect.width * PagePreviewScale, sizingTransform.rect.height * PagePreviewScale, 1);

                // Now calculate scales
                m_currentPageScale = new Vector3(mainPageScale, mainPageScale, 1);

                var spacingAsScale = SpaceBetweenPages / (ScrollRect.horizontal ? sizingTransform.rect.width : sizingTransform.rect.height);

                if (ScrollRect.horizontal)
                {
                    m_currentPageScale.x -= spacingAsScale * 2;

                    if (LockOneToOneScaleRatio) m_currentPageScale.y = m_currentPageScale.x;
                }
                else
                {
                    m_currentPageScale.y -= spacingAsScale * 2;

                    if (LockOneToOneScaleRatio) m_currentPageScale.x = m_currentPageScale.y;
                }

                m_otherPageScale = new Vector3(PagePreviewScale, PagePreviewScale, 1);
            }
        }

        protected float GetDesiredScrollRectOffset_PagePreviews()
        {
            /*float offset = 0;
            var pagesBeforeCurrent = GetPagePosition(CurrentPage) - 1;

            if (ScrollRect.horizontal)
            {
                offset -= (m_otherPageSize.x * pagesBeforeCurrent) + (SpaceBetweenPages * pagesBeforeCurrent);
                offset += m_otherPageSize.x + SpaceBetweenPages;
            }
            else
            {
                offset += (m_otherPageSize.y * pagesBeforeCurrent) + (SpaceBetweenPages * pagesBeforeCurrent);
                offset -= m_otherPageSize.y + SpaceBetweenPages;
            }

            return offset;*/

            return GetPageOffset_PagePreviews(GetCurrentPage());
        }

        public float GetPageOffset_PagePreviews(Page page)
        {
            float offset = 0;
            var pagePosition = GetPagePosition(page);
            var currentPagePosition = GetPagePosition(CurrentPage);

            var pagesBefore = pagePosition - 1;
            if (currentPagePosition < pagesBefore)
            {
                pagesBefore--;

                if (ScrollRect.horizontal)
                {
                    offset -= m_currentPageSize.x + SpaceBetweenPages;
                }
                else
                {
                    // todo
                }
            }

            if (ScrollRect.horizontal)
            {
                offset -= (m_otherPageSize.x * pagesBefore) + (SpaceBetweenPages * pagesBefore);
                offset += m_otherPageSize.x + SpaceBetweenPages;
            }
            else
            {
                offset += (m_otherPageSize.y * pagesBefore) + (SpaceBetweenPages * pagesBefore);
                offset -= m_otherPageSize.y + SpaceBetweenPages;
            }

            return offset;
        }

        protected void HandlePagePreviewPreferredSizes()
        {
            RecalculateDesiredPageSizes(); // populates m_currentPageSize and m_otherPageSize

            var layoutGroup = Viewport.GetComponent<HorizontalOrVerticalLayoutGroup>();
            if (layoutGroup != null) layoutGroup.childAlignment = TextAnchor.MiddleCenter;

            var currentPage = GetCurrentPage();

            var changed = false;
            //Pages.ForEach(page =>
            for (var x = 0; x < Pages.Count; x++)
            {
                var page = Pages[x];
                var layoutElement = page.GetComponent<LayoutElement>();

                if (page == currentPage)
                {
                    page.rectTransform.localScale = m_currentPageScale;
                }
                else
                {
                    page.rectTransform.localScale = m_otherPageScale;
                }

                layoutElement.preferredWidth = sizingTransform.rect.width;
                layoutElement.preferredHeight = sizingTransform.rect.height;

                changed = changed || page.DesiredScale != (Vector3)page.rectTransform.localScale;

                page.DesiredScale = page.rectTransform.localScale;

                if (changed)
                {
                    LayoutRebuilder.MarkLayoutForRebuild(page.rectTransform);
                }
            };
        }

        protected void HandlePagePreviewScaling()
        {
            RecalculateDesiredPageSizes();

            var currentPage = GetCurrentPage();

            //foreach (var page in Pages)
            for (var x = 0; x < Pages.Count; x++)
            {
                if (Pages[x] == currentPage) continue;

                Pages[x].ScaleToScale(m_otherPageScale, AnimationSpeed);
                //page.ScaleToSize(m_otherPageSize, AnimationSpeed);
            }

            if (currentPage != null) currentPage.ScaleToScale(m_currentPageScale, AnimationSpeed);
            //currentPage.ScaleToSize(m_currentPageSize, AnimationSpeed);
        }

        protected void HandleDrag_PagePreviews()
        {
            var closestPage = GetClosestPageNumberToScrollRectCenter();

            var pageChanged = closestPage != CurrentPage;

            if (pageChanged)
            {
                if (LimitDraggingToOnePageAtATime)
                {
                    // Stop the drag event
                    ScrollRect.OnEndDrag(new UnityEngine.EventSystems.PointerEventData(GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>()));
                }

                CurrentPage = closestPage;
                UpdatePagination();
                UpdateScrollBarPosition();
                HandlePagePreviewScaling();
            }
        }
    }
}
