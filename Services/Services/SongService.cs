using Repositories.Models;
using Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class SongService
    {
        private readonly SongRepository _songRepository;

        public SongService(SongRepository songRepository)
        {
            _songRepository = songRepository;
        }

        public void AddSongs(Song song)
        {
            _songRepository.AddSong(song);
        }

        public void EditSongs(Song song)
        {
            _songRepository.EditSong(song);
        }

        public List<Song> SearchSongs(Func<Song, bool> predicate)
        {
            return _songRepository.SearchSongs(predicate);
        }

        public List<Song> GetAllSongs()
        {
            return _songRepository.GetAllSongs();
        }

        public Song GetSelectedSong(int songId)
        {
            return _songRepository.GetSongById(songId);
        }
    }
}
