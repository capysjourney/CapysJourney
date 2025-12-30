using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropdownText : MonoBehaviour
{
    private TMP_Dropdown dropdown;

    void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        if (dropdown != null)
        {
            dropdown.onValueChanged.AddListener(OnDropdownChanged);
        }
    }

    private void OnDropdownChanged(int index)
    {
        List<string> selectedItems = new List<string>();
        int bitFieldValue = dropdown.value;

        for (int i = 0; i < dropdown.options.Count; i++)
            if ((bitFieldValue & (1 << i)) != 0)
                selectedItems.Add(dropdown.options[i].text);

        if (dropdown.captionText != null)
            dropdown.captionText.text = selectedItems.Count == 1 ? "1 Region" : $"{selectedItems.Count} Regions";
    }

}