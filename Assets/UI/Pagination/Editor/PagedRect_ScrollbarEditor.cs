using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
using System.Reflection;
using System;

namespace UI.Pagination
{
    [CustomEditor(typeof(PagedRect_Scrollbar))]
    public partial class PagedRect_ScrollbarEditor : Editor
    {                
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();            
        }        
    }
}
