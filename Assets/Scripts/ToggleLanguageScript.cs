using System;
using UnityEngine;
using UnityEngine.UI;

public class ToggleLanguageScript : MonoBehaviour
{
    public GameObject buttonGO;
    public GameObject unselectedFlag;
    public GameObject selectedFlag;

    public void SetSelected(bool selected)
    {
        selectedFlag.SetActive(selected);
        unselectedFlag.SetActive(!selected);
    }

    public void SetOnClickListener(Action listener)
    {
        buttonGO.GetComponent<Button>().onClick.AddListener(() => listener());
    }
}
