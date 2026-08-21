using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Mediaplayer2.Converters;

public class SecondTimeSpanToStringConverter : IValueConverter
{
    // Copyright (c) 2026 Timofeev Klim (eMediaplayer)
    // This program is free software: you can redistribute it and/or modify
    // it under the terms of the GNU General Public License as published by
    // the Free Software Foundation, either version 3 of the License, or
    // (at your option) any later version.
    
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TimeSpan timeSpan)
        {
            int totalHours = (int)timeSpan.TotalHours;
            return $"{totalHours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }
        return "00:00:00";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}