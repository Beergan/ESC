using System;

namespace ESC.CONCOST.Abstract;

public class RspLogin
{
    public bool Success { get; set; }

    public string Message { get; set; }

    public DateTime? TokenExpired { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Avatar { get; set; }


    public RspLogin(string message)
    {
        Message = message;
    }

    public RspLogin(string token, DateTime expired, string firstName, string lastName, string avatar)
    {
        Success = true;
        Message = token;
        TokenExpired = expired;
        FirstName = firstName;
        LastName = lastName;
        Avatar = avatar;
    }
}