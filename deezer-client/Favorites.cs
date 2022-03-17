using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace deezer_client;

public partial class Deezer
{
    public class allFeedbacksResult
    {
        public class dataWrapper<T>
        {
            [JsonProperty("data")] public List<T> Data;
            [JsonProperty("count")] public int Count;
            [JsonProperty("total")] public int Total;
        }
        public class song
        {
            [JsonProperty("SNG_ID")] public string Id;
        }

        public class album
        {
            [JsonProperty("ALB_ID")] public string Id;
        }
        public class favoritesResult
        {
            [JsonProperty("SONGS")] public dataWrapper<song> Songs;
            [JsonProperty("ALBUMS")] public dataWrapper<album> Albums;
        }
            
        [JsonProperty("FAVORITES")] public favoritesResult Favorites { get; set; }
    }
    public async Task<allFeedbacksResult> MyFavorites()
    {
        var sendData = "{\"checksums\":null}";
        var json = await ApiCallMethodRaw("user.getAllFeedbacks", sendData);
        //File.WriteAllText("/home/aeggy/RiderProjects/deezer-tool/music-player/lol.json", json);
        var path = Path.Combine(Environment.CurrentDirectory, "lol.json");
        File.WriteAllText(path, json);

        var data = JsonConvert.DeserializeObject<allFeedbacksResult>(json);
        return data;
    }
}