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
            "Canvas Transform"
        };
        
        public static void ExpandUI(int id)
        {
            var UI = GameObject.Find(id == 2 || id == 3 ? "ADVPart" : "UI");
            
            var character = UI.transform.Find(tr[id]);
            var list = character.transform.Find(id == 2 || id == 3 ? "Chara Select" : "List");
            var listRect = list.gameObject.GetComponent<RectTransform>();
            
            var ScrollView = new GameObject("ScrollView", typeof(RectTransform));
            ScrollView.transform.SetParent(character, false);

            var svScrollRect = ScrollView.AddComponent<ScrollRect>();

            var ViewPort = new GameObject("ViewPort", typeof(RectTransform));
            ViewPort.transform.SetParent(ScrollView.transform, false);
            ViewPort.AddComponent<RectMask2D>();
            ViewPort.AddComponent<Image>().color = new Color(1, 0, 0, 0.5f);
            var vpRectTransform = ViewPort.GetComponent<RectTransform>();

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
            }
        }
    }
}