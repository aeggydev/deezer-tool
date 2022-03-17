using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using deezer_client;
using McMaster.Extensions.CommandLineUtils;
using music_player;

var app = new CommandLineApplication();
app.HelpOption();
app.Command("download", c =>
{
    c.HelpOption();
    c.Description = "Download a track or an album";
    c.Command("album", ca =>
    {
        ca.HelpOption();
        var album = ca.Argument("album", "Name / ID of the album to download");
        album.IsRequired(false, "You have to specify a name / ID");
    });
    c.Command("track", ct =>
    {
        ct.HelpOption();
        var track = ct.Argument("track", "Name / ID of the track to download");
        track.IsRequired(false, "You have to specify a name / ID");
    });
    c.Option("-o|--output <FILENAME>", "Specifies where to place the file", CommandOptionType.SingleOrNoValue);
    Util.NoCommandToHelp(c);
});
app.Command("search", c =>
{
    c.HelpOption();
    c.Description = "Search for something";
    c.Command("album", ca =>
    {
        ca.Description = "Search for an album";
        var album = ca.Argument("album", "Name of the album to search for");
        album.IsRequired(false, "You have to specify a name");
        ca.HelpOption();
    });
    c.Command("track", ct =>
    {
        ct.Description = "Search for an artist";
        var artist = ct.Argument("track", "Name of the track to search");
        artist.IsRequired(false, "You have to specify a name");
        ct.HelpOption();
    });
    Util.NoCommandToHelp(c);
});
Util.NoCommandToHelp(app);
app.Execute(args);
Environment.Exit(1);

var stopwatch = new Stopwatch();
stopwatch.Start();
var arl = Environment.GetEnvironmentVariable("arl");
if (arl is null)
{
    Console.WriteLine("arl not specified as environment variable");
    Environment.Exit(1);
}
var user = await Deezer.Login(arl);

var userData = await user.MyFavorites();
var albums = userData.Favorites.Albums.Data.Take(1);
var musicDir = "home folder here";
var i = 1;
var max = 1;
Console.WriteLine();
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
    catch (Exception e)
    {
    }
}