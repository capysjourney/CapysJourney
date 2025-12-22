using UnityEngine;

public class DojoTabScript : MonoBehaviour
{
    [SerializeField] private GameObject customPage;
    [SerializeField] private GameObject libraryPage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ShowCustom();
    }

    public void ShowCustom()
    {
        libraryPage.SetActive(false);
        customPage.SetActive(true);
    }

    public void ShowLibrary()
    {
        customPage.SetActive(false);
        libraryPage.SetActive(true);
    }

}
