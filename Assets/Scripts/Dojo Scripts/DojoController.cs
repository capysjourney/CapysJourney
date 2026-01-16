using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

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
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Button startButton;
    [SerializeField] private UnityEngine.UI.Image durationSelect;
    [SerializeField] private UnityEngine.UI.Image intervalSelect;
    [SerializeField] private RectTransform durationContent;
    [SerializeField] private RectTransform intervalContent;
    [SerializeField] private RectTransform savedMeditationsContainer;
    [SerializeField] private RectTransform meditationTemplate;
    [SerializeField] private MeditationList meditationList;
    [SerializeField] private ScrollRect savedLessonsList;
    [SerializeField] private RectTransform savedLessonsTemplate;
    [SerializeField] private TMP_Dropdown region;

    private World selectedWorld = null;

    void Start()
    {
        chime.onValueChanged.AddListener(OnDropdownValueChanged);
        chime.onValueChanged.AddListener(OnChimeSelected);
        startButton.onClick.AddListener(OnStartClick);
        region.onValueChanged.AddListener(OnWorldFilterChanged);

        if (chime.options.Count > 0)
        {
            SharedData.chime = chime.options[0].text;
        }

        LoadAndDisplayBookmarkedLessons();
        LoadAndDisplayMeditations();
    }

    private void OnWorldFilterChanged(int index)
    {
        Debug.Log($"OnWorldFilterChanged called with index: {index}, option text: {region.options[index].text}");
        index = index - 1;
        string selectedOption = region.options[index].text;

        if (selectedOption == "All Worlds" || selectedOption == "Everything")
        {
            selectedWorld = null;
        }
        else
        {
            selectedWorld = World.AllWorlds.FirstOrDefault(w => w.Name == selectedOption);

            if (selectedWorld == null)
            {
                selectedWorld = null;
            }
        }

        LoadAndDisplayBookmarkedLessons();
    }

    private void LoadAndDisplayBookmarkedLessons()
    {
        foreach (Transform child in savedLessonsList.content)
        {
            if (child != savedLessonsTemplate.transform)
            {
                Destroy(child.gameObject);
            }
        }

        GameManager.WithStats(stats =>
        {
            List<Level> filteredLevels = new List<Level>();

            foreach (LevelEnum levelEnum in stats.BookmarkedLevels)
            {
                Level level = Level.LevelLookup[levelEnum];

                if (selectedWorld == null || level.World == selectedWorld)
                {
                    filteredLevels.Add(level);
                }
            }

            Debug.Log($"Displaying {filteredLevels.Count} bookmarked levels" +
                     (selectedWorld != null ? $" from {selectedWorld.Name}" : ""));

            foreach (Level level in filteredLevels)
            {
                CreateBookmarkedLessonUI(level);
            }
        }, false);
    }

    private void CreateBookmarkedLessonUI(Level level)
    {
        RectTransform newLessonUI = Instantiate(savedLessonsTemplate, savedLessonsList.content);

        TMP_Text lessonText = newLessonUI.GetComponentInChildren<TMP_Text>();
        if (lessonText != null)
        {
            lessonText.text = level.ShortName;
        }

        newLessonUI.gameObject.SetActive(true);
    }

    public static void PrintBookmarkedLevels()
    {
        GameManager.WithStats(stats =>
        {
            Debug.Log($"Found {stats.BookmarkedLevels.Count} bookmarked levels:");

            foreach (LevelEnum levelEnum in stats.BookmarkedLevels)
            {
                Level level = Level.LevelLookup[levelEnum];
            }
        }, false);
    }

    private void LoadAndDisplayMeditations()
    {
        foreach (Transform child in savedMeditationsContainer)
        {
            if (child != meditationTemplate.transform)
            {
                Destroy(child.gameObject);
            }
        }

        GameManager.WithStats(stats =>
        {
            meditationList = new MeditationList();
            meditationList.entries = stats.MeditationLog.Select(m => new MeditationEntry
            {
                duration = m.duration,
                interval = m.interval,
                chime = m.chime,
                effect = m.effect
            }).ToList();

            DisplaySavedMeditations();
        }, false);
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

        SceneManager.LoadScene("CustomMeditation");
    }

    private void OnStartClick()
    {
        SharedData.duration = GetScrollValue(durationContent, durationSelect);
        SharedData.interval = GetScrollValue(intervalContent, intervalSelect);

        SceneManager.LoadScene("CustomMeditation");
    }

    public void OnDropdownValueChanged(int index)
    {
        SharedData.chime = chime.options[index].text;
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
            return value;
        }

        return 1;
    }

    private void OnChimeSelected(int index)
    {
        if (audioSource == null)
        {
            return;
        }

        string selectedChime = chime.options[index].text;

        if (selectedChime.Contains("Classic Bell"))
        {
            audioSource.PlayOneShot(bell);
        }
        else if (selectedChime.Contains("Echo Bell"))
        {
            audioSource.PlayOneShot(bell1);
        }
        else if (selectedChime.Contains("Mellow Bell"))
        {
            audioSource.PlayOneShot(bell2);
        }
    }

    public void RefreshMeditations()
    {
        LoadAndDisplayMeditations();
    }

    public void RefreshBookmarkedLessons()
    {
        LoadAndDisplayBookmarkedLessons();
    }
}