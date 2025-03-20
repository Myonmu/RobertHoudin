using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
namespace RobertHoudin.Framework.Core.Primitives.Utilities
{
    public class NumberBuffer
    {
        private List<Number> _numbers = new();
        private int _stride;
        public int Count => _numbers.Count / _stride;
        public int Stride => _stride;

        public Vector this[int index]
        {
            get => GetVectorAt(index);
            set => SetVectorAt(index, value);
        }

        public NumberBuffer()
        {
            _stride = 1;
        }

        public NumberBuffer(int stride)
        {
            _stride = stride;
        }

        public Number GetNumberAt(int index)
        {
            return _numbers[index];
        }

        public Vector GetVectorAt(int index)
        {
            if (index >= Count) throw new IndexOutOfRangeException();
            var startIndex = index * _stride;
            var result = Vector.Alloc(_stride);
            for (int i = 0; i < _stride; i++)
            {
                result[i] = GetNumberAt(startIndex + i);
            }
            return result;
        }

        public void SetVectorAt(int index, Vector vector)
        {
            if (index >= Count) throw new IndexOutOfRangeException();
            var startIndex = index * _stride;
            for (int i = 0; i < _stride; i++)
            {
                _numbers[startIndex + i] = vector[i];
            }
        }

        public void Add(Number number)
        {
            _numbers.Add(number);
        }

        public void Add(Vector vector)
        {
            lock (_numbers)
            {
                // this will overwrite existing "excessive" items.
                var writeStart = Count * _stride;
                for (int i = 0; i < _stride; i++)
                {
                    if (writeStart + i >= _numbers.Count)
                    {
                        _numbers.Add(vector[i]);
                    }
                    else
                    {
                        _numbers[writeStart + i] = vector[i];
                    }
                }
            }

        }

        public void RemoveAt(int index)
        {
            _numbers.RemoveRange(index * _stride, _stride);
        }

        public void AddRange(NumberBuffer other)
        {
            lock (_numbers)
            {
                if (_stride != other._stride)
                {
#if UNITY_EDITOR
                    Debug.LogWarning("Attempt to append number buffer with another buffer with different stride." +
                                     " This may lead to unexpected behavior. By default, the stride of this buffer (lhs) is respected.");
#endif
                    // prealloc
                    for (var i = 0; i < other.Count * Stride; i++)
                    {
                        _numbers.Add(new Number());
                    }
                    var writeStart = Count;
                    for (int i = 0; i < other.Count; i++)
                    {
                        this[writeStart + i] = other[i];
                    }
                }
                else
                {
                    _numbers.AddRange(other._numbers);
                }
            }
        }
        public void Clear(int newStride = -1)
        {
            if (newStride > 0) _stride = newStride;
            _numbers.Clear();
        }
    }
}