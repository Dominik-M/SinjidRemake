// FindUnusedAssetsAdvanced.cs
// Editor tool that scans the project for assets that are not referenced by any other asset.
// - Builds a reverse-reference map by iterating all assets and asking for their dependencies
// - Shows reference count and which assets reference a given asset
// - Sorts results by path and groups by folder in the UI
// - Provides Delete button to remove assets directly from results
// - Warns about limitations (Resources.Load, Addressables, code-based loads)

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FindUnusedAssetsAdvanced : EditorWindow
{
    private Vector2 scrollPos;
    private List<AssetInfo> results = new List<AssetInfo>();

    // options
    private bool skipMetaFiles = true;
    private bool showOnlyUnused = true;
    private string[] ignoreExtensions = new string[] { ".cs", ".dll", ".asmdef", ".csproj", ".sln", ".user" };

    [MenuItem("Tools/Find Unused Assets (Advanced)")]
    public static void ShowWindow()
    {
        GetWindow<FindUnusedAssetsAdvanced>("Find Unused Assets");
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Scan Project", GUILayout.Height(30)))
        {
            ScanProject();
        }
        if (GUILayout.Button("Clear", GUILayout.Height(30)))
        {
            results.Clear();
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        showOnlyUnused = EditorGUILayout.ToggleLeft("Show only assets with 0 references (unused)", showOnlyUnused);
        skipMetaFiles = EditorGUILayout.ToggleLeft("Skip .meta files during scan", skipMetaFiles);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField($"Results: {results.Count}", EditorStyles.boldLabel);

        scrollPos = GUILayout.BeginScrollView(scrollPos);

        string currentFolder = null;
        for (int i = 0; i < results.Count; i++)
        {
            var ai = results[i];

            if (showOnlyUnused && ai.ReferenceCount > 0)
                continue; // only show unused if requested

            string folder = Path.GetDirectoryName(ai.Path);
            if (folder != currentFolder)
            {
                currentFolder = folder;
                EditorGUILayout.LabelField(folder, EditorStyles.boldLabel);
            }

            EditorGUILayout.BeginHorizontal();

            // short name + path
            EditorGUILayout.LabelField(Path.GetFileName(ai.Path), GUILayout.Width(240));
            EditorGUILayout.LabelField(ai.Path, GUILayout.ExpandWidth(true));

            // reference count
            EditorGUILayout.LabelField(ai.ReferenceCount.ToString(), GUILayout.Width(60));

            if (GUILayout.Button("Select", GUILayout.Width(60)))
            {
                var obj = AssetDatabase.LoadAssetAtPath<Object>(ai.Path);
                Selection.activeObject = obj;
                EditorGUIUtility.PingObject(obj);
            }

            if (GUILayout.Button("Delete", GUILayout.Width(60)))
            {
                if (EditorUtility.DisplayDialog("Delete Asset", $"Are you sure you want to delete {ai.Path}?", "Delete", "Cancel"))
                {
                    bool ok = AssetDatabase.DeleteAsset(ai.Path);
                    if (ok)
                    {
                        results.RemoveAt(i);
                        i--; // adjust index after removal
                        AssetDatabase.Refresh();
                        continue;
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Delete Failed", $"Could not delete {ai.Path}", "OK");
                    }
                }
            }

            if (ai.ReferenceCount > 0)
            {
                if (GUILayout.Button("Show Refs", GUILayout.Width(90)))
                {
                    RefsWindow.ShowWindow(ai.ReferencedBy);
                }
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }

    private void ScanProject()
    {
        results.Clear();

        string[] allPaths = AssetDatabase.GetAllAssetPaths()
            .Where(p => p.StartsWith("Assets/"))
            .ToArray();

        // map: assetPath -> set of assets that reference that asset
        var referencedBy = new Dictionary<string, HashSet<string>>();

        int total = allPaths.Length;

        for (int i = 0; i < total; i++)
        {
            string source = allPaths[i];

            // optionally skip meta files as sources
            if (skipMetaFiles && source.EndsWith(".meta"))
                continue;

            // skip folders
            if (AssetDatabase.IsValidFolder(source))
                continue;

            // progress bar
            if (i % 30 == 0)
                EditorUtility.DisplayProgressBar("Find Unused Assets", $"Scanning ({i}/{total})\n{source}", (float)i / total);

            string[] deps = AssetDatabase.GetDependencies(source, true);
            foreach (var dep in deps)
            {
                if (!dep.StartsWith("Assets/"))
                    continue;

                if (skipMetaFiles && dep.EndsWith(".meta"))
                    continue;

                if (dep == source)
                    continue;

                if (!referencedBy.TryGetValue(dep, out var set))
                {
                    set = new HashSet<string>();
                    referencedBy[dep] = set;
                }
                set.Add(source);
            }
        }

        EditorUtility.ClearProgressBar();

        string projectRoot = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);

        foreach (var path in allPaths)
        {
            if (skipMetaFiles && path.EndsWith(".meta"))
                continue;

            if (AssetDatabase.IsValidFolder(path))
                continue;

            string ext = Path.GetExtension(path).ToLowerInvariant();
            if (ignoreExtensions.Contains(ext))
                continue;

            referencedBy.TryGetValue(path, out var refs);

            long fileSize = 0;
            try
            {
                string fullPath = Path.Combine(projectRoot, path.Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(fullPath))
                    fileSize = new FileInfo(fullPath).Length;
            }
            catch { }

            results.Add(new AssetInfo
            {
                Path = path,
                ReferenceCount = refs != null ? refs.Count : 0,
                ReferencedBy = refs != null ? refs.ToList() : new List<string>(),
                FileSize = fileSize
            });
        }

        results = results.OrderBy(r => r.Path).ToList();

        Debug.Log($"FindUnusedAssets: Scan finished. {results.Count} assets scanned.");
    }

    private class AssetInfo
    {
        public string Path;
        public int ReferenceCount;
        public List<string> ReferencedBy;
        public long FileSize;
    }

    public class RefsWindow : EditorWindow
    {
        private Vector2 scroll;
        private List<string> refs = new List<string>();

        public static void ShowWindow(List<string> references)
        {
            var win = CreateInstance<RefsWindow>();
            win.refs = references;
            win.titleContent = new GUIContent("References");
            win.position = new Rect(100, 100, 600, 300);
            win.ShowUtility();
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField($"References ({refs.Count})", EditorStyles.boldLabel);
            scroll = GUILayout.BeginScrollView(scroll);
            foreach (var r in refs)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(r, GUILayout.ExpandWidth(true));
                if (GUILayout.Button("Select", GUILayout.Width(80)))
                {
                    var obj = AssetDatabase.LoadAssetAtPath<Object>(r);
                    Selection.activeObject = obj;
                    EditorGUIUtility.PingObject(obj);
                }
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();

            if (GUILayout.Button("Close")) this.Close();
        }
    }

    // NOTE / Limitations:
    // - Assets loaded only via Resources.Load, Addressables or via reflection/dynamic paths will NOT be detected as referenced.
    // - Addressables and AssetBundles are not scanned here (you would need to analyze their catalogs/labels separately).
    // - Always keep a backup or use version control before deleting anything reported as unused.
}