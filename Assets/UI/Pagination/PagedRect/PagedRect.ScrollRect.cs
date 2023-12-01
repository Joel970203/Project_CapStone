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
        private Coroutine scrollCoroutine = null;
        private Canvas m_canvas = null;
        private Canvas canvas
        {
            get
            {
                if (m_canvas == null)
                {
                    m_canvas = GetComponentInParent<Canvas>();
                }
                return m_canvas;
            }
        }

        public void CenterScrollRectOnCurrentPage(bool initial = false)
        {
            if (NumberOfPages == 0) return;

            ScrollRect.ResetDragOffset = true;

            if (Application.isPlaying && !initial)
            {
                if (scrollCoroutine != null) StopCoroutine(scrollCoroutine);
                scrollCoroutine = StartCoroutine(ScrollToDesiredPosition());
            }
            else
            {
                SetScrollRectPosition();
            }
        }

        protected void SetScrollRectPosition()
        {
            if (ShowPagePreviews) HandlePagePreviewPreferredSizes();

            float offset = NumberOfPages > 0 ? GetDesiredScrollRectOffset() : 0f;

            if (ScrollRect.horizontal)
            {
                ScrollRect.content.anchoredPosition = new Vector2(offset, 0);
            }
            else
            {
                ScrollRect.content.anchoredPosition = new Vector2(0, offset);
            }
        }

        protected IEnumerator ScrollToDesiredPosition()
        {
            float percentageComplete = 0f;

            if (ShowPagePreviews) HandlePagePreviewScaling();

            float offset = GetDesiredScrollRectOffset();

            // positioning
            scrollRectAnimation_DesiredPosition = Vector2.zero;
            scrollRectAnimation_InitialPosition = ScrollRect.content.anchoredPosition;

            if (ScrollRect.horizontal)
            {
                scrollRectAnimation_DesiredPosition.x = offset;
                scrollRectAnimation_InitialPosition.y = 0;
            }
            else
            {
                scrollRectAnimation_DesiredPosition.y = offset;
                scrollRectAnimation_InitialPosition.x = 0;
            }

            //float timeStartedMoving = Time.realtimeSinceStartup;
            DateTime timeStartedMoving = System.DateTime.Now;
            while (percentageComplete < 1f)
            {
                //float timeSinceStarted = Time.realtimeSinceStartup - timeStartedMoving;
                float timeSinceStarted = (float)(System.DateTime.Now - timeStartedMoving).TotalSeconds;
                percentageComplete = timeSinceStarted / (0.25f / AnimationSpeed);

                //ScrollRect.content.anchoredPosition = Vector2.Lerp(scrollRectAnimation_InitialPosition, scrollRectAnimation_DesiredPosition, percentageComplete);
                ScrollRect.content.anchoredPosition = Vector2.Lerp(scrollRectAnimation_InitialPosition, scrollRectAnimation_DesiredPosition, curves[AnimationCurve].Evaluate(percentageComplete));

                yield return null;
            }

            ScrollRect.content.anchoredPosition = scrollRectAnimation_DesiredPosition;
        }

        protected int GetClosestPageNumberToScrollRectCenter()
        {
            return GetPageDistancesFromScrollRectCenter().OrderBy(d => d.Value).FirstOrDefault().Key;
        }

        protected Dictionary<int, float> GetPageDistancesFromScrollRectCenter()
        {
            Dictionary<int, float> pageDistances = new Dictionary<int, float>();
            var pageContainer = Viewport.transform as RectTransform;
            var childCount = pageContainer.childCount;

            int pagePosition = 0;
            float pageSize = ScrollRect.horizontal ? sizingTransform.rect.width : sizingTransform.rect.height;
            float halfPageSize = pageSize / 2f;
            float basePosition = 0f;
            float scrollRectPosition = Math.Abs(ScrollRect.horizontal ? ScrollRect.content.transform.localPosition.x : ScrollRect.content.transform.localPosition.y);

            for (var x = 0; x < childCount; x++)
            {
                var transform = pageContainer.GetChild(x);
                if (!transform.gameObject.activeInHierarchy) continue;
                var page = transform.GetComponent<Page>();
                if (page == null) continue;

                float pageScale = ScrollRect.horizontal ? page.transform.localScale.x : page.transform.localScale.y;

                pageDistances.Add(page.PageNumber, Mathf.Abs(scrollRectPosition - basePosition - (halfPageSize * pageScale)));

                pagePosition++;
                basePosition += ((pageSize * pageScale) + SpaceBetweenPages);
            }

            return pageDistances;
        }

        protected float GetDesiredScrollRectOffset()
        {
            var currentPage = GetCurrentPage();
            if (currentPage == null) return 0f;

            return GetPageOffset(currentPage);
        }

        public float GetPageOffset(Page page)
        {
            if (ShowPagePreviews) return GetPageOffset_PagePreviews(page);

            float offset = 0;
            var pagesBeforeDesiredPage = GetPagePosition(page.PageNumber) - 1;
            var pageSize = sizingTransform.rect;

            if (ScrollRect.horizontal)
            {
                offset -= (pageSize.width + SpaceBetweenPages) * pagesBeforeDesiredPage;
            }
            else
            {
                offset += (pageSize.height + SpaceBetweenPages) * pagesBeforeDesiredPage;
            }

            return offset;
        }

        internal void StopAnimating()
        {
            if (scrollCoroutine != null) StopCoroutine(scrollCoroutine);
        }
    }
}
