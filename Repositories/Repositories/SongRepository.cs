using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class SongRepository
    {
        ListentogetherContext _context;
        public void AddSong(Song song)
        {
            _context = new();
            _context.Songs.Add(song);
            _context.SaveChanges();
        }

        public void EditSong(Song song)
        {
            _context = new();
            var existingSong = _context.Songs.Find(song.SongId);
            if (existingSong != null)
            {
                _context.Entry(existingSong).CurrentValues.SetValues(song);
                _context.SaveChanges();
            }
        }

        public List<Song> SearchSongs(Func<Song, bool> predicate)
        {
            _context = new();
            return _context.Songs.Where(predicate).ToList();
        }

        public List<Song> GetAllSongs()
        {
            _context = new();
            return _context.Songs.ToList();
        }

        public Song GetSongById(int songId)
        {
            _context = new();
            return _context.Songs.Find(songId);
        }
    }
}
