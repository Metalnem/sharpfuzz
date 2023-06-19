namespace Library;

public static class Parser
{
    public static void Parse(string s)
    {
        if (s.Length > 0 && s[0] == 'W')
        if (s.Length > 1 && s[1] == 'h')
        if (s.Length > 2 && s[2] == 'o')
        if (s.Length > 3 && s[3] == 'o')
        if (s.Length > 4 && s[4] == 'p')
        if (s.Length > 5 && s[5] == 's')
        if (s.Length > 6 && s[6] == 'i')
        if (s.Length > 7 && s[7] == 'e')
        {
            Environment.FailFast("Everything is on fire");
        }
    }
}
