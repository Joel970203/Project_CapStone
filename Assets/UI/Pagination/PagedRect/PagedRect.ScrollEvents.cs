using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Pagination
{
    public partial class PagedRect
    {
        /// <summary>
        /// Called whenever the ScrollRect's position changes
        /// </summary>
        /// <param name="newPosition"></param>
        protected void ScrollRectValueChanged(Vector2 newPosition)
        {
            _ScrollRectPosition = newPosition;

            var isDraggablePagePreviews = !ScrollRect.DisableDragging;

            if (!ShowPagePreviews || isDraggablePagePreviews)
            {
                UpdateSeamlessPagePositions();
            }
        }

        void ScrollWheelUp()
        {
            HandleScrollWheel(DeltaDirection.Next);
        }

        void ScrollWheelDown()
        {
            HandleScrollWheel(DeltaDirection.Previous);
        }

        void HandleScrollWheel(DeltaDirection direction)
        {
            bool handled = false;

            if (LoopSeamlessly && !ShowPagePreviews)
            {
                // Slightly different logic if we're on the first or last pages
                var pagePosition = GetPagePosition(CurrentPage);
                if (NumberOfPages > 3 && (pagePosition == 1 || pagePosition == NumberOfPages))
                {
                    if (direction == DeltaDirection.Next)
                    {
                        MoveFirstPageToEnd();
                        NextPage();
                        handled = true;
                    }
                    else if (direction == DeltaDirection.Previous)
                    {
                        MoveLastPageToStart();
                        PreviousPage();
                        handled = true;
                    }
                }
            }

            if(!handled)
            {
                if (direction == DeltaDirection.Next)
                {
                    NextPage();
                }
                else if (direction == DeltaDirection.Previous)
                {
                    PreviousPage();
                }
            }
        }
    }
}
