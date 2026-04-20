using System;
using System.Collections.Generic;
using System.Linq;

namespace Tutorial
{
  class Program
  {
    // Permutations via swapping. Result is returned in a list.
    static public List<string> Permutations(string s)
    {
      char[] chars = s.ToCharArray();
      List<string> final = new();

      void go(int i)
      {
        if (i >= chars.Length)
        // For i == chars.Length - 1, we'll be swapping the last
        // element with itself. There's clearly no need to do that so
        // we can stop the loop when i + 1 >= chars.Length instead.
        {
          final.Add(new(chars));
        }
        else
        {
          for (int j = i; j < chars.Length; j++)
          {
            (chars[i], chars[j]) = (chars[j], chars[i]);
            go(i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
          }
        }
      }

      go(0);
      return final;
    }

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
    static public void Garden(int n)
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

    static public int Ways(int n)
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
    static public int Jumps(int[] jumps)
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

    static public (int, string) JumpsStr(int[] jumps)
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

    // Smallest Number of Coins

    // a) Write a function that takes an array coins[] with the denominations of various coins, plus an integer sum.
    // The method should return the smallest number of coins that can form the given sum. Each denomination may be used
    // any number of times in the sum. For example, if coins is [1, 3, 4] and sum is 6, the function will return 2 since
    // there are 2 coins in the sum 3 + 3.

    // b) Modify your function so that it returns an array with the actual coin values needed to form the sum minimally.
    static int FewestCoins(int sum, int[] coins)
    {
      int[] best = new int[sum + 1];
      best[0] = 0;  // Empty sum doesn't need any coins

      for (int tgt = 1; tgt <= sum; tgt++)
      {
        int tgtBest = -1;
        foreach (int coin in coins)
        {
          if (tgt - coin < 0) continue;  // If the array is sorted, we could break instead
          if (best[tgt - coin] == -1) continue;  // The smaller sum cannot be made

          int current = best[tgt - coin] + 1;  // Use the current coin
          if (tgtBest == -1 || current < tgtBest)
            tgtBest = current;
        }
        best[tgt] = tgtBest;
      }

      return best[sum];
    }

    static List<int>? FewestCoinsList(int sum, int[] coins)
    {
      int[] best = new int[sum + 1];
      best[0] = 0;  // Empty sum doesn't need any coins
      int[] choices = new int[sum + 1];  // Which coin to take to get us to the given sum

      for (int tgt = 1; tgt <= sum; tgt++)
      {
        int tgtBest = -1;
        foreach (int coin in coins)
        {
          if (tgt - coin < 0) continue;  // If the array is sorted, we could break instead
          if (best[tgt - coin] == -1) continue;  // The smaller sum cannot be made

          int current = best[tgt - coin] + 1;  // Use the current coin
          if (tgtBest == -1 || current < tgtBest)
          {
            tgtBest = current;
            choices[tgt] = coin;
          }
        }
        best[tgt] = tgtBest;
      }

      if (best[sum] == -1) return null;  // Sum cannot be made

      int rem = sum;
      List<int> result = new();
      while (rem > 0)
      {
        result.Add(choices[rem]);
        rem -= choices[rem];
      }
      return result;
    }

    static void Main(string[] args)
    {

    }
  }
}
