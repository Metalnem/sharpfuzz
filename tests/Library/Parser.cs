namespace Library;

public static class Parser
{
    public static void Parse(string s)
    {
        if (s.Length > 0 && s[0] == 'C')
        if (s.Length > 1 && s[1] == 'o')
        if (s.Length > 2 && s[2] == 'o')
        if (s.Length > 3 && s[3] == 'k')
        if (s.Length > 4 && s[4] == 'i')
        if (s.Length > 5 && s[5] == 'n')
        if (s.Length > 6 && s[6] == 'g')
        if (s.Length > 7 && s[7] == ' ')
        if (s.Length > 8 && s[8] == 'M')
        if (s.Length > 9 && s[9] == 'C')
        if (s.Length > 10 && s[10] == '\'')
        if (s.Length > 11 && s[11] == 's')
        {
            throw new Exception("Like a pound of bacon");
        }
    }
}
