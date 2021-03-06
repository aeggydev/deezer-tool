using System;
using System.Threading.Tasks;

namespace deezer_client
{
    public partial class Deezer
    {
        public async Task MyFavorites()
        {
            var data = new {checksums = ""};
            var json = await ApiCallMethod("user.getAllFeedbacks", data);
            Console.WriteLine(json);
            /*
            var count = JObject.Parse(json).SelectToken("SONGS.count")?.ToString();
            Console.WriteLine(count);
        */
        }
    }
}