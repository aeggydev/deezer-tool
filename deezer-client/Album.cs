using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace deezer_client
{
    public class Album : IApiMethod
    {
        public Track[] tracks;
        private Deezer user;
        public static string MethodName => "album.getData";

        [JsonProperty("ALB_ID")] public string Id { get; set; }
        [JsonProperty("ALB_TITLE")] public string Title { get; set; }
        [JsonProperty("ALB_PICTURE")] public string Picture { get; set; }
        [JsonProperty("ART_ID")] public string ArtistId { get; set; }
        [JsonProperty("ART_NAME")] public string ArtistName { get; set; }

        [JsonProperty("PHYSICAL_RELEASE_DATE")]
        public DateTime PhysicalReleaseDate { get; set; }

        [JsonProperty("NB_FAN")] public int FanNumber { get; set; }
        [JsonProperty("NUMBER_DISK")] public string DiskNumber { get; set; }
        [JsonProperty("NUMBER_TRACK")] public string TrackNumber { get; set; }

        public static async Task<Album> Get(string id, Deezer user)
        {
            var data = new {ALB_ID = id};
            var json = await user.ApiCallMethod(MethodName, data);
            var album = JsonConvert.DeserializeObject<Album>(json);
            album.user = user;
            return album;
        }

        public async Task<Track[]> GetTracks()
        {
            var json = await user.ApiCallMethod("song.getListByAlbum", new {ALB_ID = Id, NB = -1});
            var trackJson = JsonConvert.SerializeObject(JObject.Parse(json).SelectToken("$.data"));
            tracks = JsonConvert.DeserializeObject<Track[]>(trackJson);
            foreach (var track in tracks)
            {
                track.user = user;
                user.downloadedSongs += 1;
            }

            return tracks;
        }

        public async Task<FileInfo[]> DownloadToDir(DirectoryInfo outputDirectory, TrackNamer namer)
        {
            if (!outputDirectory.Exists) outputDirectory.Create();

            var cover = await AlbumCover.Get(Picture, 300, "jpg", user.client);
            var coverPath = Path.Join(outputDirectory.FullName, "cover.jpg");
            File.WriteAllBytes(coverPath, cover.Image);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine($"\"{Title}\" downloading...");
            await GetTracks();
            var tasks = tracks.Select(async track =>
                {
                    var file = await track.DownloadToDir(outputDirectory, namer);
                    Tagger.TagMusicFile(file, track, new FileInfo(coverPath));
                    return file;
                })
                .ToArray();
            var data = await Task.WhenAll(tasks);
            stopwatch.Stop();
            var time = stopwatch.Elapsed;
            Console.WriteLine(
                $"Download of \"{Title}\" complete. Download took {Math.Floor(time.TotalSeconds)} seconds");
            return data;
        }
    }

    public class AlbumCover
    {
        public byte[] Image { get; set; }
        public string Extension { get; set; }
        public Dimensions Dimensions { get; set; }
        public string MimeType { get; set; }

        public static async Task<AlbumCover> Get(string id, int size, string extension, HttpClient client)
        {
            extension = extension.ToLower();
            if (!extension.Equals("jpg") && !extension.Equals("png"))
                throw new ArgumentException("Extension has to be either png or jpg.");

            var url = $"https://e-cdns-images.dzcdn.net/images/cover/{id}/{size}x{size}.{extension}";
            var res = await client.GetAsync(url);
            var data = await res.Content.ReadAsByteArrayAsync();
            var cover = new AlbumCover
            {
                Image = data,
                Dimensions = new Dimensions {X = size, Y = size},
                Extension = extension,
                MimeType = extension == "jpg" ? "image/jpeg" : "image/png"
            };
            return cover;
        }
    }
}