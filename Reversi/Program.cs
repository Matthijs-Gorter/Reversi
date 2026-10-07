using System;
using System.Windows.Forms;

internal class Program
{
    [STAThreadAttribute]
    public static void Main()
    {
        Application.Run(new Reversi());
    }
}
