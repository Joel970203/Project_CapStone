using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UI.Pagination
{
    public partial class PagedRect
    {
        public void ViewportDimensionsChanged()
        {
            if (this == null || this.gameObject == null || !this.gameObject.activeInHierarchy) return;

            for (var x = 0; x < Pages.Count; x++)
            {
                Pages[x].UpdateDimensions();
            }

            if (UsingScrollRect)
            {
                if (Application.isPlaying)
                {
                    PagedRectTimer.DelayedCall(0.05f, () => { CenterScrollRectOnCurrentPage(true); }, this);
                }
                else
                {
                    SetScrollRectPosition();
                }
            }
        }
    }
}
