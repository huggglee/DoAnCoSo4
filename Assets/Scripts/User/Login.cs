using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{

    public TMP_InputField username;
    public TMP_InputField password;

    public void OnLoginClick()
    {
        if (!string.IsNullOrEmpty(username.text) && !string.IsNullOrEmpty(password.text))
        {
            Client.Instance.Login(username.text, password.text);
        }
        else
        {
            Debug.LogError("Vui lòng nhập tên đăng nhập và mật khẩu!");
        }

    }
}
