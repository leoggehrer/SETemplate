//@BaseCode

namespace SETemplate.CodeGenApp
{
    using System;
    /// <summary>
    /// Represents a progress bar that can display a busy progress indicator in the console.
    /// </summary>
    public partial class ProgressBar
    {
        #region properties
        /// <summary>
        /// Gets or sets a value indicating whether the RunBusyProgress is active or not.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the RunBusyProgress is active; otherwise, <c>false</c>.
        /// </value>
        private static bool RunBusyProgress { get; set; }
        /// <summary>
        /// Indicates whether printing is allowed when the application is busy.
        /// </summary>
        /// <value>
        /// true if printing is allowed when the application is busy; otherwise, false.
        /// </value>
        public static bool CanBusyPrint { get; set; } = true;
        /// <summary>
        /// Gets or sets the foreground color of the console.
        /// </summary>
        /// <value>
        /// The foreground color of the console.
        /// </value>
        public static ConsoleColor ForegroundColor { get; set; } = Console.ForegroundColor;
        #endregion properties

        /// <summary>
        /// Prints a busy progress indicator in the console.
        /// </summary>
        public static void Start()
        {
            static void Write(int cursorLeft, int cursorTop, string output)
            {
                var saveCursorTop = Console.CursorTop;
                var saveCursorLeft = Console.CursorLeft;
                var saveForeColor = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.SetCursorPosition(cursorLeft, cursorTop);
                Console.Write(output);
                Console.SetCursorPosition(saveCursorLeft, saveCursorTop);
                Console.ForegroundColor = saveForeColor;
            }
            if (RunBusyProgress == false)
            {
                var head = '>';
                var runSign = '=';
                var counter = 0;

                RunBusyProgress = true;
                Console.WriteLine();

                var (Left, Top) = Console.GetCursorPosition();

                Console.WriteLine();
                Console.WriteLine();
                Task.Factory.StartNew(async () =>
                {
                    while (RunBusyProgress)
                    {
                        if (CanBusyPrint)
                        {
                            if (Left > 60)
                            {
                                var timeInSec = counter / 5;

                                for (int i = 0; i <= Left; i++)
                                {
                                    Write(i, Top, " ");
                                }
                                Left = 0;
                            }
                            else
                            {
                                Write(Left++, Top, $"{runSign}{head}");
                            }

                            if (counter % 5 == 0)
                            {
                                Write(65, Top, $"{counter / 5,5} [sec]");
                            }
                        }
                        counter++;
                        await Task.Delay(200);
                    }
                });
            }
        }

        /// <summary>
        /// Stops the execution of the busy progress.
        /// </summary>
        public static void Stop()
        {
            RunBusyProgress = false;
        }
    }
}
