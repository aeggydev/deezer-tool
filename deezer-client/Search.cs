using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace deezer_client
{
    public static class Search
    {
        public static async Task<Track[]> SearchTracks(string query, Deezer user)
        {
            var parameters = new Dictionary<string, string>
            {
                {"q", query}
            };
            var tracksLegacyJson = await user.LegacyApiCallMethod("search/track", parameters, true);
            var tracksLegacy = JsonConvert.DeserializeObject<TrackBare[]>(tracksLegacyJson);

            var tracks = await Task.WhenAll(tracksLegacy
                .Select(async track => await Track.Get(track.Id, user)).ToArray());
            tracks = tracks.GroupBy(track => track.Id).Select(x => x.First()).ToArray();
            return tracks;
        }

        public static async Task<Album[]> SearchAlbums(string query, Deezer user)
        {
            var parameters = new Dictionary<string, string>
            {
                {"q", query}
            };
            var albumsLegacyJson = await user.LegacyApiCallMethod("search/album", parameters, true);
            var albumsLegacy = JsonConvert.DeserializeObject<AlbumBare[]>(albumsLegacyJson);

            var albums = await Task.WhenAll(albumsLegacy
                .Select(async album => await Album.Get(album.Id, user)).ToArray());
            return albums;
        }

        private class TrackBare
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public ArtistBare Artist { get; set; }
        }

        private class ArtistBare
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        private class AlbumBare
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public ArtistBare Artist { get; set; }
        }
    }
}