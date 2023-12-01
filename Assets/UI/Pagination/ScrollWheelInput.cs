using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI.Pagination
{    
    public class ScrollWheelInput : MonoBehaviour
    {                                
        public Action OnScrollUp = null;
        public Action OnScrollDown = null;
        
        public float UpdateRate = 0.125f;

        private float lastUpdated = 0f;
        
        void Update()
        {
            lastUpdated += Time.deltaTime;
            
            var input = Input.GetAxis("Mouse ScrollWheel");            
            if (input != 0f)
            {                
                if (lastUpdated >= UpdateRate || Time.timeScale == 0)
                {
                    if (input < 0f)
                    {
                        if (OnScrollUp != null)
                        {
                            OnScrollUp();
                        }
                    }
                    else
                    {
                        if (OnScrollDown != null)
                        {
                            OnScrollDown();
                        }
                    }

                    lastUpdated = 0f;
                }
            }
        }
    }
}
