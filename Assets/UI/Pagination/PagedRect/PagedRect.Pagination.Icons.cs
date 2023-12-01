using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Pagination
{
    public partial class PagedRect
    {
        private void InitializePaginationIcons()
        {            
            var buttonTemplates = new List<PaginationButton>
            {
                ButtonTemplate_CurrentPage,
                ButtonTemplate_OtherPages,
                ButtonTemplate_DisabledPage
            }
            .Where(b => b != null)
            .ToList();

            foreach (var template in buttonTemplates)
            {
                var layoutGroup = template.gameObject.GetComponent<HorizontalLayoutGroup>();

                if (layoutGroup == null)
                {
                    layoutGroup = template.gameObject.AddComponent<HorizontalLayoutGroup>();
                    layoutGroup.childAlignment = TextAnchor.MiddleCenter;

#if UNITY_5_6_OR_NEWER
                layoutGroup.childControlHeight = true;
                layoutGroup.childControlWidth = true;
#endif
                }

                var layoutElement = template.gameObject.GetComponent<LayoutElement>();                
                if(layoutElement.flexibleWidth == -1) layoutElement.flexibleWidth = 0;

                if (template.Icon == null)
                {                    
                    var iconGO = new GameObject("Icon", typeof(RectTransform), typeof(Image));
                    iconGO.transform.SetParent(template.transform);
                    iconGO.transform.SetAsFirstSibling();

                    template.Icon = iconGO.GetComponent<Image>();
                    template.Icon.raycastTarget = false;

                    iconGO.SetActive(false);
                }
            }
        }
    }
}
