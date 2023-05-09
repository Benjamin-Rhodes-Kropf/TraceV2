using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace VoxelBusters.CoreLibrary.Editor.NativePlugins.Build
{
    public class BuildProcessReporter : IActiveBuildTargetChanged, IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        #region Delegates

        public delegate void BuildTargetChangeCallback(BuildTarget previousTarget, BuildTarget newTarget);

        public delegate void BuildProcessCallback(BuildReport report);

        #endregion

        #region Static events

        public static event BuildTargetChangeCallback OnBuildTargetChange;

        public static event BuildProcessCallback OnPreprocessBuild;

        public static event BuildProcessCallback OnPostprocessBuild;
        
        #endregion

        #region IActiveBuildTargetChanged implementation

        int IOrderedCallback.callbackOrder
        {
            get
            {
                return 99;
            }
        }

        void IActiveBuildTargetChanged.OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        {
            // notify listeners
            if (OnBuildTargetChange != null)
            {
                OnBuildTargetChange(previousTarget, newTarget);
            }
        }

        #endregion

        #region IPreprocessBuildWithReport implementation

        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
        {
            // notify listeners
            if (OnPreprocessBuild != null)
            {
                OnPreprocessBuild(report);
            }
        }

        #endregion

        #region IPostprocessBuildWithReport implementation

        void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
        {
            // notify listeners
            if (OnPostprocessBuild != null)
            {
                OnPostprocessBuild(report);
            }
        }

        #endregion
    }
}