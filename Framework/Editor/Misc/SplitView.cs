using UnityEngine.UIElements;

namespace RobertHoudin.Framework.Editor.Misc
{
    public class SplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits>
        {
        }
    }
}