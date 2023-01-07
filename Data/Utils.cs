using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Taraweb.Data
{
	public static class Utils
	{
		
        public static string GenerateSlug(this string phrase)
        {
            string str = phrase.RemoveAccent().ToLower();
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens   
            return str;
        }

        public static string RemoveAccent(this string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        public static string CreateSlug(string input)
        {
            // Replace all non-alphanumeric characters with hyphens
            input = input.Normalize(NormalizationForm.FormD);

            // Replace all non-alphanumeric characters with hyphens
            string slug = Regex.Replace(input, @"[^\p{L}\p{Nd}]+", "-");

            // Convert the string to lowercase
            slug = slug.ToLower();

            // Remove all leading and trailing hyphens
            slug = slug.Trim('-');

            // Replace all runs of multiple hyphens with a single hyphen
            slug = Regex.Replace(slug, @"-+", "-");

            return slug;
        }

    }
}

