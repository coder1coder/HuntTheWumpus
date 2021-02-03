namespace HuntTheWumpus
{
    internal class Log
    {
        private string _log = "";

        public void Add(string message) => _log = message + "\r\n" + _log;
        public void Clear() => _log = string.Empty;
        public override string ToString() => _log;
    }
}
