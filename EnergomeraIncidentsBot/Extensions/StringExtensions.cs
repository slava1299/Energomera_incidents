using System.Text.RegularExpressions;

namespace EnergomeraIncidentsBot.Extensions;

public static class StringExtensions
{
    public static string ExtractTextFromHTML(this string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }
    
        string plainText = Regex.Replace(str, "<[^>]+?>", " ");
        plainText = System.Net.WebUtility.HtmlDecode(plainText).Trim();

        return plainText;
    }
    
    /// <summary>
    /// Метод для перевода строки в параметр запроса для SQL сервера.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ToSqlParam(this string str)
    {
        return str.Replace("\'", "\'\'");
    }
}