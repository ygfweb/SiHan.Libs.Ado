using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SiHan.Libs.Ado
{
    internal class StringHelper
    {
        /// <summary>
        /// Pascal风格转小写下划线
        /// </summary>
        public static string PascalCaseToUnderscores(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "";
            }
            else
            {
                input = input.Trim();
                string result = "";  //目标字符串
                for (int j = 0; j < input.Length; j++)
                {
                    string temp = input[j].ToString();
                    if (Regex.IsMatch(temp, "[A-Z]"))
                    {
                        temp = "_" + temp.ToLower();
                    }
                    result = result + temp;
                }
                return result.Trim('_');
            }
        }
    }
}
