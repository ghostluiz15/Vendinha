using Vendinha.WinForms.Data;
using Vendinha.WinForms.Forms;

namespace Vendinha.WinForms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        try
        {
            using (var db = new VendinhaDbContext())
            {
                db.Database.EnsureCreated();
            }

            Application.Run(new MainForm());
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Não foi possível iniciar a Vendinha.\n\nDetalhes: {ex.Message}",
                "Erro ao iniciar",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
