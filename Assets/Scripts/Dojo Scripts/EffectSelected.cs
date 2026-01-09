using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EffectSelected : MonoBehaviour
{
    public Color outlineColor = Color.white;
    public Vector2 outlineDistance = new Vector2(2, -2);
    private List<Button> buttons = new List<Button>();
    private Button currentlySelected = null;

    [SerializeField] private AudioClip rain;
    [SerializeField] private AudioClip fire;
    [SerializeField] private AudioClip river;
    [SerializeField] private AudioClip ocean;
    [SerializeField] private AudioSource source;

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

        if (source.isPlaying)
        {
            source.Stop();
        }

        if (clickedButton.gameObject.name.Contains("Rain"))
        {
            source.PlayOneShot(rain);
        }
        else if (clickedButton.gameObject.name.Contains("Fire"))
        {
            source.PlayOneShot(fire);
        }
        else if (clickedButton.gameObject.name.Contains("River"))
        {
            source.PlayOneShot(river);
        }
        else if (clickedButton.gameObject.name.Contains("Ocean"))
        {
            source.PlayOneShot(ocean);
        }

        currentlySelected = clickedButton;
        SharedData.effectData = clickedButton.gameObject.name;
    }
}