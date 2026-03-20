using RectanglePacker.Organisation;
using System.Diagnostics;
using System.Drawing;
using TextureExport.Types;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl;
using TRLevelControl.Helpers;
using TRLevelControl.Model;

namespace TextureExport;

class Program
{
    enum Mode
    {
        Png, Html, Segments, Faces, Boxes, Dependencies, Dds, TexInfo,
    }

    public static readonly TR1LevelControl _reader1 = new();
    public static readonly TR2LevelControl _reader2 = new();
    public static readonly TR3LevelControl _reader3 = new();

    public static void ConvertFlatFaces(TR1Level level, IEnumerable<TRModel> models)
    {
        var sourcePalette = level.Palette.Select(c => c.ToTR1Color()).ToList();
        var ids = models.SelectMany(m => m.Meshes)
            .SelectMany(m => m.ColouredFaces)
            .Select(f => f.Texture)
            .Distinct().ToList();

        var map = new Dictionary<Color, int>();
        var regions = new List<TRTextileRegion>();
        int i = 0;
        foreach (var id in ids)
        {
            var img = new TRImage(8, 8);
            img.Fill(sourcePalette[id]);
            map[sourcePalette[id]] = level.ObjectTextures.Count + regions.Count;

            var texInfo = new TRObjectTexture(0, 0, 8, 8);
            regions.Add(new()
            {
                Image = img,
                Bounds = texInfo.Bounds,
                Segments =
                [
                    new()
                    {
                        Index = i++,
                        Texture = texInfo,
                    }
                ]
            });
        }

        var packer = new TR1TexturePacker(level);
        packer.AddRectangles(regions);
        packer.Pack(true);

        ushort FindTex(ushort pal)
        {
            var old = level.Palette[pal].ToTR1Color();
            return (ushort)map[old];
        }

        level.ObjectTextures.AddRange(regions.Select(r => r.Segments.First().Texture as TRObjectTexture));
        models.SelectMany(m => m.Meshes)
            .ToList().ForEach(m =>
            {
                m.ColouredRectangles.ForEach(f => f.Texture = FindTex(f.Texture));
                m.ColouredTriangles.ForEach(f => f.Texture = FindTex(f.Texture));
                m.TexturedRectangles.AddRange(m.ColouredRectangles);
                m.TexturedTriangles.AddRange(m.ColouredTriangles);
                m.ColouredRectangles.Clear();
                m.ColouredTriangles.Clear();
            });
    }

    public static void ConvertFlatFaces(TR2Level level, IEnumerable<TRModel> models)
    {
        var sourcePalette = level.Palette16.Select(c => c.ToTR1Color()).ToList();
        var ids = models.SelectMany(m => m.Meshes)
            .SelectMany(m => m.ColouredFaces)
            .Select(f => f.Texture >> 8)
            .Distinct().ToList();

        var map = new Dictionary<Color, int>();
        var regions = new List<TRTextileRegion>();
        int i = 0;
        foreach (var id in ids)
        {
            var img = new TRImage(8, 8);
            img.Fill(sourcePalette[id]);
            map[sourcePalette[id]] = level.ObjectTextures.Count + regions.Count;

            var texInfo = new TRObjectTexture(0, 0, 8, 8);
            regions.Add(new()
            {
                Image = img,
                Bounds = texInfo.Bounds,
                Segments =
                [
                    new()
                    {
                        Index = i++,
                        Texture = texInfo,
                    }
                ]
            });
        }

        var packer = new TR2TexturePacker(level);
        packer.AddRectangles(regions);
        packer.Pack(true);

        ushort FindTex(ushort pal)
        {
            var old = level.Palette16[pal >> 8].ToTR1Color();
            return (ushort)map[old];
        }

        level.ObjectTextures.AddRange(regions.Select(r => r.Segments.First().Texture as TRObjectTexture));
        models.SelectMany(m => m.Meshes)
            .ToList().ForEach(m =>
            {
                m.ColouredRectangles.ForEach(f => f.Texture = FindTex(f.Texture));
                m.ColouredTriangles.ForEach(f => f.Texture = FindTex(f.Texture));
                m.TexturedRectangles.AddRange(m.ColouredRectangles);
                m.TexturedTriangles.AddRange(m.ColouredTriangles);
                m.ColouredRectangles.Clear();
                m.ColouredTriangles.Clear();
            });
    }

    public static void ConvertFlatFaces(TR3Level level, IEnumerable<TRModel> models)
    {
        var sourcePalette = level.Palette16.Select(c => c.ToTR1Color()).ToList();
        var ids = models.SelectMany(m => m.Meshes)
            .SelectMany(m => m.ColouredFaces)
            .Select(f => f.Texture >> 8)
            .Distinct().ToList();

        var map = new Dictionary<Color, int>();
        var regions = new List<TRTextileRegion>();
        int i = 0;
        foreach (var id in ids)
        {
            var img = new TRImage(8, 8);
            img.Fill(sourcePalette[id]);
            map[sourcePalette[id]] = level.ObjectTextures.Count + regions.Count;

            var texInfo = new TRObjectTexture(0, 0, 8, 8);
            regions.Add(new()
            {
                Image = img,
                Bounds = texInfo.Bounds,
                Segments =
                [
                    new()
                    {
                        Index = i++,
                        Texture = texInfo,
                    }
                ]
            });
        }

        var packer = new TR3TexturePacker(level);
        packer.AddRectangles(regions);
        packer.Pack(true);

        ushort FindTex(ushort pal)
        {
            var old = level.Palette16[pal >> 8].ToTR1Color();
            return (ushort)map[old];
        }

        level.ObjectTextures.AddRange(regions.Select(r => r.Segments.First().Texture as TRObjectTexture));
        models.SelectMany(m => m.Meshes)
            .ToList().ForEach(m =>
            {
                m.ColouredRectangles.ForEach(f => f.Texture = FindTex(f.Texture));
                m.ColouredTriangles.ForEach(f => f.Texture = FindTex(f.Texture));
                m.TexturedRectangles.AddRange(m.ColouredRectangles);
                m.TexturedTriangles.AddRange(m.ColouredTriangles);
                m.ColouredRectangles.Clear();
                m.ColouredTriangles.Clear();
            });
    }

    public static TRModel MakeBaseModel()
    {
        var model = new TRModel();
        model.Meshes.Add(new() { Normals = [] });
        model.Animations.Add(new()
        {
            FrameRate = 1,
            Accel = new(),
            Speed = new(),
        });
        model.Animations[0].Frames.Add(new()
        {
            OffsetY = -458,
            Bounds = new(),
            Rotations = [new()],
        });

        return model;
    }

    public static void Import(TR2Level targetLevel, TRModel targetModel, TR1Level sourceLevel, TRModel sourceModel, TRMesh head)
    {
        ConvertFlatFaces(sourceLevel, [sourceModel]);

        var packer1 = new TR1TexturePacker(sourceLevel, 1024);
        var regions = packer1.GetMeshRegions(sourceModel.Meshes)
            .SelectMany(v => v.Value);
        var originalInfos = sourceLevel.ObjectTextures.ToList();

        var packer2 = new TR2TexturePacker(targetLevel, 1024);
        packer2.AddRectangles(regions);
        packer2.Pack(true);
        targetLevel.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        sourceModel.Meshes.SelectMany(m => m.TexturedFaces)
            .ToList()
            .ForEach(f =>
            {
                f.Texture = (ushort)targetLevel.ObjectTextures.IndexOf(originalInfos[f.Texture]);
            });

        if (head == null)
        {
            targetModel.Meshes.AddRange(sourceModel.Meshes.Select(m => m.Clone()));
        }
        else
        {
            targetModel.Meshes.AddRange(sourceModel.Meshes.GetRange(0, 14).Select(m => m.Clone()));
            targetModel.Meshes.Add(head.Clone());
        }
    }

    public static void ImportSprite(TR2Level targetLevel, TR1Level sourceLevel, TR1Type type, TR2Type targetType)
    {
        var packer = new TR1TexturePacker(sourceLevel);
        var seq = sourceLevel.Sprites[type];
        var regions = packer.GetSpriteRegions(seq)
            .SelectMany(v => v.Value);

        var packer2 = new TR2TexturePacker(targetLevel, 1024);
        packer2.AddRectangles(regions);
        packer2.Pack(true);

        targetLevel.Sprites[targetType] = seq;
    }

    public static void ImportSprite(TR2Level targetLevel, TR2Level sourceLevel, TR2Type type, TR2Type targetType)
    {
        var packer = new TR2TexturePacker(sourceLevel);
        var seq = sourceLevel.Sprites[type];
        var regions = packer.GetSpriteRegions(seq)
            .SelectMany(v => v.Value);

        packer = new(targetLevel, 1024);
        packer.AddRectangles(regions);
        packer.Pack(true);

        targetLevel.Sprites[targetType] = seq;
    }

    public static void Import(TR2Level targetLevel, TRModel targetModel, TR2Level sourceLevel, TRModel sourceModel, TRMesh head)
    {
        ConvertFlatFaces(sourceLevel, [sourceModel]);

        var packer = new TR2TexturePacker(sourceLevel);
        var regions = packer.GetMeshRegions(sourceModel.Meshes)
            .SelectMany(v => v.Value);
        var originalInfos = sourceLevel.ObjectTextures.ToList();

        packer = new(targetLevel, 1024);
        packer.AddRectangles(regions);
        packer.Pack(true);
        targetLevel.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        sourceModel.Meshes.SelectMany(m => m.TexturedFaces)
            .ToList()
            .ForEach(f =>
            {
                f.Texture = (ushort)targetLevel.ObjectTextures.IndexOf(originalInfos[f.Texture]);
            });

        if (head == null)
        {
            targetModel.Meshes.AddRange(sourceModel.Meshes.Select(m => m.Clone()));
        }
        else
        {
            targetModel.Meshes.AddRange(sourceModel.Meshes.GetRange(0, 14).Select(m => m.Clone()));
            targetModel.Meshes.Add(head.Clone());
        }
    }

    public static void Import(TR2Level targetLevel, TRModel targetModel, TR3Level sourceLevel, TRModel sourceModel, TRMesh head)
    {
        ConvertFlatFaces(sourceLevel, [sourceModel]);

        var packer3 = new TR3TexturePacker(sourceLevel);
        var regions = packer3.GetMeshRegions(sourceModel.Meshes)
            .SelectMany(v => v.Value);
        var originalInfos = sourceLevel.ObjectTextures.ToList();

        var packer2 = new TR2TexturePacker(targetLevel, 1024);
        packer2.AddRectangles(regions);
        packer2.Pack(true);
        targetLevel.ObjectTextures.AddRange(regions.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        sourceModel.Meshes.SelectMany(m => m.TexturedFaces)
            .ToList()
            .ForEach(f =>
            {
                f.Texture = (ushort)targetLevel.ObjectTextures.IndexOf(originalInfos[f.Texture]);
            });

        if (head == null)
        {
            targetModel.Meshes.AddRange(sourceModel.Meshes.Select(m => m.Clone()));
        }
        else
        {
            targetModel.Meshes.AddRange(sourceModel.Meshes.GetRange(0, 14).Select(m => m.Clone()));
            targetModel.Meshes.Add(head.Clone());
        }
    }

    public static TR2Level MakeBaseLevel()
    {
        var baseLevel = _reader2.Read(@"F:\tomp\all levels\tr2\wall.tr2");
        baseLevel.AnimatedTextures.Clear();
        baseLevel.Models.Clear();
        baseLevel.Rooms.Clear();
        baseLevel.Sprites.Clear();
        baseLevel.StaticMeshes.Clear();
        baseLevel.SoundEffects.Clear();
        baseLevel.SoundSources.Clear();
        baseLevel.Boxes.Clear();
        baseLevel.Entities.Clear();
        baseLevel.FloorData = new(TRGameVersion.TR2, null);
        baseLevel.ObjectTextures.Clear();
        baseLevel.Cameras.Clear();
        baseLevel.CinematicFrames.Clear();
        baseLevel.Images16 = [new() { Pixels = new ushort[256 * 256] }];
        baseLevel.Images8 = [new() { Pixels = new byte[256 * 256] }];
        return baseLevel;
    }

    public static void Repack(TR2Level level)
    {
        var packer = new TR2TexturePacker(level);
        var regions = packer.GetMeshRegions(level.DistinctMeshes);
        new TRImageDeduplicator().Deduplicate(regions);
        var rects = regions.SelectMany(v => v.Value).ToList();
        foreach (var sprite in level.Sprites.Values)
        {
            rects.AddRange(packer.GetSpriteRegions(sprite).SelectMany(v => v.Value));
        }

        level.Images16 = [new() { Pixels = new ushort[256 * 256] }];
        level.Images8 = [new() { Pixels = new byte[256 * 256] }];

        var originalInfos = level.ObjectTextures.ToList();
        level.ObjectTextures.Clear();

        var sprites = new TRDictionary<TR2Type, TRSpriteSequence>();
        foreach (var (type, seq) in level.Sprites)
            sprites[type] = seq;
        level.Sprites.Clear();

        packer = new(level, 1024);
        packer.Options.OrderMode = PackingOrderMode.Area;
        packer.AddRectangles(rects);
        packer.Pack(true);

        level.ObjectTextures.AddRange(rects.SelectMany(r => r.Segments.Select(s => s.Texture as TRObjectTexture)));
        level.ObjectTextures.RemoveAll(o => o is null);
        level.DistinctMeshes.SelectMany(m => m.TexturedFaces)
            .ToList()
            .ForEach(f =>
            {
                f.Texture = (ushort)level.ObjectTextures.IndexOf(originalInfos[f.Texture]);
            });

        level.Sprites = sprites;

        // Final texinfo dedupe
        static object TexInfoKey(TRObjectTexture t) => new
        {
            t.Bounds,
            t.Atlas,
            t.HasTriangleVertex,
            t.UVMode,
            t.BlendingMode,
        };

        var indexedTextures = level.ObjectTextures
            .Select((tex, index) => (tex, index));
        var groups = indexedTextures
            .GroupBy(x => TexInfoKey(x.tex))
            .ToList();

        var remap = groups
            .SelectMany((g, newIndex) =>
                g.Select(x => (oldIndex: x.index, newIndex)))
            .ToDictionary(x => (ushort)x.oldIndex, x => (ushort)x.newIndex);

        level.ObjectTextures = [.. groups.Select(g => g.First().tex)];
        level.DistinctMeshes
            .SelectMany(m => m.TexturedFaces)
            .Where(f => remap.ContainsKey(f.Texture))
            .ToList()
            .ForEach(f => f.Texture = remap[f.Texture]);
    }

    const uint _pop = 1;
    const uint _push = 2;
    const uint _read = 3;

    static void FixTR1Pistols(TRModel model, TR1Level level)
    {
        var mesh = model.Meshes[0];
        foreach (var v in new[] { 19, 23 })
        {
            mesh.Vertices[v].Y -= 2;
            mesh.Vertices[v].Z = 17;
        }
        foreach (var v in new[] { 18, 22 })
        {
            mesh.Vertices[v].Y -= 2;
            mesh.Vertices[v].Z = 21;
        }

        mesh = model.Meshes[1];
        foreach (var v in new[] { 19, 23 })
        {
            mesh.Vertices[v].Y -= 2;
            mesh.Vertices[v].Z = 18;
        }
        foreach (var v in new[] { 18, 22 })
        {
            mesh.Vertices[v].Y -= 2;
            mesh.Vertices[v].Z = 22;
        }

        var actualGun = level.Models[TR1Type.Pistols_M_H];
        model.Meshes[0].TexturedRectangles[15].Texture = actualGun.Meshes[1].TexturedRectangles[10].Texture;
        model.Meshes[0].TexturedRectangles[11].Texture = actualGun.Meshes[1].TexturedRectangles[14].Texture;
        model.Meshes[0].TexturedRectangles[14].Texture = actualGun.Meshes[1].TexturedRectangles[13].Texture;
        model.Meshes[0].TexturedRectangles[13].Texture = actualGun.Meshes[1].TexturedRectangles[12].Texture;

        model.Meshes[1].TexturedRectangles[15].Texture = actualGun.Meshes[2].TexturedRectangles[14].Texture;
        model.Meshes[1].TexturedRectangles[11].Texture = actualGun.Meshes[2].TexturedRectangles[10].Texture;
        model.Meshes[1].TexturedRectangles[14].Texture = actualGun.Meshes[2].TexturedRectangles[13].Texture;
        model.Meshes[1].TexturedRectangles[13].Texture = actualGun.Meshes[2].TexturedRectangles[12].Texture;
        model.Meshes[1].TexturedRectangles[14].Rotate(2);
        model.Meshes[1].TexturedRectangles[13].Rotate(2);
    }

    static void FixTR1Magnums(TRModel model, TR1Level level)
    {
        var mesh = model.Meshes[0];
        foreach (var v in new[] { 19, 23 })
        {
            //mesh.Vertices[v].Y -= 2;
            mesh.Vertices[v].Z = 17;
        }
        foreach (var v in new[] { 18, 22 })
        {
            //mesh.Vertices[v].Y -= 2;
            mesh.Vertices[v].Z = 21;
        }

        mesh = model.Meshes[1];
        foreach (var v in new[] { 19, 23 })
        {
            mesh.Vertices[v].Z = 18;
        }
        foreach (var v in new[] { 18, 22 })
        {
            mesh.Vertices[v].Z = 22;
        }

        var actualGun = level.Models[TR1Type.Magnums_M_H];
        var vvs = new ushort[] { 19, 23, 16, 20 };
        var ff = actualGun.Meshes[1].TexturedRectangles.FindIndex(f => f.Vertices.All(vvs.Contains));//14
        vvs = [19,23,16,20];
        ff = model.Meshes[0].TexturedRectangles.FindIndex(f => f.Vertices.All(vvs.Contains));//11
        model.Meshes[0].TexturedRectangles[15].Texture = actualGun.Meshes[1].TexturedRectangles[11].Texture;
        model.Meshes[0].TexturedRectangles[11].Texture = actualGun.Meshes[1].TexturedRectangles[15].Texture;
        model.Meshes[0].TexturedRectangles[14].Texture = actualGun.Meshes[1].TexturedRectangles[14].Texture;
        model.Meshes[0].TexturedRectangles[13].Texture = actualGun.Meshes[1].TexturedRectangles[13].Texture;

        model.Meshes[1].TexturedRectangles[15].Texture = actualGun.Meshes[1].TexturedRectangles[15].Texture;
        model.Meshes[1].TexturedRectangles[11].Texture = actualGun.Meshes[1].TexturedRectangles[11].Texture;
        model.Meshes[1].TexturedRectangles[14].Texture = actualGun.Meshes[1].TexturedRectangles[14].Texture;
        model.Meshes[1].TexturedRectangles[13].Texture = actualGun.Meshes[1].TexturedRectangles[13].Texture;
        model.Meshes[1].TexturedRectangles[14].Rotate(2);
        model.Meshes[1].TexturedRectangles[13].Rotate(2);
    }

    static void FixTR1Uzis(TRModel model, TR1Level level)
    {
        var actualGun = level.Models[TR1Type.Uzis_M_H];
        for (int i = 0; i < 2; i++)
        {
            var mesh = model.Meshes[i];
            foreach (var v in new[] { 19, 23 })
            {
                mesh.Vertices[v].Y -= 2;
                mesh.Vertices[v].Z = 17;
            }
            foreach (var v in new[] { 18, 22 })
            {
                mesh.Vertices[v].Y -= 2;
                mesh.Vertices[v].Z = 21;
            }
            foreach (var v in i == 0 ? [32, 35, 36, 39] : new[] { 27, 31, 24, 28 })
            {
                mesh.Vertices[v].Z++;
            }

            mesh.TexturedRectangles[15].Texture = actualGun.Meshes[1].TexturedRectangles[6].Texture;
            mesh.TexturedRectangles[11].Texture = actualGun.Meshes[1].TexturedRectangles[6].Texture;
            mesh.TexturedRectangles[14].Texture = actualGun.Meshes[1].TexturedRectangles[9].Texture;
            mesh.TexturedRectangles[13].Texture = actualGun.Meshes[1].TexturedRectangles[9].Texture;
            if (i == 0)
            {
                mesh.TexturedRectangles[14].Rotate(3);
                mesh.TexturedRectangles[13].Rotate(3);
            }
        }
    }

    static void FixTR1Shotgun(TRModel model, TR1Level level)
    {
        var mesh = model.Meshes[0];
        mesh.ColouredRectangles.RemoveAt(2);
        foreach (var v in new[] { 18, 19, 22, 23 })
        {
            mesh.Vertices[v].Y += 5;
            mesh.Vertices[v].Z = 19;
        }

        var actualGun = level.Models[TR1Type.Shotgun_M_H];
        mesh.TexturedRectangles[14].Texture = actualGun.Meshes[0].TexturedRectangles[10].Texture;
        mesh.TexturedRectangles[10].Texture = actualGun.Meshes[0].TexturedRectangles[10].Texture;
        mesh.TexturedRectangles[12].Texture = actualGun.Meshes[0].TexturedRectangles[7].Texture;
        mesh.TexturedRectangles[13].Texture = actualGun.Meshes[0].TexturedRectangles[7].Texture;
    }

    static void FixM16(TRModel model, TR2Level level)
    {
        var mesh = model.Meshes[0];
        var actualGun = level.Models[TR2Type.M16_M_H];
        
        for (int i = 5; i >= 2; i--)
        {
            mesh.TexturedTriangles.RemoveAt(i);
        }

        mesh.TexturedRectangles[17].Vertices = [22, 25, 23, 20];
        mesh.TexturedRectangles[15].Vertices = [12, 8, 21, 24];
        mesh.TexturedRectangles[17].Texture = actualGun.Meshes[0].TexturedRectangles[9].Texture;
        mesh.TexturedRectangles[15].Texture = actualGun.Meshes[0].TexturedRectangles[9].Texture;
        mesh.TexturedRectangles[14].Texture = actualGun.Meshes[0].TexturedRectangles[9].Texture;

        mesh.TexturedRectangles.Add(new()
        {
            Type = TRFaceType.Rectangle,
            Texture = actualGun.Meshes[0].TexturedRectangles[9].Texture,
            Vertices = [21, 8, 22, 20],
        });
        mesh.TexturedRectangles.Add(new()
        {
            Type = TRFaceType.Rectangle,
            Texture = actualGun.Meshes[0].TexturedRectangles[9].Texture,
            Vertices = [12, 24, 23, 25],
        });

        foreach (var v in new[] { 21, 24 })
        {
            mesh.Vertices[v].Y -= 10;
        }

        mesh.Vertices[20].X -= 4;
        mesh.Vertices[21].X -= 4;
        mesh.Vertices.Add(mesh.Vertices[8].Clone());
        mesh.Vertices.Add(mesh.Vertices[22].Clone());
        mesh.Normals.Add(mesh.Normals[8].Clone());
        mesh.Normals.Add(mesh.Normals[22].Clone());
        mesh.Vertices[^1].X -= 4;
        mesh.Vertices[^2].X -= 4;
        mesh.TexturedRectangles[^2].Vertices[1] = (ushort)(mesh.Vertices.Count - 2);
        mesh.TexturedRectangles[^2].Vertices[2] = (ushort)(mesh.Vertices.Count - 1);

        mesh.TexturedRectangles[15].Vertices[1] = (ushort)(mesh.Vertices.Count - 2);
        mesh.TexturedRectangles[17].Vertices[0] = (ushort)(mesh.Vertices.Count - 1);

        mesh.TexturedRectangles.Add(new()
        {
            Type = TRFaceType.Rectangle,
            Texture = actualGun.Meshes[0].TexturedRectangles[9].Texture,
            Vertices = [8, 22, 25, 12],
        });
    }

    static void FixGrenade(TRModel model, TR2Level level)
    {
        var mesh = model.Meshes[0];
        var actualGun = level.Models[TR2Type.GrenadeLauncher_M_H];

        mesh.TexturedRectangles[18].Texture = actualGun.Meshes[0].TexturedRectangles[13].Texture;
        mesh.TexturedRectangles[19].Texture = actualGun.Meshes[0].TexturedRectangles[14].Texture;
        mesh.TexturedRectangles[20].Texture = actualGun.Meshes[0].TexturedRectangles[15].Texture;
        mesh.TexturedRectangles[21].Texture = actualGun.Meshes[0].TexturedRectangles[16].Texture;
        mesh.TexturedRectangles[14].Texture = actualGun.Meshes[0].TexturedRectangles[16].Texture;
        mesh.TexturedRectangles[21].Rotate(1);
        mesh.TexturedRectangles[14].Rotate(1);

        var remap = new Dictionary<ushort, ushort>
        {
            [28] = 23,
            [29] = 13,
            [31] = 20,
            [30] = 8,
        };
        foreach (var (src, tar) in remap)
        {
            var vertA = mesh.Vertices[src];
            var vertB = mesh.Vertices[tar];
            vertA.Y = vertB.Y;
            vertA.Z = vertB.Z;
            if (src == 28 || src == 29)
            {
                vertA.X -= 1;
            }
            else
            {
                vertA.X += 4;
            }
        }

        foreach (var v in new[] { 24,25,26,27 })
        {
            if (v % 2 == 0)
            {
                mesh.Vertices[v].X += 4;
            }
            else
            {
                mesh.Vertices[v].X -= 1;
            }
        }

        mesh.TexturedRectangles.Add(new()
        {
            Type = TRFaceType.Rectangle,
            Texture = mesh.TexturedRectangles[10].Texture,
            Vertices = [8,20,23,13],
        });
    }

    static void FixHarpoon(TRModel model, TR2Level level)
    {
        var mesh = model.Meshes[0];
        foreach (var v in new[] { 16, 20 })
        {
            mesh.Vertices[v].Y += 5;
            mesh.Vertices[v].Z = 20;
        }
        foreach (var v in new[] { 23, 19 })
        {
            mesh.Vertices[v].Y += 5;
            mesh.Vertices[v].Z = 14;
        }
        foreach (var v in new[] { 16, 17, 18, 19 })
        {
            mesh.Vertices[v].X -= 1;
        }
        foreach (var v in new[] { 20, 21, 22, 23 })
        {
            mesh.Vertices[v].X += 5;
        }
    }

    static void FixFlare(TRModel model, TR2Level level)
    {
        var flare = level.Models[TR2Type.Flare_H].Meshes[0];
        model.Meshes[0].Vertices[14].Z = (short)(model.Meshes[0].Vertices[10].Z - 59);
        model.Meshes[0].Vertices[15].Z = (short)(model.Meshes[0].Vertices[11].Z - 59);
        model.Meshes[0].Vertices[13].Z = (short)(model.Meshes[0].Vertices[9].Z - 59);
        model.Meshes[0].Vertices[12].Z = (short)(model.Meshes[0].Vertices[8].Z - 59);

        for (int i = 6; i < model.Meshes[0].TexturedRectangles.Count; i++)
        {
            model.Meshes[0].TexturedRectangles[i].Texture = flare.TexturedRectangles[1].Texture;
        }
        model.Meshes[0].TexturedRectangles[5].Texture = flare.TexturedRectangles[0].Texture;
        model.Meshes[0].TexturedRectangles.Add(new()
        {
            Type = TRFaceType.Rectangle,
            Texture = flare.TexturedRectangles[0].Texture,
            Vertices = [14, 15, 12, 13],
        });
    }

    static void FixTR2Pistols(TRModel model, TR2Level level)
    {
        var mesh = model.Meshes[0];
        foreach (var v in new[] { 19, 23 })
        {
            mesh.Vertices[v].Y += 7;
            mesh.Vertices[v].Z = 17;
        }
        foreach (var v in new[] { 18, 22 })
        {
            mesh.Vertices[v].Y += 7;
            mesh.Vertices[v].Z = 21;
        }

        mesh = model.Meshes[1];
        foreach (var v in new[] { 19, 23 })
        {
            mesh.Vertices[v].Y += 7;
            mesh.Vertices[v].Z = 18;
        }
        foreach (var v in new[] { 18, 22 })
        {
            mesh.Vertices[v].Y += 7;
            mesh.Vertices[v].Z = 22;
        }

        var actualGun = level.Models[TR2Type.Pistols_M_H];
        for (int i = 0; i < 2; i++)
        {
            mesh = model.Meshes[i];
            mesh.TexturedRectangles[16].Texture = actualGun.Meshes[1].TexturedRectangles[10].Texture;
            mesh.TexturedRectangles[12].Texture = actualGun.Meshes[1].TexturedRectangles[6].Texture;
            mesh.TexturedRectangles[15].Texture = actualGun.Meshes[1].TexturedRectangles[9].Texture;
            mesh.TexturedRectangles[14].Texture = actualGun.Meshes[1].TexturedRectangles[8].Texture;
        }
    }

    static void FixTR2Autos(TRModel model, TR2Level level)
    {
        for (int i = 0; i < 2; i++)
        {
            var mesh = model.Meshes[i];
            foreach (var v in new[] { 30, 26 })
            {
                mesh.Vertices[v].Y += 7;
                mesh.Vertices[v].Z = 17;
            }
            foreach (var v in new[] { 31, 27 })
            {
                mesh.Vertices[v].Y += 7;
                mesh.Vertices[v].Z = 21;
                if (i == 1)
                {
                    mesh.Vertices[v].Z++;
                }
            }
            mesh.ColouredRectangles.Clear();
            mesh.TexturedRectangles[21].Rotate(1);
            mesh.TexturedRectangles[20].Rotate(3);
        }

        model.Meshes[0].ColouredRectangles.Clear();
        model.Meshes[1].ColouredRectangles.Clear();
    }

    static void PostGunFixes(TRModel model, TR2Level level)
    {
        {
            // Fix missing pistol textures on TR2 legs
            var legP1 = model.Meshes[29];
            var legP2 = model.Meshes[30];

            legP1.TexturedRectangles[9].Texture = legP2.TexturedRectangles[4].Texture;
            legP2.TexturedRectangles[9].Texture = legP1.TexturedRectangles[4].Texture;
            legP1.TexturedRectangles[8].Texture = legP2.TexturedRectangles[5].Texture;
            legP1.TexturedRectangles[8].Rotate(2);
            legP2.TexturedRectangles[9].Rotate(1);
            legP1.TexturedTriangles[5].Rotate(1);

            var vs = new ushort[] { 12,13,14,15 };
            var i = legP2.TexturedRectangles.FindIndex(f => f.Vertices.All(vs.Contains));//4

            vs = [1,2,7];
            i = legP1.TexturedTriangles.FindIndex(f => f.Vertices.All(vs.Contains));//9
        }

        // Make TR2 autos/mags holsters match pistols
        {
            var legP = model.Meshes[29];
            var legA = model.Meshes[33];
            legA.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 8));
            legA.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 8));
            legA.TexturedRectangles.AddRange(legP.TexturedRectangles.Where(f => f.Vertices.All(v => v < 8)).Select(f => f.Clone()));
            legA.TexturedTriangles.AddRange(legP.TexturedTriangles.Where(f => f.Vertices.All(v => v < 8)).Select(f => f.Clone()));

            var legM = model.Meshes[31];
            legM.TexturedRectangles[7].Texture = legP.TexturedRectangles[2].Texture;
        }
        {
            var legP = model.Meshes[30];
            var legA = model.Meshes[34];
            legA.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 8));
            legA.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 8));
            legA.TexturedRectangles.AddRange(legP.TexturedRectangles.Where(f => f.Vertices.All(v => v < 8)).Select(f => f.Clone()));
            legA.TexturedTriangles.AddRange(legP.TexturedTriangles.Where(f => f.Vertices.All(v => v < 8)).Select(f => f.Clone()));

            var legM = model.Meshes[32];
            legM.TexturedRectangles[7].Texture = legP.TexturedRectangles[2].Texture;

            var legD = model.Meshes[36];
            legD.TexturedRectangles[2].Texture = legP.TexturedRectangles[2].Texture;
        }

        {
            // Make TR2 Deagle holster more TR2-like
            var legD = model.Meshes[35];
            var legP = model.Meshes[29];
            legD.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 8));
            legD.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 8));
            legD.TexturedRectangles.AddRange(legP.TexturedRectangles.Where(f => f.Vertices.All(v => v < 8)).Select(f => f.Clone()));
            legD.TexturedTriangles.AddRange(legP.TexturedTriangles.Where(f => f.Vertices.All(v => v < 8)).Select(f => f.Clone()));

            foreach (var vs in new[] { legD.Vertices, legD.Normals })
            {
                (vs[7], vs[4]) = (vs[4], vs[7]);
                (vs[6], vs[5]) = (vs[5], vs[6]);
                (vs[3], vs[0]) = (vs[0], vs[3]);
                (vs[2], vs[1]) = (vs[1], vs[2]);
                (vs[3], vs[2]) = (vs[2], vs[3]);
            }
        }

        {
            // Make TR3 pistols similar to TR2, bar the holsters
            var leg2 = model.Meshes[29];
            var leg3 = model.Meshes[42];
            leg3.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 8));
            leg3.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 8));
            leg3.TexturedRectangles.AddRange(leg2.TexturedRectangles.Where(f => f.Vertices.All(v => v >= 8)).Select(f => f.Clone()));
            leg3.TexturedTriangles.AddRange(leg2.TexturedTriangles.Where(f => f.Vertices.All(v => v >= 8)).Select(f => f.Clone()));
            leg3.TexturedTriangles[5].Rotate(1);

            leg2 = model.Meshes[30];
            leg3 = model.Meshes[43];
            leg3.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 8));
            leg3.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 8));
            leg3.TexturedRectangles.AddRange(leg2.TexturedRectangles.Where(f => f.Vertices.All(v => v >= 8)).Select(f => f.Clone()));
            leg3.TexturedTriangles.AddRange(leg2.TexturedTriangles.Where(f => f.Vertices.All(v => v >= 8)).Select(f => f.Clone()));
        }

        {
            // Make TR3 autos holsters match pistols
            var legP = model.Meshes[42];
            var legA = model.Meshes[46];
            legA.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 8));
            legA.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 8));
            legA.TexturedRectangles.AddRange(legP.TexturedRectangles.Where(f => f.Vertices.All(v => v < 8)).Select(f => f.Clone()));
            legA.TexturedTriangles.AddRange(legP.TexturedTriangles.Where(f => f.Vertices.All(v => v < 8)).Select(f => f.Clone()));

            legP = model.Meshes[43];
            legA = model.Meshes[47];
            legA.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 8));
            legA.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 8));
            legA.TexturedRectangles.AddRange(legP.TexturedRectangles.Where(f => f.Vertices.All(v => v < 8)).Select(f => f.Clone()));
            legA.TexturedTriangles.AddRange(legP.TexturedTriangles.Where(f => f.Vertices.All(v => v < 8)).Select(f => f.Clone()));
        }

        {
            // Magnum alignment
            var legM = model.Meshes[44];
            foreach (var face in legM.TexturedFaces)
            {
                for (int i = 0; i < face.Vertices.Count; i++)
                {
                    if (face.Vertices[i] == 15)
                        face.Vertices[i] = 6;
                    else if (face.Vertices[i] == 6)
                        face.Vertices[i] = 15;
                    else if (face.Vertices[i] == 14)
                        face.Vertices[i] = 7;
                    else if (face.Vertices[i] == 7)
                        face.Vertices[i] = 14;
                }
            }

            foreach (var vs in new[] { legM.Vertices, legM.Normals })
            {
                (vs[15], vs[6]) = (vs[6], vs[15]);
                (vs[14], vs[7]) = (vs[7], vs[14]);

                (vs[5], vs[4]) = (vs[4], vs[5]);
                (vs[7], vs[6]) = (vs[6], vs[7]);
            }

            var legP = model.Meshes[42];
            legM.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 8));
            legM.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 8));
            legM.TexturedRectangles.AddRange(legP.TexturedRectangles.Where(f => f.Vertices.All(v => v < 8)).Select(f => f.Clone()));
            legM.TexturedTriangles.AddRange(legP.TexturedTriangles.Where(f => f.Vertices.All(v => v < 8)).Select(f => f.Clone()));
        }

        {
            // Magnum alignment
            var legM = model.Meshes[45];
            foreach (var face in legM.TexturedFaces)
            {
                for (int i = 0; i < face.Vertices.Count; i++)
                {
                    if (face.Vertices[i] == 15)
                        face.Vertices[i] = 6;
                    else if (face.Vertices[i] == 6)
                        face.Vertices[i] = 15;
                    else if (face.Vertices[i] == 14)
                        face.Vertices[i] = 7;
                    else if (face.Vertices[i] == 7)
                        face.Vertices[i] = 14;
                }
            }

            foreach (var vs in new[] { legM.Vertices, legM.Normals })
            {
                (vs[15], vs[6]) = (vs[6], vs[15]);
                (vs[14], vs[7]) = (vs[7], vs[14]);
                (vs[7], vs[6]) = (vs[6], vs[7]);
            }

            var legP = model.Meshes[43];
            legM.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 8));
            legM.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 8));
            legM.TexturedRectangles.AddRange(legP.TexturedRectangles.Where(f => f.Vertices.All(v => v < 8)).Select(f => f.Clone()));
            legM.TexturedTriangles.AddRange(legP.TexturedTriangles.Where(f => f.Vertices.All(v => v < 8)).Select(f => f.Clone()));
        }

        {
            // Empty Deagle holster            
            var legD = model.Meshes[49] = model.Meshes[43].Clone();
            legD.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 8));
            legD.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 8));
        }

        {
            // Nevada pistols - EZ
            var leg3 = model.Meshes[54];
            leg3.TexturedTriangles[9].Rotate(1);

            // Quick dupe of mags and autos from above
            model.Meshes[56] = model.Meshes[44].Clone();
            model.Meshes[57] = model.Meshes[45].Clone();
            model.Meshes[58] = model.Meshes[46].Clone();
            model.Meshes[59] = model.Meshes[47].Clone();
        }

        {
            var legP = model.Meshes[54].Clone();
            var legM = model.Meshes[56];
            legM.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 8));
            legM.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 8));
            legP.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 8));
            legP.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 8));

            foreach (var face in legP.TexturedFaces)
            {
                for (int i = 0; i < face.Vertices.Count; i++)
                {
                    var vtx = legP.Vertices[face.Vertices[i]];
                    var nor = legP.Normals[face.Vertices[i]];
                    var idx = legM.Vertices.IndexOf(vtx);
                    if (idx == -1)
                    {
                        idx = legM.Vertices.Count;
                        legM.Vertices.Add(vtx);
                        legM.Normals.Add(nor);
                    }
                    face.Vertices[i] = (ushort)idx;
                }
                if (face.Type == TRFaceType.Rectangle)
                    legM.TexturedRectangles.Add(face);
                else
                    legM.TexturedTriangles.Add(face);
            }

            for (int i = 8; i < 16; i++)
            {
                legM.Vertices[i].Y += 6;
            }

            legP = model.Meshes[55].Clone();
            legM = model.Meshes[57];
            legM.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 8));
            legM.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 8));
            legP.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 8));
            legP.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 8));

            foreach (var face in legP.TexturedFaces)
            {
                for (int i = 0; i < face.Vertices.Count; i++)
                {
                    var vtx = legP.Vertices[face.Vertices[i]];
                    var nor = legP.Normals[face.Vertices[i]];
                    var idx = legM.Vertices.IndexOf(vtx);
                    if (idx == -1)
                    {
                        idx = legM.Vertices.Count;
                        legM.Vertices.Add(vtx);
                        legM.Normals.Add(nor);
                    }
                    face.Vertices[i] = (ushort)idx;
                }
                if (face.Type == TRFaceType.Rectangle)
                    legM.TexturedRectangles.Add(face);
                else
                    legM.TexturedTriangles.Add(face);
            }

            for (int i = 8; i < 16; i++)
            {
                legM.Vertices[i].Y += 6;
            }
        }

        {
            var legP = model.Meshes[54].Clone();
            var legM = model.Meshes[58];
            legM.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 8));
            legM.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 8));
            legP.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 8));
            legP.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 8));

            for (int i = 8; i < legM.Vertices.Count; i++)
            {
                legM.Vertices[i].Y += 6;
                legM.Vertices[i].Z -= 6;
            }

            foreach (var face in legP.TexturedFaces)
            {
                for (int i = 0; i < face.Vertices.Count; i++)
                {
                    var vtx = legP.Vertices[face.Vertices[i]];
                    var nor = legP.Normals[face.Vertices[i]];
                    var idx = legM.Vertices.IndexOf(vtx);
                    if (idx == -1)
                    {
                        idx = legM.Vertices.Count;
                        legM.Vertices.Add(vtx);
                        legM.Normals.Add(nor);
                    }
                    face.Vertices[i] = (ushort)idx;
                }
                if (face.Type == TRFaceType.Rectangle)
                    legM.TexturedRectangles.Add(face);
                else
                    legM.TexturedTriangles.Add(face);
            }

            legP = model.Meshes[55].Clone();
            legM = model.Meshes[59];
            legM.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 8));
            legM.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 8));
            legP.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 8));
            legP.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 8));

            for (int i = 8; i < legM.Vertices.Count; i++)
            {
                legM.Vertices[i].Y += 6;
                legM.Vertices[i].Z -= 6;
            }

            foreach (var face in legP.TexturedFaces)
            {
                for (int i = 0; i < face.Vertices.Count; i++)
                {
                    var vtx = legP.Vertices[face.Vertices[i]];
                    var nor = legP.Normals[face.Vertices[i]];
                    var idx = legM.Vertices.IndexOf(vtx);
                    if (idx == -1)
                    {
                        idx = legM.Vertices.Count;
                        legM.Vertices.Add(vtx);
                        legM.Normals.Add(nor);
                    }
                    face.Vertices[i] = (ushort)idx;
                }
                if (face.Type == TRFaceType.Rectangle)
                    legM.TexturedRectangles.Add(face);
                else
                    legM.TexturedTriangles.Add(face);
            }

        }

        {
            // Nevada empty deagle
            var legD = model.Meshes[61] = model.Meshes[55].Clone();
            legD.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 8));
            legD.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 8));

            // Use dark grey for top of holster
            legD = model.Meshes[60];            
            var texInfo = level.ObjectTextures[legD.TexturedTriangles[6].Texture];
            var img = new TRImage(level.Images16[texInfo.Atlas].Pixels);
            var clip = img.Export(texInfo.Bounds);
            clip.Write((c, x, y) => Color.FromArgb(32, 32, 32));
            img.Import(clip, texInfo.Position);
            level.Images16[texInfo.Atlas].Pixels = img.ToRGB555();

            // Fix another bad texture
            var vs = new ushort[] { 10, 11, 12, 14 };
            var i = legD.TexturedTriangles.FindAll(f => f.Vertices.All(vs.Contains));
            foreach (var m in new[] {14,35,48 })
            {
                var od = model.Meshes[m];
                od.TexturedRectangles.RemoveAll(f => f.Vertices.All(vs.Contains));
                od.TexturedTriangles.RemoveAll(f => f.Vertices.All(vs.Contains));
                od.TexturedTriangles.AddRange(i.Select(g => g.Clone()));
            }
        }
    }

    static void FixTR2Uzis(TRModel model, TR2Level level)
    {
        var actualGun = level.Models[TR2Type.Uzi_M_H];
        for (int i = 0; i < 2; i++)
        {
            var mesh = model.Meshes[i];
            foreach (var v in new[] { 19, 23 })
            {
                mesh.Vertices[v].Y -= 2;
                mesh.Vertices[v].Z = 17;
            }
            foreach (var v in new[] { 18, 22 })
            {
                mesh.Vertices[v].Y -= 2;
                mesh.Vertices[v].Z = 21;
            }
            foreach (var v in i == 0 ? [32, 35, 36, 39] : new[] { 27,31,24,28 })
            {
                mesh.Vertices[v].Z++;
            }

            // Texturing is awful, maybe come back to
            //mesh.TexturedRectangles[14].Texture = actualGun.Meshes[1].TexturedRectangles[9].Texture;
            //mesh.TexturedRectangles[14].Rotate(2);
            //mesh.TexturedRectangles[13].Texture = actualGun.Meshes[1].TexturedRectangles[7].Texture;
            //mesh.TexturedRectangles[12].Texture = actualGun.Meshes[1].TexturedRectangles[8].Texture;

            //mesh.TexturedRectangles[15].Texture = actualGun.Meshes[1].TexturedRectangles[6].Texture;
            //mesh.TexturedRectangles[11].Texture = actualGun.Meshes[1].TexturedRectangles[6].Texture;

            //mesh.TexturedRectangles[i == 0 ? 21 : 20].Texture = actualGun.Meshes[1].TexturedRectangles[12].Texture;
            //mesh.TexturedRectangles[i == 0 ? 24 : 19].Texture = actualGun.Meshes[1].TexturedRectangles[15].Texture;
            //mesh.TexturedRectangles[i == 0 ? 25 : 16].Texture = actualGun.Meshes[1].TexturedRectangles[16].Texture;
            //mesh.TexturedRectangles[i == 0 ? 22 : 17].Texture = actualGun.Meshes[1].TexturedRectangles[13].Texture;
            //mesh.TexturedRectangles[i == 0 ? 23 : 18].Texture = actualGun.Meshes[1].TexturedRectangles[14].Texture;

            //mesh.TexturedRectangles[6].Texture = actualGun.Meshes[1].TexturedRectangles[2].Texture;
            //if (i == 1)
            //{
            //    mesh.TexturedRectangles[6].Rotate(2);
            //}
        }
    }

    static void FixTR2Shotgun(TRModel model, TR2Level level)
    {
        var mesh = model.Meshes[0];
        mesh.ColouredRectangles.Clear();
        foreach (var v in new[] { 18, 19, 22, 23 })
        {
            mesh.Vertices[v].Y += 5;
            mesh.Vertices[v].Z = 19;
        }
    }

    static void FixDeagle(TRMesh mesh, TRMesh actualGun)
    {
        mesh.TexturedTriangles[22].Texture = actualGun.TexturedTriangles[12].Texture;
        mesh.TexturedTriangles[23].Texture = actualGun.TexturedTriangles[13].Texture;

        foreach (var v in new[] { 10,11,14,15 })
        {
            mesh.Vertices[v].Y += (short)((v == 10 || v == 14) ? 7 : 5);
            mesh.Vertices[v].Z = (short)((v == 10 || v == 14) ? 21 : 17);
        }
    }

    static void FixTR3Shotgun(TRModel model, TR3Level level)
    {
        var mesh = model.Meshes[0];
        foreach (var v in new[] { 18, 19, 22, 23 })
        {
            mesh.Vertices[v].Y += 5;
            mesh.Vertices[v].Z = 19;
        }
    }

    static void FixTR3Grenade(TRModel model, TR3Level level)
    {
        var mesh = model.Meshes[0];
        var remap = new Dictionary<ushort, ushort>
        {
            [28] = 23,
            [29] = 13,
            [31] = 20,
            [30] = 8,
        };
        foreach (var (src, tar) in remap)
        {
            var vertA = mesh.Vertices[src];
            var vertB = mesh.Vertices[tar];
            vertA.Y = vertB.Y;
            vertA.Z = vertB.Z;
            if (src == 28 || src == 29)
            {
                vertA.X -= 1;
            }
            else
            {
                vertA.X += 4;
            }
        }

        foreach (var v in new[] { 24, 25, 26, 27 })
        {
            if (v % 2 == 0)
            {
                mesh.Vertices[v].X += 4;
            }
            else
            {
                mesh.Vertices[v].X -= 1;
            }
        }

        mesh.TexturedRectangles.Add(new()
        {
            Type = TRFaceType.Rectangle,
            Texture = mesh.TexturedRectangles[11].Texture,
            Vertices = [8, 20, 23, 13],
        });
    }

    static void DeleteHands(params TRMesh[] meshes)
    {
        foreach (var mesh in meshes)
        {
            mesh.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 8));
            mesh.ColouredRectangles.RemoveAll(f => f.Vertices.All(v => v < 8));
            mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 8));
            mesh.ColouredTriangles.RemoveAll(f => f.Vertices.All(v => v < 8));
        }
    }

    static void DeleteThighs(params TRMesh[] meshes)
    {
        foreach (var mesh in meshes)
        {
            mesh.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 13 || (v >= 21 && v <= 25)));
            mesh.ColouredRectangles.RemoveAll(f => f.Vertices.All(v => v < 13 || (v >= 21 && v <= 25)));
            mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 13 || (v >= 21 && v <= 25)));
            mesh.ColouredTriangles.RemoveAll(f => f.Vertices.All(v => v < 13 || (v >= 21 && v <= 25)));
        }
    }

    static void DeleteNevadaThighs(params TRMesh[] meshes)
    {
        foreach (var mesh in meshes)
        {
            mesh.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 18));
            mesh.ColouredRectangles.RemoveAll(f => f.Vertices.All(v => v < 18));
            mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 18));
            mesh.ColouredTriangles.RemoveAll(f => f.Vertices.All(v => v < 18));
        }
    }

    static void DeleteDeagleThighs(TRMesh mesh1, TRMesh mesh2)
    {
        {
            mesh1.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 18));
            mesh1.ColouredRectangles.RemoveAll(f => f.Vertices.All(v => v < 18));
            mesh1.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 18));
            mesh1.ColouredTriangles.RemoveAll(f => f.Vertices.All(v => v < 18));
        }
        {
            mesh2.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 13 || (v >= 21 && v <= 25)));
            mesh2.ColouredRectangles.RemoveAll(f => f.Vertices.All(v => v < 13 || (v >= 21 && v <= 25)));
            mesh2.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 13 || (v >= 21 && v <= 25)));
            mesh2.ColouredTriangles.RemoveAll(f => f.Vertices.All(v => v < 13 || (v >= 21 && v <= 25)));
        }
    }

    static void DoExtra(TR2Level baseLevel)
    {
        var baseModel = MakeBaseModel();
        baseLevel.Models[_laraSkinExtra] = baseModel;

        {
            // TR1 angry
            var caves = _reader1.Read(@"F:\tomp\all levels\tr1\level1.phd");
            var source = caves.Models[TR1Type.LaraUziAnimation_H];
            source.Meshes = [source.Meshes[14]];
            source.MeshTrees.Clear();
            Import(baseLevel, baseModel, caves, source, null);
            baseModel.MeshTrees.Add(new() { OffsetY = -330, OffsetZ = -23, Flags = _push });
        }

        {
            // TR2/3 angry
            var caves = _reader2.Read(@"F:\tomp\all levels\tr2\wall.tr2");
            var source = caves.Models[TR2Type.LaraUziAnim_H];
            source.Meshes = [source.Meshes[14]];
            source.MeshTrees.Clear();
            Import(baseLevel, baseModel, caves, source, null);
            baseModel.MeshTrees.Add(new() { OffsetZ = 160 });
        }

        {
            // TR3 angry
            var caves = _reader3.Read(@"F:\tomp\all levels\tr3\jungle.tr2");
            var source = caves.Models[TR3Type.LaraUziAnimation_H];
            source.Meshes = [source.Meshes[14]];
            source.MeshTrees.Clear();
            Import(baseLevel, baseModel, caves, source, null);
            baseModel.MeshTrees.Add(new() { OffsetZ = 160 });
        }

        void Goldify(TRMesh mesh)
        {
            var goldTex = baseLevel.Models[_laraSkin1].Meshes[61].TexturedFaces.First().Texture;
            foreach (var f in mesh.TexturedFaces)
                f.Texture = goldTex;
        }

        {
            // TR1 braid body
            var caves = _reader1.Read(@"lara_braid.phd");
            var source = caves.Models[TR1Type.LaraHairBodySwap];
            source.Meshes.RemoveAt(3);
            source.MeshTrees.RemoveAt(source.MeshTrees.Count - 1);
            Import(baseLevel, baseModel, caves, source, null);

            var mauledTorso = baseLevel.Models[_laraSkin1].Meshes[38].Clone();
            baseModel.Meshes.Insert(5, mauledTorso);
            for (int i = 26; i < 30; i++)
            {
                mauledTorso.Vertices[i].Z += 12;
            }

            {
                // Clone for gold
                var meshT = baseModel.Meshes[4].Clone();
                var meshH = baseModel.Meshes[6].Clone();
                Goldify(meshT);
                Goldify(meshH);
                baseModel.Meshes.Insert(6, meshT);
                baseModel.Meshes.Insert(8, meshH);
            }

            baseModel.MeshTrees.Add(new() { OffsetX = -160, Flags = _read }); // Torso
            baseModel.MeshTrees.Add(new() { OffsetZ = 160, Flags = _push }); // Mauled torso
            baseModel.MeshTrees.Add(new() { OffsetZ = 320, Flags = _read }); // Gold torso
            baseModel.MeshTrees.Add(new() { OffsetY = -198, OffsetZ = -23, Flags = _read }); // Head
            baseModel.MeshTrees.Add(new() { OffsetY = -198, OffsetZ = 297, Flags = _read }); // Gold Head
            baseModel.MeshTrees.Add(new() { OffsetY = -330, OffsetZ = -23, Flags = _pop }); // Angry
        }

        {
            // TR1 braid itself
            var caves = _reader1.Read("lara_braid.phd");
            var source = caves.Models[TR1Type.LaraPonytail_H_U];
            Import(baseLevel, baseModel, caves, source, null);

            baseModel.MeshTrees.Add(new() { OffsetX = -320, Flags = _read });
            baseModel.MeshTrees.AddRange(source.MeshTrees);

            // Golden
            var golds = baseModel.Meshes.GetRange(baseModel.Meshes.Count - 6, 6).Select(t => t.Clone()).ToList();
            var trees = baseModel.MeshTrees.GetRange(baseModel.MeshTrees.Count - 6, 6).Select(t => t.Clone()).ToList();
            for (int i = 0; i < 6; i++)
            {
                var mesh = golds[i];
                Goldify(mesh);
                baseModel.Meshes.Add(mesh);
                baseModel.MeshTrees.Add(trees[i]);
                if (i == 0)
                {
                    trees[i].OffsetX -= 80;
                }
            }
        }

        {
            // TR2 braid itself
            var caves = _reader2.Read(@"F:\tomp\all levels\tr2\wall.tr2");
            var source = caves.Models[TR2Type.LaraPonytail_H];
            Import(baseLevel, baseModel, caves, source, null);

            baseModel.MeshTrees.Add(new() { OffsetX = -480, Flags = _read });
            baseModel.MeshTrees.AddRange(source.MeshTrees);

            // Golden
            var golds = baseModel.Meshes.GetRange(baseModel.Meshes.Count - 6, 6).Select(t => t.Clone()).ToList();
            var trees = baseModel.MeshTrees.GetRange(baseModel.MeshTrees.Count - 6, 6).Select(t => t.Clone()).ToList();
            for (int i = 0; i < 6; i++)
            {
                var mesh = golds[i];
                Goldify(mesh);
                baseModel.Meshes.Add(mesh);
                baseModel.MeshTrees.Add(trees[i]);
                if (i == 0)
                {
                    trees[i].OffsetX -= 80;
                }
            }
        }

        {
            // TR2 dagger on hips
            var hips = baseLevel.Models[_laraSkin1].Meshes[166];
            var dagger = hips.Clone();
            hips.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 20));
            hips.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 20));
            dagger.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 20));
            dagger.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 20));

            CleanupVertices(hips);
            CleanupVertices(dagger);

            baseModel.Meshes.Add(dagger);
            baseModel.MeshTrees.Add(new() { OffsetX = -676, Flags = _read });
        }

        {
            // TR2 dagger in hand
            var caves = _reader2.Read(@"F:\tomp\all levels\tr2\house.tr2");
            var source = caves.Models[TR2Type.LaraMiscAnim_H];
            source.Meshes = [source.Meshes[10]];
            source.MeshTrees.Clear();
            DeleteHands(source.Meshes[0]);
            Import(baseLevel, baseModel, caves, source, null);

            baseModel.MeshTrees.Add(new() { OffsetY = -160, }); // Dagger in hand

            var hipsDagger = baseModel.Meshes[^2];
            var vs = new ushort[] { 47,52,25,26 };
            var face = hipsDagger.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains));

            var handDagger = baseModel.Meshes[^1];
            vs = [76, 77, 78, 79, 80, 81, 56, 57, 58, 59, 60, 61];
            handDagger.TexturedRectangles.RemoveAll(f => f.Vertices.All(vs.Contains));
            var verts = new List<List<ushort>>
            {
                new() { 53,59,58,52 },
                new() { 52,58,61,55 },
                new() { 55,61,60,54 },
                new() { 54,60,56,50 },
                new() { 50,56,57,51 },
                new() { 51,57,59,53 },
            };
            handDagger.TexturedRectangles.AddRange(verts.Select(v => new TRMeshFace
            {
                Type = TRFaceType.Rectangle,
                Vertices = v,
                Texture = face.Texture,
            }));
        }

        {
            // Fix up both daggers
            var daggerHips = baseModel.Meshes[^2];
            var daggerHand = baseModel.Meshes[^1];
            daggerHips.SelfCalculateBounds();
            daggerHand.SelfCalculateBounds();

            ushort gold;
            {
                var vs = new ushort[] { 35,37,39,41 };
                gold = daggerHand.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains)).Texture;
                var info = baseLevel.ObjectTextures[gold];
                var tile = new TRImage(baseLevel.Images16[info.Atlas].Pixels);
                var img = tile.Export(info.Bounds);
                img.Write((c, x, y) => y == 0 && (x == 13 || x == 15) ? Color.FromArgb(184, 128, 32) : c);
                tile.Import(img, info.Position);
                baseLevel.Images16[info.Atlas].Pixels = tile.ToRGB555();
            }

            {
                var vs = new ushort[] { 13,9,8,10,17,18 };
                daggerHand.TexturedTriangles.RemoveAll(f => f.Vertices.All(vs.Contains));
                daggerHand.TexturedRectangles.Add(new()
                {
                    Type = TRFaceType.Rectangle,
                    Vertices = [13,9,8,10],
                    Texture = gold,
                });
                daggerHand.TexturedRectangles.Add(new()
                {
                    Type = TRFaceType.Rectangle,
                    Vertices = [10,8,18,17],
                    Texture = gold,
                });

                vs = [20, 19, 13, 9];
                var vs2 = new ushort[] { 20,19,16,15 };
                var tex = daggerHand.TexturedTriangles.Find(f => f.Vertices.All(vs2.Contains));
                foreach (var f in daggerHand.TexturedTriangles.Where(f => f.Vertices.All(vs.Contains)))
                    f.Texture = tex.Texture;
            }

            {
                var vs = new ushort[] { 39,38,35 };
                gold = daggerHand.TexturedTriangles.Find(f => f.Vertices.All(vs.Contains)).Texture;
                vs = [17, 18, 14];
                daggerHand.TexturedTriangles.Find(f => f.Vertices.All(vs.Contains)).Texture = gold;
            }

            CleanupVertices(daggerHand);

            var map = new Dictionary<ushort, ushort>
            {
                [67] = 67,
                [29] = 58,
                [31] = 60,
                [33] = 62,
                [35] = 64,
                [24] = 53,
                [27] = 56,

                [28] = 57,
                [30] = 59,
                [32] = 61,
                [34] = 63,
                [25] = 54,
                [26] = 55,

                [46] = 25,
                [37] = 28,
                [38] = 27,
                [40] = 46,
                [42] = 45,
                [44] = 26,
                [66] = 66,

                [36] = 48,
                [47] = 47,
                [45] = 52,
                [43] = 50,
                [41] = 49,
                [39] = 51,

                [18] = 38,
                [19] = 37,
                [17] = 34,
                [16] = 33,
                [14] = 30,
                [15] = 29,
                [59] = 24,
                [55] = 18,
                [58] = 23,
                [52] = 19,

                [54] = 17,
                [61] = 32,
                [53] = 20,
                [56] = 21,
                [57] = 22,
                [60] = 31,

                [21] = 35,
                [22] = 36,
                [62] = 42,
                [63] = 43,
                [65] = 41,
                [23] = 44,
                [20] = 39,
                [64] = 40,

                [0] = 0,
                [1] = 1,
                [2] = 2,
                [3] = 3,
                [4] = 12,
                [5] = 10,
                [6] = 5,
                [7] = 4,
                [13] = 11,
                [11] = 15,
                [8] = 13,
                [10] = 16,
                [9] = 14,
                [12] = 7,
                [48] = 6,
                [51] = 8,
                [50] = 65,
                [49] = 9,
            };

            daggerHips.TexturedTriangles.Clear();
            daggerHips.TexturedRectangles.Clear();
            foreach (var face in daggerHand.TexturedFaces.Where(f => f.Vertices.All(map.ContainsKey)))
            {
                var f2 = face.Clone();
                for (int i = 0; i < f2.Vertices.Count; i++)
                    f2.Vertices[i] = map[f2.Vertices[i]];
                if (face.Type == TRFaceType.Rectangle)
                    daggerHips.TexturedRectangles.Add(f2);
                else
                    daggerHips.TexturedTriangles.Add(f2);
            }
        }

        {
            // TR3 oar
            var caves = _reader3.Read(@"F:\tomp\all levels\tr3\rapids.tr2");
            var source = caves.Models[TR3Type.LaraVehicleAnimation_H];
            source.Meshes = [source.Meshes[10]];
            source.MeshTrees.Clear();
            DeleteHands(source.Meshes[0]);
            Import(baseLevel, baseModel, caves, source, null);

            var mesh = baseModel.Meshes[^1];
            for (int i = 0; i < 20; i++)
            {
                mesh.Vertices[i].X += 4;
                mesh.Vertices[i].Y--;
            }
            var map = new Dictionary<ushort, ushort>
            {
                [21] = 8,
                [22] = 11,
                [26] = 15,
                [25] = 12,
            };
            foreach (var (v1, v2) in map)
                mesh.Vertices[v1].Z = mesh.Vertices[v2].Z;

            foreach (var v in new[] { 16, 17, 18, 19, 28, 29, 30, 31 })
            {
                mesh.Vertices[v].Y -= 4;
                if (v < 20)
                    mesh.Vertices[v].Y--;
            }

            mesh.TexturedRectangles[12].Texture = mesh.TexturedRectangles[13].Texture = mesh.TexturedRectangles[8].Texture;
            baseModel.MeshTrees.Add(new() { OffsetX = -880, Flags = _read });

            var vs = new ushort[] { 9,10,8,11 };
            var face = mesh.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains));
            face.Rotate(1);
            vs = [10, 14, 11, 15];
            face = mesh.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains));
            face.Rotate(2);
            
            vs = [22,26,23,27];
            face = mesh.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains));
            var tex = baseLevel.ObjectTextures[face.Texture].Clone();
            tex.UVMode = TRUVMode.NE_AntiClockwise;
            var texId = (ushort)baseLevel.ObjectTextures.Count;
            baseLevel.ObjectTextures.Add(tex);
            face.Texture = texId;
            face.Rotate(2);

            vs = [20, 21, 22, 23];
            face = mesh.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains));
            face.Texture = texId;
            face.Rotate(1);

            vs = [25, 21, 24, 20];
            face = mesh.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains));
            face.Texture = texId;

            vs = [25, 26, 24, 27];
            face = mesh.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains));
            face.Texture = texId;
        }

        {
            // TR3 spanner
            var caves = _reader3.Read(@"F:\tomp\all levels\tr3\mines.tr2");
            var source = caves.Models[TR3Type.LaraVehicleAnimation_H];
            source.Meshes = [source.Meshes[10]];
            source.MeshTrees.Clear();
            DeleteHands(source.Meshes[0]);
            Import(baseLevel, baseModel, caves, source, null);

            var mesh = baseModel.Meshes[^1];
            var map = new Dictionary<ushort, ushort>
            {
                [26] = 38,
                [25] = 37,
                [13] = 33,
                [14] = 34,
            };
            var del = new ushort[] {39,36,32,35 };
            mesh.TexturedRectangles.RemoveAll(f => f.Vertices.Any(del.Contains));
            foreach (var f in mesh.TexturedRectangles)
            {
                for (int i = 0; i < f.Vertices.Count; i++)
                {
                    if (map.TryGetValue(f.Vertices[i], out var v))
                        f.Vertices[i] = v;
                }
            }

            baseModel.MeshTrees.Add(new() { OffsetX = -960, Flags = _read });
        }

        {
            // TR3 drink can
            var caves = _reader3.Read(@"F:\tomp\all levels\tr3\cut8.tr2");
            var source = caves.Models[TR3Type.LaraPistolAnimation_H];
            source.Meshes = [source.Meshes[10]];
            source.MeshTrees.Clear();
            DeleteHands(source.Meshes[0]);
            Import(baseLevel, baseModel, caves, source, null);

            var mesh = baseModel.Meshes[^1];
            mesh.TexturedRectangles.Add(new()
            {
                Type = TRFaceType.Rectangle,
                Vertices = [17, 23, 24, 18],
                Texture = mesh.TexturedRectangles[9].Texture,
            });

            baseModel.MeshTrees.Add(new() { OffsetX = -1020, Flags = _read });
        }

        if (true)
        {
            // Glasses
            var caves = _reader2.Read("glasses.tr2");
            var source = caves.Models[TR2Type.LaraSnowmobAnim_H];
            source.Meshes = [source.Meshes[14]];
            source.MeshTrees.Clear();
            Import(baseLevel, baseModel, caves, source, null);

            var head = baseModel.Meshes[^1];
            head.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 61));
            head.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 61));

            CleanupVertices(head);
            var vs = new ushort[] {42,32,44,46 };
            var f = head.TexturedRectangles.Find(g => g.Vertices.All(vs.Contains));
            var m = f.Texture;
            baseLevel.ObjectTextures[f.Texture].BlendingMode = TRBlendingMode.Reflective;
            vs = [30,29,48];
            f = head.TexturedTriangles.Find(g => g.Vertices.All(vs.Contains));
            baseLevel.ObjectTextures[f.Texture].BlendingMode = TRBlendingMode.Reflective;
            vs = [34,37,51];
            f = head.TexturedTriangles.Find(g => g.Vertices.All(vs.Contains));
            baseLevel.ObjectTextures[f.Texture].BlendingMode = TRBlendingMode.Reflective;
            vs = [31,28,49];
            f = head.TexturedTriangles.Find(g => g.Vertices.All(vs.Contains));
            baseLevel.ObjectTextures[f.Texture].BlendingMode = TRBlendingMode.Reflective;

            vs = [6,7,8,9];
            f = head.TexturedRectangles.Find(g => g.Vertices.All(vs.Contains));
            head.TexturedRectangles.Add(new()
            {
                Type = TRFaceType.Rectangle,
                Vertices = [12,15,14,13],
                Texture = f.Texture,
            });

            {
                m = f.Texture;
                var t = baseLevel.ObjectTextures[f.Texture];
                var tile = baseLevel.Images16[t.Atlas];
                var img = new TRImage(tile.Pixels);
                var bit = img.Export(t.Bounds);
                bit.Write((c, x, y) => Color.FromArgb(32, 32, 32));
                img.Import(bit, t.Position);
                t.Bounds = new(t.Position, new(8, 8));
                tile.Pixels = img.ToRGB555();
            }

            head.Vertices.ForEach(v => v.Z -= 1);//28
            foreach (var v in new[] { 35,39,41,55,52,50,36, 29,30,32,42,44,46,48 })
            {
                head.Vertices[v].Z -= 1;
            }

            foreach (var v in new[] { 21,20,25,16,17,24 })
                head.Vertices[v].X -= 1;
            foreach (var v in new[] { 5,6,9,10,12,13 })
                head.Vertices[v].X += 1;

            foreach (var v in new[] { 16,17,18,19,24,27 })
                head.Vertices[v].Y += 1;
            foreach (var v in new[] { 20,21,22,23,25,26 })
                head.Vertices[v].Y -= 1;

            foreach (var v in new[] { 4,5,6,7,12,15 })
                head.Vertices[v].Y += 1;
            foreach (var v in new[] { 8,9,10,11,13,14 })
                head.Vertices[v].Y -= 1;

            foreach (var v in new[] { 6,7,8,9 })
                head.Vertices[v].X -= 1;

            head.Vertices[6].Y += 1;
            head.Vertices[9].Y -= 1;

            foreach (var v in new[] { 16,19,21,22 })
                head.Vertices[v].X += 1;

            head.Vertices[16].Y += 1;
            head.Vertices[21].Y -= 1;

            {
                vs = [0,1,2,3];
                head.TexturedRectangles.RemoveAll(f => f.Vertices.All(vs.Contains));
                var v0 = head.Vertices[0];
                var v1 = head.Vertices[1];
                var v2 = head.Vertices[2];
                var v3 = head.Vertices[3];

                v0.Y += 3;
                v1.Y += 3;
                v0.Z -= 1;
                v1.Z -= 1;
                v2.Z += 1;
                v3.Z += 1;

                var map = new Dictionary<ushort, ushort>();
                for (ushort i = 0; i < 4; i++)
                {
                    map[i] = (ushort)head.Vertices.Count;
                    head.Vertices.Add(head.Vertices[i].Clone());
                    head.Normals.Add(head.Normals[i].Clone());
                    if (i == 1 || i == 2)
                        head.Vertices[^1].X-=3;
                    else
                        head.Vertices[^1].X+=3;
                    head.Vertices[^1].Y -= 2;
                }

                head.TexturedRectangles.Add(new()
                {
                    Type = TRFaceType.Rectangle,
                    Texture = m,
                    Vertices = [1, map[1], map[2],2],
                });
                head.TexturedRectangles.Add(new()
                {
                    Type = TRFaceType.Rectangle,
                    Texture = m,
                    Vertices = [map[1], 1, 2, map[2]],
                });

                head.TexturedRectangles.Add(new()
                {
                    Type = TRFaceType.Rectangle,
                    Texture = m,
                    Vertices = [map[0], 0, 3, map[3]],
                });
                head.TexturedRectangles.Add(new()
                {
                    Type = TRFaceType.Rectangle,
                    Texture = m,
                    Vertices = [0, map[0], map[3], 3],
                });

                head.TexturedRectangles.Add(new()
                {
                    Type = TRFaceType.Rectangle,
                    Texture = m,
                    Vertices = [map[1], map[0], map[3], map[2]],
                });
                head.TexturedRectangles.Add(new()
                {
                    Type = TRFaceType.Rectangle,
                    Texture = m,
                    Vertices = [map[0], map[1], map[2], map[3]],
                });
            }

            baseModel.MeshTrees.Add(new() { OffsetX = -1160, Flags = _read });
        }

        if (true)
        {
            // Glasses - transparent
            var caves = _reader2.Read("glasses.tr2");
            var source = caves.Models[TR2Type.LaraSnowmobAnim_H];
            source.Meshes = [source.Meshes[14]];
            source.MeshTrees.Clear();
            Import(baseLevel, baseModel, caves, source, null);

            var head = baseModel.Meshes[^1];
            head.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 61));
            head.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 61));

            CleanupVertices(head);
            var vs = new ushort[] { 42, 32, 44, 46 };
            var f = head.TexturedRectangles.Find(g => g.Vertices.All(vs.Contains));
            var m = f.Texture;
            baseLevel.ObjectTextures[f.Texture].BlendingMode = TRBlendingMode.ReflectiveAlpha;
            vs = [30, 29, 48];
            f = head.TexturedTriangles.Find(g => g.Vertices.All(vs.Contains));
            baseLevel.ObjectTextures[f.Texture].BlendingMode = TRBlendingMode.ReflectiveAlpha;
            vs = [34, 37, 51];
            f = head.TexturedTriangles.Find(g => g.Vertices.All(vs.Contains));
            baseLevel.ObjectTextures[f.Texture].BlendingMode = TRBlendingMode.ReflectiveAlpha;
            vs = [31, 28, 49];
            f = head.TexturedTriangles.Find(g => g.Vertices.All(vs.Contains));
            baseLevel.ObjectTextures[f.Texture].BlendingMode = TRBlendingMode.ReflectiveAlpha;

            vs = [6, 7, 8, 9];
            f = head.TexturedRectangles.Find(g => g.Vertices.All(vs.Contains));
            head.TexturedRectangles.Add(new()
            {
                Type = TRFaceType.Rectangle,
                Vertices = [12, 15, 14, 13],
                Texture = f.Texture,
            });

            {
                m = f.Texture;
                var t = baseLevel.ObjectTextures[f.Texture];
                var tile = baseLevel.Images16[t.Atlas];
                var img = new TRImage(tile.Pixels);
                var bit = img.Export(t.Bounds);
                bit.Write((c, x, y) => Color.FromArgb(32, 32, 32));
                img.Import(bit, t.Position);
                t.Bounds = new(t.Position, new(8, 8));
                tile.Pixels = img.ToRGB555();
            }

            head.Vertices.ForEach(v => v.Z -= 1);//28
            foreach (var v in new[] { 35, 39, 41, 55, 52, 50, 36, 29, 30, 32, 42, 44, 46, 48 })
            {
                head.Vertices[v].Z -= 1;
            }

            foreach (var v in new[] { 21, 20, 25, 16, 17, 24 })
                head.Vertices[v].X -= 1;
            foreach (var v in new[] { 5, 6, 9, 10, 12, 13 })
                head.Vertices[v].X += 1;

            foreach (var v in new[] { 16, 17, 18, 19, 24, 27 })
                head.Vertices[v].Y += 1;
            foreach (var v in new[] { 20, 21, 22, 23, 25, 26 })
                head.Vertices[v].Y -= 1;

            foreach (var v in new[] { 4, 5, 6, 7, 12, 15 })
                head.Vertices[v].Y += 1;
            foreach (var v in new[] { 8, 9, 10, 11, 13, 14 })
                head.Vertices[v].Y -= 1;

            foreach (var v in new[] { 6, 7, 8, 9 })
                head.Vertices[v].X -= 1;

            head.Vertices[6].Y += 1;
            head.Vertices[9].Y -= 1;

            foreach (var v in new[] { 16, 19, 21, 22 })
                head.Vertices[v].X += 1;

            head.Vertices[16].Y += 1;
            head.Vertices[21].Y -= 1;

            {
                vs = [0, 1, 2, 3];
                head.TexturedRectangles.RemoveAll(f => f.Vertices.All(vs.Contains));
                var v0 = head.Vertices[0];
                var v1 = head.Vertices[1];
                var v2 = head.Vertices[2];
                var v3 = head.Vertices[3];

                v0.Y += 3;
                v1.Y += 3;
                v0.Z -= 1;
                v1.Z -= 1;
                v2.Z += 1;
                v3.Z += 1;

                var map = new Dictionary<ushort, ushort>();
                for (ushort i = 0; i < 4; i++)
                {
                    map[i] = (ushort)head.Vertices.Count;
                    head.Vertices.Add(head.Vertices[i].Clone());
                    head.Normals.Add(head.Normals[i].Clone());
                    if (i == 1 || i == 2)
                        head.Vertices[^1].X -= 3;
                    else
                        head.Vertices[^1].X += 3;
                    head.Vertices[^1].Y -= 2;
                }

                head.TexturedRectangles.Add(new()
                {
                    Type = TRFaceType.Rectangle,
                    Texture = m,
                    Vertices = [1, map[1], map[2], 2],
                });
                head.TexturedRectangles.Add(new()
                {
                    Type = TRFaceType.Rectangle,
                    Texture = m,
                    Vertices = [map[1], 1, 2, map[2]],
                });

                head.TexturedRectangles.Add(new()
                {
                    Type = TRFaceType.Rectangle,
                    Texture = m,
                    Vertices = [map[0], 0, 3, map[3]],
                });
                head.TexturedRectangles.Add(new()
                {
                    Type = TRFaceType.Rectangle,
                    Texture = m,
                    Vertices = [0, map[0], map[3], 3],
                });

                head.TexturedRectangles.Add(new()
                {
                    Type = TRFaceType.Rectangle,
                    Texture = m,
                    Vertices = [map[1], map[0], map[3], map[2]],
                });
                head.TexturedRectangles.Add(new()
                {
                    Type = TRFaceType.Rectangle,
                    Texture = m,
                    Vertices = [map[0], map[1], map[2], map[3]],
                });
            }

            vs = [28,29,30,31,32,33,42,43,44,45,46,47,48,49,
            34,35,36,37,38,39,40,41,50,51,52,53,54,55];
            var faces = head.TexturedRectangles.FindAll(f => f.Vertices.All(vs.Contains));
            foreach (var face in faces)
            {
                if (baseLevel.ObjectTextures[face.Texture].BlendingMode == TRBlendingMode.ReflectiveAlpha)
                    continue;
                var f2 = face.Clone();
                f2.SwapVertices(0, 1);
                f2.SwapVertices(2, 3);
                head.TexturedRectangles.Add(f2);
            }

            baseModel.MeshTrees.Add(new() { OffsetX = -1160, OffsetZ = 120, Flags = _pop });
        }

        /// Fixes

        {
            // Fix combat heads
            var badHead = baseModel.Meshes[1];
            var goodHead = baseLevel.Models[_laraSkin1].Meshes[15].Clone();

            var ve = new ushort[] { 22,23,21,13,15,14,20,12,19,16,17,0,18, };
            goodHead.TexturedRectangles.RemoveAll(f => f.Vertices.All(ve.Contains));
            goodHead.TexturedTriangles.RemoveAll(f => f.Vertices.All(ve.Contains));
            goodHead.TexturedRectangles.AddRange(badHead.TexturedRectangles.Where(f => f.Vertices.All(ve.Contains)));
            goodHead.TexturedTriangles.AddRange(badHead.TexturedTriangles.Where(f => f.Vertices.All(ve.Contains)));
            baseModel.Meshes[1] = goodHead;

            badHead = baseModel.Meshes[9];
            goodHead = baseModel.Meshes[7].Clone();
            goodHead.TexturedRectangles.RemoveAll(f => f.Vertices.All(ve.Contains));
            goodHead.TexturedTriangles.RemoveAll(f => f.Vertices.All(ve.Contains));
            goodHead.TexturedRectangles.AddRange(badHead.TexturedRectangles.Where(f => f.Vertices.All(ve.Contains)));
            goodHead.TexturedTriangles.AddRange(badHead.TexturedTriangles.Where(f => f.Vertices.All(ve.Contains)));
            baseModel.Meshes[9] = goodHead;
        }

        var frame = baseModel.Animations[0].Frames[0];
        frame.Rotations.AddRange(Enumerable.Repeat(0, baseModel.Meshes.Count - 1).Select(i => new TRAnimFrameRotation()));

        for (int i = 1; i < baseModel.Meshes.Count; i++)
        {
            CleanupVertices(baseModel.Meshes[i]);
        }
    }

    static void DoHolsters(TRModel baseModel, TR2Level baseLevel)
    {
        //var baseModel = MakeBaseModel();
        //baseLevel.Models[_laraSkinGuns] = baseModel;

        var map = new List<List<TRMesh>>();
        for (int i = 0; i < 16; i++)
            map.Add(baseLevel.Models[_laraSkin1].Meshes.GetRange(i * 15 + 1, 15));
        for (int i = 0; i < 8; i++)
            map.Add(baseLevel.Models[_laraSkin2].Meshes.GetRange(i * 15 + 1, 15));

        int xdiff = 84;
        int zdiff = 84;
        int z = 0;
        int rootX = 1064;
        int rootDiff = 300;
        var count = baseModel.Meshes.Count;

        if (false)
        {

            {
                // TR1 classic
                var rght = map[_laraClassic1][4].Clone();
                var left = map[_laraClassic1][1].Clone();
                foreach (var m in new[] { rght, left })
                {
                    m.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 13 || v > 20));
                    m.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 13 || v > 20));
                }

                baseModel.Meshes.Add(rght);
                baseModel.Meshes.Add(left);

                baseModel.MeshTrees.AddRange
                ([
                    new(){ OffsetX = -rootX, OffsetZ = 48, Flags = _read },
                new(){ OffsetX = -xdiff, Flags = _push },
                ]);
                z += zdiff;
                rootX += rootDiff;
            }

            {
                // TR1 gold
                var rght = map[_laraGold1][4].Clone();
                var left = map[_laraGold1][1].Clone();
                foreach (var m in new[] { rght, left })
                {
                    m.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 13 || v > 20));
                    m.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 13 || v > 20));
                }

                baseModel.Meshes.Add(rght);
                baseModel.Meshes.Add(left);

                baseModel.MeshTrees.AddRange
                ([
                    new(){ OffsetZ = z, Flags = _read },
                new(){ OffsetX = -xdiff, OffsetZ = z, Flags = _pop },
                ]);
                z = 0;
            }

            {
                // TR1 bacon
                var rght = map[_laraBacon][4].Clone();
                var left = map[_laraBacon][1].Clone();
                foreach (var m in new[] { rght, left })
                {
                    m.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 15));
                    m.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 15));
                }

                baseModel.Meshes.Add(rght);
                baseModel.Meshes.Add(left);

                baseModel.MeshTrees.AddRange
                ([
                    new(){ OffsetX = -rootX, OffsetZ = 48, Flags = _read },
                new(){ OffsetX = -xdiff, OffsetZ = z, Flags = _push },
                ]);
                z += zdiff;
            }

            {
                // TR1 golden bacon
                var rght = map[_laraGold2][4].Clone();
                var left = map[_laraGold2][1].Clone();
                foreach (var m in new[] { rght, left })
                {
                    m.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 15));
                    m.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 15));
                }

                baseModel.Meshes.Add(rght);
                baseModel.Meshes.Add(left);

                baseModel.MeshTrees.AddRange
                ([
                    new(){ OffsetZ = z, Flags = _read },
                new(){ OffsetX = -xdiff, OffsetZ = z, Flags = _pop },
                ]);
                z = 0;
                rootX += rootDiff;
            }

            {
                // TR2 classic
                var rght = baseModel.Meshes[29].Clone();
                var left = baseModel.Meshes[30].Clone();
                foreach (var m in new[] { rght, left })
                {
                    m.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v > 7));
                    m.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v > 7));
                }

                baseModel.Meshes.Add(rght);
                baseModel.Meshes.Add(left);

                baseModel.MeshTrees.AddRange
                ([
                    new(){ OffsetX = -rootX, OffsetZ = 48, Flags = _read },
                new(){ OffsetX = -xdiff, OffsetZ = z, Flags = _push },
                ]);
                z += zdiff;
            }

            {
                // TR2 gold
                var rght = map[_laraGold3][4].Clone();
                var left = map[_laraGold3][1].Clone();
                foreach (var m in new[] { rght, left })
                {
                    m.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 13 || v > 20));
                    m.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 13 || v > 20));
                }

                baseModel.Meshes.Add(rght);
                baseModel.Meshes.Add(left);

                baseModel.MeshTrees.AddRange
                ([
                    new(){ OffsetZ = z, Flags = _read },
                new(){ OffsetX = -xdiff, OffsetZ = z, Flags = _read },
                ]);
                z += zdiff;
            }

            {
                // TR2 Deagle
                var rght = baseModel.Meshes[35].Clone();
                rght.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v > 7));
                rght.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v > 7));

                baseModel.Meshes.Add(rght);

                baseModel.MeshTrees.AddRange
                ([
                    new(){ OffsetZ = z, Flags = _read },
                ]);
                z += zdiff;
            }

            {
                // TR2 uzis
                var rght = baseModel.Meshes[39].Clone();
                var left = baseModel.Meshes[40].Clone();
                foreach (var m in new[] { rght, left })
                {
                    m.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v > 7));
                    m.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v > 7));
                }

                baseModel.Meshes.Add(rght);
                baseModel.Meshes.Add(left);

                baseModel.MeshTrees.AddRange
                ([
                    new(){ OffsetZ = z, Flags = _read },
                new(){ OffsetX = -xdiff, OffsetZ = z, Flags = _pop },
                ]);
                z = 0;
                rootX += rootDiff;
            }

            {
                // TR3 classic
                var rght = baseModel.Meshes[42].Clone();
                var left = baseModel.Meshes[43].Clone();
                foreach (var m in new[] { rght, left })
                {
                    m.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v > 7));
                    m.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v > 7));
                }

                baseModel.Meshes.Add(rght);
                baseModel.Meshes.Add(left);

                baseModel.MeshTrees.AddRange
                ([
                    new(){ OffsetX = -rootX, OffsetZ = 48, Flags = _read },
                new(){ OffsetX = -xdiff, OffsetZ = z, Flags = _push },
                ]);
                z += zdiff;
            }

            {
                // TR3 Deagle
                var rght = baseModel.Meshes[48].Clone();
                rght.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v > 7));
                rght.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v > 7));

                baseModel.Meshes.Add(rght);

                baseModel.MeshTrees.AddRange
                ([
                    new(){ OffsetZ = z, Flags = _read },
                ]);
                z += zdiff;
            }

            {
                // TR3 uzis
                var rght = baseModel.Meshes[50].Clone();
                var left = baseModel.Meshes[51].Clone();
                foreach (var m in new[] { rght, left })
                {
                    m.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v > 7));
                    m.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v > 7));
                }

                baseModel.Meshes.Add(rght);
                baseModel.Meshes.Add(left);

                baseModel.MeshTrees.AddRange
                ([
                    new(){ OffsetZ = z, Flags = _read },
                new(){ OffsetX = -xdiff, OffsetZ = z, Flags = _pop },
                ]);
                z = 0;
                rootX += rootDiff;
            }

            {
                // TR3 nevada
                var rght = baseModel.Meshes[54].Clone();
                var left = baseModel.Meshes[55].Clone();
                foreach (var m in new[] { rght, left })
                {
                    m.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v > 7));
                    m.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v > 7));
                }

                baseModel.Meshes.Add(rght);
                baseModel.Meshes.Add(left);

                baseModel.MeshTrees.AddRange
                ([
                    new(){ OffsetX = -rootX, OffsetZ = 48, Flags = _read },
                new(){ OffsetX = -xdiff, OffsetZ = z, Flags = _push },
                ]);
                z += zdiff;
            }

            {
                // TR3 nevada Deagle
                var rght = baseModel.Meshes[60].Clone();
                rght.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v > 7));
                rght.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v > 7));

                baseModel.Meshes.Add(rght);

                baseModel.MeshTrees.AddRange
                ([
                    new(){ OffsetZ = z, Flags = _read },
                ]);
                z += zdiff;
            }

            {
                // TR3 nevada uzis
                var rght = baseModel.Meshes[62].Clone();
                var left = baseModel.Meshes[63].Clone();
                foreach (var m in new[] { rght, left })
                {
                    m.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v > 7));
                    m.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v > 7));
                }

                baseModel.Meshes.Add(rght);
                baseModel.Meshes.Add(left);

                baseModel.MeshTrees.AddRange
                ([
                    new(){ OffsetZ = z, Flags = _read },
                new(){ OffsetX = -xdiff, OffsetZ = z, Flags = _pop },
                ]);
                z += zdiff;
            }

            //{
            //    // TR3 antarc
            //    var rght = map[_laraAntarc][4].Clone();
            //    var left = map[_laraAntarc][1].Clone();
            //    foreach (var m in new[] { rght, left })
            //    {
            //        m.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 20));
            //        m.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 20));
            //    }

            //    baseModel.Meshes.Add(rght);
            //    baseModel.Meshes.Add(left);

            //    baseModel.MeshTrees.AddRange
            //    ([
            //        new(){ OffsetZ = z, Flags = _read },
            //        new(){ OffsetX = -xdiff, OffsetZ = z, Flags = _read },
            //    ]);
            //    z += zdiff;
            //}

            {
                // Wrap-up
                var frame = baseModel.Animations[0].Frames[0];
                int newFrames = baseModel.Meshes.Count - count;
                for (int i = 0; i < newFrames; i++)
                {
                    frame.Rotations.Add(new());
                    //CleanupVertices(baseModel.Meshes[i]);
                }
            }
        }

        {
            // TR1 gym
            foreach (var i in new[] { 1,4 })
            {
                var mesh = map[_laraGym1][i];
                mesh.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 20));
                mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 20));
                CleanupVertices(mesh);
            }
        }

        {
            // TR1 classic
            foreach (var i in new[] { 1, 4 })
            {
                var mesh = map[_laraClassic1][i];
                mesh.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                CleanupVertices(mesh);
            }
        }

        {
            // TR1 mauled
            foreach (var i in new[] { 1, 4 })
            {
                var mesh = map[_laraMauled][i];
                mesh.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                CleanupVertices(mesh);
            }
        }

        {
            // TR1 combo
            foreach (var i in new[] { 1, 4 })
            {
                var mesh = map[_laraCombo1][i];
                mesh.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                CleanupVertices(mesh);
            }
        }

        {
            // TR1 gold
            foreach (var i in new[] { 1, 4 })
            {
                var mesh = map[_laraGold1][i];
                mesh.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                CleanupVertices(mesh);
            }
        }

        {
            // bacon
            foreach (var i in new[] { 1, 4 })
            {
                var mesh = map[_laraBacon][i];
                mesh.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 15 && v <= 22));
                mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 15 && v <= 22));
                CleanupVertices(mesh);
            }
        }

        {
            // bacon gold
            foreach (var i in new[] { 1, 4 })
            {
                var mesh = map[_laraGold2][i];
                mesh.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 15 && v <= 22));
                mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 15 && v <= 22));
                CleanupVertices(mesh);
            }
        }

        {
            // TR2 gym
            foreach (var i in new[] { 1, 4 })
            {
                var mesh = map[_laraGym2][i];
                mesh.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 20));
                mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 20));
                CleanupVertices(mesh);
            }
        }

        {
            // TR2 classic
            foreach (var i in new[] { 1, 4 })
            {
                var mesh = map[_laraClassic2][i];
                mesh.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                CleanupVertices(mesh);
            }
        }

        {
            // TR2 diving
            foreach (var i in new[] { 1, 4 })
            {
                var mesh = map[_laraDiving][i];
                mesh.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                CleanupVertices(mesh);
            }
        }

        {
            // TR2 tibet
            foreach (var i in new[] { 1, 4 })
            {
                var mesh = map[_laraBomber][i];
                mesh.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                CleanupVertices(mesh);
            }
        }

        {
            // TR2 robe
            foreach (var i in new[] { 1, 4 })
            {
                var mesh = map[_laraRobe][i];
                mesh.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 18));
                mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 18));
                CleanupVertices(mesh);
            }
        }

        {
            // TR2 vegas
            foreach (var i in new[] { 1, 4 })
            {
                var mesh = map[_laraVegas][i];
                mesh.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                CleanupVertices(mesh);
            }
        }

        {
            // TR2 gold
            foreach (var i in new[] { 1, 4 })
            {
                var mesh = map[_laraGold3][i];
                mesh.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                CleanupVertices(mesh);
            }
        }

        {
            // TR3 gym
            foreach (var i in new[] { 1, 4 })
            {
                var mesh = map[_laraGym3][i];
                mesh.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                CleanupVertices(mesh);
            }
        }

        {
            // TR3 gym
            foreach (var i in new[] { 1, 4 })
            {
                var mesh = map[_laraGym3][i];
                mesh.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                CleanupVertices(mesh);
            }
        }

        {
            // TR3 classic
            foreach (var i in new[] { 1, 4 })
            {
                var mesh = map[_laraClassic3][i];
                mesh.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                CleanupVertices(mesh);
            }
        }

        {
            // TR3 SP
            foreach (var i in new[] { 1, 4 })
            {
                var mesh = map[_laraCoastal][i];
                mesh.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                CleanupVertices(mesh);
            }
        }

        {
            // TR3 London
            foreach (var i in new[] { 1, 4 })
            {
                var mesh = map[_laraLondon][i];
                mesh.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                CleanupVertices(mesh);
            }
        }

        {
            // TR3 Nevada
            foreach (var i in new[] { 1, 4 })
            {
                var mesh = map[_laraNevada][i];
                mesh.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                CleanupVertices(mesh);
            }
        }

        {
            // TR3 Antarc
            foreach (var i in new[] { 1, 4 })
            {
                var mesh = map[_laraAntarc][i];
                mesh.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 20));
                mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 20));
                CleanupVertices(mesh);
            }
        }

        {
            // Alt diving
            foreach (var i in new[] { 1, 4 })
            {
                var mesh = map[_laraDivingAlpha][i];
                mesh.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                CleanupVertices(mesh);
            }
        }

        {
            // N-Gage
            foreach (var i in new[] { 1, 4 })
            {
                var mesh = map[_laraNGage][i];
                mesh.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 13 && v <= 20));
                CleanupVertices(mesh);
            }

            {
                var mesh = map[_laraNGage][7];
                var vs = new ushort[] { 3,7,9,11, 2,6,8,10 };
                var tex = mesh.TexturedTriangles.Find(f => f.Vertices.All(vs.Contains)).Texture;
                mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(vs.Contains));

                var tinfo = baseLevel.ObjectTextures[tex];
                var newTinfoA = new TRObjectTexture(tinfo.Bounds);
                var newTinfoB = newTinfoA.Clone();
                newTinfoA.Atlas = newTinfoB.Atlas = tinfo.Atlas;
                newTinfoB.UVMode = TRUVMode.NE_AntiClockwise;
                baseLevel.ObjectTextures.Add(newTinfoA);
                baseLevel.ObjectTextures.Add(newTinfoB);

                mesh.TexturedRectangles.Add(new()
                {
                    Texture = (ushort)(baseLevel.ObjectTextures.Count - 2),
                    Type = TRFaceType.Rectangle,
                    Vertices = [11,9,3,7],
                });
                mesh.TexturedRectangles.Add(new()
                {
                    Texture = (ushort)(baseLevel.ObjectTextures.Count - 1),
                    Type = TRFaceType.Rectangle,
                    Vertices = [8,10,6,2],
                });

                vs = [5, 6, 8, 9];
                mesh = map[_laraNGage][0];
                tex = mesh.TexturedTriangles.Find(f => f.Vertices.All(vs.Contains)).Texture;
                mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(vs.Contains));

                tinfo = baseLevel.ObjectTextures[tex];

                var mesh2 = map[_laraClassic1][0];
                var goodFaces = mesh2.TexturedTriangles.FindAll(f => f.Vertices.All(vs.Contains));
                var goodTex = goodFaces.First().Texture;
                var goodInfo = baseLevel.ObjectTextures[goodTex];

                var tile = baseLevel.Images16[tinfo.Atlas];
                var img = new TRImage(tile.Pixels);
                var pic = img.Export(tinfo.Bounds);

                
                var newTile = new TRImage(256, 256);
                newTile.Import(pic, new(0, 0));
                baseLevel.Images16.Add(new() { Pixels = newTile.ToRGB555() });

                foreach (var goodFace in goodFaces)
                {
                    var ff = goodFace.Clone();
                    var newTinfo = baseLevel.ObjectTextures[ff.Texture].Clone();
                    newTinfo.Position = new(0, 0);
                    newTinfo.Atlas = (ushort)(baseLevel.Images16.Count - 1);
                    baseLevel.ObjectTextures.Add(newTinfo);
                    ff.Texture = (ushort)(baseLevel.ObjectTextures.Count - 1);
                    mesh.TexturedTriangles.Add(ff);
                }
            }

            {
                var mesh = map[_laraNGage][7];
                for (int i = 26; i < 30; i++)
                    mesh.Vertices[i].Z += 12;
            }

            {
                var mesh = map[_laraNGage][2];
                var vs = new ushort[] { 14,13,19,18 };
                mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(vs.Contains));
                vs = [15, 14, 20, 19];
                var tex = mesh.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains)).Texture;
                mesh.TexturedRectangles.Add(new()
                {
                    Type = TRFaceType.Rectangle,
                    Vertices = [14,13,18,19],
                    Texture = tex,
                });

                mesh = map[_laraNGage][5];
                vs = [14, 13, 19, 18 ];
                mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(vs.Contains));
                vs = [15, 14, 20, 19];
                tex = mesh.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains)).Texture;
                mesh.TexturedRectangles.Add(new()
                {
                    Type = TRFaceType.Rectangle,
                    Vertices = [13,14,19,18],
                    Texture = tex,
                });
            }

            {
                void ReplImg(int fid, ushort id)
                {
                    var tex = baseLevel.ObjectTextures[id];
                    var img = new TRImage(baseLevel.Images16[tex.Atlas].Pixels);
                    var file = @$"ngage\ng\{fid}.png";
                    if (File.Exists(file))
                    {
                        var seg = new TRImage(file);
                        img.Import(seg, tex.Position);
                        baseLevel.Images16[tex.Atlas].Pixels = img.ToRGB555();
                    }
                }
                var mesh = map[_laraNGage][0];
                var vs = new ushort[] { 9,19,7,17 };
                var f = mesh.TexturedRectangles.Find(ff => ff.Vertices.All(vs.Contains));
                ReplImg(21, f.Texture);
            }
        }

        if (false)
        {
            {
                // Strip holsters from pistols, magnums
                foreach (var i in new[] { 3, 4, 7, 8 })
                {
                    var m = baseModel.Meshes[i];
                    m.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 8));
                    m.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 8));
                }
                // Strip holsters from autos
                foreach (var i in new[] { 11, 12 })
                {
                    var m = baseModel.Meshes[i];
                    m.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 36 && v <= 43));
                    m.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 36 && v <= 43));
                }
                // Strip holsters from deagle
                foreach (var i in new[] { 14 })
                {
                    var m = baseModel.Meshes[i];
                    m.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v <= 7));
                    m.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v <= 7));
                }
                //TODO: delete mesh 15
                // Strip holsters from autos
                foreach (var i in new[] { 18, 19 })
                {
                    var m = baseModel.Meshes[i];
                    m.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 32 && v <= 39));
                    m.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 32 && v <= 39));
                }
            }

            // Strip TR2 holsters
            {
                // pistols, autos, deagle, uzis
                foreach (var i in new[] { 29, 30, 33, 34, 35, 39, 40 })
                {
                    var m = baseModel.Meshes[i];
                    m.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 8));
                    m.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 8));
                }
                // mags
                foreach (var i in new[] { 31, 32 })
                {
                    var m = baseModel.Meshes[i];
                    m.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 6 || v == 14 || v == 15));
                    m.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 6 || v == 14 || v == 15));
                }
                //TODO: delete mesh 36 (empty deagle)
            }

            // Strip TR3 holsters
            {
                // everything
                for (int i = 42; i < 52; i++)
                {
                    if (i == 49) continue;
                    var m = baseModel.Meshes[i];
                    m.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 8));
                    m.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 8));
                }
                //TODO: delete mesh 49 (empty deagle)

                // pistols, deagle, uzis
                foreach (var i in new[] { 54, 55, 60, 62, 63 })
                {
                    var m = baseModel.Meshes[i];
                    m.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 8 || v >= 30));
                    m.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 8 || v >= 30));
                }
                // mags
                foreach (var i in new[] { 56, 57 })
                {
                    var m = baseModel.Meshes[i];
                    m.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 16 && v <= 23));
                    m.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 16 && v <= 23));
                }
                // autos
                foreach (var i in new[] { 58, 59 })
                {
                    var m = baseModel.Meshes[i];
                    m.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 44 && v <= 51));
                    m.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 44 && v <= 51));
                }
                //TODO: delete 61
            }
        }
    }

    static void DoGuns(TR2Level baseLevel)
    {
        var baseModel = MakeBaseModel();
        baseLevel.Models[_laraSkinGuns] = baseModel;

        {
            // TR1
            foreach (var type in new[] { TR1Type.LaraPistolAnim_H, TR1Type.LaraMagnumAnim_H, (TR1Type)213, (TR1Type)216, TR1Type.LaraUziAnimation_H })
            {
                var isauto = (int)type == 213;
                var isdeag = (int)type == 216;
                var caves = _reader1.Read(isauto ? "auto.phd" : (isdeag ? "deagle_tr1.phd" : @"F:\tomp\all levels\tr1\level1.phd"));
                var model = caves.Models[type];
                model.Meshes =
                [
                    model.Meshes[10],
                    model.Meshes[13],

                    model.Meshes[4],
                    model.Meshes[1],
                ];

                if (type == TR1Type.LaraPistolAnim_H)
                {
                    FixTR1Pistols(model, caves);
                }
                else if (type == TR1Type.LaraMagnumAnim_H)
                {
                    FixTR1Magnums(model, caves);
                }
                else if (isdeag)
                {
                    model.Meshes.RemoveAt(1);
                }
                else if (type == TR1Type.LaraUziAnimation_H)
                {
                    FixTR1Uzis(model, caves);
                }

                if (isdeag)
                {
                    DeleteHands(model.Meshes[0]);
                    DeleteThighs(model.Meshes[1], model.Meshes[2]);
                }
                else
                {
                    DeleteHands(model.Meshes[0], model.Meshes[1]);
                    DeleteThighs(model.Meshes[2], model.Meshes[3]);
                }

                Import(baseLevel, baseModel, caves, model, null);
            }

            baseModel.MeshTrees.AddRange
            ([
                // TR1 Pistols hands
                new(){ OffsetX = 95, OffsetY = -200, Flags = _push },
                new(){ OffsetX = -185, Flags = _push },

                // TR1 Pistols legs
                new(){ OffsetX = -42, OffsetY = 200, OffsetZ = 48, Flags = _read },
                new(){ OffsetX = -142, OffsetY = 200, OffsetZ = 48, Flags = _read },

                // TR1 Magnums hands
                new(){ OffsetZ = 128, Flags = _read },
                new(){ OffsetX = -185, OffsetZ = 128, Flags = _read },

                // TR1 Magnums legs
                new(){ OffsetX = -42, OffsetY = 200, OffsetZ = 128+48, Flags = _read },
                new(){ OffsetX = -142, OffsetY = 200, OffsetZ = 128+48, Flags = _read },

                // TR1 Autos hands
                new(){ OffsetZ = 256, Flags = _read },
                new(){ OffsetX = -185, OffsetZ = 256, Flags = _read },

                // TR1 Autos legs
                new(){ OffsetX = -42, OffsetY = 200, OffsetZ = 256+48, Flags = _read },
                new(){ OffsetX = -142, OffsetY = 200, OffsetZ = 256+48, Flags = _read },

                // TR1 Deagle hand
                new(){ OffsetZ = 384, Flags = _read },

                // TR1 Deagle legs
                new(){ OffsetX = -42, OffsetY = 200, OffsetZ = 384+48, Flags = _read },
                new(){ OffsetX = -142, OffsetY = 200, OffsetZ = 384+48, Flags = _read },

                // TR1 Uzis hands
                new(){ OffsetZ = 544, Flags = _read },
                new(){ OffsetX = -185, OffsetZ = 544, Flags = _read },

                // TR1 Uzis legs
                new(){ OffsetX = -42, OffsetY = 200, OffsetZ = 544+48, Flags = _read },
                new(){ OffsetX = -142, OffsetY = 200, OffsetZ = 544+48, Flags = _read },
            ]);

            {
                var caves = _reader1.Read(@"F:\tomp\all levels\tr1\level1.phd");
                var model = caves.Models[TR1Type.LaraShotgunAnim_H];
                model.Meshes =
                [
                    model.Meshes[10],
                ];
                FixTR1Shotgun(model, caves);
                DeleteHands(model.Meshes[0]);

                Import(baseLevel, baseModel, caves, model, null);
                baseModel.MeshTrees.Add(new() { OffsetZ = 672, Flags = _read });
            }

            {
                var wall = _reader2.Read(@"F:\tomp\all levels\tr2\wall.tr2");
                var model = wall.Models[TR2Type.LaraFlareAnim_H];
                model.Meshes =
                [
                    model.Meshes[13],
                ];
                FixFlare(model, wall);
                DeleteHands(model.Meshes[0]);

                Import(baseLevel, baseModel, wall, model, null);
                baseModel.MeshTrees.Add(new() { OffsetX = -185, OffsetZ = 672, Flags = _read });
            }

            {
                var wall = _reader2.Read(@"F:\tomp\all levels\tr2\wall.tr2");
                var model = wall.Models[TR2Type.LaraM16Anim_H];
                model.Meshes =
                [
                    model.Meshes[10],
                ];
                FixM16(model, wall);
                DeleteHands(model.Meshes[0]);

                Import(baseLevel, baseModel, wall, model, null);
                baseModel.MeshTrees.Add(new() { OffsetZ = 800, Flags = _read });
            }

            {
                var wall = _reader3.Read(@"F:\tomp\all levels\tr3\jungle.tr2");
                var model = wall.Models[TR3Type.LaraMP5Animation_H];
                model.Meshes =
                [
                    model.Meshes[10],
                ];
                //FixM16(model, wall);
                DeleteHands(model.Meshes[0]);

                Import(baseLevel, baseModel, wall, model, null);
                baseModel.MeshTrees.Add(new() { OffsetX = -172, OffsetZ = 800, Flags = _read });
            }

            {
                var wall = _reader2.Read(@"F:\tomp\all levels\tr2\wall.tr2");
                var model = wall.Models[TR2Type.LaraGrenadeAnim_H];
                model.Meshes =
                [
                    model.Meshes[10],
                ];
                FixGrenade(model, wall);
                DeleteHands(model.Meshes[0]);

                Import(baseLevel, baseModel, wall, model, null);
                baseModel.MeshTrees.Add(new() { OffsetZ = 968, Flags = _read });
            }

            {
                var wall = _reader2.Read(@"F:\tomp\all levels\tr2\wall.tr2");
                var model = wall.Models[TR2Type.LaraHarpoonAnim_H];
                model.Meshes =
                [
                    model.Meshes[10],
                ];
                FixHarpoon(model, wall);
                DeleteHands(model.Meshes[0]);

                Import(baseLevel, baseModel, wall, model, null);
                baseModel.MeshTrees.Add(new() { OffsetX = -172, OffsetZ = 968, Flags = _read });
            }

            {
                var wall = _reader3.Read(@"F:\tomp\all levels\tr3\jungle.tr2");
                var model = wall.Models[TR3Type.LaraRocketAnimation_H];
                model.Meshes =
                [
                    model.Meshes[10],
                ];
                //FixGrenade(model, wall);
                DeleteHands(model.Meshes[0]);

                Import(baseLevel, baseModel, wall, model, null);
                baseModel.MeshTrees.Add(new() { OffsetY = 132, OffsetZ = 968+128, Flags = _pop });
            }
        }

        {
            // TR2
            foreach (var type in new[] { TR2Type.LaraPistolAnim_H, (TR2Type)279, TR2Type.LaraAutoAnim_H, (TR2Type)285, TR2Type.LaraUziAnim_H})
            {
                var ismag = (int)type == 279;
                var isdea = (int)type == 285;
                var wall = _reader2.Read(ismag ? "mags.tr2" : (isdea ? "deagle.tr2" : @"F:\tomp\all levels\tr2\wall.tr2"));
                var model = wall.Models[type];
                model.Meshes =
                [
                    model.Meshes[10],
                    model.Meshes[13],
                    model.Meshes[4],
                    model.Meshes[1],
                ];

                if (type == TR2Type.LaraPistolAnim_H)
                {
                    FixTR2Pistols(model, wall);
                }
                else if (type == TR2Type.LaraAutoAnim_H)
                {
                    FixTR2Autos(model, wall);
                }
                else if (isdea)
                {
                    model.Meshes.RemoveAt(1);
                }
                else if (type == TR2Type.LaraUziAnim_H)
                {
                    FixTR2Uzis(model, wall);
                }

                if (isdea)
                {
                    DeleteHands(model.Meshes[0]);
                    DeleteDeagleThighs(model.Meshes[1], model.Meshes[2]);
                }
                else
                {
                    DeleteHands(model.Meshes[0], model.Meshes[1]);
                    DeleteThighs(model.Meshes[2], model.Meshes[3]);
                }

                Import(baseLevel, baseModel, wall, model, null);
            }

            {
                // Duplicate Autos hands into TR1, get rid of dupes
                baseModel.Meshes[9] = baseModel.Meshes[35].Clone();
                baseModel.Meshes[10] = baseModel.Meshes[36].Clone();
                baseModel.Meshes.RemoveAt(39);
                baseModel.Meshes.RemoveAt(36);
                baseModel.Meshes.RemoveAt(35);
                baseModel.Meshes.RemoveAt(32);
                baseModel.Meshes.RemoveAt(31);
            }

            baseModel.MeshTrees.AddRange
            ([
                // TR2 Pistols hands
                new(){ OffsetX = -170, OffsetY = -200, Flags = _read },
                new(){ OffsetX = -185, Flags = _push },

                // TR2 Pistols legs
                new(){ OffsetX = -42, OffsetY = 200, OffsetZ = 48, Flags = _read },
                new(){ OffsetX = -142, OffsetY = 200, OffsetZ = 48, Flags = _read },

                // TR2 Magnums legs
                new(){ OffsetX = -42, OffsetY = 200, OffsetZ = 128+48, Flags = _read },
                new(){ OffsetX = -142, OffsetY = 200, OffsetZ = 128+48, Flags = _read },

                // TR2 Autos legs
                new(){ OffsetX = -42, OffsetY = 200, OffsetZ = 256+48, Flags = _read },
                new(){ OffsetX = -142, OffsetY = 200, OffsetZ = 256+48, Flags = _read },

                // TR2 Deagle legs
                new(){ OffsetX = -42, OffsetY = 200, OffsetZ = 384+48, Flags = _read },
                new(){ OffsetX = -142, OffsetY = 200, OffsetZ = 384+48, Flags = _read },

                // TR2 Uzis hands
                new(){ OffsetZ = 544, Flags = _read },
                new(){ OffsetX = -185, OffsetZ = 544, Flags = _read },

                // TR2 Uzis legs
                new(){ OffsetX = -42, OffsetY = 200, OffsetZ = 544+48, Flags = _read },
                new(){ OffsetX = -142, OffsetY = 200, OffsetZ = 544+48, Flags = _read },
            ]);

            {
                var wall = _reader2.Read(@"F:\tomp\all levels\tr2\wall.tr2");
                var model = wall.Models[TR2Type.LaraShotgunAnim_H];
                model.Meshes =
                [
                    model.Meshes[10],
                ];
                FixTR2Shotgun(model, wall);
                DeleteHands(model.Meshes[0]);

                Import(baseLevel, baseModel, wall, model, null);
                baseModel.MeshTrees.Add(new() { OffsetZ = 672, Flags = _pop });
            }
        }

        {
            // TR3
            foreach (var type in new[] { TR3Type.LaraPistolAnimation_H, TR3Type.LaraDeagleAnimation_H, TR3Type.LaraUziAnimation_H })
            {
                var wall = _reader3.Read(@"F:\tomp\all levels\tr3\jungle.tr2");
                var model = wall.Models[type];
                model.Meshes =
                [
                    model.Meshes[10],
                    model.Meshes[13],
                    model.Meshes[4],
                    model.Meshes[1],
                ];

                if (type == TR3Type.LaraDeagleAnimation_H)
                {
                    model.Meshes.RemoveAt(1);
                    FixDeagle(model.Meshes[0], wall.Models[TR3Type.Deagle_M_H].Meshes[0]);
                }

                if (type == TR3Type.LaraDeagleAnimation_H)
                {
                    DeleteHands(model.Meshes[0]);
                    DeleteDeagleThighs(model.Meshes[1], model.Meshes[2]);
                }
                else
                {
                    DeleteHands(model.Meshes[0], model.Meshes[1]);
                    DeleteThighs(model.Meshes[2], model.Meshes[3]);
                }

                Import(baseLevel, baseModel, wall, model, null);

                if (type == TR3Type.LaraPistolAnimation_H)
                {
                    // Just clone TR2's autos and mags, then replace textures later
                    for (int i = 31; i < 35; i++)
                    {
                        baseModel.Meshes.Add(baseModel.Meshes[i].Clone());
                    }
                }
            }

            {
                // Copy TR3 deagle to TR1
                baseModel.Meshes[13] = baseModel.Meshes[50].Clone();

                // Remove uzi and deagle hands
                baseModel.Meshes.RemoveAt(54);
                baseModel.Meshes.RemoveAt(53);
                baseModel.Meshes.RemoveAt(50);

                // No pistol hands
                baseModel.Meshes.RemoveAt(43);
                baseModel.Meshes.RemoveAt(42);
            }

            baseModel.MeshTrees.AddRange
            ([
                // TR3 Pistols legs
                new(){ OffsetX = -482, OffsetZ = 48, Flags = _read },
                new(){ OffsetX = -110, Flags = _push },

                // TR3 Magnums legs
                new(){ OffsetZ = 128, Flags = _read },
                new(){ OffsetX = -110, OffsetZ = 128, Flags = _read },

                // TR3 Autos legs
                new(){ OffsetZ = 256, Flags = _read },
                new(){ OffsetX = -110, OffsetZ = 256, Flags = _read },

                // TR3 Deagle legs
                new(){ OffsetZ = 384, Flags = _read },
                new(){ OffsetX = -110, OffsetZ = 384, Flags = _read },

                // TR2 Uzis legs
                new(){ OffsetZ = 544, Flags = _read },
                new(){ OffsetX = -110, OffsetZ = 544, Flags = _read },
            ]);

            {
                var caves = _reader3.Read(@"F:\tomp\all levels\tr3\jungle.tr2");
                var model = caves.Models[TR3Type.LaraShotgunAnimation_H];
                model.Meshes =
                [
                    model.Meshes[10],
                ];
                FixTR3Shotgun(model, caves);
                DeleteHands(model.Meshes[0]);

                Import(baseLevel, baseModel, caves, model, null);
                baseModel.MeshTrees.Add(new() { OffsetX = 48, OffsetY = -200, OffsetZ = 624, Flags = _read });
            }

            {
                var caves = _reader3.Read(@"F:\tomp\all levels\tr3\jungle.tr2");
                var model = caves.Models[TR3Type.LaraGrenadeAnimation_H];
                model.Meshes =
                [
                    model.Meshes[10],
                ];
                FixTR3Grenade(model, caves);
                DeleteHands(model.Meshes[0]);

                Import(baseLevel, baseModel, caves, model, null);
                baseModel.MeshTrees.Add(new() { OffsetX = -148, OffsetY = -200, OffsetZ = 624, Flags = _pop });
            }

            foreach (var type in new[] { TR3Type.LaraPistolAnimation_H, TR3Type.LaraDeagleAnimation_H, TR3Type.LaraUziAnimation_H })
            {
                var caves = _reader3.Read(@"F:\tomp\all levels\tr3\nevada.tr2");
                var model = caves.Models[type];
                model.Meshes =
                [
                    model.Meshes[4],
                    model.Meshes[1],
                ];
                DeleteNevadaThighs(model.Meshes[0]);
                if (type == TR3Type.LaraDeagleAnimation_H)
                {
                    DeleteThighs(model.Meshes[1]);
                }
                else
                {
                    DeleteNevadaThighs(model.Meshes[1]);
                }

                Import(baseLevel, baseModel, caves, model, null);

                if (type == TR3Type.LaraPistolAnimation_H)
                {
                    // Clone autos and mags for now
                    for (int i = 44; i < 48; i++)
                    {
                        baseModel.Meshes.Add(baseModel.Meshes[i].Clone());
                    }
                }
            }

            {
                var caves = _reader3.Read(@"F:\tomp\all levels\tr3\antarc.tr2");
                var model = caves.Models[TR3Type.LaraDeagleAnimation_H];
                model.Meshes =
                [
                    model.Meshes[4],
                ];
                DeleteNevadaThighs(model.Meshes[0]);

                Import(baseLevel, baseModel, caves, model, null);

                //10-17
                var m = baseModel.Meshes[^1].TexturedFaces.First(f => f.Vertices.All(v => v >= 20 && v <= 27));
                var m2 = baseModel.Meshes[60].TexturedFaces.First(f => f.Vertices.All(v => v >= 18 && v <= 25));

                var ot1 = baseLevel.ObjectTextures[m.Texture];
                var ot2 = baseLevel.ObjectTextures[m2.Texture];
                var img1 = new TRImage(baseLevel.Images16[ot1.Atlas].Pixels).Export(ot1.Bounds);
                var tile2 = new TRImage(baseLevel.Images16[ot2.Atlas].Pixels);
                tile2.Import(img1, ot2.Position);
                baseLevel.Images16[ot2.Atlas].Pixels = tile2.ToRGB555();

                baseModel.Meshes.RemoveAt(baseModel.Meshes.Count - 1);
            }

            baseModel.MeshTrees.AddRange
            ([
                // Nevada Pistols legs
                new(){ OffsetX = -748, OffsetZ = 48, Flags = _read },
                new(){ OffsetX = -110, Flags = _push },

                // Nevada Magnums legs
                new(){ OffsetZ = 128, Flags = _read },
                new(){ OffsetX = -110, OffsetZ = 128, Flags = _read },

                // Nevada Autos legs
                new(){ OffsetZ = 256, Flags = _read },
                new(){ OffsetX = -110, OffsetZ = 256, Flags = _read },

                // Nevada Deagle legs
                new(){ OffsetZ = 384, Flags = _read },
                new(){ OffsetX = -110, OffsetZ = 384, Flags = _read },

                // Nevada Uzis legs
                new(){ OffsetZ = 544, Flags = _read },
                new(){ OffsetX = -110, OffsetZ = 544, Flags = _pop },
            ]);
        }

        

        var frame = baseModel.Animations[0].Frames[0];
        for (int i = 1; i < baseModel.Meshes.Count; i++)
        {
            frame.Rotations.Add(new());
            CleanupVertices(baseModel.Meshes[i]);
            baseModel.Meshes[i].SelfCalculateBounds();
        }

        PostGunFixes(baseModel, baseLevel);

        FixNevada(baseModel, baseLevel);

        DoHolsters(baseModel, baseLevel);

        // Repeate after final fixes
        for (int i = 1; i < baseModel.Meshes.Count; i++)
        {
            CleanupVertices(baseModel.Meshes[i]);
        }
    }

    public static void CleanupVertices(TRMesh mesh)
    {
        var newVerts = mesh.TexturedFaces.SelectMany(f => f.Vertices.Select(v => mesh.Vertices[v]))
            .Distinct().ToList();
        var newNormals = mesh.TexturedFaces.SelectMany(f => f.Vertices.Select(v => mesh.Normals[v]))
            .Distinct().ToList();
        foreach (var face in mesh.TexturedFaces)
        {
            for (int j = 0; j < face.Vertices.Count; j++)
            {
                face.Vertices[j] = (ushort)newVerts.IndexOf(mesh.Vertices[face.Vertices[j]]);
            }
        }

        mesh.Vertices = newVerts;
        mesh.Normals = newNormals;
    }

    static void DoBackGuns(TR2Level level)
    {
        {
            var model = level.Models[_laraSkinGuns1];
            {
                var shifted = _reader2.Read("tr1shifted.tr2");
                var shotty = shifted.Models[(TR2Type)(int)TR1Type.LaraShotgunAnim_H];
                shotty.Meshes = [shotty.Meshes[14]];
                Import(level, model, shifted, shotty, null);

                model.MeshTrees.Add(new() { OffsetX = -1900, OffsetY = -200, OffsetZ = 38, Flags = _read });
                model.Meshes[^1].SelfCalculateBounds();
            }
            int z = 128;
            foreach (var type in new[] { 201, 219, 202, 203, 222 }) // M16, MP5, Grenade, Harpoon, Rocket
            {
                var shifted = _reader2.Read("tr1shifted.tr2");
                var shotty = shifted.Models[(TR2Type)type];
                shotty.Meshes = [shotty.Meshes[14]];
                Import(level, model, shifted, shotty, null);

                model.MeshTrees.Add(new() { OffsetZ = z, Flags = type == 201 ? _push : _read });
                model.Meshes[^1].SelfCalculateBounds();
                z += 128;
            }
            model.MeshTrees[^1].Flags = _pop;
        }

        {
            var model = level.Models[_laraSkinGuns2];
            {
                var shifted = _reader2.Read("tr2shifted.tr2");
                var shotty = shifted.Models[TR2Type.LaraShotgunAnim_H];
                shotty.Meshes = [shotty.Meshes[14]];
                Import(level, model, shifted, shotty, null);

                model.MeshTrees.Add(new() { OffsetX = -1900, OffsetY = -200, OffsetZ = 38, Flags = _read });
                model.Meshes[^1].SelfCalculateBounds();
            }
            int z = 128;
            foreach (var type in new[] { 6, 290, 7, 8, 295 }) // M16, MP5, Grenade, Harpoon, Rocket
            {
                var shifted = _reader2.Read("tr2shifted.tr2");
                var shotty = shifted.Models[(TR2Type)type];
                shotty.Meshes = [shotty.Meshes[14]];
                Import(level, model, shifted, shotty, null);

                model.MeshTrees.Add(new() { OffsetZ = z, Flags = type == 6 ? _push : _read });
                model.Meshes[^1].SelfCalculateBounds();
                z += 128;
            }
            model.MeshTrees[^1].Flags = _pop;
        }

        {
            var model = level.Models[_laraSkinGuns3];
            {
                var shifted = _reader2.Read("tr3shifted.tr2");
                var shotty = shifted.Models[(TR2Type)(int)TR3Type.LaraShotgunAnimation_H];
                shotty.Meshes = [shotty.Meshes[14]];
                Import(level, model, shifted, shotty, null);

                model.MeshTrees.Add(new() { OffsetX = -1900, OffsetY = -200, OffsetZ = 38, Flags = _read });
                model.Meshes[^1].SelfCalculateBounds();
            }
            int z = 128;
            foreach (var type in new[] { 388, 6, 8, 9, 7 }) // M16, MP5, Grenade, Harpoon, Rocket
            {
                var shifted = _reader2.Read("tr3shifted.tr2");
                var shotty = shifted.Models[(TR2Type)type];
                shotty.Meshes = [shotty.Meshes[14]];
                Import(level, model, shifted, shotty, null);

                model.MeshTrees.Add(new() { OffsetZ = z, Flags = type == 388 ? _push : _read });
                model.Meshes[^1].SelfCalculateBounds();
                z += 128;
            }
            model.MeshTrees[^1].Flags = _pop;
        }

        foreach (var type in new[] { _laraSkinGuns1, _laraSkinGuns2, _laraSkinGuns3 })
        {
            var model = level.Models[type];
            var gunHand = model.Meshes[67];
            var gunBack = model.Meshes[73];
            var map = new Dictionary<ushort[], ushort[]>
            {
                [[0,1,2,3]] = [0,1,2,3],
                [[5,3,4,2]] = [5,1,0,4],
                [[6,5,4,7]] = [6,5,4,7],
                [[0,3,5,6]] = [1,2,5,6],
                [[20, 21,17,18]] = [17, 14, 18,19],
                [[17,18,8,9]] = [18,19,8,10],
            };
            foreach (var (hand, back) in map)
            {
                var backFace = gunBack.TexturedRectangles.Find(f => f.Vertices.All(back.Contains));
                var handFace = gunHand.TexturedRectangles.Find(f => f.Vertices.All(hand.Contains));
                backFace.Texture = handFace.Texture;
            }

            var vs = new ushort[] { 5, 1, 0, 4 };
            gunBack.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains)).Rotate(1);
            vs = [22,23,24,25];
            gunBack.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains)).Rotate(1);

            vs = [20,18,22,24];
            gunBack.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains)).Rotate(2);
            vs = [20,21,24,25];
            gunBack.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains)).Rotate(2);
            vs = [21,25,23,19];
            gunBack.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains)).Rotate(2);
            vs = [18,19,22,23];
            gunBack.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains)).Rotate(2);
            vs = [18,19,10,8];
            gunBack.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains)).Rotate(2);
            
            vs = [38,39,41,42];
            gunBack.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains)).Rotate(1);
            vs = [48,42,41,43];
            gunBack.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains)).Rotate(1);
            vs = [45,48,40,43];
            gunBack.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains)).Rotate(3);

            map = new Dictionary<ushort[], ushort[]>
            {
                [[0, 12, 13, 14]] = [16,19,13,14],
                [[16,13,12,15]] = [12,13,14,15],
                [[4,15,16,17]] = [4,12,15,20],
                [[0, 4,13,16]] = [16,4,12,13],
                [[12,14,17,15]] = [15,14,20,19],
                [[26,30,31,35]] = [30,37,31,36],
                [[30,29,34,35]] = [37,35,36,34],
                [[29,28,34,33]] = [35,33,32,34],
                [[28,27,32,33]] = [28,29,32,33],
                [[27,26,31,32]] = [29,30,28,31]
            };
            foreach (var (back, hand) in map)
            {
                var backFace = gunBack.TexturedRectangles.Find(f => f.Vertices.All(back.Contains));
                var handFace = gunHand.TexturedRectangles.Find(f => f.Vertices.All(hand.Contains));
                handFace.Texture = backFace.Texture;
            }

            vs = [13,16,19,14];
            gunHand.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains)).Rotate(1);
            vs = [4,20,12,15];
            gunHand.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains)).Rotate(3);

            if (type == _laraSkinGuns3)
                break;


            gunHand = model.Meshes[69];
            gunBack = model.Meshes[75];
            map = new Dictionary<ushort[], ushort[]>
            {
                [[19,15,5,17]] = [19,23,22,16],
                [[17,5,0,16]] = [16,22,20,17],
                [[12,18,16,0]] = [21,18,17,20],
                [[12,15,18,19]] = [21,23,18,19],
                [[16,17,18,19]] = [16, 17, 18, 19],
            };
            foreach (var (back, hand) in map)
            {
                var backFace = gunBack.TexturedRectangles.Find(f => f.Vertices.All(back.Contains));
                var handFace = gunHand.TexturedRectangles.Find(f => f.Vertices.All(hand.Contains));
                handFace.Texture = backFace.Texture;
            }

            vs = [16, 17, 18, 19];
            gunBack.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains)).Rotate(3);
            gunHand.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains)).Rotate(2);
        }
    }

    static void FixNevada(TRModel model, TR2Level level)
    {
        {
            //var baseModel3 = level.Models[(TR2Type)272];
            //// Align TR3 gym and Nevada holsters with the empties
            //var map = new Dictionary<int, int>
            //{
            //    [13] = 0,
            //    [16] = 1,
            //    [15] = 3,
            //    [14] = 2,
            //};
            //foreach (var m in new[] { 1, /*4*/ })
            //{
            //    var hols = model.Meshes[m == 1 ? 55 : 54];
            //    var legG = baseModel3.Meshes[1 + m];
            //    var legN = baseModel3.Meshes[61 + m];
            //    foreach (var  (a,b) in map)
            //    {
            //        legG.Vertices[a] = hols.Vertices[b].Clone();
            //        legN.Vertices[a] = hols.Vertices[b].Clone();
            //        legG.Normals[a] = hols.Normals[b].Clone();
            //        legN.Normals[a] = hols.Normals[b].Clone();
            //    }
            //}
        }
    }

    const int _laraGym1 = 0;
    const int _laraClassic1 = 1;
    const int _laraMauled = 2;
    const int _laraCombo1 = 3;
    const int _laraGold1 = 4;
    const int _laraBacon = 5;
    const int _laraGold2 = 6;
    const int _laraGym2 = 7;
    const int _laraClassic2 = 8;
    const int _laraDiving = 9;
    const int _laraBomber = 10;
    const int _laraRobe = 11;
    const int _laraVegas = 12;
    const int _laraGold3 = 13;
    const int _laraGym3 = 14;
    const int _laraClassic3 = 15;
    const int _laraCoastal = 16;
    const int _laraLondon = 17;
    const int _laraNevada = 18;
    const int _laraAntarc = 19;
    const int _laraLeigh = 20;
    const int _laraLeighGold = 21;
    const int _laraDivingAlpha = 22;
    const int _laraNGage = 23;
    const TR2Type _laraSkin1 = (TR2Type)270;
    const TR2Type _laraSkin2 = (TR2Type)271;
    const TR2Type _laraSkinExtra = (TR2Type)272;
    const TR2Type _laraSkinGuns1 = (TR2Type)273;
    const TR2Type _laraSkinGuns2 = (TR2Type)274;
    const TR2Type _laraSkinGuns3 = (TR2Type)275;
    const TR2Type _laraSkinGuns = (TR2Type)276;
    const TR2Type _laraSkinLegs = (TR2Type)277;


    static void SortGuns(TR2Level level)
    {
        SortGuns1(level);
        SortGuns2(level);
        SortGuns3(level);

        DoBackGuns(level);

        foreach (var t in new[] { _laraSkinGuns1, _laraSkinGuns2, _laraSkinGuns3 })
        {
            var model = level.Models[t];
            var frame = model.Animations[0].Frames[0];
            for (int i = 1; i < model.Meshes.Count; i++)
                frame.Rotations.Add(new());

            // Fix Magnum rots
            for (int i = 0; i < 5; i++)
            {
                if (i == 1) continue;
                var mesh = model.Meshes[i * 11 + 1 + 5];
                ushort[] vs;
                if (t == _laraSkinGuns1 || i == 3 || i == 4)
                {
                    vs = [4, 5, 6, 7];
                }
                else
                {
                    vs = [10, 11, 12, 13];
                }
                var face = mesh.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains));
                face.Rotate(3);
            }

            if (t == _laraSkinGuns2 || t == _laraSkinGuns3)
            {
                var nevPistols = model.Meshes[48];
                nevPistols.TexturedRectangles.RemoveAll(f => f.Vertices.Any(v => v == 30 || v == 32));
                nevPistols.TexturedTriangles.RemoveAll(f => f.Vertices.Any(v => v == 30 || v == 32));
                CleanupVertices(nevPistols);
            }
        }

        level.Models.Remove(_laraSkinGuns);
    }

    static void SortGuns1(TR2Level level)
    {
        var baseModel = MakeBaseModel();
        level.Models[_laraSkinGuns1] = baseModel;
        var gunModel = level.Models[_laraSkinGuns];

        int z = 0;
        int zdiff = 150;
        {
            // Unarmed
            var right = gunModel.Meshes[3].Clone();
            var left = gunModel.Meshes[4].Clone();
            foreach (var m in new[] { left, right })
            {
                m.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 8));
                m.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 8));
                CleanupVertices(m);
            }

            baseModel.Meshes.Add(right);
            baseModel.Meshes.Add(left);
            baseModel.MeshTrees.Add(new() { OffsetX = 42, Flags = _push });
            baseModel.MeshTrees.Add(new() { OffsetX = -84, Flags = _push });
            z += zdiff;
        }

        {
            // Pistol holsters
            baseModel.Meshes.Add(gunModel.Meshes[3].Clone());
            baseModel.Meshes.Add(gunModel.Meshes[4].Clone());
            baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
            baseModel.MeshTrees.Add(new() { OffsetX = -84, OffsetZ = z, Flags = _read });
            z += zdiff;
        }

        {
            // Magnum holsters
            baseModel.Meshes.Add(gunModel.Meshes[7].Clone());
            baseModel.Meshes.Add(gunModel.Meshes[8].Clone());
            baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
            baseModel.MeshTrees.Add(new() { OffsetX = -84, OffsetZ = z, Flags = _read });
            z += zdiff;
        }

        {
            // Autos holsters
            baseModel.Meshes.Add(gunModel.Meshes[11].Clone());
            baseModel.Meshes.Add(gunModel.Meshes[12].Clone());
            baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
            baseModel.MeshTrees.Add(new() { OffsetX = -84, OffsetZ = z, Flags = _read });
            z += zdiff;
        }

        {
            // Deagle holster
            baseModel.Meshes.Add(gunModel.Meshes[14].Clone());
            baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
            z += zdiff;
        }

        {
            // Uzis holsters
            baseModel.Meshes.Add(gunModel.Meshes[18].Clone());
            baseModel.Meshes.Add(gunModel.Meshes[19].Clone());
            baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
            baseModel.MeshTrees.Add(new() { OffsetX = -84, OffsetZ = z, Flags = _pop });
            z += zdiff;
        }

        z = 108;

        
        
        // Gold
        z = 0;
        int j = 1;
        for (int i = 0; i < 6; i++)
        {
            var meshes = new List<TRMesh>() { baseModel.Meshes[j].Clone() };
            if (i != 4)
                meshes.Add(baseModel.Meshes[j + 1].Clone());

            var goldTex = level.Models[_laraSkin1].Meshes[61].TexturedFaces.First().Texture;
            foreach (var f in meshes.SelectMany(m => m.TexturedFaces))//.Where(f => IsFlat(GetImg(f, level)))))
                f.Texture = goldTex;

            baseModel.Meshes.AddRange(meshes);
            baseModel.MeshTrees.Add(new() { OffsetX = i == 0 ? -238 : 0, OffsetZ = z, Flags = _read });
            if (meshes.Count > 1)
                baseModel.MeshTrees.Add(new() { OffsetX = -84, OffsetZ = z, Flags = i == 0 ? _push : _read });

            j += meshes.Count;
            z += zdiff;
        }
        baseModel.MeshTrees[^1].Flags = _pop;

        // TR2
        z = 0;
        j = 1;
        for (int i = 0; i < 6; i++)
        {
            var meshes = new List<TRMesh>() { baseModel.Meshes[j].Clone() };
            if (i != 4)
                meshes.Add(baseModel.Meshes[j + 1].Clone());

            GunExtras.MakeTR2_TR1(meshes, gunModel, i);

            baseModel.Meshes.AddRange(meshes);
            baseModel.MeshTrees.Add(new() { OffsetX = i == 0 ? -524 : 0, OffsetZ = z, Flags = _read });
            if (meshes.Count > 1)
                baseModel.MeshTrees.Add(new() { OffsetX = -84, OffsetZ = z, Flags = i == 0 ? _push : _read });

            j += meshes.Count;
            z += zdiff;
        }
        baseModel.MeshTrees[^1].Flags = _pop;

        // TR3 A
        z = 0;
        j = 1;
        for (int i = 0; i < 6; i++)
        {
            var meshes = new List<TRMesh>() { baseModel.Meshes[j].Clone() };
            if (i != 4)
                meshes.Add(baseModel.Meshes[j + 1].Clone());

            GunExtras.MakeTR3A_TR1(meshes, gunModel, i);

            baseModel.Meshes.AddRange(meshes);
            baseModel.MeshTrees.Add(new() { OffsetX = i == 0 ? -806 : 0, OffsetZ = z, Flags = _read });
            if (meshes.Count > 1)
                baseModel.MeshTrees.Add(new() { OffsetX = -84, OffsetZ = z, Flags = i == 0 ? _push : _read });

            j += meshes.Count;
            z += zdiff;
        }
        baseModel.MeshTrees[^1].Flags = _pop;

        // TR3 B
        z = 0;
        j = 1;
        {
            // Sigh
            var mesh = gunModel.Meshes[61];
            var f = mesh.TexturedTriangles.Find(f => f.Vertices.All(v => v >= 4));
            var tin = level.ObjectTextures[f.Texture];
            level.ObjectTextures.Add(new(tin.Bounds)
            {
                Atlas = tin.Atlas,
            });
            mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 4));
            mesh.TexturedRectangles.Add(new()
            {
                Type = TRFaceType.Rectangle,
                Texture = (ushort)(level.ObjectTextures.Count - 1),
                Vertices = [6,7,5,4],
            });
        }
        for (int i = 0; i < 6; i++)
        {
            var meshes = new List<TRMesh>() { baseModel.Meshes[j].Clone() };
            if (i != 4)
                meshes.Add(baseModel.Meshes[j + 1].Clone());

            GunExtras.MakeTR3B_TR1(meshes, gunModel, i);

            baseModel.Meshes.AddRange(meshes);
            baseModel.MeshTrees.Add(new() { OffsetX = i == 0 ? -1084 : 0, OffsetZ = z, Flags = _read });
            if (meshes.Count > 1)
                baseModel.MeshTrees.Add(new() { OffsetX = -84, OffsetZ = z, Flags = i == 0 ? _push : _read });

            j += meshes.Count;
            z += zdiff;
        }
        baseModel.MeshTrees[^1].Flags = _pop;

        z = 0;
        {
            // Pistol hands
            baseModel.Meshes.Add(gunModel.Meshes[1].Clone());
            baseModel.Meshes.Add(gunModel.Meshes[2].Clone());
            baseModel.MeshTrees.Add(new() { OffsetX = -1334, OffsetY = -128, OffsetZ = z, Flags = _read });
            baseModel.MeshTrees.Add(new() { OffsetX = -84, OffsetZ = z, Flags = _push });
            z += zdiff;
        }
        {
            // Magnum hands
            baseModel.Meshes.Add(gunModel.Meshes[5].Clone());
            baseModel.Meshes.Add(gunModel.Meshes[6].Clone());
            baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
            baseModel.MeshTrees.Add(new() { OffsetX = -84, OffsetZ = z, Flags = _read });
            z += zdiff;
        }
        {
            // Autos hands
            baseModel.Meshes.Add(gunModel.Meshes[9].Clone());
            baseModel.Meshes.Add(gunModel.Meshes[10].Clone());
            baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
            baseModel.MeshTrees.Add(new() { OffsetX = -84, OffsetZ = z, Flags = _read });
            z += zdiff;
        }
        {
            // Deagle hand
            baseModel.Meshes.Add(gunModel.Meshes[13].Clone());
            baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
            z += zdiff;
        }
        {
            // Autos hands
            baseModel.Meshes.Add(gunModel.Meshes[16].Clone());
            baseModel.Meshes.Add(gunModel.Meshes[17].Clone());
            baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
            baseModel.MeshTrees.Add(new() { OffsetX = -84, OffsetZ = z, Flags = _pop });
            z += zdiff;
        }

        z = -24;
        {
            // Shotgun, flare
            baseModel.Meshes.Add(gunModel.Meshes[20].Clone());
            baseModel.Meshes.Add(gunModel.Meshes[21].Clone());
            baseModel.MeshTrees.Add(new() { OffsetX = -1334-200, OffsetY = -200, OffsetZ = z, Flags = _read });
            baseModel.MeshTrees.Add(new() { OffsetX = -148, Flags = _push });
            z += zdiff;
        }
        {
            // M16, MP5
            baseModel.Meshes.Add(gunModel.Meshes[22].Clone());
            baseModel.Meshes.Add(gunModel.Meshes[23].Clone());
            baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
            baseModel.MeshTrees.Add(new() { OffsetX = -148, OffsetZ = z, Flags = _read });
            z += zdiff;
        }
        {
            // Grenade, harpoon
            baseModel.Meshes.Add(gunModel.Meshes[24].Clone());
            baseModel.Meshes.Add(gunModel.Meshes[25].Clone());
            baseModel.MeshTrees.Add(new() { OffsetZ = z+50, Flags = _read });
            baseModel.MeshTrees.Add(new() { OffsetX = -148, OffsetZ = z+50, Flags = _read });
            z += zdiff;
        }
        {
            // Rocket
            baseModel.Meshes.Add(gunModel.Meshes[26].Clone());
            baseModel.MeshTrees.Add(new() { OffsetY = 200, OffsetZ = z, Flags = _pop });
            z += zdiff;
        }


        {
            // Yet another texture glitch - single transparent pixel on pistols in hands
            var pthighr = baseModel.Meshes[3];
            var goodFace = pthighr.TexturedRectangles.Find(f => f.Vertices.All(v => v >= 4 && v < 8));

            var phandr = baseModel.Meshes[56];
            var badFace = phandr.TexturedRectangles.Find(f => f.Vertices.All(v => v >= 12 && v < 16));

            var goodImg = GetImg(goodFace, level);
            var tinfo = level.ObjectTextures[badFace.Texture];
            var tile = new TRImage(level.Images16[tinfo.Atlas].Pixels);
            tile.Import(goodImg, tinfo.Position);
            level.Images16[tinfo.Atlas].Pixels = tile.ToRGB555();
        }
        {
            // And another, magnums wrong rot
        }
    }

    static TRImage GetImg(TRMeshFace f, TR2Level level)
    {
        var obj = level.ObjectTextures[f.Texture];
        var tile = new TRImage(level.Images16[obj.Atlas].Pixels);
        return tile.Export(obj.Bounds);
    }

    static bool IsFlat(TRImage img)
    {
        bool flat = true;
        var c0 = img.GetPixel(0, 0);
        img.Read((c, x, y) => flat &= c == c0);
        return flat;
    }

    static void FixHolsteredTR23Uzis(TRModel model, TR2Level level)
    {
        var left = model.Meshes[55];
        var t = new ushort[] { 5,6,7 };
        left.TexturedTriangles.Find(f => f.Vertices.All(t.Contains)).Rotate(2);
        t = [4, 5, 7];
        left.TexturedTriangles.Find(f => f.Vertices.All(t.Contains)).Rotate(1);
        left.Vertices[11].Y++;

        var right = model.Meshes[54];
        t = [10, 12, 8, 13,5,4,0,2];
        right.TexturedTriangles.RemoveAll(f => f.Vertices.All(t.Contains));
        t = [10, 12, 8, 13, 7,4,0,2];

        var map = new Dictionary<ushort, ushort>
        {
            [12] = 10,
            [8] = 12,
            [10] = 13,
            [13] = 8,
            [2] = 5,
            [4] = 0,
            [7] = 2,
            [0] = 4,
        };
        var faces = left.TexturedTriangles.FindAll(f => f.Vertices.All(map.ContainsKey));
        foreach (var face in faces)
        {
            var ff = face.Clone();
            for (int i = 0; i < ff.Vertices.Count; i++)
                ff.Vertices[i] = map[ff.Vertices[i]];
            right.TexturedTriangles.Add(ff);
        }

        t = [24, 25, 28, 29];
        right.TexturedTriangles.FindAll(f => f.Vertices.All(t.Contains)).ForEach(f => f.Rotate(1));

        map = new Dictionary<ushort, ushort>
        {
            [24] = 28,
            [28] = 25,
            [29] = 24,
            [25] = 29,
        };
        left.TexturedTriangles.RemoveAll(f => f.Vertices.All(map.ContainsKey));
        faces = right.TexturedTriangles.FindAll(f => f.Vertices.All(map.ContainsKey));
        foreach (var face in faces)
        {
            var ff = face.Clone();
            for (int i = 0; i < ff.Vertices.Count; i++)
                ff.Vertices[i] = map[ff.Vertices[i]];
            left.TexturedTriangles.Add(ff);
        }

        ushort oldTex;
        {
            var tr3Mesh = model.Meshes[44];
            t = [4,5,6,7];
            var tex = tr3Mesh.TexturedRectangles.Find(f => f.Vertices.All(t.Contains)).Texture;
            oldTex = tex;

            tr3Mesh.TexturedTriangles.Clear();
            tr3Mesh.TexturedRectangles.Clear();
            tr3Mesh.Vertices.Clear();
            tr3Mesh.Normals.Clear();

            tr3Mesh.TexturedRectangles.AddRange(left.TexturedRectangles.Select(f => f.Clone()));
            tr3Mesh.TexturedTriangles.AddRange(left.TexturedTriangles.Select(f => f.Clone()));
            tr3Mesh.Normals.AddRange(left.Vertices.Select(f => f.Clone()));
            tr3Mesh.Vertices.AddRange(left.Vertices.Select(f => f.Clone()));

            tr3Mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 8));
            var uvs = new List<List<ushort>>
            {
                new() { 5,6,7,4 },
                new() { 6,1,0,7 },
                new() { 1,3,2,0 },
                new() { 3,5,4,2 },
                new() { 2,4,7,0 },
                new() { 1,6,5,3 },
            };
            tr3Mesh.TexturedRectangles.AddRange(uvs.Select(v => new TRMeshFace
            {
                Vertices = v,
                Texture = tex,
                Type = TRFaceType.Rectangle,
            }));

            tr3Mesh = model.Meshes[43];
            tr3Mesh.TexturedTriangles.Clear();
            tr3Mesh.TexturedRectangles.Clear();
            tr3Mesh.Vertices.Clear();
            tr3Mesh.Normals.Clear();

            tr3Mesh.TexturedRectangles.AddRange(right.TexturedRectangles.Select(f => f.Clone()));
            tr3Mesh.TexturedTriangles.AddRange(right.TexturedTriangles.Select(f => f.Clone()));
            tr3Mesh.Normals.AddRange(right.Vertices.Select(f => f.Clone()));
            tr3Mesh.Vertices.AddRange(right.Vertices.Select(f => f.Clone()));

            tr3Mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 8));
            uvs =
            [
                [7,6,5,4],
                [3,7,4,2],
                [1, 3, 2, 0],
                [6, 1, 0, 5],
                [7, 3, 1, 6],
                [5, 0, 2, 4],
            ];
            tr3Mesh.TexturedRectangles.AddRange(uvs.Select(v => new TRMeshFace
            {
                Vertices = v,
                Texture = tex,
                Type = TRFaceType.Rectangle,
            }));
        }

        {
            var tr2Mesh = model.Meshes[33];
            t = [4, 5, 6, 7];
            var tex = tr2Mesh.TexturedRectangles.Find(f => f.Vertices.All(t.Contains)).Texture;

            tr2Mesh = model.Meshes[33] = model.Meshes[44].Clone();
            tr2Mesh.TexturedRectangles.FindAll(f => f.Texture == oldTex).ForEach(f => f.Texture = tex);

            tr2Mesh = model.Meshes[32] = model.Meshes[43].Clone();
            tr2Mesh.TexturedRectangles.FindAll(f => f.Texture == oldTex).ForEach(f => f.Texture = tex);
        }
        {
            var goldMesh = model.Meshes[22];
            t = [4, 5, 6, 7];
            var tex = goldMesh.TexturedRectangles.Find(f => f.Vertices.All(t.Contains)).Texture;

            goldMesh = model.Meshes[22] = model.Meshes[44].Clone();
            goldMesh.TexturedRectangles.ForEach(f => f.Texture = tex);
            goldMesh.TexturedTriangles.ForEach(f => f.Texture = tex);

            goldMesh = model.Meshes[21] = model.Meshes[43].Clone();
            goldMesh.TexturedRectangles.ForEach(f => f.Texture = tex);
            goldMesh.TexturedTriangles.ForEach(f => f.Texture = tex);
        }
        {
            var tr1Mesh = model.Meshes[11];
            t = [4, 5, 6, 7];
            var tex = tr1Mesh.TexturedRectangles.Find(f => f.Vertices.All(t.Contains)).Texture;

            tr1Mesh = model.Meshes[11] = model.Meshes[44].Clone();
            tr1Mesh.TexturedRectangles.FindAll(f => f.Texture == oldTex).ForEach(f => f.Texture = tex);

            tr1Mesh = model.Meshes[10] = model.Meshes[43].Clone();
            tr1Mesh.TexturedRectangles.FindAll(f => f.Texture == oldTex).ForEach(f => f.Texture = tex);
        }

        // The actual guns in hands, final thing?
        {
            var mesh = model.Meshes[64];
            t = [17, 18, 22, 23];
            var face = mesh.TexturedRectangles.Find(f => f.Vertices.All(t.Contains));
            t = [15, 8, 11, 13];
            mesh.TexturedRectangles.Find(f => f.Vertices.All(t.Contains)).Texture = face.Texture;
            t = [13,11,10,12];
            mesh.TexturedRectangles.Find(f => f.Vertices.All(t.Contains)).Texture = face.Texture;
            t = [12,10,9,14];
            mesh.TexturedRectangles.Find(f => f.Vertices.All(t.Contains)).Texture = face.Texture;

            t = [0,7,1,6];
            face = mesh.TexturedRectangles.Find(f => f.Vertices.All(t.Contains));
            t = [5,3,2,4];
            mesh.TexturedRectangles.Find(f => f.Vertices.All(t.Contains)).Texture = face.Texture;

            map = new()
            {
                [39] = 31,
                [35] = 27,
                [32] = 24,
                [38] = 30,
                [36] = 29,
                [34] = 26,
                [33] = 25,
                [37] = 28,
            };
            mesh.TexturedRectangles.RemoveAll(f => f.Vertices.All(map.ContainsValue));
            mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(map.ContainsValue));
            foreach (var pointFace in left.TexturedTriangles.Where(f => f.Vertices.All(map.ContainsKey)))
            {
                var ff = pointFace.Clone();
                for (int i = 0; i < ff.Vertices.Count; i++)
                    ff.Vertices[i] = map[ff.Vertices[i]];
                mesh.TexturedTriangles.Add(ff);
            }
        }

        {
            var mesh = model.Meshes[63];
            t = [31,24,30,27];
            var face = mesh.TexturedRectangles.Find(f => f.Vertices.All(t.Contains));
            t = [9,15,10,13];
            mesh.TexturedRectangles.Find(f => f.Vertices.All(t.Contains)).Texture = face.Texture;
            t = [10,13,12,11];
            mesh.TexturedRectangles.Find(f => f.Vertices.All(t.Contains)).Texture = face.Texture;
            t = [11,12,8,14];
            mesh.TexturedRectangles.Find(f => f.Vertices.All(t.Contains)).Texture = face.Texture;

            t = [0, 7, 1, 6];
            face = mesh.TexturedRectangles.Find(f => f.Vertices.All(t.Contains));
            t = [5, 3, 2, 4];
            mesh.TexturedRectangles.Find(f => f.Vertices.All(t.Contains)).Texture = face.Texture;
            mesh.TexturedRectangles.Find(f => f.Vertices.All(t.Contains)).Rotate(2);

            map = new()
            {
                [33] = 18,
                [39] = 23,
                [38] = 22,
                [32] = 17,
                [35] = 19,
                [36] = 21,
                [37] = 20,
                [34] = 16,
            };
            mesh.TexturedRectangles.RemoveAll(f => f.Vertices.All(map.ContainsValue));
            mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(map.ContainsValue));
            foreach (var pointFace in right.TexturedTriangles.Where(f => f.Vertices.All(map.ContainsKey)))
            {
                var ff = pointFace.Clone();
                for (int i = 0; i < ff.Vertices.Count; i++)
                    ff.Vertices[i] = map[ff.Vertices[i]];
                mesh.TexturedTriangles.Add(ff);
            }
        }
    }

    static void FixTR23Pistols(TRModel model, TR2Level level)
    {
        ushort tex = 0;
        foreach (var i in new[] { 4,26,37 })
        {
            var vs = new ushort[] { 26, 27, 15, 12 };
            var face = model.Meshes[i].TexturedRectangles.Find(f => f.Vertices.All(vs.Contains));
            face.Rotate(2);
            tex = face.Texture;
        }

        var mesh = model.Meshes[48];
        var t = new ushort[] { 13,11,12,14 };
        var f = mesh.TexturedTriangles.RemoveAll(f => f.Vertices.All(t.Contains));
        mesh.TexturedRectangles.Add(new()
        {
            Type = TRFaceType.Rectangle,
            Vertices = [13,11,12,14],
            Texture = tex,
        });
        mesh.TexturedRectangles[^1].Rotate(1);
    }

    static void SortGuns2(TR2Level level)
    {
        var baseModel = MakeBaseModel();
        level.Models[_laraSkinGuns2] = baseModel;
        var gunModel = level.Models[_laraSkinGuns];

        int z = 0;
        int zdiff = 150;
        int x = 42;
        int xdiff = 280;
        for (int i = 0; i < 3; i++)
        {
            {
                // Unarmed
                var right = gunModel.Meshes[29].Clone();
                var left = gunModel.Meshes[30].Clone();
                foreach (var m in new[] { left, right })
                {
                    m.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 8));
                    m.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 8));
                    CleanupVertices(m);
                }

                baseModel.Meshes.Add(right);
                baseModel.Meshes.Add(left);
                baseModel.MeshTrees.Add(new() { OffsetX = x, Flags = i == 0 ? _push : _read });
                baseModel.MeshTrees.Add(new() { OffsetX = -84, Flags = _push });
                z += zdiff;
            }

            {
                // Pistol holsters
                baseModel.Meshes.Add(gunModel.Meshes[29].Clone());
                baseModel.Meshes.Add(gunModel.Meshes[30].Clone());
                baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
                baseModel.MeshTrees.Add(new() { OffsetX = -84, OffsetZ = z, Flags = _read });
                z += zdiff;
            }

            {
                // Magnum holsters
                baseModel.Meshes.Add(gunModel.Meshes[31].Clone());
                baseModel.Meshes.Add(gunModel.Meshes[32].Clone());
                baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
                baseModel.MeshTrees.Add(new() { OffsetX = -84, OffsetZ = z, Flags = _read });
                z += zdiff;
            }

            {
                // Autos holsters
                baseModel.Meshes.Add(gunModel.Meshes[33].Clone());
                baseModel.Meshes.Add(gunModel.Meshes[34].Clone());
                baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
                baseModel.MeshTrees.Add(new() { OffsetX = -84, OffsetZ = z, Flags = _read });
                z += zdiff;
            }

            {
                // Deagle holster
                baseModel.Meshes.Add(gunModel.Meshes[35].Clone());
                baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
                z += zdiff;
            }

            {
                // Uzis holsters
                baseModel.Meshes.Add(gunModel.Meshes[39].Clone());
                baseModel.Meshes.Add(gunModel.Meshes[40].Clone());
                baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
                baseModel.MeshTrees.Add(new() { OffsetX = -84, OffsetZ = z, Flags = _pop });
                z += zdiff;
            }

            z = 0;
            x -= xdiff;
        }

        {
            // TR3
            {
                // Unarmed
                var right = gunModel.Meshes[42].Clone();
                var left = gunModel.Meshes[43].Clone();
                foreach (var m in new[] { left, right })
                {
                    m.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 8));
                    m.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 8));
                    CleanupVertices(m);
                }

                baseModel.Meshes.Add(right);
                baseModel.Meshes.Add(left);
                baseModel.MeshTrees.Add(new() { OffsetX = x, Flags = _read });
                baseModel.MeshTrees.Add(new() { OffsetX = -84, Flags = _push });
                z += zdiff;
            }

            {
                // Pistol holsters
                baseModel.Meshes.Add(gunModel.Meshes[42].Clone());
                baseModel.Meshes.Add(gunModel.Meshes[43].Clone());
                baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
                baseModel.MeshTrees.Add(new() { OffsetX = -84, OffsetZ = z, Flags = _read });
                z += zdiff;
            }

            {
                // Magnum holsters
                baseModel.Meshes.Add(gunModel.Meshes[44].Clone());
                baseModel.Meshes.Add(gunModel.Meshes[45].Clone());
                baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
                baseModel.MeshTrees.Add(new() { OffsetX = -84, OffsetZ = z, Flags = _read });
                z += zdiff;
            }

            {
                // Autos holsters
                baseModel.Meshes.Add(gunModel.Meshes[46].Clone());
                baseModel.Meshes.Add(gunModel.Meshes[47].Clone());
                baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
                baseModel.MeshTrees.Add(new() { OffsetX = -84, OffsetZ = z, Flags = _read });
                z += zdiff;
            }

            {
                // Deagle holster
                baseModel.Meshes.Add(gunModel.Meshes[48].Clone());
                baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
                z += zdiff;
            }

            {
                // Uzis holsters
                baseModel.Meshes.Add(gunModel.Meshes[50].Clone());
                baseModel.Meshes.Add(gunModel.Meshes[51].Clone());
                baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
                baseModel.MeshTrees.Add(new() { OffsetX = -84, OffsetZ = z, Flags = _pop });
                z += zdiff;
            }

            z = 0;
            x -= xdiff;

            {
                // Unarmed
                var right = gunModel.Meshes[54].Clone();
                var left = gunModel.Meshes[55].Clone();
                foreach (var m in new[] { left, right })
                {
                    m.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 8));
                    m.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 8));
                    CleanupVertices(m);
                }

                baseModel.Meshes.Add(right);
                baseModel.Meshes.Add(left);
                baseModel.MeshTrees.Add(new() { OffsetX = x, Flags = _read });
                baseModel.MeshTrees.Add(new() { OffsetX = -84, Flags = _push });
                z += zdiff;
            }

            {
                // Pistol holsters
                baseModel.Meshes.Add(gunModel.Meshes[54].Clone());
                baseModel.Meshes.Add(gunModel.Meshes[55].Clone());
                baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
                baseModel.MeshTrees.Add(new() { OffsetX = -84, OffsetZ = z, Flags = _read });
                z += zdiff;
            }

            {
                // Magnum holsters
                baseModel.Meshes.Add(gunModel.Meshes[56].Clone());
                baseModel.Meshes.Add(gunModel.Meshes[57].Clone());
                baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
                baseModel.MeshTrees.Add(new() { OffsetX = -84, OffsetZ = z, Flags = _read });
                z += zdiff;
            }

            {
                // Autos holsters
                baseModel.Meshes.Add(gunModel.Meshes[58].Clone());
                baseModel.Meshes.Add(gunModel.Meshes[59].Clone());
                baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
                baseModel.MeshTrees.Add(new() { OffsetX = -84, OffsetZ = z, Flags = _read });
                z += zdiff;
            }

            {
                // Deagle holster
                baseModel.Meshes.Add(gunModel.Meshes[60].Clone());
                baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
                z += zdiff;
            }

            {
                // Uzis holsters
                baseModel.Meshes.Add(gunModel.Meshes[62].Clone());
                baseModel.Meshes.Add(gunModel.Meshes[63].Clone());
                baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
                baseModel.MeshTrees.Add(new() { OffsetX = -84, OffsetZ = z, Flags = _pop });
                z += zdiff;
            }
        }

        {
            int j = 1;
            for (int i = 0; i < 6; i++)
            {
                var meshes = new List<TRMesh>() { baseModel.Meshes[j] };
                if (i != 4)
                    meshes.Add(baseModel.Meshes[j + 1]);

                GunExtras.MakeTR1_TR2(meshes, gunModel, i);
                j += meshes.Count;
            }

            for (int i = 0; i < 6; i++)
            {
                var meshes = new List<TRMesh>() { baseModel.Meshes[j] };
                if (i != 4)
                    meshes.Add(baseModel.Meshes[j + 1]);

                var goldTex = level.Models[_laraSkin1].Meshes[61].TexturedFaces.First().Texture;
                foreach (var f in meshes.SelectMany(m => m.TexturedFaces))
                    f.Texture = goldTex;

                j += meshes.Count;
            }
        }

        z = 0;
        {
            // Pistol hands
            baseModel.Meshes.Add(gunModel.Meshes[27].Clone());
            baseModel.Meshes.Add(gunModel.Meshes[28].Clone());
            baseModel.MeshTrees.Add(new() { OffsetX = -1334, OffsetY = -128, OffsetZ = z, Flags = _read });
            baseModel.MeshTrees.Add(new() { OffsetX = -84, OffsetZ = z, Flags = _push });
            z += zdiff;
        }
        {
            // Magnum hands
            baseModel.Meshes.Add(gunModel.Meshes[5].Clone());
            baseModel.Meshes.Add(gunModel.Meshes[6].Clone());
            baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
            baseModel.MeshTrees.Add(new() { OffsetX = -84, OffsetZ = z, Flags = _read });
            z += zdiff;
        }
        {
            // Autos hands
            baseModel.Meshes.Add(gunModel.Meshes[9].Clone());
            baseModel.Meshes.Add(gunModel.Meshes[10].Clone());
            baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
            baseModel.MeshTrees.Add(new() { OffsetX = -84, OffsetZ = z, Flags = _read });
            z += zdiff;
        }
        {
            // Deagle hand
            baseModel.Meshes.Add(gunModel.Meshes[13].Clone());
            baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
            z += zdiff;
        }
        {
            // Uzis hands
            baseModel.Meshes.Add(gunModel.Meshes[37].Clone());
            baseModel.Meshes.Add(gunModel.Meshes[38].Clone());
            baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
            baseModel.MeshTrees.Add(new() { OffsetX = -84, OffsetZ = z, Flags = _pop });
            z += zdiff;
        }

        z = -24;
        {
            // Shotgun, flare
            baseModel.Meshes.Add(gunModel.Meshes[41].Clone());
            baseModel.Meshes.Add(gunModel.Meshes[21].Clone());
            baseModel.MeshTrees.Add(new() { OffsetX = -1334 - 200, OffsetY = -200, OffsetZ = z, Flags = _read });
            baseModel.MeshTrees.Add(new() { OffsetX = -148, Flags = _push });
            z += zdiff;
        }
        {
            // M16, MP5
            baseModel.Meshes.Add(gunModel.Meshes[22].Clone());
            baseModel.Meshes.Add(gunModel.Meshes[23].Clone());
            baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
            baseModel.MeshTrees.Add(new() { OffsetX = -148, OffsetZ = z, Flags = _read });
            z += zdiff;
        }
        {
            // Grenade, harpoon
            baseModel.Meshes.Add(gunModel.Meshes[24].Clone());
            baseModel.Meshes.Add(gunModel.Meshes[25].Clone());
            baseModel.MeshTrees.Add(new() { OffsetZ = z + 50, Flags = _read });
            baseModel.MeshTrees.Add(new() { OffsetX = -148, OffsetZ = z + 50, Flags = _read });
            z += zdiff;
        }
        {
            // Rocket
            baseModel.Meshes.Add(gunModel.Meshes[26].Clone());
            baseModel.MeshTrees.Add(new() { OffsetY = 200, OffsetZ = z, Flags = _pop });
            z += zdiff;
        }

        FixHolsteredTR23Uzis(baseModel, level);
        FixTR23Pistols(baseModel, level);
    }

    static void SortGuns3(TR2Level level)
    {
        var baseModel = level.Models[_laraSkinGuns3] = level.Models[_laraSkinGuns2].Clone();
        var gunModel = level.Models[_laraSkinGuns];
        baseModel.Meshes[65] = gunModel.Meshes[52].Clone(); // Shotgun
        baseModel.Meshes[69] = gunModel.Meshes[53].Clone(); // Grenade
    }

    static void DoLegs(TR2Level level)
    {
        var baseModel = MakeBaseModel();
        level.Models[_laraSkinLegs] = baseModel;
        var skin1 = level.Models[_laraSkin1];
        var skin2 = level.Models[_laraSkin2];

        Directory.CreateDirectory("legs");

        ushort[] vs;
        List<TRMeshFace> GetFaces(TRMesh mesh)
        {
            return [.. mesh.TexturedFaces.Where(f => f.Vertices.All(vs.Contains))];
        }

        void Run(params TRMesh[] meshes)
        {
            var faces = meshes.SelectMany(m => GetFaces(m)).ToList();
            var texes = faces.Select(f => f.Texture).Distinct().ToList();
            var map = new Dictionary<ushort, ushort>();
            foreach (var tex in texes)
            {
                var newTex = DoLegFace(tex, level);
                if (newTex != ushort.MaxValue)
                    map[tex] = newTex;
            }

            faces.ForEach(f =>
            {
                if (map.TryGetValue(f.Texture, out ushort newTex))
                    f.Texture = newTex;
            });
        }

        int z = 0;
        int zdiff = 150;
        {
            // TR1 regular and combo
            var right = skin1.Meshes[20].Clone();
            var left = skin1.Meshes[17].Clone();
            
            vs = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
            Run(right, left);

            baseModel.Meshes.Add(right);
            baseModel.Meshes.Add(left);
            baseModel.MeshTrees.Add(new() { OffsetX = 42, Flags = _push });
            baseModel.MeshTrees.Add(new() { OffsetX = -88, Flags = _push });
            z += zdiff;
        }

        {
            // TR1 mauled
            var right = skin1.Meshes[35].Clone();
            var left = skin1.Meshes[32].Clone();

            vs = [0, 1, 2, 3, 4, 5, 6, 7, 8, 11];
            Run(right);
            vs = [0, 1, 2, 3, 4, 7, 8, 9, 10, 11];
            Run(left);

            baseModel.Meshes.Add(right);
            baseModel.Meshes.Add(left);
            baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
            baseModel.MeshTrees.Add(new() { OffsetX = -88, OffsetZ = z, Flags = _read });
            z += zdiff;
        }

        {
            // TR2 gym
            var right = skin1.Meshes[110].Clone();
            var left = skin1.Meshes[107].Clone();

            vs = [0, 1, 2, 3, 4, 13, 14, 15, 17, 6, 8];
            Run(right);
            vs = [0, 1, 2, 3, 4, 13, 14, 16, 17, 6, 8];
            Run(left);

            baseModel.Meshes.Add(right);
            baseModel.Meshes.Add(left);
            baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
            baseModel.MeshTrees.Add(new() { OffsetX = -88, OffsetZ = z, Flags = _read });
            z += zdiff;
        }

        {
            // TR2 classic+bomber
            var right = skin1.Meshes[125].Clone();
            var left = skin1.Meshes[122].Clone();

            vs = [0, 1, 2, 3, 7, 5, 8, 9, 16, 17];
            Run(right);
            vs = [0, 1, 2, 3, 4, 7, 8, 9, 10, 17];
            Run(left);

            baseModel.Meshes.Add(right);
            baseModel.Meshes.Add(left);
            baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
            baseModel.MeshTrees.Add(new() { OffsetX = -88, OffsetZ = z, Flags = _read });
            z += zdiff;
        }

        {
            // TR2 robe
            var right = skin1.Meshes[170].Clone();
            var left = skin1.Meshes[167].Clone();

            vs = [8,9,10,11,12,13,14,15,16,17];
            Run(right, left);

            baseModel.Meshes.Add(right);
            baseModel.Meshes.Add(left);
            baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
            baseModel.MeshTrees.Add(new() { OffsetX = -88, OffsetZ = z, Flags = _read });
            z += zdiff;
        }

        {
            // TR2 vegas
            var right = skin1.Meshes[185].Clone();
            var left = skin1.Meshes[182].Clone();

            vs = [0, 1, 2, 3, 5, 7, 8, 9, 16, 17];
            Run(right);
            vs = [0, 1, 2, 3, 10, 17, 8, 9, 6, 7];
            Run(left);

            baseModel.Meshes.Add(right);
            baseModel.Meshes.Add(left);
            baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
            baseModel.MeshTrees.Add(new() { OffsetX = -88, OffsetZ = z, Flags = _read });
            z += zdiff;
        }

        {
            // TR3 gym
            var right = skin1.Meshes[215];
            var left = skin1.Meshes[212];
            {
                right.TexturedTriangles.Add(new()
                {
                    Type = TRFaceType.Triangle,
                    Vertices = [16,13,17],
                    Texture = right.TexturedTriangles.First().Texture,
                });
                right.TexturedTriangles.Add(new()
                {
                    Type = TRFaceType.Triangle,
                    Vertices = [17,13,15],
                    Texture = right.TexturedTriangles.First().Texture,
                });
                right.TexturedTriangles.Add(new()
                {
                    Type = TRFaceType.Triangle,
                    Vertices = [16,14,13],
                    Texture = right.TexturedTriangles.First().Texture,
                });
                left.TexturedTriangles.Add(new()
                {
                    Type = TRFaceType.Triangle,
                    Vertices = [17, 15, 16],
                    Texture = left.TexturedTriangles.First().Texture,
                });
                left.TexturedTriangles.Add(new()
                {
                    Type = TRFaceType.Triangle,
                    Vertices = [17, 14, 15],
                    Texture = left.TexturedTriangles.First().Texture,
                });
            }

            right = right.Clone();
            left = left.Clone();

            vs = [0, 1, 2, 3, 4, 5, 6, 7, 10, 11];
            Run(right);
            vs = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
            Run(left);

            baseModel.Meshes.Add(right);
            baseModel.Meshes.Add(left);
            baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
            baseModel.MeshTrees.Add(new() { OffsetX = -88, OffsetZ = z, Flags = _read });
            z += zdiff;
        }

        {
            // TR3 classic
            var right = skin1.Meshes[230].Clone();
            var left = skin1.Meshes[227].Clone();

            vs = [0, 1, 2, 3, 5, 7, 8, 9, 16, 17];
            Run(right);
            vs = [0, 1, 2, 3, 4, 7, 8, 9, 10, 17];
            Run(left);

            baseModel.Meshes.Add(right);
            baseModel.Meshes.Add(left);
            baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
            baseModel.MeshTrees.Add(new() { OffsetX = -88, OffsetZ = z, Flags = _read });
            z += zdiff;
        }

        {
            // TR3 SP
            var right = skin2.Meshes[5].Clone();
            var left = skin2.Meshes[2].Clone();

            vs = [0, 1, 2, 3, 5, 7, 8, 9, 16, 17];
            Run(right);
            vs = [0, 1, 2, 3, 4, 7, 8, 9, 10, 17];
            Run(left);

            baseModel.Meshes.Add(right);
            baseModel.Meshes.Add(left);
            baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
            baseModel.MeshTrees.Add(new() { OffsetX = -88, OffsetZ = z, Flags = _read });
            z += zdiff;
        }

        {
            // TR3 London
            var right = skin2.Meshes[20].Clone();
            var left = skin2.Meshes[17].Clone();

            vs = [0, 1, 2, 3, 4, 7, 8, 9, 15, 16];
            Run(right);
            vs = [0, 1, 2, 3, 4, 7, 8, 9, 10, 16];
            Run(left);

            baseModel.Meshes.Add(right);
            baseModel.Meshes.Add(left);
            baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
            baseModel.MeshTrees.Add(new() { OffsetX = -88, OffsetZ = z, Flags = _read });
            z += zdiff;
        }

        {
            // TR3 Nevada
            var right = skin2.Meshes[35].Clone();
            var left = skin2.Meshes[32].Clone();

            vs = [0, 1, 2, 3, 4, 5, 6, 7, 10, 11];
            Run(right);
            vs = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
            Run(left);

            baseModel.Meshes.Add(right);
            baseModel.Meshes.Add(left);
            baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
            baseModel.MeshTrees.Add(new() { OffsetX = -88, OffsetZ = z, Flags = _read });
            z += zdiff;
        }

        {
            // TR3 Antarc
            var right = skin2.Meshes[50].Clone();
            var left = skin2.Meshes[47].Clone();

            vs = [0, 1, 2, 3, 6, 13, 14, 15, 17, 18];
            Run(right);
            vs = [0, 1, 2, 3, 4, 7, 5, 6, 15, 17];
            Run(left);

            baseModel.Meshes.Add(right);
            baseModel.Meshes.Add(left);
            baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
            baseModel.MeshTrees.Add(new() { OffsetX = -88, OffsetZ = z, Flags = _read });
            z += zdiff;
        }

        {
            // TR1 N-Gage
            var right = skin2.Meshes[110].Clone();
            var left = skin2.Meshes[107].Clone();

            vs = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
            Run(right);
            Run(left);

            baseModel.Meshes.Add(right);
            baseModel.Meshes.Add(left);
            baseModel.MeshTrees.Add(new() { OffsetZ = z, Flags = _read });
            baseModel.MeshTrees.Add(new() { OffsetX = -88, OffsetZ = z, Flags = _read });
            z += zdiff;
        }

        baseModel.MeshTrees[^1].Flags = _pop;

        var frame = baseModel.Animations[0].Frames[0];
        for (int i = 1; i < baseModel.Meshes.Count; i++)
            frame.Rotations.Add(new());
    }

    static void DoTR1RegLeg(TRImage img)
    {
        var tone = Color.FromArgb(216, 160, 96);
        img.Write((c, x, y) => y >= 20 ? tone : c);
    }

    static ushort DoLegFace(ushort tex, TR2Level level)
    {
        var info = level.ObjectTextures[tex];
        if (info.Size.Width == 8 && info.Size.Height == 8) return ushort.MaxValue;

        var tile = new TRImage(level.Images16[info.Atlas].Pixels);
        var img = tile.Export(info.Bounds);
        var id = img.GenerateID();
        
        var inf = $"legs/fixed/{id}.png";
        if (!File.Exists(inf))
        {
            img.Save($"legs/{id}.png");
            return ushort.MaxValue;
        }

        img = new(inf);

        var newTile = new TRImage();
        newTile.Import(img, new(0, 0));
        var newInfo = info.Clone();
        newInfo.Position = new(0, 0);
        newInfo.Atlas = (ushort)level.Images16.Count;
        level.ObjectTextures.Add(newInfo);

        level.Images16.Add(new() { Pixels = newTile.ToRGB555() });
        level.Images8.Add(new() { Pixels = newTile.ToRGB(level.Palette) });

        return (ushort)(level.ObjectTextures.Count - 1);
    }

    static void SplitModels(TR2Level level)
    {
        var models = new TRDictionary<TR2Type, TRModel>();
        var type = 302;

        var walkAnim = _reader2.Read(@"F:\tomp\all levels\tr2\wall.tr2").Models[TR2Type.Lara].Animations[1];
        walkAnim.Commands.Clear();
        walkAnim.Changes.Clear();
        walkAnim.NextAnimation = 0;
        walkAnim.StateID = 0;
        walkAnim.FrameRate = 1;
        walkAnim.FrameEnd = 1;
        walkAnim.Speed = new();
        walkAnim.Frames = [walkAnim.Frames[0]];

        foreach (var oldType in new[] { _laraSkin1, _laraSkin2 })
        {
            int max = oldType == _laraSkin1 ? 16 : 8;
            var bigModel = level.Models[oldType];
            for (int i = 0; i < max; i++)
            {   
                var off = i * 15 + 1;
                var newModel = MakeBaseModel();
                newModel.Meshes.Clear();
                newModel.Meshes.AddRange(bigModel.Meshes.GetRange(off, 15));
                newModel.MeshTrees.AddRange(bigModel.MeshTrees.GetRange(off, 14));
                models[(TR2Type)type++] = newModel;
                newModel.Animations[0] = walkAnim.Clone();
            }
        }

        type = 334;
        foreach (var oldType in new[] { _laraSkinExtra, _laraSkinGuns1, _laraSkinGuns2, _laraSkinGuns3, _laraSkinLegs })
            models[(TR2Type)type++] = level.Models[oldType];

        level.Models = models;
    }

    static void Main(string[] args)
    {
        //{
        //    var caves = _reader1.Read("level1.phd");
        //    var ngage = _reader2.Read("ngage.tr2");
        //    ConvertFlatFaces(caves, [caves.Models[TR1Type.Lara]]);

        //    {
        //        var packer = new TR1TexturePacker(caves);
        //        var segs = packer.GetMeshRegions(caves.Models[TR1Type.Lara].Meshes).Values.SelectMany(v => v);
        //        Directory.CreateDirectory("ngage");
        //        Directory.CreateDirectory("ngage/caves");
        //        int i = 0;
        //        foreach (var r in segs)
        //        {
        //            r.Image.Save("ngage/caves/" + i + ".png");
        //            i++;
        //        }
        //    }
        //    {
        //        var packer = new TR2TexturePacker(ngage);
        //        var segs = packer.GetMeshRegions(ngage.Models[TR2Type.LaraSnowmobAnim_H].Meshes).Values.SelectMany(v => v);
        //        Directory.CreateDirectory("ngage/ng");
        //        int i = 0;
        //        foreach (var r in segs)
        //        {
        //            r.Image.Save("ngage/ng/" + i + ".png");
        //            i++;
        //        }
        //    }

        //    return;
        //}

        if (false)
        {
            //GunExtras.MakeTR1Guns();
            //GunExtras.MakeTR1GymGuns();
            //GunExtras.MakeTR2Guns();
            //GunExtras.MakeTR2GymGuns();
            //GunExtras.MakeTR2HSHGuns();
            //GunExtras.MakeTR2VegasGuns();
            //GunExtras.MakeTR3Guns();
            //GunExtras.MakeTR3GymGuns();
            var lvl = _reader2.Read("titanic1.phd");

            foreach (var image in lvl.Images16)
            {
                var pix = new uint[image.Pixels.Length];
                for (int i = 0; i < image.Pixels.Length; i++)
                {
                    ushort t = image.Pixels[i];
                    var r = (t & 0x7c00) >> 10;
                    var g = (t & 0x03e0) >> 5;
                    var b = t & 0x001f;

                    r = (int)((r / 31.0f) * 255.0f);
                    g = (int)((g / 31.0f) * 255.0f);
                    b = (int)((b / 31.0f) * 255.0f);

                    if ((r == 255 && b == 255 && g == 0) ||
                    (r == 0 && b == 0 && g == 0))
                    {
                        pix[i] = 0;
                    }
                    else
                    {
                        int a = 0xFF;
                        pix[i] = (uint)b;
                        pix[i] |= (uint)(g << 8);
                        pix[i] |= (uint)(r << 16);
                        pix[i] |= (uint)(a << 24);
                    }
                }
                var img = new TRImage(pix);
                image.Pixels = img.ToRGB555();
            }

            var wall = _reader2.Read("wall.tr2");
            wall.ObjectTextures = lvl.ObjectTextures;
            wall.Entities = lvl.Entities;
            wall.Images16 = lvl.Images16;
            wall.Images8 = lvl.Images8;
            wall.Rooms = lvl.Rooms;
            wall.FloorData = lvl.FloorData;
            wall.Boxes = lvl.Boxes;
            wall.Models = lvl.Models;
            wall.SoundSources = lvl.SoundSources;
            wall.Cameras = lvl.Cameras;
            wall.AnimatedTextures = lvl.AnimatedTextures;
            wall.CinematicFrames = lvl.CinematicFrames;
            wall.DemoData = lvl.DemoData;
            wall.LightMap = lvl.LightMap;
            wall.Palette = lvl.Palette;
            wall.Palette16 = lvl.Palette16;
            wall.Sprites = lvl.Sprites;
            wall.StaticMeshes = lvl.StaticMeshes;
            _reader2.Write(wall, "titanic.tr2");
            return;
        }

        if (true)
        {
            var baseLevel = MakeBaseLevel();
            var baseModel1 = MakeBaseModel();
            baseLevel.Models[TR2Type.Lara] = baseModel1;
            var baseModel2 = MakeBaseModel();
            baseLevel.Models[TR2Type.LaraPistolAnim_H] = baseModel2;
            var map = new List<List<TRMesh>>();
            //var baseModel3 = MakeBaseModel();
            //baseLevel.Models[TR2Type.LaraShotgunAnim_H] = baseModel3;

            var lvl1 = _reader1.Read(@"F:\tomp\all levels\tr1\level1.phd");
            var tr1Lara = lvl1.Models[TR1Type.Lara];
            var tr1Head = tr1Lara.Meshes[14];
            ConvertFlatFaces(lvl1, [tr1Lara]);            

            var lvl2 = _reader2.Read(@"F:\tomp\all levels\tr2\wall.tr2");
            var tr2Lara = lvl2.Models[TR2Type.Lara];
            var ogLara = tr2Lara.Clone(); // Cache for nodes later
            var tr2Head = tr2Lara.Meshes[14];
            ConvertFlatFaces(lvl2, [tr2Lara]);

            var lvl3 = _reader3.Read(@"F:\tomp\all levels\tr3\jungle.tr2");
            var tr3Lara = lvl3.Models[TR3Type.Lara];
            var tr3Head = tr3Lara.Meshes[14];
            ConvertFlatFaces(lvl3, [tr3Lara]);

            {
                // 0. TR1 gym
                var gym = _reader1.Read(@"F:\tomp\all levels\tr1\gymoutfit.phd");
                gym.Models[TR1Type.Lara].Meshes[10] = gym.Models[TR1Type.LaraMiscAnim_H].Meshes[10];
                gym.Models[TR1Type.Lara].Meshes[13] = gym.Models[TR1Type.LaraMiscAnim_H].Meshes[13];
                Import(baseLevel, baseModel1, gym, gym.Models[TR1Type.Lara], tr1Head);
                map.Add([.. baseModel1.Meshes.GetRange(baseModel1.Meshes.Count - 15, 15)]);
            }

            {
                // 1. TR1 classic
                var caves = _reader1.Read(@"F:\tomp\all levels\tr1\level1.phd");
                Import(baseLevel, baseModel1, caves, caves.Models[TR1Type.Lara], null);
                tr1Head = baseModel1.Meshes[^1];
                baseModel1.Meshes[^16] = tr1Head.Clone();
                map.Add([.. baseModel1.Meshes.GetRange(baseModel1.Meshes.Count - 15, 15)]);
            }

            {
                // 2. TR1 mauled
                var valley = _reader1.Read(@"F:\tomp\all levels\tr1\level3a.phd");
                var mauled = valley.Models[TR1Type.LaraMiscAnim_H];
                for (int i = 0; i < mauled.Meshes.Count; i++)
                {
                    mauled.Meshes[i].Centre = valley.Models[TR1Type.Lara].Meshes[i].Centre;
                    mauled.Meshes[i].CollRadius = valley.Models[TR1Type.Lara].Meshes[i].CollRadius;
                }
                Import(baseLevel, baseModel1, valley, mauled, tr1Head);
                map.Add([.. baseModel1.Meshes.GetRange(baseModel1.Meshes.Count - 15, 15)]);
            }

            {
                // 3. TR1 combo
                for (int i = 16; i < 23; i++)
                {
                    baseModel1.Meshes.Add(baseModel1.Meshes[i].Clone());
                }
                for (int i = 8; i < 16; i++)
                {
                    baseModel1.Meshes.Add(baseModel1.Meshes[i].Clone());
                }
                map.Add([.. baseModel1.Meshes.GetRange(baseModel1.Meshes.Count - 15, 15)]);
            }

            {
                // 4. TR1 gold
                var midas = _reader1.Read(@"F:\tomp\all levels\tr1\level6.phd");
                var gold = midas.Models[TR1Type.LaraMiscAnim_H];
                for (int i = 0; i < gold.Meshes.Count; i++)
                {
                    gold.Meshes[i].Centre = midas.Models[TR1Type.Lara].Meshes[i].Centre;
                    gold.Meshes[i].CollRadius = midas.Models[TR1Type.Lara].Meshes[i].CollRadius;
                }
                Import(baseLevel, baseModel1, midas, gold, null);
                map.Add([.. baseModel1.Meshes.GetRange(baseModel1.Meshes.Count - 15, 15)]);
            }

            {
                // 5. TR1 bacon
                var atlantis = _reader1.Read(@"F:\tomp\all levels\tr1\level10b.phd");
                Import(baseLevel, baseModel1, atlantis, atlantis.Models[TR1Type.Doppelganger], null);
                map.Add([.. baseModel1.Meshes.GetRange(baseModel1.Meshes.Count - 15, 15)]);

                // 6. Golden bacon
                for (int i = 76; i < 91; i++)
                {
                    baseModel1.Meshes.Add(baseModel1.Meshes[i].Clone());
                }
                map.Add([.. baseModel1.Meshes.GetRange(baseModel1.Meshes.Count - 15, 15)]);
            }

            {
                // 7. TR2 gym
                var gym = _reader2.Read(@"F:\tomp\all levels\tr2\gymoutfit.tr2");
                Import(baseLevel, baseModel1, gym, gym.Models[TR2Type.Lara], tr2Head);
                map.Add([.. baseModel1.Meshes.GetRange(baseModel1.Meshes.Count - 15, 15)]);
            }

            {
                // 8. TR2 classic
                var wall = _reader2.Read(@"F:\tomp\all levels\tr2\wall.tr2");
                Import(baseLevel, baseModel1, wall, wall.Models[TR2Type.Lara], null);
                tr2Head = baseModel1.Meshes[^1];
                baseModel1.Meshes[^16] = tr2Head.Clone();
                map.Add([.. baseModel1.Meshes.GetRange(baseModel1.Meshes.Count - 15, 15)]);
            }

            {
                // 9. TR2 underwater
                var fathoms = _reader2.Read(@"F:\tomp\all levels\tr2\unwater.tr2");
                Import(baseLevel, baseModel1, fathoms, fathoms.Models[TR2Type.Lara], tr2Head);
                map.Add([.. baseModel1.Meshes.GetRange(baseModel1.Meshes.Count - 15, 15)]);
            }

            {
                // 10. TR2 Tibet
                var skidoo = _reader2.Read(@"F:\tomp\all levels\tr2\skidoo.tr2");
                Import(baseLevel, baseModel1, skidoo, skidoo.Models[TR2Type.Lara], tr2Head);
                map.Add([.. baseModel1.Meshes.GetRange(baseModel1.Meshes.Count - 15, 15)]);
            }

            {
                // 11. TR2 HSH
                var hsh = _reader2.Read(@"F:\tomp\all levels\tr2\house.tr2");
                Import(baseLevel, baseModel1, hsh, hsh.Models[TR2Type.Lara], tr2Head);
                // Following has holster straps, easier this way
                hsh = _reader2.Read(@"F:\tomp\all levels\tr2\hshoutfit.tr2");
                var lastSet = baseModel1.Meshes.GetRange(baseModel1.Meshes.Count - 15, 15);
                var hands = new[] { lastSet[10], lastSet[13] };
                baseModel1.Meshes.RemoveAll(lastSet.Contains);
                Import(baseLevel, baseModel1, hsh, hsh.Models[TR2Type.Lara], tr2Head);
                baseModel1.Meshes[^2] = hands[1];
                baseModel1.Meshes[^5] = hands[0];
                map.Add([.. baseModel1.Meshes.GetRange(baseModel1.Meshes.Count - 15, 15)]);
            }

            {
                // 12. TR2 Vegas
                var vegas = _reader2.Read(@"F:\tomp\all levels\tr2\vegasoutfit.tr2");
                Import(baseLevel, baseModel1, vegas, vegas.Models[TR2Type.Lara], tr2Head);
                map.Add([.. baseModel1.Meshes.GetRange(baseModel1.Meshes.Count - 15, 15)]);
            }

            {
                // 13. TR2/3 gold
                for (int i = 121; i < 136; i++)
                {
                    baseModel1.Meshes.Add(baseModel1.Meshes[i].Clone());
                }
                map.Add([.. baseModel1.Meshes.GetRange(baseModel1.Meshes.Count - 15, 15)]);
            }

            {
                // 14. TR3 gym
                var gym = _reader3.Read(@"F:\tomp\all levels\tr3\house.tr2");
                Import(baseLevel, baseModel1, gym, gym.Models[TR3Type.LaraSkin_H], tr3Head);
                map.Add([.. baseModel1.Meshes.GetRange(baseModel1.Meshes.Count - 15, 15)]);
            }

            {
                // 15. TR3 classic
                var jungle = _reader3.Read(@"F:\tomp\all levels\tr3\temple.tr2");
                Import(baseLevel, baseModel1, jungle, jungle.Models[TR3Type.LaraSkin_H], null);
                tr3Head = baseModel1.Meshes[^1];
                baseModel1.Meshes[^16] = tr3Head.Clone();
                map.Add([.. baseModel1.Meshes.GetRange(baseModel1.Meshes.Count - 15, 15)]);
            }

            // Split
            {
                // 16. TR3 coastal
                var shore = _reader3.Read(@"F:\tomp\all levels\tr3\shore.tr2");
                Import(baseLevel, baseModel2, shore, shore.Models[TR3Type.LaraSkin_H], tr3Head);
                map.Add([.. baseModel2.Meshes.GetRange(baseModel2.Meshes.Count - 15, 15)]);
            }

            {
                // 17. TR3 London
                var roofs = _reader3.Read(@"F:\tomp\all levels\tr3\roofs.tr2");
                Import(baseLevel, baseModel2, roofs, roofs.Models[TR3Type.LaraSkin_H], tr3Head);
                map.Add([.. baseModel2.Meshes.GetRange(baseModel2.Meshes.Count - 15, 15)]);
            }

            {
                // 18. TR3 Nevada
                var nevada = _reader3.Read(@"F:\tomp\all levels\tr3\nevada.tr2");
                Import(baseLevel, baseModel2, nevada, nevada.Models[TR3Type.LaraSkin_H], tr3Head);
                map.Add([.. baseModel2.Meshes.GetRange(baseModel2.Meshes.Count - 15, 15)]);
            }

            {
                // 19. TR3 Antarctica
                var antarc = _reader3.Read(@"F:\tomp\all levels\tr3\antarc.tr2");
                Import(baseLevel, baseModel2, antarc, antarc.Models[TR3Type.LaraSkin_H], tr3Head);
                map.Add([.. baseModel2.Meshes.GetRange(baseModel2.Meshes.Count - 15, 15)]);
            }

            {
                // 20. Sophia Leigh
                var antarc = _reader3.Read(@"F:\tomp\all levels\tr3\office.tr2");
                Import(baseLevel, baseModel2, antarc, antarc.Models[TR3Type.SophiaLee], null);
                map.Add([.. baseModel2.Meshes.GetRange(baseModel2.Meshes.Count - 15, 15)]);

                // 21. Golden Sophia
                for (int i = 61; i < 76; i++)
                {
                    if (i - 61 == 7)
                    {
                        var mesh = baseModel2.Meshes[i];
                        mesh.Vertices[18].Y -= 26;
                        mesh.Vertices[19].Y -= 26;
                        foreach (var j in new[] { 40, 41 })
                            mesh.Vertices[j].Y = mesh.Vertices[19].Y;
                    }
                    baseModel2.Meshes.Add(baseModel2.Meshes[i].Clone());
                }
                map.Add([.. baseModel2.Meshes.GetRange(baseModel2.Meshes.Count - 15, 15)]);
            }

            {
                // 22. TR2 underwater alpha
                var fathoms = _reader2.Read("titanic.tr2");
                Import(baseLevel, baseModel1, fathoms, fathoms.Models[TR2Type.Lara], tr2Head);
                map.Add([.. baseModel1.Meshes.GetRange(baseModel1.Meshes.Count - 15, 15)]);
            }

            {
                // 23. N-Gage
                var ngage = _reader2.Read("ngage.tr2");
                Import(baseLevel, baseModel1, ngage, ngage.Models[TR2Type.LaraSnowmobAnim_H], tr1Head);
                map.Add([.. baseModel1.Meshes.GetRange(baseModel1.Meshes.Count - 15, 15)]);
            }

            if (true)
            {
                // Mesh cleanup

                // Standardize spheres
                foreach (var lara in new[] { _laraGym1, _laraCombo1, _laraMauled, _laraGold1, _laraNGage })
                {
                    for (int i = 0; i < 15; i++)
                    {
                        map[lara][i].CollRadius = map[_laraClassic1][i].CollRadius;
                        map[lara][i].Centre = map[_laraClassic1][i].Centre;
                    }
                }
                foreach (var lara in new[] { _laraGym2, _laraDiving, _laraBomber, _laraRobe, _laraVegas, _laraGold3, _laraDivingAlpha })
                {
                    for (int i = 0; i < 15; i++)
                    {
                        map[lara][i].CollRadius = map[_laraClassic2][i].CollRadius;
                        map[lara][i].Centre = map[_laraClassic2][i].Centre;
                    }
                }
                foreach (var lara in new[] { _laraGym3, _laraCoastal, _laraNevada, _laraLondon, _laraAntarc, _laraLeigh, _laraLeighGold })
                {
                    for (int i = 0; i < 15; i++)
                    {
                        map[lara][i].CollRadius = map[_laraClassic3][i].CollRadius;
                        map[lara][i].Centre = map[_laraClassic3][i].Centre;
                    }
                }

                {
                    // TR1 hips - mauled version is wrong
                    map[_laraMauled][0] = map[_laraClassic1][0].Clone();
                }

                {
                    // Fix TR1 mauled torso differences
                    var mauled = map[_laraMauled][7];
                    var regular = map[_laraClassic1][7];

                    var vs = new List<List<ushort>>
                    {
                        new(){ 9,8,12,13 },
                        new(){ 3,2,0,1 },
                        new(){ 0,1,22,23 },
                        new(){ 6,7,5,4 },
                        new(){ 5,4,25,24 },
                    };
                    var all = vs.SelectMany(v => v).Distinct().ToList();
                    mauled.TexturedRectangles.RemoveAll(f => !f.Vertices.All(all.Contains));
                    mauled.TexturedTriangles.RemoveAll(f => !f.Vertices.All(all.Contains));

                    foreach (var face in regular.TexturedFaces)
                    {
                        if (vs.Any(verts => face.Vertices.All(verts.Contains)))
                            continue;
                        if (face.Type == TRFaceType.Rectangle)
                            mauled.TexturedRectangles.Add(face.Clone());
                        else
                            mauled.TexturedTriangles.Add(face.Clone());
                    }

                    for (int i = 22; i < 26; i++)
                        mauled.Vertices[i].Y = regular.Vertices[i].Y;
                }

                {
                    // Fix TR1 combo hips blue bits
                    var gymHips = map[_laraGym1][0];
                    var comboHips = map[_laraCombo1][0];
                    comboHips.TexturedRectangles[7].Texture = gymHips.TexturedRectangles[11].Texture;

                    var tile = new TRImage();
                    var y = 0;
                    foreach (var i in new[] { 5, 6 })
                    {
                        var face = comboHips.TexturedRectangles[i];
                        var tex = baseLevel.ObjectTextures[face.Texture].Clone();
                        var img = new TRImage(baseLevel.Images16[tex.Atlas].Pixels).Export(tex.Bounds);
                        img.Write((c, x, y) => y < 2 ? Color.FromArgb(216, 160, 96) : c);
                        tex.Position = new(0, y);
                        tex.Atlas = (ushort)baseLevel.Images16.Count;
                        tile.Import(img, tex.Position);
                        y += img.Height;
                        face.Texture = (ushort)baseLevel.ObjectTextures.Count;
                        baseLevel.ObjectTextures.Add(tex);
                    }

                    baseLevel.Images16.Add(new() { Pixels = tile.ToRGB555() });
                    baseLevel.Images8.Add(new() { Pixels = new byte[256*256] });
                }

                if (false)
                {
                    // Give bacon lara holsters
                    foreach (var mesh in new[] { 1,4 })
                    {
                        var cmesh = map[_laraClassic1][mesh];
                        var bmesh = map[_laraBacon][mesh];
                        bmesh.Vertices.AddRange(cmesh.Vertices.GetRange(13, 8).Select(v => v.Clone()));
                        bmesh.Normals.AddRange(cmesh.Normals.GetRange(13, 8).Select(v => v.Clone()));
                        
                        for (int i = 15; i < bmesh.Vertices.Count; i++)
                            bmesh.Vertices[i].Y += 13;

                        var faces = cmesh.TexturedFaces.Where(f => f.Vertices.All(v => v >= 13))
                            .Select(f => f.Clone()).ToList();
                        foreach (var face in faces)
                        {
                            face.Texture = bmesh.TexturedRectangles[2].Texture;
                            for (int i = 0; i < face.Vertices.Count; i++)
                                face.Vertices[i]+=2;
                            if (face.Type == TRFaceType.Rectangle)
                                bmesh.TexturedRectangles.Add(face);
                            else
                                bmesh.TexturedTriangles.Add(face);
                        }

                        if (mesh == 1)
                        {
                            bmesh.TexturedRectangles[10].Rotate(1);
                        }
                        else
                        {
                            for (int i = 10; i >= 5; i--)
                                bmesh.TexturedRectangles[i].Rotate(1);
                        }

                        // Replicate into golden bacon
                        var gmesh = map[_laraGold2][mesh];
                        gmesh.Vertices.AddRange(bmesh.Vertices.GetRange(15, 8).Select(v => v.Clone()));
                        gmesh.Normals.AddRange(bmesh.Normals.GetRange(15, 8).Select(v => v.Clone()));
                        gmesh.TexturedRectangles.AddRange(bmesh.TexturedRectangles.GetRange(5, 6).Select(f => f.Clone()));
                    }
                }

                {
                    // Gold bacon, TR2/3 gold, gold Sophia
                    var goldTex = map[_laraGold1][0].TexturedRectangles[0].Texture;
                    foreach (var mesh in map[_laraGold2])
                    {
                        mesh.TexturedFaces.ToList().ForEach(f => f.Texture = goldTex);
                    }
                    foreach (var mesh in map[_laraGold3])
                    {
                        mesh.TexturedFaces.ToList().ForEach(f => f.Texture = goldTex);
                    }
                    foreach (var mesh in map[_laraLeighGold])
                    {
                        mesh.TexturedFaces.ToList().ForEach(f => f.Texture = goldTex);
                    }
                }

                {
                    // Tibet legs
                    foreach (var idx in new[] { 0, 1, 2, 4, 5 })
                    {
                        map[_laraBomber][idx] = map[_laraClassic2][idx].Clone();
                    }
                }

                {
                    // Fix TR2 backpack transparency
                    var tex = map[_laraClassic2][7].TexturedRectangles[18].Texture;
                    map[_laraDiving][7].TexturedRectangles[18].Texture = tex;
                    map[_laraBomber][7].TexturedRectangles[25].Texture = tex;
                }

                {
                    // Fix TR2 gym holsters
                    var face = map[_laraGym2][4].TexturedTriangles[35];
                    face.Vertices = [24, 21, 26];
                    face = map[_laraGym2][4].TexturedTriangles[30];
                    face.Vertices = [26, 27, 24];
                }

                {
                    // Fix TR2 diving suit colours
                    void FixImgs(ushort[] texIds, Func<Color, int, int, Color> callback)
                    {
                        foreach (var id in texIds)
                        {
                            var tex = baseLevel.ObjectTextures[id];
                            var img = new TRImage(baseLevel.Images16[tex.Atlas].Pixels);
                            img.Write(tex.Bounds, (c, x, y) => callback(c, x - tex.Bounds.X, y - tex.Bounds.Y));
                            baseLevel.Images16[tex.Atlas].Pixels = img.ToRGB555();
                        }
                    }

                    var mesh = map[_laraDiving][7];
                    FixImgs([mesh.TexturedRectangles[1].Texture, mesh.TexturedTriangles[12].Texture],
                        (c, x, y) => c == Color.FromArgb(8, 16, 16) ? c : Color.FromArgb(224, 160, 96));

                    FixImgs([mesh.TexturedRectangles[12].Texture, mesh.TexturedRectangles[14].Texture],
                        (c, x, y) => c.A == 0 ? Color.FromArgb(216, 224, 224) : c);

                    mesh = map[_laraDiving][8];
                    FixImgs([mesh.TexturedRectangles[0].Texture, mesh.TexturedRectangles[4].Texture],
                        (c, x, y) => y >= 14 ? Color.FromArgb(224, 160, 96) : c);
                }

                {
                    // Fix TR1 gloves
                    foreach (var lara in new[] { _laraClassic1, _laraMauled })
                    {
                        var left = map[lara][13];
                        var vs = new ushort[] { 7, 3, 4, 0 };
                        var bit = left.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains));
                        bit.Rotate(2);
                    }
                }

                {
                    // Fix TR2/3 gloves
                    foreach (var lara in new[] { _laraGym2, _laraClassic2, _laraDiving, _laraBomber, _laraGym3, 
                        _laraClassic3, _laraCoastal, _laraLondon, _laraNevada, _laraAntarc, /*_laraDivingAlpha*/ })
                    {
                        var rght = map[lara][10];
                        var left = map[lara][13];
                        var vs = new ushort[] { 7,3,4,0 };
                        var bitR = rght.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains));
                        var bitL = left.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains));
                        Debug.Assert(bitR != null && bitL != null);
                        bitR.Rotate(2);

                        var baseInfo = baseLevel.ObjectTextures[bitL.Texture];
                        var texInfo = baseInfo.Clone();
                        Debug.Assert(texInfo.UVMode == TRUVMode.NE_AntiClockwise);
                        texInfo.UVMode = TRUVMode.NW_Clockwise;
                        baseLevel.ObjectTextures.Add(texInfo); // Deduping will tidy up later
                        bitR.Texture = (ushort)(baseLevel.ObjectTextures.Count - 1);

                        vs = [2,3,6,7];
                        bitR = rght.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains));
                        bitL = left.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains));
                        Debug.Assert(bitR != null && bitL != null);

                        var img = new TRImage(baseLevel.Images16[baseInfo.Atlas].Pixels).Export(baseInfo.Bounds);
                        var blue = img.GetPixel(0, 0);
                        img = new TRImage(8, 8);
                        img.Fill(blue);

                        var tex = new TRObjectTexture(8, 8, 0, 0)
                        {
                            Atlas = (ushort)baseLevel.Images16.Count
                        };
                        var tile = new TRImage();
                        tile.Import(img, tex.Position);
                        bitL.Texture = (ushort)baseLevel.ObjectTextures.Count;
                        bitR.Texture = (ushort)baseLevel.ObjectTextures.Count;
                        baseLevel.ObjectTextures.Add(tex);
                        baseLevel.Images16.Add(new() { Pixels = tile.ToRGB555() });
                        baseLevel.Images8.Add(new() { Pixels = tile.ToRGB(baseLevel.Palette) });
                    }

                    {
                        foreach (var m in new[] { 10,13 })
                        {
                            var vegas = map[_laraVegas][m];
                            var robe = map[_laraRobe][m];
                            var vs = new ushort[] { 2,3,6,7 };
                            var a = vegas.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains));
                            var b = robe.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains));
                            Debug.Assert(a != null && b != null);
                            a.Texture = b.Texture;
                        }
                    }
                }

                {
                    // Give Sophia better hands
                    foreach (var m in new[] { 10, 13 })
                    {
                        map[_laraLeigh][m].TexturedRectangles.Clear();
                        map[_laraLeigh][m].TexturedTriangles.Clear();
                        map[_laraLeigh][m].TexturedRectangles.AddRange(map[_laraRobe][m].TexturedRectangles.Select(f => f.Clone()));
                        map[_laraLeigh][m].TexturedTriangles.AddRange(map[_laraRobe][m].TexturedTriangles.Select(f => f.Clone()));
                        CleanupVertices(map[_laraLeigh][m]);
                    }

                    map[_laraLeighGold][10].TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v >= 8));
                    map[_laraLeighGold][10].TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v >= 8));
                    CleanupVertices(map[_laraLeighGold][10]);
                }

                {
                    // Fix Sophia's hair
                    var mesh = map[_laraLeigh][14];
                    var vs = new ushort[] { 49,48,27 };
                    var tex = mesh.TexturedTriangles.Find(f => f.Vertices.All(vs.Contains)).Texture;
                    var verts = new List<List<ushort>>
                    {
                        new() {2,23,48},
                        new() {49,48,23},
                        new() {50,49,23},
                        new() {23,25,50},
                        new() {51,50,25},
                        new() {46,51,25},
                        new() {25,1,46},
                        new() {47,46,1},
                    };
                    foreach (var vts in verts)
                    {
                        mesh.TexturedTriangles.Add(new()
                        {
                            Texture = tex,
                            Vertices = vts,
                            Type = TRFaceType.Triangle,
                        });
                    }

                    vs = [26,29,46,47];
                    foreach (var f in mesh.TexturedTriangles.Where(g => g.Vertices.All(vs.Contains)))
                        f.Texture = tex;

                    var goldTex = map[_laraGold1][0].TexturedRectangles[0].Texture;
                    map[_laraLeighGold][14] = mesh.Clone();
                    map[_laraLeighGold][14].TexturedFaces.ToList().ForEach(f => f.Texture = goldTex);
                }

                {
                    // Clone some diving suit stuff
                    foreach (var m in new[] { 2, 3, 5, 6, 9, 10, 12, 13 })
                    {
                        map[_laraDivingAlpha][m] = map[_laraDiving][m].Clone();
                    }

                    void FixImgs(ushort[] texIds, Func<Color, int, int, Color> callback)
                    {
                        foreach (var id in texIds)
                        {
                            var tex = baseLevel.ObjectTextures[id];
                            var img = new TRImage(baseLevel.Images16[tex.Atlas].Pixels);
                            img.Write(tex.Bounds, (c, x, y) => callback(c, x - tex.Bounds.X, y - tex.Bounds.Y));
                            baseLevel.Images16[tex.Atlas].Pixels = img.ToRGB555();
                        }
                    }

                    var mesh = map[_laraDivingAlpha][8];
                    FixImgs([mesh.TexturedRectangles[0].Texture, mesh.TexturedRectangles[4].Texture],
                        (c, x, y) => y >= 14 ? Color.FromArgb(224, 160, 96) : c);

                    var vs = new ushort[] { 12, 13, 16, 17 };
                    var f = map[_laraDiving][7].TexturedRectangles.Find(g => g.Vertices.All(vs.Contains));
                    map[_laraDivingAlpha][7].TexturedRectangles.Find(g => g.Vertices.All(vs.Contains)).Texture = f.Texture;
                    vs = [0, 1, 55, 56];
                    f = map[_laraDivingAlpha][7].TexturedRectangles.Find(g => g.Vertices.All(vs.Contains));
                    vs = [18,19,20,21];
                    map[_laraDivingAlpha][7].TexturedRectangles.Find(g => g.Vertices.All(vs.Contains)).Texture = f.Texture;

                    vs = [10,11,12,13];
                    f = map[_laraDivingAlpha][7].TexturedRectangles.Find(g => g.Vertices.All(vs.Contains));
                    FixImgs([f.Texture], (c, x, y) => c == Color.FromArgb(222,172,98) ? Color.FromArgb(224,160,96) : c);
                    vs = [14,15,16,17];
                    f = map[_laraDivingAlpha][7].TexturedRectangles.Find(g => g.Vertices.All(vs.Contains));
                    FixImgs([f.Texture], (c, x, y) => y < 9 ? Color.FromArgb(224, 160, 96) : c);

                    var ds = map[_laraDivingAlpha][1].TexturedFaces.Select(g => g.Texture).Distinct();
                    var cs = new List<Color>();
                    FixImgs([.. ds], (c, x, y) => c == Color.FromArgb(213, 164, 82) ? Color.FromArgb(224, 160, 96) : c);

                    void DumpImg(string s, int fid, ushort id)
                    {
                        var tex = baseLevel.ObjectTextures[id];
                        var img = new TRImage(baseLevel.Images16[tex.Atlas].Pixels);
                        var file = $"New Folder/Fixed/suit_{s}{fid}.png";
                        if (File.Exists(file))
                        {
                            var seg = new TRImage(file);
                            img.Import(seg, tex.Position);
                            baseLevel.Images16[tex.Atlas].Pixels = img.ToRGB555();
                        }
                        else
                        {
                            var seg = img.Export(tex.Bounds);
                            seg.Save($"New Folder/suit_{s}{fid}.png");
                        }
                    }
                    for (int i = 0; i < map[_laraDivingAlpha][7].TexturedRectangles.Count; i++)
                    {
                        DumpImg(string.Empty, i, map[_laraDivingAlpha][7].TexturedRectangles[i].Texture);
                    }
                    for (int i = 0; i < map[_laraDivingAlpha][7].TexturedTriangles.Count; i++)
                    {
                        DumpImg("t", i, map[_laraDivingAlpha][7].TexturedTriangles[i].Texture);
                    }

                    for (int i = 0; i < map[_laraDivingAlpha][0].TexturedRectangles.Count; i++)
                    {
                        DumpImg("ass", i, map[_laraDivingAlpha][0].TexturedRectangles[i].Texture);
                    }


                    vs = [0,3,7,2,6];
                    f = map[_laraDivingAlpha][8].TexturedTriangles.Find(g => g.Vertices.All(vs.Contains));
                    FixImgs([f.Texture], (c, x, y) => Color.FromArgb(224, 160, 96));
                }

                if (false)
                {
                    // Add backpack to alpha diving suit
                    var suitMesh = map[_laraDiving][7];
                    var alphaMesh = map[_laraDivingAlpha][7];
                    var vs = new ushort[] { 62,61,60,65,66,67,69,4,58,59,68,64,5,63 };
                    var vmap = new Dictionary<ushort, ushort>();
                    foreach (var vert in vs)
                    {
                        var vtx = suitMesh.Vertices[vert].Clone();
                        var nor = suitMesh.Normals[vert].Clone();
                        vmap[vert] = (ushort)alphaMesh.Vertices.Count;
                        alphaMesh.Vertices.Add(vtx);
                        alphaMesh.Normals.Add(nor);
                        if (vmap[vert] == 59 || vmap[vert] == 64)
                        {
                            alphaMesh.Vertices[^1].Z += 1;
                        }
                    }

                    foreach (var face in suitMesh.TexturedFaces.Where(f => f.Vertices.All(vs.Contains)))
                    {
                        var ff = face.Clone();
                        for (int i = 0; i < ff.Vertices.Count; i++)
                        {
                            ff.Vertices[i] = vmap[ff.Vertices[i]];
                        }
                        if (face.Type == TRFaceType.Rectangle)
                            alphaMesh.TexturedRectangles.Add(ff);
                        else
                            alphaMesh.TexturedTriangles.Add(ff);
                    }

                    vs = [67,72,66,71];
                    alphaMesh.TexturedRectangles.Add(new()
                    {
                        Type = TRFaceType.Rectangle,
                        Vertices = [64,59,4,5],
                        Texture = alphaMesh.TexturedRectangles.Find(f => f.Vertices.All(vs.Contains)).Texture,
                    });
                }

                {
                    // Bad N-Gage hands
                    foreach (var m in new[] { 10, 13 })
                    {
                        map[_laraNGage][m] = map[_laraClassic1][m].Clone();
                    }
                }
            }

            {
                // Final structure
                map[_laraGym1][14] = map[_laraClassic1][14].Clone();
                map[_laraGym2][14] = map[_laraClassic2][14].Clone();
                map[_laraGym3][14] = map[_laraClassic3][14].Clone();

                baseModel1.Meshes = [baseModel1.Meshes[0]];
                baseModel2.Meshes = [baseModel2.Meshes[0]];
                var model = baseModel1;
                for (int i = 0; i < map.Count; i++)
                {
                    var mod = i < 16 ? baseModel1 : baseModel2;
                    mod.Meshes.AddRange(map[i].Select(m => m.Clone()));
                }
            }

            var models = new[] { baseModel1, baseModel2 };
            var nodes = ogLara.MeshTrees.ToList();
            nodes.Insert(0, new());
            foreach (var model in models)
            {
                var outfitCount = (model.Meshes.Count - 1) / nodes.Count;
                var frame = model.Animations[0].Frames[0];
                for (int i = 0; i < outfitCount; i++)
                {
                    for (int j = 0; j < nodes.Count; j++)
                    {
                        var tree = nodes[j].Clone();
                        if (j == 0)
                        {
                            tree.OffsetX += i * -256;
                            tree.Flags = i > 0 ? _read : _push;
                        }
                        model.MeshTrees.Add(tree);
                        frame.Rotations.Add(new());
                        //if (j == 9 || j == 12)
                            //frame.Rotations[^1].X = 256;
                    }
                }

                frame.OffsetX = (short)((outfitCount / 2 * 256) - 128);
            }

            {
                // Bacon mesh trees
                var bacon = _reader1.Read(@"F:\tomp\all levels\tr1\level10b.phd").Models[TR1Type.Doppelganger];
                foreach (var start in new[] { 76, 91 })
                {
                    for (int i = 0; i < bacon.MeshTrees.Count; i++)
                    {
                        baseModel1.MeshTrees[start + i] = bacon.MeshTrees[i].Clone();
                    }
                }
            }

            baseLevel.Models = new()
            {
                [_laraSkin1] = baseModel1,
                [_laraSkin2] = baseModel2,
            };

            DoExtra(baseLevel);
            DoGuns(baseLevel);
            SortGuns(baseLevel);
            DoLegs(baseLevel);
            //DoHolsters(baseLevel);
            SplitModels(baseLevel);

            Repack(baseLevel);
            _reader2.Write(baseLevel, "outfits.tr2");
            return;
        }

        if (args.Length == 0 || args[0].Contains('?'))
        {
            Usage();
            return;
        }

        Mode mode = Mode.Png;
        if (args.Length > 1)
        {
            string arg = args[1].ToLower();
            if (arg == "html")
            {
                mode = Mode.Html;
            }
            else if (arg == "segments")
            {
                mode = Mode.Segments;
            }
            else if (arg == "faces")
            {
                mode = Mode.Faces;
                if (args.Length < 3)
                {
                    return;
                }
            }
            else if (arg == "boxes")
            {
                mode = Mode.Boxes;
                if (args.Length < 3)
                {
                    return;
                }
            }
            else if (arg == "depend")
            {
                mode = Mode.Dependencies;
            }
            else if (arg == "dds")
            {
                if (args.Length < 3)
                {
                    return;
                }
                mode = Mode.Dds;
            }
            else if (arg == "texinfo")
            {
                mode = Mode.TexInfo;
            }
        }

        string levelType = args[0].ToLower();

        if (mode == Mode.Dds)
        {
            if (Enum.TryParse(levelType.ToUpper(), out TRGameVersion version))
            {
                TRRExporter.Export(args[2], version);
            }
        }
        else if (mode == Mode.TexInfo)
        {
            if (Enum.TryParse(levelType.ToUpper(), out TRGameVersion version))
            {
                TRRExporter.GenerateCategories(version);
            }
        }
        else if (levelType.EndsWith(".phd"))
        {
            ExportAllTextures(args[0], _reader1.Read(args[0]), mode, args);
        }
        else if (levelType.EndsWith(".tr2"))
        {
            TRFileVersion version = DetectVersion(args[0]);
            if (version == TRFileVersion.TR2)
            {
                ExportAllTextures(args[0], _reader2.Read(args[0]), mode, args);
            }
            else if (version == TRFileVersion.TR3a || version == TRFileVersion.TR3b)
            {
                ExportAllTextures(args[0], _reader3.Read(args[0]), mode, args);
            }
        }
        else if (levelType == "tr1")
        {
            foreach (string lvl in TR1LevelNames.AsOrderedList)
            {
                if (File.Exists(lvl))
                {
                    ExportAllTextures(lvl, _reader1.Read(lvl), mode, args);
                }
            }
        }
        else if (levelType == "tr1g")
        {
            foreach (string lvl in TR1LevelNames.AsListGold)
            {
                if (File.Exists(lvl))
                {
                    ExportAllTextures(lvl, _reader1.Read(lvl), mode, args);
                }
            }
        }
        else if (levelType == "tr2g")
        {
            foreach (string lvl in TR2LevelNames.AsListGold)
            {
                if (File.Exists(lvl))
                {
                    ExportAllTextures(lvl, _reader2.Read(lvl), mode, args);
                }
            }
        }
        else if (levelType == "tr3")
        {
            foreach (string lvl in TR3LevelNames.AsOrderedList)
            {
                if (File.Exists(lvl))
                {
                    ExportAllTextures(lvl, _reader3.Read(lvl), mode, args);
                }
            }
        }
        else if (levelType == "tr3g")
        {
            foreach (string lvl in TR3LevelNames.AsListGold)
            {
                if (File.Exists(lvl))
                {
                    ExportAllTextures(lvl, _reader3.Read(lvl), mode, args);
                }
            }
        }
        else
        {
            foreach (string lvl in TR2LevelNames.AsOrderedList)
            {
                if (File.Exists(lvl))
                {
                    ExportAllTextures(lvl, _reader2.Read(lvl), mode, args);
                }
            }
        }
    }

    static TRFileVersion DetectVersion(string path)
    {
        using BinaryReader reader = new(File.Open(path, FileMode.Open));
        return (TRFileVersion)reader.ReadUInt32();
    }

    static void ExportAllTextures(string lvl, TR1Level inst, Mode mode, string[] args)
    {
        switch (mode)
        {
            case Mode.Png:
                PngExporter.Export(inst, lvl);
                break;
            case Mode.Html:
                HtmlExporter.Export(inst, lvl);
                break;
            case Mode.Faces:
                FaceMapper.DrawFaces(inst, lvl, GetRoomArgs(args[2], inst.Rooms.Count), args.Length > 3);
                break;
            case Mode.Dependencies:
                DependencyExporter.Export(inst, lvl);
                break;
            default:
                Console.WriteLine("{0} mode is not supported for TR1.", mode);
                break;
        }
    }

    static void ExportAllTextures(string lvl, TR2Level inst, Mode mode, string[] args)
    {
        switch (mode)
        {
            case Mode.Png:
                PngExporter.Export(inst, lvl);
                break;
            case Mode.Html:
                HtmlExporter.Export(inst, lvl);
                break;
            case Mode.Segments:
                SegmentExporter.Export(inst, lvl);
                break;
            case Mode.Faces:
                FaceMapper.DrawFaces(inst, lvl, GetRoomArgs(args[2], inst.Rooms.Count), args.Length > 3);
                break;
            case Mode.Boxes:
                FaceMapper.DrawBoxes(inst, lvl, GetRoomArgs(args[2], inst.Rooms.Count));
                break;
            case Mode.Dependencies:
                DependencyExporter.Export(inst, lvl);
                break;
            default:
                Console.WriteLine("{0} mode is not supported for TR2.", mode);
                break;
        }
    }

    static void ExportAllTextures(string lvl, TR3Level inst, Mode mode, string[] args)
    {
        switch (mode)
        {
            case Mode.Png:
                PngExporter.Export(inst, lvl);
                break;
            case Mode.Html:
                HtmlExporter.Export(inst, lvl);
                break;
            case Mode.Segments:
                SegmentExporter.Export(inst, lvl);
                break;
            case Mode.Faces:
                FaceMapper.DrawFaces(inst, lvl, GetRoomArgs(args[2], inst.Rooms.Count), args.Length > 3);
                break;
            case Mode.Dependencies:
                DependencyExporter.Export(inst, lvl);
                break;
            default:
                Console.WriteLine("{0} mode is not supported for TR3.", mode);
                break;
        }
    }

    static IEnumerable<int> GetRoomArgs(string arg, int fallbackCount)
    {
        if (string.Equals(arg, "all", StringComparison.CurrentCultureIgnoreCase))
        {
            return Enumerable.Range(0, fallbackCount);
        }
        return arg.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(i => i.Trim())
            .Where(i => int.TryParse(i, out int _))
            .Select(i => int.Parse(i));
    }

    static void Usage()
    {
        Console.WriteLine();
        Console.WriteLine("Usage: TextureExport [tr1 | tr1g | tr2 | tr2g | tr3 | tr3g | *.phd | *.tr2] [png | html | segments | faces | boxes | depend | dds | texinfo]");
        Console.WriteLine();

        Console.WriteLine("Target Levels");
        Console.WriteLine("\ttr1      - The original TR1 levels.");
        Console.WriteLine("\ttr1g     - The TR1 Unfinished Business levels.");
        Console.WriteLine("\ttr2      - The original TR2 levels. Default option.");
        Console.WriteLine("\ttr2g     - The TR2 Golden Mask levels.");
        Console.WriteLine("\ttr3      - The original TR3 levels.");
        Console.WriteLine("\ttr3g     - The TR3 Lost Artefact levels.");
        Console.WriteLine("\t*.phd    - Use a specific TR1 level file.");
        Console.WriteLine("\t*.tr2    - Use a specific TR2/TR3 level file.");
        Console.WriteLine();

        Console.WriteLine("Export Mode");
        Console.WriteLine("\tpng      - Export each texture tile to PNG. Default Option.");
        Console.WriteLine("\thtml     - Export all tiles to a single HTML document.");
        Console.WriteLine("\tsegments - Export each object and sprite texture to individual PNG files.");
        Console.WriteLine("\tfaces    - Create a new texture for every face in a room and mark its index. ALL can be used in place of room number list. Add additional arg to use black background.");
        Console.WriteLine("\tboxes    - Similar to faces, but mark box extents for a list of rooms.");
        Console.WriteLine("\tdepend   - Calculate which textures are shared between models and generate the JSON used in the main randomizer.");
        Console.WriteLine("\tdds      - Convert DDS files to PNG.");
        Console.WriteLine("\ttexinfo  - Generate TexInfo JSON files for texture randomization.");
        Console.WriteLine();
        
        Console.WriteLine("Examples");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\tTextureExport");
        Console.ResetColor();
        Console.WriteLine("\t\tExport all TR2 level tiles to PNG.");
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\tTextureExport tr2 html");
        Console.ResetColor();
        Console.WriteLine("\t\tExport all TR2 level tiles to HTML.");
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\tTextureExport tr2g html");
        Console.ResetColor();
        Console.WriteLine("\t\tExport all Golden Mask level tiles to HTML.");
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\tTextureExport BOAT.TR2");
        Console.ResetColor();
        Console.WriteLine("\t\tExport the Venice level tiles to PNG.");
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\tTextureExport FLOATING.TR2 segments");
        Console.ResetColor();
        Console.WriteLine("\t\tExport all object and sprite textures from Floating Islands to individual PNGs.");
        Console.WriteLine("\t\tA sub-directory will be created using the level name to store the files.");
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\tTextureExport WALL.TR2 faces 4,32");
        Console.ResetColor();
        Console.WriteLine("\t\tCreates a new texture for every face in rooms 4 and 32 and marks its index on the texture.");
        Console.WriteLine("\t\tThe level will likely be unplayable due to limits but can be viewed in trview.");
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\tTextureExport WALL.TR2 boxes 4,32");
        Console.ResetColor();
        Console.WriteLine("\t\tCreates a new texture for sectors in rooms 4 and 32, showing the box index for the sector.");
        Console.WriteLine("\t\tThe level will likely be unplayable due to limits but can be viewed in trview.");
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\tTextureExport TR3 depend");
        Console.ResetColor();
        Console.WriteLine("\t\tCycle through each TR3 level and work out which textures are shared between models.");
        Console.WriteLine("\t\tJSON files are generated for referencing in the main randomizer. This process will");
        Console.WriteLine("\t\tbe lengthy.");
        Console.WriteLine();
    }
}
