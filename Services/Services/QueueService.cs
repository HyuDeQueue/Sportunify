using Repositories.Models;

public class QueueService
{
    private Queue<Song> _songQueue;
    private List<Song> _playedSongs;
    private Song _currentSong;
    private bool _isPlaying;

    public QueueService()
    {
        _songQueue = new Queue<Song>();
        _playedSongs = new List<Song>();
    }

    public bool AddSongToQueue(Song song)
    {
        if (song != null)
        {
            _songQueue.Enqueue(song);
            return true;
        }
        return false;
    }

    public Song SkipSongInQueue()
    {
        if (_songQueue.Any())
        {
            if (_currentSong != null)
            {
                _playedSongs.Add(_currentSong);
            }

            _currentSong = _songQueue.Dequeue();
            _isPlaying = true;
            return _currentSong;
        }
        _currentSong = null;
        _isPlaying = false;
        return null;
    }

    public Song GetCurrentSong()
    {
        return _currentSong;
    }

    public void ShuffleQueue()
    {
        if (_currentSong != null)
        {
            var rng = new Random();
            var unplayedSongs = _songQueue.ToList();
            unplayedSongs = unplayedSongs.OrderBy(x => rng.Next()).ToList();
            _songQueue = new Queue<Song>(unplayedSongs);
        }
        else
        {
            var rng = new Random();
            var allSongs = _songQueue.ToList();
            allSongs = allSongs.OrderBy(x => rng.Next()).ToList();
            _songQueue = new Queue<Song>(allSongs);
        }
    }


    public List<Song> GetCurrentQueue()
    {
        return _songQueue.ToList();
    }

    public bool IsPlaying
    {
        get { return _isPlaying; }
    }

    public void RemoveSongFromQueue(Song song)
    {
        var tempQueue = _songQueue.ToList();
        tempQueue.Remove(song);
        _songQueue = new Queue<Song>(tempQueue);
    }

    public void MoveSongUpInQueue(Song song)
    {
        var tempQueue = _songQueue.ToList();
        int index = tempQueue.IndexOf(song);
        if (index > 0)
        {
            tempQueue.RemoveAt(index);
            tempQueue.Insert(index - 1, song);
            _songQueue = new Queue<Song>(tempQueue);
        }
    }

    public void MoveSongDownInQueue(Song song)
    {
        var tempQueue = _songQueue.ToList();
        int index = tempQueue.IndexOf(song);
        if (index < tempQueue.Count - 1)
        {
            tempQueue.RemoveAt(index);
            tempQueue.Insert(index + 1, song);
            _songQueue = new Queue<Song>(tempQueue);
        }
    }
}
