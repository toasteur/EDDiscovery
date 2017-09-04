﻿/*
 * Copyright © 2016 - 2017 EDDiscovery development team
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this
 * file except in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software distributed under
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
 * ANY KIND, either express or implied. See the License for the specific language
 * governing permissions and limitations under the License.
 * 
 * EDDiscovery is not affiliated with Frontier Developments plc.
 */

using System;
using System.Collections.Generic;
using System.Linq;

public static class ObjectExtensionsStrings
{
    static public bool HasChars(this string obj)
    {
        return obj != null && obj.Length > 0;
    }
    static public bool IsEmpty(this string obj)
    {
        return obj == null || obj.Length == 0;
    }

    public static string ToNullSafeString(this object obj)
    {
        return (obj ?? string.Empty).ToString();
    }

    public static string ToNANSafeString(this double obj, string format)
    {
        return (obj != double.NaN) ? obj.ToString(format) : string.Empty;
    }

    public static string ToNANNullSafeString(this double? obj, string format)
    {
        return (obj.HasValue && obj != double.NaN) ? obj.Value.ToString(format) : string.Empty;
    }

    public static string Left(this string obj, int length)      // obj = null, return "".  Length can be > string
    {
        if (obj != null)
        {
            if (length < obj.Length)
                return obj.Substring(0, length);
            else
                return obj;
        }
        else
            return string.Empty;
    }

    public static string Mid(this string obj, int start, int length)      // obj = null, return "".  Mid, start/length can be out of limits
    {
        if (obj != null)
        {
            if (start < obj.Length)        // if in range
            {
                int left = obj.Length - start;      // what is left..
                return obj.Substring(start, Math.Min(left, length));    // min of left, length
            }
        }

        return string.Empty;
    }

    public static string Alt(this string obj, string alt)
    {
        return (obj == null || obj.Length == 0) ? alt : obj;
    }

    public static string ToNullUnknownString(this object obj)
    {
        if (obj == null)
            return string.Empty;
        else
        {
            string str = obj.ToString();
            return str.Equals("Unknown") ? "" : str.Replace("_", " ");
        }
    }

    public static string QuoteString(this string obj, bool comma = false, bool bracket = false)
    {
        if (obj.Length == 0 || obj.Contains("\"") || obj.Contains(" ") || (bracket && obj.Contains(")")) || (comma && obj.Contains(",")))
            obj = "\"" + obj.Replace("\"", "\\\"") + "\"";

        return obj;
    }

    public static string Skip(this string s, string t, StringComparison c = StringComparison.InvariantCulture)
    {
        if (s.StartsWith(t, c))
            s = s.Substring(t.Length);
        return s;
    }

    public static string SkipIf(this string s, string t, bool cond, StringComparison c = StringComparison.InvariantCulture)
    {
        if (cond && s.StartsWith(t, c))
            s = s.Substring(t.Length);
        return s;
    }

    public static string QuoteStrings(this string[] obja)
    {
        string res = "";
        foreach (string obj in obja)
        {
            if (res.Length > 0)
                res += ",";

            res += "\"" + obj.Replace("\"", "\\\"") + "\"";
        }

        return res;
    }

    public static string EscapeControlChars(this string obj)
    {
        string s = obj.Replace(@"\", @"\\");        // order vital
        s = obj.Replace("\r", @"\r");
        return s.Replace("\n", @"\n");
    }

    public static string ReplaceEscapeControlChars(this string obj)
    {
        string s = obj.Replace(@"\n", "\n");
        s = s.Replace(@"\r", "\r");
        return s.Replace(@"\\", "\\");
    }

    public static void AppendPrePad(this System.Text.StringBuilder sb, string other, string prepad = " ")
    {
        if (other != null && other.Length > 0)
        {
            if (sb.Length > 0)
                sb.Append(prepad);
            sb.Append(other);
        }
    }

    public static bool AppendPrePad(this System.Text.StringBuilder sb, string other, string prefix, string prepad = " ")
    {
        if (other != null && other.Length > 0)
        {
            if (sb.Length > 0)
                sb.Append(prepad);
            if (prefix.Length > 0)
                sb.Append(prefix);
            sb.Append(other);
            return true;
        }
        else
            return false;
    }

    public static string AppendPrePad(this string sb, string other, string prepad = " ")
    {
        if (other != null && other.Length > 0)
        {
            if (sb.Length > 0)
                sb += prepad;
            sb += other;
        }
        return sb;
    }

    public static string Replace(this string str, string oldValue, string newValue, StringComparison comparison)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder(str.Length * 4);

        int previousIndex = 0;
        int index = str.IndexOf(oldValue, comparison);
        while (index != -1)
        {
            sb.Append(str.Substring(previousIndex, index - previousIndex));
            sb.Append(newValue);
            index += oldValue.Length;

            previousIndex = index;
            index = str.IndexOf(oldValue, index, comparison);
        }
        sb.Append(str.Substring(previousIndex));

        return sb.ToString();
    }

    public static int FirstCharNonWhiteSpace(this string obj)
    {
        int i = 0;
        while (i < obj.Length && char.IsWhiteSpace(obj[i]))
            i++;
        return i;
    }

    public static string PickOneOf(this string str, char separ, System.Random rx)   // pick one of x;y;z or if ;x;y;z, pick x and one of y or z
    {
        string[] a = str.Split(separ);

        if (a.Length >= 2)          // x;y
        {
            if (a[0].Length == 0)      // ;y      
            {
                string res = a[1];
                if (a.Length > 2)   // ;y;x;z
                    res += " " + a[2 + rx.Next(a.Length - 2)];

                return res;
            }
            else
                return a[rx.Next(a.Length)];
        }
        else
            return a[0];
    }

    public static string PickOneOfGroups(this string exp, System.Random rx) // pick one of x;y;z or if ;x;y;z, pick x and one of y or z, include {x;y;z}
    {
        string res = "";
        exp = exp.Trim();

        while (exp.Length > 0)
        {
            if (exp[0] == '{')
            {
                int end = exp.IndexOf('}');

                if (end == -1)              // missing end bit, assume the lot..
                    end = exp.Length;

                string pl = exp.Substring(1, end - 1);

                exp = (end < exp.Length) ? exp.Substring(end + 1) : "";

                res += pl.PickOneOf(';', rx);
            }
            else
            {
                res += exp.PickOneOf(';', rx);
                break;
            }
        }

        return res;
    }


    public static string AddSuffixToFilename(this string file, string suffix)
    {
        return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(file), System.IO.Path.GetFileNameWithoutExtension(file) + suffix) + System.IO.Path.GetExtension(file);
    }

    public static string ToStringCommaList(this System.Collections.Generic.List<string> list, int mincount = 100000, bool escapectrl = false)
    {
        string r = "";
        for (int i = 0; i < list.Count; i++)
        {
            if (i >= mincount && list[i].Length == 0)           // if >= minimum, and zero
            {
                int j = i + 1;
                while (j < list.Count && list[j].Length == 0)   // if all others are zero
                    j++;

                if (j == list.Count)        // if so, stop
                    break;
            }

            if (i > 0)
                r += ", ";

            if (escapectrl)
                r += list[i].EscapeControlChars().QuoteString(comma: true);
            else
                r += list[i].QuoteString(comma: true);
        }

        return r;
    }

    public static string ToString(this int[] a, string separ)
    {
        string outstr = "";
        if (a.Length > 0)
        {
            outstr = a[0].ToString(System.Globalization.CultureInfo.InvariantCulture);

            for (int i = 1; i < a.Length; i++)
                outstr += separ + a[i].ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        return outstr;
    }

    public static string ToStringInvariant(this int v)
    {
        return v.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }
    public static string ToStringInvariant(this long v)
    {
        return v.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }
    public static string ToStringInvariant(this bool? v)
    {
        return (v.HasValue) ? (v.Value ? "1" : "0") : "";
    }
    public static string ToStringInvariant(this double v, string format)
    {
        return v.ToString(format, System.Globalization.CultureInfo.InvariantCulture);
    }
    public static string ToStringInvariant(this double v)
    {
        return v.ToString(System.Globalization.CultureInfo.InvariantCulture);
    }
    public static string ToStringInvariant(this double? v, string format)
    {
        return (v.HasValue) ? v.Value.ToString(format) : "";
    }
    public static string ToStringInvariant(this int? v)
    {
        return (v.HasValue) ? v.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : "";
    }
    public static string ToStringInvariant(this long? v)
    {
        return (v.HasValue) ? v.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : "";
    }

    // fix word_word to Word Word
    //  s = Regex.Replace(s, @"([A-Za-z]+)([_])([A-Za-z]+)", m => { return m.Groups[1].Value.FixTitleCase() + " " + m.Groups[3].Value.FixTitleCase(); });
    // fix _word to spc Word
    //  s = Regex.Replace(s, @"([_])([A-Za-z]+)", m => { return " " + m.Groups[2].Value.FixTitleCase(); });
    // fix zeros
    //  s = Regex.Replace(s, @"([A-Za-z]+)([0-9])", "$1 $2");       // Any ascii followed by number, split
    //  s = Regex.Replace(s, @"(^0)(0+)", "");     // any 000 at start of line, remove
    //  s = Regex.Replace(s, @"( 0)(0+)", " ");     // any space 000 in middle of line, remove
    //  s = Regex.Replace(s, @"(0)([0-9]+)", "$2");   // any 0Ns left, remove 0

    enum State { space, alpha, nonalpha, digits0, digits };
    static public string SplitCapsWordFull(this string capslower, Dictionary<string, string> namerep = null)     // fixes numbers, does replacement of alpha sequences
    {
        if (capslower == null || capslower.Length == 0)
            return "";

        string s = SplitCapsWord(capslower);

        System.Text.StringBuilder sb = new System.Text.StringBuilder(256);

        State state = State.space;

        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];

            if (c == '0') // 00..
            {
                if (state == State.digits)      // if in digits, print
                    sb.Append(c);
                else if ( state != State.digits0 )  // digits0, we just ignore, otherwise we jump into it
                {
                    if (state != State.space)  // if not in space, space it out, but don't print it
                        sb.Append(' ');
                    state = State.digits0;     // digits 0.
                }
            }
            else if (c >= '1' && c <= '9')  // numbers
            {
                if (state == State.digits)
                    sb.Append(c);                   // in digits, so print it, as we have removed 0 front stuff.
                else
                {
                    if (state != State.space && state != State.digits0)
                        sb.Append(' ');     // so, we space out if came from not these two states

                    state = State.digits;           // else jump into digits, and append
                    sb.Append(c);
                }
            }
            else
            {
                if (state == State.digits0)        // left in digit 0, therefore a run of 0's, so don't lose it (since they are not inserted)
                    sb.Append('0');

                if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))   // if now alpha
                {
                    if (state == State.alpha)
                        sb.Append(c);
                    else
                    {
                        if (state != State.space)
                            sb.Append(' ');

                        state = State.alpha;

                        if (namerep != null)           // at alpha start, see if we have any global subs of alpha numerics
                        {
                            int j = i + 1;
                            for (; j < s.Length && ((s[j] >= 'A' && s[j] <= 'Z') || (s[j] >= 'a' && s[j] <= 'z') || (s[j] >= '0' && s[j] <= '9')); j++)
                                ;

                            string keyname = s.Substring(i, j - i);

                            //                        string keyname = namekeys.Find(x => s.Substring(i).StartsWith(x));

                            if (namerep.ContainsKey(keyname))
                            {
                                sb.Append(namerep[keyname]);
                                i += keyname.Length - 1;                  // skip this, we are in alpha, -1 because of i++ at top
                            }
                            else
                                sb.Append(char.ToUpper(c));
                        }
                        else
                            sb.Append(char.ToUpper(c));
                    }
                }
                else
                {
                    if (c == '_')       // _ is space
                        c = ' ';

                    if (c == ' ')       // now space, go into space mode..
                    {
                        state = State.space;
                        sb.Append(c);
                    }
                    else
                    {                                       // any other than 0-9 and a-z
                        if (state != State.nonalpha)
                        {
                            if (state != State.space)       // space it
                                sb.Append(' ');

                            state = State.nonalpha;
                        }

                        sb.Append(c);
                    }
                }
            }
        }

        if (state == State.digits0)     // if trailing 0, append.
            sb.Append("0");

        return sb.ToString();
    }

    // regexp of below : string s = Regex.Replace(capslower, @"([A-Z]+)([A-Z][a-z])", "$1 $2"); //Upper(rep)UpperLower = Upper(rep) UpperLower
    // s = Regex.Replace(s, @"([a-z\d])([A-Z])", "$1 $2");     // lowerdecUpper split
    // s = Regex.Replace(s, @"[-\s]", " "); // -orwhitespace with spc

    public static string SplitCapsWord(this string capslower)
    {
        if (capslower == null || capslower.Length == 0)
            return "";

        List<int> positions = new List<int>();
        List<string> words = new List<string>();

        int start = 0;

        if (capslower[0] == '-' || char.IsWhiteSpace(capslower[0]))  // Remove leading dash or whitespace
            start = 1;

        for (int i = 1; i <= capslower.Length; i++)
        {
            char c0 = capslower[i - 1];
            char c1 = i < capslower.Length ? capslower[i] : '\0';
            char c2 = i < capslower.Length - 1 ? capslower[i + 1] : '\0';

            if (i == capslower.Length || // End of string
                (i < capslower.Length - 1 && c0 >= 'A' && c0 <= 'Z' && c1 >= 'A' && c1 <= 'Z' && c2 >= 'a' && c2 <= 'z') || // UpperUpperLower
                (((c0 >= 'a' && c0 <= 'z') || (c0 >= '0' && c0 <= '9')) && c1 >= 'A' && c1 <= 'Z') || // LowerdigitUpper
                (c1 == '-' || c1 == ' ' || c1 == '\t' || c1 == '\r')) // dash or whitespace
            {
                if (i > start)
                    words.Add(capslower.Substring(start, i - start));

                if (i < capslower.Length && (c1 == '-' || c1 == ' ' || c1 == '\t' || c1 == '\r'))
                    start = i + 1;
                else
                    start = i;
            }
        }

        return String.Join(" ", words);
    }

    public static bool InQuotes(this string s, int max )            // left true if quote left over on line, taking care of any escapes..
    {
        bool inquote = false;
        char quotechar = ' ';

        for (int i = 0; i < max; i++)
        {
            if (s[i] == '\\' && i < max - 1 && s[i + 1] == quotechar)
                i += 1;     // ignore this, ignore "
            else if (s[i] == '"' || s[i] == '\'')
            {
                quotechar = s[i];
                inquote = !inquote;
            }
        }

        return inquote;
    }

    public static string SafeVariableString(this string normal)
    {
        string ret = "";
        foreach (char c in normal)
        {
            if (char.IsLetterOrDigit(c) || c == '_')
                ret += c;
            else
                ret += "_";
        }
        return ret;
    }

    public static string SafeFileString(this string normal)
    {
        normal = normal.Replace("*", "_star");
        normal = normal.Replace("/", "_slash");
        normal = normal.Replace("\\", "_slash");
        normal = normal.Replace(":", "_colon");
        normal = normal.Replace("?", "_qmark");

        string ret = "";
        foreach (char c in normal)
        {
            if (char.IsLetterOrDigit(c) || c == ' ' || c == '-' || c == '_')
                ret += c;
        }
        return ret;
    }

    public static string FDName(this string normal)
    {
        string n = new string(normal.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray());
        return n.ToLower();
    }

    public static bool EqualsAlphaNumOnlyNoCase(this string left, string right)
    {
        left = left.Replace("_", "").Replace(" ", "").ToLower();        // remove _, spaces and lower
        right = right.Replace("_", "").Replace(" ", "").ToLower();
        return left.Equals(right);
    }

    public static string RemoveTrailingCZeros(this string str)
    {
        int index = str.IndexOf('\0');
        if (index >= 0)
            str = str.Substring(0, index);
        return str;
    }

    public static int ApproxMatch(this string str, string other, int min)       // how many runs match between the two strings
    {
        int total = 0;
        for (int i = 0; i < str.Length; i++)
        {
            for (int j = 0; i < str.Length && j < other.Length; j++)
            {
                if (str[i] == other[j])
                {
                    int i2 = i + 1, j2 = j + 1;

                    int count = 1;
                    while (i2 < str.Length && j2 < other.Length && str[i2] == other[j2])
                    {
                        count++;
                        i2++;
                        j2++;
                    }

                    //if ( count>1)  System.Diagnostics.Debug.WriteLine("Match " + str.Substring(i) + " vs " + other.Substring(j) + " " + count);
                    if (count >= min)   // at least this number of chars in a row.
                    {
                        total += count;
                        i += count;
                        //System.Diagnostics.Debug.WriteLine(" left " + str.Substring(i));
                    }
                }
            }
        }

        //System.Diagnostics.Debug.WriteLine("** TOTAL " + str + " vs " + other + " " + total);

        return total;
    }

    public static string Truncate(this string str, int start, int length)
    {
        int len = str.Length - start;
        if (str == null || len < 1)
            return "";
        else
        {
            if (length > len)
                length = len;
            return str.Substring(start, length);
        }
    }

    static public string WordWrap(this string input, int linelen)
    {
        String[] split = input.Split(new char[] { ' ' });

        string ans = "";
        int l = 0;
        for (int i = 0; i < split.Length; i++)
        {
            ans += split[i];
            l += split[i].Length;
            if (l > linelen)
            {
                ans += Environment.NewLine;
                l = 0;
            }
            else
                ans += " ";
        }

        return ans;
    }

    static public string StackTrace(this string trace, string enclosingfunc, int lines)
    {
        int offset = trace.IndexOf(enclosingfunc);

        string ret = "";

        if (offset != -1)
        {
            CutLine(ref trace, offset);

            while (lines-- > 0)
            {
                string l = CutLine(ref trace, 0);
                if (l != "")
                {
                    if (ret != "")
                        ret = ret + Environment.NewLine + l;
                    else
                        ret = l;
                }
                else
                    break;
            }
        }
        else
            ret = trace;

        return ret;
    }

    static public string CutLine(ref string trace, int offset)
    {
        int nloffset = trace.IndexOf(Environment.NewLine, offset);
        string ret;
        if (nloffset != -1)
        {
            ret = trace.Substring(offset, nloffset - offset);
            trace = trace.Substring(nloffset);
            if (trace.Length >= Environment.NewLine.Length)
                trace = trace.Substring(Environment.NewLine.Length);
        }
        else
        {
            ret = trace;
            trace = "";
        }

        return ret;
    }

    static public int IndexOf(this string s, string[] array, out int fi)   // in array, find one with first occurance, return which one in i
    {
        int found = -1;
        fi = -1;
        for (int av = 0; av < array.Length; av++)
        {
            int pos = s.IndexOf(array[av]);
            if (pos != -1 && (found == -1 || pos < found))
            {
                found = pos;
                fi = av;
            }
        }
        return found;
    }

    public enum StartWithResult
    {
        None,
        Keyword,
        KeywordCont
    };

    public static StartWithResult StringStartsWith(ref string s, string part, string cstr, StringComparison c = StringComparison.InvariantCultureIgnoreCase)
    {
        if (part != null && s.StartsWith(part, c))
        {
            s = s.Substring(part.Length);
            if (s.StartsWith(cstr, c))
            {
                s = s.Substring(cstr.Length);
                return StartWithResult.KeywordCont;
            }
            else
                return StartWithResult.Keyword;
        }
        else
            return StartWithResult.None;
    }

    public static string FirstWord( ref string s, char[] stopchars)
    {
        int i = 0;
        while (i < s.Length && Array.IndexOf(stopchars,s[i]) == -1 )
            i++;

        string ret = s.Substring(0, i);
        s = s.Substring(i).TrimStart();
        return ret;
    }

    public static bool IsPrefix(ref string s, string t, StringComparison c = StringComparison.InvariantCulture)
    {
        if (s.StartsWith(t, c))
        {
            s = s.Substring(t.Length);
            return true;
        }
        return false;
    }

    public static string RegExWildCardToRegular(this string value)
    {
        if (value.Contains("*") || value.Contains("?"))
            return "^" + System.Text.RegularExpressions.Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$";
        else
            return "^" + value + ".*$";
    }
}

