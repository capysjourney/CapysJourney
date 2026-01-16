using UnityEngine;

public class UISoundEffect
{
    public string _audioPath;
    private UISoundEffect(string audioPath)
    {
        _audioPath = audioPath;
    }
    public static readonly UISoundEffect AchievementUnlocked = new("UIEffects/achievementUnlocked");
    public static readonly UISoundEffect BreatheInOutIndicator = new("UIEffects/breatheInOutIndicator");
    public static readonly UISoundEffect BuyItemFromShop = new("UIEffects/buyItemFromShop");
    public static readonly UISoundEffect CompleteLevel = new("UIEffects/completeLevel");
    public static readonly UISoundEffect EquipItem = new("UIEffects/equipItem");
    public static readonly UISoundEffect InitialLevelClick = new("UIEffects/initialLevelClick");
    public static readonly UISoundEffect LevelBegin = new("UIEffects/levelBegin");
    public static readonly UISoundEffect OpenLootBox = new("UIEffects/openLootBox");
    public static readonly UISoundEffect StreakIncrease = new("UIEffects/streakIncrease");
}
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    private AudioSource _audioSource;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _audioSource = gameObject.AddComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    public void PlayUIEffect(UISoundEffect uISoundEffect)
    {
        AudioClip clip = Resources.Load<AudioClip>(uISoundEffect._audioPath);
        if (clip != null)
        {
            PlaySound(clip);
        }
        else
        {
            Debug.LogWarning($"Audio clip not found at path: {uISoundEffect._audioPath}");
        }
    }

    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        _audioSource.clip = clip;
        _audioSource.volume = volume;
        _audioSource.Play();
    }

    public void PlayMusicLoop(AudioClip clip, float volume = 1f)
    {
        _audioSource.clip = clip;
        _audioSource.volume = volume;
        _audioSource.loop = true;
        _audioSource.Play();
    }

    public void StopMusic()
    {
        if (_audioSource != null)
        {
            _audioSource.Stop();
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (_audioSource != null)
        {
            _audioSource.volume = Mathf.Clamp01(volume);
        }
    }
}

