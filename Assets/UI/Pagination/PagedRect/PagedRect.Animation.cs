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
        /// Trigger a Page Show animation
        /// </summary>
        /// <param name="page"></param>
        /// <param name="direction"></param>
        /// <param name="initial"></param>
        protected void PageEnterAnimation(Page page, eDirection direction, bool initial = false)
        {
            if (!Application.isPlaying || AnimationType == eAnimationType.None || initial)
            {
                page.gameObject.SetActive(true);
            }
            else
            {
                var animationType = page.UsePageAnimationType ? page.AnimationType : AnimationType;
                switch (animationType)
                {
                    case eAnimationType.Fade:
                        {
                            //StartCoroutine(DelayedCall(0.5f, () => page.FadeIn()));
                            page.FadeIn();
                        }
                        break;

                    case eAnimationType.SlideHorizontal:
                    case eAnimationType.SlideVertical:
                        {
                            if (page.FlipAnimationDirection)
                            {
                                direction = (direction == eDirection.Left) ? eDirection.Right : eDirection.Left;
                            }

                            page.SlideIn(direction, animationType == eAnimationType.SlideVertical);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Trigger a Page Exit animation
        /// </summary>
        /// <param name="page"></param>
        /// <param name="direction"></param>
        protected void PageExitAnimation(Page page, eDirection direction)
        {
            if (!Application.isPlaying || AnimationType == eAnimationType.None)
            {
                page.gameObject.SetActive(false);
            }
            else
            {
                var animationType = page.UsePageAnimationType ? page.AnimationType : AnimationType;
                switch (animationType)
                {
                    case eAnimationType.Fade:
                        {
                            page.FadeOut();
                        }
                        break;

                    case eAnimationType.SlideHorizontal:
                    case eAnimationType.SlideVertical:
                        {
                            if (page.FlipAnimationDirection)
                            {
                                direction = (direction == eDirection.Left) ? eDirection.Right : eDirection.Left;
                            }

                            page.SlideOut(direction, animationType == eAnimationType.SlideVertical);
                        }
                        break;
                }
            }
        }
    }
}
