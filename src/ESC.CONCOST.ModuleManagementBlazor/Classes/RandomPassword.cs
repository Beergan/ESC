using System;
using System.Linq;

namespace ESC.CONCOST.ModuleManagementBlazor;

public static class RandomPassword
{
    private static string lowercase = "abcdefghijklmnopqrstuvwxyz";
    private static string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static string digits = "0123456789";
    private static string special = "@&*^#";
    private static string allChars = lowercase + uppercase + digits + special;

    public static string GeneratePassword()
    {
        Random rand = new Random();
        char[] password = new char[15];

        password[0] = lowercase[rand.Next(lowercase.Length)];
        password[1] = uppercase[rand.Next(uppercase.Length)];
        password[2] = digits[rand.Next(digits.Length)];
        password[3] = special[rand.Next(special.Length)];

        for (int i = 4; i < 15; i++)
        {
            password[i] = allChars[rand.Next(allChars.Length)];
        }

        return new string(password.OrderBy(c => rand.Next()).ToArray());
    }
}
