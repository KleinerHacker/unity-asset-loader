using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using ClassicResources = UnityEngine.Resources;

namespace UnityAssetLoader.Runtime.Projects.unity_asset_loader.Scripts.Runtime
{
    public static class AssetResourcesManager
    {
        public static AssetResources Resources { get; } = new();

        #region Classic Resources

        public static AssetResourcesLoader<Object> FromResources(string path) =>
            new(() => ClassicResources.LoadAll(path));

        public static AssetResourcesLoader<Object> FromResources(Type type, string path) =>
            new(() => ClassicResources.LoadAll(path, type));

        public static AssetResourcesLoader<T> FromResources<T>(string path) where T : Object =>
            new(() => ClassicResources.LoadAll<T>(path));

        #endregion

        #region Asset Bundles

        public static AssetResourcesLoader<Object> FromBundle(string path) =>
            new(() => AssetBundle.LoadFromFile(path).LoadAllAssets());

        public static AssetResourcesLoader<Object> FromBundle(Type type, string path) =>
            new(() => AssetBundle.LoadFromFile(path).LoadAllAssets(type));

        public static AssetResourcesLoader<T> FromBundle<T>(string path) where T : Object =>
            new(() => AssetBundle.LoadFromFile(path).LoadAllAssets<T>());

        public static string[] GetScenesFromBundle(string path) =>
            AssetBundle.LoadFromFile(path).GetAllScenePaths();

        #endregion

        #region Addressables

#if UNITY_ADDRESSABLE

        public static AssetResourcesLoader<Object> FromAddressable(string label) =>
            new(() => Addressables.LoadAssetsAsync<Object>(label).WaitForCompletion().ToArray());

        public static AssetResourcesLoader<T> FromAddressable<T>(string label) where T : Object =>
            new(() => Addressables.LoadAssetsAsync<T>(label).WaitForCompletion().ToArray());

        public static void LoadSceneFromAddressable(string label, LoadSceneMode loadMode = LoadSceneMode.Single,
            SceneReleaseMode releaseMode = SceneReleaseMode.ReleaseSceneWhenSceneUnloaded, bool activateOnLoad = true,
            int priority = 100) =>
            Addressables.LoadSceneAsync(label, loadMode, releaseMode, activateOnLoad, priority).WaitForCompletion();

        public static AsyncOperationHandle LoadSceneFromAddressableAsync(string label,
            LoadSceneMode loadMode = LoadSceneMode.Single,
            SceneReleaseMode releaseMode = SceneReleaseMode.ReleaseSceneWhenSceneUnloaded, bool activateOnLoad = true,
            int priority = 100) =>
            Addressables.LoadSceneAsync(label, loadMode, releaseMode, activateOnLoad, priority);

#endif

        #endregion

        #region Asset Database

#if UNITY_EDITOR

        public static AssetResourcesLoader<Object> FromAssetDatabase(string path) =>
            new(() => AssetDatabase.LoadAllAssetsAtPath(path));

        public static AssetResourcesLoader<Object> FromAssetDatabase(Type type, string path) =>
            new(() => AssetDatabase.LoadAllAssetsAtPath(path).Where(x => type == x.GetType()).ToArray());

        public static AssetResourcesLoader<T> FromAssetDatabase<T>(string path) where T : Object =>
            new(() => AssetDatabase.LoadAllAssetsAtPath(path).OfType<T>().ToArray());

#endif

        #endregion

        #region Delegates

        internal static void RegisterAsset(Object asset, string key) =>
            Resources.RegisterAsset(asset, key);

        internal static void RegisterAsset(Object asset) =>
            Resources.RegisterAsset(asset);

#if UNITY_EDITOR

        internal static void Reset() => Resources.Reset();

#endif

        #endregion
    }
}