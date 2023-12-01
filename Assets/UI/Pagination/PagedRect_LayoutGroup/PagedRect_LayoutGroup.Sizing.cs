using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI.Pagination
{    
    public partial class PagedRect_LayoutGroup
    {                
        /*void RecalculateDimensions()
        {
            float totalSize = 0f;
            for (int i = 0; i < rectChildren.Count; i++)
            {
                var page = rectChildren[i].GetComponent<Page>();

                if (page == null) continue; // ignore any non-page children

                if (IsVertical)
                {
                    totalSize += page.layoutElement.preferredHeight * page.DesiredScale.y;
                }
                else
                {
                    totalSize += page.layoutElement.preferredWidth * page.DesiredScale.x;
                }
            }

            if (IsVertical)
            {

            }
            else
            {

            }
        }

        protected override void OnTransformChildrenChanged()
        {
            base.OnTransformChildrenChanged();

            RecalculateDimensions();
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();

            RecalculateDimensions();
        }*/
    }
}
