/*
 * Credit where it's due: these methods are based heavily on Nobutaka's 'EasingCurvePresets' project
 * https://github.com/nobutaka/EasingCurvePresets
 * Thanks!
 */
using System;
using UnityEngine;

namespace UI.Pagination
{
    public static class PagedRectAnimationExtensions
    {
        public static readonly int resolution = 10;
        public static readonly float min = 0f;
        public static readonly float max = 1f;
        public static readonly float duration = 1f;

        public static AnimationCurve GenerateCurve(Func<float, float> func)
        {
            AnimationCurve curve = new AnimationCurve();

            for (int i = 0; i < resolution; ++i)
            {
                float time = i / (resolution - 1f);

                curve.AddKey(new Keyframe(time, func(time)));
            }

            for (var i = 0; i < resolution; ++i)
            {
                curve.SmoothTangents(i, 0);
            }

            return curve;
        }

        public static float ExpoEaseOut(float time)
        {
            return (time == duration) ? min : max * (-Mathf.Pow(2, -10 * (time / duration) + 1) + min);
        }

        public static float ExpoEaseIn(float time)
        {
            return (time == 0) ? min : max * Mathf.Pow(2, 10 * (time / duration - 1) + min);
        }

        public static float CubicEaseIn(float time)
        {
            return max * (time /= duration) * time * time + min;
        }

        public static float CubicEaseOut(float time)
        {
            return max * ((time = time / duration - 1) * time * time + 1) + min;
        }

        public static float ElasticEaseIn(float time)
        {
            if ((time /= duration) == 1)
                return min + max;

            float p = duration * 0.3f;
            float s = p / 4;

            return -(max * Mathf.Pow(2, 10 * (time -= 1)) * Mathf.Sin((time * duration - s) * (2 * Mathf.PI) / p)) + min;
        }

        public static float ElasticEaseOut(float time)
        {
            if ((time /= duration) == 1)
                return min + max;

            float p = duration * 0.3f;
            float s = p / 4;

            return (max * Mathf.Pow(2, -10 * time) * Mathf.Sin((time * duration - s) * (2 * Mathf.PI) / p) + max + min);
        }

        public static float BounceEaseOut(float time)
        {
            return BounceEaseOut(time, min, max, duration);
        }

        public static float BounceEaseOut(float time, float min, float max, float duration)
        {
            if ((time /= duration) < (1 / 2.75f))
                return max * (7.5625f * time * time) + min;
            else if (time < (2 / 2.75f))
                return max * (7.5625f * (time -= (1.5f / 2.75f)) * time + .75f) + min;
            else if (time < (2.5f / 2.75f))
                return max * (7.5625f * (time -= (2.25f / 2.75f)) * time + .9375f) + min;
            else
                return max * (7.5625f * (time -= (2.625f / 2.75f)) * time + .984375f) + min;
        }

        public static float BounceEaseIn(float time)
        {
            return BounceEaseIn(time, min, max, duration);
        }

        public static float BounceEaseIn(float time, float min, float max, float duration)
        {
            return max - BounceEaseOut(duration - time, 0, max, duration) + min;
        }

        public static float BounceEaseInOut(float time)
        {
            if (time < duration / 2f)
            {
                return BounceEaseIn(time * 2, 0, max, duration) * 0.5f + min;
            }

            return BounceEaseOut(time * 2 - duration, 0, max, duration) * 0.5f + max * 0.5f + min;
        }

        public static float BounceEaseOutIn(float time)
        {
            if (time < duration / 2f)
            {
                return BounceEaseOut(time * 2, min, max / 2, duration);
            }

            return BounceEaseIn((time * 2) - duration, min + max / 2f, duration / 2f, duration);
        }

        public static float BackEaseOut(float time)
        {
            return max * ((time = time / duration - 1) * time * ((1.70158f + 1) * time + 1.70158f) + 1) + min;
        }

        public static float BackEaseIn(float time)
        {
            return max * (time /= duration) * time * ((1.70158f + 1) * time - 1.70158f) + min;
        }
    }
}
