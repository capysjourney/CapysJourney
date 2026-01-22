using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ForgotPasswordScript : MonoBehaviour
{
    [SerializeField]
    Button _back_button;
    [SerializeField]
    Button _back_to_login_button;
    [SerializeField]
    TMP_Text _content;

    public static string userEmail;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _back_button.onClick.AddListener(OnBackButtonPressed);
        _back_to_login_button.onClick.AddListener(OnBackButtonPressed);
    }

    // Update is called once per frame
    void Update()
    {
        if (string.IsNullOrEmpty(userEmail))
        {
            _content.text = "This email address is invalid. Please check for typos or formatting issues and try again.";
        }
        else
        {
            _content.text = "Please check your email! A password reset link was sent to " + userEmail + ". If it’s not in your inbox, be sure to check your spam folder.";
        }

    }

    void OnBackButtonPressed()
    {
        SceneManager.LoadSceneAsync("Login");
    }
}
