using System;
using System.Collections.Generic;
using System.Linq;

namespace Tutorial
{
  class Program
  {
    // Coin Sums //

    // Write a function that takes an integer N and returns the number of possible ways
    // to form a sum of N crowns using Czech coins(which have denominations 1 Kč, 2 Kč,
    // 5 Kč, 10 Kč, 20 Kč, 50 Kč). Order matters: 1 + 5 and 5 + 1 are distinct sums.
    // Use dynamic programming for an efficient solution.
    static long CoinSumsBad(long amount)
    {
      long[] coins = new long[] { 1, 2, 5, 10, 20, 50 };
      long go(long i)
      {
        if (i == 0) return 1; // Exactly one way to pay 0 Kč

        long total = 0;
        foreach (long coin in coins)
        {
          if (coin > i) break;
          total += go(i - coin);
        }
        return total;
      }

      return go(amount);
    }

    static long CoinSumsDict(long amount)
    {
      long[] coins = new long[] { 1, 2, 5, 10, 20, 50 };
      Dictionary<long, long> d = new();
      d[0] = 1; // Exactly one way to pay 0 Kč

      long go(long i)
      {
        if (d.TryGetValue(i, out long res)) return res;
        long total = 0;
        foreach (long coin in coins)
        {
          if (coin > i) break;
          total += go(i - coin);
        }
        d[i] = total;
        return total;
      }

      // Be careful that the results grow really fast. Amounts larger than ~82 will
      // overflow even the 64bit longs.
      return go(amount);
    }
    // Advantage: only computes what's necessary
    // Disadvantage: dictionaries have nontrivial overhead

    static long CoinSumsArray(long amount)
    {
      long[] coins = new long[] { 1, 2, 5, 10, 20, 50 };
      long[] ways = new long[amount + 1];
      ways[0] = 1; // Exactly one way to pay 0 Kč
      for (long i = 1; i <= amount; i++)
      {
        long total = 0;
        foreach (long coin in coins)
        {
          if (coin > i) break;
          total += ways[i - coin];
        }
        ways[i] = total;
      }
      return ways[amount];
    }
    // Advantage: arrays are fast
    // Disadvantage: might compute values we don't need

    // Carrots and Parsley //

    // A garden contains N plant beds, all in a row.Each bed will contain either carrots
    // or parsley, but no two neighboring beds may both contain parsley.

    // a) Write a function void garden(int n) that takes N, the size of the garden, and prints
    // out all possible ways that exist to plant these vegetables in the garden.
    // For example, garden(5) might print

    //   C C C
    //   P C C
    //   C P C
    //   C C P
    //   P C P

    // b) Write a function int ways(int n) that takes N, the size of the garden, and returns the
    // number of possible ways that exist to plant these vegetables in the garden. For example,
    // ways(5) will print 5.
    static void Garden(int n)
    {
      Stack<char> s = new();
      void go(int i, bool allow_parsley)
      {
        if (i == 0)
        {
          Console.WriteLine(string.Join(' ', s.Reverse()));
        }
        else
        {
          s.Push('C'); // Carrot is always allowed
          go(i - 1, true);
          s.Pop();
          if (allow_parsley)
          {
            s.Push('P');
            go(i - 1, false);
            s.Pop();
          }
        }
      }
      go(n, true);
    }

    static int Ways(int n)
    {
      int[] ways = new int[n + 1];
      ways[0] = 1; // Exactly one way to set up an empty garden.
      ways[1] = 2; // For a garden with a single bed, we can plant either carrots or parsley.

      for (int i = 2; i <= n; i++)
      {
        int total = 0;
        // a) Plant carrots in the last bed
        total += ways[i - 1];
        // b) Plant parsley in the last bed, which means the previous bed must have carrots.
        total += ways[i - 2];
        ways[i] = total;
      }
      return ways[n];
    }

    // Minimum Number of Jumps //

    // Consider this problem: we are given a horizontal array of squares, each with an integer.
    // For example the array might be
    //
    //   2 3 1 1 2
    //
    // A button begins on the leftmost square. At each step, if the button is on a square with the
    // number k then the button may jump to the right by any number of squares that is less than
    // or equal to k. For example, with the array above, in the first step the button may jump
    // right either by 1 or 2 squares. We wish to determine the smallest number of steps needed for
    // the button to jump past the right edge of the array. For example, with the array above, this
    // number is 3, since the button can jump right by 1, then by 3, then by 1 or 2.

    // a) Write a function jumps() that takes an array of integers, and returns the smallest number of
    // steps needed to jump past the right edge.

    // b) Modify your function so that it also returns a string containing the sizes of the jumps to take
    // to achieve this minimum.
    static int Jumps(int[] jumps)
    {
      int[] steps = new int[jumps.Length + 1];
      steps[^1] = 0; // Past the end: 0 steps required (this isn't strictly needed since steps is init'd to zero).

      for (int i = jumps.Length - 1; i >= 0; i--)
      {
        int dist = jumps[i];
        int best = int.MaxValue;
        for (int j = 1; j <= dist; j++)
        {
          if (i + j >= steps.Length) break; // Jumping two or more positions past the end.
          best = Math.Min(best, steps[i + j]);
        }
        steps[i] = best + 1;
      }

      return steps[0];
    }

    static (int, string) JumpsStr(int[] jumps)
    {
      int[] steps = new int[jumps.Length + 1];
      int[] choices = new int[jumps.Length + 1]; // Keeps track of the best choice for each position
      steps[^1] = 0; // Past the end: 0 steps required (this isn't strictly needed since steps is init'd to zero).

      for (int i = jumps.Length - 1; i >= 0; i--)
      {
        int dist = jumps[i];
        int best = int.MaxValue;
        int bestChoice = -1;
        for (int j = 1; j <= dist; j++)
        {
          if (i + j >= steps.Length) break; // Jumping two or more positions past the end.
          if (steps[i + j] < best)
          {
            best = steps[i + j];
            bestChoice = j;
          }
        }
        steps[i] = best + 1;
        choices[i] = bestChoice;
      }

      string bestWay = "";
      int ix = 0;
      while (ix < jumps.Length)
      {
        int choice = choices[ix];
        bestWay += $"{choice} ";
        ix += choice;
      }
      return (steps[0], bestWay.Trim());
    }

    static void Main(string[] args)
    {

    }
  }
}
