using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class PlaylistRepository
    {
        ListentogetherContext _context;

        public List<Playlist> GetAllPlaylists()
        {
            _context = new();
            return _context.Playlists.ToList();
        }

        public Playlist GetPlaylistById(int playlistId)
        {
            _context = new();
            return _context.Playlists.Find(playlistId);
        }

        public void AddPlaylist(Playlist playlist)
        {
            _context = new();
            _context.Playlists.Add(playlist);
            _context.SaveChanges();
        }

        public void EditPlaylist(Playlist playlist)
        {
            _context = new();
            var existingPlaylist = _context.Playlists.Find(playlist.PlaylistId);
            if (existingPlaylist != null)
            {
                _context.Entry(existingPlaylist).CurrentValues.SetValues(playlist);
                _context.SaveChanges();
            }
        }

        public void DeletePlaylist(int playlistId)
        {
            _context = new();
            var playlist = _context.Playlists.Find(playlistId);
            if (playlist != null)
            {
                _context.Playlists.Remove(playlist);
                _context.SaveChanges();
            }
        }
    }
}
