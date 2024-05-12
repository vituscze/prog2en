using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tutorial
{
  /* Ghost

    Consider the following game. Let W be a set of words, each containing only lowercase letters from 'a' to 'z'.
    Two players take turns adding a letter to a string S, which is initially empty. A player loses if after their turn either

        they have made a word (i.e. S is a word in W), or

        no more words are possible (i.e. S is not a prefix of any word in W).

    For example, suppose that W = { "chirp", "cat", "dark", "dog", "donate", "donut" }. The game might proceed as follows:

        A chooses d

        B chooses o

        A chooses n

        B chooses u

    Now A has lost: if he chooses 't' he has made the word 'donut', and he cannot choose any other letter since then no
    other word will be possible.

    Write a program that can play Ghost. Read the list of possible English words from this file. Only use words that consist
    only of lowercase letters a-z from the Latin alphabet. Your program should be able to play either as player 1 or 2.

    Can you defeat your own program? If not, make it easier by modifying your program to choose its first move randomly from
    the set of all possible moves that don't lose immediately.
  */
  class Ghost
  {
    private List<string> words; // In alphabetic order
    private int validStart;
    private int validEnd;
    private Stack<(int start, int end)> undo = new();

    public string Word { get; private set; } = "";
    public int Turn { get; private set; } = 1;

    public Ghost(List<string> unfiltered)
    {
      words = unfiltered.Where(w => w.All(c => c >= 'a' && c <= 'z')).ToList();
      words.Sort(string.CompareOrdinal); // Compare the numeric values of the characters
      validStart = 0;
      validEnd = words.Count - 1;
    }

    private int LowerBound(char c, int from, int to)
    {
      int len = Word.Length;
      while (from <= to)
      {
        int mid = from + (to - from) / 2;
        if (words[mid].Length <= len || words[mid][len] < c) from = mid + 1;
        else to = mid - 1;
      }
      return from;
    }

    // Since draws are impossible, zero indicates a game in progress.
    public int Value()
    {
      // No words remain or the current word is in the list: the current player wins
      if (validStart > validEnd || words[validStart] == Word) return Turn == 1 ? 1 : -1;

      // Otherwise, the game is still in progress
      return 0;
    }

    public void Move(char c)
    {
      undo.Push((validStart, validEnd));
      int lower = LowerBound(c, validStart, validEnd);
      int upper = LowerBound((char)(c + 1), validStart, validEnd);
      validStart = lower;
      validEnd = upper - 1;
      Word += c;
      Turn = 3 - Turn;
    }

    public void Unmove()
    {
      (validStart, validEnd) = undo.Pop();
      Word = Word[..^1];
      Turn = 3 - Turn;
    }

    // Technically, any character constitutes a move, but we're not
    // interested in characters that aren't a part of any word.
    public List<char> PossibleMoves()
    {
      char[] valid = new char[26];
      int len = Word.Length;
      for (int i = validStart; i <= validEnd; i++)
      {
        if (words[i].Length <= len) continue;
        valid[words[i][len] - 'a']++;
      }
      List<char> result = new();
      for (int i = 0; i < 26; i++)
        if (valid[i] > 0)
          result.Add((char)('a' + i));
      return result;
    }

    public void Debug()
    {
      Console.WriteLine("remaining words:");
      for (int i = validStart; i <= validEnd; i++)
        Console.WriteLine(words[i]);
      Console.WriteLine();
    }
  }

  interface IPlayer
  {
    char NextMove(Ghost g);
  }

  class MinimaxPlayer : IPlayer
  {
    private static int Minimax(Ghost g, out char? bestMove)
    {
      bestMove = null;
      int val = g.Value();
      if (val != 0) return val;

      bool maximizing = g.Turn == 1;
      int v = maximizing ? int.MinValue : int.MaxValue;

      foreach (char m in g.PossibleMoves())
      {
        g.Move(m);
        int w = Minimax(g, out char? _);
        g.Unmove();
        if (maximizing && w > v || !maximizing && w < v)
        {
          v = w;
          bestMove = m;
        }
        if (maximizing && v == 1 || !maximizing && v == -1)
          break;
      }

      return v;
    }

    public char NextMove(Ghost g)
    {
      Minimax(g, out char? bestMove);
      return (char)bestMove!;
    }
  }

  class HumanPlayer : IPlayer
  {
    public char NextMove(Ghost g)
    {
      Console.WriteLine("possible moves:");
      Console.WriteLine(string.Join(' ', g.PossibleMoves()));
      return Console.ReadLine()![0];
    }
  }

  class Program
  {
    static void PlayGhost()
    {
      // The file includes all letters of the English alphabet as words, which wouldn't make for
      // an interesting game.
      var lines = File.ReadAllLines(@"words").Where(w => w.Length > 1).ToList();

      IPlayer player1 = new HumanPlayer();
      IPlayer player2 = new MinimaxPlayer();

      Ghost g = new(lines);
      while (g.Value() == 0)
      {
        Console.WriteLine($"player: {g.Turn}");
        Console.WriteLine($"current word: {g.Word}");
        IPlayer current = g.Turn == 1 ? player1 : player2;
        char m = current.NextMove(g);
        Console.WriteLine($"selected move: {m}");
        g.Move(m);
        Console.WriteLine("-----");
      }
      string winner = g.Value() switch { 1 => "first player", -1 => "second player", _ => "" };
      Console.WriteLine($"{winner} wins");
    }

    /* Subtract a Divisor

      Consider the following 2-player game. There are N stones on a table. On each term, a player may take any number of stones
      that exactly divides N. The player who takes the last stone loses.

      For example, suppose that N = 10. The moves might proceed as follows:

          Player 1 takes 5 stones, leaving 5.

          Player 2 takes 1 stone, leaving 4.

          Player 1 takes 2 stones, leaving 2.

          Player 2 takes 1 stone, leaving 1.

          Player 1 must take the last stone and loses.

      a) Write a function that takes a positive integer N and returns true if player 1 can always win the game for the given N.

      b) Make your function faster for large N using dynamic programming.

      c) Call N "hot" if player 1 can win the game when played with N stones, otherwise "cold". For each value M in the range 1 .. 6,
         print out the percentage of values in the range 1 .. 10^M that are hot.
    */
    static bool Stones(int n)
    {
      bool go(int rem)
      {
        if (rem == 0) // The first player trivially wins when there are no stones
          return true;

        for (int i = 1; i <= rem; i++)
        {
          if (n % i != 0) continue; // Not a valid move
          if (!go(rem - i)) return true; // If any moves lead to a second player loss, the first player wins
        }
        return false;
      }

      return go(n);
    }

    static List<int> Divisors(int n)
    {
      List<int> divisors = new();
      for (int i = 1; i <= n; i++)
        if (n % i == 0)
          divisors.Add(i);
      return divisors;
    }

    static List<int> DivisorsFaster(int n)
    {
      List<int> divisors = new();
      int i;
      for (i = 1; i * i < n; i++)
      {
        if (n % i == 0)
        {
          divisors.Add(i);
          divisors.Add(n / i);
        }
      }
      if (i * i == n) divisors.Add(i);
      // Technically, we could build two lists and then just add them together,
      // which would be faster than sorting.
      divisors.Sort();
      return divisors;
    }

    static bool StonesFaster(int n)
    {
      // Precompute the divisors.
      List<int> divisors = DivisorsFaster(n);

      bool go(int rem)
      {
        if (rem == 0)
          return true;

        foreach (int div in divisors)
        {
          if (div > rem) break;
          if (!go(rem - div)) return true;
        }
        return false;
      }

      return go(n);
    }

    static bool StonesFastest(int n)
    {
      List<int> divisors = DivisorsFaster(n);

      bool[] results = new bool[n + 1];
      results[0] = true; // The first player wins

      for (int i = 1; i <= n; i++)
      {
        foreach (int div in divisors)
        {
          if (div > i) break;
          if (!results[i - div])
          {
            results[i] = true;
            break;
          }
        }
      }

      return results[n];
    }

    static void HotPct(int n)
    {
      int wins = 0;
      for (int i = 1; i <= n ; i++)
        if (StonesFastest(i)) wins++;
      Console.WriteLine(100.0 * wins / n);
    }

    static void Main(string[] args)
    {
      HotPct(10000);
    }
  }
}
