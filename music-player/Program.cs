using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

            var userData = await user.MyFavorites();
            var albums = userData.Favorites.Albums.Data;
            var musicDir = "home folder here";
            var i = 1;
            var max = albums.Count;
            Console.WriteLine();
            albums.Reverse();
            foreach (var album in albums)
            {
                try
                {
                    var albumObj = await Album.Get(album.Id, user);
                    Console.Write($"Downloading {albumObj.ArtistName} - {albumObj.Title}");
                    await albumObj.DownloadToDir(
                        new DirectoryInfo($"{musicDir}/{albumObj.ArtistName} - {albumObj.Title}"),
                        Namers.ArtistTrackTitleNamer);
                    Console.Write($"\rDownloaded {albumObj.ArtistName} - {albumObj.Title}  -  {i++} out of {max}\n");
                }
                catch (Exception e) { }
            }
        }
    }
}