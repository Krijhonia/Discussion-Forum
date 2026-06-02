using System;
using System.Text.RegularExpressions;
namespace DiscussionForum.Models.Helpers;

public class RemoveHtmlTagHelper
{
    public static string RemoveHtmlTags(string input )
    {
        return Regex.Replace(input, "<.*?>|&.*?;", string.Empty); 
    }
}

