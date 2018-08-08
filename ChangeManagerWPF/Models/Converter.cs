using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeManagerWPF.Models.ChangeManager
{
    public static class Converter
    {
        public static byte[] HashStringToByteArray(string gitHash)
        {
            return Enumerable.Range(1, gitHash.Length / 2 - 1) // Range(1, - to get rid of "0x"  
                .Select(i => Convert.ToByte(gitHash.Substring(i * 2, 2), 16))
                .ToArray();
        }

        public static string ByteArrayToBinHex(this byte[] bytes)
        {
            return "0x" + bytes.Select(b => b.ToString("X2")).Aggregate((s1, s2) => s1 + s2).ToLower();
        }
    }
}
