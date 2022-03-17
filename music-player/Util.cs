using System;
using McMaster.Extensions.CommandLineUtils;

namespace music_player;

public class Util
{
    public static void NoCommandToHelp(CommandLineApplication c)
    {
        c.OnExecute(() =>
        {
            Console.WriteLine("Specify a subcommand");
            c.ShowHelp();
            return 1;
        });
    }
}