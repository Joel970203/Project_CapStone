using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI.Pagination
{
    [ExecuteInEditMode]
    public class PagedRect_ScrollRect : ScrollRect
    {
        public bool DisableDragging = false;

        public bool IsBeingDragged { get; protected set; }

        [HideInInspector]
        public bool ResetDragOffset = false;

        private bool notifyPagedRect = true;

        [SerializeField]
        private PagedRect _PagedRect = null;
        public PagedRect PagedRect
        {
            get
            {
                if (_PagedRect == null) _PagedRect = this.GetComponent<PagedRect>();
                return _PagedRect;
            }
        }

        private RectTransform m_contentRectTransform = null;
        protected RectTransform contentRectTransform
        {
            get
            {
                if (m_contentRectTransform == null) m_contentRectTransform = this.content.transform as RectTransform;
                return m_contentRectTransform;
            }
        }

        public PagedRect_Scrollbar ScrollBar;

        protected override void OnEnable()
        {
            base.OnEnable();

            // this makes the positioning calculations a little easier
            contentRectTransform.pivot = new Vector2(0, 1);
        }

        public override void OnScroll(PointerEventData data)
        {
            return;
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (DisableDragging) return;

            if (PagedRect != null) PagedRect.StopAnimating();

            ResetDragOffset = false;

            if (notifyPagedRect && PagedRect != null) PagedRect.OnBeginDrag(eventData);

            base.OnBeginDrag(eventData);

            IsBeingDragged = true;
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (DisableDragging) return;

            if (!IsBeingDragged) return;

            // If this is a horizontal PagedRect, only accept horizontal drag events, and vice versa if this is a Vertical PagedRect
            var analysis = AnalyseDragEvent(eventData);
            if (this.horizontal && analysis.DragPlane != DragEventAnalysis.eDragPlane.Horizontal) return;
            if (this.vertical && analysis.DragPlane != DragEventAnalysis.eDragPlane.Vertical) return;

            if (ResetDragOffset)
            {
                notifyPagedRect = false;

                OnEndDrag(eventData);
                OnBeginDrag(eventData);

                notifyPagedRect = true;
            }

            if (PagedRect != null) PagedRect.OnDrag(eventData);

            base.OnDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (DisableDragging) return;

            if (!IsBeingDragged) return;

            // we're no longer being dragged
            IsBeingDragged = false;

            // Notify PagedRect (so it can handle any OnEndDrag events if necessary)
            if (notifyPagedRect && PagedRect != null) PagedRect.OnEndDrag(eventData);

            base.OnEndDrag(eventData);
        }

        public DragEventAnalysis AnalyseDragEvent(PointerEventData data)
        {
            return new DragEventAnalysis(data);
        }

        private static Vector3 horizontalVector = new Vector2(1, 0);
        private static Vector3 verticalVector = new Vector2(0, -1);

        public Vector2 GetDirectionVector()
        {
            return horizontal ? horizontalVector : verticalVector;
        }

        public float GetOffset()
        {
            return (horizontal ? -content.anchoredPosition.x : content.anchoredPosition.y);
        }

        public float GetTotalSize()
        {
            return horizontal ? content.rect.width : content.rect.height;
        }

        public float GetPageSize()
        {
            return horizontal ? PagedRect.sizingTransform.rect.width : PagedRect.sizingTransform.rect.height;
        }

        /*protected override void LateUpdate()
        {
            base.LateUpdate();
        }*/
    }
}
