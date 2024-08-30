using Microsoft.EntityFrameworkCore;
using Repositories.Models;

namespace Repositories.Repositories
{
    public class SongRepository
    {
        private ListentogetherContext _context;

        public List<Song> GetAllSongs()
        {
            _context = new();
            return _context.Songs.Include("Category").ToList();
        }

        public Song GetSongById(int songId)
        {
            _context = new();
            return _context.Songs.SingleOrDefault(s => s.SongId == songId);
        }

        public void AddSong(Song song)
        {
            _context = new();
            _context.Songs.Add(song);
            _context.SaveChanges();
        }

        public void EditSong(Song song)
        {
            _context = new();
            _context.Songs.Update(song);
            _context.SaveChanges();
        }

        public void DeleteSong(Song song)
        {
            _context = new();
            _context.Songs.Remove(song);
            _context.SaveChanges();
        }

        public List<Song> SearchSongs(Func<Song, bool> predicate)
        {
            _context = new();
            return _context.Songs.Where(predicate).ToList();
        }

        public List<Song> GetSongsFromAccount(int accountId)
        {
            _context = new();
            return _context.Songs.Include("Category").Where(s => s.AccountId == accountId).ToList();
        }
    }
}
