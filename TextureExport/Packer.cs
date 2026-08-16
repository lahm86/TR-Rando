using System.Drawing;
using TRImageControl;
using TRLevelControl.Model;

namespace TextureExport;

public class Packer
{
    private const int ATLAS_SIZE = 256;
    private const int PADDING = 1;

    private sealed class PackTexture
    {
        public int Index { get; init; }
        public TRObjectTexture ObjectTexture { get; init; }
        public TRImage Image { get; init; }
    }

    private sealed class PackedAtlas
    {
        public uint[] Pixels { get; } =
            new uint[ATLAS_SIZE * ATLAS_SIZE];

        private readonly List<Rectangle> _freeRectangles =
        [
            new Rectangle(0, 0, ATLAS_SIZE, ATLAS_SIZE)
        ];

        public bool TryPlace(
            TRImage image,
            out Rectangle packedBounds)
        {
            int width = image.Size.Width + PADDING * 2;
            int height = image.Size.Height + PADDING * 2;

            packedBounds = default;

            // Find the smallest free rectangle that can contain
            // the padded image.
            int bestIndex = -1;
            int bestArea = int.MaxValue;

            for (int i = 0; i < _freeRectangles.Count; i++)
            {
                Rectangle free = _freeRectangles[i];

                if (free.Width < width ||
                    free.Height < height)
                {
                    continue;
                }

                int area = free.Width * free.Height;

                if (area < bestArea)
                {
                    bestArea = area;
                    bestIndex = i;
                }
            }

            if (bestIndex < 0)
                return false;

            Rectangle freeRectangle =
                _freeRectangles[bestIndex];

            packedBounds = new Rectangle(
                freeRectangle.X,
                freeRectangle.Y,
                width,
                height);

            // Copy the image, including its one-pixel replicated border.
            CopyImageWithPadding(
                image,
                Pixels,
                packedBounds);

            SplitFreeRectangle(
                freeRectangle,
                packedBounds);

            return true;
        }

        private void SplitFreeRectangle(
            Rectangle free,
            Rectangle used)
        {
            _freeRectangles.Remove(free);

            if (used.Contains(free))
                return;

            // Area above the used rectangle.
            if (used.Top > free.Top)
            {
                AddFreeRectangle(
                    new Rectangle(
                        free.X,
                        free.Y,
                        free.Width,
                        used.Top - free.Top));
            }

            // Area below the used rectangle.
            if (used.Bottom < free.Bottom)
            {
                AddFreeRectangle(
                    new Rectangle(
                        free.X,
                        used.Bottom,
                        free.Width,
                        free.Bottom - used.Bottom));
            }

            // Area to the left of the used rectangle.
            if (used.Left > free.Left)
            {
                AddFreeRectangle(
                    new Rectangle(
                        free.X,
                        used.Y,
                        used.Left - free.Left,
                        used.Height));
            }

            // Area to the right of the used rectangle.
            if (used.Right < free.Right)
            {
                AddFreeRectangle(
                    new Rectangle(
                        used.Right,
                        used.Y,
                        free.Right - used.Right,
                        used.Height));
            }

            RemoveContainedRectangles();
        }

        private void AddFreeRectangle(Rectangle rectangle)
        {
            if (rectangle.Width <= 0 ||
                rectangle.Height <= 0)
            {
                return;
            }

            _freeRectangles.Add(rectangle);
        }

        private void RemoveContainedRectangles()
        {
            for (int i = _freeRectangles.Count - 1; i >= 0; i--)
            {
                Rectangle a = _freeRectangles[i];

                for (int j = 0; j < _freeRectangles.Count; j++)
                {
                    if (i == j)
                        continue;

                    Rectangle b = _freeRectangles[j];

                    if (!b.Contains(a))
                        continue;

                    _freeRectangles.RemoveAt(i);
                    break;
                }
            }
        }
    }

    public static void RepackTextures(TR2Level level)
    {
        List<PackTexture> textures = level.ObjectTextures
            .Select((obj, index) => new PackTexture
            {
                Index = index,
                ObjectTexture = obj,
                Image = new TRImage(
                    level.Images16[obj.Atlas].Pixels)
                    .Export(obj.Bounds),
            })
            .ToList();

        // Largest first gives better utilisation.
        textures = textures
            .OrderByDescending(t =>
                t.Image.Size.Width * t.Image.Size.Height)
            .ToList();

        List<PackedAtlas> atlases = [];

        // Multiple ObjectTextures can refer to the same image.
        // We only physically pack each unique image once.
        Dictionary<string, (ushort Atlas, Rectangle Bounds)> imageLocations = [];

        foreach (PackTexture texture in textures)
        {
            string imageId = texture.Image.GenerateID();

            // This image has already been packed.
            if (imageLocations.TryGetValue(
                imageId,
                out var existing))
            {
                texture.ObjectTexture.Atlas =
                    existing.Atlas;

                // Existing.Bounds is the logical image bounds,
                // NOT the padded physical bounds.
                texture.ObjectTexture.Position =
                    existing.Bounds.Location;

                texture.ObjectTexture.Size =
                    existing.Bounds.Size;

                continue;
            }

            int packedWidth =
                texture.Image.Size.Width + PADDING * 2;

            int packedHeight =
                texture.Image.Size.Height + PADDING * 2;

            if (packedWidth > ATLAS_SIZE ||
                packedHeight > ATLAS_SIZE)
            {
                throw new InvalidOperationException(
                    $"Texture {texture.Index} is too large for an atlas " +
                    $"with {PADDING}px padding: {texture.Image.Size}");
            }

            PackedAtlas? atlas = null;
            int atlasIndex = -1;
            Rectangle packedBounds = default;

            // Try existing atlases.
            for (int i = 0; i < atlases.Count; i++)
            {
                if (!atlases[i].TryPlace(
                    texture.Image,
                    out packedBounds))
                {
                    continue;
                }

                atlas = atlases[i];
                atlasIndex = i;
                break;
            }

            // Create a new atlas if necessary.
            if (atlas == null)
            {
                atlas = new PackedAtlas();

                if (!atlas.TryPlace(
                    texture.Image,
                    out packedBounds))
                {
                    throw new InvalidOperationException(
                        $"Unable to pack texture {texture.Index}: " +
                        $"{texture.Image.Size}");
                }

                atlasIndex = atlases.Count;
                atlases.Add(atlas);
            }

            Rectangle logicalBounds = new(
                packedBounds.X + PADDING,
                packedBounds.Y + PADDING,
                texture.Image.Size.Width,
                texture.Image.Size.Height);

            ValidateBounds(
                texture.Index,
                (ushort)atlasIndex,
                packedBounds);

            ValidateBounds(
                texture.Index,
                (ushort)atlasIndex,
                logicalBounds);

            texture.ObjectTexture.Atlas =
                (ushort)atlasIndex;

            texture.ObjectTexture.Position =
                logicalBounds.Location;

            texture.ObjectTexture.Size =
                logicalBounds.Size;

            imageLocations.Add(
                imageId,
                ((ushort)atlasIndex, logicalBounds));
        }

        level.Images16 = [.. atlases.Select(atlas => new TRTexImage16 { Pixels = new TRImage(atlas.Pixels).ToRGB555() })];
        level.Images8 = [.. atlases.Select(atlas => new TRTexImage8 { Pixels = new TRImage(atlas.Pixels).ToRGB(level.Palette) })];
    }

    private static void CopyImageWithPadding(
        TRImage source,
        uint[] destination,
        Rectangle packedBounds)
    {
        int sourceWidth = source.Size.Width;
        int sourceHeight = source.Size.Height;

        int destinationWidth = ATLAS_SIZE;

        int destinationX = packedBounds.X;
        int destinationY = packedBounds.Y;

        //var newImg = new TRImage(rect.Image.Width + 2, rect.Image.Height + 2);
        //newImg.Import(rect.Image, new(1, 1));
        //newImg.Write((c, x, y) =>
        //{
        //    int srcX = Math.Clamp(x, 1, newImg.Width - 2);
        //    int srcY = Math.Clamp(y, 1, newImg.Height - 2);
        //    return newImg.GetPixel(srcX, srcY);
        //});

        for (int y = 0; y < packedBounds.Height; y++)
        {
            // Clamp source Y so that the first/last rows are replicated.
            int sourceY = Math.Clamp(
                y - PADDING,
                0,
                sourceHeight - 1);

            for (int x = 0; x < packedBounds.Width; x++)
            {
                // Clamp source X so that the first/last columns are replicated.
                int sourceX = Math.Clamp(
                    x - PADDING,
                    0,
                    sourceWidth - 1);

                uint pixel =
                    source.Pixels[
                        sourceY * sourceWidth + sourceX];

                destination[
                    (destinationY + y) * destinationWidth +
                    destinationX + x] = pixel;
            }
        }
    }

    private static void ValidateBounds(
        int textureIndex,
        ushort atlas,
        Rectangle bounds)
    {
        if (bounds.Width <= 0 ||
            bounds.Height <= 0 ||
            bounds.X < 0 ||
            bounds.Y < 0 ||
            bounds.Right > ATLAS_SIZE ||
            bounds.Bottom > ATLAS_SIZE)
        {
            throw new InvalidOperationException(
                $"Invalid packed bounds for texture {textureIndex}: " +
                $"Atlas={atlas}, Bounds={bounds}");
        }
    }
}
