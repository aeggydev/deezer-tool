﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace deezer_client;

internal static class TrackDownloadHelpers
{
    public static byte[] BlowfishKey(string id)
    {
        int ord(char c)
        {
            return char.ConvertToUtf32(c.ToString(), 0);
        }

        var secret = "g4el58wc0zvf9na1";
        string idMd5;

        using (var md5 = MD5.Create())
        {
            idMd5 = string.Join(string.Empty,
                md5.ComputeHash(Encoding.UTF8.GetBytes(id)).Select(b => b.ToString("x2")));
        }

        var nums = new List<int>();
        for (var i = 0; i < 16; i++)
        {
            var num = ord(idMd5[i]) ^ ord(idMd5[i + 16]) ^ ord(secret[i]);
            nums.Add(num);
        }

        var bytes = nums.Select(i => Convert.ToByte(i)).ToArray();
        return bytes;
    }

    public static string GetUrl(Track track, string qualityKey)
    {
        string Hexdigest(byte[] hash)
        {
            var sBuilder = new StringBuilder();
            foreach (var b in hash) sBuilder.Append(b.ToString("x2"));

            return sBuilder.ToString();
        }

        // Quality keys at https://github.com/acgonzales/pydeezer/blob/e894b2a14d13bc33d83bb71d75d0f6302357b5d8/pydeezer/constants/track_formats.py
        qualityKey = "1";

        const char magicChar = '¤';
        var step1 = string.Join(magicChar, new List<string> {track.MD5, qualityKey, track.Id, track.MediaVersion});

        string hashed;
        using (var md5 = MD5.Create())
        {
            var bytes = step1.Select(i => (byte) i).ToArray();
            var hash = md5.ComputeHash(bytes);
            hashed = Hexdigest(hash);
        }

        var step2 = hashed + magicChar + step1 + magicChar;
        step2 = step2.PadLeft(80, ' ');
        string hex;
        using (var aes = Aes.Create())
        {
            const string key = "jo6aey6haid2Teih";
            aes.Mode = CipherMode.ECB;
            var keyBytes = key.Select(i => (int) i).Select(i => (byte) i).ToArray();
            aes.Key = keyBytes;
            //aes.Padding = PaddingMode.None;
            using (var encryptor = aes.CreateEncryptor())
            {
                var bytes = step2.Select(i => (byte) i).ToArray();
                var hash = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
                hex = string.Concat(Array.ConvertAll(hash, x => x.ToString("x2")));
            }
        }

        var cdn = track.MD5[0];
        var url = $"https://e-cdns-proxy-{cdn}.dzcdn.net/mobile/1/{hex}";
        return url;
    }
}

public partial class Track
{
    public async Task<FileInfo> Download(string qualityKey, string path, string cookies)
    {
        // TODO: SPLIT FUNCTION INTO MULTIPLE FUNCTIONS
        // TODO: RESTRUCTURE CODE TO MAKE BETTER FIT OBJECT STRUCTURE
        // TODO: FIND A WORKAROUND FOR PASSING AROUND COOKIES EVERYWHERE
        // TODO: SWITCH BACK TO COOKIES BEING HELD BY CookieContainer
        // TODO: FIX METHOD SIGNATURES BEING ALL OVER THE PLACE WITH DELEGATES AND INTERFACES
        var url = TrackDownloadHelpers.GetUrl(this, qualityKey);
        var blowfishKey = TrackDownloadHelpers.BlowfishKey(Id);
        // TODO: FIX USER BEING NULL
        HttpResponseMessage res;
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Cookie", cookies);
            res = await user.client.GetAsync(url);
        }

        var i = 0;
        var stream = await res.Content.ReadAsStreamAsync();
        var file = File.Create(path);
        const int chunkSize = 2048;
        var chunk = new byte[chunkSize];
        var writer = new BinaryWriter(file);
        var engine = new BlowfishEngine();
        var cipher = new BufferedBlockCipher(new CbcBlockCipher(engine));
        var keyIv = Enumerable.Range(0, 8)
            .Select(num => (byte) num).ToArray();
        cipher.Init(false, new ParametersWithIV(new KeyParameter(blowfishKey), keyIv));
        while (await stream.ReadAsync(chunk, 0, chunk.Length) > 0)
        {
            if (i % 3 > 0)
            {
                writer.Write(chunk);
            }
            else if (chunk.Length < chunkSize)
            {
                writer.Write(chunk);
                break;
            }
            else
            {
                var data = cipher.ProcessBytes(chunk).Concat(cipher.DoFinal()).ToArray();
                writer.Write(data);
            }

            i += 1;
        }

        writer.Close();
        file.Close();

        return new FileInfo(path);
    }
}