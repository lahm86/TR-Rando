﻿namespace TRLevelControl.Model;

public class TR4Level : TRLevelBase
{
    public TR4Textiles Images { get; set; }
    public List<TR4Room> Rooms { get; set; }
    public TRDictionary<TR4Type, TRModel> Models { get; set; }
    public TRDictionary<TR4Type, TRStaticMesh> StaticMeshes { get; set; }
    public TRDictionary<TR4Type, TRSpriteSequence> Sprites { get; set; }
    public TRDictionary<byte, List<TRFlybyCamera>> Flybys { get; set; }
    public List<TRSoundSource<TR4SFX>> SoundSources { get; set; }
    public List<TR4Entity> Entities { get; set; }
    public List<TR4AIEntity> AIEntities { get; set; }
    public TRDictionary<TR4SFX, TR4SoundEffect> SoundEffects { get; set; }

    public override IEnumerable<TRMesh> DistinctMeshes => Models.Values.SelectMany(m => m.Meshes)
        .Concat(StaticMeshes.Values.Select(s => s.Mesh))
        .Distinct();
}
