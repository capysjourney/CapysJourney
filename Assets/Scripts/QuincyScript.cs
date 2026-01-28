using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuincyScript : MonoBehaviour
{
    [SerializeField] private TMP_Text _subtitle;
    [SerializeField] private TMP_Text _body;
    [SerializeField] private Button _mainButton;
    [SerializeField] private GameObject _selectionButtonArea;
    [SerializeField] private GameObject _rewardBox;
    [SerializeField] private TMP_Text _rewardText;

    [Header("Degree Buttons")]
    [SerializeField] private Button _degree1Button;
    [SerializeField] private Button _degree2Button;
    [SerializeField] private Button _degree3Button;
    [SerializeField] private Button _degree4Button;
    [SerializeField] private Button _degree5Button;

    [Header("Button Sprites")]
    [SerializeField] private Sprite _unselectedButtonSprite;
    [SerializeField] private Sprite _selectedButtonSprite;

    private TMP_Text _mainButtonText;
    private int _stage = 0;
    private int _score = 0;
    private int _currentlySelectedDegree = -1;
    private Button[] _degreeButtons;
    private Image[] _degreeButtonImages;
    private TMP_Text[] _degreeButtonTexts;
    private AgeGroup _ageGroup;
    private Action _onFinish;

    private readonly string[] questionsForOlder = new string[]
    {
        "I notice what’s happening in my body and surroundings right now, without being lost in thoughts about the past or future.",
        "I can recognize and name what I’m feeling in the moment.",
        "I allow my thoughts and emotions to exist without labeling them as “good” or “bad.”",
        "When something upsetting happens, I can pause and respond calmly instead of reacting automatically.",
        "I notice small details in my environment — like sounds, textures, or smells — that I might otherwise overlook.",
        "When I do something, I give it my full attention rather than operating on “autopilot.”",
        "I can notice an unpleasant thought or feeling and let it pass without holding onto it.",
        "I’m aware of how my emotions affect my posture, tone of voice, and actions.",
    };

    private readonly string[] questionsForYounger = new string[]
    {
        "I notice what’s around me, like Capy noticing the forest.",
        "I can tell what I’m feeling, like naming clouds in the sky.",
        "I don’t call my feelings “good” or “bad,” I just watch them float by.",
        "I can take a slow breath, like Capy before crossing a stream.",
        "I notice tiny things, like a frog’s ripple in the water.",
        "I focus on what I’m doing, like Capy building a sandcastle.",
        "I let tricky thoughts drift away, like leaves on a river.",
        "I notice how my body feels, like Capy feeling the sun and wind."
    };

    public class MindfulnessTier
    {
        public int MinScore;
        public int MaxScore;
        public string Title;
        public string Description;
        private MindfulnessTier(int minScore, int maxScore, string title, string description)
        {
            MinScore = minScore;
            MaxScore = maxScore;
            Title = title;
            Description = description;
        }
        public static MindfulnessTier BuddingTraveler = new(8, 16, "Budding Traveler",
            "You’re just beginning your mindfulness adventure, and that’s something to be proud of. Keep noticing little details around you and taking slow, gentle breaths.");
        public static MindfulnessTier GrowingAdventurer = new(17, 24, "Growing Adventurer",
            "You’re learning to pause, notice, and focus more often. Each day you’re building new skills, like Capy watching ripples move across the water.");
        public static MindfulnessTier BloomingExplorer = new(25, 32, "Blooming Explorer",
            "You’re getting better at staying present and working with tricky thoughts. You notice your feelings and let go when you need to.");
        public static MindfulnessTier MindfulVoyager = new(33, 40, "Mindful Voyager",
            "You’ve built strong mindfulness skills and respond with more calm and care. Like Capy resting by the river, you can notice and breathe with ease.");
        private static readonly List<MindfulnessTier> Tiers = new()
        {
            BuddingTraveler,
            GrowingAdventurer,
            BloomingExplorer,
            MindfulVoyager
        };
        public static MindfulnessTier GetTier(int score)
        {
            foreach (MindfulnessTier tier in Tiers)
            {
                if (score >= tier.MinScore && score <= tier.MaxScore)
                {
                    return tier;
                }
            }
            return null;
        }

    }

    public void SetOnFinish(Action onFinish)
    {
        _onFinish = onFinish;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _ageGroup = GameManager.GetAgeGroup();
        _degreeButtons = new Button[] { _degree1Button, _degree2Button, _degree3Button, _degree4Button, _degree5Button };
        _degreeButtonImages = new Image[_degreeButtons.Length];
        for (int i = 0; i < _degreeButtons.Length; i++)
        {
            _degreeButtonImages[i] = _degreeButtons[i].GetComponent<Image>();
        }
        _degreeButtonTexts = new TMP_Text[_degreeButtons.Length];
        for (int i = 0; i < _degreeButtons.Length; i++)
        {
            _degreeButtonTexts[i] = _degreeButtons[i].GetComponentInChildren<TMP_Text>();
        }
        _mainButtonText = _mainButton.GetComponentInChildren<TMP_Text>();
        _mainButton.onClick.AddListener(OnMainButtonClicked);
        _degree1Button.onClick.AddListener(() => OnDegreeButtonClicked(1));
        _degree2Button.onClick.AddListener(() => OnDegreeButtonClicked(2));
        _degree3Button.onClick.AddListener(() => OnDegreeButtonClicked(3));
        _degree4Button.onClick.AddListener(() => OnDegreeButtonClicked(4));
        _degree5Button.onClick.AddListener(() => OnDegreeButtonClicked(5));
        OnReset();
    }

    private void UpdateUI()
    {
        ConfigureStageUI();
        UpdateButtonUI();
    }

    private void ConfigureStageUI()
    {
        if (_stage == 0)
        {
            _subtitle.SetText("Instructions");
            _body.SetText("For each question, choose how true it is for you in general:\r\n\r\n1 = Never\r\n2 = Rarely\r\n3 = Sometimes\r\n4 = Often\r\n5 = Always");
            if (_mainButtonText == null)
            {
                Debug.Log("[QuincyScript] main button text is null");
                _mainButtonText = _mainButton.GetComponentInChildren<TMP_Text>();
            }
            _mainButtonText.SetText("I'm Ready!");
            _selectionButtonArea.SetActive(false);
        }
        else if (_stage <= 8)
        {
            _subtitle.SetText($"Question {_stage}");
            _body.SetText((_ageGroup == AgeGroup.Preschool || _ageGroup == AgeGroup.Child)
                ? questionsForYounger[_stage - 1]
                : questionsForOlder[_stage - 1]);
            if (_stage == 1)
            {
                _selectionButtonArea.SetActive(true);
                _mainButtonText.SetText("Next");
            }
            else if (_stage == 8)
            {
                _mainButtonText.SetText("Finish");
            }
        }
        else
        {
            OnQuincyComplete();
        }
        _rewardBox.SetActive(_stage > 8);
    }

    private void OnQuincyComplete()
    {
        _mainButtonText.SetText("Exit");
        _selectionButtonArea.SetActive(false);
        CarrotManager.IncreaseCarrots(10);
        MindfulnessTier tier = MindfulnessTier.GetTier(_score);
        _subtitle.SetText(tier.Title);
        _body.SetText(tier.Description);
        QuincyManager.CompleteQuincy(GameManager.GetCurrWorld());
    }

    private void OnMainButtonClicked()
    {
        if (1 <= _stage && _stage <= 8)
        {
            if (_currentlySelectedDegree == -1)
            {
                return;
            }
            _score += _currentlySelectedDegree;
            _currentlySelectedDegree = -1;
        }
        _stage++;
        if (_stage > 9)
        {
            _onFinish?.Invoke();
            return;
        }
        UpdateUI();
    }

    private void OnDegreeButtonClicked(int degree)
    {
        _currentlySelectedDegree = degree;
        UpdateButtonUI();
    }

    public void OnReset()
    {
        _stage = 0;
        _score = 0;
        _currentlySelectedDegree = -1;
        UpdateUI();
    }

    private void UpdateButtonUI()
    {
        for (int i = 0; i < _degreeButtons.Length; i++)
        {
            Image image = _degreeButtonImages[i];
            TMP_Text text = _degreeButtonTexts[i];
            if (i + 1 == _currentlySelectedDegree)
            {
                image.sprite = _selectedButtonSprite;
                image.color = new Color32(86, 48, 26, 255);
                text.color = new Color32(252, 248, 227, 100);
            }
            else
            {
                image.sprite = _unselectedButtonSprite;
                image.color = new Color32(255, 255, 255, 255);
                text.color = new Color32(0, 0, 0, 255);
            }
        }
    }
}
