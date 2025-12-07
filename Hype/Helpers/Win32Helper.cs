using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Hype.Helpers
{
    /// <summary>
    /// Contiene todas las definiciones P/Invoke para interactuar con la API nativa de Windows (user32.dll, kernel32.dll).
    /// </summary>
    public static class Win32Helper
    {
        #region Constants

        // Window States
        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;
        public const int SW_RESTORE = 9;

        // Hotkey Modifiers
        public const int MOD_NONE = 0x0000;
        public const int MOD_ALT = 0x0001;
        public const int MOD_CONTROL = 0x0002;
        public const int MOD_SHIFT = 0x0004;

        // ToolHelp32
        public const uint TH32CS_SNAPPROCESS = 0x00000002;

        #endregion

        #region Structs

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESSENTRY32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExeFile;
        }

        #endregion

        #region Delegates

        public delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

        #endregion

        #region External Methods (Kernel32.dll)

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

        [DllImport("kernel32.dll")]
        public static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll")]
        public static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        // IMPORTANTE: Necesario para liberar la memoria del Snapshot
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        #endregion

        #region External Methods (User32.dll)

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, int lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        #endregion

        #region Helper Logic Methods

        /// <summary>
        /// Obtiene el ID del proceso padre de un proceso dado.
        /// Utiliza ToolHelp32Snapshot para recorrer el árbol de procesos.
        /// </summary>
        public static int GetParentProcessId(int pid)
        {
            int parentPid = 0;
            IntPtr hSnapshot = IntPtr.Zero;

            try
            {
                // Tomamos una "foto" de todos los procesos del sistema
                hSnapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);

                // Verificamos si la foto es válida (IntPtr.Zero o -1 indican error)
                if (hSnapshot == IntPtr.Zero || hSnapshot == (IntPtr)(-1)) return 0;

                PROCESSENTRY32 procEntry = new PROCESSENTRY32();
                procEntry.dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32));

                if (Process32First(hSnapshot, ref procEntry))
                {
                    do
                    {
                        if (procEntry.th32ProcessID == pid)
                        {
                            parentPid = (int)procEntry.th32ParentProcessID;
                            break;
                        }
                    } while (Process32Next(hSnapshot, ref procEntry));
                }
            }
            catch
            {
                // Manejo silencioso en caso de error de acceso
            }
            finally
            {
                // CRÍTICO: Siempre cerrar el handle para evitar fugas de memoria
                if (hSnapshot != IntPtr.Zero && hSnapshot != (IntPtr)(-1))
                {
                    CloseHandle(hSnapshot);
                }
            }

            return parentPid;
        }

        #endregion
    }
}