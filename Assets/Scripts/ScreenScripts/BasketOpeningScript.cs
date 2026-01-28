using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BasketOpeningScript : MonoBehaviour
{
    [SerializeField] private Button _clickListener;
    [SerializeField] private Image _instruction;
    [SerializeField] private Sprite _tapToOpenSprite;
    [SerializeField] private Sprite _continueSprite;
    [SerializeField] private GameObject _basket;
    [SerializeField] private GameObject _light;
    [SerializeField] private Sprite _commonBasket;
    [SerializeField] private Sprite _rareBasket;
    [SerializeField] private Sprite _epicBasket;
    [SerializeField] private Sprite _legendaryBasket;
    [SerializeField] private Image _accessory;
    [SerializeField] private TMP_Text _accessoryName;

    [Header("Accessory Sprites")]
    [SerializeField] private Sprite _orangeHatSprite;
    [SerializeField] private Sprite _partyHatSprite;
    [SerializeField] private Sprite _astronautHelmetSprite;
    //[SerializeField] private Sprite _unicornHornSprite;

    [SerializeField] private Sprite _scarfSprite;
    [SerializeField] private Sprite _pearlNecklaceSprite;
    [SerializeField] private Sprite _featherBoaSprite;
    //[SerializeField] private Sprite _goldChainSprite;

    [SerializeField] private Sprite _tshirtSprite;
    [SerializeField] private Sprite _floatieSprite;
    [SerializeField] private Sprite _spaceSuitSprite;
    //[SerializeField] private Sprite _royalRobesSprite;

    [SerializeField] private Sprite _mustacheSprite;
    [SerializeField] private Sprite _heartGlassesSprite;
    [SerializeField] private Sprite _superheroMaskSprite;
    //[SerializeField] private Sprite _goldMasqueradeMaskSprite;

    [SerializeField] private Sprite _rockSprite;
    [SerializeField] private Sprite _rubberDuckSprite;
    [SerializeField] private Sprite _floatingKoiFishSprite;
    //[SerializeField] private Sprite _goldenBabyCapySprite;

    private Dictionary<Accessory, Sprite> _accessoryToSprite;
    private RectTransform _basketRectTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // precondition - GameManager.BuyBasket() has been called
        _light.SetActive(false);
        _accessory.gameObject.SetActive(false);
        _accessoryName.gameObject.SetActive(false);
        _basketRectTransform = _basket.GetComponent<RectTransform>();
        _instruction.sprite = _tapToOpenSprite;
        _accessoryToSprite = new Dictionary<Accessory, Sprite>()
        {
            { Accessory.OrangeHat, _orangeHatSprite },
            { Accessory.PartyHat, _partyHatSprite },
            { Accessory.AstronautHelmet, _astronautHelmetSprite },
            //{ Accessory.UnicornHorn, _unicornHornSprite },
            { Accessory.Scarf, _scarfSprite },
            { Accessory.PearlNecklace, _pearlNecklaceSprite },
            { Accessory.FeatherBoa, _featherBoaSprite },
            //{ Accessory.GoldChain, _goldChainSprite },
            { Accessory.Tshirt, _tshirtSprite },
            { Accessory.Floatie, _floatieSprite },
            { Accessory.SpaceSuit, _spaceSuitSprite },
            //{ Accessory.RoyalRobes, _royalRobesSprite },
            { Accessory.Mustache, _mustacheSprite },
            { Accessory.HeartGlasses, _heartGlassesSprite },
            { Accessory.SuperheroMask, _superheroMaskSprite },
            //{ Accessory.GoldMasqueradeMask, _goldMasqueradeMaskSprite },
            { Accessory.Rock, _rockSprite },
            { Accessory.RubberDuck, _rubberDuckSprite },
            { Accessory.FloatingKoiFish, _floatingKoiFishSprite },

        };
        (Tier? tier, Accessory accessory) = InventoryManager.GetLastBasketInfo();
        if(tier == null)
        {
            throw new System.InvalidOperationException("No basket info available");
        }
        Tier tier1 = tier.Value;
        _basket.GetComponent<Image>().sprite = tier1 switch
        {
            Tier.Common => _commonBasket,
            Tier.Rare => _rareBasket,
            Tier.Epic => _epicBasket,
            Tier.Legendary => _legendaryBasket,
            _ => throw new System.Exception("Unknown tier")
        };
        _basketRectTransform.anchoredPosition = new Vector2(0, 0);
        _basketRectTransform.sizeDelta = tier1 switch
        {
            Tier.Common => new Vector2(268, 268),
            Tier.Rare => new Vector2(268, 268),
            Tier.Epic => new Vector2(360.5f, 360.5f),
            Tier.Legendary => new Vector2(360.5f, 360.5f),
            _ => throw new System.Exception("Unknown tier")
        };
        _clickListener.onClick.AddListener(() => 
        {
            if(_instruction.sprite == _tapToOpenSprite)
            {
                // open basket
                AudioManager.Instance.PlayUIEffect(Sound.OpenLootBox);
                _light.SetActive(true);
                _instruction.sprite = _continueSprite;
                _basketRectTransform.anchoredPosition = new Vector2(0, 196);
                _accessory.sprite = _accessoryToSprite[accessory];
                _accessoryName.text = accessory.Name;
                _accessoryName.color = accessory.Tier switch
                {
                    Tier.Common => new Color32(174,216,115,255), 
                    Tier.Rare => new Color32(115,189,216,255),
                    Tier.Epic => new Color32(163,114,228,255),
                    Tier.Legendary => new Color32(232,186,69,255), // todo - update
                    _ => Color.white
                };
                _accessory.gameObject.SetActive(true);
                _accessoryName.gameObject.SetActive(true);
                // todo - sound
            }
            else
            {
                SceneManager.LoadSceneAsync("Den");
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
