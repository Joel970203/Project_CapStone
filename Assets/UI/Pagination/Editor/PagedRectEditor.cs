using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
using System.Reflection;
using System;

namespace UI.Pagination
{
    [CustomEditor(typeof(PagedRect))]
    public partial class PagedRectEditor : Editor
    {
        private int currentPage = -1;
        private GUIStyle headerStyle;

        public override void OnInspectorGUI()
        {        
            if(headerStyle == null)
            {
                InitStyles();
            }

            var pagedRect = (PagedRect)target;   
            if (currentPage == -1) 
            { 
                currentPage = pagedRect.editorSelectedPage != 0 ? pagedRect.editorSelectedPage : pagedRect.CurrentPage;                
            }            

            if (GUILayout.Button("Update Pagination"))
            {                
                pagedRect.UpdatePages(true, !pagedRect.LoopSeamlessly);
            }
                        
            EditorGUILayout.Space();

            var numberOfPages = pagedRect.NumberOfPages;
            var pages = Enumerable.Range(1, numberOfPages).Select(p => p.ToString()).ToArray();
            var currentPageTemp = currentPage;
                        
            GUILayout.Label("Set Current Page", headerStyle);            
            currentPage = GUILayout.SelectionGrid(Math.Max(0, currentPage - 1), pages, 5) + 1;                        

            if (currentPage != currentPageTemp)
            {
                pagedRect.isDirty = true;
                pagedRect.SetCurrentPage(currentPage);
                pagedRect.SetEditorSelectedPage(pagedRect.CurrentPage);
            }

            EditorGUILayout.Separator();

            if (DrawDefaultInspector())
            {
                pagedRect.isDirty = true;
            }
        }

        void InitStyles()
        {
            headerStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,                
                fontSize = 15,
                fontStyle = FontStyle.Bold
            };
        }        
    }
}
