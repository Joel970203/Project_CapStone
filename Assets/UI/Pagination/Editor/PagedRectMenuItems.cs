using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI.Pagination
{
    static class PagedRectMenuItems
    {
        [UnityEditor.MenuItem("GameObject/UI/Pagination/Horizontal Pagination - ScrollRect")]
        static void AddHorizontalPaginationScrollRectPrefab()
        {
            PaginationUtilities.InstantiatePrefab("HorizontalPagination - ScrollRect");
        }

        [UnityEditor.MenuItem("GameObject/UI/Pagination/Vertical Pagination - ScrollRect")]
        static void AddVerticalPaginationScrollRectPrefab()
        {
            PaginationUtilities.InstantiatePrefab("VerticalPagination - ScrollRect");
        }

        [UnityEditor.MenuItem("GameObject/UI/Pagination/Slider - ScrollRect")]
        static void AddSliderScrollRectPrefab()
        {
            PaginationUtilities.InstantiatePrefab("Slider - ScrollRect");
        }

        [UnityEditor.MenuItem("GameObject/UI/Pagination/Page Previews - Horizontal")]
        static void AddHorizontalPagePreviewsPrefab()
        {
            PaginationUtilities.InstantiatePrefab("Page Previews - Horizontal");
        }

        [UnityEditor.MenuItem("GameObject/UI/Pagination/Page Previews - Vertical")]
        static void AddVerticalPagePreviewsPrefab()
        {
            PaginationUtilities.InstantiatePrefab("Page Previews - Vertical");
        }

        [UnityEditor.MenuItem("GameObject/UI/Pagination/Slider - ScrollRect (With Nested ScrollRect)")]
        static void AddSliderScrollRectWithNestedScrollRectPrefab()
        {
            PaginationUtilities.InstantiatePrefab("Slider - ScrollRect (With Nested ScrollRect)");
        }

        [UnityEditor.MenuItem("GameObject/UI/Pagination/Tabs - Horizontal - ScrollRect")]
        static void AddTabsHorizontalScrollRect()
        {
            PaginationUtilities.InstantiatePrefab("Tabs - Horizontal - ScrollRect");
        }

        [UnityEditor.MenuItem("GameObject/UI/Pagination/(Legacy) Horizontal Pagination")]
        static void AddHorizontalPaginationPrefab()
        {
            PaginationUtilities.InstantiatePrefab("HorizontalPagination");
        }

        [UnityEditor.MenuItem("GameObject/UI/Pagination/(Legacy) Vertical Pagination")]
        static void AddVerticalPaginationPrefab()
        {
            PaginationUtilities.InstantiatePrefab("VerticalPagination");
        }

        [UnityEditor.MenuItem("GameObject/UI/Pagination/(Legacy) Slider")]
        static void AddSliderPrefab()
        {
            PaginationUtilities.InstantiatePrefab("Slider");
        }
    }
}
