using Repositories.Models;
using System.Collections.Generic;
using System.Linq;

namespace Repositories.Repositories
{
    public class PlaylistRepository
    {
        private ListentogetherContext _context;


        public List<Playlist> GetAllPlaylists()
        {
            _context = new();
            return _context.Playlists.ToList();
        }

        public List<Playlist> GetPlayListByAccountId(int accountId)
        {
            _context = new();
            return _context.Playlists.Where(x => x.AccountId == accountId).ToList();
        }

        public Playlist GetPlaylistById(int playlistId)
        {
            _context = new();
            return _context.Playlists.SingleOrDefault(p => p.PlaylistId == playlistId);
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
            _context.Playlists.Update(playlist);
            _context.SaveChanges();
        }

        public void DeletePlaylist(int playlistId)
        {
            _context = new();
            var playlist = _context.Playlists.SingleOrDefault(p => p.PlaylistId == playlistId);
            if (playlist != null)
            {
                _context.Playlists.Remove(playlist);
                _context.SaveChanges();
            }
        }
    }
}
