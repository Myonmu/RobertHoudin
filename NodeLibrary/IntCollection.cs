using System;
using System.Collections.Generic;
using RobertHoudin.Framework.Core.Primitives.Ports;

namespace RobertHoudin.NodeLibrary
{
    public class IntCollection
    {
        public List<int> list = new();
    }
    
    [Serializable]public class IntCollectionPort : RhSinglePortObjectType<IntCollection>{}
}