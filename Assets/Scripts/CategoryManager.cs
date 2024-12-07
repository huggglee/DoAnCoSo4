using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CategoryManager : MonoBehaviour
{
    public GameObject buttonPrefab; // Gán prefab button vào Inspector
    public Transform buttonContainer; // Gán Panel chứa các button
    public Animator animator;

    public Button easyButton; // Gán nút mức độ dễ vào Inspector
    public Button mediumButton; // Gán nút mức độ trung bình vào Inspector
    public Button hardButton; // Gán nút mức độ khó vào Inspector

    private string selectedCategory;

    private void Start()
    {
        LoadCategories();

        // Gán sự kiện click cho các nút mức độ khó
        easyButton.onClick.AddListener(() => SendDifficultyAndCategoryToServer("Easy"));
        mediumButton.onClick.AddListener(() => SendDifficultyAndCategoryToServer("Medium"));
        hardButton.onClick.AddListener(() => SendDifficultyAndCategoryToServer("Hard"));
    }

    public void DisplayCategories(List<string> categories)
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var category in categories)
        {
            GameObject button = Instantiate(buttonPrefab, buttonContainer);
            button.GetComponentInChildren<TextMeshProUGUI>().text = category;
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                selectedCategory = category;
                animator.SetTrigger("ShowDifficulty");
            });
        }
    }

    private void SendDifficultyAndCategoryToServer(string difficulty)
    {
        string message = $"category:{selectedCategory}:difficulty:{difficulty}";
        Client.Instance.SendMessageToServer(message);
    }

    public void LoadCategories()
    {
        Client.Instance.SendMessageToServer("get_category");
    }
}