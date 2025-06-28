using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SearchBar : MonoBehaviour
{
    // Start is called before the first frame update
    public InputField searchInput;
    public RectTransform content;
    public float spacing = 10f;

    public List<GameObject> allImages = new List<GameObject>();

    void Start()
    {
        foreach(Transform child in content) {
            allImages.Add(child.gameObject);
        }
        searchInput.onValueChanged.AddListener(SearchList);
    }

    void SearchList(string input)
    {
        input = input.ToLower();
        float yOffset = 0f;

        foreach(GameObject item in allImages)
        {
            Image image = item.GetComponent<Image>();
            string imageName = image.sprite.name.ToLower();

            if(imageName.Contains(input))
            {
                item.SetActive(true);
                RectTransform itemRect = item.GetComponent<RectTransform>();
                itemRect.anchoredPosition = new Vector2(itemRect.anchoredPosition.x, -yOffset);

                yOffset += itemRect.rect.height + spacing;
            } else
            {
                item.SetActive(false);
            }
        }

        content.sizeDelta = new Vector2(content.sizeDelta.x, yOffset);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
