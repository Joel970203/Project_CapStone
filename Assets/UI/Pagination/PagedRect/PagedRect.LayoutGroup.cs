using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Pagination
{
    public partial class PagedRect
    {
        public void UpgradeLayoutGroupIfNecessary()
        {
            if (!UsingScrollRect) return;

            var layoutGroup = ScrollRect.content.GetComponent<LayoutGroup>();

            if (!(layoutGroup is PagedRect_LayoutGroup))
            {
                RemoveLayoutGroup(layoutGroup);

                AddPagedRectLayoutGroup();
            }
            else
            {
                ((PagedRect_LayoutGroup)layoutGroup).pagedRect = this;
            }
        }

        void AddPagedRectLayoutGroup()
        {
            var newLayoutGroup = ScrollRect.content.gameObject.AddComponent<PagedRect_LayoutGroup>();

            newLayoutGroup.pagedRect = this;
            newLayoutGroup.Axis = ScrollRect.horizontal ? RectTransform.Axis.Horizontal : RectTransform.Axis.Vertical;            
        }

        void RemoveLayoutGroup(LayoutGroup layoutGroup)
        {
            DestroyImmediate(layoutGroup);            
        }
    }
}
