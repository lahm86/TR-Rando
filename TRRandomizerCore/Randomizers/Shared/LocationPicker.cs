﻿using Newtonsoft.Json;
using TRLevelControl;
using TRLevelControl.Model;
using TRRandomizerCore.Editors;
using TRRandomizerCore.Helpers;
using TRRandomizerCore.Utilities;

namespace TRRandomizerCore.Randomizers;

public class LocationPicker : IRouteManager
{
    private readonly Dictionary<string, List<Location>> _routes;

    private List<Location> _locations, _usedTriggerLocations, _currentRoute;
    private List<short> _allRooms;
    private RandomizerSettings _settings;
    private Random _generator;

    public Func<Location, bool> TriggerTestAction { get; set; }
    public Func<Location, bool, List<short>, bool> KeyItemTestAction { get; set; }
    public List<ExtRoomInfo> RoomInfos { get; set; }
    public int LevelSize { get; private set; }

    private enum KeyMode
    {
        Low,
        High,
    }

    public LocationPicker(string routePath)
    {
        _routes = JsonConvert.DeserializeObject<Dictionary<string, List<Location>>>(File.ReadAllText(routePath));
    }

    public void Initialise(string levelName, List<Location> globalLocations, RandomizerSettings settings, Random generator)
    {
        _locations = globalLocations;
        _usedTriggerLocations = new();
        _settings = settings;
        _generator = generator;
        LoadCurrentRoute(levelName);
        _locations.Shuffle(_generator);

        _allRooms = _locations
            .Select(l => l.Room)
            .Distinct()
            .ToList();

        LevelSize = _allRooms.Sum(r => RoomInfos?[r].Size ?? 0);
    }

    private void LoadCurrentRoute(string levelName)
    {
        _currentRoute = _routes.ContainsKey(levelName) ? _routes[levelName] : null;
        if (_currentRoute == null || RoomInfos == null)
        {
            return;
        }

        // Dynamic locations can indicate return paths or even added puzzle/challenge rooms.
        // If we cannot match them precisely to a room that's been added, or if they are return
        // paths but we don't want items there, mark them as invalid.
        // This assumes our level design does not overlap rooms anywhere.
        List<Location> dynamicLocations = _currentRoute
            .FindAll(l => l.RoomType == RoomType.ReturnPath || l.RoomType == RoomType.Challenge);

        foreach (Location location in dynamicLocations)
        {
            List<ExtRoomInfo> matchingInfos = RoomInfos.FindAll(r => r.Contains(location));
            if (matchingInfos.Count != 1
                || (!_settings.IncludeReturnPathLocations
                && (location.RoomType == RoomType.ReturnPath || location.RequiresReturnPath)))
            {
                InvalidateLocation(location);
            }
            else
            {
                location.Room = (short)RoomInfos.IndexOf(matchingInfos[0]);
            }
        }
    }

    private static void InvalidateLocation(Location location)
    {
        location.Room = TRConsts.NoRoom;
        location.Validated = false;
    }

    public void RandomizePickupLocation<T>(TREntity<T> entity)
        where T : Enum
    {
        Location location = GetRandomLocation();
        if (location != null)
        {
            SetLocation(entity, location);
        }
    }

    public void RandomizeKeyItemLocation<T>(TREntity<T> entity, bool hasPickupTrigger, int levelSequence)
        where T : Enum
    {
        uint keyItemID = GetKeyItemID(levelSequence, entity);
        if (!Enum.IsDefined(typeof(T), keyItemID))
        {
            return;
        }

        Location location = GetKeyItemLocation((int)keyItemID, entity, hasPickupTrigger);
        if (location != null)
        {
            SetLocation(entity, location);
        }
    }

    public Location GetKeyItemLocation<T>(int keyItemID, TREntity<T> entity, bool hasPickupTrigger)
        where T : Enum
    {
        List<short> roomPool = GetRoomPool(keyItemID);
        if (roomPool.Count == 0)
        {
            // This key item must remain static.
            return null;
        }

        Location newLocation;
        Location currentLocation = entity.GetLocation();

        // If there is a trigger for this key item that will be shifted by environment
        // changes, make sure to select a location that doesn't already have a trigger.
        // Don't test triggers if we have picked the same tile, but do allow callers to
        // handle their own specific tests in all cases.
        while (true)
        {
            newLocation = GetRandomLocation(roomPool);
            if (hasPickupTrigger && !newLocation.IsEquivalent(currentLocation)
                && (TriggerTestAction(newLocation) || _usedTriggerLocations.Contains(newLocation)))
            {
                continue;
            }

            if (KeyItemTestAction != null && !KeyItemTestAction(newLocation, hasPickupTrigger, roomPool))
            {
                continue;
            }

            break;
        }

        if (hasPickupTrigger)
        {
            _usedTriggerLocations.Add(newLocation);
        }

        return newLocation;
    }

    public List<short> GetRoomPool(int keyItemID)
    {
        List<short> roomPool = new();
        if (_currentRoute == null)
        {
            return roomPool;
        }

        List<Zone> zones = new();
        for (int i = 0; i < _currentRoute.Count; i++)
        {
            Location location = _currentRoute[i];
            if (TestLocation(location, KeyMode.Low, keyItemID))
            {
                zones.Add(new()
                {
                    LowIndex = i,
                    HighIndex = -1
                });
            }

            if (TestLocation(location, KeyMode.High, keyItemID))
            {
                if (zones.Count == 0)
                {
                    // Assume a waypoint at the start of the level has been omitted
                    zones.Add(new());
                }

                if (i == zones[^1].LowIndex)
                {
                    // Intentionally do not move this item e.g. Seraph in The Deck
                    return roomPool;
                }

                // Clamp to outside this room e.g. when several waypoints may be placed on
                // an upper bound, we want to be in the previous room, not previous waypoint.
                int highIndex = i;
                int highRoom = _currentRoute[highIndex].Room;
                while (_currentRoute[highIndex].Room == highRoom && highIndex >= 0)
                {
                    highIndex--;
                }

                zones[^1].HighIndex = highIndex;
            }
        }

        foreach (Zone zone in zones)
        {
            if (zone.HighIndex == -1)
            {
                // Assume the zone extends to level end
                zone.HighIndex = _currentRoute.Count - 1;
            }

            for (int i = zone.LowIndex; i <= zone.HighIndex; i++)
            {
                // Not validated indicates although part of a valid route overall, this
                // room can't support key items e.g. room 38 in Temple of Xian
                if (_currentRoute[i].Validated && !roomPool.Contains(_currentRoute[i].Room))
                {
                    roomPool.Add(_currentRoute[i].Room);
                }
            }
        }

        return roomPool;
    }

    public bool IsValidKeyItemLocation(int keyItemID, Location location, bool hasPickupTrigger)
    {
        List<short> roomPool = GetRoomPool(keyItemID);
        return roomPool.Contains(location.Room)
            && _locations.Any(l => l.IsEquivalent(location))
            && (KeyItemTestAction == null || KeyItemTestAction(location, hasPickupTrigger, roomPool));
    }

    private bool TestLocation(Location location, KeyMode keyTestMode, int keyItemID)
    {
        // This assumes that all possible scenarios are defined on a route
        string keyIDs = keyTestMode == KeyMode.Low ? location.KeyItemsLow : location.KeyItemsHigh;
        if (keyIDs == null || !keyIDs.Contains(keyItemID.ToString()))
        {
            return false;
        }

        if (location.Range != _settings.KeyItemRange)
        {
            return false;
        }

        return (location.RequiresReturnPath && _settings.IncludeReturnPathLocations)
            || (!location.RequiresReturnPath && !_settings.IncludeReturnPathLocations);
    }

    public int GetRoutePosition(Location location)
    {
        return _currentRoute.FindIndex(l => l.Room == location.Room);
    }

    public List<short> GetDemarkedZone(int zoneID)
    {
        List<short> rooms = new();
        int demarker = _currentRoute.FindIndex(l => l.TargetType == zoneID);
        if (demarker != -1)
        {
            rooms.AddRange(_currentRoute.GetRange(0, demarker).Select(l => l.Room));
        }
        return rooms;
    }

    public List<short> GetRouteRooms()
    {
        return _currentRoute
            .Select(l => l.Room)
            .Distinct()
            .ToList();
    }

    public int GetProximity(Location location1, Location location2)
    {
        if (_currentRoute == null)
        {
            return -1;
        }

        int routeIndex1 = _currentRoute.FindIndex(l => l.Room == location1.Room);
        int routeIndex2 = _currentRoute.FindIndex(l => l.Room == location2.Room);

        int distance = 0;
        if (routeIndex1 == -1 || routeIndex2 == -1)
        {
            return distance;
        }

        HashSet<int> visitedRooms = new();
        for (int i = Math.Min(routeIndex1, routeIndex2); i <= Math.Max(routeIndex1, routeIndex2); i++)
        {
            Location route = _currentRoute[i];
            if (route.Room != TRConsts.NoRoom && _allRooms.Contains(route.Room) && visitedRooms.Add(route.Room))
            {
                distance += RoomInfos[route.Room].Size; 
            }
        }

        return distance;
    }

    public Location GetRandomLocation(List<short> roomPool = null)
    {
        if (roomPool == null || !_locations.Any(l => roomPool.Contains(l.Room)))
        {
            roomPool = _allRooms;
        }

        Location location;
        do
        {
            location = _locations[_generator.Next(0, _locations.Count)];
        }
        while (!roomPool.Contains(location.Room));

        return location;
    }

    public uint GetKeyItemID<T>(int levelSequence, TREntity<T> entity)
        where T : Enum
    {
        // Arbitrary method of generating unique IDs per level.
        int roomX = RoomInfos[entity.Room].MinX - TRConsts.Step4;
        int roomZ = RoomInfos[entity.Room].MinZ - TRConsts.Step4;

        int x = (entity.X - roomX) / TRConsts.Step4;
        int z = (entity.Z - roomZ) / TRConsts.Step4;
        int y = entity.Y / TRConsts.Step1;

        long id = 10000
            + (levelSequence - 1) * 1000
            + (uint)(object)entity.TypeID
            + entity.Room * 2
            + x * z
            + y;
        return (uint)id;
    }

    public void SetLocation<T>(TREntity<T> entity, Location location)
        where T : Enum
    {
        entity.X = location.X;
        entity.Y = location.Y;
        entity.Z = location.Z;
        entity.Room = location.Room;
        entity.Angle = location.Angle;

        // Anything other than -1 means a sloped sector and so the location generator
        // will have picked a suitable angle for it. For flat sectors, spin the entities
        // around randomly for variety.
        if (entity.Angle == -1)
        {
            entity.Angle = (short)(_generator.Next(0, 8) * -TRConsts.Angle45);
        }
    }

    private class Zone
    {
        public int LowIndex { get; set; }
        public int HighIndex { get; set; }
    }
}
