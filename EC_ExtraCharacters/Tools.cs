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
            "Canvas ADVPart/Manipulate/Chara"
        };
        
        public static void ExpandUI(int id)
        {
            var UI = GameObject.Find(id == 2 ? "ADVPart" : "UI");
            
            var character = UI.transform.Find(tr[id]);
            var list = character.transform.Find(id == 2 ? "Chara Select" : "List");

            var ScrollView = new GameObject("ScrollView", typeof(RectTransform));
            ScrollView.transform.SetParent(character, false);

            var svScrollRect = ScrollView.AddComponent<ScrollRect>();

            var ViewPort = new GameObject("ViewPort", typeof(RectTransform));
            ViewPort.transform.SetParent(ScrollView.transform, false);
            ViewPort.AddComponent<RectMask2D>();
            ViewPort.AddComponent<Image>().color = new Color(1, 0, 0, 0);
            var vpRectTransform = ViewPort.GetComponent<RectTransform>();

            list.SetParent(ViewPort.transform, false);

            svScrollRect.content = list.gameObject.GetComponent<RectTransform>();
            svScrollRect.viewport = vpRectTransform;
            svScrollRect.horizontal = false;
            svScrollRect.scrollSensitivity = 40;

            var fitter = list.gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            switch (id)
            {
                case 0:
                    ScrollView.transform.localPosition = new Vector3(370, -292, 0);
                
                    vpRectTransform.offsetMin = new Vector2(-370, -282);
                    vpRectTransform.offsetMax = new Vector2(370, 244);
                    break;
                case 1:
                    ScrollView.transform.localPosition = new Vector3(370, -304, 0);
                
                    vpRectTransform.offsetMin = new Vector2(-370, -260);
                    vpRectTransform.offsetMax = new Vector2(370, 244);
                
                    character.Find("BaseList").gameObject.SetActive(false);
                    break;
                case 2:
                    ScrollView.transform.localPosition = new Vector3(-620, 480, 0);
                
                    vpRectTransform.offsetMin = new Vector2(-340, -300);
                    vpRectTransform.offsetMax = new Vector2(0, 50);
                    break;
            }
        }
    }
}