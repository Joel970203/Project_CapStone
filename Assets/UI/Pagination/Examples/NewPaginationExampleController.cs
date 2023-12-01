using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

namespace UI.Pagination.Examples
{
    /// <summary>
    /// Controller for the Example Scene
    /// </summary>
    class NewPaginationExampleController : MonoBehaviour
    {
        private Dictionary<string, PagedRect> exampleGameObjects = new Dictionary<string, PagedRect>();

        private void Start()
        {
            exampleGameObjects = new Dictionary<string, PagedRect>()
            {
                {"Image Gallery", imageGallery},
                {"Page Previews", pagePreviews},
                {"Mobile Menu", mobileMenu },
                {"Features", features},
                {"Simple", simple}
            };


            InitializeImageGallery();
            InitializePagePreviews();

            ShowExample("Image Gallery");
        }

        public void ShowExample(string name)
        {
            if (exampleGameObjects.ContainsKey(name))
            {
                foreach(var kvp in exampleGameObjects)
                {
                    if (kvp.Key != name) kvp.Value.gameObject.SetActive(false);
                }

                exampleGameObjects[name].gameObject.SetActive(true);
                exampleGameObjects[name].SetCurrentPage(1, true);
            }
        }

        #region Image Gallery
        [Header("Image Gallery")]
        public PagedRect imageGallery = null;
        public Dropdown ig_easingDropdown = null;

        private void InitializeImageGallery()
        {
            var easingTypes = Enum.GetNames(typeof(PagedRect.eAnimationCurve));
            ig_easingDropdown.options = easingTypes.Select(c => new Dropdown.OptionData(c)).ToList();
            ig_easingDropdown.value = (int)PagedRect.eAnimationCurve.CubicEaseOut;
            ig_easingDropdown.RefreshShownValue();

            ig_easingDropdown.onValueChanged.AddListener((i) => imageGallery.AnimationCurve = (PagedRect.eAnimationCurve)i);

        }
        #endregion

        #region Page Previews
        [Header("Page Previews")]
        public PagedRect pagePreviews = null;
        public Dropdown pp_easingDropdown = null;

        private void InitializePagePreviews()
        {
            var easingTypes = Enum.GetNames(typeof(PagedRect.eAnimationCurve));
            pp_easingDropdown.options = easingTypes.Select(c => new Dropdown.OptionData(c)).ToList();
            pp_easingDropdown.value = (int)PagedRect.eAnimationCurve.CubicEaseIn;
            pp_easingDropdown.RefreshShownValue();

            pp_easingDropdown.onValueChanged.AddListener((i) => pagePreviews.AnimationCurve = (PagedRect.eAnimationCurve)i);
        }
        #endregion

        #region Mobile Menu
        [Header("Mobile Menu")]
        public PagedRect mobileMenu = null;
        #endregion

        #region Features
        [Header("Features")]
        public PagedRect features = null;


        #endregion

        #region Simple
        [Header("Simple")]
        public PagedRect simple = null;
        #endregion
    }
}
