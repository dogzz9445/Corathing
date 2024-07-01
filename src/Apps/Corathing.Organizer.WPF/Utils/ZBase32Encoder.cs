using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Organizer.WPF.Utils;

public class ZBase32Encoder
{
    private const string Charset = "ybndrfg8ejkmcpqxot1uwisza345h769";

    public static string ToString(byte[] input)
    {
        if (input == null || input.Length == 0)
        {
            throw new System.ArgumentNullException("input");
        }

        long returnLength = (input.Length * 8L - 1) / 5 + 1;
        if (returnLength > (long)System.Int32.MaxValue)
        {
            throw new System.ArgumentException("Argument length is too large. (Parameter 'input')");
        }

        char[] returnArray = new char[returnLength];

        byte nextChar = 0, bitsRemaining = 5;
        int arrayIndex = 0;

        foreach (byte b in input)
        {
            nextChar = (byte)(nextChar | (b >> (8 - bitsRemaining)));
            returnArray[arrayIndex++] = Charset[nextChar];

            if (bitsRemaining < 4)
            {
                nextChar = (byte)((b >> (3 - bitsRemaining)) & 31);
                returnArray[arrayIndex++] = Charset[nextChar];
                bitsRemaining += 5;
            }

            bitsRemaining -= 3;
            nextChar = (byte)((b << bitsRemaining) & 31);
        }

        //if we didn't end with a full char
        if (arrayIndex != returnLength)
        {
            returnArray[arrayIndex++] = Charset[nextChar];
        }

        return new string(returnArray);
    }
}
