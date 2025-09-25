using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityAssetLoader.Runtime.Projects.unity_asset_loader.Scripts.Runtime
{
    public static class AssetResourcesManager
    {
        public static AssetResourcesLoaderBuilder<Object> FromResources(Type type, string path) =>
            new(() => Resources.LoadAll(path, type));
    }

    public abstract class AssetResourcesLoaderBuilder
    {
        protected struct Asset<T>
        {
            public string Key { get; }
            public T Object { get; }

            public Asset(string key, T o)
            {
                Key = key;
                Object = o;
            }
        }
    }

    public sealed class AssetResourcesLoaderBuilder<T> : AssetResourcesLoaderBuilder where T : Object
    {
        //Loading action with a key selector to use for
        private readonly Func<Func<T, string>, Asset<T>[]> _loadingAction;
        //Current set key selector
        private readonly Func<T, string> _keySelector = _ => null;

        internal AssetResourcesLoaderBuilder(Func<T[]> loadingAction)
        {
            _loadingAction = (keySelector) => loadingAction().Select(o => new Asset<T>(keySelector(o), o)).ToArray();
        }

        private AssetResourcesLoaderBuilder(Func<Func<T, string>, Asset<T>[]> loadingAction, Func<T, string> keySelector = null)
        {
            _loadingAction = loadingAction;
            _keySelector = keySelector ?? (_ => null);
        }

        /// <summary>
        /// Set up a key to use for all loaded assets
        /// </summary>
        public AssetResourcesLoaderBuilder<T> WithKey(string key) =>
            new(_loadingAction, _ => key);

        /// <summary>
        /// Set up a key selector to define separate keys for each loaded asset
        /// </summary>
        public AssetResourcesLoaderBuilder<T> WithKeySelector(Func<T, string> keySelector) =>
            new(_loadingAction, keySelector);

        /// <summary>
        /// Set up a converter to convert loaded assets to another type
        /// </summary>
        public AssetResourcesLoaderBuilder<TResult> WithConverter<TResult>(Func<T, TResult> converter)
            where TResult : Object => 
            //In lambda run original loading action with current key selector and convert the result
            //with help of new key selector given by the converter (type fit) 
            new(keySelector => _loadingAction(_keySelector).Select(o => new Asset<TResult>(keySelector(converter(o.Object)) ?? o.Key , converter(o.Object))).ToArray());

        /// <summary>
        /// Load all assets and register them with the AssetResources
        /// </summary>
        /// <param name="visitor">An optional visitor for the key / asset value pair</param>
        public int Load(Action<string, T> visitor = null)
        {
            var assets = _loadingAction(_keySelector);
            foreach (var asset in assets)
            {
                visitor?.Invoke(asset.Key, asset.Object);
                
                if (!string.IsNullOrEmpty(asset.Key))
                {
                    AssetResources.RegisterAssets(new[] { asset.Object }.ToArray<Object>(), asset.Key);
                }
                else
                {
                    AssetResources.RegisterAssets(new[] { asset.Object }.ToArray<Object>());
                }
            }
            
            return assets.Length;
        }
    }
}