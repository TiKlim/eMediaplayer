using System.Collections.Generic;

namespace Mediaplayer2.Models;

public class Playlist
{
    public string Name { get; set; }
    public List<string> Tracks { get; set; } = new List<string>();
}