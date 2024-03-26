using UnityEngine;
using System.IO;

public class CreateAssetBundles 
{
    [UnityEditor.MenuItem ("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles ()
    {
        string assetBundleDirectory = "Assets/StreamingAssets";
        if (!Directory.Exists(Application.streamingAssetsPath))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        //BuildPipeline.BuildAssetBundles ("Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneOSX);
        UnityEditor.BuildPipeline.BuildAssetBundles(assetBundleDirectory, UnityEditor.BuildAssetBundleOptions.None, UnityEditor.EditorUserBuildSettings.activeBuildTarget);

    }
}
