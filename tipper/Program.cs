using System;
using Tipper.UI;

namespace Tipper
{
    class Program
    {
        private static void Main(string[] args)
        {
            if (args == null) throw new ArgumentNullException("args");
            UIMainLoop.LoadMainLoop();
        }
    }
}
