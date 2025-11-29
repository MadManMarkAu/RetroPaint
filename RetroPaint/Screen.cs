using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace RetroPaint;

public class Screen : Control
{
    private BufferedGraphics? _backBuffer;
    private BufferedGraphicsContext? _context;
    private ScreenBuffer? _screenBuffer;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ScreenBuffer? ScreenBuffer
    {
        get => _screenBuffer;
        set
        {
            if (_screenBuffer != null)
            {
                // Unsubscribe from events.
                _screenBuffer.BufferChanged -= ScreenBuffer_BufferChanged;
            }

            _screenBuffer = value;

            if (_screenBuffer != null)
            {
                // Subscribe to events.
                _screenBuffer.BufferChanged += ScreenBuffer_BufferChanged;
            }

            RecreateBuffers();
            Invalidate();
        }
    }

    public Screen()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        DoubleBuffered = true;
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        RecreateBuffers();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        if (_backBuffer == null)
        {
            RecreateBuffers();

            if (_backBuffer == null)
                return;
        }

        DrawToBuffer(_backBuffer.Graphics);

        _backBuffer.Render(e.Graphics);
    }

    private void ScreenBuffer_BufferChanged(object? sender, EventArgs e)
    {
        // Temporary: force immediate redraw of the screen
        Invalidate();
        Refresh();
    }

    private void RecreateBuffers()
    {
        _backBuffer?.Dispose();
        _backBuffer = null;

        _context = BufferedGraphicsManager.Current;
        _backBuffer = _context.Allocate(CreateGraphics(), ClientRectangle);

        Invalidate();
    }

    private void DrawToBuffer(Graphics g)
    {
        g.Clear(BackColor);

        if (_screenBuffer != null)
        {
            using Bitmap screen = _screenBuffer.CreateBitmap();

            g.DrawImageUnscaled(screen, ClientRectangle);
        }
    }
}
