<snippet id="x">

The second step is to get assets from internal asset cache. All assets were loaded into memory and can access any time
without any great access time.

<tabs>
<tab title="Single Asset Access">
<code-block lang="c#">
var sprite = AssetResources.GetAsset&lt;Sprite&gt;();
</code-block>
<p>This method returns the first sprite in the asset cache.</p>
</tab>
<tab title="Multiple Asset Access">
<code-block lang="c#">
var sprites = AssetResources.GetAssets&lt;Sprite&gt;();
</code-block>
<p>This method returns all sprites in the asset cache.</p>
</tab>
</tabs>

<note>
<p>You can access these assets any time without great access time.</p>
</note>
</snippet>