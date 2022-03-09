namespace Facilitadores.Util.ConsoleApps
{
    public class ConsoleMessages
    {
        private static ConsoleMessages? _instance;
        private static readonly object _locker = new object();

        public static ConsoleMessages Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_locker)
                    {
                        if (_instance == null)
                            _instance = new ConsoleMessages();
                    }
                }
                return _instance;
            }
        }
        public void ShowSuccessMessage(string? Text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(Text);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void ShowInfoMessage(string? Text)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(Text);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void ShowSystemMessage(string? Text)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(Text);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void ShowDangerMessage(string? Text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Text);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public ConsoleKeyInfo RequestKey(string? Text)
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write(Text);
            Console.ForegroundColor = ConsoleColor.White;
            var KeyPressed = Console.ReadKey();
            return KeyPressed;
        }
        public string RequestAnswer(string? Text)
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write(Text);
            Console.ForegroundColor = ConsoleColor.White;
            var KeyPressed = Console.ReadLine()?.Trim() ?? string.Empty;
            return KeyPressed;
        }
    }
}
