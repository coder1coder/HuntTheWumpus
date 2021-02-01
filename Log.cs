namespace HuntTheWumpus
{
    internal class Log
    {
        private string _log = "";

        public void Add(string message)
        {
            _log = message + "\r\n" + _log;
        }

        public override string ToString()
        {
            return _log;
        }
    }
}
