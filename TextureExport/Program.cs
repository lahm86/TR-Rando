using TRLevelControl;
using TRLevelControl.Model;

namespace TextureExport;

class Program
{
    private static readonly List<int> _legacySkips = [306,308,315,323,329,331];
    private const int _skinStart = 302;
    private const int _skinMaxCount = 32;
    private const int _skinExtra = 334;
    private const int _holsterMeshCount = 11;
    private static readonly List<int> _gunMaps = [335, 336, 337, 339];

    public static void Main()
    {
        var level = Legacy.Run(false, false);
        StripGolds(level);
        StripExtra(level);
        StripGuns(level);

        Legacy.Repack(level, true, true);

        var con = new TR2LevelControl();
        con.Write(level, "outfits.tr2");
    }

    private static void StripGolds(TR2Level level)
    {
        foreach (var skip in _legacySkips)
        {
            level.Models.Remove((TR2Type)skip);
        }

        var id = _skinStart;
        for (int i = 0; i < _skinMaxCount; i++)
        {
            var type = (TR2Type)(_skinStart + i);
            if (level.Models.ContainsKey(type))
            {
                level.Models.ChangeKey(type, (TR2Type)id);
                id++;
            }
        }

        level.Models.Remove((TR2Type)341);
        level.Models.Remove((TR2Type)343);
        level.Models.ChangeKey((TR2Type)342, (TR2Type)341);
    }

    private static void DelMesh(TRModel model, int i)
    {
        model.Meshes.RemoveAt(i);
        if (i > 0)
            model.MeshTrees.RemoveAt(i - 1);

        foreach (var frame in model.Animations.SelectMany(a => a.Frames))
            frame.Rotations.RemoveAt(i);
    }

    private static void StripExtra(TR2Level level)
    {
        var model = level.Models[(TR2Type)_skinExtra];

        var del = new[] { 6, 8, 16, 17, 18, 19, 20, 21, 28, 29, 30, 31, 32, 33,
            48, 49, 50, 51, 52, 53, 60, 61, 62, 63, 64, 65, 72, 73, 74, 75, 76, 77 }.ToList();
        del.Sort();
        del.Reverse();
        foreach (var mesh in del)
            DelMesh(model, mesh);
    }

    private static void StripGuns(TR2Level level)
    {
        foreach (var id in _gunMaps)
        {
            var model = level.Models[(TR2Type)id];

            var trees = new[] { 23, 34, 45, 56, 65, 72 };
            foreach (var tree in trees)
                model.MeshTrees[tree - 1].OffsetX += 286;

            var start = _holsterMeshCount * 2;
            for (int i = 0; i < _holsterMeshCount; i++)
            {
                DelMesh(model, start - i);
            }
        }
    }
}
