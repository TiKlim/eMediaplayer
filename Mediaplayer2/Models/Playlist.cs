using System.Collections.Generic;
using System.IO;

namespace Mediaplayer2.Models;

public class Playlist
{
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

        // Добавляем путь к треку (можно хранить только имя файла)
        Tracks.Add(fileName);

        // Сохраняем обновленный список треков
        Save();
    }
}