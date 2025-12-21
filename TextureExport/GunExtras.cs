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
}
