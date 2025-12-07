using NAudio.CoreAudioApi;
using System;

namespace Hype.Helpers
{
    /// <summary>
    /// Gestiona las sesiones de audio del sistema utilizando NAudio.
    /// Permite silenciar procesos específicos de manera segura.
    /// </summary>
    public class AudioManager : IDisposable
    {
        #region Fields

        private MMDeviceEnumerator _deviceEnumerator;
        private bool _disposed = false;

        #endregion

        #region Constructor

        public AudioManager()
        {
            InitializeAudioDevice();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Intenta silenciar o reactivar el audio de un proceso específico.
        /// Busca coincidencias por PID exacto o por relación Padre-Hijo.
        /// </summary>
        /// <param name="targetPid">ID del proceso objetivo.</param>
        /// <param name="mute">True para silenciar, False para activar sonido.</param>
        /// <param name="safeToMute">Si es True, permite silenciar. Si es False, ignora la orden de silenciar (seguridad).</param>
        public void SetProcessMute(int targetPid, bool mute, bool safeToMute = true)
        {
            // Cláusula de guarda: Si intentamos silenciar pero no es seguro, abortamos.
            if (mute && !safeToMute) return;

            // Si el enumerador no se inicializó correctamente (ej. sin drivers de audio), abortamos.
            if (_deviceEnumerator == null) return;

            try
            {
                var device = _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                var sessions = device.AudioSessionManager.Sessions;

                // Recorremos todas las sesiones de audio activas
                for (int i = 0; i < sessions.Count; i++)
                {
                    var session = sessions[i];
                    int sessionPid = (int)session.GetProcessID;

                    // Lógica de coincidencia
                    if (IsMatchingProcess(sessionPid, targetPid))
                    {
                        session.SimpleAudioVolume.Mute = mute;
                    }
                }
            }
            catch
            {
                // Silencioso ante errores COM o de concurrencia.
                // Es preferible que falle el mute a que crashee la app.
            }
        }

        #endregion

        #region Private Methods

        private void InitializeAudioDevice()
        {
            try
            {
                _deviceEnumerator = new MMDeviceEnumerator();
            }
            catch
            {
                // El sistema de audio puede no estar disponible o fallar al iniciar.
                _deviceEnumerator = null;
            }
        }

        /// <summary>
        /// Verifica si un PID coincide con el objetivo o es un proceso hijo del mismo.
        /// </summary>
        private bool IsMatchingProcess(int sessionPid, int targetPid)
        {
            if (sessionPid == targetPid) return true;

            // Si el PID no coincide directo, verificamos si es hijo (ej. Launchers de juegos)
            if (sessionPid > 0)
            {
                // Usamos el Helper estático para verificar la jerarquía
                if (Win32Helper.GetParentProcessId(sessionPid) == targetPid)
                {
                    return true;
                }
            }

            return false;
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
                    // Liberamos el recurso COM de NAudio
                    _deviceEnumerator?.Dispose();
                }
                _disposed = true;
            }
        }

        #endregion
    }
}