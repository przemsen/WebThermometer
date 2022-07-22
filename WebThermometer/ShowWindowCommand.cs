using System;
using System.Windows.Input;

namespace WebThermometer;

class ShowWindowCommand : ICommand
{
    public bool CanExecute(object parameter)
    {
        return true;
    }

    public event EventHandler CanExecuteChanged;

    public void Execute(object parameter)
    {
        ((MainWindow)parameter).ShowWindow();
    }
}
