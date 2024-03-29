﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using deezer_client;
using McMaster.Extensions.CommandLineUtils;
using music_player;

var config = new Configuration();
var app = new CommandLineApplication();

/*
if (config.Arl == "" || config.Arl is null)
{
    Console.WriteLine("Arl not specified. Please specify one with the 'set' subcommand");
    Environment.Exit(1);
}
*/
var user = await Deezer.Login(config.Arl!);

app.HelpOption();
app.Command("set", c =>
{
    c.HelpOption();
    c.Description = "Set a setting";
    c.Command("arl", ca =>
    {
        // TODO: Tell the user how to get it
        ca.Description = "Set your arl";
        var arlArg = ca.Argument("arl", "Your Deezer account's arl");
        arlArg.IsRequired(false, "You have to specify the arl");
        ca.OnExecute(() =>
        {
            if (arlArg.Value!.Length != 192)
            {
                // TODO: Actually check with the deezer servers
                Console.WriteLine("ERROR: You haven't entered an actual deezer arl");
                Environment.Exit(1);
            }
            config.Arl = arlArg.Value;
            config.Update();
        });
    });
    Util.NoCommandToHelp(c);
});
app.Command("download", c =>
{
    c.HelpOption();
    c.Description = "Download a track or an album";
    c.Command("album", ca =>
    {
        ca.HelpOption();
        var album = ca.Argument("album", "Name / ID of the album to download");
        album.IsRequired(false, "You have to specify a name / ID");
        ca.OnExecute(() =>
        {
            var task = new Interaction(user).DownloadAlbum(album.Value!);
            task.GetAwaiter().GetResult();
        });
    });
    c.Command("track", ct =>
    {
        ct.HelpOption();
        var track = ct.Argument("track", "ID of the track to download");
        track.IsRequired(false, "You have to specify an ID");
        ct.OnExecute(() =>
        {
            var task = new Interaction(user).DownloadTrack(track.Value!);
            task.GetAwaiter().GetResult();
        });
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
        ca.OnExecute(() =>
        {
            var task = new Interaction(user).SearchAlbums(album.Value!);
            task.GetAwaiter().GetResult();
        });
    });
    c.Command("track", ct =>
    {
        ct.Description = "Search for an artist";
        var track = ct.Argument("track", "Name of the track to search");
        track.IsRequired(false, "You have to specify a name");
        ct.HelpOption();
        ct.OnExecute(() =>
        {
            var task = new Interaction(user).SearchTracks(track.Value!);
            task.GetAwaiter().GetResult();
        });
    });
    Util.NoCommandToHelp(c);
});
Util.NoCommandToHelp(app);
app.Execute(args);