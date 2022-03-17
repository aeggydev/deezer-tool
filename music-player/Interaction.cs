using System;
using System.Threading.Tasks;
using deezer_client;

namespace music_player;

public class Interaction
{
    private Deezer User { get; }
    public Interaction(Deezer user)
    {
        User = user;
    }

    public async Task SearchAlbums(string query)
    {
        var albums = await Search.SearchAlbums(query, User);
        foreach (var album in albums)
        {
            Console.WriteLine($"{album.Title} - by {album.ArtistName}, ID: {album.Id}");
        }
    }
    public async Task SearchTracks(string query)
    {
        var tracks = await Search.SearchTracks(query, User);
        foreach (var track in tracks)
        {
            Console.WriteLine($"{track.Title} - by {track.ArtistName}, album: {track.AlbumTitle}, ID: {track.Id}");
        }
    }

    public async Task DownloadTrack(string id)
    {
        var track = await Track.Get(id, User);
        await track.DownloadToDir(new(Environment.CurrentDirectory), Namers.ArtistTrackTitleNamer);
        Console.WriteLine($"\"{track.ArtistName} - {track.Title}\" downloaded");
    }

    public async Task DownloadAlbum(string id)
    {
        var album = await Album.Get(id, User);
        await album.DownloadToDir(new(Environment.CurrentDirectory), Namers.ArtistTrackTitleNamer);
        Console.WriteLine($"\"{album.ArtistName} - {album.Title}\" downloaded");
    }
}