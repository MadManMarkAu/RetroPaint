using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace RetroPaint;

/// <summary>
/// Class for representing a 256-color (8-bit) palettized screen buffer.
/// </summary>
public class ScreenBuffer
{
    /// <summary>
    /// Event fires whenever the buffer size, the palette, or a pixel value changes. Used to refresh changes on-screen.
    /// It is advisable to use this event to start an update timer rather than redrawing for every pixel.
    /// </summary>
    public event EventHandler? BufferChanged;

    #region Backing variables & properties
    private readonly PaletteEntry[] _palette; // Screen palette entries.
    private int _width;
    private int _height;
    private byte[,] _screenBuffer; // Coordinates in (y,x) order so row pixels are stored contiguously. Not necessary, but is a matter of pride.

    /// <summary>
    /// Gets the width of the screen buffer, in pixels.
    /// </summary>
    public int Width { get => _width; }

    /// <summary>
    /// Gets the height of the screen buffer, in pixels.
    /// </summary>
    public int Height { get => _height; }
    #endregion

    #region Constructors & resizing
    // Creates a new ScreenBuffer object of the given dimensions. Pixel values are initialized to 0.
    public ScreenBuffer(int width, int height)
    {
        _palette = PaletteEntry.GenerateDefaultpalette();
        _width = width;
        _height = height;
        _screenBuffer = new byte[height, width];
    }

    // Create a new ScreenBuffer from the given pixel buffer and palette data.
    public ScreenBuffer(PaletteEntry[] palette, byte[,] screenBuffer)
    {
        ArgumentNullException.ThrowIfNull(palette);
        if (palette.Length != 256) throw new ArgumentException("Palette array must be 256 entries", nameof(palette));
        ArgumentNullException.ThrowIfNull(screenBuffer);

        _palette = palette;
        _width = screenBuffer.GetLength(1);
        _height = screenBuffer.GetLength(0);
        _screenBuffer = screenBuffer;
    }

    /// <summary>
    /// Resizes the screen buffer, optionally preserving pixel data.
    /// </summary>
    /// <param name="newWidth">The new width for the screen buffer.</param>
    /// <param name="newHeight">The new height for the screen buffer.</param>
    /// <param name="preservePixels">When true, pixel values will be preserved where the old and new screen dimensions overlap.</param>
    public void ResizeBuffer(int newWidth, int newHeight, bool preservePixels)
    {
        byte[,] newScreenBuffer = new byte[newHeight, newWidth];
        int minWidth;
        int minHeight;

        // Copy pixels if requested.
        if (preservePixels)
        {
            minWidth = _width < newWidth ? _width : newWidth;
            minHeight = _height < newHeight ? _height : newHeight;

            for (int y = 0; y < minHeight; y++)
                Buffer.BlockCopy(_screenBuffer, y * _height, newScreenBuffer, y * newHeight, minWidth);
        }

        // Set new screen data.
        _width = newScreenBuffer.GetLength(1);
        _height = newScreenBuffer.GetLength(0);
        _screenBuffer = newScreenBuffer;

        OnBufferChanged();
    }
    #endregion

    #region Public functions
    /// <summary>
    /// Reads a single entry from the 256-entry screen palette.
    /// </summary>
    /// <param name="index">The palette index to read from.</param>
    /// <returns>A <see cref="Color"/> struct describing the RGB values of the palette entry.</returns>
    public Color Readpalette(byte index)
        => Color.FromArgb((int)_palette[index].Color);

    /// <summary>
    /// Writes a single entry to the 256-entry screen palette.
    /// </summary>
    /// <param name="index">the palette index to write to.</param>
    /// <param name="color">A <see cref="Color"/> struct describing the RGB values to set in the palette entry.</param>
    public void Setpalette(byte index, Color color)
    {
        _palette[index] = new(color);

        OnBufferChanged();
    }

    /// <summary>
    /// Sets a single pixel in the screen buffer. If pixel location is outside the screen buffer, this call does nothing.
    /// </summary>
    /// <param name="x">The X coordinate of the pixel to set.</param>
    /// <param name="y">The Y coordinate of the pixel to set.</param>
    /// <param name="color">The color index to set at the given pixel location.</param>
    public void SetPixel(int x, int y, byte color)
    {
        if (IsInsideScreen(x, y))
        {
            _screenBuffer[y, x] = color;

            OnBufferChanged();
        }
    }

    /// <summary>
    /// Retrieves the color index of a single pixel in the screen buffer. If pixel location is outside the screen buffer, this call returns color index 0.
    /// </summary>
    /// <param name="x">The X coordinate of the pixel to retrieve.</param>
    /// <param name="y">The Y coordinate of the pixel to retrieve.</param>
    /// <returns>A byte containing the color index stored at the given pixel coordinates.</returns>
    public byte GetPixel(int x, int y)
        => IsInsideScreen(x, y) ? _screenBuffer[y, x] : (byte)0;

    /// <summary>
    /// Tests the color index of a single pixel in the screen buffer. If the pixel color index matches the <paramref name="color"/> parameter, this function returns true.
    /// If the pixel color index does not match, or the pixel coordinates are outside of the screen area, this function returns false.
    /// </summary>
    /// <param name="x">The X coordinate of the pixel to test.</param>
    /// <param name="y">The Y coordinate of the pixel to test.</param>
    /// <param name="color">The pixel color index to test against.</param>
    /// <returns>True if pixel color index matches, otherwise false.</returns>
    public bool PixelEquals(int x, int y, byte color)
        => IsInsideScreen(x, y) && _screenBuffer[y, x] == color;

    /// <summary>
    /// Tests to make sure the given pixel coordinates exist within the screen buffer. Returns true if pixel coordinates are valid, otherwise false.
    /// </summary>
    /// <param name="x">The X coordinate to validate.</param>
    /// <param name="y">The Y coordinate to validate.</param>
    /// <returns>True if pixel coordinates are valid, otherwise false.</returns>
    public bool IsInsideScreen(int x, int y)
        => x >= 0 && y >= 0 && x < _width && y < _height;

    /// <summary>
    /// Clears the screen buffer, filling it with the given pixel color index.
    /// </summary>
    /// <param name="color">The pixel color index to fill the screen buffer with.</param>
    public void Clear(byte color)
    {
        for (int y = 0; y < _height; y++)
            for (int x = 0; x < _width; x++)
                _screenBuffer[y, x] = color;

        OnBufferChanged();
    }

    /// <summary>
    /// Flood-fills the screen buffer with a color, starting at the given coordinates. Only colors matching the pixel at the given initial coordinates will be filled.
    /// If initial pixel coorindates are outside the screen buffer, this call is ignored.
    /// </summary>
    /// <param name="x">The X coordinate to start the flood fill at.</param>
    /// <param name="y">The Y coordinate to start the flood fill at.</param>
    /// <param name="color">The color index to fill the region with.</param>
    /// <param name="stepFinishedCallback"></param>
    /// <remarks>See https://en.wikipedia.org/wiki/Flood_fill#Span_filling</remarks>
    public void Fill(int x, int y, byte color)
    {
        byte allowedColor;
        Stack<PaintEntry> stack;
        int x1;
        int x2;
        int dy;

        if (!IsInsideScreen(x, y))
            return; // Nothing to paint

        allowedColor = _screenBuffer[y, x];
        stack = new();

        stack.Push(new(x, x, y, 1));
        stack.Push(new(x, x, y - 1, -1));

        while (stack.Count > 0)
        {
            (x1, x2, y, dy) = stack.Pop();
            x = x1;

            if (PixelEquals(x, y, allowedColor))
            {
                while (PixelEquals(x - 1, y, allowedColor))
                    _screenBuffer[y, x--] = color;
                if (x < x1)
                    stack.Push(new(x, x1 - 1, y - dy, -dy));
            }

            while (x1 <= x2)
            {
                while (PixelEquals(x1, y, allowedColor))
                    _screenBuffer[y, x1++] = color;
                if (x1 > x)
                    stack.Push(new(x, x1 - 1, y + dy, dy));
                if (x1 - 1 > x2)
                    stack.Push(new(x2 + 1, x1 - 1, y - dy, -dy));
                x1++;
                while (x1 < x2 && !PixelEquals(x1, y, allowedColor))
                    x1++;
                x = x1;
            }

            OnBufferChanged();
        }
    }
    #endregion

    #region Import/Export
    /// <summary>
    /// Creates a new <see cref="Bitmap"/> object containing the screen pixels, expanded using the current palette data.
    /// </summary>
    public Bitmap CreateBitmap()
    {
        Bitmap output;
        BitmapData info;
        int[] row;

        // Create output bitmap.
        output = new(_width, _height);

        // Access bitmap memory.
        info = output.LockBits(new Rectangle(0, 0, _width, _height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

        // Do this in a try{} block so we can unlock on error.
        try
        {
            // Variable to hold one row of bitmap pixels.
            row = new int[info.Stride / 4];

            for (int y = 0; y < _height; y++)
            {
                // Resolve screen buffer color indexes into bitmap pixel colors.
                for (int x = 0; x < _width; x++)
                    row[x] = (int)_palette[_screenBuffer[y, x]].Color;

                // Copy row of new pixels into bitmap memory.
                Marshal.Copy(row, 0, info.Scan0 + y * info.Stride, info.Stride / 4);
            }
        }
        finally
        {
            output.UnlockBits(info);
        }

        return output;
    }

    /// <summary>
    /// Creates a new <see cref="ScreenBuffer"/> object using the standard VGA "mode 13" color palette, populating the screen pixels from the given bitmap.
    /// </summary>
    /// <param name="bitmap">The <see cref="Bitmap"/> object to get the screen dimensions and pixel colors from.</param>
    /// <returns></returns>
    /// <remarks>Uses a "nearest color" algorithm to resolve the pixel color index from the bitmap RGB values.</remarks>
    public static ScreenBuffer FromBitmap(Bitmap bitmap)
    {
        int width = bitmap.Width;
        int height = bitmap.Height;
        PaletteEntry[] palette = PaletteEntry.GenerateDefaultpalette();
        byte[,] pixels = new byte[height, width];
        BitmapData info;
        int[] row; // Wish this could be uint, but 32-bit int is fine if we cast it later (negatives should wrap appropriately).

        // Pull out the bitmap memery.
        info = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

        // Do this in a try{} block so we can unlock on error.
        try
        {
            // Variable to store one row of bitmap pixel data.
            row = new int[info.Stride / 4];

            for (int y = 0; y < height; y++)
            {
                // Copy one row of bitmap pixels out of bitmap memory.
                Marshal.Copy(info.Scan0 + y * info.Stride, row, 0, info.Stride / 4);

                // Convert bitmap pixels into color indexes.
                for (int x = 0; x < width; x++)
                    pixels[y, x] = (byte)PaletteEntry.GetNearestColorIndex((uint)row[x], palette);
            }
        }
        finally
        {
            bitmap.UnlockBits(info);
        }

        return new(palette, pixels);
    }
    #endregion

    private void OnBufferChanged()
        => BufferChanged?.Invoke(this, EventArgs.Empty);

    // Stack entry for the Fill() function
    private readonly struct PaintEntry(int x1, int x2, int y, int dy)
    {
        public readonly int X1 = x1;
        public readonly int X2 = x2;
        public readonly int Y = y;
        public readonly int DY = dy;

        public void Deconstruct(out int x1, out int x2, out int y, out int dy)
        {
            x1 = X1;
            x2 = X2;
            y = Y;
            dy = DY;
        }
    }
}
