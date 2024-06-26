﻿using System.Text;
using TRLevelControl.Model;

namespace TRLevelControl.Helpers;

public class Room
{
    public sbyte Floor { get; set; }
    public sbyte Ceiling { get; set; }
    public int NumXSectors { get; set; }
    public int NumZSectors { get; set; }
    public List<TRRoomSector> Sectors { get; set; }
    public List<FloorPlan> FloorPlan { get; set; }
    public List<Box> Boxes { get; set; }

    public static Room Create(TRRoom room)
    {
        return new()
        {
            Floor = (sbyte)(room.Info.YBottom / TRConsts.Step1),
            Ceiling = (sbyte)(room.Info.YTop / TRConsts.Step1),
            NumXSectors = room.NumXSectors,
            NumZSectors = room.NumZSectors,
            Sectors = room.Sectors,
            FloorPlan = new(),
            Boxes = new()
        };
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        for (int z = NumZSectors - 1; z >= 0; z--)
        {
            for (int x = 0; x < NumXSectors; x++)
            {
                TRRoomSector sector = Sectors[x * NumZSectors + z];
                sb.Append((sector.IsWall ? "WALL" : sector.BoxIndex.ToString().PadLeft(4, '0')) + " ");
            }
            sb.AppendLine();
        }
        foreach (Box box in Boxes)
        {
            sb.AppendLine(string.Format("{0}: {1}", box.Index.ToString().PadLeft(4, '0'), string.Join(",", box.Overlaps.Select(b => b.Index.ToString().PadLeft(4, '0')))));
        }
        sb.AppendLine();
        return sb.ToString();
    }
}
