using UnityEngine;
using UnityEngine.UI;

public class AccessoryButtonScript : MonoBehaviour
{
    public Image Image;
    [SerializeField] private Button button;
    [SerializeField] private GameObject checkmark;
    public Accessory accessory;
    public void SetOnClickListener(UnityEngine.Events.UnityAction action) => button.onClick.AddListener(action);

    public void Check(bool check)
    {
        checkmark.SetActive(check);
    }

}
