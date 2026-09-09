using System.Windows.Input;

namespace Sims.BuildingManagement.WPF.ViewModels;

public class NavigationItem
{
    public NavigationItem(string title, ICommand openCommand)
    {
        Title = title;
        OpenCommand = openCommand;
    }

    public string Title { get; }

    public ICommand OpenCommand { get; }
}
