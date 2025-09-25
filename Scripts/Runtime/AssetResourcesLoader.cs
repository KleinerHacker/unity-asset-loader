using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace UnityAssetLoader.Runtime.Projects.unity_asset_loader.Scripts.Runtime
{
    [Obsolete("Use AssetResourcesManager instead. Maybe there are bugs in this implementation in case of using keySelector.")]
    public static class AssetResourcesLoader
    {
        #region Classic Resources

        public static void LoadFromResources(Type type, string path, Func<Object, Object> converter = null)
        {
            var objects = DoLoadFromResources(type, path, converter);
            AssetResources.RegisterAssets(objects);
        }

        public static void LoadFromResources(Type type, string path, string key, Func<Object, Object> converter = null)
        {
            var objects = DoLoadFromResources(type, path, converter);
            AssetResources.RegisterAssets(objects, key);
        }

        public static void LoadFromResources(Type type, string path, Func<Object, string> keySelector,
            Func<Object, Object> converter = null)
        {
            var objects = DoLoadFromResources(type, path, converter);
            foreach (var o in objects)
            {
                var key = keySelector(o);
                AssetResources.RegisterAssets(new[] { o }, key);
            }
        }

        private static Object[] DoLoadFromResources(Type type, string path, Func<Object, Object> converter)
        {
            var objects = Resources.LoadAll(path, type);
            if (converter != null)
            {
                objects = objects.Select(converter).ToArray();
            }

            return objects;
        }

        public static void LoadFromResources<T>(string path, Func<T, Object> converter = null) where T : Object =>
            LoadFromResources(typeof(T), path, o => converter?.Invoke((T)o));

        public static void LoadFromResources<T>(string path, string key, Func<T, Object> converter = null)
            where T : Object =>
            LoadFromResources(typeof(T), path, key, o => converter?.Invoke((T)o));

        public static void LoadFromResources<T>(string path, Func<T, string> keySelector,
            Func<T, Object> converter = null) where T : Object =>
            LoadFromResources(typeof(T), path, o => keySelector((T)o), o => converter?.Invoke((T)o));

        public static void LoadFromResources(string path, Func<Object, Object> converter = null)
        {
            var objects = DoLoadFromResources(path, converter);
            AssetResources.RegisterAssets(objects);
        }

        public static void LoadFromResources(string path, string key, Func<Object, Object> converter = null)
        {
            var objects = DoLoadFromResources(path, converter);
            AssetResources.RegisterAssets(objects, key);
        }

        public static void LoadFromResources(string path, Func<Object, string> keySelector,
            Func<Object, Object> converter = null)
        {
            var objects = DoLoadFromResources(path, converter);
            foreach (var o in objects)
            {
                var key = keySelector(o);
                AssetResources.RegisterAssets(new[] { o }, key);
            }
        }

        private static Object[] DoLoadFromResources(string path, Func<Object, Object> converter)
        {
            var objects = Resources.LoadAll(path);
            if (converter != null)
            {
                objects = objects.Select(converter).ToArray();
            }

            return objects;
        }

        #endregion

        #region Asset Bundle

        public static void LoadFromBundle(Type type, string path,
            AssetResourcesLoaderBundleUnload unload = AssetResourcesLoaderBundleUnload.Unload,
            Func<Object, Object> converter = null)
        {
            var assets = DoLoadFromBundle(type, path, unload, converter);
            AssetResources.RegisterAssets(assets);
        }

        public static void LoadFromBundle(Type type, string path, string key,
            AssetResourcesLoaderBundleUnload unload = AssetResourcesLoaderBundleUnload.Unload,
            Func<Object, Object> converter = null)
        {
            var assets = DoLoadFromBundle(type, path, unload, converter);
            AssetResources.RegisterAssets(assets, key);
        }

        public static void LoadFromBundle(Type type, string path, Func<Object, string> keySelector,
            AssetResourcesLoaderBundleUnload unload = AssetResourcesLoaderBundleUnload.Unload,
            Func<Object, Object> converter = null)
        {
            var assets = DoLoadFromBundle(type, path, unload, converter);
            foreach (var o in assets)
            {
                var key = keySelector(o);
                AssetResources.RegisterAssets(new[] { o }, key);
            }
        }

        private static Object[] DoLoadFromBundle(Type type, string path, AssetResourcesLoaderBundleUnload unload,
            Func<Object, Object> converter)
        {
            var bundle = AssetBundle.LoadFromFile(path);
            try
            {
                var assets = bundle.LoadAllAssets(type);
                if (converter != null)
                {
                    assets = assets.Select(converter).ToArray();
                }

                return assets;
            }
            finally
            {
                if (unload != AssetResourcesLoaderBundleUnload.DoNotUnload)
                {
                    bundle.Unload(unload == AssetResourcesLoaderBundleUnload.UnloadComplete);
                }
            }
        }

        public static void LoadFromBundle<T>(string path,
            AssetResourcesLoaderBundleUnload unload = AssetResourcesLoaderBundleUnload.Unload,
            Func<T, Object> converter = null) where T : Object =>
            LoadFromBundle(typeof(T), path, unload, o => converter?.Invoke((T)o));

        public static void LoadFromBundle<T>(string path, string key,
            AssetResourcesLoaderBundleUnload unload = AssetResourcesLoaderBundleUnload.Unload,
            Func<T, Object> converter = null) where T : Object =>
            LoadFromBundle(typeof(T), path, key, unload, o => converter?.Invoke((T)o));

        public static void LoadFromBundle<T>(string path, Func<T, string> keySelector,
            AssetResourcesLoaderBundleUnload unload = AssetResourcesLoaderBundleUnload.Unload,
            Func<T, Object> converter = null) where T : Object =>
            LoadFromBundle(typeof(T), path, o => keySelector((T)o), unload, o => converter?.Invoke((T)o));

        public static AsyncOperation LoadFromBundleAsync(Type type, string path,
            AssetResourcesLoaderBundleUnload unload = AssetResourcesLoaderBundleUnload.Unload,
            Func<Object, Object> converter = null, Action onFinished = null) =>
            DoLoadFromBundleAsync(type, path, unload, converter, onFinished, AssetResources.RegisterAssets);

        public static AsyncOperation LoadFromBundleAsync(Type type, string path, string key,
            AssetResourcesLoaderBundleUnload unload = AssetResourcesLoaderBundleUnload.Unload,
            Func<Object, Object> converter = null, Action onFinished = null) =>
            DoLoadFromBundleAsync(type, path, unload, converter, onFinished,
                objects => AssetResources.RegisterAssets(objects, key));

        public static AsyncOperation LoadFromBundleAsync(Type type, string path, Func<Object, string> keySelector,
            AssetResourcesLoaderBundleUnload unload = AssetResourcesLoaderBundleUnload.Unload,
            Func<Object, Object> converter = null, Action onFinished = null) =>
            DoLoadFromBundleAsync(type, path, unload, converter, onFinished,
                objects =>
                {
                    foreach (var o in objects)
                    {
                        var key = keySelector(o);
                        AssetResources.RegisterAssets(new[] { o }, key);
                    }
                });

        private static AsyncOperation DoLoadFromBundleAsync(Type type, string path,
            AssetResourcesLoaderBundleUnload unload, Func<Object, Object> converter, Action onFinished,
            Action<Object[]> registerAction)
        {
            var bundleRequest = AssetBundle.LoadFromFileAsync(path);
            bundleRequest.completed += _ =>
            {
                var assetRequest = bundleRequest.assetBundle.LoadAllAssetsAsync(type);
                assetRequest.completed += _ =>
                {
                    try
                    {
                        var assets = converter != null
                            ? assetRequest.allAssets.Select(converter).ToArray()
                            : assetRequest.allAssets;
                        registerAction(assets);
                    }
                    finally
                    {
                        if (unload != AssetResourcesLoaderBundleUnload.DoNotUnload)
                        {
                            bundleRequest.assetBundle.UnloadAsync(unload ==
                                                                  AssetResourcesLoaderBundleUnload.UnloadComplete);
                        }

                        onFinished?.Invoke();
                    }
                };
            };

            return bundleRequest;
        }

        public static void LoadFromBundleAsync<T>(string path,
            AssetResourcesLoaderBundleUnload unload = AssetResourcesLoaderBundleUnload.Unload,
            Func<T, Object> converter = null, Action onFinished = null)
            where T : Object => LoadFromBundleAsync(typeof(T), path, unload, o => converter?.Invoke((T)o), onFinished);

        public static void LoadFromBundleAsync<T>(string path, string key,
            AssetResourcesLoaderBundleUnload unload = AssetResourcesLoaderBundleUnload.Unload,
            Func<T, Object> converter = null, Action onFinished = null)
            where T : Object =>
            LoadFromBundleAsync(typeof(T), path, key, unload, o => converter?.Invoke((T)o), onFinished);

        public static void LoadFromBundleAsync<T>(string path, Func<T, string> keySelector,
            AssetResourcesLoaderBundleUnload unload = AssetResourcesLoaderBundleUnload.Unload,
            Func<T, Object> converter = null, Action onFinished = null)
            where T : Object => LoadFromBundleAsync(typeof(T), path, o => keySelector((T)o), unload,
            o => converter?.Invoke((T)o), onFinished);

        public static void LoadFromBundle(string path,
            AssetResourcesLoaderBundleUnload unload = AssetResourcesLoaderBundleUnload.Unload,
            Func<Object, Object> converter = null)
        {
            var assets = DoLoadFromBundle(path, unload, converter);
            AssetResources.RegisterAssets(assets);
        }

        public static void LoadFromBundle(string path, string key,
            AssetResourcesLoaderBundleUnload unload = AssetResourcesLoaderBundleUnload.Unload,
            Func<Object, Object> converter = null)
        {
            var assets = DoLoadFromBundle(path, unload, converter);
            AssetResources.RegisterAssets(assets, key);
        }

        public static void LoadFromBundle(string path, Func<Object, string> keySelector,
            AssetResourcesLoaderBundleUnload unload = AssetResourcesLoaderBundleUnload.Unload,
            Func<Object, Object> converter = null)
        {
            var assets = DoLoadFromBundle(path, unload, converter);
            foreach (var o in assets)
            {
                var key = keySelector(o);
                AssetResources.RegisterAssets(new[] { o }, key);
            }
        }

        private static Object[] DoLoadFromBundle(string path, AssetResourcesLoaderBundleUnload unload,
            Func<Object, Object> converter)
        {
            var bundle = AssetBundle.LoadFromFile(path);
            try
            {
                var assets = bundle.LoadAllAssets();
                if (converter != null)
                {
                    assets = assets.Select(converter).ToArray();
                }

                return assets;
            }
            finally
            {
                if (unload != AssetResourcesLoaderBundleUnload.DoNotUnload)
                {
                    bundle.Unload(unload == AssetResourcesLoaderBundleUnload.UnloadComplete);
                }
            }
        }

        public static AsyncOperation LoadFromBundleAsync(string path,
            AssetResourcesLoaderBundleUnload unload = AssetResourcesLoaderBundleUnload.Unload,
            Func<Object, Object> converter = null, Action onFinished = null) =>
            DoLoadFromBundleAsync(path, unload, converter, onFinished, AssetResources.RegisterAssets);

        public static AsyncOperation LoadFromBundleAsync(string path, string key,
            AssetResourcesLoaderBundleUnload unload = AssetResourcesLoaderBundleUnload.Unload,
            Func<Object, Object> converter = null, Action onFinished = null) =>
            DoLoadFromBundleAsync(path, unload, converter, onFinished,
                objects => AssetResources.RegisterAssets(objects, key));

        public static AsyncOperation LoadFromBundleAsync(string path, Func<Object, string> keySelector,
            AssetResourcesLoaderBundleUnload unload = AssetResourcesLoaderBundleUnload.Unload,
            Func<Object, Object> converter = null, Action onFinished = null) =>
            DoLoadFromBundleAsync(path, unload, converter, onFinished,
                objects =>
                {
                    foreach (var o in objects)
                    {
                        var key = keySelector(o);
                        AssetResources.RegisterAssets(new[] { o }, key);
                    }
                });

        private static AsyncOperation DoLoadFromBundleAsync(string path, AssetResourcesLoaderBundleUnload unload,
            Func<Object, Object> converter, Action onFinished, Action<Object[]> registerAction)
        {
            var bundleRequest = AssetBundle.LoadFromFileAsync(path);
            bundleRequest.completed += _ =>
            {
                var assetRequest = bundleRequest.assetBundle.LoadAllAssetsAsync();
                assetRequest.completed += _ =>
                {
                    try
                    {
                        var assets = converter != null
                            ? assetRequest.allAssets.Select(converter).ToArray()
                            : assetRequest.allAssets;
                        registerAction(assets);
                    }
                    finally
                    {
                        if (unload != AssetResourcesLoaderBundleUnload.DoNotUnload)
                        {
                            bundleRequest.assetBundle.UnloadAsync(unload ==
                                                                  AssetResourcesLoaderBundleUnload.UnloadComplete);
                        }

                        onFinished?.Invoke();
                    }
                };
            };

            return bundleRequest;
        }

        public static string[] LoadScenesFromBundle(string path,
            AssetResourcesLoaderBundleUnload unload = AssetResourcesLoaderBundleUnload.DoNotUnload)
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

        public static void LoadScenesFromBundleAsync(string path,
            AssetResourcesLoaderBundleUnload unload = AssetResourcesLoaderBundleUnload.DoNotUnload,
            Action<string[]> onFinished = null)
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
                        bundleRequest.assetBundle.UnloadAsync(unload ==
                                                              AssetResourcesLoaderBundleUnload.UnloadComplete);
                    }
                }
            };
        }

        #endregion

        #region Asset Database (Editor Only)

#if UNITY_EDITOR
        public static void LoadFromAssetDatabase(Type type, string path, Func<Object, Object> converter = null)
        {
            var assets = DoLoadFromAssetDatabase(type, path, converter);
            AssetResources.RegisterAssets(assets);
        }

        public static void LoadFromAssetDatabase(Type type, string path, string key,
            Func<Object, Object> converter = null)
        {
            var assets = DoLoadFromAssetDatabase(type, path, converter);
            AssetResources.RegisterAssets(assets, key);
        }

        public static void LoadFromAssetDatabase(Type type, string path, Func<Object, string> keySelector,
            Func<Object, Object> converter = null)
        {
            var assets = DoLoadFromAssetDatabase(type, path, converter);
            foreach (var o in assets)
            {
                var key = keySelector(o);
                AssetResources.RegisterAssets(new[] { o }, key);
            }
        }

        private static Object[] DoLoadFromAssetDatabase(Type type, string path, Func<Object, Object> converter) =>
            AssetDatabase.LoadAllAssetsAtPath(path)
                .Where(x => type == x.GetType())
                .Select(converter ?? (x => x))
                .ToArray();

        public static void LoadFromAssetDatabase<T>(string path, Func<T, Object> converter = null) where T : Object =>
            LoadFromAssetDatabase(typeof(T), path, o => converter?.Invoke((T)o));

        public static void LoadFromAssetDatabase<T>(string path, string key, Func<T, Object> converter = null)
            where T : Object =>
            LoadFromAssetDatabase(typeof(T), path, key, o => converter?.Invoke((T)o));

        public static void LoadFromAssetDatabase<T>(string path, Func<T, string> keySelector,
            Func<T, Object> converter = null) where T : Object =>
            LoadFromAssetDatabase(typeof(T), path, o => keySelector((T)o), o => converter?.Invoke((T)o));

        public static void LoadFromAssetDatabase(string path, Func<Object, Object> converter = null)
        {
            var assets = DoLoadFromAssetDatabase(path, converter);
            AssetResources.RegisterAssets(assets);
        }

        public static void LoadFromAssetDatabase(string path, string key, Func<Object, Object> converter = null)
        {
            var assets = DoLoadFromAssetDatabase(path, converter);
            AssetResources.RegisterAssets(assets, key);
        }

        public static void LoadFromAssetDatabase(string path, Func<Object, string> keySelector,
            Func<Object, Object> converter = null)
        {
            var assets = DoLoadFromAssetDatabase(path, converter);
            foreach (var o in assets)
            {
                var key = keySelector(o);
                AssetResources.RegisterAssets(new[] { o }, key);
            }
        }

        private static Object[] DoLoadFromAssetDatabase(string path, Func<Object, Object> converter) =>
            AssetDatabase.LoadAllAssetsAtPath(path)
                .Select(converter ?? (x => x))
                .ToArray();

        public static void LoadFromBundleDeclaration(Type type, string bundleName,
            Func<Object, Object> converter = null)
        {
            var assets = DoLoadFromBundleDeclaration(type, bundleName, converter);
            AssetResources.RegisterAssets(assets);
        }

        public static void LoadFromBundleDeclaration(Type type, string bundleName, string key,
            Func<Object, Object> converter = null)
        {
            var assets = DoLoadFromBundleDeclaration(type, bundleName, converter);
            AssetResources.RegisterAssets(assets, key);
        }

        public static void LoadFromBundleDeclaration(Type type, string bundleName, Func<Object, string> keySelector,
            Func<Object, Object> converter = null)
        {
            var assets = DoLoadFromBundleDeclaration(type, bundleName, converter);
            foreach (var o in assets)
            {
                var key = keySelector(o);
                AssetResources.RegisterAssets(new[] { o }, key);
            }
        }

        private static Object[] DoLoadFromBundleDeclaration(Type type, string bundleName,
            Func<Object, Object> converter) =>
            AssetDatabase.GetAssetPathsFromAssetBundle(bundleName)
                .SelectMany(AssetDatabase.LoadAllAssetsAtPath)
                .Where(x => type == x.GetType())
                .Select(converter ?? (x => x))
                .ToArray();

        public static void LoadFromBundleDeclaration<T>(string bundleName, Func<T, Object> converter = null)
            where T : Object =>
            LoadFromBundleDeclaration(typeof(T), bundleName, o => converter?.Invoke((T)o));

        public static void LoadFromBundleDeclaration<T>(string bundleName, string key, Func<T, Object> converter = null)
            where T : Object =>
            LoadFromBundleDeclaration(typeof(T), bundleName, key, o => converter?.Invoke((T)o));

        public static void LoadFromBundleDeclaration<T>(string bundleName, Func<T, string> keySelector,
            Func<T, Object> converter = null) where T : Object =>
            LoadFromBundleDeclaration(typeof(T), bundleName, o => keySelector((T)o), o => converter?.Invoke((T)o));

        public static void LoadFromBundleDeclaration(string bundleName, Func<Object, Object> converter = null)
        {
            var assets = DoLoadFromBundleDeclaration(bundleName, converter);
            AssetResources.RegisterAssets(assets);
        }
        
        public static void LoadFromBundleDeclaration(string bundleName, string key, Func<Object, Object> converter = null)
        {
            var assets = DoLoadFromBundleDeclaration(bundleName, converter);
            AssetResources.RegisterAssets(assets, key);
        }
        
        public static void LoadFromBundleDeclaration(string bundleName, Func<Object, string> keySelector, Func<Object, Object> converter = null)
        {
            var assets = DoLoadFromBundleDeclaration(bundleName, converter);
            foreach (var o in assets)
            {
                var key = keySelector(o);
                AssetResources.RegisterAssets(new[] { o }, key);
            }
        }

        private static Object[] DoLoadFromBundleDeclaration(string bundleName, Func<Object, Object> converter) =>
            AssetDatabase.GetAssetPathsFromAssetBundle(bundleName)
                .SelectMany(AssetDatabase.LoadAllAssetsAtPath)
                .Select(converter ?? (x => x))
                .ToArray();
#endif

        #endregion

        #region Adressables

#if UNITY_ADDRESSABLE

        public static void LoadAssetFromAddressable<T>(string addressableKey, Func<T, Object> converter = null) where T : Object
        {
            var asset = DoLoadAssetFromAddressable(addressableKey, converter);
            AssetResources.RegisterAssets(new [] { asset });
        }
        
        public static void LoadAssetFromAddressable<T>(string addressableKey, string key, Func<T, Object> converter = null) where T : Object
        {
            var asset = DoLoadAssetFromAddressable(addressableKey, converter);
            AssetResources.RegisterAssets(new [] { asset }, key);
        }
        
        private static Object DoLoadAssetFromAddressable<T>(string key, Func<T, Object> converter) where T : Object
        {
            var asset = Addressables.LoadAssetAsync<T>(key).WaitForCompletion();
            return converter != null ? converter(asset) : asset;
        }

        public static AsyncOperationHandle LoadAssetFromAddressableAsync<T>(string addressableKey,
            Func<T, Object> converter = null, Action onFinished = null) where T : Object =>
            DoLoadAssetFromAddressableAsync(addressableKey, converter, onFinished, o => AssetResources.RegisterAssets(new [] { o }));
        
        public static AsyncOperationHandle LoadAssetFromAddressableAsync<T>(string addressableKey, string key,
            Func<T, Object> converter = null, Action onFinished = null) where T : Object =>
            DoLoadAssetFromAddressableAsync(addressableKey, converter, onFinished, o => AssetResources.RegisterAssets(new [] { o }, key));

        private static AsyncOperationHandle DoLoadAssetFromAddressableAsync<T>(string addressableKey, Func<T, Object> converter,
            Action onFinished, Action<Object> registerAction) where T : Object
        {
            var handle = Addressables.LoadAssetAsync<T>(addressableKey);
            handle.Completed += HandleOnCompleted;

            return handle;

            void HandleOnCompleted(AsyncOperationHandle<T> obj)
            {
                try
                {
                    if (obj.Status != AsyncOperationStatus.Succeeded)
                        throw new InvalidOperationException("Failed to load asset from addressable asset: " + addressableKey);

                    var asset = converter != null ? converter(obj.Result) : obj.Result;
                    registerAction(asset);
                }
                finally
                {
                    handle.Completed -= HandleOnCompleted;
                    onFinished?.Invoke();
                }
            }
        }

        public static void LoadAssetsFromAddressable<T>(string addressableKey, Func<T, Object> converter = null) where T : Object
        {
            var assets = DoLoadAssetsFromAddressable(addressableKey, converter);
            AssetResources.RegisterAssets(assets.Select(x => x.conv).ToArray());
        }
        
        public static void LoadAssetsFromAddressable<T>(string addressableKey, string key, Func<T, Object> converter = null) where T : Object
        {
            var assets = DoLoadAssetsFromAddressable(addressableKey, converter);
            AssetResources.RegisterAssets(assets.Select(x => x.conv).ToArray(), key);
        }

        public static void LoadAssetsFromAddressable<T>(string addressableKey, Func<T, string> keySelector, Func<T, Object> converter = null) where T : Object
        {
            var assets = DoLoadAssetsFromAddressable(addressableKey, converter);
            foreach (var o in assets)
            {
                var key = keySelector(o.orig);
                AssetResources.RegisterAssets(new [] { o.conv }, key);
            }
        }

        private static (T orig,Object conv)[] DoLoadAssetsFromAddressable<T>(string addressableKey, Func<T, Object> converter) where T : Object
        {
            var assets = Addressables.LoadAssetsAsync<T>(addressableKey).WaitForCompletion();
            return converter != null 
                ? assets.Select(x => (x, converter(x))).ToArray() 
                : assets.Select(x => (x, (Object) x)).ToArray();
        }

        public static AsyncOperationHandle LoadAssetsFromAddressableAsync<T>(string addressableKey, Func<T, Object> converter = null, Action onFinished = null) where T : Object =>
        DoLoadAssetsFromAddressableAsync(addressableKey, converter, onFinished,
            objects => AssetResources.RegisterAssets(objects.Select(x => x.conv).ToArray()));
        
        public static AsyncOperationHandle LoadAssetsFromAddressableAsync<T>(string addressableKey, string key, Func<T, Object> converter = null, Action onFinished = null) where T : Object =>
            DoLoadAssetsFromAddressableAsync(addressableKey, converter, onFinished,
                objects => AssetResources.RegisterAssets(objects.Select(x => x.conv).ToArray(), key));
        
        public static AsyncOperationHandle LoadAssetsFromAddressableAsync<T>(string addressableKey, Func<T, string> keySelector, Func<T, Object> converter = null, Action onFinished = null) where T : Object =>
            DoLoadAssetsFromAddressableAsync(addressableKey, converter, onFinished, assets =>
            {
                foreach (var o in assets)
                {
                    var key = keySelector(o.orig);
                    AssetResources.RegisterAssets(new [] { o.conv }, key);
                }
            });

        private static AsyncOperationHandle DoLoadAssetsFromAddressableAsync<T>(string addressableKey,
            Func<T, Object> converter, Action onFinished, Action<(T orig, Object conv)[]> registerAction) where T : Object
        {
            var handle = Addressables.LoadAssetsAsync<T>(addressableKey);
            handle.Completed += HandleOnCompleted;

            return handle;

            void HandleOnCompleted(AsyncOperationHandle<IList<T>> obj)
            {
                try
                {
                    if (obj.Status != AsyncOperationStatus.Succeeded)
                        throw new InvalidOperationException("Failed to load asset from addressable asset: " + addressableKey);

                    var assets = converter != null 
                        ? obj.Result.Select(x => (x, converter(x))).ToArray() 
                        : obj.Result.Select(x => (x, (Object) x)).ToArray();
                    registerAction(assets);  
                }
                finally
                {
                    handle.Completed -= HandleOnCompleted;
                    onFinished?.Invoke();  
                }
            }
        }

#endif

        #endregion
    }

    public enum AssetResourcesLoaderBundleUnload
    {
        DoNotUnload,
        Unload,
        UnloadComplete
    }
}