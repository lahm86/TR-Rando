namespace TRLevelControl.Model;

public enum TRFileVersion : uint
{
    Unknown  = 0,
    TR1      = 0x20,
    TR2Alpha = 0x26,
    TR2      = 0x2D,
    TR3a     = 0xFF080038,
    TR3b     = 0xFF180038,
    TR45     = 0x00345254, // classic
    TRR4     = 0x34585254, // remastered
    TRR5     = 0x35585254,
}
