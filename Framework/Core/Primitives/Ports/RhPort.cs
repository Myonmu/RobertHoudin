using System;
using System.Collections.Generic;
using RobertHoudin.Framework.Core.Primitives.Nodes;
using UnityEngine;
using Type = System.Type;

namespace RobertHoudin.Framework.Core.Primitives.Ports
{
    public enum PortType
    {
        Input,
        Output
    }

    [Serializable]
    public abstract class RhPort
    {
        /// <summary>
        /// Whether this port should propagate node evaluation.
        /// When false, nodes connected with this port will not be evaluated.
        /// This usually happens for a datasource port that uses None or PropertyBlock
        /// data types.
        /// </summary>
        public virtual bool IsActive { get; } = true;
        [HideInInspector]public string name;
        public virtual PortType Type { get; }
        public abstract RhPort[] GetConnectedPorts();
        [HideInInspector] public RhNode node;
        [SerializeField][HideInInspector] private string _guid;
        public string GUID
        {
            get
            {
                #if UNITY_EDITOR
                if (string.IsNullOrEmpty(_guid))
                {
                    _guid = UnityEditor.GUID.Generate().ToString();
                }
                #endif

                return _guid;
            }
        }

        public virtual Type AcceptedType { get; }

        public abstract void SetValue(object value);
        public abstract object GetValue();

        public abstract void Connect(RhPort toPort);
        public abstract void Disconnect(RhPort port);
        public abstract void ForwardValue(RhPort target);

        public abstract RhPort GetPortAtIndex(int index);
        
        public abstract int GetConnectedPortCount();
        public abstract List<string> GetConnectedPortGuids();
    }
}