using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Reflection;

namespace UI.Pagination
{
    class PassThroughDragEvents : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public List<GameObject> Targets = null;
        public List<string> DesiredTargetTypes = null;
        Dictionary<string, Dictionary<MonoBehaviour, MethodInfo>> m_Events = new Dictionary<string, Dictionary<MonoBehaviour, MethodInfo>>();

        static List<string> eventTypes = new List<string>()
        {
            "OnBeginDrag",
            "OnEndDrag",
            "OnDrag"
        };

        Vector2 m_dragStartPosition = new Vector2();
        Vector2 m_dragEndPosition = new Vector2();
        Vector2 m_delta = new Vector2();
        bool m_dragging = false;

        public bool PassThroughHorizontalDragEvents = true;
        public bool PassThroughVerticalDragEvents = true;

        void Start()
        {
            Initialise();
        }

        public void Initialise()
        {
            m_Events.Clear();

            if (Targets == null || Targets.Count == 0 || DesiredTargetTypes == null || DesiredTargetTypes.Count == 0) return;

            foreach (var eventType in eventTypes)
            {
                foreach (var target in Targets)
                {
                    if (target == null) continue;

                    var components = target.GetComponents<MonoBehaviour>();
                    foreach (var component in components)
                    {
                        var type = component.GetType();

                        if (!DesiredTargetTypes.Contains(type.Name)) continue;

                        var methodInfo = type.GetMethod(eventType, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                        if (methodInfo != null)
                        {
                            if (!m_Events.ContainsKey(eventType))
                            {
                                m_Events.Add(eventType, new Dictionary<MonoBehaviour, MethodInfo>());
                            }

                            m_Events[eventType].Add(component, methodInfo);
                        }
                    }
                }
            }
        }

        void Update()
        {
            if (m_dragging)
            {
                m_dragStartPosition = Input.mousePosition;
            }
        }

        public void OnEndDrag(PointerEventData data)
        {
            if (!m_dragging) return;

            m_dragging = false;

            // if this object is used on a scrollview, data.delta is always (0,0) for some reason, so we need to calculate it ourselves
            m_dragEndPosition = Input.mousePosition;
            m_delta = m_dragEndPosition - m_dragStartPosition;
            data.delta = m_delta;

            if (!m_Events.ContainsKey("OnEndDrag")) return;

            foreach (var kvp in m_Events["OnEndDrag"])
            {
                kvp.Value.Invoke(kvp.Key, new object[] { data });
            }
        }

        public void OnBeginDrag(PointerEventData data)
        {
            var analysis = new DragEventAnalysis(data);

            if (!(this.PassThroughHorizontalDragEvents && analysis.DragPlane == DragEventAnalysis.eDragPlane.Horizontal)
             && !(this.PassThroughVerticalDragEvents && analysis.DragPlane == DragEventAnalysis.eDragPlane.Vertical))
            {
                return;
            }

            m_dragging = true;

            if (!m_Events.ContainsKey("OnBeginDrag")) return;

            foreach (var kvp in m_Events["OnBeginDrag"])
            {
                kvp.Value.Invoke(kvp.Key, new object[] { data });
            }
        }

        public void OnDrag(PointerEventData data)
        {
            if (!m_dragging) return;
            if (!m_Events.ContainsKey("OnDrag")) return;

            foreach (var kvp in m_Events["OnDrag"])
            {
                kvp.Value.Invoke(kvp.Key, new object[] { data });
            }
        }
    }
}
