namespace Hype
{
    partial class HypeForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(HypeForm));

            notifyIcon1 = new NotifyIcon(components);
            contextMenuStrip1 = new ContextMenuStrip(components);
            mostrarToolStripMenuItem = new ToolStripMenuItem();
            mostrarTodoToolStripMenuItem = new ToolStripMenuItem();
            salirToolStripMenuItem1 = new ToolStripMenuItem();

            lblToggleMode = new Label();
            pbToggleMode = new PictureBox();
            timer1 = new System.Windows.Forms.Timer(components);

            lblClose = new Label();
            lblMinimize = new Label();

            lblHotkeyLabel = new Label();
            lblHotkey = new Label();

            pbNoob = new PictureBox();
            panel1 = new Panel();

            label1 = new Label();
            cbSenuelo = new ComboBox();

            contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbToggleMode).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbNoob).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();

            // 
            // notifyIcon1
            // 
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            notifyIcon1.Icon = (Icon)resources.GetObject("notifyIcon1.Icon");
            notifyIcon1.Text = "HYPE";
            notifyIcon1.Visible = true;
            notifyIcon1.MouseDoubleClick += notifyIcon1_MouseDoubleClick;

            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[]
            {
                mostrarToolStripMenuItem,
                mostrarTodoToolStripMenuItem,
                salirToolStripMenuItem1
            });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(146, 70);

            // 
            // mostrarToolStripMenuItem
            // 
            mostrarToolStripMenuItem.Name = "mostrarToolStripMenuItem";
            mostrarToolStripMenuItem.Size = new Size(145, 22);
            mostrarToolStripMenuItem.Text = "Mostrar";
            mostrarToolStripMenuItem.Click += mostrarToolStripMenuItem_Click;

            // 
            // mostrarTodoToolStripMenuItem
            // 
            mostrarTodoToolStripMenuItem.Name = "mostrarTodoToolStripMenuItem";
            mostrarTodoToolStripMenuItem.Size = new Size(145, 22);
            mostrarTodoToolStripMenuItem.Text = "Mostrar Todo";
            mostrarTodoToolStripMenuItem.Click += mostrarTodoToolStripMenuItem_Click;

            // 
            // salirToolStripMenuItem1
            // 
            salirToolStripMenuItem1.Name = "salirToolStripMenuItem1";
            salirToolStripMenuItem1.Size = new Size(145, 22);
            salirToolStripMenuItem1.Text = "Salir";
            salirToolStripMenuItem1.Click += salirToolStripMenuItem_Click;

            // 
            // lblToggleMode
            // 
            lblToggleMode.AutoSize = true;
            lblToggleMode.Cursor = Cursors.Hand;
            lblToggleMode.Font = new Font("Sans Serif Collection", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            lblToggleMode.ForeColor = Color.RoyalBlue;
            lblToggleMode.Location = new Point(71, 140);
            lblToggleMode.Name = "lblToggleMode";
            lblToggleMode.Size = new Size(125, 37);
            lblToggleMode.TabIndex = 4;
            lblToggleMode.Text = "Ocultar Todo Excepto";
            lblToggleMode.Click += ToggleMode_Click;

            // 
            // pbToggleMode
            // 
            pbToggleMode.BorderStyle = BorderStyle.FixedSingle;
            pbToggleMode.Cursor = Cursors.Hand;
            pbToggleMode.Location = new Point(219, 153);
            pbToggleMode.Name = "pbToggleMode";
            pbToggleMode.Size = new Size(24, 24);
            pbToggleMode.TabIndex = 5;
            pbToggleMode.TabStop = false;
            pbToggleMode.Click += pbToggleMode_Click;

            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;

            // 
            // lblClose
            // 
            lblClose.AutoSize = true;
            lblClose.Cursor = Cursors.Hand;
            lblClose.Font = new Font("Arial Black", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            lblClose.Location = new Point(316, 0);
            lblClose.Name = "lblClose";
            lblClose.Size = new Size(18, 18);
            lblClose.TabIndex = 7;
            lblClose.Text = "X";
            lblClose.TextAlign = ContentAlignment.MiddleLeft;
            lblClose.Click += lblClose_Click;

            // 
            // lblMinimize
            // 
            lblMinimize.AutoSize = true;
            lblMinimize.BackColor = Color.Transparent;
            lblMinimize.Cursor = Cursors.Hand;
            lblMinimize.Font = new Font("Arial Black", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            lblMinimize.ForeColor = Color.Orange;
            lblMinimize.Location = new Point(295, 0);
            lblMinimize.Name = "lblMinimize";
            lblMinimize.Size = new Size(15, 18);
            lblMinimize.TabIndex = 8;
            lblMinimize.Text = "_";
            lblMinimize.Click += lblMinimize_Click;

            // 
            // lblHotkeyLabel
            // 
            lblHotkeyLabel.AutoSize = true;
            lblHotkeyLabel.Location = new Point(71, 116);
            lblHotkeyLabel.Name = "lblHotkeyLabel";
            lblHotkeyLabel.Size = new Size(135, 15);
            lblHotkeyLabel.TabIndex = 9;
            lblHotkeyLabel.Text = "Activar / Desactivar con:";

            // 
            // lblHotkey
            // 
            lblHotkey.AutoSize = true;
            lblHotkey.BorderStyle = BorderStyle.Fixed3D;
            lblHotkey.Cursor = Cursors.Hand;
            lblHotkey.Location = new Point(222, 114);
            lblHotkey.Name = "lblHotkey";
            lblHotkey.Size = new Size(21, 17);
            lblHotkey.TabIndex = 10;
            lblHotkey.Text = "F8";
            lblHotkey.Click += lblHotkey_Click;

            // 
            // pbNoob
            // 
            pbNoob.Image = (Image)resources.GetObject("pbNoob.Image");
            pbNoob.Location = new Point(4, 8);
            pbNoob.Name = "pbNoob";
            pbNoob.Size = new Size(194, 52);
            pbNoob.SizeMode = PictureBoxSizeMode.StretchImage;
            pbNoob.TabIndex = 11;
            pbNoob.TabStop = false;

            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(32, 33, 36);
            panel1.Controls.Add(pbNoob);
            panel1.Controls.Add(lblClose);
            panel1.Controls.Add(lblMinimize);
            panel1.Location = new Point(8, 4);
            panel1.Margin = new Padding(6);
            panel1.Name = "panel1";
            panel1.Size = new Size(334, 72);
            panel1.TabIndex = 12;
            panel1.MouseDown += Form1_MouseDown;
            panel1.MouseMove += Form1_MouseMove;
            panel1.MouseUp += Form1_MouseUp;

            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 378);
            label1.Name = "label1";
            label1.Size = new Size(38, 15);
            label1.TabIndex = 13;
            label1.Text = "label1";

            // 
            // cbSenuelo
            // 
            cbSenuelo.BackColor = Color.FromArgb(40, 40, 40);
            cbSenuelo.DropDownStyle = ComboBoxStyle.DropDownList;
            cbSenuelo.ForeColor = Color.White;
            cbSenuelo.FormattingEnabled = true;
            cbSenuelo.Location = new Point(12, 396);
            cbSenuelo.Name = "cbSenuelo";
            cbSenuelo.Size = new Size(324, 23);
            cbSenuelo.TabIndex = 14;

            // 
            // HypeForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.GhostWhite;
            ClientSize = new Size(348, 431);
            Controls.Add(cbSenuelo);
            Controls.Add(label1);
            Controls.Add(panel1);
            Controls.Add(lblHotkey);
            Controls.Add(lblHotkeyLabel);
            Controls.Add(pbToggleMode);
            Controls.Add(lblToggleMode);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            Name = "HypeForm";
            Text = "HYPE";
            TopMost = true;

            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            KeyDown += Form1_KeyDown;
            MouseDown += Form1_MouseDown;
            MouseMove += Form1_MouseMove;
            MouseUp += Form1_MouseUp;
            Resize += Form1_Resize;

            contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pbToggleMode).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbNoob).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private NotifyIcon notifyIcon1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem mostrarToolStripMenuItem;
        private ToolStripMenuItem mostrarTodoToolStripMenuItem;
        private ToolStripMenuItem salirToolStripMenuItem1;

        private Label lblToggleMode;
        private PictureBox pbToggleMode;
        private System.Windows.Forms.Timer timer1;

        private Label lblClose;
        private Label lblMinimize;
        private Label lblHotkeyLabel;
        private Label lblHotkey;
        private PictureBox pbNoob;
        private Panel panel1;
        private Label label1;
        private ComboBox cbSenuelo;
    }
}
