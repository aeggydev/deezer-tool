using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using deezer_client;

namespace music_player
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var arl = Environment.GetEnvironmentVariable("arl");
            var user = await Deezer.Login(arl);

            var t = await Track.Get("1090068082", user);
            var outputdir = Environment.GetEnvironmentVariable("music_dir");
            await t.DownloadToDir(new DirectoryInfo(outputdir), Namers.TrackTitleNamer);
        }
    }
}