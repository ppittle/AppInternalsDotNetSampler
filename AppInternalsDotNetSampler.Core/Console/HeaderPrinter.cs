using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AppInternalsDotNetSampler.Core.Console
{
    public static class HeaderPrinter
    {
        public enum Style
        {
            CenterCaps,
            Left
        }

        public const string Stars = "********************************************************";

        public static string HeaderPrint(string s, Style style)
        {
            if (s.Length > Stars.Length - 2)
                throw new Exception("Update HeaderPrinter");

            switch (style)
            {
                case Style.Left:
                    return string.Format(
                        "*{0}{1}*",
                        s, Pad(Stars.Length - s.Length - 2));
                case Style.CenterCaps:
                {
                    var totalPad = Stars.Length - 2 - s.Length;

                    var leftPad = totalPad/2;
                    var rightPad = totalPad - leftPad;

                    return string.Format(
                        "*{0}{1}{2}*",
                        Pad(leftPad), s.ToUpper(), Pad(rightPad));
                }

                default:
                    throw new NotImplementedException("Update HeaderPrinter");
            }
        }

        private static string Pad(int length)
        {
            return new string(' ', length);
        }
    }

}
