using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class TextUtilities
{
    public static string RemoveSpecialCharacters(this string str)
    {
        StringBuilder sb = new StringBuilder();
        foreach (char c in str)
        {
            if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
            {
                sb.Append(c);
            }
            else if (c == ' ')
            {
                sb.Append('_');
            }
        }
        string output = sb.ToString();
        if (output.Equals(""))
            output = "unnamed";
        return output;
    }
}
