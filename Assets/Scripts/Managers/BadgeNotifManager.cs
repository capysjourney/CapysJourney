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
    [SerializeField] private Transform notificationParent;

    private const float displayDuration = 3.5f;
    private const float fadeInDuration = 0.5f;
    private const float fadeOutDuration = 0.5f;

    private static readonly Queue<BadgeInfo> BadgeQueue = new();
    private static bool IsDisplayingNotification = false;

    /// <summary>
    /// Whether the scene was destroyed while displaying a notification
    /// </summary>
    private static bool WasInterrupted = false;

    /// <summary>
    /// If WasInterrupted is true, how much time was left in the display duration
    /// </summary>
    private static float RemainingDuration = displayDuration;

    private void Awake()
    {
        _instance = this;
    }

    private void OnEnable()
    {
        // Resume processing if queue has items after scene load
        if (BadgeQueue.Count > 0 && !IsDisplayingNotification)
        {
            StartCoroutine(ProcessNotificationQueue(startFromMiddle: WasInterrupted, remainingDuration: RemainingDuration));
        }
    }

    private void OnDestroy()
    {
        if (IsDisplayingNotification) WasInterrupted = true;
        IsDisplayingNotification = false;
    }

    public void ShowBadgeNotifications(List<Badge> badges)
    {
        foreach (var badge in badges)
        {
            BadgeInfo badgeInfo = badge.GetInfo();
            if (badgeInfo != null)
            {
                BadgeQueue.Enqueue(badgeInfo);
            }
        }
        if (!IsDisplayingNotification)
        {
            StartCoroutine(ProcessNotificationQueue(startFromMiddle: false));
        }
    }

    private IEnumerator ProcessNotificationQueue(bool startFromMiddle, float remainingDuration = displayDuration)
    {
        IsDisplayingNotification = true;

        while (BadgeQueue.Count > 0)
        {
            BadgeInfo currentBadge = BadgeQueue.Peek();
            yield return StartCoroutine(DisplayNotification(currentBadge, startFromMiddle, remainingDuration));
            BadgeQueue.Dequeue();
        }

        IsDisplayingNotification = false;
    }

    /// <summary>
    /// Displays a notification for given badge. If startFromMiddle is true, skips the fade-in animation and play for duration seconds.
    /// Otherwise, plays the full animation from fade-in to fade-out.
    /// 
    private IEnumerator DisplayNotification(BadgeInfo badge, bool startFromMiddle, float duration)
    {
        GameObject notificationPrefab = Resources.Load<GameObject>("Prefabs/AchievementNotif");

        if (notificationPrefab == null)
        {
            Debug.LogWarning("BadgeManager: Could not load AchievementNotif prefab from Resources/Prefabs/AchievementNotif!");
            yield break;
        }

        if (notificationParent == null)
        {
            Debug.LogWarning("BadgeManager: Could not find valid notification parent in current scene!");
            yield break;
        }

        GameObject notification = Instantiate(notificationPrefab, notificationParent);
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
        notifScript.SetOnClose(() => {
            BadgeQueue.Dequeue();
            IsDisplayingNotification = false;
            StopAllCoroutines();
            notification.SetActive(false);
        });
        if (!startFromMiddle)
        {
            AudioManager.Instance.PlayUIEffect(Sound.AchievementUnlocked);
            // Fade in
            yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, fadeInDuration));
        }
        // Display
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            RemainingDuration = duration - elapsed;
            yield return null;
        }
        RemainingDuration = 0f;

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