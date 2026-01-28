using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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
        PlayerStats stats = DataManager.GetStats();
        if (stats == null || stats.MeditationLog.Count == 0) return;

        int siblingIndex = transform.parent.parent.GetSiblingIndex();
        int index = siblingIndex - 1;

        if (index >= 0 && index < stats.MeditationLog.Count)
        {
            var entryToRemove = stats.MeditationLog.ElementAt(index);
            stats.MeditationLog.Remove(entryToRemove);

            stats.SaveToFirestore();

            Destroy(transform.parent.parent.gameObject);

            DojoController dojoController = FindFirstObjectByType<DojoController>();
            if (dojoController != null)
            {
                dojoController.RefreshMeditations();
            }
        }
    }
}
