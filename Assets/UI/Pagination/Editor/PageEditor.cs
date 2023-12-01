using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
using System.Reflection;
using System;

namespace UI.Pagination
{
    [CustomEditor(typeof(Page))]
    public partial class PageEditor : Editor
    {        
        public override void OnInspectorGUI()
        {                    
            if (DrawDefaultInspector())
            {
                var page = (Page)target;
                var pagedRect = page.GetPagedRect();
                
                if (pagedRect != null)
                {
                    PagedRectTimer.DelayedCall(0f, () => page.NotifyPagedRectOfChange(), pagedRect);
                }
            }
        }        
    }
}
