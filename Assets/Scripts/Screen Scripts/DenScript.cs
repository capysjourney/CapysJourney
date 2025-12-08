using System.Collections.Generic;
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
    [SerializeField] private GameObject _capy;
    [SerializeField] private GameObject _capyGroup;

    private Dictionary<Accessory, Sprite> _accessorySpriteMap;
    private AccessoryType? _selectedAccessoryTab = null;
    private Dictionary<Button, AccessoryType> _accessoryTypeOfTabButton;
    private Dictionary<AccessoryType, Button> _buttonOfType;
    private Dictionary<AccessoryType, Sprite> _selectedTabMap;
    private Dictionary<AccessoryType, Sprite> _unselectedTabMap;
    private Dictionary<Accessory, Sprite> _selectedAccessoryMap;
    private Dictionary<Accessory, Sprite> _unselectedAccessoryMap;
    private Dictionary<Accessory, AccessoryTransform> _transformOfAccessory;

    private class AccessoryTransform
    {
        public float LeftAnchor;
        public float RightAnchor;
        public float TopAnchor;
        public float BottomAnchor;
        public float Left;
        public float Right;
        public float Top;
        public float Bottom;
        public AccessoryTransform(float leftAnchor = 0, float rightAnchor = 1, float topAnchor = 1, float bottomAnchor = 0, float left = 0, float right = 0, float top = 0, float bottom = 0)
        {
            LeftAnchor = leftAnchor;
            RightAnchor = rightAnchor;
            TopAnchor = topAnchor;
            BottomAnchor = bottomAnchor;
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _commonBasket.SetOnClickListener(() => BuyBasket(Tier.Common));
        _rareBasket.SetOnClickListener(() => BuyBasket(Tier.Rare));
        _epicBasket.SetOnClickListener(() => BuyBasket(Tier.Epic));
        _legendaryBasket.SetOnClickListener(() => BuyBasket(Tier.Legendary));
        _accessorySpriteMap = new()
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
        _accessoryTypeOfTabButton = new()
        {
            { _hatTab, AccessoryType.Hat },
            { _neckwearTab, AccessoryType.Neckwear },
            { _clothingTab, AccessoryType.Clothing },
            { _faceTab, AccessoryType.Facewear },
            { _petTab, AccessoryType.Pet }
        };
        _buttonOfType = new()
        {
            { AccessoryType.Hat, _hatTab },
            { AccessoryType.Neckwear, _neckwearTab },
            { AccessoryType.Clothing, _clothingTab },
            { AccessoryType.Facewear, _faceTab },
            { AccessoryType.Pet, _petTab }
        };
        _selectedTabMap = new()
        {
            { AccessoryType.Hat, _hatTabSelected },
            { AccessoryType.Neckwear, _neckwearTabSelected },
            { AccessoryType.Clothing, _clothingTabSelected },
            { AccessoryType.Facewear, _faceTabSelected },
            { AccessoryType.Pet, _petTabSelected }
        };
        _unselectedTabMap = new()
        {
            { AccessoryType.Hat, _hatTabUnselected },
            { AccessoryType.Neckwear, _neckwearTabUnselected },
            { AccessoryType.Clothing, _clothingTabUnselected },
            { AccessoryType.Facewear, _faceTabUnselected },
            { AccessoryType.Pet, _petTabUnselected }
        };
        _selectedAccessoryMap = new()
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
        _unselectedAccessoryMap = new()
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
        _transformOfAccessory = new()
        {
            { Accessory.OrangeHat, new AccessoryTransform(leftAnchor:0.4456357f, rightAnchor:0.658876f, bottomAnchor: 0.9135278f, top:-28.3f) },
            { Accessory.PartyHat, new AccessoryTransform(leftAnchor:0.468876f, rightAnchor:0.6783799f, bottomAnchor:0.8678057f, top:-41.5f) },
            { Accessory.AstronautHelmet, new AccessoryTransform(leftAnchor: 0.1317519f, bottomAnchor:0.386f, right:-3.5f, top:-24.6f) },
            //{ Accessory.UnicornHorn, new AccessoryTransform() },
            { Accessory.Scarf, new AccessoryTransform(leftAnchor:0.2325039f,rightAnchor:0.8256202f, bottomAnchor:0.3794722f, topAnchor:0.7180001f) },
            { Accessory.PearlNecklace, new AccessoryTransform(leftAnchor:0.2325039f,rightAnchor:0.8256202f, bottomAnchor:0.3794722f, topAnchor:0.7180001f) },
            { Accessory.FeatherBoa, new AccessoryTransform(leftAnchor:0.2015039f,rightAnchor:0.9031241f,bottomAnchor:0.2326111f,topAnchor:0.6805278f) },
            //{ Accessory.GoldChain, new AccessoryTransform() },
            { Accessory.Tshirt, new AccessoryTransform(leftAnchor:0.07749613f,rightAnchor:0.8255969f,bottomAnchor:0.1978889f,topAnchor:0.6526946f) },
            { Accessory.Floatie, new AccessoryTransform(leftAnchor:0.003875969f, rightAnchor:0.8953722f, bottomAnchor:0.184f, topAnchor:0.5035278f)},
            { Accessory.SpaceSuit, new AccessoryTransform(leftAnchor:0.08137209f,rightAnchor:0.8604962f,bottomAnchor:0.066f, topAnchor:0.6456946f) },
            //{ Accessory.RoyalRobes, new AccessoryTransform() },
            { Accessory.Mustache, new AccessoryTransform(leftAnchor:0.624f,bottomAnchor:0.566f, topAnchor:0.7360001f,right:-7f) },
            { Accessory.HeartGlasses, new AccessoryTransform(leftAnchor:0.2712481f,rightAnchor:0.9806201f,bottomAnchor:0.6805278f,topAnchor:0.9720001f) },
            { Accessory.SuperheroMask, new AccessoryTransform(leftAnchor:0.313876f, rightAnchor:0.9457442f, bottomAnchor:0.6734722f,topAnchor:0.913f) },
            //{ Accessory.GoldMasqueradeMask, new AccessoryTransform() },
            { Accessory.Rock, new AccessoryTransform(rightAnchor:0.3333799f,bottomAnchor:0.02083333f,topAnchor:0.25f,left:-23) },
            { Accessory.RubberDuck, new AccessoryTransform(leftAnchor:0.693752f,bottomAnchor:0.06247223f,topAnchor:0.3785278f,right:-10)},
            { Accessory.FloatingKoiFish, new AccessoryTransform(leftAnchor:0.6472481f, bottomAnchor:0.198f, topAnchor:0.5384722f,right:-21) }
            //{ Accessory.GoldenBabyCapy, new AccessoryTransform() }
        };
        foreach (var pair in _accessoryTypeOfTabButton)
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
        bool bought = GameManager.BuyBasket(type);
        if (bought)
        {
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
        List<Accessory> accessories = GameManager.GetOwnedAccessoriesOfType(type);
        ConfigureAndShowPopup(type, accessories);
        ConfigureGrid(type, accessories);
    }

    private void ConfigureAndShowPopup(AccessoryType type, List<Accessory> accessories)
    {
        _selectedAccessoryTab = type;
        _buttonOfType[type].image.sprite = _selectedTabMap[type];
        _accessoryPopupHeader.text = type switch
        {
            AccessoryType.Hat => "Hats",
            AccessoryType.Neckwear => "Neckwear",
            AccessoryType.Clothing => "Clothing",
            AccessoryType.Facewear => "Facewear",
            AccessoryType.Pet => "Pets",
            _ => throw new System.Exception("Unknown AccessoryType")
        };
        _numAccessoriesOwnedText.text = $"{accessories.Count}/{GameManager.NumTotalAccessoriesOfType(type)} Owned";
        float highestY = -28.5f;
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

    private void ConfigureGrid(AccessoryType type, List<Accessory> accessories)
    {
        foreach (Transform child in _accessoryGrid.transform)
        {
            Destroy(child.gameObject);
        }
        Accessory accInUse = GameManager.GetCurrentAccessoryOfType(type);
        foreach (Accessory accessory in accessories)
        {
            GameObject accessoryItem = Instantiate(_accessoryItemPrefab, _accessoryGrid.transform);
            AccessoryButtonScript accBtn = accessoryItem.GetComponent<AccessoryButtonScript>();

            // if no accessory of this type is used, or if this accessory is the one in use, show selected sprite. Otherwise, show unselected sprite
            Sprite sprite = accInUse == null || accInUse == accessory ? _selectedAccessoryMap[accessory] : _unselectedAccessoryMap[accessory];
            accBtn.Check(accInUse == accessory);
            accBtn.Image = accessoryItem.GetComponent<Image>();
            accBtn.Image.sprite = sprite;
            accBtn.accessory = accessory;
            accBtn.SetOnClickListener(() =>
            {
                // if the accessory is already in use, reset to no accessory
                bool reset = GameManager.GetCurrentAccessoryOfType(type) != null
                    && accBtn.Image.sprite == _selectedAccessoryMap[accessory];
                if (reset) GameManager.StopUsingAccessory(type);
                else GameManager.UseAccessory(accessory);
                foreach (Transform child in _accessoryGrid.transform)
                {
                    AccessoryButtonScript childButtonScript = child.GetComponent<AccessoryButtonScript>();
                    Accessory childAccessory = childButtonScript.accessory;
                    if (childAccessory != null)
                    {
                        Sprite childSprite = !reset && childAccessory != accessory
                                ? _unselectedAccessoryMap[childAccessory]
                                : _selectedAccessoryMap[childAccessory];
                        childButtonScript.Image.sprite = childSprite;
                        childButtonScript.Check(!reset && childAccessory == accessory);
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
        foreach (Button tabButton in _accessoryTypeOfTabButton.Keys)
        {
            tabButton.image.sprite = _unselectedTabMap[_accessoryTypeOfTabButton[tabButton]];
        }
    }

    private void DressCapy()
    {
        // get all accessories currently being used
        Accessory hat = GameManager.GetCurrentAccessoryOfType(AccessoryType.Hat);
        Accessory neckwear = GameManager.GetCurrentAccessoryOfType(AccessoryType.Neckwear);
        Accessory clothing = GameManager.GetCurrentAccessoryOfType(AccessoryType.Clothing);
        Accessory facewear = GameManager.GetCurrentAccessoryOfType(AccessoryType.Facewear);
        Accessory pet = GameManager.GetCurrentAccessoryOfType(AccessoryType.Pet);
        Accessory[] accessories = { clothing, facewear, pet, neckwear, hat }; // order matters
        foreach (Transform child in _capyGroup.transform)
        {
            if (child != _capy.transform)
            {
                Destroy(child.gameObject);
            }
        }
        Sprite[] sprites = new Sprite[5];
        for (int i = 0; i < accessories.Length; i++)
        {
            Accessory acc = accessories[i];
            if (acc == null) continue;
            Sprite sprite = _accessorySpriteMap[acc];
            sprites[i] = sprite;
            GameObject gameObject = new();
            gameObject.transform.SetParent(_capyGroup.transform);
            AccessoryTransform transform = _transformOfAccessory[acc];
            Image img = gameObject.AddComponent<Image>();
            img.preserveAspect = true;
            img.sprite = sprite;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(transform.LeftAnchor, transform.BottomAnchor);
            rectTransform.anchorMax = new Vector2(transform.RightAnchor, transform.TopAnchor);
            rectTransform.offsetMin = new Vector2(transform.Left, transform.Bottom);
            rectTransform.offsetMax = new Vector2(transform.Right, transform.Top);
            rectTransform.localScale = new Vector3(1, 1, 1);
        }
    }
}
