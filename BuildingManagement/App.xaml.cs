using System.Windows;
using System.Windows.Controls;

namespace Sims.BuildingManagement
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            new Window
            {
                Title = "BuildingManagement",
                Width = 480,
                Height = 240,
                Content = new TextBlock
                {
                    Text = "Skelet WPF projekta je spreman.",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
            }.Show();
        }
    }
}
