using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    private SpriteRenderer rend;

    private Sprite frontSprite, backSprite;

    private bool coroutineAllowed, facedUp;
    private bool isClick = true;

    public delegate void CardClicked(Card card);
    public static event CardClicked OnCardClicked;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        if (rend == null)
        {
            Debug.LogError("SpriteRenderer is not attached to the Card prefab.");
            return;
        }
        rend.sprite = backSprite; // Đặt sprite mặt sau khi khởi tạo
        coroutineAllowed = true;
        facedUp = false;
    }

    private void OnMouseDown()
    {
        if (coroutineAllowed && isClick)
        {
            StartCoroutine(RotateCard());
            OnCardClicked?.Invoke(this);
        }
    }

    public IEnumerator RotateCard()
    {
        coroutineAllowed = false;

        if (!facedUp)
        {
            for (float i = 0f; i <= 180f; i += 10f)
            {
                transform.rotation = Quaternion.Euler(0f, i, 0f);
                if (i == 90f)
                {
                    rend.sprite = frontSprite; // Đặt sprite mặt trước khi xoay đến 90 độ
                }
                yield return new WaitForSeconds(0.01f);
            }
        }
        else if (facedUp)
        {
            for (float i = 180f; i >= 0f; i -= 10f)
            {
                transform.rotation = Quaternion.Euler(0f, i, 0f);
                if (i == 90f)
                {
                    rend.sprite = backSprite; // Đặt sprite mặt sau khi xoay đến 90 độ
                }
                yield return new WaitForSeconds(0.01f);
            }
        }

        coroutineAllowed = true;
        facedUp = !facedUp;
    }

    // Phương thức để thiết lập sprite cho thẻ
    public void SetSprites(Sprite front, Sprite back)
    {
        if (rend == null)
        {
            rend = GetComponent<SpriteRenderer>();
            if (rend == null)
            {
                Debug.LogError("SpriteRenderer is not attached to the Card prefab.");
                return;
            }
        }
        frontSprite = front;
        backSprite = back;
        rend.sprite = backSprite; // Đặt lại sprite mặt sau khi khởi tạo
    }

    public Sprite GetFrontSprite()
    {
        return frontSprite;
    }

    // Phương thức để kiểm tra xem thẻ có đang lật lên hay không
    public bool IsFacedUp()
    {
        return facedUp;
    }
    public void SetClick(bool click)
    {
        isClick = click;
    }
}