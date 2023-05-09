using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JsonUtility = VoxelBusters.Parser.JsonUtility;

namespace VoxelBusters.CoreLibrary.Helpers
{
    public class DefaultJsonServiceProvider : IJsonServiceProvider
    {
        #region Static methods

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnLoad()
        {
            if (ExternalServiceProvider.JsonServiceProvider != null)
            {
                return;
            }

            // set default service provider
            ExternalServiceProvider.JsonServiceProvider = new DefaultJsonServiceProvider();
        }

        #endregion

        #region IJsonServiceProvider implementation

        public string ToJson(object obj)
        {
            return JsonUtility.ToJSON(obj);
        }

        public object FromJson(string jsonString)
        {
            return JsonUtility.FromJSON(jsonString);
        }

        #endregion
    }
}