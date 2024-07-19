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

        public String CategoryMeaning(int id)
        {
            switch (id)
            {
                case 1:
                    return "Nhạc buồn";
                case 2:
                    return "Nhạc sàn";
                case 3:
                    return "Nhạc KPOP";
                case 4:
                    return "Nhạc USUK";
                case 5:
                    return "Nhạc Việt";
                case 6:
                    return "Nhạc không lời";
                default:
                    return "Chưa phân loại";   
            }
        }
    }
}
