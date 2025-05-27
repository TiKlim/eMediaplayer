using System.Collections.Generic;
using System.IO;

namespace Mediaplayer2.Models;

public class Playlist
{
    public string Name { get; set; }
    public List<string> Tracks { get; set; } = new List<string>();
    public string FilePath { get; set; }

    public void Save()
    {
        File.WriteAllLines(FilePath, Tracks);
    }
}