using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Proyecto26;
using Newtonsoft.Json;


public class AuthManager : MonoBehaviour
{
    public GameObject menuForm;
    public GameObject loginForm;

    [Header("Login / Sign In")]
    public InputField SignInEmail;
    public InputField SignInPassword;

    [Header("Registr / Sign Up")]
    public InputField SignUpLogin;
    public InputField SignUpEmail;
    public InputField SignUpPassword;
    public InputField SignUpConfirmPassword;

    [Space(10)]
    public Text SignUpMessangeText;
    public Text SignInMessengeText;

    private string AUTH_KEY = "AIzaSyD1x4pKtZzfHg0FEjcW3PFw9Du6QbvJhz4";

    public static Action OnUserSignIn; // Login
    public static Action OnUserSignUp; // Register
  

    private void Start()
    {
        OnUserSignUp += SignUp;
        OnUserSignIn += SignIn;
    }

    private void OnEnable()
    {
        OnUserSignUp += SignUp;
        OnUserSignIn += SignIn;
    }

    private void OnDisable()
    {
        OnUserSignUp -= SignUp;
        OnUserSignIn -= SignIn;
    }

    private void OnDestroy()
    {
        OnUserSignUp -= SignUp;
        OnUserSignIn -= SignIn;
    }

    public void GetUserByEmail(string email)
    {
        Database.FindUserByEmail(email, GetUserByEmail);
    }

    public void GetUserByEmail(RequestException exception, ResponseHelper helper)
    {
        try
        {
            Dictionary<string, UserData> dict = JsonConvert.DeserializeObject<Dictionary<string, UserData>>(helper.Text);
            foreach(KeyValuePair<string, UserData> pair in dict)
            {
                menuForm.SetActive(true);
                loginForm.SetActive(false);

                break;
            }
        }
        catch (Exception e)
        {
            Debug.Log("User Data not Loaded!");
        }
    }


    public void SignIn()
    {
        string email = SignInEmail.text;
        string password = SignInPassword.text;

        SignIn(email, password);
    }

    public void SignIn(string email, string password)
    {
        SignInMessengeText.text = "Search Account...";
        SignInMessengeText.color = Color.black;

        string data = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";

        RestClient.Post<AuthData>($"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={AUTH_KEY}", data, SignInCallback);
    }

    public void SignInCallback(RequestException exception, ResponseHelper helper, AuthData data)
    {
        try
        {
            SignInMessengeText.text = "Account Initilised!";
            SignInMessengeText.color = Color.green;
            GetUserByEmail(data.email);
        }
        catch (Exception e)
        {
            SignInMessengeText.text = exception.Message;
            SignInMessengeText.color = Color.red;
        }
    }

    public void SignUp() 
    {
        string email = SignUpEmail.text;
        string login = SignUpLogin.text;
        string password = SignUpPassword.text;

        bool IsEmailEmpty = string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email);
        bool IsPasswordEmpty = string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password);

        SignUpMessangeText.text = "Waiting, you`r account creating...";
        SignUpMessangeText.color= Color.white;



        if (!string.IsNullOrEmpty(login))
        {
            Database.GetUserByLogin(login, GetUserByLoginCallback);
        }
    }

    private void GetUserByLoginCallback(RequestException exception, RequestHelper helper, UserData userData)
    {
        throw new NotImplementedException();
    }

    public void GetUserByLoginCallback(RequestException exception, ResponseHelper helper, UserData userData)
    {
        if (userData == null)
        {
            if (SignUpPassword.text.Length >= 6 && SignUpConfirmPassword.text.Length >= 6)
            {
                if (SignUpConfirmPassword.text == SignUpPassword.text)
                {
                    string data = "{\"email\":\"" + SignUpEmail.text + "\",\"password\":\"" + SignUpPassword.text + "\",\"returnSecureToken\":true}";
                    RestClient.Post<AuthData>($"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={AUTH_KEY}", data, SignUpCallback);
                }
                else if (SignUpConfirmPassword.text != SignUpPassword.text)
                {
                    SignUpMessangeText.text = "Password not equals!";
                    SignUpMessangeText.color = Color.red;
                }
            }
            else if (SignUpPassword.text.Length <= 6 && SignUpConfirmPassword.text.Length <= 6)
            {
                SignUpMessangeText.text = "Password not enough lenght";
                SignUpMessangeText.color = Color.red;
            }
        }
        else
        {
            SignUpMessangeText.text = "Account with you`r login have in system!";
            SignUpMessangeText.color = Color.red;
        }
    }

    public void SignUpCallback(RequestException exception, ResponseHelper helper, AuthData data)
    {
        try
        {
            string Login = SignUpLogin.text;
            string Email = SignUpEmail.text;
            string Password = SignUpPassword.text;
            string ConfirmPassword = SignUpConfirmPassword.text;

            SignUpMessangeText.text = "Account created!";
            SignUpMessangeText.color = Color.green;

            UserData userData = new UserData(Login, Email, Password, ConfirmPassword);

            Database.SendToDatabase(userData, Login);
        }
        catch(Exception e)
        {
            SignUpMessangeText.text = e.Message;
            SignUpMessangeText.color = Color.red;
        }
    }
}

[System.Serializable]
public class AuthData
{
    public string localId;
    public string email;
}
