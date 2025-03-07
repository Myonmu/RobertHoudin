using System;
using System.Collections.Generic;
namespace RobertHoudin.Utils.RuntimeCompatible
{
    public enum ClampSearchMode
    {
        None,
        StrictLowerBound,
        StrictUpperBound
    }
    /// <summary>
    /// Contains methods for searching values clamping a given value in a list.
    /// List must be SORTED.
    /// </summary>
    public static class ClampSearch
    {
        /// <summary>
        /// Finds 2 indices with values clamping the eval point. (Binary search under the hood)
        /// </summary>
        /// <param name="list">a sorted list</param>
        /// <param name="evalPoint">value to clamp</param>
        /// <param name="before">index of element with value just below the eval point</param>
        /// <param name="after">index of element with value just above the eval point</param>
        /// <typeparam name="T">list element type</typeparam>
        /// <exception cref="Exception">list is empty</exception>
        public static void FindClampingIndices<T>(List<T> list, T evalPoint, out int before, out int after, ClampSearchMode mode = ClampSearchMode.None)
            where T : IComparable<T>
        {
            if (list.Count == 0) throw new Exception("List is empty");
            var i = list.Count - 1;
            after = i;
            before = 0;
            while (after - before > 1)
            {
                var compareResult = list[i].CompareTo(evalPoint);
                if (mode is ClampSearchMode.StrictLowerBound && compareResult < 0 ||
                    mode is not ClampSearchMode.StrictLowerBound && compareResult <= 0)
                {
                    before = i;
                    i = (i + list.Count - 1) / 2;
                }
                else if(mode is ClampSearchMode.StrictUpperBound && compareResult > 0 ||
                        mode is not ClampSearchMode.StrictUpperBound && compareResult >= 0)
                {
                    after = i;
                    i /= 2;
                }
            }
        }

        /// <summary>
        /// Finds 2 indices with values clamping the eval point. (Binary search under the hood)
        /// </summary>
        /// <param name="list">sorted list</param>
        /// <param name="fieldSelector">extract the interested value from list element</param>
        /// <param name="evalPoint">value to clamp</param>
        /// <param name="before">index of element with value just below the eval point</param>
        /// <param name="after">index of element with value just above the eval point</param>
        /// <typeparam name="T">list element type</typeparam>
        /// <typeparam name="U">value type</typeparam>
        /// <exception cref="Exception"></exception>
        public static void FindClampingIndices<T, U>(List<T> list, Func<T, U> fieldSelector,
            U evalPoint, out int before, out int after, ClampSearchMode mode = ClampSearchMode.None) where U : IComparable<U>
        {
            if (list.Count == 0) throw new Exception("List is empty");
            var i = list.Count - 1;
            after = i;
            before = 0;
            while (after - before > 1)
            {
                var compareResult = fieldSelector(list[i]).CompareTo(evalPoint);
                if (mode is ClampSearchMode.StrictLowerBound && compareResult < 0 ||
                    mode is not ClampSearchMode.StrictLowerBound && compareResult <= 0)
                {
                    before = i;
                    i = (i + list.Count - 1) / 2;
                }
                else if(mode is ClampSearchMode.StrictUpperBound && compareResult > 0 ||
                        mode is not ClampSearchMode.StrictUpperBound && compareResult >= 0)
                {
                    after = i;
                    i /= 2;
                }
            }
        }
    }
}