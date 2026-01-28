using UnityEngine;

public class Sound
{
    public string AudioPath;
    private Sound(string audioPath)
    {
        AudioPath = audioPath;
    }
    public static readonly Sound AchievementUnlocked = new("UIEffects/achievementUnlocked");
    public static readonly Sound BreatheInOutIndicator = new("UIEffects/breatheInOutIndicator");
    public static readonly Sound BuyItemFromShop = new("UIEffects/buyItemFromShop");
    public static readonly Sound CompleteLevel = new("UIEffects/completeLevel");
    public static readonly Sound EquipItem = new("UIEffects/equipItem");
    public static readonly Sound InitialLevelClick = new("UIEffects/initialLevelClick");
    public static readonly Sound LevelBegin = new("UIEffects/levelBegin");
    public static readonly Sound OpenLootBox = new("UIEffects/openLootBox");
    public static readonly Sound StreakIncrease = new("UIEffects/streakIncrease");
    public static readonly Sound MainTheme = new("Theme/CJTheme");
}
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    private AudioSource _musicSource;
    public bool IsMusicPlaying => _musicSource != null && _musicSource.isPlaying;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        _musicSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayUIEffect(Sound uISoundEffect)
    {
        AudioClip clip = Resources.Load<AudioClip>(uISoundEffect.AudioPath);
        if (clip != null)
        {
            PlaySound(clip);
        }
        else
        {
            Debug.LogWarning($"Audio clip not found at path: {uISoundEffect.AudioPath}");
        }
    }

    public void PlayMusic(Sound musicSound)
    {
        AudioClip clip = Resources.Load<AudioClip>(musicSound.AudioPath);
        if (clip != null)
        {
            PlayMusicLoop(clip);
        }
        else
        {
            Debug.LogWarning($"Audio clip not found at path: {musicSound.AudioPath}");
        }
    }

    private void PlaySound(AudioClip clip, float volume = 1f)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
        Destroy(audioSource, clip.length);
    }

    private void PlayMusicLoop(AudioClip clip, float volume = 1f)
    {
        _musicSource.clip = clip;
        _musicSource.volume = volume;
        _musicSource.loop = true;
        _musicSource.Play();
    }

    public void StopMusic()
    {
        if (_musicSource != null)
        {
            _musicSource.Stop();
        }
    }

    public void PauseMusic()
    {
        if (_musicSource != null)
        {
            _musicSource.Pause();
        }
    }

    public void UnpauseMusic()
    {
        if (_musicSource != null)
        {
            _musicSource.UnPause();
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (_musicSource != null)
        {
            _musicSource.volume = Mathf.Clamp01(volume);
        }
    }
    public static string GetAudioName(Level level, AgeGroup ageGroup)
    {
        return level.GetAudioFilePathOfAgeGroup(ageGroup);
    }
}

