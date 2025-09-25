using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace UnityAssetLoader.Runtime.Projects.unity_asset_loader.Scripts.Runtime
{
    public sealed class AssetResources
    {
        private readonly IDictionary<Type, IList<Object>> DefaultResources = new Dictionary<Type, IList<Object>>();

        private readonly IDictionary<string, IDictionary<Type, IList<Object>>> Resources =
            new Dictionary<string, IDictionary<Type, IList<Object>>>();

        public Object GetAsset(Type type) => GetAsset(type, DefaultResources);

        public Object GetAsset(Type type, string key)
        {
            if (!Resources.TryGetValue(key, out var resources))
                return null;

            return GetAsset(type, resources);
        }

        private Object GetAsset(Type type, IDictionary<Type, IList<Object>> resources)
        {
            if (!resources.TryGetValue(type, out var list))
                return null;

            if (list.Count <= 0)
                return null;

            return list[0];
        }

        public Object[] GetAssets(Type type) => GetAssets(type, DefaultResources);

        public Object[] GetAssets(Type type, string key)
        {
            if (!Resources.TryGetValue(key, out var resources))
                return Array.Empty<Object>();

            return GetAssets(type, resources);
        }

        private Object[] GetAssets(Type type, IDictionary<Type, IList<Object>> resources)
        {
            if (!resources.TryGetValue(type, out var list))
                return Array.Empty<Object>();

            return list.ToArray();
        }

        public T GetAsset<T>() where T : Object => (T)GetAsset(typeof(T));

        public T GetAsset<T>(string key) where T : Object => (T)GetAsset(typeof(T), key);

        public T[] GetAssets<T>() where T : Object => GetAssets(typeof(T)).Cast<T>().ToArray();

        public T[] GetAssets<T>(string key) where T : Object => GetAssets(typeof(T), key).Cast<T>().ToArray();

        internal void RegisterAsset(Object o) => RegisterAsset(o, DefaultResources);

        internal void RegisterAsset(Object o, string key)
        {
            if (!Resources.ContainsKey(key))
            {
                Resources.Add(key, new Dictionary<Type, IList<Object>>());
            }

            RegisterAsset(o, Resources[key]);
        }

        private void RegisterAsset(Object o, IDictionary<Type, IList<Object>> resources)
        {
            if (!resources.ContainsKey(o.GetType()))
            {
                resources.Add(o.GetType(), new List<Object>());
            }

            resources[o.GetType()].Add(o);
        }

#if UNITY_EDITOR

        internal void Reset()
        {
            Resources.Clear();
            DefaultResources.Clear();
        }

#endif
    }
}