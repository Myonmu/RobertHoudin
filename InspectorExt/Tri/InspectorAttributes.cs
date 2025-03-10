using System;
namespace RobertHoudin.InspectorExt
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonAttribute : TriInspector.ButtonAttribute
    {
        public ButtonAttribute() : base() {}
        public ButtonAttribute(string name) : base(name){}
    }
}