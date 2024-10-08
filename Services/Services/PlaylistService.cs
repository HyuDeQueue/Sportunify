﻿using Repositories.Models;
using Repositories.Repositories;

namespace Services.Services
{
    public class PlaylistService
    {
        private readonly PlaylistRepository _playlistRepository;
        private readonly PlaylistSongRepository _inplaylistRepository;

        public PlaylistService(PlaylistRepository playlistRepository, PlaylistSongRepository inplaylistRepository)
        {
            _playlistRepository = playlistRepository;
            _inplaylistRepository = inplaylistRepository;
        }
        public PlaylistService()
        {
            _playlistRepository = new();
            _inplaylistRepository = new();
        }

        public List<PlaylistSong> GetAllPlaylistSongs(int playlistId)
        {
            return _inplaylistRepository.GetPlaylistSongs(playlistId);
        }

        public List<Playlist> GetPlayListByAccountId(int accountId)
        {
            return _playlistRepository.GetPlayListByAccountId(accountId);
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
            var inplaylist = new PlaylistSong
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
