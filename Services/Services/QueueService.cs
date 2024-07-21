using Repositories.Models;
using System.Collections.Generic;
using System.Linq;

namespace Services.Services
{
    public class QueueService
    {
        private readonly Queue<Song> _songQueue = new();
        private Song _currentSong;
        private bool _isPlaying;

        public void PlayQueue()
        {
            if (_songQueue.Any())
            {
                _isPlaying = true;
                _currentSong = _songQueue.Dequeue();
                // Add logic to play the current song (Simulated in MainWindow)
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
            _currentSong = null;
            _isPlaying = false;
            return false;
        }

        public void AddSongToQueue(Song song)
        {
            _songQueue.Enqueue(song);
        }

        public void RemoveSongFromQueue(Song song)
        {
            var songs = _songQueue.ToList();
            if (songs.Remove(song))
            {
                _songQueue.Clear();
                foreach (var s in songs)
                {
                    _songQueue.Enqueue(s);
                }
            }
        }

        public void MoveSongUpInQueue(Song song)
        {
            var songs = _songQueue.ToList();
            var index = songs.IndexOf(song);
            if (index > 0)
            {
                var temp = songs[index - 1];
                songs[index - 1] = song;
                songs[index] = temp;
                _songQueue.Clear();
                foreach (var s in songs)
                {
                    _songQueue.Enqueue(s);
                }
            }
        }

        public void MoveSongDownInQueue(Song song)
        {
            var songs = _songQueue.ToList();
            var index = songs.IndexOf(song);
            if (index < songs.Count - 1)
            {
                var temp = songs[index + 1];
                songs[index + 1] = song;
                songs[index] = temp;
                _songQueue.Clear();
                foreach (var s in songs)
                {
                    _songQueue.Enqueue(s);
                }
            }
        }

        public bool IsPlaying => _isPlaying;

        public Song GetCurrentSong() => _currentSong;

        public List<Song> GetCurrentQueue() => _songQueue.ToList();
    }
}
