using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class InplaylistRepository
    {
        ListentogetherContext _context;

        public void AddSongToPlaylist(Inplaylist inplaylist)
        {
            _context = new();
            _context.Inplaylists.Add(inplaylist);
            _context.SaveChanges();
        }

        public void DeleteSongFromPlaylist(int playlistId, int songId)
        {
            _context = new();
            var inplaylist = _context.Inplaylists
                .FirstOrDefault(ip => ip.PlaylistId == playlistId && ip.SongId == songId);
            if (inplaylist != null)
            {
                _context.Inplaylists.Remove(inplaylist);
                _context.SaveChanges();
            }
        }
    }
}
