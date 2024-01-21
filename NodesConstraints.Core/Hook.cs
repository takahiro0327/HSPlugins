using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using HarmonyLib;
using System.Linq;

namespace NodesConstraints
{
    [HarmonyPatch]
    public class DynamicBoneHook
    {
        static string pathFilter = "/ct_clothesBot";

        static string GetTransformPath( Transform transform )
        {
            List<string> paths = new List<string>();

            while( transform != null )
            {
                paths.Add(transform.name);
                transform = transform.parent;
            }

            StringBuilder builder = new StringBuilder();

            for (int i = paths.Count - 1; i >= 0; --i)
            {
                builder.Append('/');
                builder.Append(paths[i]);
            }

            return builder.ToString();
        }

        static public void Log( string methodName, DynamicBone dynamicBone )
        {
            var path = GetTransformPath(dynamicBone.transform);

            if (!path.Contains(pathFilter) || System.Array.IndexOf(dynamicBone.gameObject.GetComponents<DynamicBone>(), dynamicBone) != 5 )
                return;

            var transformPos = dynamicBone.transform.position * 1000;
            var particlePos0 = dynamicBone.m_Particles[0].m_Position * 1000;
            var particleTransformPos = dynamicBone.m_Particles[0].m_Transform.position * 1000;

            var particlePos1 = dynamicBone.m_Particles[1].m_Position * 1000;
            var particleTransformPos1 = dynamicBone.m_Particles[1].m_Transform.position * 1000;

            System.Console.WriteLine($"{Time.frameCount:D7} {methodName} {path} {dynamicBone.m_Particles[0].m_Transform.name} {dynamicBone.m_Particles[1].m_Transform.name} {transformPos:F1} [0]{particlePos0:F1} {particleTransformPos:F1} [1]{particlePos1:F2} {particleTransformPos1:F2}");
        }

        [HarmonyPrefix, HarmonyPatch(typeof(DynamicBone), nameof(DynamicBone.Update))]
        static bool PreUpdate( DynamicBone __instance )
        {
            Log("PreUpdate      ", __instance);
            return true;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(DynamicBone), nameof(DynamicBone.Update))]
        static void PostUpdate(DynamicBone __instance)
        {
            Log("PostUpdate     ", __instance);
            return;
        }


        [HarmonyPrefix, HarmonyPatch(typeof(DynamicBone), nameof(DynamicBone.LateUpdate))]
        static bool PreLateUpdate(DynamicBone __instance)
        {
            Log("PreLateUpdate  ", __instance);
            return true;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(DynamicBone), nameof(DynamicBone.LateUpdate))]
        static void PostLateUpdate(DynamicBone __instance)
        {
            Log("PostLateUpdate ", __instance);
            return;
        }

        static void Log( string methodName )
        {
            foreach (var dynamicBone in GameObject.FindObjectsOfType<DynamicBone>())
            {
                if (dynamicBone.isActiveAndEnabled)
                    Log(methodName, dynamicBone);
            }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(NodesConstraints), "ApplyNodesConstraints")]
        static bool PreApplyNodesConstraints(NodesConstraints __instance)
        {
            Log("PreApplyNodeC  ");            
            return true;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(NodesConstraints), "ApplyNodesConstraints")]
        static void PostApplyNodesConstraints(NodesConstraints __instance)
        {
            Log("PostApplyNodeC ");
            return;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(NodesConstraints), "ApplyConstraints")]
        static bool PreApplyConstraints(NodesConstraints __instance)
        {
            Log("PreApplyC      ");
            return true;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(NodesConstraints), "ApplyConstraints")]
        static void PostApplyConstraints(NodesConstraints __instance)
        {
            Log("PostApplyC     ");
            return;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(NodesConstraintsLater), "Update")]
        static bool PreLaterUpdate(NodesConstraintsLater __instance)
        {
            Log("NCLater.Update ");
            return true;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(NodesConstraints), "Update")]
        static bool PreNodesConstraintsUpdate(NodesConstraints __instance)
        {
            Log("NC.Update      ");
            return true;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(NodesConstraintsFaster), "Update")]
        static bool PreNodesConstraintsFasterUpdate(NodesConstraints __instance)
        {
            Log("NCFaster.Update");
            return true;
        }
    }
}
