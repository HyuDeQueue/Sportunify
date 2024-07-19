using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class Song
{
    public int SongId { get; set; }

    public string Title { get; set; } = null!;

    public int Category { get; set; }

    public string ArtistName { get; set; } = null!;

    public byte[] SongMedia { get; set; } = null!;

    public int AccountId { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<Inplaylist> Inplaylists { get; set; } = new List<Inplaylist>();
}
