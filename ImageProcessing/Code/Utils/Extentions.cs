using MathLab.Entities;
using System;
using System.Text;

namespace Lab_4
{
    public static class FloatArrayExtention
    {
        public static float GetMax(this float[] array)
        {
            float result = array[0];
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i] > result)
                    result = array[i];
            }
            return result;
        }

        public static float[] Subtract(this float[] array, float[] with)
        {
            float[] result = (float[])array.Clone();
            for(int i = 0; i < array.Length; i++)
            {
                result[i] -= with[i];
            }
            return result;
        }

        public static float[] PasteValues(this float[] array, float[] from)
        {
            for(int i = 0; i < array.Length; i++)
            {
                array[i] = from[i];
            }

            return array;
        }

        public static float[] Abs(this float[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = Math.Abs(array[i]);
            }

            return array;
        }
    }
}
