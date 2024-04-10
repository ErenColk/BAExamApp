namespace BAExamApp.MVC.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Kelimeleri başlık tipine formatlar.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>Formatlanmış string veri döndürür</returns>
    public static string TitleFormat(this string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            return char.ToUpper(name[0]) + name.Substring(1).ToLower();
        }

        return name;
    }
}
