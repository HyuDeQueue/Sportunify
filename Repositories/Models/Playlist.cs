using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class Playlist
{
    public int PlaylistId { get; set; }

    public string PlaylistName { get; set; } = null!;

    public string? Description { get; set; }

    public int AccountId { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<Inplaylist> Inplaylists { get; set; } = new List<Inplaylist>();
}
