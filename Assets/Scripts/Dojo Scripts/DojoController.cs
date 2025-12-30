using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class MeditationEntry
{
    public int duration;
    public int interval;
    public string chime;
    public string effect;
}

[System.Serializable]
public class MeditationList
{
    public List<MeditationEntry> entries = new List<MeditationEntry>();
}


public class DojoController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown chime;
    [SerializeField] private AudioClip bell;
    [SerializeField] private AudioClip bell1;
    [SerializeField] private AudioClip bell2;
    [SerializeField] private AudioClip bell3;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Button startButton;
    [SerializeField] private UnityEngine.UI.Image durationSelect;
    [SerializeField] private UnityEngine.UI.Image intervalSelect;
    [SerializeField] private RectTransform durationContent;
    [SerializeField] private RectTransform intervalContent;
    [SerializeField] private RectTransform savedMeditationsContainer;
    [SerializeField] private RectTransform meditationTemplate;

    private MeditationList meditationList;

    void Start()
    {
        chime.onValueChanged.AddListener(OnDropdownValueChanged);
        chime.onValueChanged.AddListener(OnChimeSelected);
        startButton.onClick.AddListener(OnStartClick);

        if (chime.options.Count > 0)
        {
            SharedData.chime = chime.options[0].text;
        }

        LoadAndDisplayMeditations();
    }

    private void LoadAndDisplayMeditations()
    {
        string json = PlayerPrefs.GetString("SavedMeditations", "");

        if (!string.IsNullOrEmpty(json))
        {
            meditationList = JsonUtility.FromJson<MeditationList>(json);
            UnityEngine.Debug.Log("Loaded " + meditationList.entries.Count + " meditations");
        }
        else
        {
            meditationList = new MeditationList();
            UnityEngine.Debug.Log("No saved meditations found, created new list");
        }

        DisplaySavedMeditations();
    }

    private void DisplaySavedMeditations()
    {
        foreach (Transform child in savedMeditationsContainer)
        {
            if (child != meditationTemplate.transform)
            {
                Destroy(child.gameObject);
            }
        }

        foreach (MeditationEntry entry in meditationList.entries)
        {
            CreateMeditationUI(entry);
        }
    }

    private void CreateMeditationUI(MeditationEntry entry)
    {
        RectTransform newEntryUI = Instantiate(meditationTemplate, savedMeditationsContainer);

        Transform numMinutesTransform = newEntryUI.transform.Find("Top")?.Find("Num Minutes");
        if (numMinutesTransform != null)
        {
            TMP_Text numMinutesText = numMinutesTransform.GetComponent<TMP_Text>();
            if (numMinutesText != null)
            {
                numMinutesText.text = entry.duration.ToString();
            }
        }

        string meditationInfo = "";

        if (!string.IsNullOrEmpty(entry.chime) && entry.chime != "None")
        {
            meditationInfo = "Chimes every " + entry.interval + " minutes";
        }

        if (!string.IsNullOrEmpty(entry.effect) && entry.effect != "None")
        {
            if (!string.IsNullOrEmpty(meditationInfo))
            {
                meditationInfo += ", ";
            }
            meditationInfo += entry.effect;
        }

        Transform meditationTextTransform = newEntryUI.transform.Find("Meditation Info")?.Find("Meditation text");
        if (meditationTextTransform != null)
        {
            TMP_Text meditationText = meditationTextTransform.GetComponent<TMP_Text>();
            if (meditationText != null)
            {
                meditationText.text = meditationInfo;
            }
        }

        Button loadButton = newEntryUI.GetComponent<Button>();
        if (loadButton != null)
        {
            loadButton.onClick.RemoveAllListeners();
            loadButton.onClick.AddListener(() => LoadMeditation(entry));
        }

        newEntryUI.SetParent(savedMeditationsContainer, false);
        newEntryUI.gameObject.SetActive(true);
    }

    private void LoadMeditation(MeditationEntry entry)
    {
        SharedData.duration = entry.duration;
        SharedData.interval = entry.interval;
        SharedData.chime = entry.chime;
        SharedData.effectData = entry.effect;

        UnityEngine.Debug.Log("Loaded meditation: " + entry.duration + " min, " + entry.effect);

        SceneManager.LoadScene("CustomMeditation");
    }

    private void OnStartClick()
    {
        SharedData.duration = GetScrollValue(durationContent, durationSelect);
        SharedData.interval = GetScrollValue(intervalContent, intervalSelect);

        UnityEngine.Debug.Log("DojoController - Duration: " + SharedData.duration);
        UnityEngine.Debug.Log("DojoController - Interval: " + SharedData.interval);
        UnityEngine.Debug.Log("DojoController - Chime: " + SharedData.chime);
        UnityEngine.Debug.Log("DojoController - Effect: " + SharedData.effectData);

        SceneManager.LoadScene("CustomMeditation");
    }

    public void OnDropdownValueChanged(int index)
    {
        SharedData.chime = chime.options[index].text;
        UnityEngine.Debug.Log("Chime selected: " + SharedData.chime);
    }

    private int GetScrollValue(RectTransform content, UnityEngine.UI.Image select)
    {
        Vector3 selectPos = select.transform.position;
        Button[] children = content.GetComponentsInChildren<Button>();

        float minDistance = float.MaxValue;
        Button closestButton = null;

        foreach (Button child in children)
        {
            float distance = Mathf.Abs(selectPos.y - child.transform.position.y);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestButton = child;
            }
        }

        if (closestButton != null)
        {
            int value = int.Parse(closestButton.GetComponentInChildren<TMP_Text>().text);
            UnityEngine.Debug.Log("Scroll value selected: " + value);
            return value;
        }

        return 1;
    }

    private void OnChimeSelected(int index)
    {
        if (audioSource == null)
        {
            UnityEngine.Debug.LogError("AudioSource is null!");
            return;
        }

        string selectedChime = chime.options[index].text;
        UnityEngine.Debug.Log("Playing chime preview: " + selectedChime);

        if (selectedChime.Contains("Bell 1"))
        {
            audioSource.PlayOneShot(bell1);
        }
        else if (selectedChime.Contains("Bell 2"))
        {
            audioSource.PlayOneShot(bell2);
        }
        else if (selectedChime.Contains("Bell 3"))
        {
            audioSource.PlayOneShot(bell3);
        }
        else if (selectedChime.Contains("Bell"))
        {
            audioSource.PlayOneShot(bell);
        }
    }

    public void RefreshMeditations()
    {
        LoadAndDisplayMeditations();
    }
}