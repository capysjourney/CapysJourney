using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    void Start()
    {
        chime.onValueChanged.AddListener(OnDropdownValueChanged);
        chime.onValueChanged.AddListener(OnChimeSelected);
        startButton.onClick.AddListener(OnStartClick);

        if (chime.options.Count > 0)
        {
            SharedData.chime = chime.options[0].text;
        }
    }

    private void OnStartClick()
    {
        SharedData.duration = GetScrollValue(durationContent, durationSelect);
        SharedData.interval = GetScrollValue(intervalContent, intervalSelect);
        SharedData.effectData = "Rain";

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
}