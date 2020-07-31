using System.Linq;

using UnityEngine;
using UnityEngine.UI;

namespace EC_ExtraCharacters
{
    public static class Tools
    {
        private static readonly string[] tr =
        {
            "Initialize/Canvas/Init/Character",
            "BaseSetting/Canvas/Init/Character",
            "Canvas ADVPart/Manipulate/Chara",
            "Canvas Transform",
            "HPart/PartSetting/Canvas/GroupTree",
            "HPart/MotionSetting/Canvas/CopyCategory/BG/Position/WhoWhere/Src/SrcChara/List/Scroll View",
            "HPart/MotionSetting/Canvas/CopyCategory/BG/Position/WhoWhere/Dst/DstChara/List/Scroll View",
            "HPart/MotionSetting/Canvas/chara"
        };
        
        public static void ExpandUI(int id)
        {
            var UI = GameObject.Find(id == 2 || id == 3 ? "ADVPart" : "UI");
            
            var character = UI.transform.Find(tr[id]);

            if (id == 5 || id == 6)
            {
                var sRect = character.gameObject.AddComponent<ScrollRect>();
                sRect.content = character.Find("Viewport/Content").GetComponent<RectTransform>();
                sRect.viewport = character.Find("Viewport").GetComponent<RectTransform>();
                sRect.horizontal = false;
                sRect.scrollSensitivity = 40;

                var fit = sRect.content.gameObject.AddComponent<ContentSizeFitter>();
                fit.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                fit.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                
                return;
            }

            Transform list;
            if (id == 7)
            {
                var listObj = new GameObject("List", typeof(RectTransform));
                listObj.transform.SetParent(character, false);
                
                list = listObj.transform;

                var temp = character.Cast<Transform>().Where(child => child.name.Contains("CharaInfo")).ToList();

                foreach (var t in temp)
                    t.SetParent(list.transform, false);
            }
            else
            {
                list = character.transform.Find(id == 2 || id == 3 ? "Chara Select" : "List");
            }

            var listRect = list.gameObject.GetComponent<RectTransform>();
            
            var ScrollView = new GameObject("ScrollView", typeof(RectTransform));
            ScrollView.transform.SetParent(character, false);

            var svScrollRect = ScrollView.AddComponent<ScrollRect>();

            var ViewPort = new GameObject("ViewPort", typeof(RectTransform));
            ViewPort.transform.SetParent(ScrollView.transform, false);
            ViewPort.AddComponent<RectMask2D>();
            
            var vpRectTransform = ViewPort.GetComponent<RectTransform>();
            
            var vpImg = ViewPort.AddComponent<Image>();
            vpImg.color = new Color(1, 0, 0, 0.5f);
            
            list.SetParent(ViewPort.transform, false);

            svScrollRect.content = listRect;
            svScrollRect.viewport = vpRectTransform;
            svScrollRect.horizontal = false;
            svScrollRect.scrollSensitivity = 40;

            var fitter = list.gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var oldPos = ScrollView.transform.localPosition;
            
            switch (id)
            {
                case 0:
                    ScrollView.transform.localPosition = new Vector3(oldPos.x - 11f, oldPos.y, 0);
                    
                    vpRectTransform.offsetMin = new Vector2(-370f, -282f);
                    vpRectTransform.offsetMax = new Vector2(370f, 244f);
                    break;
                case 1:
                    ScrollView.transform.localPosition = new Vector3(oldPos.x - 11f, oldPos.y, 0);
                
                    vpRectTransform.offsetMin = new Vector2(-370f, -260f);
                    vpRectTransform.offsetMax = new Vector2(370f, 244f);
                
                    character.Find("BaseList").gameObject.SetActive(false);
                    break;
                case 2:
                    listRect.offsetMin = new Vector2(0, listRect.offsetMin.y);
                    
                    ScrollView.transform.localPosition = new Vector3(-620f, 490, 0);
                
                    vpRectTransform.offsetMin = new Vector2(-160, -300);
                    vpRectTransform.offsetMax = new Vector2(0, 50);
                    break;
                case 3:
                    listRect.offsetMin = new Vector2(0, listRect.offsetMin.y);
                    
                    ScrollView.transform.localPosition = new Vector3(-795f, 490, 0);
                
                    vpRectTransform.offsetMin = new Vector2(-160, -300);
                    vpRectTransform.offsetMax = new Vector2(0, 50);
                    break;
                case 4:
                    listRect.offsetMin = new Vector2(0, listRect.offsetMin.y);
                    
                    ScrollView.transform.localPosition = new Vector3(50, -226, 0);
                
                    vpRectTransform.offsetMin = new Vector2(-50, -348);
                    vpRectTransform.offsetMax = new Vector2(297, 184);

                    var btn = list.Find("Button");
                    btn.SetParent(ScrollView.transform.parent, true);
                    
                    btn.transform.localPosition = new Vector3(17, -570, 0);
                    
                    vpImg.sprite = list.GetComponent<Image>().sprite;
                    vpImg.type = Image.Type.Sliced;
                    vpImg.color = Color.white;
                    
                    break;
                case 7:
                    ScrollView.AddComponent<LayoutElement>().preferredHeight = 300;
                    ScrollView.transform.SetSiblingIndex(2);
                    
                    //listRect.offsetMin = new Vector2(-109, listRect.offsetMin.y);
                    
                    //ScrollView.transform.localPosition = new Vector3(-210, -137, 0);
                    
                    break;
            }
        }
    }
}