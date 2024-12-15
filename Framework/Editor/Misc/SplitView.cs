using UnityEngine.UIElements;

namespace RobertHoudin.Framework.Editor.Misc
{

    [UxmlElement]
    public partial class SplitView : TwoPaneSplitView
    {

    }

#if !UNITY_6000_0_OR_NEWER
    public class SplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits>
        {
        }
    }
#endif
}