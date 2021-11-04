# unity-asset-loader
Unity extension to give an easy way to import assets from different sources.

# install
Add this repository directly into Unity.

# usage

### Types of Loader
There are three loaders:
* **Resource Loader** `AssetResourcesLoader` - Load assets from "Resources" folder(s).
* **Bundle Loader** `AssetBundleLoader` - Load assets from an asset bundle file.
* **Editor Loader** `AssetEditorLoader` - Load assets in Editor mode via Asset Database.

First the asset you want to use must load initial:
```c#
//Load all assets of given type from given path
AssetResourcesLoader.Instance.LoadAssets<AssetType>(AssetPath);

//Load all assets of given type from asset bundles found in given path
AssetBundleLoader.Instance.LoadAssets<AssetType>(BundlePath);

//Load all assets of given type from asset database in given path, use it only in Editor script parts!
AssetEditorLoader.Instance.LoadAssets<AssetType>(ProjectPath);
```

Second you can access cached asset(s) via loader:
```c#
AssetResourcesLoader.Instance.GetAsset<AssetType>();
AssetResourcesLoader.Instance.GetAssets<AssetType>();

AssetBundleLoader.Instance.GetAsset<AssetType>();
AssetBundleLoader.Instance.GetAssets<AssetType>();

AssetEditorLoader.Instance.GetAsset<AssetType>();
AssetEditorLoader.Instance.GetAssets<AssetType>();
```
