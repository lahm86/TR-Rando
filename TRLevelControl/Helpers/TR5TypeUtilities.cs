using TRLevelControl.Model;

namespace TRLevelControl.Helpers;

public static class TR5TypeUtilities
{
    private static readonly List<TR5Type> _pickupTypes = new()
    {
        TR5Type.PistolsItem,
        TR5Type.PistolsAmmoItem,
        TR5Type.ShotgunItem,
        TR5Type.ShotgunAmmo1Item,
        TR5Type.ShotgunAmmo2Item,
        TR5Type.UziItem,
        TR5Type.UziAmmoItem,
        TR5Type.HkItem,
        TR5Type.HkAmmoItem,
        TR5Type.RevolverItem,
        TR5Type.RevolverAmmoItem,
        TR5Type.CrossbowItem,
        TR5Type.CrossbowAmmo1Item,
        TR5Type.CrossbowAmmo2Item,
        TR5Type.LasersightItem,
        TR5Type.SilencerItem,
        TR5Type.FlareInvItem,
        TR5Type.BigmediItem,
        TR5Type.SmallmediItem,
    };

    private static readonly List<TR5Type> _doorTypes = new()
    {
        TR5Type.DoorType1,
        TR5Type.DoorType2,
        TR5Type.DoorType3,
        TR5Type.DoorType4,
        TR5Type.DoorType5,
        TR5Type.DoorType6,
        TR5Type.DoorType7,
        TR5Type.DoorType8,
        TR5Type.PushpullDoor1,
        TR5Type.PushpullDoor2,
        TR5Type.KickDoor1,
        TR5Type.KickDoor2,
        TR5Type.UnderwaterDoor,
        TR5Type.DoubleDoors,
        TR5Type.Trapdoor1,
        TR5Type.Trapdoor2,
        TR5Type.Trapdoor3,
        TR5Type.FloorTrapdoor1,
        TR5Type.FloorTrapdoor2,
        TR5Type.CeilingTrapdoor1,
        TR5Type.CeilingTrapdoor2,
        TR5Type.ScalingTrapdoor,
    };

    public static bool IsAnyPickupType(TR5Type type)
    {
        return _pickupTypes.Contains(type)
        || (type >= TR5Type.PuzzleItem1 && type <= TR5Type.BurningTorchItem);
    }

    public static bool IsDoorType(TR5Type type)
        => _doorTypes.Contains(type);
}
