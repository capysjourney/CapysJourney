using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginScript : MonoBehaviour
{

    private Color DisabledButtonColor = new(1f, 1f, 1f, .3f);

    [SerializeField] private Button _logInTab;
    [SerializeField] private Button _signUpTab;
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private GameObject _errorMessageGO;
    [SerializeField] private AgeInputScript _ageInputScript;
    [SerializeField] private TMP_Text _emailLabel;
    [SerializeField] private TMP_InputField _emailField;
    [SerializeField] private TMP_InputField _passwordField;
    [SerializeField] private TMP_InputField _confirmPasswordField;
    [SerializeField] private GameObject _confirmPasswordBox;
    // todo - implement forgot password
    //[SerializeField] private Button _forgotPasswordButton;
    [SerializeField] private Button _passwordRevealButton;
    [SerializeField] private Button _confirmRevealButton;
    [SerializeField] private Button _logInOrSignUpButton;
    [SerializeField] private Button _continueAsGuestBtn;
    [SerializeField] private Button _appleButton;
    [SerializeField] private Button _googleButton;
    private TMP_Text _logInOrSignUpText;
    private Image _logInTabImage;
    private Image _signUpTabImage;
    private TMP_Text _errorMessage;
    private LoginInputBoxScript _emailInputBoxScript;
    private LoginInputBoxScript _passwordInputBoxScript;
    private LoginInputBoxScript _confirmPasswordInputBoxScript;

    private FirebaseAuth _auth = null;
    private bool _isLoginMode = true;
    private bool _isChild = true;

    void Start()
    {
        _auth = FirebaseAuth.DefaultInstance;
        _logInOrSignUpText = _logInOrSignUpButton.GetComponentInChildren<TMP_Text>();
        _logInTabImage = _logInTab.GetComponent<Image>();
        _signUpTabImage = _signUpTab.GetComponent<Image>();
        _errorMessage = _errorMessageGO.GetComponentInChildren<TMP_Text>();
        _emailInputBoxScript = _emailField.GetComponentInParent<LoginInputBoxScript>();
        _passwordInputBoxScript = _passwordField.GetComponentInParent<LoginInputBoxScript>();
        _confirmPasswordInputBoxScript = _confirmPasswordField.GetComponentInParent<LoginInputBoxScript>();
        ResetError();
        _emailInputBoxScript.SetOnResetErrorState(ResetError);
        _passwordInputBoxScript.SetOnResetErrorState(ResetError);
        _confirmPasswordInputBoxScript.SetOnResetErrorState(ResetError);
        _logInTab.onClick.AddListener(OnLoginTabClicked);
        _signUpTab.onClick.AddListener(OnSignUpTabClicked);
        //_forgotPasswordButton.onClick.AddListener(OnForgotPasswordClicked);
        _logInOrSignUpButton.onClick.AddListener(OnLogInOrSignUpClicked);
        _appleButton.onClick.AddListener(OnAppleButtonClicked);
        _googleButton.onClick.AddListener(OnGoogleButtonClicked);
        _passwordRevealButton.onClick.AddListener(() => OnRevealClicked(_passwordField));
        _confirmRevealButton.onClick.AddListener(() => OnRevealClicked(_confirmPasswordField));
        _ageInputScript.SetOnAgeChangedListener(OnAgeChanged);
        OnLoginTabClicked();
        _continueAsGuestBtn.onClick.AddListener(OnContinueAsGuest);
    }

    private void OnRevealClicked(TMP_InputField inputField)
    {
        bool isPassword = inputField.contentType == TMP_InputField.ContentType.Password;
        if (isPassword)
        {
            inputField.contentType = TMP_InputField.ContentType.Standard;
        }
        else
        {
            inputField.contentType = TMP_InputField.ContentType.Password;
        }
        inputField.ForceLabelUpdate();
    }

    private void ResetError()
    {
        _errorMessageGO.SetActive(false);
        _emailInputBoxScript.SetErrorState(false);
        _passwordInputBoxScript.SetErrorState(false);
        _confirmPasswordInputBoxScript.SetErrorState(false);
    }

    private void OnLoginTabClicked()
    {
        _isLoginMode = true;
        _titleText.text = "Log In";
        _confirmPasswordBox.SetActive(false);
        //_forgotPasswordButton.gameObject.SetActive(true);
        _logInOrSignUpText.text = "Log In";
        _logInTabImage.color = Color.white;
        _signUpTabImage.color = DisabledButtonColor;
        ResetTextFields();
        ResetError();
        _ageInputScript.gameObject.SetActive(false);
    }

    private void ResetTextFields()
    {
        _emailField.text = "";
        _passwordField.text = "";
        _confirmPasswordField.text = "";
    }

    private void OnSignUpTabClicked()
    {
        _isLoginMode = false;
        _titleText.text = "Sign Up";
        _confirmPasswordBox.SetActive(true);
        //_forgotPasswordButton.gameObject.SetActive(false);
        _logInOrSignUpText.text = "Sign Up";
        _logInTabImage.color = DisabledButtonColor;
        _signUpTabImage.color = Color.white;
        ResetError();
        ResetTextFields();
        OnAgeChanged(_ageInputScript.GetAge());
        _ageInputScript.gameObject.SetActive(true);
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
        if (email.Length == 0)
        {
            _errorMessage.SetText("Email cannot be empty!");
            _errorMessageGO.SetActive(true);
            _emailInputBoxScript.SetErrorState(true);
            return;
        }
        if (password.Length == 0)
        {
            _errorMessage.SetText("Password cannot be empty!");
            _errorMessageGO.SetActive(true);
            _passwordInputBoxScript.SetErrorState(true);
            return;
        }
        if (_isLoginMode)
        {
            ResetError();
            OnLogin(email, password);
        }
        else
        {
            string confirmPassword = _confirmPasswordField.text;
            if (password != confirmPassword)
            {
                Debug.Log("Passwords do not match");
                _errorMessage.SetText("Passwords do not match!");
                _errorMessageGO.SetActive(true);
                _passwordInputBoxScript.SetErrorState(true);
                _confirmPasswordInputBoxScript.SetErrorState(true);
                _passwordField.SetTextWithoutNotify("");
                _confirmPasswordField.SetTextWithoutNotify("");
                return;
            }
            ResetError();
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
        _auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                if (task.IsCanceled)
                {
                    Debug.Log("CreateUserWithEmailAndPasswordAsync was canceled.");
                }
                else
                {
                    Debug.Log("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                }
                _errorMessage.SetText("Registration failed. Please try again.");
                _errorMessageGO.SetActive(true);
                return;
            }
            _errorMessageGO.SetActive(false);
            FirebaseUser user = task.Result.User;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                user.DisplayName, user.UserId);
            GameManager.NeedParentConfirmation = _isChild;
            SceneManager.LoadSceneAsync("Onboarding");
        });
    }

    private void OnLogin(string email, string password)
    {
        _auth = FirebaseAuth.DefaultInstance;
        if (_auth == null)
        {
            Debug.Log("Auth is null");
            _errorMessage.SetText("Authentication service is unavailable. Please try again later.");
            _errorMessageGO.SetActive(true);
            return;
        }
        _auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                if (task.IsCanceled)
                {
                    Debug.Log("SignInWithEmailAndPasswordAsync was canceled.");
                }
                else
                {
                    Debug.Log("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                }
                _errorMessage.SetText("Login failed. Please check your credentials and try again.");
                _errorMessageGO.SetActive(true);
                return;
            }
            FirebaseUser user = task.Result.User;
            _errorMessageGO.SetActive(false);
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                user.DisplayName, user.UserId);
            SceneManager.LoadSceneAsync("Journey");
        });
    }

    private void OnContinueAsGuest()
    {
        // todo - guest login logic
        SceneManager.LoadSceneAsync("Onboarding");
    }

    private void OnAgeChanged(int newAge)
    {
        _isChild = newAge <= 13;
        if (_isChild)
        {
            _emailLabel.SetText("Parent's Email");
        }
        else
        {
            _emailLabel.SetText("Email");
        }
    }
    void Update()
    {
        if (_auth == null && FirebaseManager.Instance.IsFirebaseInitialized)
        {
            _auth = FirebaseAuth.DefaultInstance;
        }
    }
}
