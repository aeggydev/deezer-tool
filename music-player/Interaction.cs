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
            Console.WriteLine($"{album.Title} - by {album.ArtistName}");
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
}