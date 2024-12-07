//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Net.Sockets;
//using UnityEditor.PackageManager;
//using UnityEngine;

//public class CardManager : MonoBehaviour
//{
//    public Card CardPrefab; // Prefab duy nhất
//    public Transform CardPosition;
//    public Vector2 StartPosition = new Vector2(-2f, 3.5f);
//    public Sprite backSprite;
//    public List<Sprite> frontSprites; // Danh sách các sprite mặt trước
//    public List<Sprite> frontSpritesVoca;
//    public GameObject gameOver;

//    private Vector2 offset = new Vector2(1.3f, 2f);
//    [HideInInspector]
//    public List<Card> Cards;

//    private List<Card> selectedCards = new List<Card>();

//    private TcpClient client;
//    private NetworkStream stream;

//    void Start()
//    {
//        Card.OnCardClicked += OnCardClicked;
//        SpawnCardMesh(4, 4, StartPosition, offset, false);
//        Debug.Log("Flip card");
//        StartCoroutine(FlipAllCardsTemporarily(8f)); // Lật tất cả các thẻ lên và úp lại sau 5 giây
//    }

//    void Update()
//    {
//        if (Cards.Count == 0)
//        {
//            GameOver();
//        }
//    }

//    public void LoadCards(List<(Sprite word, Sprite meaning)> wordPairs)
//    {
//        clearCard();
//        List<(Sprite, Sprite)> cardPairs = new List<(Sprite, Sprite)>();
//        foreach (var pair in wordPairs)
//        {
//            cardPairs.Add(pair);
//        }
//        RenderCards(cardPairs);
//    }

//    public void clearCard()
//    {
//        foreach (var card in Cards)
//        {
//            Destroy(card.gameObject);
//        }
//        Cards.Clear();
//    }

//    private void SpawnCardMesh(int row, int col, Vector2 pos, Vector2 offset, bool scaleDown)
//    {
//        List<(Sprite, Sprite)> cardPairs = new List<(Sprite, Sprite)>();
//        for (int i = 0; i < frontSprites.Count && i < frontSpritesVoca.Count; i++)
//        {
//            cardPairs.Add((frontSprites[i], frontSpritesVoca[i]));
//        }

//        cardPairs = ShuffleList(cardPairs);
//        RenderCards(cardPairs);
//    }

//    private void RenderCards(List<(Sprite, Sprite)> cardPairs)
//    {
//        List<Vector3> positions = new List<Vector3>();
//        for (int i = 0; i < 4; i++)
//        {
//            for (int j = 0; j < 4; j++)
//            {
//                positions.Add(new Vector3(StartPosition.x + j * offset.x, StartPosition.y - i * offset.y, 0.0f));
//            }
//        }

//        positions = ShuffleList(positions);

//        for (int i = 0; i < cardPairs.Count; i++)
//        {
//            var tempCardVoca = (Card)Instantiate(CardPrefab, CardPosition.position, CardPosition.transform.rotation);
//            tempCardVoca.name = "Card" + i;
//            tempCardVoca.SetSprites(cardPairs[i].Item1, backSprite);
//            Cards.Add(tempCardVoca);

//            var tempCardMeaning = (Card)Instantiate(CardPrefab, CardPosition.position, CardPosition.transform.rotation);
//            tempCardMeaning.name = "Card" + i;
//            tempCardMeaning.SetSprites(cardPairs[i].Item2, backSprite);
//            Cards.Add(tempCardMeaning);
//        }

//        for (int i = 0; i < Cards.Count; i++)
//        {
//            if (i < positions.Count)
//            {
//                StartCoroutine(MoveToPosition(positions[i], Cards[i]));
//            }
//            else
//            {
//                Debug.LogError("Không đủ vị trí để di chuyển thẻ!");
//                break;
//            }
//        }
//    }

//    private void OnDestroy()
//    {
//        Card.OnCardClicked -= OnCardClicked;
//    }

//    private void OnCardClicked(Card card)
//    {
//        if (selectedCards.Contains(card))
//            return;

//        selectedCards.Add(card);

//        if (selectedCards.Count == 2)
//        {
//            SetAllCardsClick(false);
//            StartCoroutine(CheckCards());
//        }
//    }

//    private IEnumerator CheckCards()
//    {
//        yield return new WaitForSeconds(1f);

//        if (selectedCards[0].name == selectedCards[1].name)
//        {
//            Destroy(selectedCards[0].gameObject);
//            Destroy(selectedCards[1].gameObject);
//            Cards.Remove(selectedCards[0]);
//            Cards.Remove(selectedCards[1]);
//        }
//        else
//        {
//            StartCoroutine(selectedCards[0].RotateCard());
//            StartCoroutine(selectedCards[1].RotateCard());
//        }
//        SetAllCardsClick(true);
//        selectedCards.Clear();
//    }

//    private List<T> ShuffleList<T>(List<T> list)
//    {
//        for (int i = 0; i < list.Count; i++)
//        {
//            T temp = list[i];
//            int randomIndex = UnityEngine.Random.Range(i, list.Count);
//            list[i] = list[randomIndex];
//            list[randomIndex] = temp;
//        }
//        return list;
//    }

//    private IEnumerator MoveToPosition(Vector3 target, Card obj)
//    {
//        var randomDis = 7;
//        while (obj.transform.position != target)
//        {
//            obj.transform.position = Vector3.MoveTowards(obj.transform.position, target, randomDis * Time.deltaTime);
//            yield return 0;
//        }
//    }

//    private IEnumerator FlipAllCardsTemporarily(float delay)
//    {
//        SetAllCardsClick(false);
//        yield return new WaitForSeconds(delay / 2);

//        foreach (var card in Cards)
//        {
//            if (!card.IsFacedUp())
//            {
//                StartCoroutine(card.RotateCard());
//            }
//        }

//        yield return new WaitForSeconds(delay);

//        foreach (var card in Cards)
//        {
//            if (card.IsFacedUp())
//            {
//                StartCoroutine(card.RotateCard());
//            }
//        }
//        SetAllCardsClick(true);
//    }

//    private void SetAllCardsClick(bool clickable)
//    {
//        foreach (var card in Cards)
//        {
//            card.SetClick(clickable);
//        }
//    }

//    public void GameOver()
//    {
//        gameOver.SetActive(true);
//    }
//}

using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public Card CardPrefab; // Prefab duy nhất
    public Transform CardPosition;
    public Vector2 StartPosition = new Vector2(-2f, 3.5f);
    public Sprite backSprite;
    public List<Sprite> frontSprites; // Danh sách các sprite mặt trước
    public List<Sprite> frontSpritesVoca;
    public GameObject gameOver;

    private Vector2 offset = new Vector2(1.3f, 2f);
    [HideInInspector]
    public List<Card> Cards;


    private List<Card> selectedCards = new List<Card>();

    private TcpClient client;
    private NetworkStream stream;

    void Start()
    {
        Card.OnCardClicked += OnCardClicked;
        // Không cần tạo card trong Start nữa
         StartCoroutine(FlipAllCardsTemporarily(8f)); // Lật tất cả các thẻ lên và úp lại sau 5 giây
    }

    void Update()
    {
        if (Cards.Count == 0)
        {
            Debug.Log("Game Over");
            GameOver();
        }
    }

    public void clearCard()
    {
        foreach (var card in Cards)
        {
            Destroy(card.gameObject);
        }
        Cards.Clear();
    }

    public void LoadAllCard(List<(Sprite, Sprite)> cardPairs)
    {
        for (int i = 0; i < cardPairs.Count; i++)
        {
            var tempCardVoca = (Card)Instantiate(CardPrefab, CardPosition.position, CardPosition.transform.rotation);
            tempCardVoca.name = "Card" + i;
            tempCardVoca.SetSprites(cardPairs[i].Item1, backSprite);
            Cards.Add(tempCardVoca);

            var tempCardMeaning = (Card)Instantiate(CardPrefab, CardPosition.position, CardPosition.transform.rotation);
            tempCardMeaning.name = "Card" + i;
            tempCardMeaning.SetSprites(cardPairs[i].Item2, backSprite);
            Cards.Add(tempCardMeaning);
        }
    }
    public void LoadCards(List<(Sprite word, Sprite meaning)> wordPairs, int rows, int cols)
    {
        Debug.Log(cols);
        Debug.Log(rows);
        clearCard();
        List<(Sprite, Sprite)> cardPairs = new List<(Sprite, Sprite)>();
        foreach (var pair in wordPairs)
        {
            cardPairs.Add(pair);
        }
        RenderCards(cardPairs,rows,cols);
    }


    private void RenderCards(List<(Sprite, Sprite)> cardPairs, int rows, int cols)
    {
        //Debug.Log(cols);
        //Debug.Log(rows);
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                positions.Add(new Vector3(StartPosition.x + j * offset.x, StartPosition.y - i * offset.y, 0.0f));
            }
        }

        positions = ShuffleList(positions);

        for (int i = 0; i < cardPairs.Count; i++)
        {
            var tempCardVoca = (Card)Instantiate(CardPrefab, CardPosition.position, CardPosition.transform.rotation);
            tempCardVoca.name = "Card" + i;
            tempCardVoca.SetSprites(cardPairs[i].Item1, backSprite);
            Cards.Add(tempCardVoca);

            var tempCardMeaning = (Card)Instantiate(CardPrefab, CardPosition.position, CardPosition.transform.rotation);
            tempCardMeaning.name = "Card" + i;
            tempCardMeaning.SetSprites(cardPairs[i].Item2, backSprite);
            Cards.Add(tempCardMeaning);
        }

        for (int i = 0; i < Cards.Count; i++)
        {
            if (i < positions.Count)
            {
                StartCoroutine(MoveToPosition(positions[i], Cards[i]));
            }
            else
            {
                Debug.LogError("Không đủ vị trí để di chuyển thẻ!");
                break;
            }
        }
    }

    private void OnDestroy()
    {
        Card.OnCardClicked -= OnCardClicked;
    }

    private void OnCardClicked(Card card)
    {
        if (selectedCards.Contains(card))
            return;

        selectedCards.Add(card);

        if (selectedCards.Count == 2)
        {
            SetAllCardsClick(false);
            StartCoroutine(CheckCards());
        }
    }

    private IEnumerator CheckCards()
    {
        yield return new WaitForSeconds(1f);

        if (selectedCards[0].name == selectedCards[1].name)
        {
            Destroy(selectedCards[0].gameObject);
            Destroy(selectedCards[1].gameObject);
            Cards.Remove(selectedCards[0]);
            Cards.Remove(selectedCards[1]);
        }
        else
        {
            StartCoroutine(selectedCards[0].RotateCard());
            StartCoroutine(selectedCards[1].RotateCard());
        }
        SetAllCardsClick(true);
        selectedCards.Clear();
    }

    private List<T> ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = UnityEngine.Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list;
    }

    private IEnumerator MoveToPosition(Vector3 target, Card obj)
    {
        var randomDis = 7;
        while (obj.transform.position != target)
        {
            obj.transform.position = Vector3.MoveTowards(obj.transform.position, target, randomDis * Time.deltaTime);
            yield return 0;
        }
    }

    private IEnumerator FlipAllCardsTemporarily(float delay)
    {
        SetAllCardsClick(false);
        yield return new WaitForSeconds(delay / 2);

        foreach (var card in Cards)
        {
            if (!card.IsFacedUp())
            {
                StartCoroutine(card.RotateCard());
            }
        }

        yield return new WaitForSeconds(delay);

        foreach (var card in Cards)
        {
            if (card.IsFacedUp())
            {
                StartCoroutine(card.RotateCard());
            }
        }
        SetAllCardsClick(true);
    }

    private void SetAllCardsClick(bool clickable)
    {
        foreach (var card in Cards)
        {
            card.SetClick(clickable);
        }
    }

    public void GameOver()
    {
        if (gameOver == null)
        {
            Debug.LogError("gameOver GameObject is not assigned in the CardManager script");
            return;
        }
        Debug.Log("Game Over method called");
        gameOver.SetActive(true);
    }
}