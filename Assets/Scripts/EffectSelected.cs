using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EffectSelected : MonoBehaviour
{
    public Color outlineColor = Color.white;
    public Vector2 outlineDistance = new Vector2(2, -2);

    private List<Button> buttons = new List<Button>();
    private Button currentlySelected = null;

    void Start()
    {
        buttons.AddRange(GetComponentsInChildren<Button>());

        foreach (Button button in buttons)
        {
            Outline outline = button.GetComponent<Outline>();
            if (outline == null)
            {
                outline = button.gameObject.AddComponent<Outline>();
            }

            outline.effectColor = outlineColor;
            outline.effectDistance = outlineDistance;
            outline.enabled = false;

            button.onClick.AddListener(() => OnButtonClicked(button));
        }
    }

    private void OnButtonClicked(Button clickedButton)
    {
        if (currentlySelected != null)
        {
            Outline prevOutline = currentlySelected.GetComponent<Outline>();
            if (prevOutline != null)
            {
                prevOutline.enabled = false;
            }
        }

        Outline outline = clickedButton.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = true;
        }

        currentlySelected = clickedButton;
    }
}