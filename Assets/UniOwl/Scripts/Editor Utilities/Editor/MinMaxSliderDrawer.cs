using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UniOwl.Editor
{
    [CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
    public class MinMaxSliderDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var minMaxAttribute = (MinMaxSliderAttribute)attribute;
            var propertyType = property.propertyType;

            label.tooltip = minMaxAttribute.Min.ToString("F2") + " to " + minMaxAttribute.Max.ToString("F2");

            Rect controlRect = EditorGUI.PrefixLabel(position, label);
            Rect[] splittedRect = SplitRect(controlRect, 3);

            if (propertyType == SerializedPropertyType.Vector2)
            {
                EditorGUI.BeginChangeCheck();

                Vector2 vector = property.vector2Value;
                float minVal = vector.x;
                float maxVal = vector.y;

                minVal = EditorGUI.FloatField(splittedRect[0], float.Parse(minVal.ToString("F2")));
                maxVal = EditorGUI.FloatField(splittedRect[2], float.Parse(maxVal.ToString("F2")));

                EditorGUI.MinMaxSlider(splittedRect[1], ref minVal, ref maxVal,
                    minMaxAttribute.Min, minMaxAttribute.Max);

                if (minVal < minMaxAttribute.Min)
                    minVal = minMaxAttribute.Min;

                if (maxVal > minMaxAttribute.Max)
                    maxVal = minMaxAttribute.Max;

                vector = new Vector2(minVal > maxVal ? maxVal : minVal, maxVal);

                if (EditorGUI.EndChangeCheck())
                    property.vector2Value = vector;
            }
            else if (propertyType == SerializedPropertyType.Vector2Int)
            {
                EditorGUI.BeginChangeCheck();

                Vector2Int vector = property.vector2IntValue;
                float minVal = vector.x;
                float maxVal = vector.y;

                minVal = EditorGUI.FloatField(splittedRect[0], minVal);
                maxVal = EditorGUI.FloatField(splittedRect[2], maxVal);

                EditorGUI.MinMaxSlider(splittedRect[1], ref minVal, ref maxVal,
                    minMaxAttribute.Min, minMaxAttribute.Max);

                if (minVal < minMaxAttribute.Min)
                    maxVal = minMaxAttribute.Min;

                if (minVal > minMaxAttribute.Max)
                    maxVal = minMaxAttribute.Max;

                vector = new Vector2Int(Mathf.FloorToInt(minVal > maxVal ? maxVal : minVal), Mathf.FloorToInt(maxVal));

                if (EditorGUI.EndChangeCheck())
                    property.vector2IntValue = vector;
            }
        }

        private Rect[] SplitRect(Rect rectToSplit, int n)
        {
            Rect[] rects = new Rect[n];

            for (int i = 0; i < n; i++)
                rects[i] = new Rect(rectToSplit.position.x +
                                    (i * rectToSplit.width / n), rectToSplit.position.y, rectToSplit.width / n,
                    rectToSplit.height);

            int padding = (int)rects[0].width - 40;
            int space = 5;

            rects[0].width -= padding + space;
            rects[2].width -= padding + space;

            rects[1].x -= padding;
            rects[1].width += padding * 2;

            rects[2].x += padding + space;

            return rects;
        }

        private static VisualTreeAsset sliderTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Plugins/UniOwl/Editor Default Resources/MinMaxSliderWithFields.uxml");
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var value = property.vector2Value;
            var sliderAttribute = property.GetAttribute<MinMaxSliderAttribute>();

            var fullSlider = sliderTemplate.CloneTree();

            var inspectorName = property.GetAttribute<InspectorNameAttribute>();
            string name = inspectorName?.displayName ?? property.displayName;
            fullSlider.Q<Label>("Name").text = name;
            
            var slider = fullSlider.Q<MinMaxSlider>("Slider");
            slider.lowLimit = sliderAttribute.Min;
            slider.highLimit = sliderAttribute.Max;
            slider.minValue = value.x;
            slider.maxValue = value.y;
            slider.BindProperty(property);
            
            var min = fullSlider.Q<FloatField>("Min");
            min.formatString = "0.##";
            min.BindProperty(property.FindPropertyRelative("x"));
            var max = fullSlider.Q<FloatField>("Max");
            max.formatString = "0.##";
            max.BindProperty(property.FindPropertyRelative("y"));
            
            return fullSlider;
        }
    }
}