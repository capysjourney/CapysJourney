using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    [SerializeField] private ToggleSwitchScript _switchScript;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _appearanceArrow;
    [SerializeField] private GameObject _appearanceInputArea;
    [SerializeField] private Button _ageArrow;
    [SerializeField] private GameObject _ageInputArea;
    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private Slider _musicSlider;
    private bool _appearanceEnabled = false;
    private bool _ageEnabled = false;

    void Start()
    {
        ConfigureSwitch();
        _backButton.onClick.AddListener(() => SceneManager.LoadSceneAsync("Profile"));
        _appearanceArrow.onClick.AddListener(() => ToggleAppearance());
        _appearanceInputArea.SetActive(false);
        _ageArrow.onClick.AddListener(() => ToggleAge());
        _ageInputArea.SetActive(false);
        _volumeSlider.value = PlayerPrefs.GetFloat("Volume", 100);
        _musicSlider.value = PlayerPrefs.GetFloat("Music", 100);
        _volumeSlider.onValueChanged.AddListener((float value) => PlayerPrefs.SetFloat("Volume", value));
        _musicSlider.onValueChanged.AddListener((float value) => PlayerPrefs.SetFloat("Music", value));
       
    }

    private void ConfigureSwitch()
    {
        _switchScript.SetOnToggleOn(() => PlayerPrefs.SetInt("notifications", 1));
        _switchScript.SetOnToggleOff(() => PlayerPrefs.SetInt("notifications", 0));
        _switchScript.SetSwitch(PlayerPrefs.GetInt("notifications", 0) == 1);
    }

    private void ToggleAppearance()
    {
        _appearanceEnabled = !_appearanceEnabled;
        _appearanceInputArea.SetActive(_appearanceEnabled);
        // todo - animate
    }

    private void ToggleAge()
    {
        _ageEnabled = !_ageEnabled;
        _ageInputArea.SetActive(_ageEnabled);
        // todo - animate
    }
}
