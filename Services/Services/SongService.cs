using Repositories.Models;
using Repositories.Repositories;

namespace Services.Services
{
    public class SongService
    {
        private SongRepository _songRepository = new();

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

        public List<Song> GetSongsFromAccount(int accountId)
        {
            return _songRepository.GetSongsFromAccount(accountId);
        }

        public List<Song> SearchSongByNameOrArtist(string? name, string? artist)
        {
            name = name?.ToLower() ?? string.Empty;
            artist = artist?.ToLower() ?? string.Empty;

            if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(artist))
            {
                return _songRepository.GetAllSongs();
            }

            return _songRepository.GetAllSongs()
                                  .Where(x => x.Title.ToLower().Contains(name) && x.ArtistName.ToLower().Contains(artist))
                                  .ToList();
        }


        public void DeleteSong(Song x)
        {
            _songRepository.DeleteSong(x);
        }
    }
}
