using System;
using System.Windows.Input;

namespace Mediaplayer2.Models;

public class RelayCommand : ICommand
{
    // Copyright (c) 2026 Timofeev Klim (eMediaplayer)
    // This program is free software: you can redistribute it and/or modify
    // it under the terms of the GNU General Public License as published by
    // the Free Software Foundation, either version 3 of the License, or
    // (at your option) any later version.
    
    private readonly Action _execute;
    private readonly Func<bool> _canExecute;
    public event EventHandler CanExecuteChanged;
    
    public RelayCommand(Action execute, Func<bool> canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }
    
    public bool CanExecute(object parameter) => _canExecute == null || _canExecute();
    
    public void Execute(object parameter) => _execute();
    
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}