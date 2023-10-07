using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalToolkit
{
    /// <summary>
    /// This class handles the seperation, and reconstitution of comma seperated strings
    /// to and from a single CSL and a list or array of strings.
    /// Combined string seperates items with a comma-space pair.  Individual strings will
    /// be trimmed of leading and trailing white space.
    /// </summary>
    public class CommaSeperatedString
    {
        /// <summary>
        /// The string used to substitue for embedded commas
        /// </summary>
        public static readonly string CommaSeperatorSubstitute = @"%26";

        /// <summary>
        /// Given a string containing CommaSeperatorSubstitutes, replaces those with simple commas.
        /// 
        /// </summary>
        /// <param name="stringToBeModified"></param>
        /// <param name="newSeperator"></param>
        /// <returns></returns>
        public static string ReplaceSubstitutesWithThis(string stringToBeModified,string newSeperator)
        {
            return(stringToBeModified?.Replace( CommaSeperatorSubstitute, newSeperator));
        }
        /// <summary>
        /// the set of strings as a single strng of comma seperated strings
        /// </summary>
        public string cslString { get; set; }

        /// <summary>
        /// The set of strings as a List of string
        /// </summary>
        public List<string> cslList { get; set; } = new List<string>();

        /// <summary>
        /// Constructor accepting a comma seperated list of strings as a single string
        /// </summary>
        /// <param name="cslString"></param>
        public CommaSeperatedString(string cslString)
        {
            SetCombinedString(cslString);
        }

        /// <summary>
        /// Constructor accepting a List of strings
        /// </summary>
        /// <param name="cslList"></param>
        public CommaSeperatedString(List<string> cslList)
        {
            SetStringList(cslList);

        }

        /// <summary>
        /// Constructor accepting an array of strings
        /// </summary>
        /// <param name="cslArray"></param>
        public CommaSeperatedString(string[] cslArray)
        {
           SetStringArray(cslArray);
        }

        /// <summary>
        /// Constructor accepting an Enumerable of strings
        /// </summary>
        /// <param name="cslEnumerable"></param>
        public CommaSeperatedString(IEnumerable<string> cslEnumerable)
        {
            SetEnumerableOfStrings(cslEnumerable);
        }

        /// <summary>
        /// Saves the items in the enumerable of strings as a List and returns
        /// the items as a single comma seperated string.
        /// </summary>
        /// <param name="cslEnumerable"></param>
        /// <returns></returns>
        public string SetEnumerableOfStrings(IEnumerable<string> cslEnumerable) 
        {
            this.cslList = (cslEnumerable??Enumerable.Empty<string>()).ToList();
            cslString = "";
            foreach (string csl in cslList)
            {
                string temp = csl.Replace(",", CommaSeperatorSubstitute);
                cslString += temp.Trim() + ", ";

            }
            cslString = cslString.Trim();
            if (cslString.EndsWith(",", StringComparison.CurrentCulture))
            {
                cslString = cslString.Substring(0, cslString.Length - 1).Trim();
            }

            return cslString;
        }

        /// <summary>
        /// Saves the combined comma-seperated string and returns the comma seperated
        /// items as a List of string
        /// </summary>
        /// <param name="cslString"></param>
        /// <returns></returns>
        public List<string> SetCombinedString(string cslString)
        {
            this.cslString = cslString.Trim();
            cslList = new List<string>();
            if (cslString.Contains(",", StringComparison.CurrentCulture))
            {
                var values = cslString.Split(',');
                foreach (var value in values ?? Array.Empty<string>())
                {
                    string temp = value.Replace(CommaSeperatorSubstitute, ", ");
                    cslList.Add(value);
                }
            }
            else
            {
                cslList.Add(cslString.Trim());
            }
            return cslList;
        }

        /// <summary>
        /// Saves the List of strings and returns the items as a comma seperated 
        /// combined string
        /// </summary>
        /// <param name="cslList"></param>
        /// <returns></returns>
        public string SetStringList(List<string> cslList)
        {
            this.cslList = cslList;
            cslString = "";
            foreach (string csl in cslList ?? new List<string>())
            {
                var temp = csl.Replace(",", CommaSeperatorSubstitute);
                cslString += temp.Trim() + ", ";

            }
            cslString = cslString.Trim();
            if (cslString.EndsWith(",", StringComparison.CurrentCulture))
            {
                cslString = cslString.Substring(0, cslString.Length - 1).Trim();
            }

            return cslString;
        }

        /// <summary>
        /// Saves the array as a List and returns the items as a comma seperated
        /// combined string
        /// </summary>
        /// <param name="cslArray"></param>
        /// <returns></returns>
        public string SetStringArray(string[] cslArray)
        {
            cslString = "";
            cslList= new List<string>();
            foreach (string csl in cslArray ?? Array.Empty<string>())
            {
                var temp=csl.Replace(",", CommaSeperatorSubstitute);
                cslString += temp.Trim() + ", ";
                cslList.Add(csl.Trim());
            }
            cslString = cslString.Trim();
            if (cslString.EndsWith(",", StringComparison.CurrentCulture))
            {
                cslString = cslString.Substring(0, cslString.Length - 1).Trim();
            }
            return(cslString);
        }

        /// <summary>
        /// returns the stored list as a single comma seperated string.  Items with embedded commas
        /// will have them substituted with CommaSeperatedSubstitute
        /// </summary>
        /// <returns></returns>
        public string GetCommaSeperatedString()
        {
            return cslString.Trim();
        }

        /// <summary>
        /// returns the stored list as a List of string
        /// </summary>
        /// <returns></returns>
        public List<string> GetCslList()
        {
            return cslList;
        }

        /// <summary>
        /// returns the stored list as a string array
        /// </summary>
        /// <returns></returns>
        public string[] GetCslArray()
        {
            return cslList.AsEnumerable().ToArray();
        }

        /// <summary>
        /// returns the stored list as an enumerable of string
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetCslEnumerable()
        {
            return cslList.AsEnumerable<string>();
        }
    }
}
