﻿using TRLevelControl.Model;

namespace TRDataControl.Environment;

public class EMDuplicateSwitchTriggerFunction : EMDuplicateTriggerFunction
{
    public ushort NewSwitchIndex { get; set; }
    public ushort OldSwitchIndex { get; set; }

    public override void ApplyToLevel(TR1Level level)
    {
        EMLevelData data = GetData(level);

        SetupLocations(data, level.Entities);

        // Duplicate the triggers to the switch's location
        base.ApplyToLevel(level);

        // Go one step further and replace the duplicated trigger with the new switch ref
        UpdateTriggers(data, level.FloorData, location =>
            level.GetRoomSector(data.ConvertLocation(location)));
    }

    public override void ApplyToLevel(TR2Level level)
    {
        EMLevelData data = GetData(level);

        SetupLocations(data, level.Entities);

        base.ApplyToLevel(level);

        UpdateTriggers(data, level.FloorData, location =>
            level.GetRoomSector(data.ConvertLocation(location)));
    }

    public override void ApplyToLevel(TR3Level level)
    {
        EMLevelData data = GetData(level);

        SetupLocations(data, level.Entities);

        base.ApplyToLevel(level);

        UpdateTriggers(data, level.FloorData, location =>
            level.GetRoomSector(data.ConvertLocation(location)));
    }

    private void SetupLocations(EMLevelData data, List<TR1Entity> entities)
    {
        // Get a location for the switch we're interested in
        TR1Entity switchEntity = entities[data.ConvertEntity(NewSwitchIndex)];
        Locations = new List<EMLocation>
        {
            new() {
                X = switchEntity.X,
                Y = switchEntity.Y,
                Z = switchEntity.Z,
                Room = data.ConvertRoom(switchEntity.Room)
            }
        };

        // Get the location of the old switch
        switchEntity = entities[data.ConvertEntity(OldSwitchIndex)];
        BaseLocation = new EMLocation
        {
            X = switchEntity.X,
            Y = switchEntity.Y,
            Z = switchEntity.Z,
            Room = data.ConvertRoom(switchEntity.Room)
        };
    }

    private void SetupLocations(EMLevelData data, List<TR2Entity> entities)
    {
        // Get a location for the switch we're interested in
        TR2Entity switchEntity = entities[data.ConvertEntity(NewSwitchIndex)];
        Locations = new List<EMLocation>
        {
            new() {
                X = switchEntity.X,
                Y = switchEntity.Y,
                Z = switchEntity.Z,
                Room = data.ConvertRoom(switchEntity.Room)
            }
        };

        // Get the location of the old switch
        switchEntity = entities[data.ConvertEntity(OldSwitchIndex)];
        BaseLocation = new EMLocation
        {
            X = switchEntity.X,
            Y = switchEntity.Y,
            Z = switchEntity.Z,
            Room = data.ConvertRoom(switchEntity.Room)
        };
    }

    private void SetupLocations(EMLevelData data, List<TR3Entity> entities)
    {
        // Get a location for the switch we're interested in
        TR3Entity switchEntity = entities[data.ConvertEntity(NewSwitchIndex)];
        Locations = new List<EMLocation>
        {
            new() {
                X = switchEntity.X,
                Y = switchEntity.Y,
                Z = switchEntity.Z,
                Room = data.ConvertRoom(switchEntity.Room)
            }
        };

        // Get the location of the old switch
        switchEntity = entities[data.ConvertEntity(OldSwitchIndex)];
        BaseLocation = new EMLocation
        {
            X = switchEntity.X,
            Y = switchEntity.Y,
            Z = switchEntity.Z,
            Room = data.ConvertRoom(switchEntity.Room)
        };
    }

    private void UpdateTriggers(EMLevelData data, FDControl control, Func<EMLocation, TRRoomSector> sectorGetter)
    {
        short newSwitchIndex = data.ConvertEntity(NewSwitchIndex);
        foreach (EMLocation location in Locations)
        {
            TRRoomSector baseSector = sectorGetter.Invoke(location);

            List<FDEntry> keyTriggers = control[baseSector.FDIndex].FindAll(e => e is FDTriggerEntry);
            foreach (FDEntry entry in keyTriggers)
            {
                (entry as FDTriggerEntry).SwitchOrKeyRef = newSwitchIndex;
            }
        }
    }
}
