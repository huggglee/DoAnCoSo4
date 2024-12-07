using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Register : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    public void OnRegisterClick()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            Client.Instance.Register(username, password);
        }
        else
        {
            Debug.LogError("Vui lòng nhập tên đăng nhập và mật khẩu!");
        }
    }
}
