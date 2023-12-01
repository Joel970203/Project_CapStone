using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UI.Pagination
{
    public partial class PagedRect
    {
        public int DefaultPage;

        // As of v1.38 this option is no longer available
        //[Tooltip("If this is true, then the pages property will automatically be populated from any child GameObjects with the 'Page' component."), HideInInspector]
        //public bool AutoDiscoverPages = true;

        [Header("Pagination")]
        [Tooltip("If this is set to false, the page buttons will not be shown.")]
        public bool ShowPagination = true;

        public bool ShowFirstAndLastButtons = true;
        public bool ShowPreviousAndNextButtons = true;

        [Tooltip("If there are too many page buttons to show at once, use this field to limit the number of visible buttons. 0 == No Limit")]
        public int MaximumNumberOfButtonsToShow = 15;

        [Tooltip("Set this to false to hide the button templates in edit mode")]
        public bool ShowButtonTemplatesInEditor = true;

        public bool ShowPageButtons = true;

        public bool ShowNumbersOnButtons = true;
        public bool ShowPageTitlesOnButtons = false;
        public bool ShowPageIconsOnButtons = false;

        [Header("Performance")]
        [Tooltip("If this is set to true, PagedRect will check for changes to the page collection each frame. If it is set to false, PagedRect will only update its pagination/etc. when you call UpdatePagination() or UpdateDisplay(). ")]
        public bool MonitorPageCollectionForChanges = false;

        [Header("Animation")]
        [Range(0.1f, 5f)]
        public float AnimationSpeed = 1.0f;
        public eAnimationCurve AnimationCurve = eAnimationCurve.Linear;

        [Header("Legacy (Non-ScrollRect) Animation")]
        public eAnimationType AnimationType = eAnimationType.SlideHorizontal;
        protected eAnimationType previousAnimationTypeValue;


        [Header("Automation")]
        public bool AutomaticallyMoveToNextPage = false;
        public float DelayBetweenPages = 5f;
        public bool LoopEndlessly = true;
        protected float _timeSinceLastPage = 0f;

        [Header("New Page Template")]
        [Tooltip("Optional Template for adding new pages dynamically at runtime.")]
        public Page NewPageTemplate;

        [Header("Keyboard Input")]
        public bool UseKeyboardInput = false;
        public KeyCode PreviousPageKey = KeyCode.LeftArrow;
        public KeyCode NextPageKey = KeyCode.RightArrow;
        public KeyCode FirstPageKey = KeyCode.Home;
        public KeyCode LastPageKey = KeyCode.End;

        [Header("Dragging")]
        public bool LimitDraggingToOnePageAtATime = false;

        [Header("Legacy (Non-ScrollRect) Input")]
        public bool UseSwipeInput = true;

        [Header("ScrollRect")]
        public bool UseSwipeInputForScrollRect = true;
        public float SwipeDeltaThreshold = .1f;
        public float SpaceBetweenPages = 0f;
        public bool LoopSeamlessly = false;
        public bool ShowScrollBar = false;

        [Header("Scroll Wheel Input")]
        public bool UseScrollWheelInput = false;
        public bool OnlyUseScrollWheelInputWhenMouseIsOver = true;

        [Header("Highlight")]
        public bool HighlightWhenMouseIsOver = false;
        public Color NormalColor = new Color(1f, 1f, 1f);
        public Color HighlightColor = new Color(0.9f, 0.9f, 0.9f);

        protected bool mouseIsOverPagedRect = false;

        [Header("Events")]
        public PageChangedEventType PageChangedEvent = new PageChangedEventType();

        [Serializable]
        public class PageChangedEventType : UnityEngine.Events.UnityEvent<Page, Page> { }

        [Header("Page Previews")]
        public bool ShowPagePreviews = false;
        public float PagePreviewScale = 0.25f;
        public bool LockOneToOneScaleRatio = true;
        public bool EnablePagePreviewOverlays = true;
        public Sprite PagePreviewOverlayImage;
        public Color PagePreviewOverlayNormalColor;
        public Color PagePreviewOverlayHoverColor;
        public float PagePreviewOverlayScaleOverride = 1f;

        //private Vector3 m_currentPageSize = Vector3.zero;
        private Vector3 m_currentPageSize = Vector3.zero;
        private Vector3 m_otherPageSize = Vector3.zero;

        private Vector3 m_currentPageScale = Vector3.zero;
        private Vector3 m_otherPageScale = Vector3.zero;

        [Header("References")]
        //public UnityEngine.UI.ScrollRect ScrollRect;
        public PagedRect_ScrollRect ScrollRect;
        public GameObject ScrollRectViewport;
        public GameObject Viewport;
        public GameObject Pagination;
        public PaginationButton ButtonTemplate_CurrentPage;
        public PaginationButton ButtonTemplate_OtherPages;
        public PaginationButton ButtonTemplate_DisabledPage;

        public PaginationButton Button_PreviousPage;
        public PaginationButton Button_NextPage;
        public PaginationButton Button_FirstPage;
        public PaginationButton Button_LastPage;

        public RuntimeAnimatorController AnimationControllerTemplate;

        public List<Page> Pages = new List<Page>();

        public int editorSelectedPage = 1;

        public RectTransform sizingTransform;

        /// <summary>
        /// This is used to check for changes to the Page collection and avoid updating except where necessary
        /// If we do update unnecessarily, the scene gets marked as dirty without it actually needing to be
        /// </summary>
        protected List<Page> _pages = new List<Page>();
        public bool isDirty { get; set; }
    }
}
