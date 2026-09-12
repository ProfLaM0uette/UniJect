using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace LaM0uette.UniJect
{
    internal sealed class StrippingLevelCheck : IPreprocessBuildWithReport
    {
        #region Statements

        public int callbackOrder
        {
            get { return 0; }
        }

        #endregion

        #region Methods

        public void OnPreprocessBuild(BuildReport report)
        {
            NamedBuildTarget target = NamedBuildTarget.FromBuildTargetGroup(
                UnityEditor.BuildPipeline.GetBuildTargetGroup(report.summary.platform));

            if (PlayerSettings.GetScriptingBackend(target) != ScriptingImplementation.IL2CPP)
                return;

            ManagedStrippingLevel level = PlayerSettings.GetManagedStrippingLevel(target);

            if (level <= ManagedStrippingLevel.Low)
                return;

            Debug.LogWarning(
                "UniJect: " + IssueCode.UJ014 + " — this build is IL2CPP with managed stripping at " + level +
                ". Types reached only through [Inject] can be stripped. Keep stripping at Low, or add the " +
                "affected types to a link.xml.");
        }

        #endregion
    }
}
