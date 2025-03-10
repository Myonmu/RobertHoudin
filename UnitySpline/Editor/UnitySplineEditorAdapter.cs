using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEditor.Splines;
using Object = UnityEngine.Object;
namespace RobertHoudin.UnitySpline.Editor
{
    public static class UnitySplineEditorAdapter
    {
        private static MethodInfo _selectionGetter;
        private static Type _selectionType;
        private static Func<object, Object> _objectGetter;
        private static Func<object, int> _targetIndexGetter;
        private static Func<object, int> _knotIndexGetter;
        private static Func<object, int> _tangentIndexGetter;
        [InitializeOnLoadMethod]
        private static void SetupEvents()
        {
            EnsureSetup();
            SplineSelection.changed += OnSelectionChanged;
        }

        private static void EnsureSetup()
        {
            // Not frequently called, so plain reflection is OK
            _selectionType = typeof(SplineSelection).Assembly.GetType("UnityEditor.Splines.SelectableSplineElement");
            _objectGetter = (o) => (Object)_selectionType.GetField("target").GetValue(o);
            _targetIndexGetter = (o) => (int)_selectionType.GetField("targetIndex").GetValue(o);
            _knotIndexGetter = (o) => (int)_selectionType.GetField("knotIndex").GetValue(o);
            _tangentIndexGetter = (o) => (int)_selectionType.GetField("tangentIndex").GetValue(o);
            _selectionGetter = typeof(SplineSelection)
                .GetProperty("selection",
                    BindingFlags.NonPublic | BindingFlags.Static)
                .GetGetMethod(true);
        }
        private static void OnSelectionChanged()
        {
            if(_selectionGetter == null) EnsureSetup();
            var result = _selectionGetter.Invoke(null, null);
            var list = result as IEnumerable;
            foreach (var o in list)
            {
                if (o.GetType() != _selectionType) continue;
                var target = _objectGetter(o);
                var targetIndex = _targetIndexGetter(o);
                var knot = _knotIndexGetter(o);
                var tangent = _tangentIndexGetter(o);
                
                break;
            }
        }
    }
}