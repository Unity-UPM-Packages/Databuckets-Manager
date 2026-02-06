using System;
using System.Collections.Generic;
using System.Text;
using Databuckets;
using TheLegends.Base.UnitySingleton;
using TheLegends.Unity.Utils;
using UnityEngine;

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
            CalculateRetentionAndActiveDays();
            InvokeRepeating(nameof(CheckConnection), 0f, 10f);

            Log("Databuckets Initialized");
#endif
        }

        public void SetCommonProperty(string key, object value)
        {
#if USE_DATABUCKETS
            DatabucketsTracker.SetCommonProperty(key, value);
            UpdateCachedProperty(key, value);

            Log("Property Set: " + key + " = " + value);
#endif
        }

        public void SetCommonProperties(Dictionary<string, object> properties)
        {
#if USE_DATABUCKETS
            DatabucketsTracker.SetCommonProperties(properties);
            foreach (var property in properties)
            {
                UpdateCachedProperty(property.Key, property.Value);

                Log("Property Set: " + property.Key + " = " + property.Value);
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
                        properties.Add(key, PlayerPrefs.GetInt(key, 0));
                    }
                    else
                    {
                        properties.Add(key, PlayerPrefs.GetString(key, "Not Available"));
                    }
                }
            }

            if (properties.Count > 0)
            {
                SetCommonProperties(properties);
            }
#endif
        }

        private void UpdateCachedProperty(string key, object value)
        {
#if USE_DATABUCKETS
            if (key == DefaultProperties.CURRENT_LEVEL || key.EndsWith("_n"))
            {
                PlayerPrefs.SetInt(key, Convert.ToInt32(value));
            }
            else
            {
                PlayerPrefs.SetString(key, value?.ToString() ?? "");
            }

            PlayerPrefs.Save();
#endif
        }

        public void RecordEvent(string eventName, Dictionary<string, object> parameters = null)
        {
#if USE_DATABUCKETS
            DatabucketsTracker.Record(eventName, parameters);

            string paramStr = GetParamsStr(parameters);
            Log("Event Recorded: " + eventName + " | Parameters: " + paramStr);
#endif
        }

        public void RecordEventWithTiming(string eventName, Dictionary<string, object> parameters, string timingProp, string startEvent)
        {
#if USE_DATABUCKETS
            DatabucketsTracker.RecordWithTiming(eventName, parameters, timingProp, startEvent);

            string paramStr = GetParamsStr(parameters);
            Log("Event With Timing Recorded: " + eventName + " | Timing Property: " + timingProp + " | Start Event: " + startEvent + " | Parameters: " + paramStr);
#endif
        }

        private void CalculateRetentionAndActiveDays()
        {
#if USE_DATABUCKETS

            string lastActiveDateStr = PlayerPrefs.GetString("last_active_date", "");

            if ((PlayerPrefs.GetInt(DefaultProperties.RETENTION_DAY) == 0) && (PlayerPrefs.GetInt(DefaultProperties.RETENTION_DAY) == 0) && string.IsNullOrEmpty(lastActiveDateStr))
            {
                PlayerPrefs.SetString("last_active_date", DateTime.UtcNow.Date.ToString("o"));
                PlayerPrefs.Save();

                return;
            }

            DateTime lastActiveDate = DateTime.MinValue;
            DateTime currentDate = DateTime.UtcNow.Date;


            if (!string.IsNullOrEmpty(lastActiveDateStr))
            {
                DateTime.TryParse(lastActiveDateStr, out lastActiveDate);
            }

            var days = currentDate.Day - lastActiveDate.Day;
            if (days >= 1)
            {
                int activeDays = PlayerPrefs.GetInt(DefaultProperties.ACTIVE_DAY, 0);
                activeDays += 1;
                SetCommonProperty(DefaultProperties.ACTIVE_DAY, activeDays);
                SetCommonProperty(DefaultProperties.RETENTION_DAY, PlayerPrefs.GetInt(DefaultProperties.RETENTION_DAY) + days);

                PlayerPrefs.SetString("last_active_date", currentDate.ToString("o"));
                PlayerPrefs.Save();
            }
#endif
        }


        public async void CheckConnection()
        {
            ConnectionsUtils.GetConnectionType((connectionType) =>
            {
                switch (connectionType)
                {
                    case EConnectionsType.OFFLINE:
                        SetCommonProperty(DefaultProperties.CONNECTION_TYPE, EConnectionsType.OFFLINE.ToString());
                        break;
                    case EConnectionsType.WIFI:
                        SetCommonProperty(DefaultProperties.CONNECTION_TYPE, EConnectionsType.WIFI.ToString());
                        break;
                    case EConnectionsType.MOBILE_DATA:
                        SetCommonProperty(DefaultProperties.CONNECTION_TYPE, EConnectionsType.MOBILE_DATA.ToString());
                        break;
                    default:
                        SetCommonProperty(DefaultProperties.CONNECTION_TYPE, EConnectionsType.UNKNOWN.ToString());
                        break;
                }
            });
        }

        #region Logging
        public void Log(string message)
        {
            Debug.Log("Databuckets------: " + message);
        }

        public void LogWarning(string message)
        {
            Debug.LogWarning("Databuckets------: " + message);
        }

        public void LogError(string message)
        {
            Debug.LogError("Databuckets------: " + message);
        }

        public void LogException(Exception exception)
        {
            Debug.LogException(exception);
        }

        private string GetParamsStr(Dictionary<string, object> parameters)
        {
            if (parameters == null || parameters.Count == 0)
            {
                return "No Parameters";
            }

            var sb = new StringBuilder();

            foreach (var param in parameters)
            {
                sb.Append(param.Key)
                  .Append(": ")
                  .Append(param.Value)
                  .Append(", ");
            }

            return sb.ToString();
        }

        #endregion

    }
}
