using System;
using System.Drawing;
using System.Windows.Forms;

namespace Hype
{
    public partial class AboutForm : Form
    {
        #region Constructor

        public AboutForm(Image logoImage, bool isEnglish)
        {
            // Delegamos la construcción visual al archivo UI
            InitializeUI(logoImage, isEnglish);
        }

        #endregion

        #region Event Handlers

        private void CloseForm_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Si necesitas pintar algo personalizado sobre el formulario, hazlo aquí
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        #endregion
    }
}