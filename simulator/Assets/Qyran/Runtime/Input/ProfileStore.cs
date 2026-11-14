using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Qyran.Input
{
    public sealed class ProfileStore
    {
        readonly string directory;
        public ProfileStore(string directory) => this.directory = directory;

        public string PathFor(string deviceKey)
        {
            using var hash = SHA256.Create();
            string name = BitConverter.ToString(hash.ComputeHash(Encoding.UTF8.GetBytes(deviceKey)))
                .Replace("-", "").Substring(0, 16).ToLowerInvariant();
            return Path.Combine(directory, name + ".json");
        }

        public void Save(string deviceKey, ControllerProfile profile)
        {
            if (!profile.Validate(out string error)) throw new ArgumentException(error);
            Directory.CreateDirectory(directory);
            string path = PathFor(deviceKey);
            string temporary = path + ".tmp";
            File.WriteAllText(temporary, JsonUtility.ToJson(profile, true), new UTF8Encoding(false));
            if (File.Exists(path)) File.Replace(temporary, path, null);
            else File.Move(temporary, path);
        }

        public bool TryLoad(string deviceKey, out ControllerProfile profile, out string error)
        {
            profile = null;
            error = "";
            string path = PathFor(deviceKey);
            if (!File.Exists(path)) return false;
            try
            {
                var loaded = JsonUtility.FromJson<ControllerProfile>(File.ReadAllText(path));
                if (loaded == null || !loaded.Validate(out error)) return false;
                profile = loaded;
                return true;
            }
            catch (Exception ex) when (ex is IOException || ex is ArgumentException || ex is UnauthorizedAccessException)
            {
                error = "Could not load controller profile: " + ex.Message;
                return false;
            }
        }
    }
}
