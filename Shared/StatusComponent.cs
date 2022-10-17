namespace Lib.Shared
{
    public class StatusComponent
    {
        private StatusEnum _status = StatusEnum.Idle;

        public StatusComponent() { }

        public string GetStatus() => _status.ToString().ToLower();

        public StatusEnum GetStatusEnum() => _status;

        public StatusEnum Progress()
        {
            _status = _status switch
            {
                StatusEnum.Idle => StatusEnum.Online,
                StatusEnum.Online => StatusEnum.Finished,
                _ => _status
            };
            return _status;
        }
    }
}
