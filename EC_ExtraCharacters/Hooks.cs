using System;
using System.Linq;
using System.Reflection.Emit;
using System.Collections.Generic;

using HEdit;
using ADVPart.Manipulate;

using HarmonyLib;
using HPlay;
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

        [HarmonyPostfix, HarmonyPatch(typeof(HPart), ".ctor")]
        public static void HPart_ctor_ChangeCoordCount1(HPart __instance)
        {
            __instance.coordinateInfos = new HPart.CoordinateInfo[EC_ExtraCharacters.charaCount];
            
            for (var i = 0; i < __instance.coordinateInfos.Length; i++)
                __instance.coordinateInfos[i] = new HPart.CoordinateInfo();
        }

        [HarmonyPostfix, HarmonyPatch(typeof(HPart), ".ctor", new [] { typeof(HPart) })]
        public static void HPart_ctor_ChangeCoordCount2(HPart __instance, HPart _part)
        {
            __instance.coordinateInfos = new HPart.CoordinateInfo[EC_ExtraCharacters.charaCount];
            
            for (var i = 0; i < __instance.coordinateInfos.Length; i++)
                __instance.coordinateInfos[i] = new HPart.CoordinateInfo(_part.coordinateInfos[i]);
        }

        [HarmonyPrefix, HarmonyPatch(typeof(InitializeCanvas), "Start")]
        public static void InitializeCanvas_Start_ExpandUI() => Tools.ExpandUI(0);
        
        [HarmonyPrefix, HarmonyPatch(typeof(BaseSettingCharacterUI), "Start")]
        public static void BaseSettingCharacterUI_Start_ExpandUI() => Tools.ExpandUI(1);

        [HarmonyPrefix, HarmonyPatch(typeof(PartSettingCanvas), "Start")]
        public static void PartSettingCanvas_Start_ExpandUI() => Tools.ExpandUI(4);
        
        [HarmonyPrefix, HarmonyPatch(typeof(MotionCopyUI), "Start")]
        public static void MotionCopyUI_Start_ExpandUI()
        {
            Tools.ExpandUI(5);
            Tools.ExpandUI(6);
        }

        [HarmonyPrefix, HarmonyPatch(typeof(HPlayCharacterChange), "Start")]
        public static void HPlayCharacterChange_Start_ExpandUI() => Tools.ExpandUI(8);
        
        [HarmonyPrefix, HarmonyPatch(typeof(HPlayData), "Start")]
        public static void HPlayData_Start_ChangeCount(HPlayData __instance)
        {
            __instance.characterChangeInfos = new HPlayData.CharacterChangeInfo[EC_ExtraCharacters.charaCount];
            __instance.groupInfos = new HPlayData.GroupInfo[EC_ExtraCharacters.charaCount];
            __instance.playCharaInfos = new HPlayData.PlayCharaInfo[EC_ExtraCharacters.charaCount];
        }
        
        [HarmonyPrefix, HarmonyPatch(typeof(HPlayScene), "Start")]
        public static void HPlayScene_Start_ChangeCount(ref AnimationParameterCtrl[] ___parametersCtrls)
        {
            var newparams = new AnimationParameterCtrl[EC_ExtraCharacters.charaCount];
            
            Array.Copy(___parametersCtrls, newparams, Math.Min(___parametersCtrls.Length, newparams.Length));

            var trav1 = Traverse.Create(newparams[0]);
            
            for (var i = ___parametersCtrls.Length; i < EC_ExtraCharacters.charaCount; i++)
            {
                var param = new AnimationParameterCtrl();
                
                var trav = Traverse.Create(param);
                trav.Field("motions").SetValue(trav1.Field("motions").GetValue<float[]>());
                trav.Field("fluctuations").SetValue(trav1.Field("fluctuations").GetValue<Fluctuation[]>());
                
                newparams[i] = param;
            }

            ___parametersCtrls = newparams;
        }
        
        [HarmonyPrefix, HarmonyPatch(typeof(HPlaySECtrl), "Start")]
        public static void HPlaySECtrl_Start_ChangeCount(ref Array ___oldGroupInfos)
        {
            var type = ___oldGroupInfos.GetType().GetElementType();
            if (type == null) 
                return;
            
            var newSelects = Array.CreateInstance(type, EC_ExtraCharacters.charaCount);

            ___oldGroupInfos = newSelects;
        }
        
        [HarmonyPrefix, HarmonyPatch(typeof(HPlayVoiceCtr), "Start")]
        public static void HPlayVoiceCtr_Start_ChangeCount(ref Motion.Face[] ___playFaces, ref Array ___oldGroupInfos)
        {
            ___playFaces = new Motion.Face[EC_ExtraCharacters.charaCount];
            
            var type = ___oldGroupInfos.GetType().GetElementType();
            if (type == null) 
                return;
            
            var newSelects = Array.CreateInstance(type, EC_ExtraCharacters.charaCount);

            ___oldGroupInfos = newSelects;
        }
        
        [HarmonyPostfix, HarmonyPatch(typeof(EditUICtrl), "Active")]
        public static void EditUICtrl_Active_FixPos(EditUICtrl.Mode _mode)
        {
            if (_mode != EditUICtrl.Mode.CharaTransform) 
                return;
            
            var UI = GameObject.Find("ADVPart");
            var ScrollView = UI.transform.Find("Canvas Transform/ScrollView");

            if (ScrollView == null)
                return;
            
            var position1 = UI.transform.Find("Canvas ADVPart/Manipulate/Item/State").position;
            position1 += new Vector3(-10, 0, 0);
                    
            ScrollView.transform.position = position1;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(PartInfoClothSetUI), "Start")]
        public static void PartInfoClothSetUI_Start_ExpandUI(ref PartInfoClothSetUI.CoordinateUIInfo[] ___coordinateUIs)
        {
            var newSelects = new PartInfoClothSetUI.CoordinateUIInfo[EC_ExtraCharacters.charaCount];
            Array.Copy(___coordinateUIs, newSelects, Math.Min(___coordinateUIs.Length, newSelects.Length));

            var UI = GameObject.Find("UI");
            var orig = UI.transform.Find("HPart/PartInfoSetting/Canvas/ClothSetCategory/BG/SetList/Scroll View/Viewport/Content/Set (1)");

            for (var i = ___coordinateUIs.Length; i < EC_ExtraCharacters.charaCount; i++)
            {
                var gCopy = UnityEngine.Object.Instantiate(orig, orig.parent);
                gCopy.name = $"Set ({i})";

                var comp = new PartInfoClothSetUI.CoordinateUIInfo
                {
                    objRoot = gCopy.gameObject,
                    txtCharaName = gCopy.Find("imgTitle/Mask/Name").GetComponent<Text>(),
                    txtCoordinateName = gCopy.Find("Coordinate/Image/Mask/Text").GetComponent<Text>(),
                    btnContinuation = gCopy.Find("Coordinate/btnContinuation").GetComponent<Button>(),
                    btnEntry = gCopy.Find("Coordinate/btnChange").GetComponent<Button>(),
                    btnRelease = gCopy.Find("Coordinate/btnReset").GetComponent<Button>(),
                };

                newSelects[i] = comp;
            }

            ___coordinateUIs = newSelects;
        }
        
        [HarmonyPrefix, HarmonyPatch(typeof(GroupCharaSelect), "InitUI")]
        public static void GroupCharaSelect_InitUI_ExpandUI(ref GroupCharaSelect.ToggleText[] ___ttCharas)
        {
            var newSelects = new GroupCharaSelect.ToggleText[EC_ExtraCharacters.charaCount];
            Array.Copy(___ttCharas, newSelects, Math.Min(___ttCharas.Length, newSelects.Length));

            var UI = GameObject.Find("UI");
            var orig = UI.transform.Find("HPart/MotionSetting/Canvas/chara/CharaInfo1");

            for (var i = ___ttCharas.Length; i < EC_ExtraCharacters.charaCount; i++)
            {
                var gCopy = UnityEngine.Object.Instantiate(orig, orig.parent);
                gCopy.name = "CharaInfo" + i;

                var comp = new GroupCharaSelect.ToggleText
                {
                    tgl = gCopy.GetComponentInChildren<Toggle>(),
                    text = gCopy.Find("tglGroupChara/mask/Label").GetComponent<Text>(),
                    objRoot = gCopy.gameObject,
                    objState = gCopy.Find("Status").gameObject,
                    status =
                    {
                        btnHalf = gCopy.Find("Status/Cloth/State/btnHalf").GetComponent<Button>(),
                        btnNude = gCopy.Find("Status/Cloth/State/btnNude").GetComponent<Button>(),
                        btnWear = gCopy.Find("Status/Cloth/State/btnWear").GetComponent<Button>(),
                        btnOff = gCopy.Find("Status/Accessory/State/btnOff").GetComponent<Button>(),
                        btnOn = gCopy.Find("Status/Accessory/State/btnOn").GetComponent<Button>(),
                        btnSiruOff = gCopy.Find("Status/Siru/btnSiru").GetComponent<Button>(),
                        tglDraw = gCopy.Find("Status/BodyDraw/tglDraw").GetComponent<Toggle>(),
                        tglGuideDraw = gCopy.Find("Status/GuideDraw/tglDraw").GetComponent<Toggle>()
                    }
                };

                newSelects[i] = comp;
            }

            ___ttCharas = newSelects;
            
            Tools.ExpandUI(7);
        }
        
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
        
        [HarmonyTranspiler, HarmonyPatch(typeof(HPart), ".ctor", typeof(HPart))]
        public static IEnumerable<CodeInstruction> HPart_ctor_ChangeCoordCount(IEnumerable<CodeInstruction> instructions) => ChangeCharaCount(instructions, 8, new CodeInstruction(OpCodes.Ldc_I4_S, EC_ExtraCharacters.charaCount), false, "HPart_ctor_ChangeCoordCount");

        [HarmonyTranspiler, HarmonyPatch(typeof(PartSettingCanvas), "AddGroup", new Type[] {})]
        public static IEnumerable<CodeInstruction> PartSettingCanvas_AddGroup_ChangeCharaCount1(IEnumerable<CodeInstruction> instructions) => ChangeCharaCount(instructions, 8, new CodeInstruction(OpCodes.Ldc_I4_S, EC_ExtraCharacters.charaCount), false, "PartSettingCanvas_AddGroup_ChangeCharaCount1");
        
        [HarmonyTranspiler, HarmonyPatch(typeof(PartSettingCanvas), "AddGroup", typeof(HPart.Group))]
        public static IEnumerable<CodeInstruction> PartSettingCanvas_AddGroup_ChangeCharaCount2(IEnumerable<CodeInstruction> instructions) => ChangeCharaCount(instructions, 8, new CodeInstruction(OpCodes.Ldc_I4_S, EC_ExtraCharacters.charaCount), false, "PartSettingCanvas_AddGroup_ChangeCharaCount2");
            
        public static IEnumerable<CodeInstruction> InitializeCanvas_AddChara_ChangeCharaCount1(IEnumerable<CodeInstruction> instructions) => ChangeCharaCount(instructions, 8, new CodeInstruction(OpCodes.Ldc_I4_S, EC_ExtraCharacters.charaCount), false, "InitializeCanvas_AddChara_ChangeCharaCount1");
        
        public static IEnumerable<CodeInstruction> InitializeCanvas_AddChara_ChangeCharaCount2(IEnumerable<CodeInstruction> instructions) => ChangeCharaCount(instructions, 7, new CodeInstruction(OpCodes.Ldc_I4_S, EC_ExtraCharacters.charaCount - 1), false, "InitializeCanvas_AddChara_ChangeCharaCount2");
    }
}