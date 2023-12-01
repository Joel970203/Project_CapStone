using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UI.Pagination
{
    public partial class PagedRect
    {
        public int CurrentPage { get; protected set; }

        public int NumberOfPages { get { return Pages.Count; } }

        private MobileInput _MobileInput = null;
        public MobileInput MobileInput
        {
            get
            {
                if ((object)_MobileInput == null)
                {
                    _MobileInput = this.GetComponent<MobileInput>();

                    if ((object)_MobileInput == null && Application.isPlaying)
                    {
                        _MobileInput = this.gameObject.AddComponent<MobileInput>();

                        if (this.AnimationType == eAnimationType.SlideHorizontal)
                        {
                            _MobileInput.OnSwipeLeft = NextPage;
                            _MobileInput.OnSwipeRight = PreviousPage;
                        }
                        else if (this.AnimationType == eAnimationType.SlideVertical)
                        {
                            _MobileInput.OnSwipeUp = NextPage;
                            _MobileInput.OnSwipeDown = PreviousPage;
                        }
                        else
                        {
                            _MobileInput.OnSwipeLeft = _MobileInput.OnSwipeUp = () => this.NextPage();
                            _MobileInput.OnSwipeRight = _MobileInput.OnSwipeDown = () => this.PreviousPage();
                        }
                    }
                }

                return _MobileInput;
            }
        }

        private ScrollWheelInput _ScrollWheelInput = null;
        public ScrollWheelInput ScrollWheelInput
        {
            get
            {
                if ((object)_ScrollWheelInput == null)
                {
                    _ScrollWheelInput = this.GetComponent<ScrollWheelInput>();

                    if ((object)_ScrollWheelInput == null && Application.isPlaying)
                    {
                        _ScrollWheelInput = this.gameObject.AddComponent<ScrollWheelInput>();

                        _ScrollWheelInput.OnScrollUp = () => this.ScrollWheelUp();
                        _ScrollWheelInput.OnScrollDown = () => this.ScrollWheelDown();
                    }
                }

                return _ScrollWheelInput;
            }
        }

        private UnityEngine.UI.Image _imageComponent;
        protected UnityEngine.UI.Image imageComponent
        {
            get
            {
                if (_imageComponent == null)
                {
                    _imageComponent = this.GetComponent<UnityEngine.UI.Image>();
                }

                return this._imageComponent;
            }
        }

        private UnityEngine.UI.HorizontalOrVerticalLayoutGroup _layoutGroup;
        protected UnityEngine.UI.HorizontalOrVerticalLayoutGroup layoutGroup
        {
            get
            {
                if (_layoutGroup == null && Viewport != null)
                {
                    _layoutGroup = Viewport.GetComponent<UnityEngine.UI.HorizontalOrVerticalLayoutGroup>();
                }

                return _layoutGroup;
            }
        }


        private bool? _UsingScrollRect;
        /// <summary>
        /// If this is true, this PagedRect has detected that it is using a ScrollRect, and some behaviour will work differently (e.g. MobileInput)
        /// </summary>
        public bool UsingScrollRect
        {
            get
            {
                if (!_UsingScrollRect.HasValue)
                {
                    _UsingScrollRect = ScrollRect != null;
                }

                return _UsingScrollRect.Value;
            }
        }

        protected Vector2 _ScrollRectPosition = new Vector2();

        [NonSerialized]
        protected bool firstPageSet = false;

        protected List<KeyValuePair<double, Action>> delayedEditorActions = new List<KeyValuePair<double, Action>>();

        Vector2 scrollRectAnimation_InitialPosition = Vector2.zero;
        Vector2 scrollRectAnimation_DesiredPosition = Vector2.zero;
    }
}
