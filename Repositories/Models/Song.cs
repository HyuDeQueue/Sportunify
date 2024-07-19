using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class Song
{
    public int SongId { get; set; }

    public string Title { get; set; } = null!;

    public string ArtistName { get; set; } = null!;

    public byte[] SongMedia { get; set; } = null!;

    public int AccountId { get; set; }

    public int CategoryId { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();
}
