using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DailyScript : MonoBehaviour
{
    [SerializeField] private Button _moodCheckInBtn;
    [SerializeField] private TMP_Text _moodCheckInText;
    [SerializeField] private Button _breathworkBtn;
    [SerializeField] private Button _gratitudeBtn;
    [SerializeField] private Button _moodLogBtn;
    [SerializeField] private GameObject _moodLog;
    [SerializeField] private GameObject _moodEntryPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LocalizedString checkInText = new("String Table", "Mood Check-In Question") {
            { "username", new StringVariable { Value = PlayerPrefs.GetString("username") } }
        };
        _moodCheckInText.SetText(checkInText.GetLocalizedString());
        _moodCheckInBtn.onClick.AddListener(OnMoodCheckIn);
        _breathworkBtn.onClick.AddListener(OnBreathwork);
        _gratitudeBtn.onClick.AddListener(OnGratitude);
        _moodLogBtn.onClick.AddListener(OnMoodLog);
        int numChildren = _moodLog.transform.childCount;
        for (int i = numChildren - 1; i >= 0; i--)
        {
            Destroy(_moodLog.transform.GetChild(i).gameObject);
        }
        LinkedList<MoodEntry> entries = DailyExercisesManager.GetMoodEntries();
        foreach (MoodEntry entry in entries)
        {
            GameObject entryObject = Instantiate(_moodEntryPrefab, _moodLog.transform);
            MoodEntryScript entryScript = entryObject.GetComponent<MoodEntryScript>();
            entryScript.SetEntry(entry);
        }
    }

    private void OnMoodCheckIn() => SceneManager.LoadSceneAsync("Mood");

    private void OnBreathwork() => SceneManager.LoadSceneAsync("Breathwork");

    private void OnGratitude() => SceneManager.LoadSceneAsync("Gratitude");

    private void OnMoodLog() => SceneManager.LoadSceneAsync("MoodLog");

}
