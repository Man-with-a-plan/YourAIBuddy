using System.IO;
using UnityEngine;
using  UnityEditor;
using static System.IO.Directory;
using static System.IO.Path;
using static UnityEditor.AssetDatabase;


    public static class Setup
    {
          [MenuItem("Tools/Setup/Create Default Folders")]
        public static void CreateDefaultFolders()
        {
        Folders.CreateDefault("_Project", "animation", "art", "Materials", "Prefabs", "ScriptableObjects", "Scripts", "settings");
        Refresh();


        }
        static class Folders
        {
            public static void CreateDefault(string root, params string[] folders)
            {
                var fullpath = Combine(Application.dataPath, root);
                foreach (var folder in folders)
                {
                    var path = Path.Combine(fullpath, folder);
                    if (!Exists(path))
                    {
                        CreateDirectory(path);
                    }
                }

            }
        }
    }

