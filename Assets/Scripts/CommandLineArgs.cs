using System.Linq;

public static class CommandLineArgs
{
    static string[] args = System.Environment.GetCommandLineArgs();

    public static bool Has( string arg )
    {
        return args.Any( a => a.ToLower() == arg.ToLower() );
    }
}

