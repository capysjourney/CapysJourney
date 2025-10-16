using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GratitudeScript : MonoBehaviour
{
    [SerializeField] private Button _writeGratitudeBtn;
    [SerializeField] private GameObject _darkener;
    [SerializeField] private GameObject _inputPaper;
    [SerializeField] private GameObject _pastPaper;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Button _doneBtn;
    [SerializeField] private Button _cancelBtn;
    [SerializeField] private TMP_Text _pastText;
    [SerializeField] private TMP_Text _dateText;
    [SerializeField] private Button _yellowBackBtn;
    [SerializeField] private Button _backBtn;
    [SerializeField] private Sprite _paper;
    [SerializeField] private PaperLeafScript _leaf1;
    [SerializeField] private PaperLeafScript _leaf2;
    [SerializeField] private PaperLeafScript _leaf3;
    [SerializeField] private PaperLeafScript _leaf4;
    [SerializeField] private PaperLeafScript _leaf5;
    [SerializeField] private PaperLeafScript _leaf6;
    [SerializeField] private PaperLeafScript _leaf7;
    [SerializeField] private PaperLeafScript _leaf8;
    [SerializeField] private PaperLeafScript _leaf9;
    [SerializeField] private PaperLeafScript _leaf10;
    [SerializeField] private GameObject _popup;
    [SerializeField] private TMP_Text _carrotCount;
    [SerializeField] private Button _returnBtn;
    private PaperLeafScript[] _leaves;
    private int _numLeavesInUse = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeLeaves();
        _darkener.SetActive(false);
        _popup.SetActive(false);
        _inputPaper.SetActive(false);
        _pastPaper.SetActive(false);
        HideLeaves();
        _inputField.text = "";
        _writeGratitudeBtn.onClick.AddListener(() =>
        {
            _darkener.SetActive(true);
            _inputPaper.SetActive(true);
            HideLeaves();
        });
        _backBtn.onClick.AddListener(ReturnToDaily);
        _returnBtn.onClick.AddListener(ReturnToDaily);
        _cancelBtn.onClick.AddListener(() =>
        {
            _darkener.SetActive(false);
            _inputPaper.SetActive(false);
            _inputField.text = "";
            ShowLeaves();
        });
        _yellowBackBtn.onClick.AddListener(() =>
        {
            _darkener.SetActive(false);
            _pastPaper.SetActive(false);
            ShowLeaves();
        });
        _darkener.GetComponent<Button>().onClick.AddListener(() =>
        {
            _darkener.SetActive(false);
            _inputPaper.SetActive(false);
            _pastPaper.SetActive(false);
            _inputField.text = "";
            ShowLeaves();
        });
        _doneBtn.onClick.AddListener(() =>
        {
            if (_inputField.text.Length == 0)
            {
                return;
            }
            LogGratitude();
        });
        SetLeaves();
    }
    private void ReturnToDaily() => SceneManager.LoadSceneAsync("Daily");

    private void SetLeaves()
    {
        LinkedList<GratitudeEntry> entries = GameManager.GetGratitudeEntries();
        int i = 0;
        _numLeavesInUse = Math.Min(entries.Count, _leaves.Length);
        foreach (GratitudeEntry entry in entries)
        {
            if (i >= _numLeavesInUse) break;
            PaperLeafScript leaf = _leaves[i];
            leaf.SetText(entry.Timestamp.ToString("MMM dd"));
            leaf.SetOnClickListener(() =>
            {
                _darkener.SetActive(true);
                _pastPaper.SetActive(true);
                _pastText.SetText(entry.Text);
                _dateText.SetText(ConvertDate(entry.Timestamp));
                HideLeaves();
            });
            leaf.Show();
            i++;
        }
    }

    private void HideLeaves()
    {
        if (_leaves == null)
        {
            InitializeLeaves();
        }
        foreach (PaperLeafScript leaf in _leaves)
        {
            leaf.Hide();
        }
    }

    private void ShowLeaves()
    {
        for (int i = 0; i < _numLeavesInUse; i++)
        {
            _leaves[i].Show();
        }
    }

    private void InitializeLeaves()
    {
        _leaves = new PaperLeafScript[] { _leaf1, _leaf2, _leaf3, _leaf4, _leaf5, _leaf6, _leaf7, _leaf8, _leaf9, _leaf10 };
    }

    private static string ConvertDate(DateTime date)
    {
        DateTime today = DateTime.Today;
        int daysAgo = (today - date.Date).Days;
        int day = date.Day;
        string suffix = day % 10 == 1 && day != 11 ? "st"
                    : day % 10 == 2 && day != 12 ? "nd"
                    : day % 10 == 3 && day != 13 ? "rd"
                    : "th";
        string monthDay = date.ToString($"MMMM {day}");
        string agoPart = daysAgo switch
        {
            0 => "(today)",
            1 => "(1 day ago)",
            _ when daysAgo > 1 => $"({daysAgo} days ago)",
            _ => ""
        };
        return $"{monthDay}{suffix} {agoPart}".Trim();
    }

    private void LogGratitude()
    {
        _darkener.SetActive(true);
        _popup.SetActive(true);
        _inputPaper.SetActive(false);
        int carrotsEarned = GameManager.LoggedGratitudeToday() ? 0 : 10;
        GameManager.LogGratitude(_inputField.text, DateTime.Now);
        _carrotCount.text = carrotsEarned.ToString();
        _numLeavesInUse = Math.Min(_numLeavesInUse + 1, _leaves.Length);
        _inputField.text = "";
    }

    // Update is called once per frame
    void Update()
    {

    }
}
