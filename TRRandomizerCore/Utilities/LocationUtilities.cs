﻿using System;
using System.Collections.Generic;
using System.Linq;
using TRFDControl;
using TRFDControl.FDEntryTypes;
using TRFDControl.Utilities;
using TRLevelReader.Model;
using TRRandomizerCore.Helpers;

namespace TRRandomizerCore.Utilities
{
    public static class LocationUtilities
    {
        public static bool ContainsSecret(this Location location, TRLevel level, FDControl floorData)
        {
            TRRoomSector sector = FDUtilities.GetRoomSector(location.X, location.Y, location.Z, (short)location.Room, level, floorData);
            return SectorContainsSecret(sector, floorData);
        }

        public static bool IsSlipperySlope(this Location location, TRLevel level, FDControl floorData)
        {
            TRRoomSector sector = FDUtilities.GetRoomSector(location.X, location.Y, location.Z, (short)location.Room, level, floorData);
            return SectorIsSlipperySlope(sector, floorData);
        }

        public static bool SectorContainsSecret(TRRoomSector sector, FDControl floorData)
        {
            if (sector.FDIndex != 0)
            {
                return floorData.Entries[sector.FDIndex].Find(e => e is FDTriggerEntry) is FDTriggerEntry trigger
                    && trigger.TrigType == FDTrigType.Pickup
                    && trigger.TrigActionList.Find(a => a.TrigAction == FDTrigAction.SecretFound) != null;
            }

            return false;
        }

        public static bool SectorIsSlipperySlope(TRRoomSector sector, FDControl floorData)
        {
            return sector.FDIndex != 0
                && floorData.Entries[sector.FDIndex].Find(e => e is FDSlantEntry slant && slant.Type == FDSlantEntryType.FloorSlant) is FDSlantEntry floorSlant
                && (Math.Abs(floorSlant.XSlant) > 2 || Math.Abs(floorSlant.ZSlant) > 2);
        }

        public static int GetCornerHeight(TRRoomSector sector, FDControl floorData, int x, int z)
        {
            sbyte floor = sector.Floor;
            if (sector.FDIndex != 0)
            {
                FDEntry entry = floorData.Entries[sector.FDIndex].Find(e => (e is FDSlantEntry s && s.Type == FDSlantEntryType.FloorSlant)
                    || (e is TR3TriangulationEntry tri && tri.IsFloorTriangulation));
                if (entry is FDSlantEntry slant)
                {
                    sbyte corner0 = sector.Floor;
                    sbyte corner1 = sector.Floor;
                    sbyte corner2 = sector.Floor;
                    sbyte corner3 = sector.Floor;

                    if (slant.XSlant > 0)
                    {
                        corner0 += slant.XSlant;
                        corner1 += slant.XSlant;
                    }
                    else if (slant.XSlant < 0)
                    {
                        corner2 -= slant.XSlant;
                        corner3 -= slant.XSlant;
                    }

                    if (slant.ZSlant > 0)
                    {
                        corner0 += slant.ZSlant;
                        corner2 += slant.ZSlant;
                    }
                    else if (slant.ZSlant < 0)
                    {
                        corner1 -= slant.ZSlant;
                        corner3 -= slant.ZSlant;
                    }

                    if ((x & 1023) < 512)
                    {
                        floor = (z & 1023) < 512 ? corner0 : corner1;
                    }
                    else
                    {
                        floor = (z & 1023) < 512 ? corner3 : corner2;
                    }
                }
                else if (entry is TR3TriangulationEntry triangulation)
                {
                    List<byte> triangleCorners = new List<byte>
                    {
                        triangulation.TriData.C00,
                        triangulation.TriData.C01,
                        triangulation.TriData.C10,
                        triangulation.TriData.C11
                    };

                    int max = triangleCorners.Max();
                    List<sbyte> corners = new List<sbyte>
                    {
                        (sbyte)(max - triangleCorners[0]),
                        (sbyte)(max - triangleCorners[1]),
                        (sbyte)(max - triangleCorners[2]),
                        (sbyte)(max - triangleCorners[3])
                    };

                    if ((x & 1023) < 512)
                    {
                        floor += (z & 1023) < 512 ? corners[0] : corners[1];
                    }
                    else
                    {
                        floor += (z & 1023) < 512 ? corners[2] : corners[3];
                    }
                }
            }

            return floor * 256;
        }
    }
}