using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using Newtonsoft.Json;
using Resxul.Models;
using Serilog;

namespace Resxul.Services
{
    internal class ProfileService
    {
        private readonly ObservableCollection<Profile> _profiles;

        public ProfileService()
        {
            _profiles = new ObservableCollection<Profile>();

            var profilePaths = new List<string>();

            try
            {
                if (Directory.Exists(Global.ProfilesFolderPath))
                    profilePaths = Directory.GetFiles(Global.ProfilesFolderPath, "*.json").ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unable to get profile paths.");
            }

            foreach (var path in profilePaths)
            {
                try
                {
                    var profile = JsonConvert.DeserializeObject<Profile>(File.ReadAllText(path));
                    profile.FileName = Path.GetFileName(path);

                    _profiles.Add(profile);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Unable to read profile \"{path}\".");
                }
            }

            Profiles = new ReadOnlyObservableCollection<Profile>(_profiles);
        }

        public ReadOnlyObservableCollection<Profile> Profiles { get; }

        public void AddProfile(Profile profile)
        {
            profile.FileName = profile.Name.Replace(" ", "-") + ".json";
            _profiles.Add(profile);
        }

        public void DeleteProfile(Profile profile)
        {
            _profiles.Remove(profile);
        }

        public void SaveProfiles()
        {
            try
            {
                if (Directory.Exists(Global.ProfilesFolderPath))
                    Directory.GetFiles(Global.ProfilesFolderPath, "*.json").Apply(File.Delete);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unable to delete profiles.");
            }

            foreach (var profile in Profiles)
            {
                var path = Path.Combine(Global.ProfilesFolderPath, profile.FileName);

                try
                {
                    File.WriteAllText(path, JsonConvert.SerializeObject(profile, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Unable to save profile \"{profile.Name}\" to \"{path}\".");
                }

            }
        }
    }
}
