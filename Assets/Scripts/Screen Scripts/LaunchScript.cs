using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LaunchScript : MonoBehaviour
{
    [SerializeField] private GameObject _loadingCircle;
    [SerializeField] private TMP_Text _tipText;
    private const float LoadingLength = 3f;
    private const float RotationSpeed = -90;
    private const int MaxYoungAge = 14;
    private readonly string[] _youngUserTips = new string[]
    {
        "Can you hear one sound right now? Just listen.",
        "Try to follow your next breath all the way in and out.",
        "When your mind runs, your breath can guide you back.",
        "Your thoughts are like bubbles-they float away.",
        "Close your eyes for a moment. What do you feel?",
        "Take a breath. Feel it come and go.",
        "Right now is a good place to be.",
        "Listen for the quietest sound you can hear.",
        "Notice how your body feels right now.",
        "Stillness is a kind of strength",
        "Let your body be still, like a tree.",
        "Even one breath can help you reset."
    };
    private readonly string[] _oldUserTips = new string[]
    {
        "Be the sky, not the storm.",
        "Thoughts pass. Awareness stays.",
        "Notice what's here. before the mind explains it.",
        "You don't have to chase calm. It finds you when you stop running.",
        "The breath doesn't try. It just is.",
        "Sit back. Watch the show inside your head without getting pulled in.",
        "Let this moment be your whole world-just for now.",
        "Every breath is a tiny homecoming.",
        "The mind wants to run. The heart wants to rest.",
        "Beneath the noise, you're still here.",
        "See the thought. Smile at it. Let it go.",
        "Even silence has something to say.",
        "Breathe in... breathe out. That's enough for now.",
        "You are not your thoughts. Just the one noticing them.",
        "Let the moment be exactly as it is.",
        "Wandering mind? Gently return to now.",
        "Feel your feet on the ground. You're here.",
        "Thoughts come and go like clouds. Let them.",
        "A deep breath can change everything.",
        "No need to fix or force. Just notice.",
        "Even one mindful moment is a win.",
        "Today, try to listen like it's the first time.",
        "Behind the chatter of the mind, awareness is always still.",
        "Be the witness, not the weather.",
        "There's no such thing as a \"perfect\" meditation. Just presence.",
        "You don't need to control thoughts-just stop feeding them."
    };

    void Start()
    {
        int age = PlayerPrefs.GetInt("age", 0);
        if (age <= MaxYoungAge)
        {
            _tipText.SetText(RandomElem(_youngUserTips));
        }
        else
        {
            _tipText.SetText(RandomElem(_oldUserTips));
        }
        StartCoroutine(WaitThenChangeScreen());
    }

    private string RandomElem(string[] arr)
    {
        return arr[new System.Random().Next(0, arr.Length - 1)];
    }

    void Update()
    {
        _loadingCircle.transform.Rotate(0, 0, RotationSpeed * Time.deltaTime);
    }

    IEnumerator WaitThenChangeScreen()
    {
        yield return new WaitForSeconds(LoadingLength);
        if (!PlayerPrefs.HasKey("username"))
        {
            SceneManager.LoadSceneAsync("Onboarding");
        }
        else
        {
            // todo uncomment
            SceneManager.LoadSceneAsync("Journey");
        }
    }
}
