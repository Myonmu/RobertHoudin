using System.Collections.Generic;
using UnityEngine;

namespace RobertHoudin.Framework.Core.Primitives.Events
{
    [CreateAssetMenu(fileName = "RhEvent", menuName = "RobertHoudin/Rh Event")]
    public class RhEvent : ScriptableObject, IListener, ISubscribable
    {
        [TextArea] [SerializeField] private string eventDescription;
        public bool isActive = true;
        private readonly List<IListener> subscribers = new();

        public void Update()
        {
            if (!isActive) return;
            foreach (var t in subscribers)
                t.Update();
        }
        public void Subscribe(IListener listener)
        {
            subscribers.Add(listener);
        }
        public void Unsubscribe(IListener listener)
        {
            subscribers.Remove(listener);
        }

        public static RhEvent operator +(RhEvent evt, IListener sub)
        {
            evt.subscribers.Add(sub);
            return evt;
        }

        public static RhEvent operator -(RhEvent evt, IListener sub)
        {
            evt.subscribers.Remove(sub);
            return evt;
        }
    }
}