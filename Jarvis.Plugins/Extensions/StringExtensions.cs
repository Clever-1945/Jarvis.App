namespace Jarvis.Plugins.Extensions;

public static class StringExtensions
{
    private static Dictionary<char, HashSet<char>> charKeyboard = GetCharKeyboardInternal();

    private static Dictionary<char, HashSet<char>> GetCharKeyboardInternal()
    {
        var dictionary = new Dictionary<char, HashSet<char>>();
        dictionary['q'] = new HashSet<char>(new char[] { 'й' });
        dictionary['w'] = new HashSet<char>(new char[] { 'ц' });
        dictionary['e'] = new HashSet<char>(new char[] { 'у' });
        dictionary['r'] = new HashSet<char>(new char[] { 'к' });
        dictionary['t'] = new HashSet<char>(new char[] { 'е' });
        dictionary['y'] = new HashSet<char>(new char[] { 'н' });
        dictionary['u'] = new HashSet<char>(new char[] { 'г' });
        dictionary['i'] = new HashSet<char>(new char[] { 'ш' });
        dictionary['o'] = new HashSet<char>(new char[] { 'щ' });
        dictionary['p'] = new HashSet<char>(new char[] { 'з' });
        
        dictionary['['] = new HashSet<char>(new char[] { 'х' });
        dictionary[']'] = new HashSet<char>(new char[] { 'ъ' });
        dictionary['a'] = new HashSet<char>(new char[] { 'ф' });
        dictionary['s'] = new HashSet<char>(new char[] { 'ы' });
        dictionary['d'] = new HashSet<char>(new char[] { 'в' });
        dictionary['f'] = new HashSet<char>(new char[] { 'а' });
        dictionary['g'] = new HashSet<char>(new char[] { 'п' });
        dictionary['h'] = new HashSet<char>(new char[] { 'р' });
        dictionary['j'] = new HashSet<char>(new char[] { 'о' });
        dictionary['k'] = new HashSet<char>(new char[] { 'л' });
        
        dictionary['l'] = new HashSet<char>(new char[] { 'д' });
        dictionary[';'] = new HashSet<char>(new char[] { 'ж' });
        dictionary['\''] = new HashSet<char>(new char[] { 'э' });
        dictionary['z'] = new HashSet<char>(new char[] { 'я' });
        dictionary['x'] = new HashSet<char>(new char[] { 'ч' });
        dictionary['c'] = new HashSet<char>(new char[] { 'с' });
        dictionary['v'] = new HashSet<char>(new char[] { 'м' });
        dictionary['b'] = new HashSet<char>(new char[] { 'и' });
        dictionary['n'] = new HashSet<char>(new char[] { 'т' });
        dictionary['m'] = new HashSet<char>(new char[] { 'ь' });
        
        dictionary[','] = new HashSet<char>(new char[] { 'б' });
        dictionary['.'] = new HashSet<char>(new char[] { 'ю' });
        dictionary['`'] = new HashSet<char>(new char[] { 'ё' });

        return dictionary;
    }

    /// <summary>
    /// Проверить равенство строк с учетом разных раскладок клавиатуры
    /// </summary>
    /// <param name="leftString"></param>
    /// <param name="rightString"></param>
    /// <returns></returns>
    public static bool IsEqualsKeyboard(this string leftString, string rightString)
    {
        if (String.IsNullOrEmpty(leftString) && String.IsNullOrEmpty(rightString))
            return true;

        var left = leftString.Trim().ToCharArray();
        var right = rightString.Trim();
        if (right.Length != left.Length)
            return false;

        for (int i = 0; i < left.Length; i++)
        {
            var ch = char.ToLower(left[i]);
            if (!charKeyboard.ContainsKey(ch))
            {
                foreach (var keyValue in charKeyboard)
                {
                    if (keyValue.Value.Contains(ch))
                    {
                        left[i] = keyValue.Key;
                        break;
                    }
                }
            }
        }

        return String.Equals(new string(left), right, StringComparison.OrdinalIgnoreCase);
    }
}