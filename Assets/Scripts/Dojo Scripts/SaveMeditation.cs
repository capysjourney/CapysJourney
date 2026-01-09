using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        LoadMeditations();
        MeditationEntry newEntry = new MeditationEntry
        {
            duration = GetScrollValue(durationContent, durationSelect),
            interval = GetScrollValue(intervalContent, intervalSelect),
            chime = SharedData.chime,
            effect = SharedData.effectData
        };

        meditationList.entries.Add(newEntry);
        SaveMeditations();


        if (dojoController != null)
        {
            dojoController.RefreshMeditations();
        }
    }

    private void LoadMeditations()
    {
        string json = PlayerPrefs.GetString("SavedMeditations", "");

        if (!string.IsNullOrEmpty(json))
        {
            meditationList = JsonUtility.FromJson<MeditationList>(json);
        }
        else
        {
            meditationList = new MeditationList();
        }
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