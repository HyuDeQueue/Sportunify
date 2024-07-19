using Repositories.Models;
using System.Collections.Generic;
using System.Linq;

namespace Repositories.Repositories
{
    public class PlaylistSongRepository
    {
        private ListentogetherContext _context;


        public void AddSongToPlaylist(PlaylistSong playlistSong)
        {
            _context = new();
            _context.PlaylistSongs.Add(playlistSong);
            _context.SaveChanges();
        }

        public void DeleteSongFromPlaylist(int playlistId, int songId)
        {
            _context = new();
            var playlistSong = _context.PlaylistSongs.SingleOrDefault(ps => ps.PlaylistId == playlistId && ps.SongId == songId);
            if (playlistSong != null)
            {
                _context.PlaylistSongs.Remove(playlistSong);
                _context.SaveChanges();
            }
        }
    }
}
