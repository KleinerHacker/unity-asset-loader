using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace UnityAssetLoader.Runtime.asset_loader.Scripts.Runtime
{
    public static class AssetResources
    {
        private static readonly IDictionary<Type, IList<Object>> Resources = new Dictionary<Type, IList<Object>>();

        public static Object GetAsset(Type type)
        {
            if (!Resources.ContainsKey(type))
                return null;

            var list = Resources[type];
            if (list.Count <= 0)
                return null;

            return list[0];
        }

        public static Object[] GetAssets(Type type)
        {
            if (!Resources.ContainsKey(type))
                return null;

            return Resources[type].ToArray();
        }

        public static T GetAsset<T>() where T : Object
        {
            return (T)GetAsset(typeof(T));
        }

        public static T[] GetAssets<T>() where T : Object
        {
            return GetAssets(typeof(T)).Cast<T>().ToArray();
        }

        internal static void RegisterAssets(Object[] objects)
        {
            foreach (var o in objects)
            {
                if (!Resources.ContainsKey(o.GetType()))
                {
                    Resources.Add(o.GetType(), new List<Object>());
                }
                
                Resources[o.GetType()].Add(o);
            }
        }
    }
}