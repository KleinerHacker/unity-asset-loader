# Resource Asset Loader

The first variant to load assets is the classical way to load from resources. To do this, following this steps:

<procedure title="Provide assets as resource" id="provide-assets-as-resources">
<step>
    <p>Create a folder named <code>Resources</code> on any place in asset folder structure</p>
</step>
<step>
    <p>Place any assets to load manually as resource at runtime</p>
</step>
<note title="Relative path handling of resources">
    <p>Please note that all assets in any <code>Resources</code> folder starts relative with the folder root.
        It means that the assets e. g. <code>Assets/demo/Resources/a.png</code> and <code>Assets/Resources/b.png</code>
        are reached via <code>./a.png</code> and <code>./b.png</code> 
    </p>
</note>
</procedure>

## Load assets on startup of app

The first step to use a resource asset at runtime is to load this asset resource at start of app or whenever you need 
this asset. My opinion is to load this assets at app start or on the first loading screen. 

To do this, call the static method <code>LoadFromResources</code>:

<tabs>
<tab title="Synchronous">
<code-block lang="c#">
AssetResourcesLoader.LoadFromResources&lt;Sprite&gt;("my-resource-path");
</code-block>
<p>This method load the asset(s) from given resource path by type. While loading the code stops here.</p>
</tab>
<tab title="Asynchronous">
<code-block lang="c#">
AssetResourcesLoader.LoadFromResourcesAsync&lt;Sprite&gt;("my-resource-path", 
    () => Debug.WriteLine("Finished"));
</code-block>
<p>This method load the asset(s) from given resource path by type. The main thread does not stop here. 
Also if loading is finished the callback method is invoked.</p>
</tab>
</tabs>

<note>
    <p>This method do not return any object. This method only loads the assets into memory to can access any time in code.</p>
</note>

## Get loaded assets from cache
<include from="access-asset-loader.md" element-id="x"/>