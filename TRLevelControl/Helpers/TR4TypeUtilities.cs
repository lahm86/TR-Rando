using TRLevelControl.Model;

namespace TRLevelControl.Helpers;

public static class TR4TypeUtilities
{
    private static readonly List<TR4Type> _pickupTypes = new()
    {
        TR4Type.PistolsItem,
        TR4Type.PistolsAmmoItem,
        TR4Type.ShotgunItem,
        TR4Type.ShotgunAmmo1Item,
        TR4Type.ShotgunAmmo2Item,
        TR4Type.UziItem,
        TR4Type.UziAmmoItem,
        TR4Type.SixshooterItem,
        TR4Type.SixshooterAmmoItem,
        TR4Type.CrossbowItem,
        TR4Type.CrossbowAmmo1Item,
        TR4Type.CrossbowAmmo2Item,
        TR4Type.CrossbowAmmo3Item,
        TR4Type.LasersightItem,
        TR4Type.GrenadeGunItem,
        TR4Type.GrenadeGunAmmo1Item,
        TR4Type.GrenadeGunAmmo2Item,
        TR4Type.GrenadeGunAmmo3Item,
        TR4Type.FlareInvItem,
        TR4Type.LargeMed,
        TR4Type.SmallMed,
        TR4Type.Waterskin1Empty,
        TR4Type.Waterskin2Empty,
    };

    private static readonly List<TR4Type> _doorTypes = new()
    {
        TR4Type.DoorType1,
        TR4Type.DoorType2,
        TR4Type.DoorType3,
        TR4Type.DoorType4,
        TR4Type.DoorType5,
        TR4Type.DoorType6,
        TR4Type.DoorType7,
        TR4Type.DoorType8,
        TR4Type.PushpullDoor1,
        TR4Type.PushpullDoor2,
        TR4Type.KickDoor1,
        TR4Type.KickDoor2,
        TR4Type.UnderwaterDoor,
        TR4Type.DoubleDoors,
        TR4Type.Trapdoor1,
        TR4Type.Trapdoor2,
        TR4Type.Trapdoor3,
        TR4Type.FloorTrapdoor1,
        TR4Type.FloorTrapdoor2,
        TR4Type.CeilingTrapdoor1,
        TR4Type.CeilingTrapdoor2,
        TR4Type.ScalingTrapdoor,
    };

    public static bool IsAnyPickupType(TR4Type type)
    {
        return _pickupTypes.Contains(type)
        || (type >= TR4Type.PuzzleItem1 && type <= TR4Type.QuestItem6);
    }

    public static bool IsDoorType(TR4Type type)
        => _doorTypes.Contains(type);
}
