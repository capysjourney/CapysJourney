using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadgeNotifManager : MonoBehaviour
{
    private static BadgeNotifManager _instance;
    public static BadgeNotifManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<BadgeNotifManager>();
                if (_instance == null)
                {
                    GameObject go = new("BadgeManager");
                    _instance = go.AddComponent<BadgeNotifManager>();
                }
            }
            return _instance;
        }
    }

    [Header("UI References")]
    [SerializeField] private Transform notificationParent;

    private const float displayDuration = 4f;
    private const float fadeInDuration = 0.5f;
    private const float fadeOutDuration = 0.5f;

    private readonly Queue<Badge> _badgeQueue = new();
    private bool _isDisplayingNotification = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    public void ShowBadgeNotifications(List<BadgeEnum> badges)
    {
        foreach (var badgeEnum in badges)
        {
            Badge badge = Badge.BadgeOfEnum[badgeEnum];
            if (badge != null)
            {
                _badgeQueue.Enqueue(badge);
            }
        }

        if (!_isDisplayingNotification)
        {
            StartCoroutine(ProcessNotificationQueue());
        }
    }

    private IEnumerator ProcessNotificationQueue()
    {
        _isDisplayingNotification = true;

        while (_badgeQueue.Count > 0)
        {
            Badge currentBadge = _badgeQueue.Dequeue();
            yield return StartCoroutine(DisplayNotification(currentBadge));
        }

        _isDisplayingNotification = false;
    }

    private IEnumerator DisplayNotification(Badge badge)
    {
        GameObject notificationPrefab = Resources.Load<GameObject>("Prefabs/AchievementNotif");

        if (notificationPrefab == null)
        {
            Debug.LogWarning("BadgeManager: Could not load AchievementNotif prefab from Resources/Prefabs/AchievementNotif!");
            yield break;
        }

        if (notificationParent == null)
        {
            Debug.LogWarning("BadgeManager: Notification parent not assigned!");
            yield break;
        }

        GameObject notification = Instantiate(notificationPrefab, notificationParent);

        AudioManager.Instance.PlayUIEffect(Sound.AchievementUnlocked); 
        if (!notification.TryGetComponent<BadgeNotifScript>(out var notifScript))
        {
            Debug.LogWarning("BadgeManager: AchievementNotif prefab is missing BadgeNotifScript component!");
            Destroy(notification);
            yield break;
        }


        if (!notification.TryGetComponent<CanvasGroup>(out var canvasGroup))
        {
            canvasGroup = notification.AddComponent<CanvasGroup>();
        }

        // Set badge data
        notifScript.SetBadge(badge);
        notifScript.SetOnClose(() => Destroy(notification));

        // Fade in
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, fadeInDuration));

        // Display
        yield return new WaitForSeconds(displayDuration);

        // Fade out
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0f, fadeOutDuration));

        Destroy(notification);
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        canvasGroup.alpha = startAlpha;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
    }
}