using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Organizer.WPF.Utils;

[Flags]
public enum BothCheckFlag
{
    // Defaults
    None = 0,
    Any  = 1 << 0,
    A    = 1 << 1,
    B    = 1 << 2,

    // Combinations
    Both = A | B,
}

public static class FlagHelper
{
    public static BothCheckFlag GetFlag(bool a, bool b)
    {
        var flag = BothCheckFlag.None;
        flag |= a || b ? BothCheckFlag.Any : BothCheckFlag.None;
        flag |= a ? BothCheckFlag.A : BothCheckFlag.None;
        flag |= b ? BothCheckFlag.B : BothCheckFlag.None;
        return flag;
    }

    public static BothCheckFlag GetSwitchFlag(bool a, bool b)
    {
        BothCheckFlag flag = GetFlag(a, b);
        if ((flag & BothCheckFlag.Both) == BothCheckFlag.Both)
            return BothCheckFlag.Both;
        if ((flag & BothCheckFlag.A) == BothCheckFlag.A)
            return BothCheckFlag.A;
        if ((flag & BothCheckFlag.B) == BothCheckFlag.B)
            return BothCheckFlag.B;
        return BothCheckFlag.None;
    }
}

public class TestFlagHelper
{
    public static void Test_TestFlagHelper_GetSwitchFlag()
    {
        bool a = true;
        bool b = true;

        ((Func<BothCheckFlag, Action>)((BothCheckFlag a) => a switch
        {
            BothCheckFlag.Both => () =>
            {
                Console.WriteLine("Both");
            },
            BothCheckFlag.A => () =>
            {
                Console.WriteLine("A");
            },
            BothCheckFlag.B => () =>
            {
                Console.WriteLine("B");
            },
            _ => () =>
            {
                Console.WriteLine("None");
            },
        }))(FlagHelper.GetSwitchFlag(a, b));
    }
}
