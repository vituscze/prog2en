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

    // c) Add an Unmove() method.
    Stack<Move> undo = new();

    public Game Clone()
    {
      // Mention: MemberwiseClone()
      Game g = new();
      g.board = (Player[,])board.Clone();
      g.moves = moves;
      g.Turn = Turn;
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
      // c)
      undo.Push(m);
      return true;
    }

    // c)
    public bool Unmove()
    {
      if (undo.Count == 0) return false;

      Move m = undo.Pop();
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
    // a) Keeping track of considered game states.
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
        g1.Move(m); // g.Move(m);
        int w = Minimax(g1, out Move? _);
        // g.Unmove();
        // a)
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
      // a)
      considered = 0;
      // b) Print out the number of milliseconds it takes to make each move.
      System.Diagnostics.Stopwatch sw = new();
      sw.Start();
      Minimax(g, out Move? best);
      // a)
      Console.WriteLine($"considered {considered} moves");
      // b)
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

    public int Turn;

    Stack<bool> undo = new();

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

      undo.Push(left);
      Turn = 3 - Turn;
      return true;
    }

    public bool Unmove()
    {
      if (start == 0 && end == coins.Length) return false;

      bool left = undo.Pop();
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

  class Program
  {
    static void Main(string[] args)
    {
      Tournament t = new(new MinimaxPlayer(), new RandomPlayer());
      t.Run(3);
    }

    static (int, int) CoinOptimal(CoinGame g)
    {
      if (g.Value() is (int, int) result)
        return result;

      // Both players are trying to maximize their values, we just need to select the proper one.
      int getValue((int, int) p) => g.Turn == 1 ? p.Item1 : p.Item2;

      g.Move(false);
      var falseValue = CoinOptimal(g);
      g.Unmove();
      g.Move(true);
      var trueValue = CoinOptimal(g);
      g.Unmove();

      return getValue(falseValue) > getValue(trueValue) ? falseValue : trueValue;
    }

    // Question: do we need to use a pair? Can't we just return a single number and use the standard minimax?
  }
}
