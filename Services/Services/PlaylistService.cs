using Repositories.Models;
using Repositories.Repositories;
using System;
using System.Collections.Generic;

namespace Services.Services
{
    public class PlaylistService
    {
        private readonly PlaylistRepository _playlistRepository;
        private readonly InplaylistRepository _inplaylistRepository;

        public PlaylistService(PlaylistRepository playlistRepository, InplaylistRepository inplaylistRepository)
        {
            _playlistRepository = playlistRepository;
            _inplaylistRepository = inplaylistRepository;
        }

        public List<Playlist> GetAllPlaylists()
        {
            return _playlistRepository.GetAllPlaylists();
        }

        public Playlist GetSelectedPlaylist(int playlistId)
        {
            return _playlistRepository.GetPlaylistById(playlistId);
        }

        public void AddPlaylist(Playlist playlist)
        {
            _playlistRepository.AddPlaylist(playlist);
        }

        public void EditPlaylist(Playlist playlist)
        {
            _playlistRepository.EditPlaylist(playlist);
        }

        public void DeletePlaylist(int playlistId)
        {
            _playlistRepository.DeletePlaylist(playlistId);
        }

        public void AddSongToPlaylist(int playlistId, int songId, int position)
        {
            var inplaylist = new Inplaylist
            {
                PlaylistId = playlistId,
                SongId = songId,
                Position = position
            };
            _inplaylistRepository.AddSongToPlaylist(inplaylist);
        }

        public void DeleteSongFromPlaylist(int playlistId, int songId)
        {
            _inplaylistRepository.DeleteSongFromPlaylist(playlistId, songId);
        }

        public List<Playlist> SearchPlaylists(Func<Playlist, bool> predicate)
        {
            return _playlistRepository.GetAllPlaylists().Where(predicate).ToList();
        }
    }
}
