namespace Lib.Shared
{
    public class Delay
    {
        private readonly DateTime _creationDate;
        private readonly TimeSpan _initialDelay;
        private bool _hasWaited = false;
        private bool HasWaited
        {
            get { return _hasWaited; }
            set
            {
                if (value)
                {
                    _hasWaited = true;
                }
            }
        }

        public Delay(TimeSpan initialDelay = default)
        {
            _initialDelay = initialDelay;
            _creationDate = DateTime.UtcNow;
        }

        public async ValueTask<bool> WaitOnce(CancellationToken cancellationToken)
        {
            if (!HasWaited && _creationDate + _initialDelay > DateTime.UtcNow)
            {
                return await WaitForInitialDelay(cancellationToken);
            }
            return false;

            async Task<bool> WaitForInitialDelay(CancellationToken cancellationToken)
            {
                var wait = _creationDate + _initialDelay - DateTime.UtcNow;
                if (wait.Ticks <= 0)
                {
                    return false;
                }
                await Task.Delay(wait, cancellationToken);
                HasWaited = true;
                return true;
            }

        }
    }
}
