using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Hype
{
    /// <summary>
    /// Crea una ventana nativa invisible (Message-Only Window) para interceptar mensajes WM_HOTKEY del sistema.
    /// Esto permite detectar combinaciones de teclas globales incluso si la aplicación no tiene el foco.
    /// </summary>
    public class HotkeyListener : NativeWindow, IDisposable
    {
        #region Constants

        private const int WM_HOTKEY = 0x0312;
        private const int HOTKEY_ID = 1; // ID arbitrario para identificar nuestro atajo

        #endregion

        #region Fields

        private readonly Action _onHotkeyPress;
        private bool _isRegistered = false;
        private bool _disposed = false;

        #endregion

        #region P/Invoke Definitions

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        #endregion

        #region Constructor

        /// <summary>
        /// Inicializa el escucha de teclas globales.
        /// </summary>
        /// <param name="onHotkeyPressCallback">Acción a ejecutar cuando se detecta la tecla.</param>
        public HotkeyListener(Action onHotkeyPressCallback)
        {
            _onHotkeyPress = onHotkeyPressCallback ?? throw new ArgumentNullException(nameof(onHotkeyPressCallback));

            // Creamos el Handle de la ventana nativa inmediatamente.
            // Usamos CreateParams vacío porque no necesitamos una ventana visible, solo el "buzón" de mensajes.
            this.CreateHandle(new CreateParams());
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Registra una combinación de teclas global.
        /// Si ya existía una registrada, la reemplaza.
        /// </summary>
        /// <param name="modifier">Teclas modificadoras (Alt, Ctrl, Shift).</param>
        /// <param name="key">Tecla virtual principal (ej. F8).</param>
        /// <returns>True si el registro fue exitoso, False si la tecla ya está ocupada por otro programa.</returns>
        public bool Register(int modifier, int key)
        {
            // Si ya tenemos una registrada, la limpiamos antes de poner la nueva
            if (_isRegistered)
            {
                Unregister();
            }

            // Intentamos registrar. Windows devuelve 'false' si otra app ya tomó esa tecla.
            _isRegistered = RegisterHotKey(this.Handle, HOTKEY_ID, modifier, key);

            return _isRegistered;
        }

        /// <summary>
        /// Elimina el registro del hotkey actual y deja de escuchar.
        /// </summary>
        public void Unregister()
        {
            if (_isRegistered)
            {
                UnregisterHotKey(this.Handle, HOTKEY_ID);
                _isRegistered = false;
            }
        }

        #endregion

        #region Native Window Logic

        /// <summary>
        /// Método del sistema que procesa los mensajes de Windows enviados a este Handle.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // Verificamos si el mensaje es de tipo HOTKEY y si coincide con nuestro ID
            if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID)
            {
                _onHotkeyPress?.Invoke();
            }
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Liberamos recursos administrados si los hubiera
                }

                // Liberación de recursos no administrados (CRÍTICO)
                Unregister();     // Le decimos a Windows que suelte la tecla
                this.DestroyHandle(); // Destruimos la ventana fantasma

                _disposed = true;
            }
        }

        #endregion
    }
}