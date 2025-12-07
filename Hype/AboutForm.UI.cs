using System;
using System.Drawing;
using System.Windows.Forms;

namespace Hype
{
    public partial class AboutForm
    {
        #region Theme Constants

        private readonly Color _colorBackground = Color.FromArgb(32, 33, 36);
        private readonly Color _colorAccent = Color.FromArgb(100, 149, 237);
        private readonly Color _colorTextGray = Color.Gray;
        private readonly Color _colorTextLight = Color.FromArgb(220, 220, 220);

        #endregion

        #region UI Initialization

        private void InitializeUI(Image logoImage, bool isEnglish)
        {
            // --- Form Settings ---
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(400, 460);
            this.BackColor = _colorAccent; // Borde externo (simulado con padding)
            this.Padding = new Padding(1);

            // --- Main Panel ---
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = _colorBackground
            };
            this.Controls.Add(mainPanel);

            // --- Texts Localization ---
            string txtSubtitle = isEnglish ? "Hide Your Program: Enhanced" : "Hide Your Program: Enhanced";
            string txtDev = isEnglish
                ? "Developed by: David Platas\nContact: davarman10@gmail.com"
                : "Desarrollado por: David Platas\nContacto: davarman10@gmail.com";

            string txtGuide = isEnglish ?
                "⚡ QUICK GUIDE:\n\n" +
                "• [Hotkey]: Toggles Panic Mode (Hide/Show).\n\n" +
                "• List: Check the windows you want to control.\n\n" +
                "• Mode 'Hide Selected': Hides ONLY checked items.\n" +
                "• Mode 'Hide All Except': Hides EVERYTHING but checked.\n\n" +
                "• Safe Zone (Decoy): Choose an app (e.g. Excel) to\n" +
                "   automatically open/maximize when hiding games.\n\n" +
                "🔇 Hidden apps are automatically MUTED."
                :
                "⚡ GUÍA RÁPIDA:\n\n" +
                "• [Tecla]: Activa/Desactiva el pánico (Ocultar/Mostrar).\n\n" +
                "• Lista: Marca las ventanas que deseas controlar.\n\n" +
                "• Modo 'Ocultar lo seleccionado': Solo oculta lo que marques.\n" +
                "• Modo 'Ocultar Todo Excepto': Oculta todo MENOS lo marcado.\n\n" +
                "• Zona Segura (Señuelo): Elige una app (ej. Excel) para que se\n" +
                "   abra automáticamente al ocultar tus programas.\n\n" +
                "🔇 El audio de las apps ocultas se silencia automáticamente.";

            // --- UI Elements Creation ---

            // 1. Logo
            PictureBox pbLogo = new PictureBox
            {
                Size = new Size(250, 70),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = logoImage
            };
            // Centrado horizontal
            pbLogo.Location = new Point((this.Width - pbLogo.Width) / 2, 20);
            mainPanel.Controls.Add(pbLogo);

            // 2. Subtítulo
            Label lblSubtitle = new Label
            {
                Text = txtSubtitle,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = _colorAccent,
                AutoSize = true
            };
            mainPanel.Controls.Add(lblSubtitle);
            // Centrado después de añadirlo para calcular el ancho real
            lblSubtitle.Location = new Point((this.Width - lblSubtitle.Width) / 2, 95);

            // 3. Instrucciones
            Label lblInstrucciones = new Label
            {
                AutoSize = false,
                Size = new Size(360, 210),
                Location = new Point(20, 130),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = _colorTextLight,
                Text = txtGuide
            };
            mainPanel.Controls.Add(lblInstrucciones);

            // 4. Línea Separadora
            Label lblLine = new Label
            {
                BorderStyle = BorderStyle.Fixed3D,
                Size = new Size(340, 2),
                ForeColor = Color.DimGray
            };
            lblLine.Location = new Point((this.Width - 340) / 2, 350);
            mainPanel.Controls.Add(lblLine);

            // 5. Créditos Desarrollador
            Label lblDev = new Label
            {
                Text = txtDev,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                ForeColor = _colorTextGray,
                TextAlign = ContentAlignment.TopCenter,
                AutoSize = true
            };
            mainPanel.Controls.Add(lblDev);
            lblDev.Location = new Point((this.Width - lblDev.Width) / 2, 370);

            // 6. Versión
            Label lblVersion = new Label
            {
                Text = "v 2.1.0 (Pro)",
                Font = new Font("Consolas", 8, FontStyle.Regular),
                ForeColor = Color.FromArgb(60, 60, 60),
                AutoSize = true
            };
            mainPanel.Controls.Add(lblVersion);
            // Posición relativa a la esquina inferior derecha
            lblVersion.Location = new Point(this.Width - 90, this.Height - 25);

            // 7. Botón Cerrar (X)
            Label lblClose = new Label
            {
                Text = "X",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = _colorTextGray,
                Cursor = Cursors.Hand,
                AutoSize = true
            };
            lblClose.Location = new Point(this.Width - 30, 5);

            // Eventos visuales (Hover)
            lblClose.MouseEnter += (s, e) => lblClose.ForeColor = Color.White;
            lblClose.MouseLeave += (s, e) => lblClose.ForeColor = _colorTextGray;

            // Conexión lógica (Evento Click)
            lblClose.Click += CloseForm_Click;

            mainPanel.Controls.Add(lblClose);

            // Cerrar al hacer clic fuera (comodidad)
            mainPanel.Click += CloseForm_Click;
            lblInstrucciones.Click += CloseForm_Click;

            // Configuración final
            this.ShowInTaskbar = false;
        }

        #endregion
    }
}