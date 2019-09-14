using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CreepRateApp.Core
{
    public static class CheckData
    {
        public static bool IsNumeric(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
            }
            else
            { 
                return false; 
            }
        }

        public static bool IsInt(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
            return Regex.IsMatch(value, @"^[+-]?\d*$");
            }
            else
            { 
                return false; 
            }
        }

        public static bool IsUnsign(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
            return Regex.IsMatch(value, @"^\d*[.]?\d*$");
            }
            else
            { 
                return false; 
            }
        }

        public static bool isTel(string strInput)
        {
            if (!string.IsNullOrWhiteSpace(strInput))
            {
                return Regex.IsMatch(strInput, @"\d{3}-\d{8}|\d{4}-\d{7}");
            }
            else
            {
                return false;
            }
        }
    }
}
