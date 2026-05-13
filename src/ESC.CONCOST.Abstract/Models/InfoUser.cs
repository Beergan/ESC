using System;
using System.Collections.Generic;

namespace ESC.CONCOST.Abstract;

public class InfoUser
{
    public Guid? Guid { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string UserName { get; set; }

    public bool IsAuthenticated { get; set; }

    public List<InfoUserClaim> Claims { get; set; }

    public string Avatar { get; set; }

    public string EnterpriseCode { get; set; }

    public string EnterpriseName { get; set; }
}

public class InfoUserClaim
{
    public InfoUserClaim(string key, string value)
    {
        Key = key;
        Value = value;
    }
    public string Key { get; set; }
    public string Value { get; set; }
}