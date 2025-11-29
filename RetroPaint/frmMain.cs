namespace RetroPaint;

public partial class frmMain : Form
{
    private ScreenBuffer? _screen;

    public frmMain()
    {
        InitializeComponent();
    }

    private void frmMain_DragEnter(object sender, DragEventArgs e)
    {
        e.Effect = (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop)) ? DragDropEffects.Copy : DragDropEffects.None;
    }

    private void frmMain_DragDrop(object sender, DragEventArgs e)
    {
        try
        {
            string? file = ((string[]?)e.Data?.GetData(DataFormats.FileDrop))?.FirstOrDefault();

            if (file != null && File.Exists(file))
            {
                using Bitmap bmp = new(file);
                _screen = ScreenBuffer.FromBitmap(bmp);

                sScreen.ScreenBuffer = _screen;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"There was an error performing this operation\r\n\r\n{ex.GetType().Name}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void sScreen_MouseClick(object sender, MouseEventArgs e)
    {
        try
        {
            Text = "Painting...";
            _screen?.Fill(e.X, e.Y, 14);
            Text = "Done";
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"There was an error performing this operation\r\n\r\n{ex.GetType().Name}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
