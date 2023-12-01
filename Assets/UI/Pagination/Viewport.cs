using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI.Pagination
{
    [ExecuteInEditMode]
    public class Viewport : MonoBehaviour
    {
        private PagedRect _pagedRect;

        public void Initialise(PagedRect pagedRect)
        {
            this._pagedRect = pagedRect;
        }

        void OnRectTransformDimensionsChange()
        {            
            if (_pagedRect == null) return;            

            _pagedRect.ViewportDimensionsChanged();
        } 
    }
}
