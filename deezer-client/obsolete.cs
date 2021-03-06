/*using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace deezer_client
{
    public class ArtistBare
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public async Task<Artist> Get() => await Artist.GetId(Id);
    }

    public class Artist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string Picture { get; set; }
        public string PictureSmall { get; set; }
        public string PictureMedium { get; set; }
        public string PictureBig { get; set; }
        public string PictureXl { get; set; }
        [JsonProperty("nb_album")] public int AlbumNumber { get; set; }
        [JsonProperty("nb_fan")] public int FanNumber { get; set; }

        public void _debug()
        {
            Console.WriteLine(Name);
        }

        public static async Task<Artist> GetId(int id)
        {
            var url = $"https://api.deezer.com/artist/{id}";
            var res = await Helper.Client.GetAsync(url);
            var body = await res.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<Artist>(body, Helper.SerializerSettings);

            return data;
        }

        public class TrackBare
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public async Task<Track> Get() => await Track.GetId(Id);
        }

        public class Track
        {
            public string Title { get; set; }
            public string TitleShort { get; set; }
            public string Link { get; set; }
            public int Id { get; set; }
            [JsonProperty("duration")] private int _duration { get; set; }

            [JsonProperty("")] public TimeSpan Duration => TimeSpan.FromSeconds(_duration);
            public int TrackPosition { get; set; }
            public int DiskNumber { get; set; }
            public DateTime ReleaseDate { get; set; }
            public ArtistBare Artist { get; set; }

            public void _debug()
            {
                Console.WriteLine($"{TrackPosition}: {Title}");
                var mins = Duration.Minutes;
                var secs = Duration.Seconds.ToString().PadLeft(2, '0');
                Console.WriteLine($"Duration: {mins}:{secs}");
            }

            public static async Task<Track> GetId(int id)
            {
                var url = $"https://api.deezer.com/track/{id}";
                var res = await Helper.Client.GetAsync(url);
                var body = await res.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<Track>(body, Helper.SerializerSettings);

                return data;
            }

            public class InternalTrack
            {
            }
        }

        public class AlbumBare
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public async Task<Album> Get() => await Album.GetId(Id);
        }

        public class Album
        {
            private class TracksInAlbum
            {
                public List<Track> Data { get; set; }
            }

            public int Id { get; set; }
            public string Title { get; set; }
            public string Link { get; set; }
            public string Cover { get; set; }
            public string CoverSmall { get; set; }
            public string CoverMedium { get; set; }
            public string CoverBig { get; set; }
            public string CoverXl { get; set; }
            [JsonProperty("label")] public string LabelName { get; set; }
            public int Duration { get; set; } // TODO: Use TimeSpan
            public DateTime ReleaseDate { get; set; }
            public bool Available { get; set; }
            [JsonProperty("tracks")] private TracksInAlbum _tracks { get; set; }

            [JsonProperty("")]
            public List<Track> Tracks
            {
                get => _tracks.Data;
            }

            public ArtistBare Artist { get; set; }

            public void _debug()
            {
                Console.WriteLine($"Title: {Title}");
                Console.WriteLine($"Duration: {TimeSpan.FromSeconds(Duration).Minutes}");
                Console.WriteLine("Tracks:");
                foreach (var track in Tracks)
                {
                    track._debug();
                }
            }

            public static async Task<Album> GetId(int id)
            {
                var url = $"https://api.deezer.com/album/{id}";
                var res = await Helper.Client.GetAsync(url);
                var body = await res.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<Album>(body, Helper.SerializerSettings);

                return data;
            }
        }
    }
}*/

