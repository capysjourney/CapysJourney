using Firebase.Auth;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginScript : MonoBehaviour
{

    private Color DisabledButtonColor = new(1f, 1f, 1f, .3f);

    //[SerializeField] private Button _continueAsGuestBtn;
    [SerializeField] private Button _logInTab;
    [SerializeField] private Button _signUpTab;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_InputField _emailField;
    [SerializeField] private TMP_InputField _passwordField;
    [SerializeField] private TMP_InputField _confirmPasswordField;
    [SerializeField] private GameObject _confirmPasswordBox;
    [SerializeField] private Button _forgotPasswordButton;
    [SerializeField] private Button _logInOrSignUpButton;
    [SerializeField] private Button _appleButton;
    [SerializeField] private Button _googleButton;
    private TMP_Text _logInOrSignUpText;
    private Image _logInTabImage;
    private Image _signUpTabImage;

    private FirebaseAuth _auth = null;
    private bool _isLoginMode = true;
    private bool _toOnboarding = false;
    private bool _toJourney = false;

    void Start()
    {
        _auth = FirebaseAuth.DefaultInstance;
        _logInOrSignUpText = _logInOrSignUpButton.GetComponentInChildren<TMP_Text>();
        _logInTabImage = _logInTab.GetComponent<Image>();
        _signUpTabImage = _signUpTab.GetComponent<Image>();
        _logInTab.onClick.AddListener(OnLoginTabClicked);
        _signUpTab.onClick.AddListener(OnSignUpTabClicked);
        _forgotPasswordButton.onClick.AddListener(OnForgotPasswordClicked);
        _logInOrSignUpButton.onClick.AddListener(OnLogInOrSignUpClicked);
        _appleButton.onClick.AddListener(OnAppleButtonClicked);
        _googleButton.onClick.AddListener(OnGoogleButtonClicked);
        OnLoginTabClicked();
        //_continueAsGuestBtn.onClick.AddListener(() => OnContinueAsGuest());
    }

    private void OnLoginTabClicked()
    {
        _isLoginMode = true;
        _titleText.text = "Log In";
        _confirmPasswordBox.SetActive(false);
        _forgotPasswordButton.gameObject.SetActive(true);
        _logInOrSignUpText.text = "Log In";
        _logInTabImage.color = Color.white;
        _signUpTabImage.color = DisabledButtonColor;
    }

    private void OnSignUpTabClicked()
    {
        _isLoginMode = false;
        _titleText.text = "Sign Up";
        _confirmPasswordBox.SetActive(true);
        _forgotPasswordButton.gameObject.SetActive(false);
        _logInOrSignUpText.text = "Sign Up";
        _logInTabImage.color = DisabledButtonColor;
        _signUpTabImage.color = Color.white;
    }

    private void OnForgotPasswordClicked()
    {
        // TODO - Implement forgot password logic here
        Debug.Log("Forgot Password Clicked");
    }

    private void OnLogInOrSignUpClicked()
    {
        string email = _emailField.text;
        string password = _passwordField.text;
        Debug.Log("Email: " + email + " Password: " + password);
        if (_isLoginMode)
        {
            Debug.Log("login mode");
            OnLogin(email, password);
        }
        else
        {
            string confirmPassword = _confirmPasswordField.text;
            if (password != confirmPassword)
            {
                Debug.LogError("Passwords do not match!");
                //todo - unmatched passwords UI feedback
                return;
            }
            Debug.Log("onregister called");
            OnRegister(email, password);
        }
    }

    private void OnAppleButtonClicked()
    {
        // TODO - Implement Apple sign-in logic here
        Debug.Log("Apple Sign-In Clicked");
    }

    private void OnGoogleButtonClicked()
    {
        // TODO - Implement Google sign-in logic here
        Debug.Log("Google Sign-In Clicked");
    }

    private void OnRegister(string email, string password)
    {
        _auth = FirebaseAuth.DefaultInstance;
        if (_auth == null)
        {
            Debug.Log("Auth is null");
            return;
        }
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
            FirebaseUser user = task.Result.User;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                user.DisplayName, user.UserId);
            _toOnboarding = true;
        });
    }

    private void OnLogin(string email, string password)
    {
        _auth = FirebaseAuth.DefaultInstance;
        if (_auth == null)
        {
            Debug.Log("Auth is null");
            return;
        }
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
        if (_auth == null && FirebaseManager.Instance.IsFirebaseInitialized)
        {
            _auth = FirebaseAuth.DefaultInstance;
        }
    }
}
