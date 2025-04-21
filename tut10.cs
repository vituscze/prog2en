using System;
using System.Collections.Generic;
using System.Linq;

namespace Tutorial
{
  class Program
  {
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
      // common[i, j] == s <=> s is the longest common subsequence of a[..i] and b[..j]

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

    // Dropping Eggs //

    // Consider the following problem: we're in a building with F floors and
    // we have E eggs. We want to figure out what's the highest floor
    // we can drop an egg from without it breaking.
    //
    // Few assumptions to make our lives easier:
    //
    // a) the eggs are identical: if an egg survives a drop from some floor,
    //    all other eggs will do as well
    // b) if an egg doesn't survive a drop from some floor, it won't survive
    //    a drop from any higher floor; similarly, if it survives a drop from
    //    some floor, it will survive a drop from any lower floor
    //
    // How many drops do we have to do to correctly identify the critical
    // floor (even in the worst case)?
    //
    // Some hints: What do we have to do when the building only has one floor?
    // What if we only have one egg?
    static public int MinDrops(int floors, int eggs)
    {
      Dictionary<(int floor, int egg), int> memoized = new();

      int go(int floors, int eggs)
      {
        if (floors == 0) return 0;
        if (eggs == 0) return int.MaxValue;
        if (floors == 1) return 1;
        if (eggs == 1) return floors;

        var key = (floors, eggs);
        if (memoized.TryGetValue(key, out int value))
        {
          return value;
        }

        int result = int.MaxValue;
        for (int floor = 1; floor <= floors; floor++)
        {
          int candidate = Math.Max(go(floor - 1, eggs - 1), go(floors - floor, eggs));
          if (candidate < result) result = candidate;
        }

        result++;
        memoized[key] = result;
        return result;
      }

      // Unlike in the previous cases, we cannot easily extract the choices --
      // they depend on what the critical floor is. However, we can extract a *strategy*
      // (if you tell me how many unexplored floors there are and how many eggs you have,
      // I can tell you where to drop the next egg from).
      return go(floors, eggs);
    }

    // Introduction to game playing, minimax
    //
    // How to avoid code such as
    //   if (player.type == HUMAN) { ... }
    //   else if (player.type == AI) { ... }
    //
    // Object-oriented design, SOLID
    //  * Single responsibility principle
    //  * Open-closed principle
    //  * Liskov substitution principle
    //  * Interface segregation principle
    //  * Dependency inversion principle

    static void Main(string[] args)
    {
    }
  }
}
