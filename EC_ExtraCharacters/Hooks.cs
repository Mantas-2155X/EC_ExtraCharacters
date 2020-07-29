using System;
using System.Linq;
using System.Reflection.Emit;
using System.Collections.Generic;

using HEdit;
using ADVPart;
using ADVPart.Manipulate;

using HarmonyLib;

using TMPro;

using UnityEngine.UI;

namespace EC_ExtraCharacters
{
    public static class Hooks
    {
        public static void PatchSpecial(Harmony harmony)
        {
            {
                var iteratorType = typeof(InitializeCanvas).GetNestedType("<AddChara>c__AnonStorey1", AccessTools.all);
                var iteratorMethod = AccessTools.Method(iteratorType, "<>m__3");
                var transpiler = new HarmonyMethod(typeof(Hooks), nameof(InitializeCanvas_AddChara_ChangeCharaCount1));
                harmony.Patch(iteratorMethod, null, null, transpiler);
            }
            
            {
                var iteratorType = typeof(InitializeCanvas).GetNestedType("<AddChara>c__AnonStorey1", AccessTools.all);
                var iteratorMethod = AccessTools.Method(iteratorType, "<>m__2");
                var transpiler = new HarmonyMethod(typeof(Hooks), nameof(InitializeCanvas_AddChara_ChangeCharaCount2));
                harmony.Patch(iteratorMethod, null, null, transpiler);
            }
        }

        [HarmonyPrefix, HarmonyPatch(typeof(InitializeCanvas), "Start")]
        public static void InitializeCanvas_Start_ExpandUI() => Tools.ExpandUI(0);
        
        [HarmonyPrefix, HarmonyPatch(typeof(BaseSettingCharacterUI), "Start")]
        public static void BaseSettingCharacterUI_Start_ExpandUI() => Tools.ExpandUI(1);

        [HarmonyPrefix, HarmonyPatch(typeof(CharaUICtrl), "Init")]
        public static void CharaUICtrl_Init_ExpandUI(CharaUICtrl __instance, ref Array ___selects)
        {
            if (___selects.Length != 8)
                return;

            Tools.ExpandUI(2);
            
            var type = ___selects.GetType().GetElementType();
            if (type == null) 
                return;
            
            var newSelects = Array.CreateInstance(type, EC_ExtraCharacters.charaCount);
            Array.Copy(___selects, newSelects, Math.Min(___selects.Length, newSelects.Length));

            var orig = __instance.transform.Find("ScrollView/ViewPort/Chara Select/Toggle Chara 0");

            for (var i = ___selects.Length; i < EC_ExtraCharacters.charaCount; i++)
            {
                var gCopy = UnityEngine.Object.Instantiate(orig, orig.parent);
                gCopy.name = "Toggle Chara " + (i + 1);
                
                var obj = Activator.CreateInstance(type, true);
                
                var goFieldInfo = type.GetField("gameObject");
                goFieldInfo.SetValue(obj, gCopy.gameObject);
                
                var tglFieldInfo = type.GetField("toggle");
                tglFieldInfo.SetValue(obj, gCopy.GetComponentInChildren<Toggle>());
                
                var imageFieldInfo = type.GetField("textMesh");
                imageFieldInfo.SetValue(obj, gCopy.GetComponentInChildren<TextMeshProUGUI>());

                newSelects.SetValue(obj, i);
            }

            ___selects = newSelects;
        }
        
        [HarmonyTranspiler, HarmonyPatch(typeof(HEditData), "Check")]
        public static IEnumerable<CodeInstruction> HEditData_Check_ChangeCharaCount(IEnumerable<CodeInstruction> instructions)
        {
            var il = instructions.ToList();
            
            {
                var index = il.FindIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_7);
                if (index <= 0)
                {
                    EC_ExtraCharacters.Logger.LogMessage("Failed transpiling 'HEditData_Check_ChangeCharaCount' Ldc_I4_7 index not found!");
                    EC_ExtraCharacters.Logger.LogWarning("Failed transpiling 'HEditData_Check_ChangeCharaCount' Ldc_I4_7 index not found!");
                    return il;
                }
        
                il[index] = new CodeInstruction(OpCodes.Ldc_I4_S, EC_ExtraCharacters.charaCount - 1);
            }
            
            {
                var index = il.FindIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_8);
                if (index <= 0)
                {
                    EC_ExtraCharacters.Logger.LogMessage("Failed transpiling 'HEditData_Check_ChangeCharaCount' Ldc_I4_8 index not found!");
                    EC_ExtraCharacters.Logger.LogWarning("Failed transpiling 'HEditData_Check_ChangeCharaCount' Ldc_I4_8 index not found!");
                    return il;
                }
        
                il[index] = new CodeInstruction(OpCodes.Ldc_I4_S, EC_ExtraCharacters.charaCount);
            }
            
            {
                var index = il.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_8);
                if (index <= 0)
                {
                    EC_ExtraCharacters.Logger.LogMessage("Failed transpiling 'HEditData_Check_ChangeCharaCount' Ldc_I4_8 index not found!");
                    EC_ExtraCharacters.Logger.LogWarning("Failed transpiling 'HEditData_Check_ChangeCharaCount' Ldc_I4_8 index not found!");
                    return il;
                }
        
                il[index] = new CodeInstruction(OpCodes.Ldc_I4_S, EC_ExtraCharacters.charaCount);
            }
            
            return il;
        }
        
        public static IEnumerable<CodeInstruction> InitializeCanvas_AddChara_ChangeCharaCount1(IEnumerable<CodeInstruction> instructions)
        {
            var il = instructions.ToList();
            
            var index = il.FindIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_8);
            if (index <= 0)
            {
                EC_ExtraCharacters.Logger.LogMessage("Failed transpiling 'InitializeCanvas_AddChara_ChangeCount' Ldc_I4_8 index not found!");
                EC_ExtraCharacters.Logger.LogWarning("Failed transpiling 'InitializeCanvas_AddChara_ChangeCount' Ldc_I4_8 index not found!");
                return il;
            }
        
            il[index] = new CodeInstruction(OpCodes.Ldc_I4_S, EC_ExtraCharacters.charaCount);
            
            return il;
        }
        
        public static IEnumerable<CodeInstruction> InitializeCanvas_AddChara_ChangeCharaCount2(IEnumerable<CodeInstruction> instructions)
        {
            var il = instructions.ToList();
            
            var index = il.FindIndex(instruction => instruction.opcode == OpCodes.Ldc_I4_7);
            if (index <= 0)
            {
                EC_ExtraCharacters.Logger.LogMessage("Failed transpiling 'InitializeCanvas_AddChara_ChangeCount' Ldc_I4_7 index not found!");
                EC_ExtraCharacters.Logger.LogWarning("Failed transpiling 'InitializeCanvas_AddChara_ChangeCount' Ldc_I4_7 index not found!");
                return il;
            }
        
            il[index] = new CodeInstruction(OpCodes.Ldc_I4_S, EC_ExtraCharacters.charaCount - 1);
            
            return il;
        }
    }
}