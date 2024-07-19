using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Services
{
    public class QueueService
    {
        private readonly Queue<Song> _songQueue = new Queue<Song>();
        private readonly Queue<Playlist> _playlistQueue = new Queue<Playlist>();
        private Song _currentSong;
        private bool _isPlaying;

        public void PlayQueue()
        {
            if (_songQueue.Any())
            {
                _isPlaying = true;
                _currentSong = _songQueue.Dequeue();
                // Add logic to play the current song
            }
        }

        public void PauseQueue()
        {
            if (_isPlaying)
            {
                _isPlaying = false;
                // Add logic to pause the current song
            }
        }

        public void ResumeQueue()
        {
            if (!_isPlaying && _currentSong != null)
            {
                _isPlaying = true;
                // Add logic to resume the current song
            }
        }

        public void ClearQueue()
        {
            _songQueue.Clear();
            _playlistQueue.Clear();
            _currentSong = null;
            _isPlaying = false;
        }

        public void ShuffleQueue()
        {
            var songs = _songQueue.ToList();
            var rng = new Random();
            _songQueue.Clear();
            while (songs.Any())
            {
                var index = rng.Next(songs.Count);
                _songQueue.Enqueue(songs[index]);
                songs.RemoveAt(index);
            }
        }

        public bool SkipSongInQueue()
        {
            if (_songQueue.Any())
            {
                _currentSong = _songQueue.Dequeue();
                // Add logic to skip to the next song
                return true;
            }
            return false;
        }

        public void AddSongToQueue(Song song)
        {
            _songQueue.Enqueue(song);
        }

        public void AddPlaylistToQueue(Playlist playlist)
        {
            // Assuming you want to add all songs from the playlist to the queue
            foreach (var inplaylist in playlist.Inplaylists.OrderBy(ip => ip.Position))
            {
                _songQueue.Enqueue(inplaylist.Song);
            }
        }

        public bool RemoveSongFromQueue(Song song)
        {
            var list = _songQueue.ToList();
            if (list.Remove(song))
            {
                _songQueue.Clear();
                foreach (var s in list)
                {
                    _songQueue.Enqueue(s);
                }
                return true;
            }
            return false;
        }

        public void ReplaceCurrentQueueWithPlaylist(Playlist playlist)
        {
            _songQueue.Clear();
            AddPlaylistToQueue(playlist);
        }

        public void ReplaceCurrentQueueWithSong(Song song)
        {
            _songQueue.Clear();
            _songQueue.Enqueue(song);
        }

        // Optional method to get the current queue as a list (for testing or UI purposes)
        public List<Song> GetCurrentQueue()
        {
            return _songQueue.ToList();
        }
    }
}
