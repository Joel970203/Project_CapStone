using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UI.Pagination
{
    public partial class PagedRect
    {
        protected void ShowHighlight()
        {
            imageComponent.color = HighlightColor;
        }

        protected void ClearHighlight()
        {
            imageComponent.color = NormalColor;
        } 
    }
}
