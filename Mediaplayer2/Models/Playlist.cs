using System.Collections.Generic;
using System.IO;

namespace Mediaplayer2.Models;

public class Playlist
{
    // Copyright (c) 2026 Timofeev Klim (eMediaplayer)
    // This program is free software: you can redistribute it and/or modify
    // it under the terms of the GNU General Public License as published by
    // the Free Software Foundation, either version 3 of the License, or
    // (at your option) any later version.
    
    public string Name { get; set; }
    public List<string> Tracks { get; set; } = new List<string>();
    
    // Путь к папке плейлиста, а не к файлу
    public string FolderPath { get; set; }

    // Метод сохранения метаданных плейлиста (например, список треков)
    public void Save()
    {
        string playlistFile = Path.Combine(FolderPath, "playlist.txt");
        File.WriteAllLines(playlistFile, Tracks);
    }

    // Метод добавления трека с копированием файла в папку плейлиста
    public void AddTrack(string sourceFilePath)
    {
        if (!Directory.Exists(FolderPath))
            Directory.CreateDirectory(FolderPath);

        string fileName = Path.GetFileName(sourceFilePath);
        string destFilePath = Path.Combine(FolderPath, fileName);

        // Копируем файл (перезаписываем, если уже есть)
        File.Copy(sourceFilePath, destFilePath, true);

        // Добавляем путь к треку
        Tracks.Add(destFilePath); // Сохраняем полный путь к файлу

        // Сохраняем обновленный список треков
        Save();
    }
}