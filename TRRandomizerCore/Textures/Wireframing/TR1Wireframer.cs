﻿using System.Drawing;
using TRImageControl;
using TRImageControl.Packing;
using TRLevelControl.Helpers;
using TRLevelControl.Model;
using TRRandomizerCore.Utilities;

namespace TRRandomizerCore.Textures;

public class TR1Wireframer : AbstractTRWireframer<TR1Type, TR1Level>
{
    private static readonly List<TR1Type> _laraEntities = new()
    {
        TR1Type.Lara, TR1Type.LaraPonytail_H_U, TR1Type.CutsceneActor1,
        TR1Type.LaraPistolAnim_H, TR1Type.LaraShotgunAnim_H, TR1Type.LaraMagnumAnim_H,
        TR1Type.LaraUziAnimation_H, TR1Type.LaraMiscAnim_H, TR1Type.CameraTarget_N,
        TR1Type.FlameEmitter_N, TR1Type.NonShootingAtlantean_N, TR1Type.ShootingAtlantean_N,
        TR1Type.MidasHand_N
    };

    private static readonly List<TR1Type> _enemyPlaceholderEntities = new()
    {
        TR1Type.NonShootingAtlantean_N, TR1Type.ShootingAtlantean_N
    };

    private static readonly List<TR1Type> _additionalEnemyEntities = new()
    {
        TR1Type.Missile1_H, TR1Type.Missile2_H, TR1Type.Missile3_H,
        TR1Type.CutsceneActor2, TR1Type.CutsceneActor3, TR1Type.CutsceneActor4,
        TR1Type.AdamEgg, TR1Type.ScionHolder, TR1Type.ScionPiece3_S_P, TR1Type.ScionPiece4_S_P,
        TR1Type.Skateboard, TR1Type.Doppelganger
    };

    private static readonly List<TR1Type> _pickupModels = new()
    {
        TR1Type.Pistols_M_H, TR1Type.Shotgun_M_H, TR1Type.Magnums_M_H, TR1Type.Uzis_M_H,
        TR1Type.ShotgunAmmo_M_H, TR1Type.MagnumAmmo_M_H, TR1Type.UziAmmo_M_H,
        TR1Type.SmallMed_M_H, TR1Type.LargeMed_M_H,
        TR1Type.Puzzle1_M_H, TR1Type.Puzzle2_M_H, TR1Type.Puzzle3_M_H, TR1Type.Puzzle4_M_H,
        TR1Type.Key1_M_H, TR1Type.Key2_M_H, TR1Type.Key3_M_H, TR1Type.Key4_M_H,
        TR1Type.Quest1_M_H, TR1Type.Quest2_M_H,
        TR1Type.ScionPiece_M_H, TR1Type.LeadBar_M_H
    };

    public override bool Is8BitPalette => true;

    private TR1TexturePacker _packer;

    protected override TRTexturePacker CreatePacker(TR1Level level)
    {
        return _packer = new TR1TexturePacker(level);
    }

    protected override bool IsSkybox(TR1Type type)
    {
        return false;
    }

    protected override bool IsInteractableModel(TR1Type type)
    {
        return TR1TypeUtilities.IsSwitchType(type)
            || TR1TypeUtilities.IsKeyholeType(type)
            || TR1TypeUtilities.IsSlotType(type)
            || TR1TypeUtilities.IsPushblockType(type)
            || type == TR1Type.Barricade
            || type == TR1Type.Compass_M_H;
    }

    protected override bool ShouldSolidifyModel(TR1Type type)
    {
        return _data.Has3DPickups && _pickupModels.Contains(type);
    }

    protected override int GetBlackPaletteIndex(TR1Level level)
    {
        return ImportColour(level, Color.Black);
    }

    protected override TRDictionary<TR1Type, TRModel> GetModels(TR1Level level)
    {
        return level.Models;
    }

    protected override IEnumerable<IEnumerable<TRFace>> GetRoomFace3s(TR1Level level)
    {
        List<List<TRFace>> faces = new();
        foreach (TR1Room room in level.Rooms)
        {
            faces.Add(room.Mesh.Triangles);
        }
        return faces;
    }

    protected override IEnumerable<IEnumerable<TRFace>> GetRoomFace4s(TR1Level level)
    {
        List<List<TRFace>> faces = new();
        foreach (TR1Room room in level.Rooms)
        {
            faces.Add(room.Mesh.Rectangles);
        }
        return faces;
    }

    protected override int ImportColour(TR1Level level, Color c)
    {
        _packer.PaletteControl ??= new();
        return _packer.PaletteControl.AddPredefinedColour(c);
    }

    protected override bool IsLaraModel(TR1Type type)
    {
        return _laraEntities.Contains(type);
    }

    protected override bool IsEnemyModel(TR1Type type)
    {
        return TR1TypeUtilities.IsEnemyType(type) || _additionalEnemyEntities.Contains(type);
    }

    protected override bool IsEnemyPlaceholderModel(TR1Type type)
    {
        return _enemyPlaceholderEntities.Contains(type);
    }

    protected override void ResetPaletteTracking(TR1Level level)
    {
        _packer.PaletteControl?.MergePredefinedColours();
    }

    protected override void SetSkyboxVisible(TR1Level level) { }

    protected override Dictionary<TRFace, List<TRVertex>> CollectLadders(TR1Level level)
    {
        return new();
    }

    protected override List<TRFace> CollectTriggerFaces(TR1Level level, List<FDTrigType> triggerTypes)
    {
        return FaceUtilities.GetTriggerFaces(level, triggerTypes, false);
    }

    protected override List<TRFace> CollectDeathFaces(TR1Level level)
    {
        return FaceUtilities.GetTriggerFaces(level, new(), true);
    }

    protected override Dictionary<ushort, TRTextileRegion> CreateSpecialSegments(TR1Level level, Color colour)
    {
        Dictionary<ushort, TRTextileRegion> segments = new();
        foreach (SpecialTextureHandling special in _data.SpecialTextures)
        {
            switch (special.Type)
            {
                case SpecialTextureType.MidasDoors:
                    foreach (ushort texture in special.Textures)
                    {
                        if (CreateMidasDoor(level, colour, texture, special.Mode) is TRTextileRegion segment)
                        {
                            segments[texture] = segment;
                        }
                    }
                    break;
            }
        }

        return segments;
    }

    private TRTextileRegion CreateMidasDoor(TR1Level level, Color colour, ushort textureIndex, SpecialTextureMode mode)
    {
        TR1Type doorType = FindDoorModel(level, textureIndex);
        if (doorType == default)
        {
            return null;
        }

        TR1Entity doorInstance = level.Entities.Find(e => e.TypeID == doorType);
        if (doorInstance == null)
        {
            return null;
        }

        const int width = 64;
        const int height = 16;
                    
        TRTextileSegment segment = CreateSegment(new(0, 0, width, height));
        TRImage frame = CreateFrame(width, height, colour, false);

        int flags = (doorInstance.Flags & 0x3E00) >> 9;
        for (int i = 0; i < 5; i++)
        {
            int x = 3 + i * 12;
            int y = 3;
            int w = 10;
            int h = 10;

            // Make a smaller rectangle
            frame.DrawRectangle(colour, x, y, w - 1, h - 1);

            // Decorate based on the door's bits
            bool doorBitSet = (flags & (1 << i)) == 0;
            switch (mode)
            {
                case SpecialTextureMode.MidasDoorBars:
                    // Bar at top = lever up; at bottom = lever down
                    y += doorBitSet ? 5 : 1;
                    frame.DrawLine(colour, x + 4, y, x + 4, y + 3);
                    frame.DrawLine(colour, x + 5, y, x + 5, y + 3);
                    break;
                case SpecialTextureMode.MidasDoorFill:
                    // Empty blocks need "filled" - levers go down
                    if (!doorBitSet)
                    {
                        frame.Fill(new(x, y, w - 1, h - 1), colour);
                    }
                    break;
                case SpecialTextureMode.MidasDoorLines:
                    if (doorBitSet)
                    {
                        // Lever up
                        frame.DrawLine(colour, x + 1, y + 4, x + 8, y + 4);
                        frame.DrawLine(colour, x + 1, y + 5, x + 8, y + 5);
                    }
                    else
                    {
                        // Lever down
                        frame.DrawLine(colour, x + 4, y + 1, x + 4, y + 8);
                        frame.DrawLine(colour, x + 5, y + 1, x + 5, y + 8);
                    }
                    break;
                case SpecialTextureMode.MidasDoorDiagonals:
                    if (doorBitSet)
                    {
                        // Lever up \
                        frame.DrawLine(colour, x + 1, y + 1, x + 8, y + 8);
                    }
                    else
                    {
                        // Lever down /
                        frame.DrawLine(colour, x + 1, y + 8, x + 8, y + 1);
                    }
                    break;
            }
        }

        return new(segment, frame);
    }

    private static TR1Type FindDoorModel(TR1Level level, ushort textureIndex)
    {
        foreach (var (type, model) in level.Models)
        {
            if (!TR1TypeUtilities.IsDoorType(type))
            {
                continue;
            }

            foreach (TRMesh mesh in model.Meshes)
            {
                if (mesh.TexturedRectangles.Any(f => f.Texture == textureIndex))
                {
                    return type;
                }
            }
        }

        return default;
    }
}
