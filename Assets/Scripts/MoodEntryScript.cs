using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoodEntryScript : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private Image _capy;
    [SerializeField] private TMP_Text _moodText;
    [SerializeField] private TMP_Text _dateText;
    [SerializeField] private Sprite _awfulOutlined;
    [SerializeField] private Sprite _badOutlined;
    [SerializeField] private Sprite _mehOutlined;
    [SerializeField] private Sprite _goodOutlined;
    [SerializeField] private Sprite _superOutlined;

    public void SetEntry(MoodEntry entry)
    {
        _moodText.text = entry.MoodLevel.ToString();
        _dateText.text = FormatDate(entry.Timestamp);
        _capy.sprite = entry.MoodLevel switch
        {
            Mood.Super => _superOutlined,
            Mood.Good => _goodOutlined,
            Mood.Meh => _mehOutlined,
            Mood.Bad => _badOutlined,
            Mood.Awful => _awfulOutlined,
            _ => throw new Exception("Invalid mood")
        };

        _background.color = entry.MoodLevel switch
        {
            Mood.Super => new Color32(74, 175, 87, 255),
            Mood.Good => new Color32(139, 194, 85, 255),
            Mood.Meh => new Color32(255, 192, 45, 255),
            Mood.Bad => new Color32(255, 152, 31, 255),
            Mood.Awful => new Color32(245, 67, 54, 255),
            _ => new Color32(255, 192, 45, 255),
        };
    }

    private static string FormatDate(DateTime date)
    {
        DateTime today = DateTime.Now.Date;
        DateTime yesterday = today.AddDays(-1);
        string timePart = date.ToString("h:mm tt");

        if (date.Date == today)
        {
            return $"Today • {timePart}";
        }
        if (date.Date == yesterday)
        {
            return $"Yesterday • {timePart}";
        }
        return date.ToString("MM/dd/yyyy • h:mm tt");
    }
}
