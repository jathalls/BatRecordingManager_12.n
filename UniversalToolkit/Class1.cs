using System;
using System.IO;
using System.Text.RegularExpressions;

namespace UniversalToolkit
{
    /// <summary>
    /// Class About - describes the contents of the library
    /// </summary>
    public class About
    {
        public About() { }

        public static string Details()
        {
            return (@" Universal Toolkit
by Justin A. T. Halls
(C)2020

A general library of useful classes, functions and controls primarily for use
with Bat Recording Manager and Pulse Train Anlaysis, but also available for
more generic use.  Replaces the XCeed Extended WPF Toolkit which has gone over
to licensing which prohibits even voluntary contributions in return for
distributed copies of software.
");
        }
    }

    /// <summary>
    /// Generic static class of generically useful static functions
    /// </summary>
    public static class UniversalTools
    {
        public static void WriteArrayToFile<T>(string fullyQualifiedFile, T[] data)
        {
            if (string.IsNullOrWhiteSpace(fullyQualifiedFile) || data == null) return;

            if (File.Exists(fullyQualifiedFile))
            {
                string bakFile = Path.ChangeExtension(fullyQualifiedFile, "bak");
                if (File.Exists(bakFile)) File.Delete(bakFile);
                File.Move(fullyQualifiedFile, bakFile);
            }

            using (var sw = new StreamWriter(File.OpenWrite(fullyQualifiedFile)))
            {
                foreach (T val in data)
                {
                    sw.WriteLine(val);
                }
            }
        }

        public static string GenerateTag(string location,DateTime date)
        {
            string result = "";
            if(string.IsNullOrWhiteSpace(location)) { return (result); }
            string pattern = @"^([a-zA-Z\s]+)";
            var regex=Regex.Matches(location, pattern);
            if (regex != null)
            {
                if (!String.IsNullOrEmpty(regex[0].Groups[1].Value))
                {
                    var locationName = regex[0].Groups[1].Value.Trim();
                    var capsRegex = Regex.Matches(locationName, "[A_Z]");
                    if (capsRegex != null)
                    {
                        for (int i = 0; i < capsRegex.Count; i++)
                        {
                            result += capsRegex[i].Value;
                        }
                    }
                    if (locationName.Length <= 3)
                    {
                        result = locationName.ToUpper();

                    }
                    else
                    {
                        if (result.Length == 0) result += locationName[0];
                        if (result.Length == 1) result += locationName[locationName.Length / 2];
                        if (result.Length == 2) result += locationName[locationName.Length - 1];
                        result = result.ToUpper();
                    }
                }
                else
                {
                    result = "TAG";
                }

            }
            else result = "TAG";

            result += $"{date.Year%100:00}-0_{date.Year:0000}{date.Month:00}{date.Day:00}";

            return (result);
        }

    }
}
