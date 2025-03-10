﻿using System;
using System.Collections.Generic;
namespace RobertHoudin.Utils.RuntimeCompatible
{
    public static class ListUtils
    {
        public static void Resize<T>(this List<T> list, int newSize) where T : new()
        {
            for (var i = list.Count; i < newSize; i++)
            {
                list.Add(new T());
            }
            for (var i = list.Count; i > newSize; i--)
            {
                list.RemoveAt(i - 1);
            }
        }

        public static void Resize<T>(this List<T> list, int newSize
            , Func<T> constructor, Action<T> destructor)
        {
            for (var i = list.Count; i < newSize; i++)
            {
                list.Add(constructor());
            }
            for (var i = list.Count; i > newSize; i--)
            {
                destructor(list[i - 1]);
                list.RemoveAt(i - 1);
            }
        }
    }
}