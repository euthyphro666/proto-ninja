using System.Collections.Generic;

namespace SomethingSpecific.ProtoNinja
{
    public static class Extensions
    {

        public static int[] GetUniqueRandomIndexArray<T>(this T[] arr, int count)
        {
            int[] result = new int[count];
            var numbersInOrder = new List<int>();
            for (var i = 0; i < arr.Length; i++)
            {
                numbersInOrder.Add(i);
            }
            for (var i = 0; i < count; i++)
            {
                var randomIndex = UnityEngine.Random.Range(0, numbersInOrder.Count);
                result[i] = numbersInOrder[randomIndex];
                numbersInOrder.RemoveAt(randomIndex);
            }
            return result;
        }
    }
}