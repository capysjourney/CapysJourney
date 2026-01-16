using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class SaveMeditation : MonoBehaviour
{
    [SerializeField] private DojoController dojoController;
    [SerializeField] private UnityEngine.UI.Image durationSelect;
    [SerializeField] private UnityEngine.UI.Image intervalSelect;
    [SerializeField] private RectTransform durationContent;
    [SerializeField] private RectTransform intervalContent;
    private Button saveButton;
    private MeditationList meditationList;

    void Start()
    {
        saveButton = GetComponent<Button>();
        if (saveButton != null)
        {
            saveButton.onClick.AddListener(OnSaveButtonClick);
        }
    }

    private void OnSaveButtonClick()
    {
        MeditationEntry newEntry = new MeditationEntry
        {
            duration = GetScrollValue(durationContent, durationSelect),
            interval = GetScrollValue(intervalContent, intervalSelect),
            chime = SharedData.chime,
            effect = SharedData.effectData
        };

        PlayerStats stats = GameManager.GetStats();
        stats.MeditationLog.AddFirst(newEntry);
        stats.SaveToFirestore();

        if (dojoController != null)
        {
            dojoController.RefreshMeditations();
        }
    }

    private void LoadMeditations()
    {
        PlayerStats stats = GameManager.GetStats();

        stats.LoadMeditationsFromFirestore(loadedList =>
        {
            meditationList = new MeditationList();
            meditationList.entries = loadedList.ToList();
        });
    }

    private void SaveMeditations()
    {
        string json = JsonUtility.ToJson(meditationList);
        PlayerPrefs.SetString("SavedMeditations", json);
        PlayerPrefs.Save();
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

}