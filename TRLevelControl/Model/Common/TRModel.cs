﻿namespace TRLevelControl.Model;

public class TRModel : ICloneable
{
    public List<TRAnimation> Animations { get; set; } = new();
    public List<TRMeshTreeNode> MeshTrees { get; set; } = new();
    public List<TRMesh> Meshes { get; set; } = new();

    public TRModel Clone()
    {
        return new()
        {
            Animations = new(Animations.Select(a => a.Clone())),
            MeshTrees = new(MeshTrees.Select(m => m.Clone())),
            Meshes = new(Meshes.Select(m => m.Clone())),
        };
    }

    object ICloneable.Clone()
        => Clone();
}
