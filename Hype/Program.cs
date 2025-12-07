using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;

namespace Hype
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // --- PASO 1: ELIMINAR ZOMBIES ---
            // Antes de iniciar, buscamos si ya hay una instancia de Hype corriendo
            // y la cerramos a la fuerza para liberar la Hotkey y reiniciar todo.

            Process procesoActual = Process.GetCurrentProcess();
            // Buscamos procesos con el mismo nombre que el nuestro
            Process[] procesosExistentes = Process.GetProcessesByName(procesoActual.ProcessName);

            foreach (Process p in procesosExistentes)
            {
                // Si encontramos un proceso con nuestro nombre, pero que NO somos nosotros (ID diferente)
                if (p.Id != procesoActual.Id)
                {
                    try
                    {
                        // Lo matamos para liberar recursos y la tecla global
                        p.Kill();
                        p.WaitForExit(1000); // Esperamos hasta 1 segundo a que se muera
                    }
                    catch
                    {
                        // Si falla (por permisos), no podemos hacer mucho, seguimos adelante
                    }
                }
            }

            // --- PASO 2: INICIO NORMAL ---
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Iniciamos el formulario principal (HypeForm)
            Application.Run(new HypeForm());

        }
    }
}