[System.Serializable]
public class UserData
{
    public string Login;
    public string Email;
    public string Password;
    public string ConfirmPAssword;

    public UserData(string login, string email, string password, string confirmPassword) 
    {
        this.Login = login;
        this.Email = email;
        this.Password = password;
        this.ConfirmPAssword = confirmPassword;
    }
}
