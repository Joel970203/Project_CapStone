using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

namespace UI.Pagination
{
    internal class DelayedEditorAction
    {
        internal double TimeToExecute;
        internal Action Action;
        internal MonoBehaviour ActionTarget;

        public DelayedEditorAction(double timeToExecute, Action action, MonoBehaviour actionTarget)
        {
            TimeToExecute = timeToExecute;
            Action = action;
            ActionTarget = actionTarget;
        }
    }

    public static class PagedRectTimer
    {
#if UNITY_EDITOR
        static List<DelayedEditorAction> delayedEditorActions = new List<DelayedEditorAction>();

        static PagedRectTimer()
        {
            if (!Application.isPlaying) UnityEditor.EditorApplication.update += EditorUpdate;
        }
#endif

        static void EditorUpdate()
        {
#if UNITY_EDITOR
            if (Application.isPlaying) return;

            var actionsToExecute = delayedEditorActions.Where(dea => UnityEditor.EditorApplication.timeSinceStartup >= dea.TimeToExecute).ToList();

            if (!actionsToExecute.Any()) return;

            for (var x = 0; x < actionsToExecute.Count; x++)
            {
                try
                {
                    if (actionsToExecute[x].ActionTarget != null && !PaginationUtilities.IsPrefab(actionsToExecute[x].ActionTarget)) // don't execute if the target is gone
                    {
                        actionsToExecute[x].Action.Invoke();
                    }
                }
                finally
                {
                    delayedEditorActions.Remove(actionsToExecute[x]);
                }
            }
#endif
        }

        public static void DelayedCall(float delay, Action action, MonoBehaviour actionTarget)
        {
            if (Application.isPlaying)
            {
                if (actionTarget.gameObject.activeInHierarchy) actionTarget.StartCoroutine(_DelayedCall(delay, action));
            }
#if UNITY_EDITOR
            else
            {
                delayedEditorActions.Add(new DelayedEditorAction(UnityEditor.EditorApplication.timeSinceStartup + delay, action, actionTarget));
            }
#endif
        }

        private static IEnumerator _DelayedCall(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);

            action.Invoke();
        }
    }
}
