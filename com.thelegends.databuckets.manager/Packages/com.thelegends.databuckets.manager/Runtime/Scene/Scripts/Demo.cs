using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TheLegends.Base.Databuckets
{
    public class Demo : MonoBehaviour
    {
        public Button initBtn;
        public Button updatePropsBtn;
        public Button startEventBtn;
        public Button endEventBtn;

        void OnEnable()
        {
            initBtn.onClick.AddListener(InitDatabuckets);
            updatePropsBtn.onClick.AddListener(UpdateProperties);
            startEventBtn.onClick.AddListener(StartEvent);
            endEventBtn.onClick.AddListener(EndEvent);
        }

        private void EndEvent()
        {
            DatabucketsManager.Instance.RecordEventWithTiming("end", new Dictionary<string, object>()
            {
                { "demo_end_time", DateTime.UtcNow.ToString("o") }
            }, "demo_duration", "start");
        }

        private void StartEvent()
        {
            DatabucketsManager.Instance.RecordEvent("start", new Dictionary<string, object>()
            {
                { "start", 1 }
            });
        }

        public void InitDatabuckets()
        {
            DatabucketsManager.Instance.Init();
        }
        
        public void UpdateProperties()
        {
            DatabucketsManager.Instance.SetCommonProperty(DefaultProperties.CURRENT_LEVEL, 5);
            DatabucketsManager.Instance.SetCommonProperty(DefaultProperties.IS_IAP_USER, 1);
        }
    }
}
