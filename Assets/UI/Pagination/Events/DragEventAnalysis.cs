using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI.Pagination
{        
    public class DragEventAnalysis
    {
        private PointerEventData data;

        public enum eDragPlane
        {
            Horizontal,
            Vertical,
            None
        }

        public eDragPlane DragPlane
        {
            get
            {
                if (Math.Abs(data.delta.x) > Math.Abs(data.delta.y))
                {
                    return eDragPlane.Horizontal;
                }

                if (Math.Abs(data.delta.y) > Math.Abs(data.delta.x))
                {
                    return eDragPlane.Vertical;
                }

                return eDragPlane.None;
            }
        }
            
        public DragEventAnalysis(PointerEventData data)
        {
            this.data = data;                
        }            
    }    
}
