using System;
using System.Collections.Generic;
using System.Linq;

namespace Tutorial
{
  class Program
  {
    // Subset Sum //

    // Write a function that takes an array a[] of integers and an integer k,
    // and returns true if any subset of a (which need not be contiguous) has sum k.
    // Use dynamic programming.
    static public bool HasSum(int[] a, int k)
    {
      // sum[i] <=> there is a subset that sums to i
      bool[] sum = new bool[k + 1];
      sum[0] = true;

      // Keep track of the maximum possible sum since we
      // don't need to go all the way to k in every iteration.
      int max = 0;
      foreach (int elem in a)
      {
        for (int i = 0; i <= max; i++)
        {
          if (i + elem > k) break;
          if (sum[i]) sum[i + elem] = true;
        }

        max += elem;
      }

      return sum[k];
    }

    // Square Submatrix //

    // Write a function that takes a square matrix a[,] containing only 0s and 1s.
    // The method should return the offset and size of the largest square submatrix
    // of a that contains only 1s. Use dynamic programming.
    public struct Square
    {
      public int x;
      public int y;
      public int size;
    }

    static public Square LargestSubmatrix(int[,] a)
    {
      if (a.GetLength(0) != a.GetLength(1)) throw new ArgumentException("not a square matrix");
      int dim = a.GetLength(0);

      int[,] size = new int[dim, dim];
      // Initialize the size array by only considering submatrices
      // of size 1 along the edges.
      for (int i = 0; i < dim; i++)
      {
        if (a[i, 0] == 1) size[i, 0] = 1;
        if (a[0, i] == 1) size[0, i] = 1;
      }

      // Fill the inner part of the array.
      for (int i = 1; i < dim; i++)
      {
        for (int j = 1; j < dim; j++)
        {
          if (a[i, j] == 1) // Submatrix could be extended
          {
            size[i, j] = 1 + new int[] { size[i - 1, j], size[i, j - 1], size[i - 1, j - 1] }.Min();
          }
        }
      }

      // Alternatively, we could find the largest submatrix while filling
      // up the size array above.
      Square best = new();
      for (int i = 0; i < dim; ++i)
      {
        for (int j = 0; j < dim; ++j)
        {
          if (size[i, j] > best.size)
          {
            best.size = size[i, j];
            best.x = i - best.size + 1;
            best.y = j - best.size + 1;
          }
        }
      }
      return best;
    }

    // Text Segmentation //

    // a) Write a function that takes an array string words[] containing a set of words, plus
    // a string s. The function should return true if s can be written by concatenating one or
    // more words from the given set. A word may be used more than once in the concatenation.
    // For example, if words[] = { "pot", "potato", "to", "topo" } and s = "topottopopotatotopo",
    // the function will return true, since s = "to" + "pot" + "topo" + "potato" + "topo".
    // A solution that backtracks may have exponential time in the worst case. Use
    // dynamic programming for an efficient solution.
    static public bool Segmentation(string[] words, string s)
    {
      bool[] possible = new bool[s.Length + 1];
      // To make matters simpler, we allow the possibility of picking nothing
      // (to disallow it, just check if s == "" and handle it separately).
      possible[0] = true;

      for (int i = 1; i <= s.Length; ++i)
      {
        string part = s[..i];
        foreach (string word in words)
        {
          if (part.EndsWith(word) && possible[i - word.Length])
          {
            possible[i] = true;
          }
        }
      }

      return possible[s.Length];
    }

    // b) Modify your function so that it returns the number of ways that a string s may be written
    // by concatenating words from the given set.
    static public int SegmentationCount(string[] words, string s)
    {
      int[] ways = new int[s.Length + 1];
      ways[0] = 1;

      for (int i = 1; i <= s.Length; ++i)
      {
        string part = s[..i];
        foreach (string word in words)
        {
          if (part.EndsWith(word))
          {
            ways[i] += ways[i - word.Length];
          }
        }
      }

      return ways[s.Length];
    }

    // Longest Common Subsequence //

    // Write a function that takes two strings and returns the longest common subsequence of the
    // strings, i.e. the longest (not necessarily contiguous) subsequence that belongs to both strings.
    // Use dynamic programming.
    static public string LCS(string a, string b)
    {
      string[,] common = new string[a.Length + 1, b.Length + 1];
      // common[i, j] == a <=> a is the longest common subsequence of a[..i] and b[..j]

      // Fill the edges by setting them to empty strings.
      for (int i = 0; i <= a.Length; i++) common[i, 0] = "";
      for (int j = 1; j <= b.Length; j++) common[0, j] = ""; // We don't need to set common[0, 0] again.

      for (int i = 1; i <= a.Length; i++)
      {
        for (int j = 1; j <= b.Length; j++)
        {
          if (a[i - 1] == b[j - 1])
          {
            // Subsequence can be extended.
            common[i, j] = common[i - 1, j - 1] + a[i - 1];
          }
          else
          {
            common[i, j] = common[i - 1, j].Length > common[i, j - 1].Length ? common[i - 1, j] : common[i, j - 1];
          }
        }
      }

      // To consider: can we save some memory by not storing entire strings?
      return common[a.Length, b.Length];
    }

    static void Main(string[] args)
    {

    }
  }
}
