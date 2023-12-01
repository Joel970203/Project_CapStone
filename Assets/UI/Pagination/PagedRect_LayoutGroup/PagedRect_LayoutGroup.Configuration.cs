using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI.Pagination
{    
    public partial class PagedRect_LayoutGroup
    {
        public PagedRect pagedRect;

        [SerializeField]
        protected RectTransform.Axis m_Axis = RectTransform.Axis.Horizontal;
        public RectTransform.Axis Axis { get { return m_Axis; } set { SetProperty<RectTransform.Axis>(ref m_Axis, value); } }

        public bool IsVertical { get { return m_Axis == RectTransform.Axis.Vertical; } }
    }
}
