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
            "HPart/MotionSetting/Canvas/chara",
            "Canvas/Panel/Select",
            "System/Canvas/System",
            "HPart/CanvasIKParentSelect/BG/Frame/imgParent",
            "HPart/IKParentSetting/Canvas/chara"
        };
        
        public static void ExpandUI(int id)
        {
            var UI = GameObject.Find(id == 2 || id == 3 ? "ADVPart" : id == 8 ? "HPlayCharacterChange" : "UI");
            
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
            if (id == 7 || id == 11)
            {
                var listObj = new GameObject("List", typeof(RectTransform));
                listObj.transform.SetParent(character, false);
                
                list = listObj.transform;

                var temp = character.Cast<Transform>().Where(child => child.name.Contains("CharaInfo")).ToList();
                foreach (var t in temp)
                {
                    t.SetParent(list.transform, false);
                    
                    if(t.name == "CharaInfo7")
                        t.GetComponent<VerticalLayoutGroup>().padding = new RectOffset(8, 0, 0, 0);
                }
            }
            else
            {
                list = character.transform.Find(id == 2 || id == 3 ? "Chara Select" : id == 9 ? "Member" : "List");
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
            vpImg.color = new Color(1, 0, 0, 0f);
            
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
                    
                    vpRectTransform.offsetMin = new Vector2(-160, -335);
                    vpRectTransform.offsetMax = new Vector2(0, 0);

                    Vector3 position;
                    
                    position = UI.transform.Find("Canvas ADVPart/Manipulate/Item/State").position;
                    position += new Vector3(-10, 0, 0);
                    
                    ScrollView.transform.position = position;
                    break;
                case 3:
                    listRect.offsetMin = new Vector2(0, listRect.offsetMin.y);
                    
                    vpRectTransform.offsetMin = new Vector2(-160, -335);
                    vpRectTransform.offsetMax = new Vector2(0, 0);

                    Vector3 position1;
                    
                    position1 = UI.transform.Find("Canvas ADVPart/Manipulate/Item/State").position;
                    position1 += new Vector3(-10, 0, 0);
                    
                    ScrollView.transform.position = position1;
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

                    list.gameObject.AddComponent<VerticalLayoutGroup>();
    
                    vpRectTransform.offsetMin = new Vector2(-42, -150);
                    vpRectTransform.offsetMax = new Vector2(158, 150);
                    
                    listRect.offsetMin = new Vector2(-108, listRect.offsetMin.y);

                    break;
                case 8:
                    listRect.offsetMin = new Vector2(0, listRect.offsetMin.y);
                    
                    vpRectTransform.offsetMin = new Vector2(-570, -372);
                    vpRectTransform.offsetMax = new Vector2(570, 452);
                    break;
                case 9:
                    listRect.offsetMin = new Vector2(0, listRect.offsetMin.y);
                    
                    ScrollView.transform.localPosition = new Vector3(70, -210, 0);

                    vpRectTransform.offsetMin = new Vector2(-70, -128);
                    vpRectTransform.offsetMax = new Vector2(55, 128);
                    break;
                case 10:
                    vpRectTransform.offsetMin = new Vector2(-315, -105);
                    vpRectTransform.offsetMax = new Vector2(315, 68);
                    break;
                case 11:
                    ScrollView.AddComponent<LayoutElement>().preferredHeight = 300;
                    ScrollView.transform.SetSiblingIndex(2);

                    list.gameObject.AddComponent<VerticalLayoutGroup>();
    
                    vpRectTransform.offsetMin = new Vector2(-42, -150);
                    vpRectTransform.offsetMax = new Vector2(158, 150);
                    
                    listRect.offsetMin = new Vector2(-108, listRect.offsetMin.y);

                    break;
            }
        }
    }
}