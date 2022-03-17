using System;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace music_player;

public class Configuration
{
    private string Path { get; }
    public string? Arl { get; set; }

    private record ConfigFile(string Arl);

    public Configuration()
    {
        Path = GetConfigPath();
        Read();
    }

    public void Update()
    {
        var data = JsonConvert.SerializeObject(new ConfigFile(Arl));
        File.WriteAllText(Path, data);
    }

    private void Read()
    {
        var fileData = File.ReadAllText(Path);
        var data = JsonConvert.DeserializeObject<ConfigFile>(fileData);
        Arl = data.Arl;
    }

    private static void InitConfig(string path)
    {
        Console.WriteLine("No config file. Creating a new one");
        File.WriteAllText(path, JsonConvert.SerializeObject(new ConfigFile("")));
    }

    private static string ConfigPathLinux()
    {
        var home = Environment.GetEnvironmentVariable("HOME")!;
        var directoryPath = System.IO.Path.Combine(home, ".config", "deezer-dl");
        var directory = Directory.CreateDirectory(directoryPath);
        var configPath = System.IO.Path.Combine(directoryPath, "config.json");
        if (!File.Exists(configPath))
            InitConfig(configPath);
        return configPath;
    }

    private string ConfigPathWindows()
    {
        return "";
    }
    public string GetConfigPath()
    {
        var path = Environment.OSVersion.Platform switch
        {
            PlatformID.Unix => ConfigPathLinux(),
            _ => throw new Exception("Unsupported os")
        };
        return path;
    }
}