using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DropdownArrow : MonoBehaviour
{
    private bool _enabled = false;
    private Button _button;
    private const float AnimationDuration = .25f;

    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() =>
        {
            StopAllCoroutines();
            StartCoroutine(ToggleAppearance());
        });
    }
    private IEnumerator ToggleAppearance()
    {
        _enabled = !_enabled;
        Quaternion initialQuaternion = transform.rotation;
        float finalAngle = _enabled ? -90f : 0f;
        Quaternion finalQuaternion = Quaternion.Euler(0, 0, finalAngle);
        float time = 0;
        while (time < AnimationDuration)
        {
            time += Time.deltaTime;
            float lerpFactor = time / AnimationDuration;
            transform.rotation = Quaternion.LerpUnclamped(initialQuaternion, finalQuaternion, lerpFactor);
            yield return null; // wait until next frame
        }
        transform.rotation = finalQuaternion; // just in case
    }
}
