using UnityEngine;

namespace TheLegends.Base.Databuckets
{
    public class DatabucketsSettings : ScriptableObject
    {
        public const string ResDir = "Assets/TripSoft/Databuckets/Resources";
        public const string FileName = "DatabucketsSettingsAsset";
        public const string FileExtension = ".asset";

        private static DatabucketsSettings _instance;
        public static DatabucketsSettings Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = Resources.Load<DatabucketsSettings>(FileName);
                return _instance;
            }
        }

        public string api_endpoint = "";
        public string api_key = "";

        public string APIEndpoint
        {
            get { return api_endpoint; }
            set { api_endpoint = value; }
        }

        public string APIKey
        {
            get { return api_key; }
            set { api_key = value; }
        }

        public bool enableExxceptionLogTracking = true;
    }
}