using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI.Pagination
{
    /// <summary>
    /// An individual page within a PagedRect.
    /// </summary>
    public class Page : MonoBehaviour
    {
        public int PageNumber { get; set; }

        [SerializeField, Tooltip("Sets the text shown on the button if 'ShowPageTitlesOnButtons' is set")]
        public string PageTitle = "";

        [SerializeField, Tooltip("Sets the icon shown on the button if 'ShowPageIconsOnButtons' is set")]
        public Sprite PageIcon = null;
        public Color PageIconColor = Color.white;

        [SerializeField, Tooltip("Should this page be accessible?")]
        public bool PageEnabled = true;

        [SerializeField, Tooltip("Should this button be shown on pagination?")]
        public bool ShowOnPagination = true;

        [SerializeField]
        public PageEvent OnShowEvent = new PageEvent();
        [SerializeField]
        public PageEvent OnHideEvent = new PageEvent();

        [Serializable]
        public class PageEvent : UnityEngine.Events.UnityEvent { }

        public bool Initialised { get; protected set; }
        public Animator Animator { get; protected set; }

        public bool Visible { get; protected set; }

        protected PagedRect _pagedRect { get; set; }

        protected Vector3 initialPosition { get; set; }

        protected CanvasGroup _CanvasGroup;
        public CanvasGroup CanvasGroup
        {
            get
            {
                if (_CanvasGroup == null)
                {
                    _CanvasGroup = this.GetComponent<CanvasGroup>();
                    if (_CanvasGroup == null)
                    {
                        _CanvasGroup = this.gameObject.AddComponent<CanvasGroup>();
                    }
                }

                return _CanvasGroup;
            }
        }

        public bool UsePageAnimationType = false;
        public PagedRect.eAnimationType AnimationType;
        public bool FlipAnimationDirection = false;

        private LayoutElement m_layoutElement;
        public LayoutElement layoutElement
        {
            get
            {
                if (m_layoutElement == null) m_layoutElement = this.GetComponent<LayoutElement>();

                if (m_layoutElement == null) m_layoutElement = this.gameObject.AddComponent<LayoutElement>();

                return m_layoutElement;
            }
        }

        private PageOverlay m_pageOverlay;
        public PageOverlay pageOverlay
        {
            get
            {
                if (m_pageOverlay == null)
                {
                    var pageOverlayGameObject = PaginationUtilities.InstantiatePrefab("Page Overlay");

                    pageOverlayGameObject.transform.SetParent(this.transform);

                    m_pageOverlay = pageOverlayGameObject.GetComponent<PageOverlay>();

                    m_pageOverlay.Initialise(this, _pagedRect);
                }

                return m_pageOverlay;
            }
        }

        private RectTransform m_rectTransform;
        public RectTransform rectTransform
        {
            get
            {
                if (m_rectTransform == null) m_rectTransform = this.GetComponent<RectTransform>();
                return m_rectTransform;
            }
        }

        [HideInInspector]
        public bool IsDuplicate = false;

        [HideInInspector]
        public Page OriginalPage = null;

        public Vector3 DesiredScale = Vector3.one;


        /// <summary>
        /// Initialise this Page object and attach it to its parent PagedRect.
        /// </summary>
        /// <param name="pagedRect"></param>
        public void Initialise(PagedRect pagedRect)
        {
            if (Initialised) return;

            initialPosition = this.transform.localPosition;

            Initialised = true;

            _pagedRect = pagedRect;

            UpdateDimensions();

            if (Application.isPlaying)
            {
                if (!pagedRect.UsingScrollRect)
                {
                    Animator = this.GetComponent<Animator>();

                    if (Animator == null)
                    {
                        // setup the animator for this page
                        Animator = this.gameObject.AddComponent<Animator>();
                    }

                    Animator.runtimeAnimatorController = Instantiate(pagedRect.AnimationControllerTemplate) as RuntimeAnimatorController;
                }
                else
                {
                    Animator = this.GetComponent<Animator>();
                    if (Animator != null) Animator.enabled = false;
                }

                if (pagedRect.ShowPagePreviews)
                {
                    PagedRectTimer.DelayedCall(0, () =>
                    {
                        if (pagedRect.CurrentPage != this.PageNumber) ShowOverlay();
                    }, this);
                }
            }
        }

        public void UpdateDimensions()
        {
            if (_pagedRect == null) return;

            RectTransform rectTransform = _pagedRect.sizingTransform;
            if (rectTransform == null) rectTransform = (RectTransform)_pagedRect.transform;

            var rect = rectTransform.rect;
            if (rect.height > 0) layoutElement.preferredHeight = rect.height;
            if (rect.width > 0) layoutElement.preferredWidth = rect.width;
        }

        /// <summary>
        /// Called when this Page is shown. Triggers any OnShow events that have been set.
        /// </summary>
        public void OnShow()
        {
            Visible = true;

            if (OnShowEvent != null)
            {
                OnShowEvent.Invoke();
            }

            HideOverlay();
        }

        /// <summary>
        /// Called when this Page is hidden. Triggers any OnHide events that have been set.
        /// </summary>
        public void OnHide()
        {
            Visible = false;

            if (OnHideEvent != null)
            {
                OnHideEvent.Invoke();
            }

            if (_pagedRect.ShowPagePreviews) ShowOverlay();
        }

        /// <summary>
        /// Show a Fade-In animation.
        /// </summary>
        public void FadeIn()
        {
            gameObject.SetActive(true);
            PlayNewAnimation("FadeIn");
        }

        /// <summary>
        /// Show a Fade-Out animation.
        /// </summary>
        public void FadeOut()
        {
            if (!this.gameObject.activeInHierarchy) return;

            PlayNewAnimation("FadeOut");
            StartCoroutine(DisableWhenAnimationIsComplete());
        }

        /// <summary>
        /// Show a Slide-In animation.
        /// </summary>
        /// <param name="directionFrom"></param>
        /// <param name="vertical"></param>
        public void SlideIn(PagedRect.eDirection directionFrom, bool vertical = false)
        {
            gameObject.SetActive(true);

            var direction = directionFrom.ToString();

            if (vertical)
            {
                direction = directionFrom == PagedRect.eDirection.Left ? "Top" : "Bottom";
            }

            PlayNewAnimation("SlideIn_" + direction);
        }

        /// <summary>
        /// Show a Slide-Out animation.
        /// </summary>
        /// <param name="directionTo"></param>
        /// <param name="vertical"></param>
        public void SlideOut(PagedRect.eDirection directionTo, bool vertical = false)
        {
            if (!this.gameObject.activeInHierarchy) return;

            var direction = directionTo.ToString();

            if (vertical)
            {
                direction = directionTo == PagedRect.eDirection.Left ? "Top" : "Bottom";
            }

            PlayNewAnimation("SlideOut_" + direction);

            StartCoroutine(DisableWhenAnimationIsComplete());
        }

        /// <summary>
        /// Used to disable this Page GameObject once a FadeOut/SlideOut animation has completed.
        /// </summary>
        /// <returns></returns>
        protected IEnumerator DisableWhenAnimationIsComplete()
        {
            yield return new WaitForSeconds(1f / _pagedRect.AnimationSpeed);

            if (_pagedRect.GetCurrentPage() != this)    // if we are the current page, then the user has scrolled back to us
            {
                gameObject.SetActive(false);
                ResetPositionAndAlpha();
            }
        }

        /// <summary>
        /// Stop any animations currently being played and switch to a new one.
        /// </summary>
        /// <param name="animationName"></param>
        protected void PlayNewAnimation(string animationName)
        {
            Animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            Animator.speed = _pagedRect.AnimationSpeed;
            Animator.enabled = true;
            Animator.StopPlayback();

            Animator.Play(animationName);
        }

        public void LegacyReset()
        {
            Animator.StopPlayback();
            Animator.enabled = false;

            if (_pagedRect.GetCurrentPage() != this)
            {
                gameObject.SetActive(false);
            }

            ResetPositionAndAlpha();
        }

        /// <summary>
        /// Return this Page to its default position and visibility
        /// </summary>
        public void ResetPositionAndAlpha()
        {
            this.transform.localPosition = initialPosition;

            // reset alpha values too
            this.CanvasGroup.alpha = 1;
        }

        /// <summary>
        /// Enable this Page
        /// </summary>
        public void EnablePage()
        {
            this.PageEnabled = true;
            _pagedRect.UpdatePages();
        }

        /// <summary>
        /// Disable this Page (this will cause its pagination button to become disabled)
        /// </summary>
        public void DisablePage()
        {
            this.PageEnabled = false;
            _pagedRect.UpdatePages();
        }

        public void OverlayClicked()
        {
            if (_pagedRect.ShowPagePreviews) _pagedRect.SetCurrentPage(this);
        }


        private Coroutine scaleCoroutine = null;

        public void ScaleToScale(Vector3 scale, float animationSpeed = 0.5f)
        {
            if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);

            if (scale == (Vector3)rectTransform.localScale) return;

            scaleCoroutine = StartCoroutine(ScaleToScaleInternal(scale, animationSpeed));
        }

        protected IEnumerator ScaleToScaleInternal(Vector3 scale, float animationSpeed)
        {
            float percentageComplete = 0f;
            float timeStartedMoving = Time.realtimeSinceStartup;
            float timeSinceStarted = 0f;

            Vector3 initialScale = rectTransform.localScale;

            while (percentageComplete < 1f)
            {
                timeSinceStarted = Time.realtimeSinceStartup - timeStartedMoving;

                var temp = rectTransform.localScale;

                rectTransform.localScale = new Vector3(
                    Mathf.Lerp(initialScale.x, scale.x, percentageComplete),
                    Mathf.Lerp(initialScale.y, scale.y, percentageComplete),
                    1);

                percentageComplete = timeSinceStarted / (0.25f / animationSpeed);

                if (temp != rectTransform.localScale)
                {
                    DesiredScale = rectTransform.localScale;
                    LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
                }

                yield return null;
            }

            rectTransform.localScale = scale;
            DesiredScale = rectTransform.localScale;
        }

        /// <summary>
        /// Move this page to the specified position
        /// (position starts at 1 for page 1)
        /// </summary>
        /// <param name="position"></param>
        public void SetPagePosition(int position)
        {
            position = Math.Max(1, position);
            position = Math.Min(_pagedRect.NumberOfPages, position);

            rectTransform.SetSiblingIndex(position);

            _pagedRect.UpdatePages(true, true);
        }

        /*public void ScaleToSize(Vector3 size, float animationSpeed = 0.5f)
        {
            if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);

            // if we're already the right size, don't bother
            if (size == rectTransform.rect.size) return;

            scaleCoroutine = StartCoroutine(ScaleToSizeInternal(size, animationSpeed));
        }

        protected IEnumerator ScaleToSizeInternal(Vector3 size, float animationSpeed)
        {
            float percentageComplete = 0f;
            float timeStartedMoving = Time.time;
            float timeSinceStarted = 0f;
            
            Vector3 initialSize = rectTransform.rect.size;

            while (percentageComplete < 1f)
            {
                timeSinceStarted = Time.time - timeStartedMoving;

                layoutElement.preferredWidth = Mathf.Lerp(initialSize.x, size.x, percentageComplete);
                layoutElement.preferredHeight = Mathf.Lerp(initialSize.y, size.y, percentageComplete);

                percentageComplete = timeSinceStarted / (0.25f / animationSpeed);
                yield return null;
            }

            layoutElement.preferredWidth = size.x;
            layoutElement.preferredHeight = size.y;            
        }*/

        public void ShowOverlay()
        {
            if (!_pagedRect.EnablePagePreviewOverlays) return;

            if (pageOverlay != null)
            {
                pageOverlay.gameObject.SetActive(true);

                ScaleOverlay();
            }
        }

        public void HideOverlay()
        {
            if (pageOverlay != null) pageOverlay.gameObject.SetActive(false);
        }

        void ScaleOverlay()
        {
            if (!_pagedRect.UsingScrollRect) return;

            if (_pagedRect.ScrollRect.horizontal)
            {
                pageOverlay.transform.localScale = new Vector3(1, _pagedRect.PagePreviewOverlayScaleOverride, 1);
            }
            else
            {
                pageOverlay.transform.localScale = new Vector3(_pagedRect.PagePreviewOverlayScaleOverride, 1, 1);
            }
        }

        public void NotifyPagedRectOfChange()
        {
            if (_pagedRect != null) _pagedRect.UpdatePagination();
        }

        public PagedRect GetPagedRect()
        {
            return _pagedRect;
        }

        #region Menu Items
#if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/UI/Pagination/Page")]
        static void AddPagePrefab()
        {
            var page = PaginationUtilities.InstantiatePrefab("Page", typeof(UI.Pagination.Viewport));

            if (page != null)
            {
                var pagedRect = page.GetComponentInParent<PagedRect>();
                if (pagedRect != null)
                {
                    UnityEditor.EditorApplication.delayCall += () =>
                    {
                        pagedRect.UpdateDisplay();
                        pagedRect.ShowLastPage();
                    };
                }
            }
        }
#endif
        #endregion
    }
}
