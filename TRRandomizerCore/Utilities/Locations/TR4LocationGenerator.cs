using System.Numerics;
using TRLevelControl.Model;
using TRRandomizerCore.Helpers;

namespace TRRandomizerCore.Utilities;

public class TR4LocationGenerator : AbstractLocationGenerator<TR4Type, TR4Level>
{
    public override bool CrawlspacesAllowed => true;
    public override bool WadingAllowed => true;

    protected override TRRoomSector GetSector(Location location, TR4Level level)
    {
        return level.GetRoomSector(location);
    }

    protected override TRRoomSector GetSector(int x, int z, int roomIndex, TR4Level level)
    {
        TR4Room room = level.Rooms[roomIndex];
        return room.GetSector(x, z);
    }

    protected override List<TRRoomSector> GetRoomSectors(TR4Level level, int room)
    {
        return level.Rooms[room].Sectors.ToList();
    }

    protected override TRDictionary<TR4Type, TRStaticMesh> GetStaticMeshes(TR4Level level)
    {
        return level.StaticMeshes;
    }

    protected override int GetRoomCount(TR4Level level)
    {
        return level.Rooms.Count;
    }

    protected override short GetFlipMapRoom(TR4Level level, short room)
    {
        return level.Rooms[room].AlternateRoom;
    }

    protected override bool IsRoomValid(TR4Level level, short room)
    {
        return true;
    }

    protected override Dictionary<TR4Type, List<Location>> GetRoomStaticMeshLocations(TR4Level level, short room)
    {
        Dictionary<TR4Type, List<Location>> locations = new();
        foreach (TR4RoomStaticMesh staticMesh in level.Rooms[room].StaticMeshes)
        {
            if (!locations.ContainsKey(staticMesh.ID))
            {
                locations[staticMesh.ID] = new();
            }
            locations[staticMesh.ID].Add(new()
            {
                X = staticMesh.X,
                Y = staticMesh.Y,
                Z = staticMesh.Z,
                Room = room
            });
        }

        return locations;
    }

    protected override ushort GetRoomDepth(TR4Level level, short room)
    {
        return level.Rooms[room].NumZSectors;
    }

    protected override int GetRoomYTop(TR4Level level, short room)
    {
        return level.Rooms[room].Info.YTop;
    }

    protected override Vector2 GetRoomPosition(TR4Level level, short room)
    {
        return new Vector2(level.Rooms[room].Info.X, level.Rooms[room].Info.Z);
    }

    protected override int GetHeight(TR4Level level, Location location, bool waterOnly)
    {
        return _floorData.GetHeight(location.X, location.Z, location.Room, level.Rooms, waterOnly);
    }
}
