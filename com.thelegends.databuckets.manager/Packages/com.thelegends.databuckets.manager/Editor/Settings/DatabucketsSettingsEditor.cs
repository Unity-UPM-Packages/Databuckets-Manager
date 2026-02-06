using System.IO;
using UnityEditor;
using UnityEngine;

namespace TheLegends.Base.Databuckets
{
    [CustomEditor(typeof(DatabucketsSettings))]
    public class DatabucketsSettingsEditor : Editor
    {
        private static DatabucketsSettings instance = null;

        public static DatabucketsSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<DatabucketsSettings>(DatabucketsSettings.FileName);
                }

                if (instance != null)
                {
                    Selection.activeObject = instance;
                }
                else
                {
                    Directory.CreateDirectory(DatabucketsSettings.ResDir);

                    instance = CreateInstance<DatabucketsSettings>();

                    string assetPath = Path.Combine(DatabucketsSettings.ResDir, DatabucketsSettings.FileName);
                    string assetPathWithExtension = Path.ChangeExtension(assetPath, DatabucketsSettings.FileExtension);
                    AssetDatabase.CreateAsset(instance, assetPathWithExtension);
                    AssetDatabase.SaveAssets();
                }

                return instance;
            }
        }

        [MenuItem("TripSoft/Databuckets Settings")]
        public static void OpenInspector()
        {
            if (Instance == null)
            {
                Debug.Log("Creat new Databuckets Settings");
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            Instance.APIEndpoint = EditorGUILayout.TextField("API Endpoint", Instance.APIEndpoint);
            Instance.APIKey = EditorGUILayout.TextField("API Key", Instance.APIKey);
            Instance.enableExxceptionLogTracking = EditorGUILayout.Toggle("Enable Exception Log Tracking", Instance.enableExxceptionLogTracking);

            if (EditorGUI.EndChangeCheck())
            {
                Save((DatabucketsSettings)target);
            }
        }

        private void Save(Object target)
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
