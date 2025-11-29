namespace RetroPaint
{
    partial class frmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            sScreen = new Screen();
            SuspendLayout();
            // 
            // sScreen
            // 
            sScreen.Dock = DockStyle.Fill;
            sScreen.Location = new Point(0, 0);
            sScreen.Name = "sScreen";
            sScreen.Size = new Size(800, 450);
            sScreen.TabIndex = 0;
            sScreen.MouseClick += sScreen_MouseClick;
            // 
            // Form1
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(sScreen);
            Name = "Form1";
            Text = "Form1";
            DragDrop += frmMain_DragDrop;
            DragEnter += frmMain_DragEnter;
            ResumeLayout(false);
        }

        #endregion

        private Screen sScreen;
    }
}
