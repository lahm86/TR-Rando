﻿using System.Collections.Generic;
using TREnvironmentEditor.Helpers;
using TRFDControl;
using TRFDControl.FDEntryTypes;
using TRFDControl.Utilities;
using TRLevelReader.Model;

namespace TREnvironmentEditor.Model.Types
{
    public class EMKillLaraFunction : BaseEMFunction
    {
        public List<EMLocation> Locations { get; set; }

        public override void ApplyToLevel(TR2Level level)
        {
            FDControl control = new FDControl();
            control.ParseFromLevel(level);

            foreach (EMLocation location in Locations)
            {
                TRRoomSector sector = FDUtilities.GetRoomSector(location.X, location.Y, location.Z, (short)ConvertItemNumber(location.Room, level.NumRooms), level, control);
                CreateTrigger(sector, control);
            }

            control.WriteToLevel(level);
        }

        public override void ApplyToLevel(TR3Level level)
        {
            FDControl control = new FDControl();
            control.ParseFromLevel(level);

            foreach (EMLocation location in Locations)
            {
                TRRoomSector sector = FDUtilities.GetRoomSector(location.X, location.Y, location.Z, (short)ConvertItemNumber(location.Room, level.NumRooms), level, control);
                CreateTrigger(sector, control);
            }

            control.WriteToLevel(level);
        }

        private void CreateTrigger(TRRoomSector sector, FDControl control)
        {
            // If there is no floor data create the FD to begin with.
            if (sector.FDIndex == 0)
            {
                control.CreateFloorData(sector);
            }

            List<FDEntry> entries = control.Entries[sector.FDIndex];
            if (entries.FindIndex(e => e is FDKillLaraEntry) == -1)
            {
                entries.Add(new FDKillLaraEntry
                {
                    Setup = new FDSetup(FDFunctions.KillLara)
                });
            }
        }
    }
}