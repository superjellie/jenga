using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Jenga {
    // Runtime simple general save and load system
    public static class SaveMaster {

        public static string savesPath => 
            $"{Application.persistentDataPath}/Saves";
        
        // Readonly
        // Use to save to general data for every player
        public static string generalProfile = "general.save";


        // Readonly
        // Use those variables to make relevant UI
        public static bool isSaving = false;
        public static bool isLoading = false;
        public static bool isBusy => isSaving || isLoading;

        //
        public static Dictionary<string, Dictionary<string, object>> 
            loadedProfiles = new();


        public delegate void SaveLoadDelegate();

        // Initiate save and load for everybody subscribed 
        // Subscribe to them in MonoBehaviours on Awake()
        public static event SaveLoadDelegate onSave;
        public static event SaveLoadDelegate onLoad;

        public static void SaveDataToProfiles() => onSave.Invoke();
        public static void LoadDataFromProfiles() => onLoad.Invoke();

        // Initiate save
        // It will copy relevant state
        // And then save it to disc asyncroniosly
        public static void SaveProfilesToDisk() {
            isSaving = true;

            if (onSave != null)
                onSave.Invoke();

            var tasks = new List<System.Threading.Tasks.Task>();

            foreach (var (profile, data) in loadedProfiles) {
                var path = $"{savesPath}/{profile}";
                Directory.CreateDirectory(savesPath);
                
                if (!loadedProfiles.ContainsKey(profile)) 
                    loadedProfiles[profile] = new();

                var profileData 
                    = JsonConvert.SerializeObject(
                        data, 
                        Formatting.Indented, 
                        new JsonSerializerSettings()
                            { TypeNameHandling = TypeNameHandling.Objects }
                    );
                tasks.Add(File.WriteAllTextAsync(path, profileData));
            }

            System.Threading.Tasks.Task.WhenAll(tasks)
                .ContinueWith((task) => {
                    isSaving = false;
                });
        }

        // Initiate load
        // It will asyncroniosly load profile and add it to loaded profiles
        public static void LoadProfilesFromDisk(string[] profiles) { 
            isLoading = true;
            var tasks = new List<System.Threading.Tasks.Task>();
            foreach (var profile in profiles) {
                var path = $"{savesPath}/{profile}";
                
                if (!File.Exists(path)) {
                    loadedProfiles[profile] = new();
                    continue;
                }

                tasks.Add(
                    File.ReadAllTextAsync(path)
                        .ContinueWith((task) => {
                            var json = task.Result;
                            loadedProfiles[profile] = JsonConvert
                                .DeserializeObject
                                        <Dictionary<string, object>>(
                                    json, 
                                    new JsonSerializerSettings()
                                        { TypeNameHandling = TypeNameHandling.Objects }
                                );
                        })
                );
            }

            System.Threading.Tasks.Task.WhenAll(tasks)
                .ContinueWith((task) => {
                    if (onLoad != null)
                        onLoad.Invoke();
                    isLoading = false;
                });
        }

        // Removes loaded profile instance
        public static void RemoveLoadedProfile(string profile)
            => loadedProfiles.Remove(profile);
        public static void RemoveAllLoadedProfile()
            => loadedProfiles.Clear();

        // Reads value from loaded profile
        public static T ReadValue<T>(string profile, string key) {
            if (!loadedProfiles.ContainsKey(profile)) return default(T);
            if (loadedProfiles[profile][key] is T t) return t;
            return default(T);
        }

        // Writes file to loaded profile
        public static void WriteValue<T>(string profile, string key, T value) {
            if (!loadedProfiles.ContainsKey(profile)) return;
            loadedProfiles[profile][key] = value;
        }

        public static string[] GetAllProfiles() 
            => Directory.GetFiles(
                savesPath, "*.save", SearchOption.TopDirectoryOnly
            );
        

    }
}