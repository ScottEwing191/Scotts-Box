using UnityEditor;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
public static class CanvasPickingEditor
{
    [MenuItem("Tools/Scott's Box/Enable Picking Canvases")]
    private static void EnablePickingCanvases()
    {
        var canvases = GameObject.FindObjectsOfType<Canvas>().Select(x => x.gameObject).ToArray();
        SceneVisibilityManager.instance.EnablePicking(canvases, true);
    }

    [MenuItem("Tools/Scott's Box/Disable Picking Canvases")]
    private static void DisablePickingCanvases()
    {
        var canvases = GameObject.FindObjectsOfType<Canvas>().Select(x => x.gameObject).ToArray();
        SceneVisibilityManager.instance.DisablePicking(canvases, true);
    }
}
#endif