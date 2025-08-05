/*using System.Security.Cryptography;
using System.Text;

public static class TextProcessing
{
    public static string NormalizeInput(string input)
    {
        var words = input.ToLower()
            .Split(new[] { ',', ' ', '|', '-' }, System.StringSplitOptions.RemoveEmptyEntries);
        
        System.Array.Sort(words);
        return string.Join(" ", words).Trim();
    }

    public static string GenerateCacheKey(string input)
    {
        using(MD5 md5 = MD5.Create())
        {
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sb = new StringBuilder();
            foreach(byte b in hash) sb.Append(b.ToString("x2"));
            return sb.ToString() + ".png";
        }
    }
}
*/