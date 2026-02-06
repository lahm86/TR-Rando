using TRDataControl;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Model;

namespace TextureExport;

public static class GunExtras
{
    const int _unarmed = 0;
    const int _pistols = 1;
    const int _mags = 2;
    const int _autos = 3;
    const int _deagle = 4;
    const int _uzis = 5;

    static IEnumerable<TRMeshFace> GetTR1Faces(List<TRMesh> meshes, int index)
    {
        IEnumerable<TRMeshFace> targets = null;
        switch (index)
        {
            case _unarmed:
                targets = meshes.SelectMany(m => m.TexturedFaces);
                break;
            case _pistols:
            case _mags:
                targets = meshes.SelectMany(m => m.TexturedFaces).Where(f => f.Vertices.All(v => v >= 8));
                break;
            case _autos:
                targets = meshes.SelectMany(m => m.TexturedFaces).Where(f => f.Vertices.All(v => v >= 36 && v <= 43));
                break;
            case _deagle:
                targets = meshes.SelectMany(m => m.TexturedFaces).Where(f => f.Vertices.All(v => v < 8));
                break;
            case _uzis:
                targets = meshes.SelectMany(m => m.TexturedFaces).Where(f => f.Vertices.All(v => v >= 32 && v <= 39));
                break;
        }
        return targets;
    }

    public static void MakeTR2_TR1(List<TRMesh> meshes, TRModel gunModel, int index)
    {
        var targets = GetTR1Faces(meshes, index);
        if (targets != null)
        {
            var baseMesh = gunModel.Meshes[36];
            var face = baseMesh.TexturedRectangles.Find(f => f.Vertices.All(v => v >= 4));
            targets.ToList().ForEach(f => f.Texture = face.Texture);
        }
    }

    public static void MakeTR3A_TR1(List<TRMesh> meshes, TRModel gunModel, int index)
    {
        var targets = GetTR1Faces(meshes, index);
        if (targets != null)
        {
            var baseMesh = gunModel.Meshes[49];
            var face = baseMesh.TexturedRectangles.Find(f => f.Vertices.All(v => v >= 4));
            targets.ToList().ForEach(f => f.Texture = face.Texture);
        }
    }

    public static void MakeTR3B_TR1(List<TRMesh> meshes, TRModel gunModel, int index)
    {
        var targets = GetTR1Faces(meshes, index);
        if (targets != null)
        {
            var baseMesh = gunModel.Meshes[61];
            var face = baseMesh.TexturedRectangles.Find(f => f.Vertices.All(v => v >= 4));
            targets.ToList().ForEach(f => f.Texture = face.Texture);
        }
    }

    static IEnumerable<TRMeshFace> GetTR2Faces(List<TRMesh> meshes, int index)
    {
        IEnumerable<TRMeshFace> targets = null;
        switch (index)
        {
            case _unarmed:
                targets = meshes.SelectMany(m => m.TexturedFaces);
                break;
            case _pistols:
            case _deagle:
            case _uzis:
                targets = meshes.SelectMany(m => m.TexturedFaces).Where(f => f.Vertices.All(v => v < 8));
                break;
            case _mags:
                targets = meshes.SelectMany(m => m.TexturedFaces).Where(f => f.Vertices.All(v => v < 6 || v == 14 || v == 15));
                break;
            case _autos:
                targets = meshes.SelectMany(m => m.TexturedFaces).Where(f => f.Vertices.All(v => v >= 36 && v <= 43));
                break;
        }
        return targets;
    }

    public static void MakeTR1_TR2(List<TRMesh> meshes, TRModel gunModel, int index)
    {
        var targets = GetTR2Faces(meshes, index);
        if (targets != null)
        {
            var baseMesh = gunModel.Meshes[3];
            var face = baseMesh.TexturedRectangles.Find(f => f.Vertices.All(v => v >= 12));
            targets.ToList().ForEach(f => f.Texture = face.Texture);
        }
    }

    static Dictionary<TR2Type, int> _map2 = new()
    {
        [TR2Type.LaraShotgunAnim_H] = 2,
        [TR2Type.Flares_M_H] = 188,
        [TR2Type.LaraFlareAnim_H] = 192,
        [TR2Type.Flare_H] = 193,
        [TR2Type.FlareSparks_S_H] = 194,
        [TR2Type.LaraM16Anim_H] = 201,
        [TR2Type.LaraGrenadeAnim_H] = 202,
        [TR2Type.LaraHarpoonAnim_H] = 203,
        [TR2Type.M16_M_H] = 204,
        [TR2Type.GrenadeLauncher_M_H] = 205,
        [TR2Type.Harpoon_M_H] = 206,
        [TR2Type.M16Ammo_M_H] = 207,
        [TR2Type.Grenades_M_H] = 208,
        [TR2Type.HarpoonAmmo_M_H] = 209,
        [TR2Type.M16Gunflare_H] = 210,
        [TR2Type.GrenadeProjectile_H] = 211,
        [TR2Type.HarpoonProjectile_H] = 212,
        [TR2Type.LaraAutoAnim_H] = 213,
        [TR2Type.Autos_M_H] = 214,
        [TR2Type.AutoAmmo_M_H] = 215,
        [TR2Type.Flares_S_P] = 187,
        [TR2Type.M16_S_P] = 241,
        [TR2Type.GrenadeLauncher_S_P] = 242,
        [TR2Type.Harpoon_S_P] = 243,
        [TR2Type.M16Ammo_S_P] = 244,
        [TR2Type.Grenades_S_P] = 245,
        [TR2Type.HarpoonAmmo_S_P] = 246,
        [TR2Type.Automags_S_P] = 248,
        [TR2Type.AutoAmmo_S_P] = 249,
    };
    static Dictionary<TR3Type, int> _map3 = new()
    {
        [TR3Type.LaraDeagleAnimation_H] = 216,
        [TR3Type.Deagle_M_H] = 217,
        [TR3Type.DeagleAmmo_M_H] = 218,
        [TR3Type.LaraMP5Animation_H] = 219,
        [TR3Type.MP5_M_H] = 220,
        [TR3Type.MP5Ammo_M_H] = 221,
        [TR3Type.LaraRocketAnimation_H] = 222,
        [TR3Type.RocketLauncher_M_H] = 223,
        [TR3Type.Rockets_M_H] = 224,
        [TR3Type.RocketSingle] = 225,
        [TR3Type.Deagle_P] = 251,
        [TR3Type.DeagleAmmo_P] = 252,
        [TR3Type.MP5_P] = 253,
        [TR3Type.MP5Ammo_P] = 254,
        [TR3Type.RocketLauncher_P] = 255,
        [TR3Type.Rockets_P] = 256,
    };

    public static TR2Level BaseTR1Guns()
    {
        var level = Program.MakeBaseLevel();


        TRMesh hips;
        {
            var model = Program.MakeBaseModel();
            var caves = Program._reader1.Read(@"F:\tomp\all levels\tr1\level1.phd");
            var shotgunAnim = caves.Models[TR1Type.LaraShotgunAnim_H];
            caves.Palette[255] = new();
            hips = shotgunAnim.Meshes[0];
            hips.ColouredRectangles.AddRange(hips.TexturedRectangles);
            hips.ColouredTriangles.AddRange(hips.TexturedTriangles);
            hips.TexturedTriangles.Clear();
            hips.TexturedRectangles.Clear();
            hips.ColouredFaces.ToList().ForEach(f => f.Texture = 255);

            var gun = shotgunAnim.Meshes[7];
            shotgunAnim.Meshes = [hips, gun];

            Program.Import(level, model, caves, shotgunAnim, null);

            model.Meshes.RemoveAt(0);
            for (int i = 0; i < 13; i++)
                model.Meshes.Insert(0, model.Meshes[0]);

            var wall = Program._reader2.Read(@"F:\tomp\all levels\tr2\wall.tr2");
            model.Animations = wall.Models[TR2Type.LaraShotgunAnim_H].Animations;
            // Fix using holster SFX on draw
            (model.Animations[1].Commands[0] as TRSFXCommand).SoundID = (short)TR1SFX.LaraDraw;
            model.MeshTrees = shotgunAnim.MeshTrees;

            gun = model.Meshes[14];
            gun.Vertices.ForEach(v =>
            {
                v.Y += 204;
                v.Z += 25;
            });
            gun.Centre = new() { X = 42, Y = 99, Z = 70 };
            gun.CollRadius = 105;

            gun.TexturedRectangles.RemoveAll(f => f.Vertices.All(v => v < 30));
            gun.TexturedTriangles.RemoveAll(f => f.Vertices.All(v => v < 30));
            gun.Vertices.RemoveRange(0, 30);
            gun.Normals.RemoveRange(0, 30);
            gun.TexturedFaces.ToList().ForEach(f =>
            {
                for (int i = 0; i < f.Vertices.Count; i++)
                {
                    f.Vertices[i] -= 30;
                }
            });

            level.Models[(TR2Type)_map2[TR2Type.LaraShotgunAnim_H]] = model;
            hips = model.Meshes[0];
            model.Meshes[10] = model.Meshes[10].Clone();
            model.Meshes[13] = model.Meshes[13].Clone();
        }

        {
            var model = Program.MakeBaseModel();
            var wall = Program._reader2.Read(@"F:\tomp\all levels\tr2\wall.tr2");
            var flares = wall.Models[TR2Type.LaraFlareAnim_H];
            flares.Meshes = [flares.Meshes[13]];
            Program.Import(level, model, wall, flares, null);

            model.Meshes = [.. Enumerable.Repeat(0, 15).Select(i => hips.Clone())];
            model.Animations = flares.Animations;
            model.MeshTrees = flares.MeshTrees;

            level.Models[(TR2Type)_map2[TR2Type.LaraFlareAnim_H]] = model;
            
        }

        foreach (var type in new[] {
            TR2Type.LaraAutoAnim_H, TR2Type.LaraM16Anim_H, TR2Type.LaraHarpoonAnim_H, TR2Type.LaraGrenadeAnim_H,
        })
        {
            var model = Program.MakeBaseModel();
            var wall = Program._reader2.Read(@"F:\tomp\all levels\tr2\wall.tr2");
            var flares = wall.Models[type];
            flares.Meshes = [flares.Meshes[14]];
            Program.Import(level, model, wall, flares, null);

            model.Meshes.RemoveAt(0);
            var gun = model.Meshes[0];
            model.Meshes = [.. Enumerable.Repeat(0, 15).Select(i => hips)];
            if (type != TR2Type.LaraAutoAnim_H)
                model.Meshes[14] = gun;
            model.Animations = flares.Animations;
            model.MeshTrees = flares.MeshTrees;

            level.Models[(TR2Type)_map2[type]] = model;

            model.Meshes[10] = model.Meshes[10].Clone();
            if (type == TR2Type.LaraAutoAnim_H)
            {
                model.Meshes[1] = model.Meshes[1].Clone();
                model.Meshes[4] = model.Meshes[4].Clone();
                model.Meshes[13] = model.Meshes[13].Clone();
            }
        }

        foreach (var type in new[] {
            TR2Type.Flare_H, TR2Type.FlareSparks_S_H, TR2Type.Flares_M_H,
            TR2Type.M16_M_H, TR2Type.M16Ammo_M_H, TR2Type.M16Gunflare_H,
            TR2Type.GrenadeLauncher_M_H, TR2Type.GrenadeProjectile_H, TR2Type.Grenades_M_H,
            TR2Type.Harpoon_M_H, TR2Type.HarpoonAmmo_M_H, TR2Type.HarpoonProjectile_H,
            TR2Type.AutoAmmo_M_H, TR2Type.Autos_M_H,
        })
        {
            var model = Program.MakeBaseModel();
            var wall = Program._reader2.Read(@"F:\tomp\all levels\tr2\wall.tr2");
            var flares = wall.Models[type];
            Program.Import(level, model, wall, flares, null);

            model.Meshes.RemoveAt(0);
            model.Animations = flares.Animations;
            model.MeshTrees = flares.MeshTrees;

            level.Models[(TR2Type)_map2[type]] = model;
        }

        foreach (var type in new[] {
            TR2Type.Flares_S_P,
            TR2Type.M16_S_P,
            TR2Type.GrenadeLauncher_S_P,
            TR2Type.Harpoon_S_P,
            TR2Type.M16Ammo_S_P,
            TR2Type.Grenades_S_P,
            TR2Type.HarpoonAmmo_S_P,
            TR2Type.Automags_S_P,
            TR2Type.AutoAmmo_S_P
        })
        {
            var wall = Program._reader2.Read(@"F:\tomp\all levels\tr2\wall.tr2");
            Program.ImportSprite(level, wall, type, (TR2Type)_map2[type]);
        }

        foreach (var type in new[] {
            TR3Type.LaraDeagleAnimation_H, TR3Type.LaraMP5Animation_H, TR3Type.LaraRocketAnimation_H,
        })
        {
            var model = Program.MakeBaseModel();
            var wall = Program._reader3.Read(@"F:\tomp\all levels\tr3\jungle.tr2");
            var flares = wall.Models[type];
            flares.Meshes = [flares.Meshes[14]];
            Program.Import(level, model, wall, flares, null);

            model.Meshes.RemoveAt(0);
            var gun = model.Meshes[0];
            model.Meshes = [.. Enumerable.Repeat(0, 15).Select(i => hips)];
            if (type != TR3Type.LaraDeagleAnimation_H)
                model.Meshes[14] = gun;
            model.Animations = flares.Animations;
            model.MeshTrees = flares.MeshTrees;

            level.Models[(TR2Type)_map3[type]] = model;

            model.Meshes[10] = model.Meshes[10].Clone();
            if (type == TR3Type.LaraDeagleAnimation_H)
            {
                model.Meshes[1] = model.Meshes[1].Clone();
                model.Meshes[4] = model.Meshes[4].Clone();
            }
        }

        foreach (var type in new[] {
            TR3Type.Deagle_M_H, TR3Type.DeagleAmmo_M_H, TR3Type.MP5_M_H, TR3Type.MP5Ammo_M_H,
            TR3Type.RocketLauncher_M_H, TR3Type.Rockets_M_H, TR3Type.RocketSingle,
        })
        {
            var model = Program.MakeBaseModel();
            var wall = Program._reader3.Read(@"F:\tomp\all levels\tr3\jungle.tr2");
            var flares = wall.Models[type];
            Program.Import(level, model, wall, flares, null);

            model.Meshes.RemoveAt(0);
            model.Animations = flares.Animations;
            model.MeshTrees = flares.MeshTrees;

            level.Models[(TR2Type)_map3[type]] = model;
        }

        var map4 = new Dictionary<TR3Type, string>
        {
            [TR3Type.Deagle_P] = "DEAGLE_S_P",
            [TR3Type.DeagleAmmo_P] = "DEAGLEAMMO_S_P",
            [TR3Type.MP5_P] = "MP5_S_P",
            [TR3Type.MP5Ammo_P] = "MP5AMMO_S_P",
            [TR3Type.RocketLauncher_P] = "ROCKETLAUNCHER_S_P",
            [TR3Type.Rockets_P] = "ROCKETAMMO_S_P",
        };
        foreach (var (type,name) in map4)
        {
            var blob = TRBlobControl.Read<TR1Blob>(name + ".TRB");
            var tile = new TRImage();
            tile.Import(blob.Textures.First().Image, new(0, 0));
            level.Images16.Add(new() { Pixels = tile.ToRGB555() });
            level.Images8.Add(new() { Pixels = tile.ToRGB(level.Palette) });
            var tinfo = blob.Textures.First().Segments.First().Texture as TRSpriteTexture;
            tinfo.Position = new(0, 0);
            tinfo.Atlas = (ushort)(level.Images16.Count - 1);
            level.Sprites[(TR2Type)_map3[type]] = new()
            {
                Textures = [tinfo],
            };
        }

        return level;
    }

    public static void MakeTR1Guns()
    {
        var level = BaseTR1Guns();
        Program.Repack(level);
        Program._reader2.Write(level, "tr1guns.tr2");
    }

    public static void MakeTR1GymGuns()
    {
        var level = BaseTR1Guns();

        var hips = level.Models[(TR2Type)_map2[TR2Type.LaraShotgunAnim_H]].Meshes[0];

        foreach (var type in new[] { TR1Type.Lara, TR1Type.LaraPistolAnim_H, TR1Type.LaraMagnumAnim_H, TR1Type.LaraUziAnimation_H, TR1Type.Gunflare_H })
        {
            var model = Program.MakeBaseModel();
            var wall = Program._reader1.Read(@"F:\tomp\all levels\tr1\level1.phd");
            var flares = wall.Models[type];
            if (type != TR1Type.Gunflare_H)
                flares.Meshes = [flares.Meshes[14]];
            Program.Import(level, model, wall, flares, null);

            model.Meshes.RemoveAt(0);

            if (type == TR1Type.Lara)
            {
                model.Meshes = [.. Enumerable.Repeat(0, 15).Select(i => hips.Clone())];
                model.Meshes[0] = hips;
            }
            else if (type != TR1Type.Gunflare_H)
            {
                model.Meshes = [.. Enumerable.Repeat(0, 15).Select(i => hips)];
                model.Meshes[1] = model.Meshes[1].Clone();
                model.Meshes[4] = model.Meshes[4].Clone();
                model.Meshes[10] = model.Meshes[10].Clone();
                model.Meshes[13] = model.Meshes[13].Clone();
                if (type == TR1Type.LaraUziAnimation_H)
                {
                    model.Meshes[14] = model.Meshes[14].Clone();
                }
            }

            model.Animations = flares.Animations;
            model.MeshTrees = flares.MeshTrees;

            level.Models[(TR2Type)(int)type] = model;
        }

        {
            var wall = Program._reader1.Read(@"F:\tomp\all levels\tr1\level1.phd");
            Program.ImportSprite(level, wall, TR1Type.Ricochet_S_H, (TR2Type)(int)TR1Type.Ricochet_S_H);
        }

        Program.Repack(level);
        Program._reader2.Write(level, "tr1gymguns.tr2");
    }

    static readonly Dictionary<TR1Type, int> _map21 = new()
    {
        [TR1Type.LaraMagnumAnim_H] = 279,
        [TR1Type.Magnums_M_H] = 280,
        [TR1Type.MagnumAmmo_M_H] = 281,
        [TR1Type.Magnums_S_P] = 282,
        [TR1Type.MagnumAmmo_S_P] = 283,
    };

    static readonly Dictionary<TR3Type, int> _map23 = new()
    {
        [TR3Type.LaraDeagleAnimation_H] = 285,
        [TR3Type.Deagle_M_H] = 286,
        [TR3Type.DeagleAmmo_M_H] = 287,
        [TR3Type.Deagle_P] = 288,
        [TR3Type.DeagleAmmo_P] = 289,
        [TR3Type.LaraMP5Animation_H] = 290,
        [TR3Type.MP5_M_H] = 291,
        [TR3Type.MP5Ammo_M_H] = 292,
        [TR3Type.MP5_P] = 293,
        [TR3Type.MP5Ammo_P] = 294,
        [TR3Type.LaraRocketAnimation_H] = 295,
        [TR3Type.RocketLauncher_M_H] = 296,
        [TR3Type.Rockets_M_H] = 297,
        [TR3Type.RocketSingle] = 298,
        [TR3Type.RocketLauncher_P] = 299,
        [TR3Type.Rockets_P] = 300,
    };

    public static TR2Level BaseTR2Guns()
    {
        var level = Program.MakeBaseLevel();

        TRMesh hips;
        {
            var model = Program.MakeBaseModel();
            var caves = Program._reader1.Read(@"F:\tomp\all levels\tr1\level1.phd");
            var magnumAnim = caves.Models[TR1Type.LaraMagnumAnim_H];
            caves.Palette[255] = new();
            hips = magnumAnim.Meshes[0];
            hips.ColouredRectangles.AddRange(hips.TexturedRectangles);
            hips.ColouredTriangles.AddRange(hips.TexturedTriangles);
            hips.TexturedTriangles.Clear();
            hips.TexturedRectangles.Clear();
            hips.ColouredFaces.ToList().ForEach(f => f.Texture = 255);

            magnumAnim.Meshes = [hips];

            Program.Import(level, model, caves, magnumAnim, null);

            model.Meshes = [.. Enumerable.Repeat(0, 15).Select(i => hips)];
            model.Meshes[1] = model.Meshes[1].Clone();
            model.Meshes[4] = model.Meshes[4].Clone();
            model.Meshes[10] = model.Meshes[10].Clone();
            model.Meshes[13] = model.Meshes[13].Clone();

            level.Models[(TR2Type)_map21[TR1Type.LaraMagnumAnim_H]] = model;
            model.MeshTrees = magnumAnim.MeshTrees;
            model.Animations = magnumAnim.Animations;
            hips = model.Meshes[0];
        }

        foreach (var type in new[] {
            TR1Type.Magnums_M_H, TR1Type.MagnumAmmo_M_H,
        })
        {
            var model = Program.MakeBaseModel();
            var wall = Program._reader1.Read(@"F:\tomp\all levels\tr1\level1.phd");
            var flares = wall.Models[type];
            Program.Import(level, model, wall, flares, null);

            model.Meshes.RemoveAt(0);
            model.Animations = flares.Animations;
            model.MeshTrees = flares.MeshTrees;

            if (type == TR1Type.Magnums_M_H)
            {
                model.Meshes[0].TexturedTriangles.Clear();
            }

            level.Models[(TR2Type)_map21[type]] = model;
        }

        foreach (var type in new[] {
            TR1Type.Magnums_S_P,
            TR1Type.MagnumAmmo_S_P,
        })
        {
            var wall = Program._reader1.Read(@"F:\tomp\all levels\tr1\level1.phd");
            Program.ImportSprite(level, wall, type, (TR2Type)_map21[type]);
        }

        foreach (var type in new[] {
            TR3Type.LaraDeagleAnimation_H, TR3Type.LaraMP5Animation_H, TR3Type.LaraRocketAnimation_H,
        })
        {
            var model = Program.MakeBaseModel();
            var wall = Program._reader3.Read(@"F:\tomp\all levels\tr3\jungle.tr2");
            var flares = wall.Models[type];
            flares.Meshes = [flares.Meshes[14]];
            Program.Import(level, model, wall, flares, null);

            model.Meshes.RemoveAt(0);
            var gun = model.Meshes[0];
            model.Meshes = [.. Enumerable.Repeat(0, 15).Select(i => hips)];
            if (type != TR3Type.LaraDeagleAnimation_H)
                model.Meshes[14] = gun;
            model.Animations = flares.Animations;
            model.MeshTrees = flares.MeshTrees;

            level.Models[(TR2Type)_map23[type]] = model;

            model.Meshes[10] = model.Meshes[10].Clone();
            if (type == TR3Type.LaraDeagleAnimation_H)
            {
                model.Meshes[1] = model.Meshes[1].Clone();
                model.Meshes[4] = model.Meshes[4].Clone();
            }
        }

        foreach (var type in new[] {
            TR3Type.Deagle_M_H, TR3Type.DeagleAmmo_M_H, TR3Type.MP5_M_H, TR3Type.MP5Ammo_M_H,
            TR3Type.RocketLauncher_M_H, TR3Type.Rockets_M_H, TR3Type.RocketSingle,
        })
        {
            var model = Program.MakeBaseModel();
            var wall = Program._reader3.Read(@"F:\tomp\all levels\tr3\jungle.tr2");
            var flares = wall.Models[type];
            Program.Import(level, model, wall, flares, null);

            model.Meshes.RemoveAt(0);
            model.Animations = flares.Animations;
            model.MeshTrees = flares.MeshTrees;

            level.Models[(TR2Type)_map23[type]] = model;
        }

        var map4 = new Dictionary<TR3Type, string>
        {
            [TR3Type.Deagle_P] = "DEAGLE_S_P",
            [TR3Type.DeagleAmmo_P] = "DEAGLEAMMO_S_P",
            [TR3Type.MP5_P] = "MP5_S_P",
            [TR3Type.MP5Ammo_P] = "MP5AMMO_S_P",
            [TR3Type.RocketLauncher_P] = "ROCKETLAUNCHER_S_P",
            [TR3Type.Rockets_P] = "ROCKETAMMO_S_P",
        };
        foreach (var (type, name) in map4)
        {
            var blob = TRBlobControl.Read<TR2Blob>("2/" + name + ".TRB");
            var tile = new TRImage();
            tile.Import(blob.Textures.First().Image, new(0, 0));
            level.Images16.Add(new() { Pixels = tile.ToRGB555() });
            level.Images8.Add(new() { Pixels = tile.ToRGB(level.Palette) });
            var tinfo = blob.Textures.First().Segments.First().Texture as TRSpriteTexture;
            tinfo.Position = new(0, 0);
            tinfo.Atlas = (ushort)(level.Images16.Count - 1);
            level.Sprites[(TR2Type)_map23[type]] = new()
            {
                Textures = [tinfo],
            };
        }

        return level;
    }

    public static void MakeTR2Guns()
    {
        var level = BaseTR2Guns();
        Program.Repack(level);
        Program._reader2.Write(level, "tr2guns.tr2");
    }

    public static void MakeTR2GymGuns()
    {
        var level = BaseTR2Guns();

        var hips = level.Models[(TR2Type)_map21[TR1Type.LaraMagnumAnim_H]].Meshes[0];

        foreach (var type in new[] { TR2Type.LaraPistolAnim_H, TR2Type.LaraShotgunAnim_H, TR2Type.LaraAutoAnim_H, TR2Type.LaraUziAnim_H, TR2Type.LaraM16Anim_H, TR2Type.LaraGrenadeAnim_H, TR2Type.LaraHarpoonAnim_H})
        {
            var model = Program.MakeBaseModel();
            var wall = Program._reader2.Read(@"F:\tomp\all levels\tr2\wall.tr2");
            var flares = wall.Models[type];
            
            flares.Meshes = [flares.Meshes[14]];
            Program.Import(level, model, wall, flares, null);

            model.Meshes.RemoveAt(0);

            var gun = model.Meshes[0];
            if (type == TR2Type.LaraPistolAnim_H)
            {
                model.Meshes[0] = hips;
            }
            model.Meshes = [.. Enumerable.Repeat(0, 15).Select(i => hips)];
            if (type == TR2Type.LaraPistolAnim_H || type == TR2Type.LaraAutoAnim_H || type == TR2Type.LaraUziAnim_H)
            {
                model.Meshes[1] = model.Meshes[1].Clone();
                model.Meshes[4] = model.Meshes[4].Clone();
            }
            model.Meshes[10] = model.Meshes[10].Clone();
            if (type != TR2Type.LaraGrenadeAnim_H && type != TR2Type.LaraM16Anim_H && type != TR2Type.LaraHarpoonAnim_H)
            {
                model.Meshes[13] = model.Meshes[13].Clone();
            }
            if (type != TR2Type.LaraPistolAnim_H && type != TR2Type.LaraAutoAnim_H)
            {
                model.Meshes[14] = type == TR2Type.LaraUziAnim_H ? hips.Clone() : gun.Clone();
            }

            model.Animations = flares.Animations;
            model.MeshTrees = flares.MeshTrees;

            level.Models[(TR2Type)(int)type] = model;
        }

        foreach (var type in new[] { TR2Type.Pistols_M_H, TR2Type.Shotgun_M_H, TR2Type.Autos_M_H, TR2Type.Uzi_M_H, TR2Type.Harpoon_M_H, TR2Type.M16_M_H, TR2Type.GrenadeLauncher_M_H, TR2Type.Gunflare_H, TR2Type.M16Gunflare_H })
        {
            var model = Program.MakeBaseModel();
            var wall = Program._reader2.Read(@"F:\tomp\all levels\tr2\wall.tr2");
            var flares = wall.Models[type];
            Program.Import(level, model, wall, flares, null);

            model.Meshes.RemoveAt(0);
            model.Animations = flares.Animations;
            model.MeshTrees = flares.MeshTrees;

            level.Models[type] = model;
        }

        level.Sprites.Clear();

        Program.Repack(level);
        Program._reader2.Write(level, "tr2gymguns.tr2");
    }

    public static void MakeTR2HSHGuns()
    {
        var level = BaseTR2Guns();

        var hips = level.Models[(TR2Type)_map21[TR1Type.LaraMagnumAnim_H]].Meshes[0];

        foreach (var type in new[] { TR2Type.LaraPistolAnim_H, TR2Type.LaraAutoAnim_H, TR2Type.LaraUziAnim_H, TR2Type.LaraM16Anim_H, TR2Type.LaraGrenadeAnim_H, TR2Type.LaraHarpoonAnim_H })
        {
            var model = Program.MakeBaseModel();
            var wall = Program._reader2.Read(@"F:\tomp\all levels\tr2\wall.tr2");
            var flares = wall.Models[type];

            flares.Meshes = [flares.Meshes[14]];
            Program.Import(level, model, wall, flares, null);

            model.Meshes.RemoveAt(0);

            var gun = model.Meshes[0];
            if (type == TR2Type.LaraPistolAnim_H)
            {
                model.Meshes[0] = hips;
            }
            model.Meshes = [.. Enumerable.Repeat(0, 15).Select(i => hips)];
            if (type == TR2Type.LaraPistolAnim_H || type == TR2Type.LaraAutoAnim_H || type == TR2Type.LaraUziAnim_H)
            {
                model.Meshes[1] = model.Meshes[1].Clone();
                model.Meshes[4] = model.Meshes[4].Clone();
            }
            model.Meshes[10] = model.Meshes[10].Clone();
            if (type != TR2Type.LaraGrenadeAnim_H && type != TR2Type.LaraM16Anim_H && type != TR2Type.LaraHarpoonAnim_H)
            {
                model.Meshes[13] = model.Meshes[13].Clone();
            }
            if (type != TR2Type.LaraPistolAnim_H && type != TR2Type.LaraAutoAnim_H)
            {
                model.Meshes[14] = type == TR2Type.LaraUziAnim_H ? hips.Clone() : gun.Clone();
            }

            model.Animations = flares.Animations;
            model.MeshTrees = flares.MeshTrees;

            level.Models[(TR2Type)(int)type] = model;
        }

        foreach (var type in new[] { TR2Type.Pistols_M_H, TR2Type.Autos_M_H, TR2Type.Uzi_M_H, TR2Type.Harpoon_M_H, TR2Type.M16_M_H, TR2Type.GrenadeLauncher_M_H, TR2Type.M16Gunflare_H, TR2Type.GrenadeProjectile_H, TR2Type.HarpoonProjectile_H })
        {
            var model = Program.MakeBaseModel();
            var wall = Program._reader2.Read(@"F:\tomp\all levels\tr2\wall.tr2");
            var flares = wall.Models[type];
            Program.Import(level, model, wall, flares, null);

            model.Meshes.RemoveAt(0);
            model.Animations = flares.Animations;
            model.MeshTrees = flares.MeshTrees;

            level.Models[type] = model;
        }

        level.Sprites.Clear();
        {
            var wall = Program._reader2.Read(@"F:\tomp\all levels\tr2\wall.tr2");
            Program.ImportSprite(level, wall, TR2Type.Pistols_S_P, TR2Type.Pistols_S_P);
        }

        Program.Repack(level);
        Program._reader2.Write(level, "tr2hshguns.tr2");
    }

    public static void MakeTR2VegasGuns()
    {
        var level = BaseTR2Guns();

        var hips = level.Models[(TR2Type)_map21[TR1Type.LaraMagnumAnim_H]].Meshes[0];

        foreach (var type in new[] { TR2Type.LaraM16Anim_H, TR2Type.LaraGrenadeAnim_H, TR2Type.LaraHarpoonAnim_H })
        {
            var model = Program.MakeBaseModel();
            var wall = Program._reader2.Read(@"F:\tomp\all levels\tr2\wall.tr2");
            var flares = wall.Models[type];

            flares.Meshes = [flares.Meshes[14]];
            Program.Import(level, model, wall, flares, null);

            model.Meshes.RemoveAt(0);

            var gun = model.Meshes[0];
            if (type == TR2Type.LaraPistolAnim_H)
            {
                model.Meshes[0] = hips;
            }
            model.Meshes = [.. Enumerable.Repeat(0, 15).Select(i => hips)];
            if (type == TR2Type.LaraPistolAnim_H || type == TR2Type.LaraAutoAnim_H || type == TR2Type.LaraUziAnim_H)
            {
                model.Meshes[1] = model.Meshes[1].Clone();
                model.Meshes[4] = model.Meshes[4].Clone();
            }
            model.Meshes[10] = model.Meshes[10].Clone();
            if (type != TR2Type.LaraGrenadeAnim_H && type != TR2Type.LaraM16Anim_H && type != TR2Type.LaraHarpoonAnim_H)
            {
                model.Meshes[13] = model.Meshes[13].Clone();
            }
            if (type != TR2Type.LaraPistolAnim_H && type != TR2Type.LaraAutoAnim_H)
            {
                model.Meshes[14] = type == TR2Type.LaraUziAnim_H ? hips.Clone() : gun.Clone();
            }

            model.Animations = flares.Animations;
            model.MeshTrees = flares.MeshTrees;

            level.Models[(TR2Type)(int)type] = model;
        }

        foreach (var type in new[] { TR2Type.Harpoon_M_H, TR2Type.M16_M_H, TR2Type.GrenadeLauncher_M_H, TR2Type.Gunflare_H, TR2Type.M16Gunflare_H, TR2Type.GrenadeProjectile_H, TR2Type.HarpoonProjectile_H })
        {
            var model = Program.MakeBaseModel();
            var wall = Program._reader2.Read(@"F:\tomp\all levels\tr2\wall.tr2");
            var flares = wall.Models[type];
            Program.Import(level, model, wall, flares, null);

            model.Meshes.RemoveAt(0);
            model.Animations = flares.Animations;
            model.MeshTrees = flares.MeshTrees;

            level.Models[type] = model;
        }

        level.Sprites.Clear();

        Program.Repack(level);
        Program._reader2.Write(level, "tr2vegasguns.tr2");
    }
}
