using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace deezer_client
{
    public class Artist
    {
        private Deezer user;
        public static string MethodName => "deezer.pageArtist";

        public static async Task<Artist> Get(string id, Deezer user)
        {
            // TODO: ADD PROPS TO CLASS
            var data = new {ART_ID = id, LANG = "en"};
            var json = await user.ApiCallMethod(MethodName, data);
            File.WriteAllText("artist.json", json);
            var artist = JsonConvert.DeserializeObject<Artist>(json);
            artist.user = user;
            return artist;
        }
    }
}