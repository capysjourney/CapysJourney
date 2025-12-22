using Firebase.Auth;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginScript : MonoBehaviour
{
    [SerializeField] private TMP_InputField _registerEmailField;
    [SerializeField] private TMP_InputField _registerPasswordField;
    [SerializeField] private TMP_InputField _loginEmailField;
    [SerializeField] private TMP_InputField _loginPasswordField;

    [SerializeField] private Button _registerBtn;
    [SerializeField] private Button _loginBtn;
    [SerializeField] private Button _continueAsGuestBtn;
    private FirebaseAuth _auth = null;
    private bool _toOnboarding = false;
    private bool _toJourney = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (FirebaseManager.Instance.IsFirebaseInitialized)
        {
            _auth = FirebaseAuth.DefaultInstance;
        }
        _registerBtn.onClick.AddListener(() => OnRegister(_registerEmailField.text, _registerPasswordField.text));
        _loginBtn.onClick.AddListener(() => OnLogin(_loginEmailField.text, _loginPasswordField.text));
        _continueAsGuestBtn.onClick.AddListener(() => OnContinueAsGuest());
    }

    private void OnRegister(string email, string password)
    {
        if(_auth == null)
        _auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }
            // Firebase user has been created.
            FirebaseUser user = task.Result.User;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                user.DisplayName, user.UserId);
            _toOnboarding = true;
        });
    }

    private void OnLogin(string email, string password)
    {
        _auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }
            FirebaseUser user = task.Result.User;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                user.DisplayName, user.UserId);
            _toJourney = true;
        });
    }

    private void OnContinueAsGuest()
    {
        // todo - guest login logic
        _toOnboarding = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_toOnboarding)
        {
            SceneManager.LoadSceneAsync("Onboarding");
            _toOnboarding = false;
        }
        else if (_toJourney)
        {
            SceneManager.LoadSceneAsync("Journey");
            _toJourney = false;
        }
        if(_auth==null && FirebaseManager.Instance.IsFirebaseInitialized)
        {
            _auth = FirebaseAuth.DefaultInstance;
        }
    }
}
