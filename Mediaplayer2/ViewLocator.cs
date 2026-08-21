using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Mediaplayer2.ViewModels;

namespace Mediaplayer2
{
    public class ViewLocator : IDataTemplate
    {
        // Copyright (c) 2026 Timofeev Klim (eMediaplayer)
        // This program is free software: you can redistribute it and/or modify
        // it under the terms of the GNU General Public License as published by
        // the Free Software Foundation, either version 3 of the License, or
        // (at your option) any later version.
        
        public Control Build(object? data)
        {
            var name = data!.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }

            return new TextBlock { Text = "Not Found: " + name };
        }

        public bool Match(object? data)
        {
            return data is ViewModelBase;
        }
    }
}