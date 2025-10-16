using UnityEngine;
using UnityEngine.UI;

public class AppearanceSelector : MonoBehaviour
{
    [SerializeField] private Button _lightButton;
    [SerializeField] private Button _darkButton;
    [SerializeField] private Button _systemButton;
    [SerializeField] private Sprite _uncheckedButton;
    [SerializeField] private Sprite _checkedButton;

    void Start()
    {
        Button[] radioButtons = new Button[] { _lightButton, _darkButton, _systemButton };
        foreach (Button button in radioButtons)
        {
            button.onClick.RemoveAllListeners();
        }
        string appearance = PlayerPrefs.GetString("appearance", "light");
        RadioButtonClicked(radioButtons, ButtonOf(appearance), appearance);
        _lightButton.onClick.AddListener(() => RadioButtonClicked(radioButtons, _lightButton, "light"));
        _darkButton.onClick.AddListener(() => RadioButtonClicked(radioButtons, _darkButton, "dark"));
        _systemButton.onClick.AddListener(() => RadioButtonClicked(radioButtons, _systemButton, "system"));
    }

    private void RadioButtonClicked(Button[] buttons, Button clickedbutton, string mode)
    {
        Button clickedButton = ButtonOf(mode);
        foreach (Button button in buttons)
        {
            if (button != clickedButton)
            {
                button.image.sprite = _uncheckedButton;
            }
            else
            {
                button.image.sprite = _checkedButton;
            }
        }
        PlayerPrefs.SetString("appearance", mode);
    }
    private Button ButtonOf(string appearance)
    {
        return appearance switch
        {
            "light" => _lightButton,
            "dark" => _darkButton,
            "system" => _systemButton,
            _ => _lightButton,
        };
    }
}
