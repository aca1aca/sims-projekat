using System;
using System.Windows.Input;

namespace Sims.BuildingManagement.WPF.ViewModels;

// Delegira izvrsavanje prosledjenim funkcijama, 
// pa View kroz binding poziva
// metode ViewModel-a

public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool> _canExecute;

    public RelayCommand(Action execute, Func<bool> canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

    public void Execute(object parameter) => _execute();
}
