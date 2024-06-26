﻿using System.Diagnostics;
using TRLevelControl.Model;

namespace TRImageControl.Packing;

public class TR4TexturePacker : TRTexturePacker
{
    private readonly TR4Level _level;
    private readonly TRGroupPackingMode _mode;
    private readonly int _numImages;

    private Dictionary<TRGroupPackingMode, List<TRTexture>> _textureCache;

    public override int NumLevelImages => _numImages;

    public TR4TexturePacker(TR4Level level, TRGroupPackingMode mode, int maximumTiles = 32)
        : base(GetMaximumTiles(level, mode, maximumTiles))
    {
        _level = level;
        _mode = mode;
        _numImages = mode switch
        {
            TRGroupPackingMode.Room => _level.Images.Rooms.Count,
            TRGroupPackingMode.Object => _level.Images.Objects.Count,
            TRGroupPackingMode.Bump => _level.Images.Bump.Count,
            _ => level.Images.Count,
        };

        LoadLevel();
    }

    private static int GetMaximumTiles(TR4Level level, TRGroupPackingMode mode, int maximumTiles)
    {
        return mode switch
        {
            TRGroupPackingMode.Room => maximumTiles - 2 - level.Images.Objects.Count - level.Images.Bump.Count,
            TRGroupPackingMode.Object => maximumTiles - 2 - level.Images.Rooms.Count - level.Images.Bump.Count,
            TRGroupPackingMode.Bump => maximumTiles - 2 - level.Images.Rooms.Count - level.Images.Objects.Count,
            _ => level.Images.Count,
        };
    }

    public override TRImage GetImage(int tileIndex)
    {
        TRTexImage32 image = _mode switch
        {
            TRGroupPackingMode.Room => _level.Images.Rooms.Images32[tileIndex],
            TRGroupPackingMode.Object => _level.Images.Objects.Images32[tileIndex],
            TRGroupPackingMode.Bump => _level.Images.Bump.Images32[tileIndex],
            TRGroupPackingMode.All => _level.Images.GetImage32(tileIndex),
            _ => throw new Exception(),
        };

        return new(image.Pixels);
    }

    public override void SetImage(int tileIndex, TRImage image)
    {
        TRTexImage16 image16;
        TRTexImage32 image32;
        switch (_mode)
        {
            case TRGroupPackingMode.Room:
                image16 = _level.Images.Rooms.Images16[tileIndex];
                image32 = _level.Images.Rooms.Images32[tileIndex];
                break;
            case TRGroupPackingMode.Object:
                image16 = _level.Images.Objects.Images16[tileIndex];
                image32 = _level.Images.Objects.Images32[tileIndex];
                break;
            case TRGroupPackingMode.Bump:
                image16 = _level.Images.Bump.Images16[tileIndex];
                image32 = _level.Images.Bump.Images32[tileIndex];
                break;
            default:
                throw new Exception("All packing mode is read-only.");
        }

        image16.Pixels = image.ToRGB555();
        image32.Pixels = image.ToRGB32();
    }

    protected override void CreateImageSpace(int count)
    {
        List<TRTexImage16> images16;
        List<TRTexImage32> images32;
        switch (_mode)
        {
            case TRGroupPackingMode.Room:
                images16 = _level.Images.Rooms.Images16;
                images32 = _level.Images.Rooms.Images32;
                break;
            case TRGroupPackingMode.Object:
                images16 = _level.Images.Objects.Images16;
                images32 = _level.Images.Objects.Images32;
                break;
            case TRGroupPackingMode.Bump:
                images16 = _level.Images.Bump.Images16;
                images32 = _level.Images.Bump.Images32;
                break;
            default:
                throw new Exception("All packing mode is read-only.");
        }

        for (int i = 0; i < count; i++)
        {
            images32.Add(new());
            images16.Add(new());
        }
    }

    protected override List<TRTextileSegment> LoadObjectSegments()
    {
        List<TRTextileSegment> segments = new();
        List<TRTexture> group = null;
        if (_mode != TRGroupPackingMode.All)
        {
            CacheTextures();
            group = _textureCache[_mode];
        }

        for (int i = 0; i < _level.ObjectTextures.Count; i++)
        {
            TRObjectTexture texture = _level.ObjectTextures[i];
            if (group != null && !group.Contains(texture) || !texture.IsValid())
                continue;

            segments.Add(new()
            {
                Index = i,
                Texture = texture,
            });
        }

        return segments;
    }

    protected override List<TRTextileSegment> LoadSpriteSegments()
    {
        List<TRTextileSegment> segments = new();
        if (_mode != TRGroupPackingMode.Object && _mode != TRGroupPackingMode.All)
        {
            return segments;
        }

        List<TRSpriteTexture> sprites = _level.Sprites.SelectMany(s => s.Value.Textures).ToList();
        for (int i = 0; i < sprites.Count; i++)
        {
            TRSpriteTexture texture = sprites[i];
            segments.Add(new()
            {
                Index = i,
                Texture = texture,
            });
        }

        return segments;
    }

    private void CacheTextures()
    {
        ushort roomCount = (ushort)_level.Images.Rooms.Count;
        ushort objectCount = (ushort)_level.Images.Objects.Count;
        _textureCache = new()
        {
            [TRGroupPackingMode.Room] = new(),
            [TRGroupPackingMode.Object] = new(),
            [TRGroupPackingMode.Bump] = new(),
        };

        foreach (TRObjectTexture texture in _level.ObjectTextures)
        {
            if (texture.Atlas < roomCount)
            {
                _textureCache[TRGroupPackingMode.Room].Add(texture);
            }
            else if (texture.Atlas < roomCount + objectCount)
            {
                _textureCache[TRGroupPackingMode.Object].Add(texture);
                texture.Atlas -= roomCount;
            }
            else
            {
                _textureCache[TRGroupPackingMode.Bump].Add(texture);
                texture.Atlas -= roomCount;
                texture.Atlas -= objectCount;
            }
        }

        foreach (TRSpriteTexture texture in _level.Sprites.SelectMany(s => s.Value.Textures))
        {
            Debug.Assert(texture.Atlas >= roomCount && texture.Atlas < roomCount + objectCount);
            _textureCache[TRGroupPackingMode.Object].Add(texture);
            texture.Atlas -= roomCount;
        }
    }

    protected override void PostCommit()
    {
        if (_mode == TRGroupPackingMode.All)
        {
            throw new Exception("All packing mode is read-only.");
        }

        IEnumerable<TRTexture> addedTextures = _rectangles.SelectMany(r => r.Segments).Select(s => s.Texture);
        if (_mode == TRGroupPackingMode.Object)
        {
            _textureCache[TRGroupPackingMode.Object].AddRange(addedTextures);
        }
        else if (_mode == TRGroupPackingMode.Bump)
        {
            _textureCache[TRGroupPackingMode.Bump].AddRange(addedTextures);
        }

        ushort offset = (ushort)_level.Images.Rooms.Count;
        foreach (TRTexture texture in _textureCache[TRGroupPackingMode.Object])
        {
            texture.Atlas += offset;
        }

        offset += (ushort)_level.Images.Objects.Count;
        foreach (TRTexture texture in _textureCache[TRGroupPackingMode.Bump])
        {
            texture.Atlas += offset;
        }
    }
}
