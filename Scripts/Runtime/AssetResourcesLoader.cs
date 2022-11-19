using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityAssetLoader.Runtime.asset_loader.Scripts.Runtime
{
    public static class AssetResourcesLoader
    {
        public static void LoadFromResources(Type type, string path)
        {
            var objects = Resources.LoadAll(path, type);
            RegisterObjects(objects);
        }

        public static void LoadFromResources<T>(string path) where T : Object => LoadFromResources(typeof(T), path);
        
        public static void LoadFromResourcesAsync(Type type, string path) => Task.Run(() => LoadFromResources(type, path));

        public static void LoadFromResourcesAsync<T>(string path) where T : Object => LoadFromResourcesAsync(typeof(T), path);

        public static void LoadFromResources(string path)
        {
            var objects = Resources.LoadAll(path);
            RegisterObjects(objects);
        }
        
        public static void LoadFromResourcesAsync(string path) => Task.Run(() => LoadFromResources(path));

        public static void LoadFromBundle(Type type, string path)
        {
            var bundle = AssetBundle.LoadFromFile(path);
            try
            {
                var assets = bundle.LoadAllAssets(type);
                RegisterObjects(assets);
            }
            finally
            {
                AssetBundle.UnloadAllAssetBundles(false);
            }
        }

        public static void LoadFromBundle<T>(string path) where T : Object => LoadFromBundle(typeof(T), path);
        
        public static void LoadFromBundleAsync(Type type, string path)
        {
            var bundleRequest = AssetBundle.LoadFromFileAsync(path);
            bundleRequest.completed += _ =>
            {
                var assetRequest = bundleRequest.assetBundle.LoadAllAssetsAsync(type);
                assetRequest.completed += _ => RegisterObjects(assetRequest.allAssets);
            };
        }

        public static void LoadFromBundleAsync<T>(string path) where T : Object => LoadFromBundleAsync(typeof(T), path);

        public static void LoadFromBundle(string path)
        {
            var bundle = AssetBundle.LoadFromFile(path);
            try
            {
                var assets = bundle.LoadAllAssets();
                RegisterObjects(assets);
            }
            finally
            {
                AssetBundle.UnloadAllAssetBundles(false);
            }
        }
        
        public static void LoadFromBundleAsync(string path)
        {
            var bundleRequest = AssetBundle.LoadFromFileAsync(path);
            bundleRequest.completed += _ =>
            {
                var assetRequest = bundleRequest.assetBundle.LoadAllAssetsAsync();
                assetRequest.completed += _ => RegisterObjects(assetRequest.allAssets);
            };
        }

#if UNITY_EDITOR
        public static void LoadFromAssetDatabase(Type type, string path)
        {
            var assets = AssetDatabase.LoadAllAssetsAtPath(path)
                .Where(x => type == x.GetType())
                .ToArray();
            RegisterObjects(assets);
        }

        public static void LoadFromAssetDatabase<T>(string path) where T : Object => LoadFromAssetDatabase(typeof(T), path);

        public static void LoadFromAssetDatabase(string path)
        {
            var assets = AssetDatabase.LoadAllAssetsAtPath(path)
                .ToArray();
            RegisterObjects(assets);
        }

        public static void LoadFromBundleDeclaration(Type type, string bundleName)
        {
            var assets = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName)
                .SelectMany(AssetDatabase.LoadAllAssetsAtPath)
                .Where(x => type == x.GetType())
                .ToArray();
            RegisterObjects(assets);
        }

        public static void LoadFromBundleDeclaration<T>(string bundleName) where T : Object => LoadFromBundleDeclaration(typeof(T), bundleName);

        public static void LoadFromBundleDeclaration(string bundleName)
        {
            var assets = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName)
                .SelectMany(AssetDatabase.LoadAllAssetsAtPath)
                .ToArray();
            RegisterObjects(assets);
        }
#endif

        private static void RegisterObjects(Object[] objects)
        {
            AssetResources.RegisterAssets(objects);
        }
    }
}