using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DenScript : MonoBehaviour
{
    [Header("Basket Options")]
    [SerializeField] private BasketOptionScript _commonBasket;
    [SerializeField] private BasketOptionScript _rareBasket;
    [SerializeField] private BasketOptionScript _epicBasket;
    [SerializeField] private BasketOptionScript _legendaryBasket;

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

    [Header("Accessory Unselected Tab Sprites")]
    [SerializeField] private Sprite _orangeHatUnselected;
    [SerializeField] private Sprite _partyHatUnselected;
    [SerializeField] private Sprite _astronautHelmetUnselected;
    //[SerializeField] private Sprite _unicornHornUnselected;

    [SerializeField] private Sprite _scarfUnselected;
    [SerializeField] private Sprite _pearlNecklaceUnselected;
    [SerializeField] private Sprite _featherBoaUnselected;
    //[SerializeField] private Sprite _goldChainUnselected;

    [SerializeField] private Sprite _tshirtUnselected;
    [SerializeField] private Sprite _floatieUnselected;
    [SerializeField] private Sprite _spaceSuitUnselected;
    //[SerializeField] private Sprite _royalRobesUnselected;

    [SerializeField] private Sprite _mustacheUnselected;
    [SerializeField] private Sprite _heartGlassesUnselected;
    [SerializeField] private Sprite _superheroMaskUnselected;
    //[SerializeField] private Sprite _goldMasqueradeMaskUnselected;

    [SerializeField] private Sprite _rockUnselected;
    [SerializeField] private Sprite _rubberDuckUnselected;
    [SerializeField] private Sprite _floatingKoiFishUnselected;
    //[SerializeField] private Sprite _goldenBabyCapyUnselected;

    [Header("Accessory Selected Tab Sprites")]
    [SerializeField] private Sprite _orangeHatSelected;
    [SerializeField] private Sprite _partyHatSelected;
    [SerializeField] private Sprite _astronautHelmetSelected;
    //[SerializeField] private Sprite _unicornHornSelected;

    [SerializeField] private Sprite _scarfSelected;
    [SerializeField] private Sprite _pearlNecklaceSelected;
    [SerializeField] private Sprite _featherBoaSelected;
    //[SerializeField] private Sprite _goldChainSelected;

    [SerializeField] private Sprite _tshirtSelected;
    [SerializeField] private Sprite _floatieSelected;
    [SerializeField] private Sprite _spaceSuitSelected;
    //[SerializeField] private Sprite _royalRobesSelected;

    [SerializeField] private Sprite _mustacheSelected;
    [SerializeField] private Sprite _heartGlassesSelected;
    [SerializeField] private Sprite _superheroMaskSelected;
    //[SerializeField] private Sprite _goldMasqueradeMaskSelected;

    [SerializeField] private Sprite _rockSelected;
    [SerializeField] private Sprite _rubberDuckSelected;
    [SerializeField] private Sprite _floatingKoiFishSelected;
    //[SerializeField] private Sprite _goldenBabyCapySelected;

    [Header("Accessory Tabs")]
    [SerializeField] private Button _hatTab;
    [SerializeField] private Button _neckwearTab;
    [SerializeField] private Button _clothingTab;
    [SerializeField] private Button _faceTab;
    [SerializeField] private Button _petTab;

    [Header("Accessory Tab Selected Sprites")]
    [SerializeField] private Sprite _hatTabSelected;
    [SerializeField] private Sprite _neckwearTabSelected;
    [SerializeField] private Sprite _clothingTabSelected;
    [SerializeField] private Sprite _faceTabSelected;
    [SerializeField] private Sprite _petTabSelected;

    [Header("Accessory Tab Unselected Sprites")]
    [SerializeField] private Sprite _hatTabUnselected;
    [SerializeField] private Sprite _neckwearTabUnselected;
    [SerializeField] private Sprite _clothingTabUnselected;
    [SerializeField] private Sprite _faceTabUnselected;
    [SerializeField] private Sprite _petTabUnselected;

    [Header("Misc")]
    [SerializeField] private GameObject _accessoryPopup;
    [SerializeField] private RectTransform _triangle;
    [SerializeField] private Button _closePopupButton;
    [SerializeField] private TMP_Text _accessoryPopupHeader;
    [SerializeField] private TMP_Text _numAccessoriesOwnedText;
    [SerializeField] private GridLayoutGroup _accessoryGrid;
    [SerializeField] private GameObject _accessoryItemPrefab;

    /// <summary>
    /// Parent object for all capy accessories; has an image component of capy.
    /// </summary>
    [SerializeField] private GameObject _capyGroup;

    private Dictionary<Accessory, Sprite> _spriteByAccessory;

    /// <summary>
    /// Currently selected accessory tab; null if no tab is selected
    /// </summary>
    private AccessoryType? _selectedAccessoryTab = null;

    private Dictionary<Button, AccessoryType> _accessoryTypeByTabButton;
    private Dictionary<AccessoryType, Button> _tabButtonByAccessoryType;

    private Dictionary<AccessoryType, Sprite> _selectedTabSpriteByType;
    private Dictionary<AccessoryType, Sprite> _unselectedTabSpriteByType;
    private Dictionary<Accessory, Sprite> _selectedSpriteByAccessory;
    private Dictionary<Accessory, Sprite> _unselectedSpriteByAccessory;
    private Dictionary<Accessory, AccessoryTransform> _transformByAccessory;

    private class AccessoryTransform
    {
        public float PosX;
        public float PosY;
        public float Width;
        public float Height;
        public AccessoryTransform(float posX = 0.0f, float posY = 0.0f, float width = 0.0f, float height = 0.0f)
        {
            PosX = posX;
            PosY = posY;
            Width = width;
            Height = height;
        }
    }

    void Start()
    {
        _commonBasket.SetOnClickListener(() => BuyBasket(Tier.Common));
        _rareBasket.SetOnClickListener(() => BuyBasket(Tier.Rare));
        _epicBasket.SetOnClickListener(() => BuyBasket(Tier.Epic));
        _legendaryBasket.SetOnClickListener(() => BuyBasket(Tier.Legendary));
        _spriteByAccessory = new()
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
            //{ Accessory.GoldenBabyCapy, _goldenBabyCapySprite }
        };
        _accessoryTypeByTabButton = new()
        {
            { _hatTab, AccessoryType.Hat },
            { _neckwearTab, AccessoryType.Neckwear },
            { _clothingTab, AccessoryType.Clothing },
            { _faceTab, AccessoryType.Facewear },
            { _petTab, AccessoryType.Pet }
        };
        _tabButtonByAccessoryType = _accessoryTypeByTabButton.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        _selectedTabSpriteByType = new()
        {
            { AccessoryType.Hat, _hatTabSelected },
            { AccessoryType.Neckwear, _neckwearTabSelected },
            { AccessoryType.Clothing, _clothingTabSelected },
            { AccessoryType.Facewear, _faceTabSelected },
            { AccessoryType.Pet, _petTabSelected }
        };
        _unselectedTabSpriteByType = new()
        {
            { AccessoryType.Hat, _hatTabUnselected },
            { AccessoryType.Neckwear, _neckwearTabUnselected },
            { AccessoryType.Clothing, _clothingTabUnselected },
            { AccessoryType.Facewear, _faceTabUnselected },
            { AccessoryType.Pet, _petTabUnselected }
        };
        _selectedSpriteByAccessory = new()
        {
            { Accessory.OrangeHat, _orangeHatSelected },
            { Accessory.PartyHat, _partyHatSelected },
            { Accessory.AstronautHelmet, _astronautHelmetSelected },
            //{ Accessory.UnicornHorn, _unicornHornSelected },
            { Accessory.Scarf, _scarfSelected },
            { Accessory.PearlNecklace, _pearlNecklaceSelected },
            { Accessory.FeatherBoa, _featherBoaSelected },
            //{ Accessory.GoldChain, _goldChainSelected },
            { Accessory.Tshirt, _tshirtSelected },
            { Accessory.Floatie, _floatieSelected },
            { Accessory.SpaceSuit, _spaceSuitSelected },
            //{ Accessory.RoyalRobes, _royalRobesSelected },
            { Accessory.Mustache, _mustacheSelected },
            { Accessory.HeartGlasses, _heartGlassesSelected },
            { Accessory.SuperheroMask, _superheroMaskSelected },
            //{ Accessory.GoldMasqueradeMask, _goldMasqueradeMaskSelected },
            { Accessory.Rock, _rockSelected },
            { Accessory.RubberDuck, _rubberDuckSelected },
            { Accessory.FloatingKoiFish, _floatingKoiFishSelected },
            //{ Accessory.GoldenBabyCapy, _goldenBabyCapySelected  
        };
        _unselectedSpriteByAccessory = new()
        {
            { Accessory.OrangeHat, _orangeHatUnselected },
            { Accessory.PartyHat, _partyHatUnselected },
            { Accessory.AstronautHelmet, _astronautHelmetUnselected },
            //{ Accessory.UnicornHorn, _unicornHornUnselected },
            { Accessory.Scarf, _scarfUnselected },
            { Accessory.PearlNecklace, _pearlNecklaceUnselected },
            { Accessory.FeatherBoa, _featherBoaUnselected },
            //{ Accessory.GoldChain, _goldChainUnselected },
            { Accessory.Tshirt, _tshirtUnselected },
            { Accessory.Floatie, _floatieUnselected },
            { Accessory.SpaceSuit, _spaceSuitUnselected },
            //{ Accessory.RoyalRobes, _royalRobesUnselected },
            { Accessory.Mustache, _mustacheUnselected },
            { Accessory.HeartGlasses, _heartGlassesUnselected },
            { Accessory.SuperheroMask, _superheroMaskUnselected },
            //{ Accessory.GoldMasqueradeMask, _goldMasqueradeMaskUnselected },
            { Accessory.Rock, _rockUnselected },
            { Accessory.RubberDuck, _rubberDuckUnselected },
            { Accessory.FloatingKoiFish, _floatingKoiFishUnselected }
            //{ Accessory.GoldenBabyCapy, _goldenBabyCapyUnselected}
        };
        _transformByAccessory = new()
        {
            { Accessory.OrangeHat, new AccessoryTransform(posX: 15,posY:123,width:53.61f,height:47.04f) },
            { Accessory.PartyHat, new AccessoryTransform(posX: 14.3f,posY: 128.4f,width:50f, height:70f) },
            { Accessory.AstronautHelmet, new AccessoryTransform(posX: 17,posY: 64,width: 204.1f,height:177.03f) },
            //{ Accessory.UnicornHorn, new AccessoryTransform() },
            { Accessory.Scarf, new AccessoryTransform(posX: 6.1f,posY: 9.1f, width: 132.09f, height:89.4f) },
            { Accessory.PearlNecklace, new AccessoryTransform(posX: 6.1f,posY: 12.9f, width: 132.09f, height:89.4f) },
            { Accessory.FeatherBoa, new AccessoryTransform(posX: 6.6f, posY: -13.6f, width: 143.9f, height: 105.4f) },
            //{ Accessory.GoldChain, new AccessoryTransform() },
            { Accessory.Tshirt, new AccessoryTransform(posX: -7.5f, posY: -11.6f, width: 179.02f, height: 108.6f) },
            { Accessory.Floatie, new AccessoryTransform(posX: -8.3f, posY: -32.5f, width: 189.3f, height: 108.6f) },
            { Accessory.SpaceSuit, new AccessoryTransform(posX: -5.6f,posY: -36.3f,width: 189.4f, height:140.5f) },
            //{ Accessory.RoyalRobes, new AccessoryTransform() },
            { Accessory.Mustache, new AccessoryTransform(posX: 72.2f, posY: 38.5f, width: 87.9f, height: 46f) },
            { Accessory.HeartGlasses, new AccessoryTransform(posX: 29.3f, posY: 80.3f, width: 174,height: 66.8f) },
            { Accessory.SuperheroMask, new AccessoryTransform(posX: 29.3f, posY: 74, width: 138.7f, height: 60f) },
            //{ Accessory.GoldMasqueradeMask, new AccessoryTransform() },
            { Accessory.Rock, new AccessoryTransform(posX: -81.5f, posY: -92.5f, width: 99.56f, height: 56.48f) },
            { Accessory.RubberDuck, new AccessoryTransform(posX: 80f, posY: -71.5f, width: 80.91f, height: 79f) },
            { Accessory.FloatingKoiFish, new AccessoryTransform(posX: 75.8f, posY: -32.9f, width: 105.77f, height:79f) }
            //{ Accessory.GoldenBabyCapy, new AccessoryTransform() }
        };
        foreach (var pair in _accessoryTypeByTabButton)
        {
            Button tabButton = pair.Key;
            AccessoryType type = pair.Value;
            tabButton.onClick.AddListener(() => SelectAccessoryTab(type));
        }
        _closePopupButton.onClick.AddListener(ClosePopup);
        ClosePopup();
        DressCapy();
    }

    private void BuyBasket(Tier type)
    {
        bool bought = InventoryManager.BuyBasket(type);
        if (bought)
        {
            AudioManager.Instance.PlayUIEffect(Sound.BuyItemFromShop);
            SceneManager.LoadSceneAsync("BasketOpening");
        }
        else
        {
            // todo
            Debug.Log("couldn't buy basket");
        }
    }

    private void SelectAccessoryTab(AccessoryType type)
    {
        if (type == _selectedAccessoryTab)
        {
            ClosePopup();
            return;
        }
        ResetAccessoryTabs();
        List<Accessory> accessories = InventoryManager.GetOwnedAccessoriesOfType(type);
        ConfigureAndShowPopup(type, accessories);
        ConfigurePopupGrid(type, accessories);
    }

    private void ConfigureAndShowPopup(AccessoryType type, List<Accessory> accessories)
    {
        _selectedAccessoryTab = type;
        _tabButtonByAccessoryType[type].image.sprite = _selectedTabSpriteByType[type];
        _accessoryPopupHeader.text = type switch
        {
            AccessoryType.Hat => "Hats",
            AccessoryType.Neckwear => "Neckwear",
            AccessoryType.Clothing => "Clothing",
            AccessoryType.Facewear => "Facewear",
            AccessoryType.Pet => "Pets",
            _ => throw new System.Exception("Unknown AccessoryType")
        };
        _numAccessoriesOwnedText.text = $"{accessories.Count}/{InventoryManager.NumTotalAccessoriesOfType(type)} Owned";
        float highestY = -28.5f;

        // difference in height between tab buttons
        float diff = 65f;

        float newTriangleY = type switch
        {
            AccessoryType.Hat => highestY,
            AccessoryType.Neckwear => highestY,
            AccessoryType.Clothing => highestY,
            AccessoryType.Facewear => highestY - diff,
            AccessoryType.Pet => highestY - 2 * diff,
            _ => throw new System.Exception("Unknown AccessoryType")
        };
        _triangle.anchoredPosition = new Vector2(_triangle.anchoredPosition.x, newTriangleY);
        float highestPopupY = -58.5f;
        float newPopupY = type switch
        {
            AccessoryType.Hat => highestPopupY,
            AccessoryType.Neckwear => highestPopupY - diff,
            AccessoryType.Clothing => highestPopupY - 2 * diff,
            AccessoryType.Facewear => highestPopupY - 2 * diff,
            AccessoryType.Pet => highestPopupY - 2 * diff,
            _ => throw new System.Exception("Unknown AccessoryType")
        };
        RectTransform popupRect = _accessoryPopup.GetComponent<RectTransform>();
        popupRect.anchoredPosition = new Vector2(popupRect.anchoredPosition.x, newPopupY);
        _accessoryPopup.SetActive(true);
    }

    private void ConfigurePopupGrid(AccessoryType type, List<Accessory> accessories)
    {
        foreach (Transform child in _accessoryGrid.transform)
        {
            Destroy(child.gameObject);
        }
        Accessory accInUse = InventoryManager.GetCurrentAccessoryOfType(type);
        foreach (Accessory accessory in accessories)
        {
            GameObject accessoryItem = Instantiate(_accessoryItemPrefab, _accessoryGrid.transform);
            AccessoryButtonScript accessoryBtn = accessoryItem.GetComponent<AccessoryButtonScript>();

            // if no accessory of this type is used, or if this accessory is the one in use, show selected sprite. Otherwise, show unselected sprite
            Sprite sprite = accInUse == null || accInUse == accessory
                ? _selectedSpriteByAccessory[accessory]
                : _unselectedSpriteByAccessory[accessory];
            accessoryBtn.SetCheckmark(accInUse == accessory);
            accessoryBtn.Image = accessoryItem.GetComponent<Image>();
            accessoryBtn.Image.sprite = sprite;
            accessoryBtn.accessory = accessory;
            accessoryBtn.SetOnClickListener(() =>
            {
                // if the accessory is already in use, reset to no accessory
                bool reset = InventoryManager.GetCurrentAccessoryOfType(type) != null
                    && accessoryBtn.Image.sprite == _selectedSpriteByAccessory[accessory];
                if (reset) { InventoryManager.StopUsingAccessory(type); }
                else
                {
                    AudioManager.Instance.PlayUIEffect(Sound.EquipItem);
                    InventoryManager.UseAccessory(accessory);
                }
                foreach (Transform childButtonTransform in _accessoryGrid.transform)
                {
                    AccessoryButtonScript childButtonScript = childButtonTransform.GetComponent<AccessoryButtonScript>();
                    Accessory childAccessory = childButtonScript.accessory;
                    if (childAccessory != null)
                    {
                        Sprite childSprite = !reset && childAccessory != accessory
                                ? _unselectedSpriteByAccessory[childAccessory]
                                : _selectedSpriteByAccessory[childAccessory];
                        childButtonScript.Image.sprite = childSprite;
                        childButtonScript.SetCheckmark(!reset && childAccessory == accessory);
                    }
                }
                DressCapy();
            });
        }
    }

    private void ClosePopup()
    {
        _accessoryPopup.SetActive(false);
        ResetAccessoryTabs();
        _selectedAccessoryTab = null;
    }

    private void ResetAccessoryTabs()
    {
        foreach (Button tabButton in _accessoryTypeByTabButton.Keys)
        {
            tabButton.image.sprite = _unselectedTabSpriteByType[_accessoryTypeByTabButton[tabButton]];
        }
    }

    private void DressCapy()
    {
        Accessory hat = InventoryManager.GetCurrentAccessoryOfType(AccessoryType.Hat);
        Accessory neckwear = InventoryManager.GetCurrentAccessoryOfType(AccessoryType.Neckwear);
        Accessory clothing = InventoryManager.GetCurrentAccessoryOfType(AccessoryType.Clothing);
        Accessory facewear = InventoryManager.GetCurrentAccessoryOfType(AccessoryType.Facewear);
        Accessory pet = InventoryManager.GetCurrentAccessoryOfType(AccessoryType.Pet);
        // order matters here for layering
        Accessory[] accessories = { clothing, facewear, pet, neckwear, hat };

        foreach (Transform child in _capyGroup.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Accessory accessory in accessories)
        {
            if (accessory == null) continue;
            Sprite sprite = _spriteByAccessory[accessory];
            GameObject accessoryGO = new();
            accessoryGO.transform.SetParent(_capyGroup.transform);
            AccessoryTransform transform = _transformByAccessory[accessory];
            Image img = accessoryGO.AddComponent<Image>();
            img.preserveAspect = true;
            img.sprite = sprite;
            RectTransform rectTransform = accessoryGO.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = new Vector2(transform.PosX, transform.PosY);
            rectTransform.sizeDelta = new Vector2(transform.Width, transform.Height);
            rectTransform.localScale = new Vector3(1, 1, 1);
        }
    }
}
