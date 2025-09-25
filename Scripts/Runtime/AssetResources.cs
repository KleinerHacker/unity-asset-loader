using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace UnityAssetLoader.Runtime.Projects.unity_asset_loader.Scripts.Runtime
{
    public static class AssetResources
    {
        private static readonly IDictionary<Type, IList<Object>> DefaultResources = new Dictionary<Type, IList<Object>>();
        private static readonly IDictionary<string, IDictionary<Type, IList<Object>>> Resources = new Dictionary<string, IDictionary<Type, IList<Object>>>();

        public static Object GetAsset(Type type) => GetAsset(type, DefaultResources);

        public static Object GetAsset(Type type, string key)
        {
            if (!Resources.TryGetValue(key, out var resources))
                return null;
            
            return GetAsset(type, resources);
        }

        private static Object GetAsset(Type type, IDictionary<Type, IList<Object>> resources)
        {
            if (!resources.TryGetValue(type, out var list))
                return null;

            if (list.Count <= 0)
                return null;

            return list[0];
        }

        public static Object[] GetAssets(Type type) => GetAssets(type, DefaultResources);

        public static Object[] GetAssets(Type type, string key)
        {
            if (!Resources.TryGetValue(key, out var resources))
                return Array.Empty<Object>();
            
            return GetAssets(type, resources);
        }

        private static Object[] GetAssets(Type type, IDictionary<Type, IList<Object>> resources)
        {
            if (!resources.TryGetValue(type, out var list))
                return Array.Empty<Object>();

            return list.ToArray();
        }

        public static T GetAsset<T>() where T : Object => (T)GetAsset(typeof(T));
        
        public static T GetAsset<T>(string key) where T : Object => (T)GetAsset(typeof(T), key);

        public static T[] GetAssets<T>() where T : Object => GetAssets(typeof(T)).Cast<T>().ToArray();
        
        public static T[] GetAssets<T>(string key) where T : Object => GetAssets(typeof(T), key).Cast<T>().ToArray();

        internal static void RegisterAssets(Object[] objects) => RegisterAssets(objects, DefaultResources);

        internal static void RegisterAssets(Object[] objects, string key)
        {
            if (!Resources.ContainsKey(key))
            {
                Resources.Add(key, new Dictionary<Type, IList<Object>>());
            }
            
            RegisterAssets(objects, Resources[key]);
        }

        private static void RegisterAssets(Object[] objects, IDictionary<Type, IList<Object>> resources)
        {
            foreach (var o in objects)
            {
                if (!resources.ContainsKey(o.GetType()))
                {
                    resources.Add(o.GetType(), new List<Object>());
                }
                
                resources[o.GetType()].Add(o);
            }
        }
        
        #if UNITY_EDITOR

        internal static void Reset()
        {
            Resources.Clear();
            DefaultResources.Clear();
        }
        
        #endif
    }
}