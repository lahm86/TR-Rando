using System.Drawing;
using TRImageControl;
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
        var level = Legacy.Run(false);
        StripGolds(level);
        StripExtra(level);
        StripGuns(level);

        CleanupTextures(level);
        Legacy.Repack(level, true);

        WriteLevel(level);
    }

    private static void WriteLevel(TR2Level level)
    {
        var con = new TR2LevelControl();
        con.Write(level, "outfits.tr2");

        var models = new TRDictionary<TR2Type, TRModel>();
        foreach (var (t, m) in level.Models)
            models[t] = m;
        level.Models.Clear();

        // TR1
        {
            var id = 258;
            for (int i = 0; i < _skinMaxCount; i++)
            {
                var type = (TR2Type)(_skinStart + i);
                if (models.TryGetValue(type, out var value))
                {
                    level.Models[(TR2Type)id] = value;
                    id++;
                }
            }
            level.Models[(TR2Type)290] = models[(TR2Type)_skinExtra];
            level.Models[(TR2Type)291] = models[(TR2Type)335];
            level.Models[(TR2Type)292] = models[(TR2Type)338];
            level.Models[(TR2Type)296] = models[(TR2Type)340];
            level.Models[(TR2Type)297] = models[(TR2Type)341];
            con.Write(level, "outfits1.tr2");
            level.Models.Clear();
        }

        // TR3
        {
            var id = 393;
            for (int i = 0; i < _skinMaxCount; i++)
            {
                var type = (TR2Type)(_skinStart + i);
                if (models.TryGetValue(type, out var value))
                {
                    level.Models[(TR2Type)id] = value;
                    id++;
                }
            }
            level.Models[(TR2Type)425] = models[(TR2Type)_skinExtra];
            level.Models[(TR2Type)426] = models[(TR2Type)337];
            level.Models[(TR2Type)427] = models[(TR2Type)338];
            level.Models[(TR2Type)433] = models[(TR2Type)340];
            level.Models[(TR2Type)434] = models[(TR2Type)341];
            con.Write(level, "outfits3.tr2");
            level.Models.Clear();
        }

        // TR4
        {
            var id = 465;
            for (int i = 0; i < _skinMaxCount; i++)
            {
                var type = (TR2Type)(_skinStart + i);
                if (models.TryGetValue(type, out var value))
                {
                    level.Models[(TR2Type)id] = value;
                    id++;
                }
            }
            level.Models[(TR2Type)497] = models[(TR2Type)_skinExtra];
            level.Models[(TR2Type)498] = models[(TR2Type)339];
            level.Models[(TR2Type)499] = models[(TR2Type)338];
            level.Models[(TR2Type)501] = models[(TR2Type)340];
            level.Models[(TR2Type)502] = models[(TR2Type)341];
            con.Write(level, "outfits4.tr2");
            level.Models.Clear();
        }
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

    private class TexInfo
    {
        public TRObjectTexture Obj { get; set; }
        public TRImage Img { get; set; }
    }

    private class ImageReplacement
    {
        public int ImageIndex { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
    }

    private static void CleanupTextures(TR2Level level)
    {
        List<TexInfo> texInfos = [.. level.ObjectTextures.Select(t => new TexInfo
        {
            Obj = t.Clone(),
            Img = new TRImage(level.Images16[t.Atlas].Pixels).Export(t.Bounds),
        })];

        NormalizeSolidTextures(texInfos);

        var imageRemappings = new Dictionary<int, ImageReplacement>();
        var imageById = new Dictionary<string, int>();

        for (int i = 0; i < texInfos.Count; i++)
        {
            string id = texInfos[i].Img.GenerateID();

            if (imageById.TryGetValue(id, out int existing))
            {
                imageRemappings[i] = new()
                {
                    ImageIndex = existing,
                    OffsetX = 0,
                    OffsetY = 0,
                };
            }
            else
            {
                imageById.Add(id, i);
            }
        }

        Console.WriteLine($"Exact image duplicates: {imageRemappings.Count}");

        int containedCount = 0;
        var candidates = Enumerable
            .Range(0, texInfos.Count)
            .Where(i => !imageRemappings.ContainsKey(i))
            .OrderByDescending(i =>
                texInfos[i].Img.Size.Width *
                texInfos[i].Img.Size.Height)
            .ToList();

        for (int i = 0; i < texInfos.Count; i++)
        {
            if (imageRemappings.ContainsKey(i))
                continue;

            var image = texInfos[i].Img;
            int imageArea = image.Size.Width * image.Size.Height;

            foreach (int candidate in candidates)
            {
                if (candidate == i)
                    continue;

                var container = texInfos[candidate].Img;
                int containerArea = container.Size.Width * container.Size.Height;

                if (containerArea < imageArea)
                    break;

                if (container.Size.Width < image.Size.Width ||
                    container.Size.Height < image.Size.Height)
                {
                    continue;
                }

                if (!Contains(container, image, out int offsetX, out int offsetY))
                {
                    continue;
                }

                imageRemappings[i] = new()
                {
                    ImageIndex = candidate,
                    OffsetX = offsetX,
                    OffsetY = offsetY,
                };

                containedCount++;
                Console.WriteLine(
                    $"Contained image: {i} -> {candidate} " +
                    $"at ({offsetX}, {offsetY})");
                break;
            }
        }

        Console.WriteLine(
            $"Contained images: {containedCount}");

        foreach (int index in imageRemappings.Keys.ToList())
        {
            imageRemappings[index] =
                ResolveImageReplacement(index, imageRemappings);
        }

        foreach (var (source, replacement) in imageRemappings)
        {
            var sourceInfo = texInfos[source];
            var destinationInfo = texInfos[replacement.ImageIndex];

            var newBounds = new Rectangle(
                destinationInfo.Obj.Bounds.X + replacement.OffsetX,
                destinationInfo.Obj.Bounds.Y + replacement.OffsetY,
                sourceInfo.Img.Size.Width,
                sourceInfo.Img.Size.Height);

            ValidateBounds(source, replacement.ImageIndex, destinationInfo.Obj.Atlas, newBounds);

            sourceInfo.Obj.Atlas = destinationInfo.Obj.Atlas;
            sourceInfo.Obj.Bounds = newBounds;
        }
        
        var objectTextureByKey = new Dictionary<ObjectTextureKey, int>();
        var objectTextureRemappings = new Dictionary<int, int>();
        var newObjectTextures = new List<TRObjectTexture>();

        for (int i = 0; i < texInfos.Count; i++)
        {
            var obj = texInfos[i].Obj;
            var key = new ObjectTextureKey(obj);

            if (objectTextureByKey.TryGetValue(key, out int existing))
            {
                objectTextureRemappings[i] = existing;
                continue;
            }

            int newIndex = newObjectTextures.Count;

            objectTextureByKey.Add(key, newIndex);
            objectTextureRemappings[i] = newIndex;
            newObjectTextures.Add(obj);
        }

        Console.WriteLine(
            $"ObjectTextures: {level.ObjectTextures.Count} -> " +
            $"{newObjectTextures.Count}");

        foreach (var face in level.DistinctMeshes.SelectMany(m => m.TexturedFaces))
        {
            if (!objectTextureRemappings.TryGetValue(
                face.Texture,
                out int replacement))
            {
                throw new InvalidOperationException(
                    $"No ObjectTexture remapping exists for index {face.Texture}.");
            }

            face.Texture = (ushort)replacement;
        }

        level.ObjectTextures = newObjectTextures;

        if (level.DistinctMeshes
            .SelectMany(m => m.TexturedFaces)
            .Any(f => f.Texture >= level.ObjectTextures.Count))
        {
            throw new InvalidOperationException(
                "Texture remapping produced an invalid ObjectTexture index.");
        }
    }

    private static void NormalizeSolidTextures(List<TexInfo> texInfos)
    {
        foreach (TexInfo texInfo in texInfos)
        {
            if (!IsSolid(texInfo.Img, out uint pixel))
                continue;

            texInfo.Img = CreateSolidImage(pixel);

            texInfo.Obj.Bounds = new Rectangle(
                texInfo.Obj.Bounds.Location,
                texInfo.Img.Size);
        }
    }

    private readonly record struct ObjectTextureKey(
        ushort Atlas,
        Rectangle Bounds,
        TRBlendingMode BlendingMode,
        bool HasTriangleVertex,
        TRUVMode UVMode
    ) {
        public ObjectTextureKey(TRObjectTexture obj)
            : this(obj.Atlas,
                obj.Bounds,
                obj.BlendingMode,
                obj.HasTriangleVertex,
                obj.UVMode)
        { }
    }

    private static ImageReplacement ResolveImageReplacement(int index,
        Dictionary<int, ImageReplacement> remappings)
    {
        int current = index;
        int offsetX = 0;
        int offsetY = 0;

        while (remappings.TryGetValue(current, out ImageReplacement replacement))
        {
            offsetX += replacement.OffsetX;
            offsetY += replacement.OffsetY;
            current = replacement.ImageIndex;
        }

        return new()
        {
            ImageIndex = current,
            OffsetX = offsetX,
            OffsetY = offsetY,
        };
    }

    private static void ValidateBounds(int sourceIndex, int destinationIndex,
        ushort atlas, Rectangle bounds)
    {
        if (bounds.Width <= 0 ||
            bounds.Height <= 0 ||
            bounds.X < 0 ||
            bounds.Y < 0 ||
            bounds.Right > 256 ||
            bounds.Bottom > 256)
        {
            throw new InvalidOperationException(
                $"Invalid texture bounds generated while remapping " +
                $"{sourceIndex} -> {destinationIndex}: " +
                $"Atlas={atlas}, Bounds={bounds}");
        }
    }

    private static bool IsSolid(TRImage image, out uint pixel)
    {
        pixel = image.Pixels[0];
        for (int i = 1; i < image.Pixels.Length; i++)
        {
            if (image.Pixels[i] != pixel)
                return false;
        }
        return true;
    }

    private static TRImage CreateSolidImage(uint pixel)
    {
        const int size = 4;
        return new TRImage(new Size(size, size),
            Enumerable.Repeat(pixel, size * size).ToArray());
    }

    private static bool Contains(TRImage outer, TRImage inner, out int offsetX, out int offsetY)
    {
        int outerWidth = outer.Size.Width;
        int outerHeight = outer.Size.Height;

        int innerWidth = inner.Size.Width;
        int innerHeight = inner.Size.Height;

        for (int y = 0; y <= outerHeight - innerHeight; y++)
        {
            for (int x = 0; x <= outerWidth - innerWidth; x++)
            {
                bool match = true;

                for (int innerY = 0; innerY < innerHeight; innerY++)
                {
                    int outerIndex =
                        (y + innerY) * outerWidth + x;

                    int innerIndex =
                        innerY * innerWidth;

                    for (int innerX = 0; innerX < innerWidth; innerX++)
                    {
                        if (outer.Pixels[outerIndex + innerX] !=
                            inner.Pixels[innerIndex + innerX])
                        {
                            match = false;
                            break;
                        }
                    }

                    if (!match)
                        break;
                }

                if (match)
                {
                    offsetX = x;
                    offsetY = y;
                    return true;
                }
            }
        }

        offsetX = 0;
        offsetY = 0;

        return false;
    }
}
