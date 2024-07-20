using Repositories.Models;

namespace Services.Services
{
    public class QueueService
    {
        private readonly Queue<Song> _songQueue = new Queue<Song>();
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

        public bool IsPlaying => _isPlaying;

        public Song GetCurrentSong() => _currentSong;

        // Optional method to get the current queue as a list (for testing or UI purposes)
        public List<Song> GetCurrentQueue()
        {
            return _songQueue.ToList();
        }
    }
}
