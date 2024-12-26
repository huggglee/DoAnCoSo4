using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CategoryManager : MonoBehaviour
{
    public GameObject buttonPrefab; // Gán prefab button vào Inspector
    public Transform buttonContainer; // Gán Panel chứa các button
    public GameObject buttonPrefab2; // Gán prefab button vào Inspector
    public Transform buttonContainer2; // Gán Panel chứa các button
    public Animator animator;


    //public Button easyButton; // Gán nút mức độ dễ vào Inspector
    //public Button mediumButton; // Gán nút mức độ trung bình vào Inspector
    //public Button hardButton; // Gán nút mức độ khó vào Inspector

    private string selectedCategoryId;
    private string selectedLevelId;

    private void Start()
    {
        LoadCategories();

        // Gán sự kiện click cho các nút mức độ khó
        //easyButton.onClick.AddListener(() => SendDifficultyAndCategoryToServer("Easy"));
        //mediumButton.onClick.AddListener(() => SendDifficultyAndCategoryToServer("Medium"));
        //hardButton.onClick.AddListener(() => SendDifficultyAndCategoryToServer("Hard"));
    }

    public void DisplayCategories(List<(string id, string name)> categories)
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var category in categories)
        {
            GameObject button = Instantiate(buttonPrefab, buttonContainer);
            button.GetComponentInChildren<TextMeshProUGUI>().text = category.name;
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                selectedCategoryId = category.id;
                animator.SetTrigger("ShowDifficulty");
                PlayerPrefs.SetString("categoryId", selectedCategoryId);
            });
        }
    }

    public void DisplayLevels(List<(string id, string name)> levels)
    {
        foreach (Transform child in buttonContainer2)
        {
            Destroy(child.gameObject);
        }

        foreach (var level in levels)
        {
            GameObject button = Instantiate(buttonPrefab2, buttonContainer2);
            button.SetActive(true);
            button.GetComponentInChildren<TextMeshProUGUI>().text = level.name;
            string sprite = "BackGroundBtn" + level.name;
            var background = Client.Instance.LoadSprite(sprite);
            button.GetComponentInChildren<Image>().sprite =background ;
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                selectedLevelId = level.id;
                SendDifficultyAndCategoryToServer(selectedLevelId);
                PlayerPrefs.SetString("levelId", selectedCategoryId);
            });
        }
    }


    private void SendDifficultyAndCategoryToServer(string difficulty)
    {
        string message = $"category:{selectedCategoryId}:difficulty:{difficulty}";
        Client.Instance.SendMessageToServer(message);
    }

    public void LoadCategories()
    {
        Client.Instance.SendMessageToServer("get_category");
        Client.Instance.SendMessageToServer("get_level");
    }
}