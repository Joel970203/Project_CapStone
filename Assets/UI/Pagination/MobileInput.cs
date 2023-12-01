using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI.Pagination
{
    [RequireComponent(typeof(PagedRect))]
    public class MobileInput : MonoBehaviour, IEndDragHandler
    {
        private readonly Vector2 mXAxis = new Vector2(1, 0);
        private readonly Vector2 mYAxis = new Vector2(0, 1);

        private const float mAngleRange = 30;
        public float mMinSwipeDist = 15f;

        private const float mMinVelocity = 600f;

        private Vector2 mStartPosition;
        private float mSwipeStartTime;

        public Action OnSwipeRight = null;
        public Action OnSwipeLeft = null;
        public Action OnSwipeUp = null;
        public Action OnSwipeDown = null;

        private RectTransform _rectTransform;
        private RectTransform rectTransform
        {
            get
            {
                if (_rectTransform == null) _rectTransform = this.GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        private Canvas _canvas;
        private Canvas canvas
        {
            get
            {
                if (_canvas == null) _canvas = this.GetComponentInParent<Canvas>();
                return _canvas;
            }
        }

        private PagedRect _pagedRect;
        private PagedRect pagedRect
        {
            get
            {
                if (_pagedRect == null) _pagedRect = this.GetComponent<PagedRect>();
                return _pagedRect;
            }
        }

        private bool swipeInProgress = false;

        private void OnDisable()
        {
            swipeInProgress = false;
        }

        void Update()
        {
            if (canvas.renderMode == RenderMode.WorldSpace) return;

            // Mouse button down, possible chance for a swipe
            if (Input.GetMouseButtonDown(0))
            {
                // Record start time and position
                mStartPosition = new Vector2(Input.mousePosition.x,
                                             Input.mousePosition.y);

                if ((canvas.renderMode == RenderMode.ScreenSpaceOverlay && RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mStartPosition))
                 || (canvas.renderMode == RenderMode.ScreenSpaceCamera && RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mStartPosition, canvas.worldCamera)))
                {
                    mSwipeStartTime = Time.realtimeSinceStartup;
                    swipeInProgress = true;
                }
                else
                {
                    mStartPosition = Vector2.zero;
                    swipeInProgress = false;
                }
            }

            // Mouse button up, possible chance for a swipe
            if (swipeInProgress && Input.GetMouseButtonUp(0))
            {
                float deltaTime = Time.realtimeSinceStartup - mSwipeStartTime;

                Vector2 endPosition = new Vector2(Input.mousePosition.x,
                                                   Input.mousePosition.y);
                Vector2 swipeVector = endPosition - mStartPosition;

                float velocity = swipeVector.magnitude / deltaTime;

                if (velocity > mMinVelocity &&
                    swipeVector.magnitude > mMinSwipeDist)
                {
                    // if the swipe has enough velocity and enough distance

                    swipeVector.Normalize();

                    float angleOfSwipe = Vector2.Dot(swipeVector, mXAxis);
                    angleOfSwipe = Mathf.Acos(angleOfSwipe) * Mathf.Rad2Deg;

                    // Detect left and right swipe
                    if (angleOfSwipe < mAngleRange)
                    {
                        if (OnSwipeRight != null)
                        {
                            OnSwipeRight();
                        }
                    }
                    else if ((180.0f - angleOfSwipe) < mAngleRange)
                    {
                        if (OnSwipeLeft != null)
                        {
                            OnSwipeLeft();
                        }
                    }
                    else
                    {
                        // Detect top and bottom swipe
                        angleOfSwipe = Vector2.Dot(swipeVector, mYAxis);
                        angleOfSwipe = Mathf.Acos(angleOfSwipe) * Mathf.Rad2Deg;
                        if (angleOfSwipe < mAngleRange)
                        {
                            if (OnSwipeUp != null)
                            {
                                OnSwipeUp();
                            }
                        }
                        else if ((180.0f - angleOfSwipe) < mAngleRange)
                        {
                            if (OnSwipeDown != null)
                            {
                                OnSwipeDown();
                            }
                        }
                    }
                }
            }

            if (swipeInProgress && !Input.GetMouseButton(0))
            {
                swipeInProgress = false;
            }
        }

        #region World Space
        public void OnEndDrag(PointerEventData data)
        {
            if (canvas.renderMode != RenderMode.WorldSpace) return;

            // Let PagedRect's internal drag code handle this:
            pagedRect.HandleDragDelta(data);
        }
        #endregion
    }
}
