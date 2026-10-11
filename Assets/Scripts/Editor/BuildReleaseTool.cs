#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace CrystalForge.Editor
{
    public static class BuildReleaseTool
    {
        [MenuItem("CrystalForge/5. Build Windows Executable (.EXE)")]
        public static void BuildWindowsExecutable()
        {
            string buildDirectory = "Builds/CrystalForge_Release";
            if (!Directory.Exists(buildDirectory))
            {
                Directory.CreateDirectory(buildDirectory);
            }

            string exePath = Path.Combine(buildDirectory, "CrystalForge.exe");
            string[] scenes = { "Assets/Scenes/SampleScene.unity" };

            Debug.Log($"<color=#2EA3FF><b>[CrystalForge]</b> Bắt đầu đóng gói bản dựng Windows x64: {exePath}...</color>");

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = exePath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"<color=#00FF88><b>[CrystalForge]</b> Đóng gói thành công! Dung lượng: {summary.totalSize / (1024 * 1024):0.##} MB. File: {exePath}</color>");
            }
            else if (summary.result == BuildResult.Failed)
            {
                Debug.LogError($"<color=#FF3344><b>[CrystalForge]</b> Đóng gói thất bại với {summary.totalErrors} lỗi!</color>");
            }
        }
    }
}
#endif
