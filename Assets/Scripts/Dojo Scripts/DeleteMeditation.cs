using UnityEngine;
using UnityEngine.UI;

public class DeleteMeditation : MonoBehaviour
{
    private Button deleteButton;

    void Start()
    {
        deleteButton = GetComponent<Button>();
        if (deleteButton != null)
        {
            deleteButton.onClick.AddListener(OnDeleteButtonClick);
        }
    }

    public void OnDeleteButtonClick()
    {
        string json = PlayerPrefs.GetString("SavedMeditations", "");

        if (string.IsNullOrEmpty(json))
        {
            return;
        }

        MeditationList meditationList = JsonUtility.FromJson<MeditationList>(json);

        int siblingIndex = transform.parent.parent.GetSiblingIndex();
        int index = siblingIndex - 1;

        if (index >= 0 && index < meditationList.entries.Count)
        {
            meditationList.entries.RemoveAt(index);

            json = JsonUtility.ToJson(meditationList);
            PlayerPrefs.SetString("SavedMeditations", json);
            PlayerPrefs.Save();

            Destroy(transform.parent.parent.gameObject);

            DojoController dojoController = FindObjectOfType<DojoController>();
            if (dojoController != null)
            {
                dojoController.RefreshMeditations();
            }
        }
    }
}