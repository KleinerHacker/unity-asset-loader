using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityAssetLoader.Runtime.Projects.unity_asset_loader.Scripts.Runtime
{
    public static class AssetResourcesLoader
    {
        public static void LoadFromResources(Type type, string path)
        {
            var objects = Resources.LoadAll(path, type);
            RegisterObjects(objects);
        }

        public static void LoadFromResources<T>(string path) where T : Object => LoadFromResources(typeof(T), path);
        
        public static void LoadFromResourcesAsync(Type type, string path, Action onFinished) => Task.Run(() =>
        {
            LoadFromResources(type, path);
            onFinished?.Invoke();
        });

        public static void LoadFromResourcesAsync<T>(string path, Action onFinished = null) where T : Object => LoadFromResourcesAsync(typeof(T), path, onFinished);

        public static void LoadFromResources(string path)
        {
            var objects = Resources.LoadAll(path);
            RegisterObjects(objects);
        }
        
        public static void LoadFromResourcesAsync(string path, Action onFinished = null) => Task.Run(() =>
        {
            LoadFromResources(path);
            onFinished?.Invoke();
        });

        public static void LoadFromBundle(Type type, string path, AssetResourcesLoaderBundleUnload unload = AssetResourcesLoaderBundleUnload.Unload)
        {
            var bundle = AssetBundle.LoadFromFile(path);
            try
            {
                var assets = bundle.LoadAllAssets(type);
                RegisterObjects(assets);
            }
            finally
            {
                if (unload != AssetResourcesLoaderBundleUnload.DoNotUnload)
                {
                    bundle.Unload(unload == AssetResourcesLoaderBundleUnload.UnloadComplete);
                }
            }
        }

        public static void LoadFromBundle<T>(string path, AssetResourcesLoaderBundleUnload unload = AssetResourcesLoaderBundleUnload.Unload) where T : Object => LoadFromBundle(typeof(T), path, unload);
        
        public static void LoadFromBundleAsync(Type type, string path, AssetResourcesLoaderBundleUnload unload = AssetResourcesLoaderBundleUnload.Unload, Action onFinished = null)
        {
            var bundleRequest = AssetBundle.LoadFromFileAsync(path);
            bundleRequest.completed += _ =>
            {
                var assetRequest = bundleRequest.assetBundle.LoadAllAssetsAsync(type);
                assetRequest.completed += _ =>
                {
                    try
                    {
                        RegisterObjects(assetRequest.allAssets);
                        onFinished?.Invoke();
                    }
                    finally
                    {
                        if (unload != AssetResourcesLoaderBundleUnload.DoNotUnload)
                        {
                            bundleRequest.assetBundle.UnloadAsync(unload == AssetResourcesLoaderBundleUnload.UnloadComplete);
                        }
                    }
                };
            };
        }

        public static void LoadFromBundleAsync<T>(string path, AssetResourcesLoaderBundleUnload unload = AssetResourcesLoaderBundleUnload.Unload, Action onFinished = null) where T : Object => LoadFromBundleAsync(typeof(T), path, unload, onFinished);

        public static void LoadFromBundle(string path, AssetResourcesLoaderBundleUnload unload = AssetResourcesLoaderBundleUnload.Unload, Action onFinished = null)
        {
            var bundle = AssetBundle.LoadFromFile(path);
            try
            {
                var assets = bundle.LoadAllAssets();
                RegisterObjects(assets);
                onFinished?.Invoke();
            }
            finally
            {
                if (unload != AssetResourcesLoaderBundleUnload.DoNotUnload)
                {
                    bundle.Unload(unload == AssetResourcesLoaderBundleUnload.UnloadComplete);
                }
            }
        }
        
        public static void LoadFromBundleAsync(string path, AssetResourcesLoaderBundleUnload unload = AssetResourcesLoaderBundleUnload.Unload, Action onFinished = null)
        {
            var bundleRequest = AssetBundle.LoadFromFileAsync(path);
            bundleRequest.completed += _ =>
            {
                var assetRequest = bundleRequest.assetBundle.LoadAllAssetsAsync();
                assetRequest.completed += _ =>
                {
                    try
                    {
                        RegisterObjects(assetRequest.allAssets);
                        onFinished?.Invoke();
                    }
                    finally
                    {
                        if (unload != AssetResourcesLoaderBundleUnload.DoNotUnload)
                        {
                            bundleRequest.assetBundle.UnloadAsync(unload == AssetResourcesLoaderBundleUnload.UnloadComplete);
                        }
                    }
                };
            };
        }
        
        public static string[] LoadScenesFromBundle(string path, AssetResourcesLoaderBundleUnload unload = AssetResourcesLoaderBundleUnload.DoNotUnload)
        {
            var bundle = AssetBundle.LoadFromFile(path);
            try
            {
                return bundle.GetAllScenePaths();
            }
            finally
            {
                if (unload != AssetResourcesLoaderBundleUnload.DoNotUnload)
                {
                    bundle.Unload(unload == AssetResourcesLoaderBundleUnload.UnloadComplete);
                }
            }
        }
        
        public static void LoadScenesFromBundleAsync(string path, AssetResourcesLoaderBundleUnload unload = AssetResourcesLoaderBundleUnload.DoNotUnload, Action<string[]> onFinished = null)
        {
            var bundleRequest = AssetBundle.LoadFromFileAsync(path);
            bundleRequest.completed += _ =>
            {
                try
                {
                    var allScenePaths = bundleRequest.assetBundle.GetAllScenePaths();
                    onFinished?.Invoke(allScenePaths);
                }
                finally
                {
                    if (unload != AssetResourcesLoaderBundleUnload.DoNotUnload)
                    {
                        bundleRequest.assetBundle.UnloadAsync(unload == AssetResourcesLoaderBundleUnload.UnloadComplete);
                    }
                }
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

    public enum AssetResourcesLoaderBundleUnload
    {
        DoNotUnload,
        Unload,
        UnloadComplete
    }
}