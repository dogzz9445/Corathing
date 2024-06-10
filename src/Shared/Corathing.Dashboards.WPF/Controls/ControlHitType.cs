namespace Corathing.Dashboards.WPF.Controls;

[Flags]
public enum ControlHitType
{
    None   = 0,
    Body   = 1 << 0,
    Line   = 1 << 1,
    Left   = 1 << 2,
    Right  = 1 << 3,
    Top    = 1 << 4,
    Bottom = 1 << 5,
};
