using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace deezer_client
{
    public delegate string TrackNamer(Track track);

    public static class Namers
    {
        public static string TrackTitleNamer(Track track)
        {
            return $"{track.Title}.mp3";
        }

        public static string ArtistTrackTitleNamer(Track track)
        {
            return $"{track.ArtistName} - {track.Title}.mp3";
        }
    }

    public partial class Track : IApiMethod
    {
        [JsonProperty("DURATION")] private int _duration;
        public string cookies;
        public Deezer user;
        public static string MethodName => "song.getData";

        [JsonProperty("SNG_TITLE")] public string Title { get; set; }
        [JsonProperty("SNG_ID")] public string Id { get; set; }
        [JsonProperty("DIGITAL_RELEASE_DATE")] public DateTime DigitalReleaseDate { get; set; }

        [JsonProperty("PHYSICAL_RELEASE_DATE")]
        public DateTime PhysicalReleaseDate { get; set; }

        [JsonProperty("MD5_ORIGIN")] public string MD5 { get; set; }
        [JsonProperty("MEDIA_VERSION")] public string MediaVersion { get; set; }
        [JsonProperty("TRACK_NUMBER")] public int TrackNumber { get; set; }
        [JsonProperty("DISK_NUMBER")] public int DiskNumber { get; set; }
        public TimeSpan Duration => TimeSpan.FromSeconds(_duration);
        [JsonProperty("ALB_TITLE")] public string AlbumTitle { get; set; }
        [JsonProperty("ALB_ID")] public string AlbumId { get; set; }
        [JsonProperty("ART_NAME")] public string ArtistName { get; set; }
        [JsonProperty("ART_ID")] public string ArtistId { get; set; }
        [JsonProperty("LYRICS_ID")] public string LyricsId { get; set; }

        public static async Task<Track> Get(string id, Deezer user)
        {
            var data = new {SNG_ID = id};
            var json = await user.ApiCallMethod(MethodName, data);
            var track = JsonConvert.DeserializeObject<Track>(json);
            track.user = user;
            return track;
        }

        public async Task<FileInfo> DownloadToDir(DirectoryInfo outputDirectory, TrackNamer namer)
        {
            var filename = namer(this);
            foreach (var invalid in Path.GetInvalidFileNameChars()) filename = filename.Replace(invalid, '_');
            var path = Path.Combine(outputDirectory.FullName, filename);
            var res = await Download("1", path, cookies);
            return res;
        }
    }
}