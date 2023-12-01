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
        /// Call a function after the specified delay.
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="call"></param>
        /// <returns></returns>
        public System.Collections.IEnumerator DelayedCall(float delay, Action call)
        {
            if (delay == 0)
            {
                yield return new WaitForEndOfFrame();
            }
            else
            {
                yield return new WaitForSeconds(delay);
            }

            call.Invoke();
        }

#if UNITY_EDITOR
        protected void DelayedEditorAction(double delay, Action action)
        {
            delayedEditorActions.Add(new KeyValuePair<double, Action>(UnityEditor.EditorApplication.timeSinceStartup + delay, action));
        }
#endif
    }
}
