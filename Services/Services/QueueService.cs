using Repositories.Models;

public class QueueService
{
    private Queue<Song> _songQueue;
    private List<Song> _originalQueue; // Lưu trữ hàng đợi ban đầu để shuffle
    private Song _currentSong;
    private bool _isPlaying;

    public QueueService()
    {
        _songQueue = new Queue<Song>();
        _originalQueue = new List<Song>();
    }

    public bool AddSongToQueue(Song song)
    {
        if (song != null)
        {
            _songQueue.Enqueue(song);
            _originalQueue.Add(song); // Cập nhật hàng đợi ban đầu
            return true;
        }
        return false;
    }

    public Song SkipSongInQueue()
    {
        if (_songQueue.Any())
        {
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
        var rng = new Random();
        var shuffledQueue = _originalQueue.OrderBy(x => rng.Next()).ToList();
        _songQueue = new Queue<Song>(shuffledQueue);
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
        _originalQueue.Remove(song);
        _songQueue = new Queue<Song>(_originalQueue);
    }

    public void MoveSongUpInQueue(Song song)
    {
        int index = _originalQueue.IndexOf(song);
        if (index > 0)
        {
            _originalQueue.RemoveAt(index);
            _originalQueue.Insert(index - 1, song);
            _songQueue = new Queue<Song>(_originalQueue);
        }
    }

    public void MoveSongDownInQueue(Song song)
    {
        int index = _originalQueue.IndexOf(song);
        if (index < _originalQueue.Count - 1)
        {
            _originalQueue.RemoveAt(index);
            _originalQueue.Insert(index + 1, song);
            _songQueue = new Queue<Song>(_originalQueue);
        }
    }
}
