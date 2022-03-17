using System.IO;
using TagLib;
using File = TagLib.File;

namespace deezer_client;

internal class Tagger
{
    public static void TagMusicFile(FileInfo file, Track track, FileInfo cover)
    {
        // TODO: ADD TAGS THAT ARE ONLY IN ALBUM OBJECTS
        // TODO: ADD ARTISTS
        var tfile = File.Create(file.FullName);
        var tag = tfile.Tag;
        tag.Title = track.Title;
        tag.Album = track.AlbumTitle;
        tag.AlbumArtists = new[] {track.ArtistName};
        tag.Year = (uint) track.PhysicalReleaseDate.Year;
        tag.Disc = (uint) track.DiskNumber;
        tag.Track = (uint) track.TrackNumber;
        tag.Pictures = new IPicture[1] {new Picture(cover.FullName)};
        tfile.Save();
    }
}