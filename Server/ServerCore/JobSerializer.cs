

public class JobSerializer
{
    private Queue<Action> _jobQueue = new Queue<Action>();
    private object _lock = new object();
    private bool _isProcessing = false;

    /// <summary>
    /// 큐에 작업 추가
    /// </summary>
    public void Enqueue(Action job)
    {
        // Lock Instance for Thread Safety
        lock (_lock)
        {
            _jobQueue.Enqueue(job);
            if (!_isProcessing)
            {
                // Start Processing Jobs
                _isProcessing = true;
                ProcessJobs();
            }
        }
    }

    /// <summary>
    /// 큐 작업 처리
    /// </summary>
    private void ProcessJobs()
    {
        // Threading Using Task
        Task.Run(() =>
        {
            while (true)
            {
                Action? job = null;
                lock (_lock)
                {
                    if (_jobQueue.Count > 0)
                    {
                        job = _jobQueue.Dequeue();
                    }
                    else
                    {
                        _isProcessing = false;
                        break;
                    }
                }

                job?.Invoke();
            }
        });
    }
}