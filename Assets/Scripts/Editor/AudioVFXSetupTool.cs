#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using CrystalForge.Audio;
using CrystalForge.VFX;

namespace CrystalForge.Editor
{
    public static class AudioVFXSetupTool
    {
        [MenuItem("CrystalForge/4. Setup VFX & Audio Juice")]
        public static void SetupAudioAndVFX()
        {
            Undo.SetCurrentGroupName("Setup VFX & Audio Juice");
            int undoGroup = Undo.GetCurrentGroup();

            var managersObj = GameObject.Find("_Managers");
            if (managersObj == null)
            {
                managersObj = new GameObject("_Managers");
                Undo.RegisterCreatedObjectUndo(managersObj, "Create _Managers");
            }

            // 1. Gắn AudioManager
            var audioMgr = managersObj.GetComponent<AudioManager>() ?? managersObj.AddComponent<AudioManager>();

            // 2. Gắn VFXManager
            var vfxMgr = managersObj.GetComponent<VFXManager>() ?? managersObj.AddComponent<VFXManager>();

            // 3. Đảm bảo Camera có AudioListener
            var cam = Camera.main;
            if (cam != null && cam.GetComponent<AudioListener>() == null)
            {
                cam.gameObject.AddComponent<AudioListener>();
            }

            Undo.CollapseUndoOperations(undoGroup);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("<color=#00FF88><b>[CrystalForge]</b> Đã thiết lập hoàn tất VFX Particle System & Hệ thống Âm thanh Thủ tục (Procedural Audio)!</color>");
        }
    }
}
#endif
