using System;
namespace RobertHoudin.InspectorExt
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonAttribute : Sirenix.OdinInspector.ButtonAttribute
    {
        public ButtonAttribute() : base() {}
        public ButtonAttribute(string name) : base(name){}
    }
}