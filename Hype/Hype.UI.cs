using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using static Hype.Helpers.Win32Helper;

namespace Hype
{
    /// <summary>
    /// Parte visual (UI) de la clase principal HypeForm.
    /// Contiene la configuración de estilos, colores, fuentes y disposición de los controles.
    /// </summary>
    public partial class HypeForm
    {
        // --- COLORES Y ESTILOS ---
        private readonly Color colorFondo = Color.FromArgb(32, 33, 36);
        private readonly Color colorLista = Color.FromArgb(40, 40, 40);
        private readonly Color colorTexto = Color.FromArgb(240, 240, 240);
        private readonly Color colorAcento = Color.FromArgb(100, 149, 237);
        private readonly Color colorGrisSuave = Color.FromArgb(160, 160, 160);

        private readonly Font fuenteModerna = new Font("Segoe UI", 9.5f, FontStyle.Regular);
        private readonly Font fuenteBold = new Font("Segoe UI", 10, FontStyle.Bold);

        /// <summary>
        /// Inicializa todo el diseño y estilos del formulario.
        /// </summary>
        private void InicializarDiseñoCompleto()
        {
            InicializarTreeView();
            AplicarEstiloModerno();
            UpdateLanguageUI();
        }

        private void InicializarTreeView()
        {
            treeApps = new TreeView
            {
                CheckBoxes = true,
                BackColor = colorLista,
                ForeColor = colorTexto,
                BorderStyle = BorderStyle.None,
                ShowLines = false,
                ShowPlusMinus = true,
                FullRowSelect = true,
                Sorted = true,
                Font = new Font("Segoe UI", 9f)
            };

            treeApps.AfterCheck += TreeApps_AfterCheck;
            Controls.Add(treeApps);
        }

        private void AplicarEstiloModerno()
        {
            BackColor = colorFondo;
            Padding = new Padding(1);

            // Etiquetas
            lblHotkeyLabel.ForeColor = colorAcento;
            lblHotkeyLabel.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            lblHotkey.ForeColor = colorTexto;
            lblHotkey.Font = fuenteBold;

            lblToggleMode.ForeColor = colorAcento;
            lblToggleMode.Font = fuenteBold;

            lblClose.ForeColor = colorGrisSuave;
            lblMinimize.ForeColor = colorGrisSuave;

            // TreeView
            int listY = 135;
            treeApps.Location = new Point(8, listY);
            treeApps.Width = Width - 16;
            treeApps.Height = Height - listY - 65;

            // Etiqueta inferior
            label1.ForeColor = colorAcento;
            label1.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            label1.Location = new Point(8, treeApps.Bottom + 5);
            label1.AutoSize = true;

            // ComboBox
            cbSenuelo.BackColor = colorLista;
            cbSenuelo.ForeColor = Color.White;
            cbSenuelo.FlatStyle = FlatStyle.Flat;
            cbSenuelo.Location = new Point(8, label1.Bottom + 2);
            cbSenuelo.Width = Width - 16;
            cbSenuelo.DropDownStyle = ComboBoxStyle.DropDownList;

            pbToggleMode.BackColor = Color.Transparent;
            pbNoob.Cursor = Cursors.Hand;

            // Orden visual
            pbNoob.BringToFront();
            lblClose.BringToFront();
            lblMinimize.BringToFront();

            // Menú contextual oscuro
            contextMenuStrip1.Renderer = new DarkMenuRenderer();

            var sep = new ToolStripSeparator();
            var languageItem = new ToolStripMenuItem("English / Español");
            languageItem.Click += (s, _) =>
            {
                useEnglish = !useEnglish;
                UpdateLanguageUI();
            };

            contextMenuStrip1.Items.Insert(contextMenuStrip1.Items.Count - 1, sep);
            contextMenuStrip1.Items.Insert(contextMenuStrip1.Items.Count - 1, languageItem);
        }

        private void UpdateLanguageUI()
        {
            if (useEnglish)
            {
                lblHotkeyLabel.Text = "Toggle with:";
                label1.Text = "Safe Zone (Decoy):";
                mostrarToolStripMenuItem.Text = "Show";
                mostrarTodoToolStripMenuItem.Text = "Show All";
                salirToolStripMenuItem1.Text = "Exit";
            }
            else
            {
                lblHotkeyLabel.Text = "Activar / Desactivar con:";
                label1.Text = "Zona Segura (Señuelo):";
                mostrarToolStripMenuItem.Text = "Mostrar";
                mostrarTodoToolStripMenuItem.Text = "Mostrar Todo";
                salirToolStripMenuItem1.Text = "Salir";
            }

            UpdateToggleModeLabel();

            if (cbSenuelo.Items.Count > 0)
                cbSenuelo.Items[0] = useEnglish ? "--- None ---" : "--- Ninguno ---";

            CentrarControles(lblHotkeyLabel, lblHotkey, 78);
            CentrarControles(lblToggleMode, pbToggleMode, 102);
        }

        private void CentrarControles(Control izquierdo, Control derecho, int y)
        {
            const int espacio = 5;
            int anchoTotal = izquierdo.Width + espacio + derecho.Width;
            int startX = (ClientSize.Width - anchoTotal) / 2;

            izquierdo.Location = new Point(startX, y);
            derecho.Location = new Point(startX + izquierdo.Width + espacio, y);
        }

        private void UpdateToggleModeLabel()
        {
            lblToggleMode.Text = useEnglish
                ? (modoInvertido ? "Hide All Except" : "Hide Selected")
                : (modoInvertido ? "Ocultar todo excepto" : "Ocultar lo seleccionado");

            CentrarControles(lblToggleMode, pbToggleMode, 102);
        }

        private void UpdateHotkeyButtonText()
        {
            string text = string.Empty;

            if ((currentModifier & MOD_CONTROL) > 0) text += "Ctrl + ";
            if ((currentModifier & MOD_ALT) > 0) text += "Alt + ";
            if ((currentModifier & MOD_SHIFT) > 0) text += "Shift + ";

            text += currentKeyEnum.ToString();
            lblHotkey.Text = text;
            lblHotkeyLabel.Visible = true;

            CentrarControles(lblHotkeyLabel, lblHotkey, 78);
        }

        private void UpdateSenueloCombo()
        {
            var titles = new System.Collections.Generic.HashSet<string>();

            foreach (TreeNode parent in treeApps.Nodes)
                foreach (TreeNode child in parent.Nodes)
                    titles.Add(child.Text);

            if (cbSenuelo.Items.Count == titles.Count + 1)
                return;

            string selected = cbSenuelo.SelectedItem as string;

            cbSenuelo.BeginUpdate();
            cbSenuelo.Items.Clear();
            cbSenuelo.Items.Add(useEnglish ? "--- None ---" : "--- Ninguno ---");

            foreach (string t in titles)
                cbSenuelo.Items.Add(t);

            if (selected != null && cbSenuelo.Items.Contains(selected))
                cbSenuelo.SelectedItem = selected;
            else
                cbSenuelo.SelectedIndex = 0;

            cbSenuelo.EndUpdate();
        }

        private void pbToggleMode_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using var arrowPen = new Pen(modoInvertido ? colorAcento : colorGrisSuave, 2f);

            int ox = 2, oy = 4;

            e.Graphics.DrawLine(arrowPen, ox, oy + 4, ox + 10, oy + 4);
            e.Graphics.DrawLine(arrowPen, ox + 10, oy + 4, ox + 7, oy + 1);
            e.Graphics.DrawLine(arrowPen, ox + 10, oy + 4, ox + 7, oy + 7);

            e.Graphics.DrawLine(arrowPen, ox + 14, oy + 12, ox + 4, oy + 12);
            e.Graphics.DrawLine(arrowPen, ox + 4, oy + 12, ox + 7, oy + 9);
            e.Graphics.DrawLine(arrowPen, ox + 4, oy + 12, ox + 7, oy + 15);
        }
    }

    // --- CLASES DE RENDERIZADO DEL MENÚ (VISUALES) ---
    public class DarkMenuRenderer : ToolStripProfessionalRenderer
    {
        public DarkMenuRenderer() : base(new DarkMenuColors()) { }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = Color.FromArgb(240, 240, 240);
            base.OnRenderItemText(e);
        }
    }

    public class DarkMenuColors : ProfessionalColorTable
    {
        private readonly Color fondo = Color.FromArgb(40, 40, 40);

        public override Color ToolStripDropDownBackground => fondo;
        public override Color MenuBorder => Color.FromArgb(32, 33, 36);
        public override Color MenuItemSelected => Color.FromArgb(100, 149, 237);
        public override Color MenuItemBorder => Color.FromArgb(100, 149, 237);
        public override Color MenuItemSelectedGradientBegin => Color.FromArgb(100, 149, 237);
        public override Color MenuItemSelectedGradientEnd => Color.FromArgb(100, 149, 237);
        public override Color ImageMarginGradientBegin => fondo;
        public override Color ImageMarginGradientMiddle => fondo;
        public override Color ImageMarginGradientEnd => fondo;
    }
}
