using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeButtonClick : MonoBehaviour
{
    [SerializeField] private string sceneName = "Journey";

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(OnHomeButtonClick);
        }
    }

    private void OnHomeButtonClick()
    {
        SceneManager.LoadScene(sceneName);
    }

    void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnHomeButtonClick);
        }
    }
}