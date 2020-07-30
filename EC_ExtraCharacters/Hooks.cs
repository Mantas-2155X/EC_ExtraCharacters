using System;
using System.Linq;
using System.Reflection.Emit;
using System.Collections.Generic;

using HEdit;
using ADVPart.Manipulate;

using HarmonyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace EC_ExtraCharacters
{
    public static class Hooks
    {
        public static void PatchSpecial(Harmony harmony)
        {
            { // why is TransformUICtrl internal?...
                var iteratorType = AccessTools.TypeByName("ADVPart.Manipulate.Chara.TransformUICtrl");
                var iteratorMethod = AccessTools.Method(iteratorType, "Init");
                var prefix = new HarmonyMethod(typeof(Hooks), nameof(TransformUICtrl_Init_ExpandUI));
                harmony.Patch(iteratorMethod, prefix);
            }
            
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

        [HarmonyPrefix, HarmonyPatch(typeof(PartSettingCanvas), "Init")]
        public static void PartSettingCanvas_Init_ExpandUI() => Tools.ExpandUI(4);
        
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
        
        public static void TransformUICtrl_Init_ExpandUI(ref Array ___selectUIs)
        {
            if (___selectUIs.Length != 8)
                return;

            Tools.ExpandUI(3);
            
            var type = ___selectUIs.GetType().GetElementType();
            if (type == null) 
                return;
            
            var newSelects = Array.CreateInstance(type, EC_ExtraCharacters.charaCount);
            Array.Copy(___selectUIs, newSelects, Math.Min(___selectUIs.Length, newSelects.Length));

            var adv = GameObject.Find("ADVPart");
            var orig = adv.transform.Find("Canvas Transform/ScrollView/ViewPort/Chara Select/Toggle Chara 0");

            for (var i = ___selectUIs.Length; i < EC_ExtraCharacters.charaCount; i++)
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

            ___selectUIs = newSelects;
        }

        private static IEnumerable<CodeInstruction> ChangeCharaCount(IEnumerable<CodeInstruction> instructions, int num, CodeInstruction instr, bool last, string name)
        {
            var il = instructions.ToList();
            
            var index = last ? 
                il.FindLastIndex(instruction => instruction.opcode == (num == 7 ? OpCodes.Ldc_I4_7 : OpCodes.Ldc_I4_8)) : 
                il.FindIndex(instruction => instruction.opcode == (num == 7 ? OpCodes.Ldc_I4_7 : OpCodes.Ldc_I4_8));
            
            if (index <= 0)
            {
                EC_ExtraCharacters.Logger.LogMessage("Failed transpiling " + name + " Ldc_I4_" + num + " index not found!");
                EC_ExtraCharacters.Logger.LogWarning("Failed transpiling " + name + " Ldc_I4_" + num + " index not found!");
                return il;
            }
    
            il[index] = instr;

            return il;
        }

        [HarmonyTranspiler, HarmonyPatch(typeof(HEditData), "Check")]
        public static IEnumerable<CodeInstruction> HEditData_Check_ChangeCharaCount(IEnumerable<CodeInstruction> instructions)
        {
            var instr = instructions.ToList();
            ChangeCharaCount(instr, 7, new CodeInstruction(OpCodes.Ldc_I4_S, EC_ExtraCharacters.charaCount - 1), false, "HEditData_Check_ChangeCharaCount");
            ChangeCharaCount(instr, 8, new CodeInstruction(OpCodes.Ldc_I4_S, EC_ExtraCharacters.charaCount), false, "HEditData_Check_ChangeCharaCount");
            ChangeCharaCount(instr, 8, new CodeInstruction(OpCodes.Ldc_I4_S, EC_ExtraCharacters.charaCount), true, "HEditData_Check_ChangeCharaCount");

            return instr;
        }
        
        [HarmonyTranspiler, HarmonyPatch(typeof(PartSettingCanvas), "AddGroup", new Type[] {})]
        public static IEnumerable<CodeInstruction> PartSettingCanvas_AddGroup_ChangeCharaCount1(IEnumerable<CodeInstruction> instructions)
        {
            return ChangeCharaCount(instructions, 8, new CodeInstruction(OpCodes.Ldc_I4_S, EC_ExtraCharacters.charaCount), false, "PartSettingCanvas_AddGroup_ChangeCharaCount");
        }
        
        [HarmonyTranspiler, HarmonyPatch(typeof(PartSettingCanvas), "AddGroup", typeof(HPart.Group))]
        public static IEnumerable<CodeInstruction> PartSettingCanvas_AddGroup_ChangeCharaCount2(IEnumerable<CodeInstruction> instructions)
        {
            return ChangeCharaCount(instructions, 8, new CodeInstruction(OpCodes.Ldc_I4_S, EC_ExtraCharacters.charaCount), false, "PartSettingCanvas_AddGroup_ChangeCharaCount");
        }
        
        public static IEnumerable<CodeInstruction> InitializeCanvas_AddChara_ChangeCharaCount1(IEnumerable<CodeInstruction> instructions)
        {
            return ChangeCharaCount(instructions, 8, new CodeInstruction(OpCodes.Ldc_I4_S, EC_ExtraCharacters.charaCount), false, "InitializeCanvas_AddChara_ChangeCount");
        }
        
        public static IEnumerable<CodeInstruction> InitializeCanvas_AddChara_ChangeCharaCount2(IEnumerable<CodeInstruction> instructions)
        {
            return ChangeCharaCount(instructions, 7, new CodeInstruction(OpCodes.Ldc_I4_S, EC_ExtraCharacters.charaCount - 1), false, "InitializeCanvas_AddChara_ChangeCount");
        }
    }
}