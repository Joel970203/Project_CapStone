using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
using System.Reflection;
using System;

namespace UI.Pagination
{
    [CustomEditor(typeof(PagedRect_ScrollRect))]
    public partial class PagedRect_ScrollRectEditor : Editor
    {
        //public SerializedObject so;
        //public SerializedProperty DisableDragging;

        public void OnEnable()
        {
          //  so = new SerializedObject(target);
          //  DisableDragging = so.FindProperty("DisableDragging");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();

            //EditorGUILayout.PropertyField(DisableDragging);
        }        
    }
}
