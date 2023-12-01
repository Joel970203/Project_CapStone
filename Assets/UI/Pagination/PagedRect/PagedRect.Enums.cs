using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UI.Pagination
{
    public partial class PagedRect
    {        
        public enum eAnimationType
        {
            None,
            SlideHorizontal,
            SlideVertical,
            Fade
        }

        public enum eDirection
        {
            Left,
            Right
        }

        public enum DeltaDirection
        {
            None,
            Next,
            Previous
        }
    }
}
