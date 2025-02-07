using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;

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
        public static Dictionary<string, ADT.Map<string, string>> 
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

            var tasks = new List<Task<bool>>();
            foreach (var (profile, data) in loadedProfiles) {
                var path = $"{savesPath}/{profile}";

                if (!File.Exists(path)) File.Create(path);
                if (!loadedProfiles.ContainsKey(profile)) 
                    loadedProfiles[profile] = new ADT.Map<string, string>();

                var profileData = JsonUtility.ToJson(data);

                tasks.Add(
                    File.WriteAllTextAsync(profile, profileData)
                        .ContinueWith((task) => isSaving = false)
                );
            }

            System.Threading.Tasks.Task.WhenAll(tasks)
                .ContinueWith((task) => isSaving = false);
        }

        // Initiate load
        // It will asyncroniosly load profile and add it to loaded profiles
        public static void LoadProfilesFromDisk(string[] profiles) { 
            isLoading = true;
            var tasks = new List<System.Threading.Tasks.Task>();
            foreach (var profile in profiles) {
                var path = $"{savesPath}/{profile}";
                if (!File.Exists(path)) File.Create(path);

                tasks.Add(
                    File.ReadAllTextAsync(path)
                        .ContinueWith((task) => {
                            var result = task.Result;
                            var map = JsonUtility
                                .FromJson<ADT.Map<string, string>>(result);
                            loadedProfiles[profile] = map;

                            isLoading = false;
                        })
                );
            }

            System.Threading.Tasks.Task.WhenAll(tasks)
                .ContinueWith((task) => isLoading = false);
        }

        // Removes loaded profile instance
        public static void RemoveLoadedProfile(string profile)
            => loadedProfiles.Remove(profile);
        public static void RemoveAllLoadedProfile()
            => loadedProfiles.Clear();

        // Reads value from loaded profile
        public static T ReadValue<T>(string profile, string key) {
            if (!loadedProfiles.ContainsKey(profile)) return default(T);

            return JsonUtility.FromJson<T>(loadedProfiles[profile][key]);
        }

        // Writes file to loaded profile
        public static void WriteValue<T>(string profile, string key, T value) {
            if (!loadedProfiles.ContainsKey(profile)) return;

            loadedProfiles[profile][key] = JsonUtility.ToJson(value);
        }

        public static string[] GetAllProfiles() 
            => Directory.GetFiles(
                savesPath, "*.save", SearchOption.TopDirectoryOnly
            );
        

    }
}