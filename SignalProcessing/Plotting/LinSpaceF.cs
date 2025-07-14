using System.Collections;
using System.Collections.Generic;
using Unity.Burst;

namespace RobertHoudin.SignalProcessing.Plotting
{
    public class LinSpaceF: IEnumerable<float>
    {
        public float start;
        public float end;
        public float step;

        public LinSpaceF(float end, float step):this(0, end, step)
        {
        }
        public LinSpaceF(float start, float end, float step)
        {
            this.start = start;
            this.end = end;
            this.step = step;
        }
        
        public IEnumerator<float> GetEnumerator()
        {
            return new LinSpaceEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public struct LinSpaceEnumerator : IEnumerator<float>
    {
        private LinSpaceF _linSpace;
        private int _evalCounter;
        public LinSpaceEnumerator(LinSpaceF linSpace)
        {
            _linSpace = linSpace;
            _evalCounter = 0;
            Current = _linSpace.start;
        }
        
        [BurstDiscard]
        public bool MoveNext()
        {
            Current =  _linSpace.start + _linSpace.step * _evalCounter;
            _evalCounter++;
            return Current < _linSpace.end;
        }

        public void Reset()
        {
            _evalCounter = 0;
        }

        public float Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }
    }
}