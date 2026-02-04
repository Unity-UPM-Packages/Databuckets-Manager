using System;
using System.Collections.Generic;
using Databuckets;
using TheLegends.Base.UnitySingleton;

namespace TheLegends.Base.Databuckets
{
    public class DatabucketsManager : PersistentMonoSingleton<DatabucketsManager>
    {
        public void Init()
        {
#if USE_DATABUCKETS
            DatabucketsTracker.Init(DatabucketsSettings.Instance.APIEndpoint, DatabucketsSettings.Instance.APIKey);

            if (DatabucketsSettings.Instance.enableExxceptionLogTracking)
            {
                DatabucketsTracker.EnableExceptionLogTracking();
            }
            else
            {
                DatabucketsTracker.DisableExceptionLogTracking();
            }

            LoadCommonPropertiesFromPrefs();
#endif
        }

        public void SetCommonProperty(string key, object value)
        {
#if USE_DATABUCKETS
            DatabucketsTracker.SetCommonProperty(key, value);
            UpdateCachedProperty(key, value);
#endif
        }

        public void SetCommonProperties(Dictionary<string, object> properties)
        {
#if USE_DATABUCKETS
            DatabucketsTracker.SetCommonProperties(properties);
            foreach (var property in properties)
            {
                UpdateCachedProperty(property.Key, property.Value);
            }
#endif
        }

        public void LoadCommonPropertiesFromPrefs()
        {
#if USE_DATABUCKETS
            var properties = new Dictionary<string, object>();
            var fields = typeof(DefaultProperties).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy);

            foreach (var field in fields)
            {
                if (field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(string))
                {
                    string key = (string)field.GetValue(null);
                    if (key == DefaultProperties.CURRENT_LEVEL || key.EndsWith("_n"))
                    {
                        properties.Add(key, UnityEngine.PlayerPrefs.GetInt(key, -1));
                    }
                    else
                    {
                        properties.Add(key, UnityEngine.PlayerPrefs.GetString(key, ""));
                    }
                }
            }

            if (properties.Count > 0)
            {
                DatabucketsTracker.SetCommonProperties(properties);
            }
#endif
        }

        private void UpdateCachedProperty(string key, object value)
        {
#if USE_DATABUCKETS
            if (key == DefaultProperties.CURRENT_LEVEL || key.EndsWith("_n"))
            {
                UnityEngine.PlayerPrefs.SetInt(key, Convert.ToInt32(value));
            }
            else
            {
                UnityEngine.PlayerPrefs.SetString(key, value?.ToString() ?? "");
            }

            UnityEngine.PlayerPrefs.Save();
#endif
        }

        public void RecordEvent(string eventName, Dictionary<string, object> properties = null)
        {
#if USE_DATABUCKETS
            DatabucketsTracker.Record(eventName, properties);
#endif
        }

        public void RecordEventWithTiming(string eventName, Dictionary<string, object> properties, string timingProp, string startEvent)
        {
#if USE_DATABUCKETS
            DatabucketsTracker.RecordWithTiming(eventName, properties, timingProp, startEvent);
#endif
        }

    }
}
