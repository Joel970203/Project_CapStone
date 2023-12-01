#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
#define PRE_UNITY_5_3
#endif

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

#if !PRE_UNITY_5_3
using UnityEngine.SceneManagement;
#endif

namespace UI.Pagination.Examples
{
    /// <summary>
    /// Controller for the Example Scene
    /// </summary>
    class PaginationExampleController : MonoBehaviour
    {
        #pragma warning disable 0649
        [Header("Horizontal Example")]
        public PagedRect HorizontalPaginationExample = null;
        public InputField HorizontalAnimationSpeedTextField = null;
        public InputField HorizontalDelayTextField = null;
        public List<Button> HorizontalAnimationTypeButtons = null;
        public InputField HorizontalMaximumNumberOfButtonsToShowField = null;

        [Header("Vertical Example")]
        public PagedRect VerticalPaginationExample = null;
        public InputField VerticalAnimationSpeedTextField = null;
        public InputField VerticalDelayTextField = null;
        public List<Button> VerticalAnimationTypeButtons = null;
        public InputField VerticalMaximumNumberOfButtonsToShowField = null;

        [Header("Dynamic Pages Example")]
        public PagedRect DynamicPagesExample = null;
        public Button ToggleLastPageButton = null;

        [Header("Slider Example")]
        public PagedRect SliderExample = null;

        [Header("Character Creation Example")]
        public PagedRect CharacterCreationExample = null;
        public InputField CharacterCreationNameField = null;        
        public List<Button> CharacterCreationClassButtons = null;
        public Color CharacterCreationButtonNormalColor;
        public Color CharacterCreationButtonHighlightedColor;
        public List<PagedRect> CharacterCreation_StatInputList = null;
        public InputField CharacterCreation_UnallocatedStatPointsInput = null;

        public Page CharacterCreation_StatsPage = null;
        public Page CharacterCreation_CreatePage = null;
        
        protected string characterCreation_SelectedClass = null;

        [Header("Slider ScrollRect Example")]
        public PagedRect SliderScrollRectExample = null;

        [Header("Page Previews Example")]
        public PagedRect PagePreviewsHorizontalExample = null;
        public PagedRect PagePreviewsVerticalExample = null;

        [Header("Nested ScrollRect Example")]
        public PagedRect NestedScrollRectExample = null;

        [Header("Tabs Example")]
        public PagedRect TabsHorizontalScrollRectExample = null;
        
        [Header("Theme")]
        public Color ExampleButtonNormalColor;
        public Color ExampleButtonHighlightedColor;                

        [Header("Controls")]
        public List<Button> ControlButtons = null;

        private List<PagedRect> examples = new List<PagedRect>();        
        #pragma warning restore 0649

        void Start()
        {            
            // collection for convenience when switching examples
            examples = new PagedRect[] 
            { 
                HorizontalPaginationExample, 
                VerticalPaginationExample, 
                DynamicPagesExample, 
                SliderExample, 
                CharacterCreationExample,
                SliderScrollRectExample,
                PagePreviewsHorizontalExample,
                PagePreviewsVerticalExample,
                NestedScrollRectExample,
                TabsHorizontalScrollRectExample
            }
            .Where(e => e != null).ToList();            
        }
        
        /// <summary>
        /// Reloads the current scene
        /// </summary>
        public void ResetExample()
        {   
#if !PRE_UNITY_5_3         
            // For Unity 5.3 and above:
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
#else
            // For versions below 5.3
            Application.LoadLevel(Application.loadedLevelName);
#endif
        }

        /// <summary>
        /// Switch from one example to another
        /// </summary>
        /// <param name="newExample"></param>
        public void SwitchExample(string newExample)
        {
            examples.ForEach(e => e.gameObject.SetActive(false));

            PagedRect example = null;

            switch (newExample)
            {
                case "HorizontalPaginationExample":
                    example = HorizontalPaginationExample;
                    break;
                case "VerticalPaginationExample":
                    example = VerticalPaginationExample;
                    break;
                case "DynamicPagesExample":
                    example = DynamicPagesExample;
                    break;
                case "SliderExample":
                    example = SliderExample;
                    break;
                case "CharacterCreationExample":
                    example = CharacterCreationExample;
                    break;
                case "SliderScrollRectExample":
                    example = SliderScrollRectExample;
                    break;
                case "PagePreviewsHorizontalExample":
                    example = PagePreviewsHorizontalExample;
                    break;
                case "PagePreviewsVerticalExample":
                    example = PagePreviewsVerticalExample;
                    break;
                case "NestedScrollRectExample":
                    example = NestedScrollRectExample;
                    break;
                case "TabsHorizontalScrollRectExample":
                    example = TabsHorizontalScrollRectExample;
                    break;
            }

            example.gameObject.SetActive(true);
            example.SetCurrentPage(1, true);
        }

        public void HighlightExampleButton(Button selectedButton)
        {
            ControlButtons.ForEach(b =>
            {
                var colors = b.colors;
                colors.normalColor = ExampleButtonNormalColor;
                b.colors = colors;

                b.image.color = ExampleButtonNormalColor;
            });

            var selectedButtonColors = selectedButton.colors;
            selectedButtonColors.normalColor = ExampleButtonHighlightedColor;
            selectedButton.colors = selectedButtonColors;

            selectedButton.image.color = ExampleButtonHighlightedColor;
        }

        #region Horizontal
        public void SetHorizontalAnimationSpeedText(float animationSpeed)
        {
            HorizontalAnimationSpeedTextField.text = animationSpeed.ToString();
        }

        public void HighlightHorizontalAnimationTypeButton(Button selectedButton)
        {
            HorizontalAnimationTypeButtons.ForEach(b =>
            {
                var colors = b.colors;
                colors.normalColor = ExampleButtonNormalColor;
                b.colors = colors;

                b.image.color = ExampleButtonNormalColor;
            });

            var selectedButtonColors = selectedButton.colors;
            selectedButtonColors.normalColor = ExampleButtonHighlightedColor;
            selectedButton.colors = selectedButtonColors;

            selectedButton.image.color = ExampleButtonHighlightedColor;
        }

        public void SetHorizontalDelayText(float delay)
        {
            HorizontalDelayTextField.text = delay.ToString() + "s";
        }
                
        public void SetHorizontalMaximumNumberOfButtonsToShowText(float maxNumber)
        {
            HorizontalMaximumNumberOfButtonsToShowField.text = maxNumber.ToString();
        }
        #endregion

        #region Vertical
        public void SetVerticalAnimationSpeedText(float animationSpeed)
        {
            VerticalAnimationSpeedTextField.text = animationSpeed.ToString();
        }
        
        public void HighlightVerticalAnimationTypeButton(Button selectedButton)
        {
            VerticalAnimationTypeButtons.ForEach(b =>
            {
                var colors = b.colors;
                colors.normalColor = ExampleButtonNormalColor;
                b.colors = colors;

                b.image.color = ExampleButtonNormalColor;
            });

            var selectedButtonColors = selectedButton.colors;
            selectedButtonColors.normalColor = ExampleButtonHighlightedColor;
            selectedButton.colors = selectedButtonColors;

            selectedButton.image.color = ExampleButtonHighlightedColor;
        }

        public void SetVerticalDelayText(float delay)
        {
            VerticalDelayTextField.text = delay.ToString() + "s";
        }

        public void SetVerticalMaximumNumberOfButtonsToShowText(float maxNumber)
        {
            VerticalMaximumNumberOfButtonsToShowField.text = maxNumber.ToString();
        }
        #endregion

        #region Dynamic Pages
        public void DynamicPageExample_AddPage()
        {
            var page = DynamicPagesExample.AddPageUsingTemplate();
            page.PageTitle = "Page " + DynamicPagesExample.NumberOfPages;
            
            // Calling UpdatePagination so as to update the PageTitle value
            DynamicPagesExample.UpdatePagination();

            // we can now customise the template
            var text = page.GetComponentsInChildren<Text>().FirstOrDefault(t => t.name == "Text");

            if (text != null)
            {
                text.text = "This is a dynamically added page.\r\nIts page number is " + DynamicPagesExample.NumberOfPages + ".";
            }

            ToggleLastPageButton.interactable = true;
        }

        public void DynamicPageExample_RemoveLastPage()
        {
            var lastPage = DynamicPagesExample.Pages.LastOrDefault(l => l.PageTitle != "Main Page");
                        
            if (lastPage != null)
            {
                DynamicPagesExample.RemovePage(lastPage, true);

                // Disable the Toggle button if need be
                if (DynamicPagesExample.NumberOfPages <= 1)
                {
                    ToggleLastPageButton.interactable = false;
                }
            }
        }

        public void DynamicPageExample_ToggleLastPage()
        {
            var lastPage = DynamicPagesExample.Pages.LastOrDefault(l => l.PageTitle != "Main Page");

            if (lastPage != null)
            {
                if (lastPage.PageEnabled)
                {
                    lastPage.DisablePage();
                }
                else
                {
                    lastPage.EnablePage();
                }
            }
        }

        #endregion

        #region Slider
        #endregion

        #region Character Creation Example
        public void CharacterCreation_CheckIfStatsPageShouldBeEnabled()
        {
            bool nameSet = !String.IsNullOrEmpty(CharacterCreationNameField.text);                        
            bool classSet = !String.IsNullOrEmpty(this.characterCreation_SelectedClass);


            var pageEnabled = CharacterCreation_StatsPage.PageEnabled;
            
            if (nameSet && classSet)
            {
                CharacterCreation_StatsPage.PageEnabled = true;

                // Note: if you want to trigger an event when the "Confirm" button is clicked,
                // the easiest way to set this up is to add an event handler to the 
                // 'OnShow' section of the Confirm page
                
                // As this is an example, we'll stop here                
            }
            else
            {
                CharacterCreation_StatsPage.PageEnabled = false;
            }

            // If something changed
            if (pageEnabled != CharacterCreation_StatsPage.PageEnabled)
            {
                CharacterCreationExample.UpdatePagination();
            }
        }        

        public void CharacterCreation_HighlightClassButton(Button selectedButton)
        {
            CharacterCreationClassButtons.ForEach(b =>
            {
                var colors = b.colors;
                colors.normalColor = CharacterCreationButtonNormalColor;
                b.colors = colors;

                b.image.color = CharacterCreationButtonNormalColor;
            });

            var selectedButtonColors = selectedButton.colors;
            selectedButtonColors.normalColor = CharacterCreationButtonHighlightedColor;
            selectedButton.colors = selectedButtonColors;

            selectedButton.image.color = CharacterCreationButtonHighlightedColor;
        }

        public void CharacterCreation_SetClass(string _class)
        {            
            characterCreation_SelectedClass = _class;
            
            CharacterCreation_CheckIfStatsPageShouldBeEnabled();            
        }
        
        public void CharacterCreation_StatsUpdate(Page NewPage, Page OldPage)
        {
            int unallocatedStatPoints = 35; // 5 * 5 + 10            

            foreach (var pagedRect in CharacterCreation_StatInputList)
            {
                var value = pagedRect.CurrentPage;

                unallocatedStatPoints -= value;
            }

            // Disable the pagedrect next buttons if need be
            var disableNextButtons = unallocatedStatPoints > 0;
            CharacterCreation_StatInputList.ForEach(i => i.Button_NextPage.Button.interactable = disableNextButtons);
                        
            CharacterCreation_UnallocatedStatPointsInput.text = unallocatedStatPoints.ToString();

            var oldValue = CharacterCreation_CreatePage.PageEnabled;
            CharacterCreation_CreatePage.PageEnabled = unallocatedStatPoints <= 0;
            if (oldValue != CharacterCreation_CreatePage.PageEnabled)
            {
                CharacterCreationExample.UpdatePagination();
            }
        }

        #endregion

        /// <summary>
        /// Call a function after the specified delay.
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="call"></param>
        /// <returns></returns>
        public System.Collections.IEnumerator DelayedCall(float delay, Action call)
        {
            yield return new WaitForSeconds(delay);

            call.Invoke();
        }
    }
}
