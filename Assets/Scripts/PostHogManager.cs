using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// PostHog Manager for Unity - Handles analytics event tracking
/// </summary>
public class PostHogManager : MonoBehaviour
{
    private static PostHogManager _instance;
    public static PostHogManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("PostHogManager");
                _instance = go.AddComponent<PostHogManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    [Header("PostHog Configuration")]
    [SerializeField] private string apiKey = "phc_m1dZcID4wUPtpdi1AZNBlFTmzLZK2ScPqOjf4HgWIjl";
    [SerializeField] private string host = "https://us.i.posthog.com";
    [SerializeField] private bool enableTracking = true;

    private string _distinctId;
    private Queue<PostHogEvent> _eventQueue = new Queue<PostHogEvent>();
    private bool _isInitialized = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Initialize PostHog with a distinct user ID
    /// </summary>
    public void Initialize(string distinctId = null)
    {
        if (!enableTracking || string.IsNullOrEmpty(apiKey))
        {
            Debug.LogWarning("PostHog: Tracking disabled or API key not set");
            return;
        }

        if (string.IsNullOrEmpty(distinctId))
        {
            // Generate or retrieve a unique user ID
            _distinctId = PlayerPrefs.GetString("posthog_distinct_id", Guid.NewGuid().ToString());
            if (!PlayerPrefs.HasKey("posthog_distinct_id"))
            {
                PlayerPrefs.SetString("posthog_distinct_id", _distinctId);
                PlayerPrefs.Save();
            }
        }
        else
        {
            _distinctId = distinctId;
            PlayerPrefs.SetString("posthog_distinct_id", _distinctId);
            PlayerPrefs.Save();
        }

        _isInitialized = true;
        Debug.Log($"PostHog: Initialized with distinct_id: {_distinctId}");

        // Process any queued events
        StartCoroutine(ProcessEventQueue());
    }

    /// <summary>
    /// Track an event with optional properties
    /// </summary>
    public void Capture(string eventName, Dictionary<string, object> properties = null)
    {
        if (!enableTracking)
        {
            Debug.LogWarning($"PostHog: Tracking disabled, ignoring event '{eventName}'");
            return;
        }

        if (!_isInitialized)
        {
            Debug.LogWarning($"PostHog: Not initialized yet, queueing event '{eventName}'");
            // Queue the event if not initialized yet
            _eventQueue.Enqueue(new PostHogEvent { EventName = eventName, Properties = properties });
            return;
        }

        Debug.Log($"PostHog: Capturing event '{eventName}'");
        StartCoroutine(SendEvent(eventName, properties));
    }

    /// <summary>
    /// Identify a user with properties
    /// </summary>
    public void Identify(string distinctId, Dictionary<string, object> properties = null)
    {
        if (!enableTracking || string.IsNullOrEmpty(apiKey))
        {
            return;
        }

        _distinctId = distinctId;
        PlayerPrefs.SetString("posthog_distinct_id", _distinctId);
        PlayerPrefs.Save();

        StartCoroutine(SendIdentify(properties));
    }

    /// <summary>
    /// Set user properties
    /// </summary>
    public void SetUserProperties(Dictionary<string, object> properties)
    {
        if (!enableTracking || !_isInitialized)
        {
            return;
        }

        StartCoroutine(SendSetUserProperties(properties));
    }

    private IEnumerator ProcessEventQueue()
    {
        while (_eventQueue.Count > 0)
        {
            var queuedEvent = _eventQueue.Dequeue();
            yield return StartCoroutine(SendEvent(queuedEvent.EventName, queuedEvent.Properties));
        }
    }

    private IEnumerator SendEvent(string eventName, Dictionary<string, object> properties = null)
    {
        // Add default properties
        var eventProperties = properties ?? new Dictionary<string, object>();
        eventProperties["$lib"] = "unity";
        eventProperties["$lib_version"] = Application.version;
        eventProperties["$os"] = SystemInfo.operatingSystem;
        eventProperties["$device_type"] = Application.platform.ToString();
        eventProperties["$screen_width"] = Screen.width;
        eventProperties["$screen_height"] = Screen.height;
        eventProperties["$app_version"] = Application.version;
        eventProperties["$app_build"] = Application.buildGUID;

        // Note: $token not needed in properties when using /e/ endpoint with api_key in payload
        // Use /e/ endpoint (PostHog's standard endpoint, most reliable)
        // Format: api_key, event, distinct_id, properties (with $token inside properties)
        var payload = new Dictionary<string, object>
        {
            { "api_key", apiKey },
            { "event", eventName },
            { "distinct_id", _distinctId },
            { "properties", eventProperties }
        };

        string json = SerializeToJson(payload);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        //Debug.Log($"PostHog: Sending event '{eventName}' to {host}/e/");
        //Debug.Log($"PostHog: JSON payload: {json}");

        using (UnityWebRequest request = new UnityWebRequest($"{host}/e/", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                //Debug.Log($"PostHog: Successfully sent event '{eventName}'. Response: {request.downloadHandler.text}");
            }
            else
            {
                Debug.LogError($"PostHog: Failed to send event '{eventName}': {request.error}");
                Debug.LogError($"PostHog: Response code: {request.responseCode}");
                if (request.downloadHandler != null && !string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    Debug.LogError($"PostHog: Response body: {request.downloadHandler.text}");
                }
            }
        }
    }

    private IEnumerator SendIdentify(Dictionary<string, object> properties = null)
    {
        // PostHog identify event format
        var eventProperties = new Dictionary<string, object>
        {
            { "$token", apiKey },
            { "$distinct_id", _distinctId }
        };

        if (properties != null && properties.Count > 0)
        {
            eventProperties["$set"] = properties;
        }

        var eventData = new Dictionary<string, object>
        {
            { "event", "$identify" },
            { "distinct_id", _distinctId },
            { "type", "identify" }, // REQUIRED: identify events use "identify" type
            { "timestamp", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
            { "properties", eventProperties }
        };

        // PostHog batch API expects an array of events
        var batch = new List<Dictionary<string, object>> { eventData };

        var payload = new Dictionary<string, object>
        {
            { "api_key", apiKey },
            { "batch", batch }
        };

        string json = SerializeToJson(payload);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        Debug.Log($"PostHog: Sending identify event to {host}/batch/");
        Debug.Log($"PostHog: JSON payload: {json}");

        using (UnityWebRequest request = new UnityWebRequest($"{host}/batch/", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"PostHog: Successfully sent identify event. Response: {request.downloadHandler.text}");
            }
            else
            {
                Debug.LogError($"PostHog: Failed to identify user: {request.error}");
                Debug.LogError($"PostHog: Response code: {request.responseCode}");
                if (request.downloadHandler != null && !string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    Debug.LogError($"PostHog: Response body: {request.downloadHandler.text}");
                }
            }
        }
    }

    private IEnumerator SendSetUserProperties(Dictionary<string, object> properties)
    {
        var eventProperties = new Dictionary<string, object>
        {
            { "$token", apiKey },
            { "$set", properties }
        };

        var eventData = new Dictionary<string, object>
        {
            { "event", "$set" },
            { "distinct_id", _distinctId },
            { "type", "capture" }, // REQUIRED by PostHog batch API
            { "timestamp", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
            { "properties", eventProperties }
        };

        // PostHog batch API expects an array of events
        var batch = new List<Dictionary<string, object>> { eventData };

        var payload = new Dictionary<string, object>
        {
            { "api_key", apiKey },
            { "batch", batch }
        };

        string json = SerializeToJson(payload);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        Debug.Log($"PostHog: Sending set user properties to {host}/batch/");
        Debug.Log($"PostHog: JSON payload: {json}");

        using (UnityWebRequest request = new UnityWebRequest($"{host}/batch/", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"PostHog: Successfully set user properties. Response: {request.downloadHandler.text}");
            }
            else
            {
                Debug.LogError($"PostHog: Failed to set user properties: {request.error}");
                Debug.LogError($"PostHog: Response code: {request.responseCode}");
                if (request.downloadHandler != null && !string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    Debug.LogError($"PostHog: Response body: {request.downloadHandler.text}");
                }
            }
        }
    }

    /// <summary>
    /// Serialize a dictionary to JSON string
    /// </summary>
    private string SerializeToJson(Dictionary<string, object> dict)
    {
        var sb = new StringBuilder();
        sb.Append("{");
        bool first = true;
        foreach (var kvp in dict)
        {
            if (!first) sb.Append(",");
            first = false;
            sb.Append($"\"{EscapeJson(kvp.Key)}\":");
            sb.Append(SerializeValue(kvp.Value));
        }
        sb.Append("}");
        return sb.ToString();
    }

    private string SerializeValue(object value)
    {
        if (value == null) return "null";
        if (value is string str) return $"\"{EscapeJson(str)}\"";
        if (value is bool b) return b ? "true" : "false";
        if (value is int || value is long) return value.ToString();
        if (value is float f) return f.ToString("F", System.Globalization.CultureInfo.InvariantCulture);
        if (value is double d) return d.ToString("F", System.Globalization.CultureInfo.InvariantCulture);
        if (value is Dictionary<string, object> dict) return SerializeToJson(dict);

        // Handle lists and arrays
        if (value is System.Collections.IEnumerable enumerable && !(value is string))
        {
            var sb = new StringBuilder();
            sb.Append("[");
            bool first = true;
            foreach (var item in enumerable)
            {
                if (!first) sb.Append(",");
                first = false;
                sb.Append(SerializeValue(item));
            }
            sb.Append("]");
            return sb.ToString();
        }

        // Fallback: convert to string
        return $"\"{EscapeJson(value.ToString())}\"";
    }

    private string EscapeJson(string str)
    {
        if (string.IsNullOrEmpty(str)) return str;
        return str.Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }

    private class PostHogEvent
    {
        public string EventName;
        public Dictionary<string, object> Properties;
    }

    /// <summary>
    /// Test method to verify PostHog is working - sends a simple test event
    /// </summary>
    public void TestPostHog()
    {
        if (!_isInitialized)
        {
            Debug.LogWarning("PostHog: Not initialized, cannot send test event");
            return;
        }

        var testProperties = new Dictionary<string, object>
        {
            { "test_source", "unity_manual_test" },
            { "test_timestamp", DateTime.UtcNow.ToString() }
        };

        Debug.Log("PostHog: Sending test event 'unity_test_event'");
        Capture("unity_test_event", testProperties);
    }

    // Public getters for configuration
    public bool IsInitialized => _isInitialized;
    public string DistinctId => _distinctId;
}

