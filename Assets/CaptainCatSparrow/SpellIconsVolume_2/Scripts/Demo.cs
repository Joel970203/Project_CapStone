using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.CaptainCatSparrow.SpellIconsVolume_2.Druid.Demo
{
    public class Demo : MonoBehaviour
    {
        [SerializeField]
        private Sprite[] icons;
        [SerializeField]
        private GameObject iconPrefab;
        private ScrollRect _scroll;
        // Start is called before the first frame update
        void Start()
        {
            var canvas = GameObject.Find("Canvas").transform;
            _scroll = canvas.Find("Scroll/Scroll View").GetComponent<ScrollRect>();
            PopulateGrid();
        }

        private void PopulateGrid()
        {
            foreach (var sprite in icons)
            {
                var icon = Instantiate(iconPrefab);
                icon.transform.Find("Mask/Image").GetComponent<Image>().sprite = sprite;
                icon.transform.SetParent(_scroll.content);
            }
        }
    }
}
