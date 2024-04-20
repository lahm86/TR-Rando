﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using TRLevelControl.Model;

namespace TRLevelControlTests;

public class TR45Observer : ObserverBase
{
    private readonly Dictionary<TRChunkType, ZipWrapper> _inflatedReads = new();
    private readonly Dictionary<TRChunkType, ZipWrapper> _inflatedWrites = new();

    private readonly Dictionary<uint, List<byte>> _meshPadding = new();

    public override void TestOutput(byte[] input, byte[] output)
    {
        CollectionAssert.AreEquivalent(_inflatedReads.Keys, _inflatedWrites.Keys);

        foreach (TRChunkType type in _inflatedReads.Keys)
        {
            CollectionAssert.AreEqual(_inflatedReads[type].Data, _inflatedWrites[type].Data);
        }

        // At this stage, everything zipped matches. We want to check for unzipped matches, so do
        // so by stripping out everything that has been zipped from both streams.
        List<byte> oldData = new(input);
        List<byte> newData = new(output);

        List<ZipWrapper> unzips = new(_inflatedReads.Values);
        unzips.Sort((z1, z2) => z2.StreamEnd.CompareTo(z1.StreamEnd));
        foreach (ZipWrapper zip in unzips)
        {
            oldData.RemoveRange((int)zip.StreamStart, (int)(zip.StreamEnd - zip.StreamStart));
        }

        unzips = new(_inflatedWrites.Values);
        unzips.Sort((z1, z2) => z2.StreamEnd.CompareTo(z1.StreamEnd));
        foreach (ZipWrapper zip in unzips)
        {
            newData.RemoveRange((int)zip.StreamStart, (int)(zip.StreamEnd - zip.StreamStart));
        }

        CollectionAssert.AreEqual(oldData, newData);
    }

    public override void OnChunkRead(long startPosition, long endPosition, TRChunkType chunkType, byte[] data)
    {
        _inflatedReads[chunkType] = new()
        {
            StreamStart = startPosition,
            StreamEnd = endPosition,
            Data = data
        };
    }

    public override void OnChunkWritten(long startPosition, long endPosition, TRChunkType chunkType, byte[] data)
    {
        _inflatedWrites[chunkType] = new()
        {
            StreamStart = startPosition,
            StreamEnd = endPosition,
            Data = data
        };
    }

    public override void OnMeshPaddingRead(uint meshPointer, List<byte> values)
    {
        _meshPadding[meshPointer] = values;
    }

    public override List<byte> GetMeshPadding(uint meshPointer)
    {
        return _meshPadding.ContainsKey(meshPointer) ? _meshPadding[meshPointer] : null;
    }

    class ZipWrapper
    {
        public long StreamStart { get; set; }
        public long StreamEnd { get; set; }
        public byte[] Data { get; set; }
    }
}