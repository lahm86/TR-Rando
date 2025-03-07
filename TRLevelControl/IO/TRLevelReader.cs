﻿using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.Numerics;
using TRLevelControl.Model;

namespace TRLevelControl;

public class TRLevelReader : BinaryReader
{
    private readonly ITRLevelObserver _observer;

    public TRLevelReader(Stream stream, ITRLevelObserver observer = null)
        : base(stream)
    {
        _observer = observer;
    }

    public TRLevelReader Inflate(TRChunkType chunkType)
    {
        long position = BaseStream.Position;
        uint expectedLength = ReadUInt32();
        uint compressedLength = ReadUInt32();

        byte[] data = ReadUInt8s(compressedLength);

        MemoryStream inflatedStream = new();
        using MemoryStream ms = new(data);
        using InflaterInputStream inflater = new(ms);

        inflater.CopyTo(inflatedStream);

        _observer?.OnChunkRead(position, BaseStream.Position, chunkType, inflatedStream.ToArray());

        if (inflatedStream.Length != expectedLength)
        {
            throw new InvalidDataException($"Inflated stream length mismatch: got {inflatedStream.Length}, expected {expectedLength}");
        }

        inflatedStream.Position = 0;
        return new(inflatedStream);
    }

    public uint PeekUInt()
    {
        uint value = ReadUInt32();
        BaseStream.Position -= sizeof(uint);
        return value;
    }

    public void ReadUntil(long position)
    {
        long distance = position - BaseStream.Position;
        for (long i = 0; i < distance; i++)
        {
            ReadByte();
        }
    }

    public byte[] ReadUInt8s(long numData)
    {
        byte[] data = new byte[numData];
        for (int i = 0; i < numData; i++)
        {
            data[i] = ReadByte();
        }

        return data;
    }

    public short[] ReadInt16s(long numData)
    {
        short[] data = new short[numData];
        for (int i = 0; i < numData; i++)
        {
            data[i] = ReadInt16();
        }

        return data;
    }

    public ushort[] ReadUInt16s(long numData)
    {
        ushort[] data = new ushort[numData];
        for (int i = 0; i < numData; i++)
        {
            data[i] = ReadUInt16();
        }

        return data;
    }

    public int[] ReadInt32s(long numData)
    {
        int[] data = new int[numData];
        for (int i = 0; i < numData; i++)
        {
            data[i] = ReadInt32();
        }

        return data;
    }

    public uint[] ReadUInt32s(long numData)
    {
        uint[] data = new uint[numData];
        for (int i = 0; i < numData; i++)
        {
            data[i] = ReadUInt32();
        }

        return data;
    }

    public float[] ReadSingles(long numData)
    {
        float[] data = new float[numData];
        for (int i = 0; i < numData; i++)
        {
            data[i] = ReadSingle();
        }
        return data;
    }

    public List<TRTexImage8> ReadImage8s(long numImages)
    {
        List<TRTexImage8> images = new((int)numImages);
        for (long i = 0; i < numImages; i++)
        {
            images.Add(new()
            {
                Pixels = ReadBytes(TRConsts.TPageSize)
            });
        }
        return images;
    }

    public List<TRTexImage16> ReadImage16s(long numImages)
    {
        List<TRTexImage16> images = new((int)numImages);
        for (long i = 0; i < numImages; i++)
        {
            images.Add(new()
            {
                Pixels = ReadUInt16s(TRConsts.TPageSize)
            });
        }
        return images;
    }

    public List<TRTexImage32> ReadImage32s(long numImages)
    {
        List<TRTexImage32> images = new((int)numImages);
        for (long i = 0; i < numImages; i++)
        {
            images.Add(ReadImage32());
        }
        return images;
    }

    public TRTexImage32 ReadImage32()
    {
        return new()
        {
            Pixels = ReadUInt32s(TRConsts.TPageSize)
        };
    }

    public List<TRColour> ReadColours(long numColours, byte multiplier = 1)
    {
        List<TRColour> colours = new((int)numColours);
        for (long i = 0; i < numColours; i++)
        {
            colours.Add(ReadColour(multiplier));
        }
        return colours;
    }

    public TRColour ReadColour(byte multiplier = 1)
    {
        return new()
        {
            Red = (byte)(ReadByte() * multiplier),
            Green = (byte)(ReadByte() * multiplier),
            Blue = (byte)(ReadByte() * multiplier)
        };
    }

    public List<TRColour4> ReadColour4s(long numColours)
    {
        List<TRColour4> colours = new((int)numColours);
        for (long i = 0; i < numColours; i++)
        {
            colours.Add(new()
            {
                Red = ReadByte(),
                Green = ReadByte(),
                Blue = ReadByte(),
                Alpha = ReadByte()
            });
        }
        return colours;
    }

    public TRColour4 ReadRGBA()
    {
        return new()
        {
            Red = ReadByte(),
            Green = ReadByte(),
            Blue = ReadByte(),
            Alpha = ReadByte()
        };
    }

    public TR5Colour ReadTR5Colour()
    {
        return new()
        {
            R = ReadSingle(),
            G = ReadSingle(),
            B = ReadSingle(),
        };
    }

    public List<TR1Entity> ReadTR1Entities(long numEntities)
    {
        List<TR1Entity> entities = new();
        for (int i = 0; i < numEntities; i++)
        {
            entities.Add(ReadTR1Entity());
        }
        return entities;
    }

    public TR1Entity ReadTR1Entity()
    {
        return new()
        {
            TypeID = (TR1Type)ReadInt16(),
            Room = ReadInt16(),
            X = ReadInt32(),
            Y = ReadInt32(),
            Z = ReadInt32(),
            Angle = ReadInt16(),
            Intensity = ReadInt16(),
            Flags = ReadUInt16()
        };
    }

    public List<TR2Entity> ReadTR2Entities(long numEntities)
    {
        List<TR2Entity> entities = new();
        for (int i = 0; i < numEntities; i++)
        {
            entities.Add(ReadTR2Entity());
        }
        return entities;
    }

    public TR2Entity ReadTR2Entity()
    {
        return new()
        {
            TypeID = (TR2Type)ReadInt16(),
            Room = ReadInt16(),
            X = ReadInt32(),
            Y = ReadInt32(),
            Z = ReadInt32(),
            Angle = ReadInt16(),
            Intensity1 = ReadInt16(),
            Intensity2 = ReadInt16(),
            Flags = ReadUInt16()
        };
    }

    public List<TR3Entity> ReadTR3Entities(long numEntities)
    {
        List<TR3Entity> entities = new();
        for (int i = 0; i < numEntities; i++)
        {
            entities.Add(ReadTR3Entity());
        }
        return entities;
    }

    public TR3Entity ReadTR3Entity()
    {
        return new()
        {
            TypeID = (TR3Type)ReadInt16(),
            Room = ReadInt16(),
            X = ReadInt32(),
            Y = ReadInt32(),
            Z = ReadInt32(),
            Angle = ReadInt16(),
            Intensity1 = ReadInt16(),
            Intensity2 = ReadInt16(),
            Flags = ReadUInt16()
        };
    }

    public List<TR4Entity> ReadTR4Entities(long numEntities)
    {
        List<TR4Entity> entities = new();
        for (int i = 0; i < numEntities; i++)
        {
            entities.Add(ReadTR4Entity());
        }
        return entities;
    }

    public TR4Entity ReadTR4Entity()
    {
        return new()
        {
            TypeID = (TR4Type)ReadInt16(),
            Room = ReadInt16(),
            X = ReadInt32(),
            Y = ReadInt32(),
            Z = ReadInt32(),
            Angle = ReadInt16(),
            Intensity = ReadInt16(),
            OCB = ReadInt16(),
            Flags = ReadUInt16()
        };
    }

    public List<TR4AIEntity> ReadTR4AIEntities(long numEntities)
    {
        List<TR4AIEntity> entities = new();
        for (int i = 0; i < numEntities; i++)
        {
            entities.Add(ReadTR4AIEntity());
        }
        return entities;
    }

    public TR4AIEntity ReadTR4AIEntity()
    {
        return new()
        {
            TypeID = (TR4Type)ReadInt16(),
            Room = ReadInt16(),
            X = ReadInt32(),
            Y = ReadInt32(),
            Z = ReadInt32(),
            OCB = ReadInt16(),
            Flags = ReadUInt16(),
            Angle = ReadInt16(),
            Box = ReadInt16()
        };
    }

    public List<TR5Entity> ReadTR5Entities(long numEntities)
    {
        List<TR5Entity> entities = new();
        for (int i = 0; i < numEntities; i++)
        {
            entities.Add(ReadTR5Entity());
        }
        return entities;
    }

    public TR5Entity ReadTR5Entity()
    {
        return new()
        {
            TypeID = (TR5Type)ReadInt16(),
            Room = ReadInt16(),
            X = ReadInt32(),
            Y = ReadInt32(),
            Z = ReadInt32(),
            Angle = ReadInt16(),
            Intensity = ReadInt16(),
            OCB = ReadInt16(),
            Flags = ReadUInt16()
        };
    }

    public List<TR5AIEntity> ReadTR5AIEntities(long numEntities)
    {
        List<TR5AIEntity> entities = new();
        for (int i = 0; i < numEntities; i++)
        {
            entities.Add(ReadTR5AIEntity());
        }
        return entities;
    }

    public TR5AIEntity ReadTR5AIEntity()
    {
        return new()
        {
            TypeID = (TR5Type)ReadInt16(),
            Room = ReadInt16(),
            X = ReadInt32(),
            Y = ReadInt32(),
            Z = ReadInt32(),
            OCB = ReadInt16(),
            Flags = ReadUInt16(),
            Angle = ReadInt16(),
            Box = ReadInt16()
        };
    }

    public List<TRFace> ReadRoomFaces(long numFaces, TRFaceType type, TRGameVersion version)
    {
        List<TRFace> faces = new();
        for (int i = 0; i < numFaces; i++)
        {
            faces.Add(ReadRoomFace(type, version));
        }
        return faces;
    }

    public TRFace ReadRoomFace(TRFaceType type, TRGameVersion version)
    {
        TRFace face = new()
        {
            Type = type,
            Vertices = new(ReadUInt16s((int)type)),
        };

        ReadFaceTexture(face, version);

        return face;
    }

    public List<TRMeshFace> ReadMeshFaces(long numFaces, TRFaceType type, TRGameVersion version)
    {
        List<TRMeshFace> faces = new();
        for (int i = 0; i < numFaces; i++)
        {
            faces.Add(ReadMeshFace(type, version));
        }
        return faces;
    }

    public TRMeshFace ReadMeshFace(TRFaceType type, TRGameVersion version)
    {
        TRMeshFace face = new()
        {
            Type = type,
            Vertices = new(ReadUInt16s((int)type)),
        };

        ReadFaceTexture(face, version);
        if (version >= TRGameVersion.TR4)
        {
            face.Effects = ReadUInt16();
        }

        return face;
    }

    private void ReadFaceTexture(TRFace face, TRGameVersion version)
    {
        ushort texture = ReadUInt16();
        if (version < TRGameVersion.TR3)
        {
            // No extra flags
            face.Texture = texture;
        }
        else
        {
            face.Texture = (ushort)(texture & (version == TRGameVersion.TR5 ? 0x3FFF : 0x7FFF));
            face.DoubleSided = (texture & 0x8000) > 0;
            if (version == TRGameVersion.TR5)
            {
                face.UnknownFlag = (texture & 0x4000) > 0;
            }
        }
    }

    public TRFixedFloat32 ReadFixed32()
    {
        return new()
        {
            Whole = ReadInt16(),
            Fraction = ReadUInt16()
        };
    }

    public TRBoundingBox ReadBoundingBox()
    {
        return new()
        {
            MinX = ReadInt16(),
            MaxX = ReadInt16(),
            MinY = ReadInt16(),
            MaxY = ReadInt16(),
            MinZ = ReadInt16(),
            MaxZ = ReadInt16()
        };
    }

    public List<TRVertex> ReadVertices(long numVertices)
    {
        List<TRVertex> vertices = new();
        for (int i = 0; i < numVertices; i++)
        {
            vertices.Add(ReadVertex());
        }
        return vertices;
    }

    public TRVertex ReadVertex(bool reverse = false)
    {
        TRVertex vertex = new()
        {
            X = ReadInt16(),
            Y = ReadInt16(),
            Z = ReadInt16()
        };

        if (reverse)
        {
            (vertex.Z, vertex.X) = (vertex.X, vertex.Z);
        }

        return vertex;
    }

    public TRVertex8 ReadVertex8()
    {
        return new()
        {
            X = ReadByte(),
            Y = ReadByte(),
            Z = ReadByte()
        };
    }

    public TRVertex32 ReadVertex32()
    {
        return new()
        {
            X = ReadInt32(),
            Y = ReadInt32(),
            Z = ReadInt32()
        };
    }

    public TR5Vertex ReadTR5Vertex()
    {
        return new()
        {
            X = ReadSingle(),
            Y = ReadSingle(),
            Z = ReadSingle()
        };
    }

    public Vector3 ReadVector3()
    {
        return new()
        {
            X = ReadSingle(),
            Y = ReadSingle(),
            Z = ReadSingle()
        };
    }

    public TRRoomInfo ReadRoomInfo(TRGameVersion version)
    {
        int x = ReadInt32();
        if (version == TRGameVersion.TR5)
        {
            ReadInt32(); // Always 0
        }

        return new()
        {
            X = x,
            Z = ReadInt32(),
            YBottom = ReadInt32(),
            YTop = ReadInt32()
        };
    }

    public List<TRRoomPortal> ReadRoomPortals(long numPortals)
    {
        List<TRRoomPortal> portals = new();
        for (int i = 0; i < numPortals; i++)
        {
            portals.Add(ReadRoomPortal());
        }
        return portals;
    }

    public TRRoomPortal ReadRoomPortal()
    {
        return new()
        {
            AdjoiningRoom = ReadUInt16(),
            Normal = ReadVertex(),
            Vertices = new(ReadVertices(4))
        };
    }

    public List<TRRoomSector> ReadRoomSectors(long numSectors, TRGameVersion version)
    {
        List<TRRoomSector> sectors = new();
        for (int i = 0; i < numSectors; i++)
        {
            sectors.Add(ReadRoomSector(version));
        }
        return sectors;
    }

    public TRRoomSector ReadRoomSector(TRGameVersion version)
    {
        TRRoomSector sector = new()
        {
            FDIndex = ReadUInt16(),
            BoxIndex = ReadUInt16(),
            RoomBelow = ReadByte(),
            Floor = ReadSByte(),
            RoomAbove = ReadByte(),
            Ceiling = ReadSByte()
        };

        if (version >= TRGameVersion.TR3 && sector.BoxIndex != TRConsts.NoBox)
        {
            sector.Material = (TRMaterial)(sector.BoxIndex & 0xF);
            sector.BoxIndex = (ushort)((sector.BoxIndex & 0x7FF0) >> 4);
        }

        return sector;
    }

    public TRBox ReadBox(TRGameVersion version)
    {
        uint zmin, zmax, xmin, xmax;
        if (version == TRGameVersion.TR1)
        {
            zmin = ReadUInt32();
            zmax = ReadUInt32();
            xmin = ReadUInt32();
            xmax = ReadUInt32();

            if ((zmax + 1) % TRConsts.Step4 == 0)
            {
                zmax++;
            }
            if ((xmax + 1) % TRConsts.Step4 == 0)
            {
                xmax++;
            }
        }
        else
        {
            zmin = (uint)(ReadByte() << TRConsts.WallShift);
            zmax = (uint)(ReadByte() << TRConsts.WallShift);
            xmin = (uint)(ReadByte() << TRConsts.WallShift);
            xmax = (uint)(ReadByte() << TRConsts.WallShift);
        }

        return new()
        {
            ZMin = zmin,
            ZMax = zmax,
            XMin = xmin,
            XMax = xmax,
            TrueFloor = ReadInt16()
        };
    }

    public List<TRObjectTextureVert> ReadObectTextureVertices(long numVertices)
    {
        List<TRObjectTextureVert> vertices = new();
        for (int i = 0; i < numVertices; i++)
        {
            vertices.Add(ReadObjectTextureVertex());
        }
        return vertices;
    }

    public TRObjectTextureVert ReadObjectTextureVertex()
    {
        return new()
        {
            U = ReadUInt16(),
            V = ReadUInt16()
        };
    }

    public List<TRSpriteTexture> ReadSpriteTextures(long numTextures, TRGameVersion version)
    {
        List<TRSpriteTexture> textures = new();
        for (int i = 0; i < numTextures; i++)
        {
            textures.Add(ReadSpriteTexture(version));
        }
        return textures;
    }

    public TRSpriteTexture ReadSpriteTexture(TRGameVersion version)
    {
        TRSpriteTexture sprite = new()
        {
            Atlas = ReadUInt16(),
        };

        byte x = ReadByte();
        byte y = ReadByte();
        ushort width = ReadUInt16();
        ushort height = ReadUInt16();
        short left = ReadInt16();
        short top = ReadInt16();
        short right = ReadInt16();
        short bottom = ReadInt16();

        if (version < TRGameVersion.TR4)
        {
            sprite.X = x;
            sprite.Y = y;
            sprite.Width = (ushort)((width + 1) / TRConsts.TPageWidth);
            sprite.Height = (ushort)((height + 1) / TRConsts.TPageHeight);
            sprite.Alignment = new()
            {
                Left = left,
                Top = top,
                Right = right,
                Bottom = bottom
            };
        }
        else
        {
            sprite.X = (byte)left;
            sprite.Y = (byte)top;
            sprite.Width = (ushort)(width / TRConsts.TPageWidth + 1);
            sprite.Height = (ushort)(height / TRConsts.TPageHeight + 1);
            sprite.Alignment = new()
            {
                Left = x,
                Top = y,
                Right = right,
                Bottom = bottom
            };
        }

        return sprite;
    }

    public List<TRAnimatedTexture> ReadAnimatedTextures(long numGroups)
    {
        List<TRAnimatedTexture> textures = new();
        for (int i = 0; i < numGroups; i++)
        {
            textures.Add(ReadAnimatedTexture());
        }
        return textures;
    }

    public TRAnimatedTexture ReadAnimatedTexture()
    {
        TRAnimatedTexture texture = new()
        {
            Textures = new()
        };

        int numTextures = ReadUInt16() + 1;
        for (int j = 0; j < numTextures; j++)
        {
            texture.Textures.Add(ReadUInt16());
        }

        return texture;
    }

    public List<TRCamera> ReadCameras(long numCameras)
    {
        List<TRCamera> cameras = new();
        for (int i = 0; i < numCameras; i++)
        {
            cameras.Add(ReadCamera());
        }
        return cameras;
    }

    public TRCamera ReadCamera()
    {
        return new()
        {
            X = ReadInt32(),
            Y = ReadInt32(),
            Z = ReadInt32(),
            Room = ReadInt16(),
            Flag = ReadUInt16()
        };
    }

    public List<TRSoundSource<T>> ReadSoundSources<T>(long numSources)
        where T : Enum
    {
        List<TRSoundSource<T>> sources = new();
        for (int i = 0; i < numSources; i++)
        {
            sources.Add(ReadSoundSource<T>());
        }
        return sources;
    }

    public TRSoundSource<T> ReadSoundSource<T>()
        where T : Enum
    {
        return new()
        {
            X = ReadInt32(),
            Y = ReadInt32(),
            Z = ReadInt32(),
            ID = (T)(object)(uint)ReadUInt16(),
            Mode = (TRSoundMode)ReadUInt16()
        };
    }

    public List<TRCinematicFrame> ReadCinematicFrames(long numFrames)
    {
        List<TRCinematicFrame> frames = new();
        for (int i = 0; i < numFrames; i++)
        {
            frames.Add(ReadCinematicFrame());
        }
        return frames;
    }

    public TRCinematicFrame ReadCinematicFrame()
    {
        return new()
        {
            Target = ReadVertex(),
            Position = ReadVertex(true),
            FOV = ReadInt16(),
            Roll = ReadInt16()
        };
    }
}
