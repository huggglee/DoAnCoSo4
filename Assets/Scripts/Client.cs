using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI;

public class Client : MonoBehaviour
{
    public static Client Instance { get; private set; }
    private TcpClient client;
    private NetworkStream stream;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ client khi chuyển scene
        }
        else
        {
            Destroy(gameObject); // Nếu đã có 1 instance, hủy bỏ instance mới
        }
    }

    void Start()
    {
        ConnectToServer();
    }

    private void ConnectToServer()
    {
        try
        {
            client = new TcpClient("192.168.43.59", 8080);
            stream = client.GetStream();
            Debug.Log("Kết nối đến server thành công!");
        }
        catch (SocketException e)
        {
            Debug.LogError("Không thể kết nối đến server: " + e.Message);
        }
    }

    public void Register(string username, string password)
    {
        if (client != null && client.Connected)
        {
            SendMessageToServer($"register:{username}:{password}");
        }
        else
        {
            Debug.LogError("Chưa kết nối với server!");
        }
    }

    public void Login(string username, string password)
    {
        if (client != null && client.Connected)
        {
            SendMessageToServer($"login:{username}:{password}");
        }
        else
        {
            Debug.Log("Chưa kết nối với server!");
        }
    }

    public void selectCategory(string category)
    {
        if (client != null && client.Connected)
        {
            SendMessageToServer($"category:{category}");
        }
        else
        {
            Debug.Log("Chưa kết nối với server!");
        }
    }

    public void SendMessageToServer(string message)
    {
        byte[] data = Encoding.UTF8.GetBytes(message);
        stream.Write(data, 0, data.Length);

        byte[] buffer = new byte[1024];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        HandleServerResponse(response);
    }

    private void HandleServerResponse(string response)
    {
        if (response == "Login:success")
        {
            Debug.Log("Đăng nhập thành công!");
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene("Category");
        }
        else if (response.StartsWith("categories"))
        {
            Debug.Log("Received categories response");
            var categories = response.Substring("categories".Length + 1).Split('|');
            List<string> options = new List<string>();

            foreach (var category in categories)
            {
                var data = category.Split(':');
                options.Add(data[1]);
            }

            GameObject categoryManager = GameObject.FindGameObjectWithTag("CategoryManager");
            if (categoryManager != null)
            {
                Debug.Log("CategoryManager found");
                CategoryManager ca = categoryManager.GetComponent<CategoryManager>();
                if (ca != null)
                {
                    Debug.Log("CategoryManager component found");
                    ca.DisplayCategories(options);
                }
                else
                {
                    Debug.LogError("CategoryManager component not found on the GameObject");
                }
            }
            else
            {
                Debug.LogError("CategoryManager GameObject not found");
            }
        }
        else if (response.StartsWith("words"))
        {
            var words = response.Substring("words".Length + 1).Split('|');
            var level = words[0];
            words = words.Skip(1).ToArray();
            var random = new System.Random();
            words = words.OrderBy(x => random.Next()).ToArray();
            int row = 0, col = 0;

            if (level == "Easy")
            {
                row = 2;
                col = 2;
            }
            else if (level == "Medium")
            {
                row = 2;
                col = 4;
            }
            else if (level == "Hard")
            {
                row = 4;
                col = 4;
            }

            int useCardPairs = row * col/2;
            List<(Sprite, Sprite)> cardPairs = new List<(Sprite, Sprite)>();

            for (int i = 0; i < useCardPairs; i++)
            {
                Debug.Log(words[i]);
                var wordSprite = LoadSprite(words[i]);
                var meaningSprite = LoadSprite("V" + words[i]);
                cardPairs.Add((wordSprite, meaningSprite));
            }

            SceneManager.sceneLoaded += (scene, mode) => OnMyGameSceneLoaded(scene, mode, cardPairs, row, col);
            SceneManager.LoadScene("MyGame");
        }
    }

    private void OnMyGameSceneLoaded(Scene scene, LoadSceneMode mode, List<(Sprite, Sprite)> cardPairs, int row, int col)
    {
        Debug.Log(col);
        Debug.Log(row);
        if (scene.name == "MyGame")
        {
            SceneManager.sceneLoaded -= (s, m) => OnMyGameSceneLoaded(s, m, cardPairs, row, col);
            GameObject cardManager = GameObject.FindGameObjectWithTag("CardManager");
            if (cardManager != null)
            {
                var cardManagerScript = cardManager.GetComponent<CardManager>();
                if (cardManagerScript != null)
                {
                    cardManagerScript.LoadCards(cardPairs, row, col);
                }
                else
                {
                    Debug.LogError("CardManager component not found on the GameObject");
                }
            }
            else
            {
                Debug.LogError("CardManager GameObject not found");
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Test")
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            GameObject categoryManager = GameObject.FindGameObjectWithTag("CategoryManager");
            if (categoryManager != null)
            {
                CategoryManager ca = categoryManager.GetComponent<CategoryManager>();
                if (ca != null)
                {
                    ca.LoadCategories();
                }
            }
        }
    }

    private Sprite LoadSprite(string name)
    {
        return Resources.Load<Sprite>($"Sprites/{name}");
    }

    private void OnApplicationQuit()
    {
        if (stream != null) stream.Close();
        if (client != null) client.Close();
    }
}