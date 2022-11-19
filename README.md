# unity-asset-loader
Unity extension to give an easy way to import assets from different sources.

# install
Add this repository directly into Unity.

### Open UPM
URL: https://package.openupm.com

Scope: org.pcsoft

# usage

### Types of Loader

* Use the class `AssetResourcesLoader` to load assets from a source. For support in editor play there are
multiple methods to load assets in editor only.
* Use class `AassetResources` to get the loaded assets from memory.

First the asset you want to use must load initial:
```c#
//Load all assets of given type from given path
AssetResourcesLoader.LoadFromResources<AssetType>(AssetPath);

//Load all assets of given type from asset bundles found in given path
AssetResourcesLoader.LoadFromBundle<AssetType>(BundlePath);

//Load all assets of given type from asset database in given path, use it only in Editor script parts!
AssetResourcesLoader.LoadFromAssetDatbase<AssetType>(ProjectPath);

//Load all assets of given type from asset database bundle declaration by name; only for Editor
AssetResourcesLoader.LoadFromBundleDeclaration<AssetType>(BundleName);
```

Second you can access cached asset(s) via loader:
```c#
AssetResources.GetAsset<AssetType>();
AssetResources.GetAssets<AssetType>();
```
