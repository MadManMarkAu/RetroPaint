namespace RetroPaint;

/// <summary>
/// Structure for storing a single screen palette entry, consisting of RGB values.
/// </summary>
public readonly struct PaletteEntry
{
    public readonly byte R;
    public readonly byte G;
    public readonly byte B;
    public readonly uint Color;

    /// <summary>
    /// Creates a new <see cref="PaletteEntry"/> struct from the given RGB color values.
    /// </summary>
    /// <param name="r">The red component of this palette entry.</param>
    /// <param name="g">The green component of this palette entry.</param>
    /// <param name="b">The blue component of this palette entry.</param>
    public PaletteEntry(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
        Color = ((uint)r << 16) | ((uint)g << 8) | ((uint)b << 0) | 0xFF000000;
    }

    /// <summary>
    /// Creates a new <see cref="PaletteEntry"/> struct from the given color value, in the format 0xXXRRGGBB, where XX is ignored.
    /// </summary>
    /// <param name="color">A <see cref="uint"/> value describing the color for this new <see cref="PaletteEntry"/>. Alpha values are ignored.</param>
    public PaletteEntry(uint color)
    {
        R = (byte)((color & 0x00FF0000) >> 16);
        G = (byte)((color & 0x0000FF00) >> 8);
        B = (byte)((color & 0x000000FF) >> 0);
        Color = color | 0xFF000000;
    }

    /// <summary>
    /// Creates a new <see cref="PaletteEntry"/> struct from the given .NET <see cref="Color"/>.
    /// </summary>
    /// <param name="color">A <see cref="Color"/> struct describing the color for this new <see cref="PaletteEntry"/>. Alpha values are ignored.</param>
    public PaletteEntry(Color color)
    {
        uint col = (uint)color.ToArgb();

        R = (byte)((col & 0x00FF0000) >> 16);
        G = (byte)((col & 0x0000FF00) >> 8);
        B = (byte)((col & 0x000000FF) >> 0);
        Color = col | 0xFF000000;
    }

    /// <summary>
    /// Returns the squared distance between this color and the <see cref="PaletteEntry"/> color described by <paramref name="other"/> in the 3D color cube.
    /// </summary>
    /// <param name="other">A color palette entry to test against.</param>
    /// <returns>The squared distance, in pixel color values.</returns>
    public int DistanceSquared(PaletteEntry other)
        => ((R - other.R) * (R - other.R)) +
           ((G - other.G) * (G - other.G)) +
           ((B - other.B) * (B - other.B));

    /// <summary>
    /// Generates the default 8-bit VGA color palette from old video cards.
    /// </summary>
    /// <returns>A <see cref="PaletteEntry"/> array of 256 entries representing the old VGA "mode 13h" color palette.</returns>
    public static PaletteEntry[] GenerateDefaultpalette()
    {
        // Color list sourced from: https://gist.github.com/cesarmiquel/1780ab6078b9735371d1f10a9d60d233.
        uint[] colors = [0x000000, 0x0002aa, 0x14aa00, 0x00aaaa, 0xaa0003, 0xaa00aa, 0xaa5500, 0xaaaaaa, 0x555555, 0x5555ff, 0x55ff55, 0x55ffff, 0xff5555, 0xfd55ff, 0xffff55, 0xffffff,
                         0x000000, 0x101010, 0x202020, 0x353535, 0x454545, 0x555555, 0x656565, 0x757575, 0x8a8a8a, 0x9a9a9a, 0xaaaaaa, 0xbababa, 0xcacaca, 0xdfdfdf, 0xefefef, 0xffffff,
                         0x0004ff, 0x4104ff, 0x8203ff, 0xbe02ff, 0xfd00ff, 0xfe00be, 0xff0082, 0xff0041, 0xff0008, 0xff4105, 0xff8200, 0xffbe00, 0xffff00, 0xbeff00, 0x82ff00, 0x41ff01,
                         0x24ff00, 0x22ff42, 0x1dff82, 0x12ffbe, 0x00ffff, 0x00beff, 0x0182ff, 0x0041ff, 0x8282ff, 0x9e82ff, 0xbe82ff, 0xdf82ff, 0xfd82ff, 0xfe82df, 0xff82be, 0xff829e,
                         0xff8282, 0xff9e82, 0xffbe82, 0xffdf82, 0xffff82, 0xdfff82, 0xbeff82, 0x9eff82, 0x82ff82, 0x82ff9e, 0x82ffbe, 0x82ffdf, 0x82ffff, 0x82dfff, 0x82beff, 0x829eff,
                         0xbabaff, 0xcabaff, 0xdfbaff, 0xefbaff, 0xfebaff, 0xfebaef, 0xffbadf, 0xffbaca, 0xffbaba, 0xffcaba, 0xffdfba, 0xffefba, 0xffffba, 0xefffba, 0xdfffba, 0xcaffbb,
                         0xbaffba, 0xbaffca, 0xbaffdf, 0xbaffef, 0xbaffff, 0xbaefff, 0xbadfff, 0xbacaff, 0x010171, 0x1c0171, 0x390171, 0x550071, 0x710071, 0x710055, 0x710039, 0x71001c,
                         0x710001, 0x711c01, 0x713900, 0x715500, 0x717100, 0x557100, 0x397100, 0x1c7100, 0x097100, 0x09711c, 0x067139, 0x037155, 0x007171, 0x005571, 0x003971, 0x001c71,
                         0x393971, 0x453971, 0x553971, 0x613971, 0x713971, 0x713961, 0x713955, 0x713945, 0x713939, 0x714539, 0x715539, 0x716139, 0x717139, 0x617139, 0x557139, 0x45713a,
                         0x397139, 0x397145, 0x397155, 0x397161, 0x397171, 0x396171, 0x395571, 0x394572, 0x515171, 0x595171, 0x615171, 0x695171, 0x715171, 0x715169, 0x715161, 0x715159,
                         0x715151, 0x715951, 0x716151, 0x716951, 0x717151, 0x697151, 0x617151, 0x597151, 0x517151, 0x51715a, 0x517161, 0x517169, 0x517171, 0x516971, 0x516171, 0x515971,
                         0x000042, 0x110041, 0x200041, 0x310041, 0x410041, 0x410032, 0x410020, 0x410010, 0x410000, 0x411000, 0x412000, 0x413100, 0x414100, 0x314100, 0x204100, 0x104100,
                         0x034100, 0x034110, 0x024120, 0x014131, 0x004141, 0x003141, 0x002041, 0x001041, 0x202041, 0x282041, 0x312041, 0x392041, 0x412041, 0x412039, 0x412031, 0x412028,
                         0x412020, 0x412820, 0x413120, 0x413921, 0x414120, 0x394120, 0x314120, 0x284120, 0x204120, 0x204128, 0x204131, 0x204139, 0x204141, 0x203941, 0x203141, 0x202841,
                         0x2d2d41, 0x312d41, 0x352d41, 0x3d2d41, 0x412d41, 0x412d3d, 0x412d35, 0x412d31, 0x412d2d, 0x41312d, 0x41352d, 0x413d2d, 0x41412d, 0x3d412d, 0x35412d, 0x31412d,
                         0x2d412d, 0x2d4131, 0x2d4135, 0x2d413d, 0x2d4141, 0x2d3d41, 0x2d3541, 0x2d3141, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000, 0x000000,];

        return [.. colors.Select(x => new PaletteEntry(x))];
    }

    /// <summary>
    /// Given the color value and current palette, find the nearest color palette index for the passed color.
    /// Uses Pythagoras' Theorem (d^2 = r^2 + g^2 + b^2) to find the nearest color index in a color cube.
    /// </summary>
    /// <param name="color">The RGB color values to use when finding the color palette index, in the format 0xXXRRGGBB, with XX being ignored.</param>
    /// <param name="palette">A 256-entry array of <see cref="PaletteEntry"/> structs containing the color palette to search through.</param>
    /// <returns>The color palette index chosen that best represents the RGB color values passed.</returns>
    public static int GetNearestColorIndex(uint color, PaletteEntry[] palette)
    {
        PaletteEntry testColor = new(color);
        long distance;
        long closestDistance = long.MaxValue;
        int closestIndex = 0;

        // Test each color palette entry.
        for (int index = 0; index < palette.Length; index++)
        {
            distance = palette[index].DistanceSquared(testColor);

            // If this entry is closer, choose it.
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = index;
            }
        }

        return closestIndex;
    }
}
