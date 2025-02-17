using System;
using System.Collections.Generic;
using System.Linq;

namespace Tutorial
{
  class Program
  {
    // Subsets, Revisited //

    // In the lecture we wrote a function that prints all subsets of {1, ..., N}
    // using a prefix parameter. Rewrite this function using any of the other 3 techniques
    // that we used for solving the abc problem.
    static public void SubsetsPrefix(int n)
    {
      void go(string prefix, int i)
      {
        if (i > n)
        {
          Console.WriteLine("{" + prefix + "}");
        }
        else
        {
          go(prefix + i + " ", i + 1);
          go(prefix, i + 1);
        }
      }

      go(" ", 1);
    }

    static public void SubsetsStack(int n)
    {
      Stack<int> s = new();

      void go(int i)
      {
        if (i > n)
        {
          Console.WriteLine("{ " + string.Join(" ", s.Reverse()) + " }");
        }
        else
        {
          s.Push(i);
          go(i + 1);
          s.Pop();
          go(i + 1);
        }
      }

      go(1);
    }

    static public void SubsetsArray(int n)
    {
      bool[] member = new bool[n];

      void go(int i)
      {
        if (i >= member.Length)
        {
          Console.Write("{ ");
          for (int j = 0; j < member.Length; j++)
          {
            if (member[j])
            {
              Console.Write($"{j + 1} ");
            }
          }
          Console.WriteLine("}");
        }
        else
        {
          member[i] = true;
          go(i + 1);
          member[i] = false;
          go(i + 1);
        }
      }

      go(0);
    }

    static public void SubsetsDigits(int n)
    {
      for (int i = 0; i < 1 << n; i++)
      {
        Console.Write("{ ");
        for (int j = 0; j < n; j++)
        {
          // Test j-th bit of the number i.
          if (((i >> j) & 1) != 0)
          {
            Console.Write($"{j + 1} ");
          }
        }
        Console.WriteLine("}");
      }
    }

    // Permutations //

    // Write a function void permutations(string s) that prints all permutations of a string.
    // For example, permutations("cat") might print
    //
    //   cat
    //   cta
    //   act
    //   atc
    //   tca
    //   tac
    static public void Permutations(string s)
    {
      // We start with an array filled with NUL characters,
      // which generally won't be in any string.
      char[] chars = new char[s.Length];

      void go(int i)
      {
        if (i >= chars.Length)
        {
          Console.WriteLine(new string(chars));
        }
        else
        {
          for (int j = 0; j < chars.Length; j++)
          {
            // Already used position, skip.
            if (chars[j] != 0) continue;

            chars[j] = s[i];
            go(i + 1);
            chars[j] = (char)0;
          }
        }
      }

      go(0);
    }

    // Mini-Sudoku //

    // Write a method that reads a mini-Sudoku puzzle from standard input.
    // Empty squares will be represented by the number 0:
    //
    //   3040
    //   0103
    //   2300
    //   1002
    //
    // The method should print out a solution to the puzzle if it can find one, otherwise null.
    static public void MiniSudoku()
    {
      int[,] sudoku = new int[4, 4];
      for (int i = 0; i < 4; i++)
      {
        string s = Console.ReadLine()!;
        for (int j = 0; j < 4; j++)
        {
          sudoku[i, j] = int.Parse($"{s[j]}");
        }
      }

      bool check(int x, int y, int h, int w)
      {
        int[] count = new int[4];
        for (int i = x; i < x + h; i++)
        {
          for (int j = y; j < y + w; j++)
          {
            if (sudoku[i, j] != 0) count[sudoku[i, j] - 1]++;
          }
        }

        // Checks for valid partial solutions - cells are allowed to be 0.
        return count.All(x => x <= 1);
      }

      bool check_sudoku()
      {
        for (int i = 0; i < 4; i++)
        {
          if (!check(i, 0, 1, 4)) return false;
          if (!check(0, i, 4, 1)) return false;
          if (!check(2 * (i / 2), 2 * (i % 2), 2, 2)) return false;
        }
        return true;
      }

      bool go(int i)
      {
        if (i >= 4 * 4)
        {
          // All cells have been filled, perform final check.
          return check_sudoku();
        }
        else
        {
          int x = i / 4;
          int y = i % 4;
          if (sudoku[x, y] != 0)
          {
            // Already filled, continue with the next one;
            return go(i + 1);
          }
          else
          {
            for (int candidate = 1; candidate <= 4; candidate++)
            {
              sudoku[x, y] = candidate;
              if (!check_sudoku()) continue;
              if (go(i + 1))
              {
                return true;
              }
            }
            sudoku[x, y] = 0;
            return false;
          }
        }
      }

      bool r = go(0);
      if (r)
      {
        for (int x = 0; x < 4; x++)
        {
          for (int y = 0; y < 4; y++)
          {
            Console.Write(sudoku[x, y]);
          }
          Console.WriteLine();
        }
      }
      else
      {
        Console.WriteLine("null");
      }
    }

    // Partitions //

    // A partition of a non-negative integer n is a sequence of positive integers that add up to n.
    // Order does not matter: 1 + 3 and 3 + 1 are the same partition of 4.
    //
    // Write a function partitions(n) that prints all compositions of an integer n.
    // For example, partitions(3) will print (in some order):
    //
    //   1 + 1 + 1
    //   2 + 1
    //   3
    static public void Partitions(int n)
    {
      Stack<int> s = new();

      void go(int rem, int max)
      {
        if (rem == 0)
        {
          Console.WriteLine(string.Join(" + ", s.Reverse().Select(x => x.ToString())));
        }
        else
        {
          for (int i = 1; i <= Math.Min(max, rem); i++)
          {
            s.Push(i);
            go(rem - i, i);
            s.Pop();
          }
        }
      }

      go(n, int.MaxValue);
    }

    // Combinations //

    // Write a function void combinations(int[] a, int k) that takes an array a and an integer k, and prints all combinations of k elements of a.For example, the call
    //
    //   int[] a = { 2, 4, 6, 8, 10 };
    //   combinations(a, 3);
    //
    // might produce this output:
    //
    //   2 4 6
    //   2 4 8
    //   2 4 10
    //   2 6 8
    //   2 6 10
    //   2 8 10
    //   4 6 8
    //   4 6 10
    //   4 8 10
    //   6 8 10
    static public void Combinations(int[] a, int k)
    {
      if (k > a.Length) return;
      if (k == 0)
      {
        // There is one trivial combination with 0 elements - the empty combination.
        Console.WriteLine("[]");
        return;
      }

      int[] res = new int[k];

      void go(int i, int rem)
      {
        if (rem == 0)
        {
          Console.WriteLine("[" + string.Join(",", res.Select(x => x.ToString())) + "]");
        }
        else if (i < a.Length)
        {
          res[^rem] = a[i];
          go(i + 1, rem - 1);
          go(i + 1, rem);
        }
        else
        {
          // Not enough elements to finish this particular combination.
          // Discussion: can we improve the conditions to make sure this case never happens?
          //
          //   if (a.Length - i - 1 >= rem) go(i + 1, rem);
        }
      }

      go(0, k);
    }

    static void Main(string[] args)
    {

    }
  }
}
