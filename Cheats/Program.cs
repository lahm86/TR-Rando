using TRLevelControl;
using TRLevelControl.Model;

namespace Cheats;

internal class Program
{
    static void Main()
    {
        string dir = string.Empty;        
        while (dir.Length == 0 || !Directory.Exists(dir))
        {
            Console.Write("PDP directory: ");
            dir = Console.ReadLine();
        }

        int version = -1;
        while (version == -1)
        {
            Console.Write("Game version (1,2 etc): ");
            if (int.TryParse(Console.ReadLine(), out version)
                && Enum.IsDefined(typeof(TRGameVersion), (version - 1)))
            {
                break;
            }
            else
            {
                version = -1;
            }
        }

        switch ((TRGameVersion)(version - 1))
        {
            case TRGameVersion.TR1:
                Cheat(new TR1PDPControl(), TRGameVersion.TR1, dir);
                break;
            case TRGameVersion.TR2:
                Cheat(new TR2PDPControl(), TRGameVersion.TR2, dir);
                break;
            case TRGameVersion.TR3:
                Cheat(new TR3PDPControl(), TRGameVersion.TR3, dir);
                break;
            case TRGameVersion.TR4:
                Cheat(new TR4PDPControl(), TRGameVersion.TR4, dir);
                break;
            case TRGameVersion.TR5:
                Cheat(new TR5PDPControl(), TRGameVersion.TR5, dir);
                break;
        }

        Console.WriteLine();
        Console.WriteLine("Done");
    }

    static void Cheat<T>(TRPDPControlBase<T> control, TRGameVersion version, string dir)
        where T : Enum
    {
        foreach (var file in Directory.EnumerateFiles(dir, "*.pdp"))
        {
            var pdp = control.Read(file);

            TRFXCommand endLevelCmd = new()
            {
                FrameNumber = 1,
                EffectID = (short)TR1FX.EndLevel,
            };

            // Side-step left to end level
            pdp[default].Animations[65].Commands.Add(endLevelCmd);

            // Underwater roll to end level
            int uwRollAnim = version == TRGameVersion.TR1 ? 99 : 203;
            pdp[default].Animations[uwRollAnim].Commands.Add(endLevelCmd);

            control.Write(pdp, file);
        }
    }
}
