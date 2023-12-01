using System.Collections.Generic;
using UnityEngine;

namespace UI.Pagination
{
    public partial class PagedRect
    {
        // Note: removed some curves that don't look good.

        private static Dictionary<eAnimationCurve, AnimationCurve> curves = new Dictionary<eAnimationCurve, AnimationCurve>()
        {
            {eAnimationCurve.Linear, UnityEngine.AnimationCurve.Linear(0, 0, 1, 1)},
            {eAnimationCurve.EaseInOut, UnityEngine.AnimationCurve.EaseInOut(0, 0, 1, 1)},
            {eAnimationCurve.ExpoEaseIn, PagedRectAnimationExtensions.GenerateCurve(PagedRectAnimationExtensions.ExpoEaseIn)},
            //{eAnimationCurve.ExpoEaseOut, PagedRectAnimationExtensions.GenerateCurve(PagedRectAnimationExtensions.ExpoEaseOut)},
            {eAnimationCurve.CubicEaseIn, PagedRectAnimationExtensions.GenerateCurve(PagedRectAnimationExtensions.CubicEaseIn)},
            {eAnimationCurve.CubicEaseOut, PagedRectAnimationExtensions.GenerateCurve(PagedRectAnimationExtensions.CubicEaseOut)},
            {eAnimationCurve.ElasticEaseIn, PagedRectAnimationExtensions.GenerateCurve(PagedRectAnimationExtensions.ElasticEaseIn)},
            {eAnimationCurve.ElasticEaseOut, PagedRectAnimationExtensions.GenerateCurve(PagedRectAnimationExtensions.ElasticEaseOut)},
            {eAnimationCurve.BounceEaseIn, PagedRectAnimationExtensions.GenerateCurve(PagedRectAnimationExtensions.BounceEaseIn)},
            {eAnimationCurve.BounceEaseOut, PagedRectAnimationExtensions.GenerateCurve(PagedRectAnimationExtensions.BounceEaseOut)},
            //{eAnimationCurve.BounceEaseInOut, PagedRectAnimationExtensions.GenerateCurve(PagedRectAnimationExtensions.BounceEaseInOut)},
            //{eAnimationCurve.BounceEaseOutIn, PagedRectAnimationExtensions.GenerateCurve(PagedRectAnimationExtensions.BounceEaseOutIn)},
            {eAnimationCurve.BackEaseIn, PagedRectAnimationExtensions.GenerateCurve(PagedRectAnimationExtensions.BackEaseIn)},
            {eAnimationCurve.BackEaseOut, PagedRectAnimationExtensions.GenerateCurve(PagedRectAnimationExtensions.BackEaseOut)},
        };

        public enum eAnimationCurve
        {
            Linear,
            EaseInOut,
            ExpoEaseIn,
            //ExpoEaseOut,
            CubicEaseIn,
            CubicEaseOut,
            ElasticEaseIn,
            ElasticEaseOut,
            BounceEaseIn,
            BounceEaseOut,
            //BounceEaseInOut,
            //BounceEaseOutIn,
            BackEaseIn,
            BackEaseOut
        }

    }
}
