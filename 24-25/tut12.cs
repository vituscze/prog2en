using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Tutorial
{
  enum Player
  {
    None,
    Cross,
    Circle
  }

  class Move
  {
    public int x;
    public int y;

    public Move(int x, int y)
    {
      this.x = x;
      this.y = y;
    }
  }

  class Game
  {
    Player[,] board = new Player[3, 3];
    int moves = 0;

    public Player Turn = Player.Cross;

    public Game Clone()
    {
      Game g = (Game)MemberwiseClone();
      g.board = (Player[,])board.Clone();
      return g;
    }

    private bool CheckPlayer(Player player)
    {
      bool check(int x, int y, int dx, int dy)
      {
        for (int i = 0; i < 3; i++)
          if (board[x + i * dx, y + i * dy] != player)
            return false;
        return true;
      }

      for (int i = 0; i < 3; i++)
        if (check(0, i, 1, 0) || check(i, 0, 0, 1)) return true;
      return check(0, 0, 1, 1) || check(2, 0, -1, 1);
    }

    public Player? Winner()
    {
      if (CheckPlayer(Player.Cross)) return Player.Cross;
      if (CheckPlayer(Player.Circle)) return Player.Circle;
      return moves == 9 ? Player.None : null;
    }

    public bool Move(Move m)
    {
      if (board[m.x, m.y] != Player.None) return false;

      board[m.x, m.y] = Turn;
      Turn = Turn == Player.Cross ? Player.Circle : Player.Cross;
      moves++;
      return true;
    }

    // c) Add an Unmove() method.
    public bool Unmove(Move m)
    {
      if (moves == 0) return false;

      board[m.x, m.y] = Player.None;
      Turn = Turn == Player.Cross ? Player.Circle : Player.Cross;
      moves--;
      return true;
    }

    public List<Move> PossibleMoves()
    {
      List<Move> m = new();
      for (int x = 0; x < 3; x++)
        for (int y = 0; y < 3; y++)
          if (board[x, y] == Player.None)
            m.Add(new Move(x, y));
      return m;
    }

    public void Print()
    {
      for (int x = 0; x < 3; x++)
      {
        for (int y = 0; y < 3; y++)
          Console.Write(board[x, y] switch { Player.None => " ", Player.Circle => "o", Player.Cross => "x", _ => "" });
        Console.WriteLine();
      }
    }
  }

  interface IPlayer
  {
    Move NextMove(Game g);
  }

  class RandomPlayer : IPlayer
  {
    Random rng = new();

    public Move NextMove(Game g)
    {
      var moves = g.PossibleMoves();
      return moves[rng.Next(moves.Count)];
    }
  }

  class MinimaxPlayer : IPlayer
  {
    int considered;

    int Minimax(Game g, out Move? best)
    {
      best = null;
      if (g.Winner() is Player p)
        return p switch { Player.Cross => 1, Player.Circle => -1, _ => 0 };

      bool maximizing = g.Turn == Player.Cross;
      int v = maximizing ? int.MinValue : int.MaxValue;

      foreach (Move m in g.PossibleMoves())
      {
        Game g1 = g.Clone();
        g1.Move(m);
        int w = Minimax(g1, out Move? _);

        /*
        g.Move(m);
        int w = Minimax(g, out Move? _);
        g.Unmove(m);
        */

        considered++;
        if (maximizing && w > v || !maximizing && w < v)
        {
          v = w;
          best = m;
        }
        // d) Extreme value pruning
        if (maximizing && v == 1 || !maximizing && v == -1)
          break;
      }

      return v;
    }

    public Move NextMove(Game g)
    {
      considered = 0;
      System.Diagnostics.Stopwatch sw = new();
      sw.Start();
      Minimax(g, out Move? best);
      Console.WriteLine($"considered {considered} moves");
      Console.WriteLine($"took {sw.ElapsedMilliseconds} ms");
      Console.WriteLine("---");
      return best!;
    }
  }

  class HumanPlayer : IPlayer
  {
    public Move NextMove(Game g)
    {
      g.Print();
      Console.WriteLine("---");
      Move m = new(0, 0);
      var g2 = g.Clone();
      do
      {
        m.x = int.Parse(Console.ReadLine()!);
        m.y = int.Parse(Console.ReadLine()!);
      } while (!g2.Move(m));
      return m;
    }
  }

  class Tournament
  {
    public IPlayer cross;
    public IPlayer circle;

    public Tournament(IPlayer cross, IPlayer circle)
    {
      this.cross = cross;
      this.circle = circle;
    }

    public void Run(int rounds = 1)
    {
      string name(Player p) => p switch { Player.Cross => "cross", Player.Circle => "circle", _ => "none" };

      for (int i = 0; i < rounds; i++)
      {
        Game g = new();
        while (true)
        {
          if (g.Winner() is Player p)
          {
            Console.WriteLine($"game {i}: winner = {name(p)}");
            break;
          }
          g.Move((g.Turn == Player.Cross ? cross : circle).NextMove(g));
        }
      }
    }
  }

  // Coin Game //

  // Consider the following game.There is a row of N coins on the table, with values V1, V2, ... VN.
  // On each player's turn, they can take either the first coin in the row or the last coin in the row.
  // Write a method that takes an array with the values of the coins, and returns the maximum amount
  // of money that the first player can win if both players play optimally.
  class CoinGame
  {
    int[] coins;
    int start;
    int end;

    int firstScore = 0;
    int secondScore = 0;

    public int Turn { get; private set; }

    public CoinGame(int[] coins)
    {
      this.coins = coins;
      start = 0;
      end = coins.Length;
      Turn = 1;
    }

    public bool Move(bool left)
    {
      if (start == end) return false;
      int score;
      if (left)
      {
        score = coins[start];
        start++;
      }
      else
      {
        score = coins[end - 1];
        end--;
      }

      if (Turn == 1) firstScore += score;
      else secondScore += score;

      Turn = 3 - Turn;
      return true;
    }

    public bool Unmove(bool left)
    {
      if (left && start == 0 || !left && end == coins.Length) return false;

      int score;
      if (left)
      {
        start--;
        score = coins[start];
      }
      else
      {
        end++;
        score = coins[end - 1];
      }

      Turn = 3 - Turn;

      if (Turn == 1) firstScore -= score;
      else secondScore -= score;

      return true;
    }

    public (int, int)? Value() => start == end ? (firstScore, secondScore) : null;
  }

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

  interface IGhostPlayer
  {
    char NextMove(Ghost g);
  }

  class MinimaxGhostPlayer : IGhostPlayer
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

  class HumanGhostPlayer : IGhostPlayer
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
    static void Main(string[] args)
    {
      // Tic-tac-toe
      Tournament t = new(new MinimaxPlayer(), new RandomPlayer());
      t.Run(3);

      // Coin game
      Console.WriteLine(CoinOptimal(new CoinGame([10, 20, 15])).Item1);

      // Ghost game
      PlayGhost();
    }

    static (int, int) CoinOptimal(CoinGame g)
    {
      if (g.Value() is (int, int) result)
        return result;

      // Both players are trying to maximize their values, we just need to select the proper one.
      int getValue((int, int) p) => g.Turn == 1 ? p.Item1 : p.Item2;

      g.Move(false);
      var falseValue = CoinOptimal(g);
      g.Unmove(false);
      g.Move(true);
      var trueValue = CoinOptimal(g);
      g.Unmove(true);

      return getValue(falseValue) > getValue(trueValue) ? falseValue : trueValue;
    }

    // Question: do we need to use a pair? Can't we just return a single number and use the standard minimax?
    // Question: this algorithm is clearly exponential, can we do better?

    static void PlayGhost()
    {
      // The file includes all letters of the English alphabet as words, which wouldn't make for
      // an interesting game.
      var lines = File.ReadAllLines(@"words").Where(w => w.Length > 1).ToList();

      IGhostPlayer player1 = new HumanGhostPlayer();
      IGhostPlayer player2 = new MinimaxGhostPlayer();

      Ghost g = new(lines);
      while (g.Value() == 0)
      {
        Console.WriteLine($"player: {g.Turn}");
        Console.WriteLine($"current word: {g.Word}");
        IGhostPlayer current = g.Turn == 1 ? player1 : player2;
        char m = current.NextMove(g);
        Console.WriteLine($"selected move: {m}");
        g.Move(m);
        Console.WriteLine("-----");
      }
      string winner = g.Value() switch { 1 => "first player", -1 => "second player", _ => "" };
      Console.WriteLine($"{winner} wins");
    }
  }
}
