using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
namespace NoobCore.Tests.Algorithms
{
    [TestFixture]
    public class ArrayTests
    {
        /// <summary>
        /// 将二维矩阵原地顺时针旋转 90 度
        /// </summary>
        /// <param name="matrix"></param>
        public void Rotate(int[][] matrix)
        {
            int n = matrix.Length;
            // 先沿对角线镜像对称二维矩阵
            for (int i = 0; i < n; i++)
            {
                for (int j = i; j < n; j++)
                {
                    // swap(matrix[i][j], matrix[j][i]);
                    int temp = matrix[i][j];
                    matrix[i][j] = matrix[j][i];
                    matrix[j][i] = temp;
                    
                }
            }
            // 然后反转二维矩阵的每一行
            foreach (int[] row in matrix)
            {
                Array.Reverse(row);
            }
        }

        // 反转一维数组
        /// <summary>
        /// 反转一维数组
        /// </summary>
        /// <param name="arr"></param>
        private void Reverse(int[] arr)
        {
            int i = 0, j = arr.Length - 1;
            while (j > i)
            {
                // swap(arr[i], arr[j]);
                int temp = arr[i];
                arr[i] = arr[j];
                arr[j] = temp;
                i++;
                j--;
            }
        }
    }
}
