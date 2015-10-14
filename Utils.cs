using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic
{
    static class Utils
    {
        public static string[] SplitString(string stringLine)
        {
            char separator = ',';
            string[] sArray = stringLine.ToUpper().Split(separator);
            for (int i = 0; i < sArray.Length; i++)
            {
                sArray[i] = sArray[i].Trim();
            }
            return sArray;
        }
    }
}
