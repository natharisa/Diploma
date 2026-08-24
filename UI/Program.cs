using System;
using System.Windows.Forms;

namespace UI
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            bool mostrarLogin = true;

            while (mostrarLogin)
            {
                using (Login login = new Login())
                {
                    if (login.ShowDialog() != DialogResult.OK)
                    {
                        break;
                    }
                }

                using (MainForm mainForm = new MainForm())
                {
                    System.Windows.Forms.Application.Run(mainForm);
                    mostrarLogin = mainForm.DialogResult == DialogResult.Retry;
                }
            }
        }
    }
}
