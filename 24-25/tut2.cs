using System;

namespace ConsoleApp1
{
  class Program
  {
    // New ReCodEx assignment //

    // S Begins With T //
    //
    // Write a program that reads two strings S and T, each on its own line.
    // The program should print "true" if S starts with T, "false" otherwise.
    // Do not use any library functions.
    static bool Begins(string s, string t)
    {
      // If you want to read the strings from the standard input, you'd just
      // call the function like so: Begins(Console.ReadLine()!, Console.ReadLine()!)

      if (t.Length > s.Length)
      {
        return false;
      }

      for (int i = 0; i < t.Length; i++)
      {
        if (s[i] != t[i])
        {
          return false;
        }
      }

      return true;
    }

    // Printing with Commas //
    //
    // Write a program that reads a single line containing an integer which may be
    // arbitrarily large, and writes the integer with embedded commas.
    //
    // Input: 28470562348756298345
    // Output: 28,470,562,348,756,298,345
    static string AddCommas(string s)
    {
      int l = s.Length;
      // Because strings are immutable, this isn't very efficient.
      // In C#, we'd have to use a special StringBuilder class that we'll
      // talk about later.
      string r = "";

      for (int i = 0; i < s.Length; i++)
      {
        r += s[i];
        // Order of the digit we're currently considering.
        // Only add a comma if the order is divisible by three, except
        // for units which don't need a comma.
        int pos = l - i - 1;
        if (pos > 0 && pos % 3 == 0)
        {
          r += ",";
        }
      }

      return r;
    }

    // Sorting Two Values //
    //
    // Write a program that reads a sequence of integers on a single line. The sequence
    // is guaranteed to contain at most two distinct values. For example, it might be
    //
    // 17 17 17 11 17 11 11 11 17 11 17 11 11
    //
    // The program should print out the sequence in sorted order:
    //
    // 11 11 11 11 11 11 11 17 17 17 17 17 17
    //
    // How efficient is your program?
    static void Solve()
    {
      // Just like in Python, we can split a string given a separator.
      // There are a ton of possible ways of calling Split, here we just
      // need the simplest one where we give it a single character as
      // the separator.
      string[] line = Console.ReadLine()!.Split(' ');

      // If you want something closer to the Python split() (without any
      // arguments), you can use the following:
      //
      // Split(null as char[], StringSplitOptions.RemoveEmptyEntries)
      //
      // Explanation: Split with null array splits on any whitespace (spaces,
      // tabs, newlines). If you have two spaces in a row, it would add an
      // empty string to the result, hence the RemoveEmptyEntries option.

      // We can (and might want to) use the same option for the first split:
      //
      // string[] line = Console.ReadLine()!.Split(' ', StringSplitOptions.RemoveEmptyEntries);

      // int vs int? - the latter can contain a special value "null"
      // (a bit like None in Python).
      int? first = null;
      int firstCount = 0;
      int? second = null;
      int secondCount = 0;

      foreach (string s in line)
      {
        int num = int.Parse(s);
        if (first == null || first == num)
        {
          first = num;
          firstCount++;
        }
        else if (second == null || second == num)
        {
          second = num;
          secondCount++;
        }
        else
        {
          // More than 2 unique values.
        }
      }

      // We'll be printing firstCount copies of first at the start, make sure first contains
      // the smaller value.
      if (first > second)
      {
        (first, firstCount, second, secondCount) = (second, secondCount, first, firstCount);
      }

      for (int i = 0; i < firstCount; i++)
      {
        Console.Write(first);
        Console.Write(" ");
      }
      for (int i = 0; i < secondCount; i++)
      {
        Console.Write(second);
        Console.Write(" ");
      }

      // Small improvement: don't print the space after the last number

      Console.WriteLine();
    }

    // Lots of Casts //
    //
    // Consider these functions:
    //
    // a)
    static int a(int i)
    {
      return (int)(float)i;
    }

    // b)
    static int b(int i)
    {
      return (int)(double)i;
    }

    // c)
    static long c(long l)
    {
      return (long)(double)l;
    }

    // Which of these functions, if any, do you believe is equal to the identity function,
    // i.e. it always returns the same value it was given? If you're not sure, run experiments
    // to find out the answer.

    // Let's take a look at how floats and doubles are represented in computers:
    //
    // https://en.wikipedia.org/wiki/Single-precision_floating-point_format
    // https://en.wikipedia.org/wiki/Double-precision_floating-point_format
    //
    // We can see that for floats (single precision floating point numbers), we have 1 bit for
    // the sign, 8 bits for the exponent (from -127 to 128, -127 and 128 have special meaning)
    // and 23 bits for the *mantissa*.
    //
    // The smallest exponent (-127) is used for zero and subnormal numbers (which we'll skip for now).
    // There are two zeros in this representation (positive and negative). The largest exponen
    // (128) is used for infinity and NaN (not a number). The largest number has the bits
    // 0x7F7FFFFF (0 11111110 11111111111111111111111) and represents ~3.4028e38.
    //
    // It seems like we should be able to fit an int in there comfortably. The problem is that
    // we only have 23 bits for the mantissa, so we cannot exactly represent numbers with more than
    // 24 bits of precision (don't forget the "hidden" bit) - and int clearly needs up to 32 bits.

    // Doubles (double precision floating point number) has 1 bit for the sign, 11 bits for the
    // exponent and 52 bits for the mantissa. With a total of 53 bits of precision, storing an
    // int shouldn't be a problem. But as before, long is too large to be stored in double exactly.

    static void Test()
    {
      int num = 1_000_000_001;
      int a_num = a(num);
      Console.WriteLine($"{num} {a_num} {num == a_num}");

      long num_long = 30_000_000_000_000_001;
      long c_num = c(num_long);
      Console.WriteLine($"{num_long} {c_num} {num_long == c_num}");
    }

    // Double Experiment //

    // Consider these questions:
    //
    // a) A double has an initial value of 1.0. How many times can we divide it by 2 before
    // it becomes 0?
    //
    // b) A double has an initial value of 1.0. How many times can we multiply it by 2 before
    // it becomes Double.PositiveInfinity?
    //
    // Guess at the answers to (a) and (b). Then write a problem that performs these experiments
    // and prints out the resulting values.

    // Let's start with b) first. We know that for doubles, the exponent can be anything between
    // -1023 and 1024. Just like with floats, -1023 and 1024 have special meaning. Of interest to
    // us is the exponent 1024, which represents either an infinity or NaN. 1.0 has pretty simple
    // representation: 0 sign, 0 exponent (the bits corresponding to that exponent are 01111111111)
    // and 0 mantissa (don't forget the hidden bit) - 0x3FF0000000000000. It will thus take us
    // exactly 1024 doublings to reach infinity (remember that one doubling will simply increase the
    // exponent by one).

    // We would expect the same argument to also apply to halving. We need to go from exponent 0 to
    // exponent -1023, which should take 1023 divisions. But if we perform the experiment, we get
    // 1075 instead. The reason are subnormal numbers - numbers without the hidden bit. If the exponent
    // is the lowest possible (-1023) and the mantissa is nonzero, it is assumed that the hidden
    // bit isn't present! We thus need additional 52 (= length of the mantissa) divisions to get to
    // zero.
    static void DoubleExperiment()
    {
      int i = 0;
      double d = 1.0;
      while (d != 0.0)
      {
        d *= 0.5;
        i++;
      }

      Console.WriteLine($"Reached 0 after {i} divisions");

      i = 0;
      d = 1.0;
      while (d != double.PositiveInfinity)
      {
        d *= 2.0;
        i++;
      }

      Console.WriteLine($"Reached inf after {i} multiplications");
    }

    // Integer Square Root //
    // Write a program that reads an integer n, and computes and prints the integer square root of n,
    // i.e. the non-negative integer i such that i^2 = n. If no such integer exists, print "not a square".
    // Do not call any library functions.Your method must run in time O(log N) in the worst case.
    static long ISqrt(long n)
    {
      if (n < 0)
      {
        // Or we could throw an exception
        return -1;
      }

      long lo = 0, hi = n;
      while (lo <= hi)
      {
        // long mid = (lo + hi) / 2;
        // Is this correct? What if lo and hi are really large?
        // Let's test it:
        //
        // long a = 0x7FFFFFFFFFFFFFFF;
        // long b = a - 20;
        // Console.WriteLine((a + b) / 2);
        //
        // Better approach:
        //
        // Console.WriteLine(a + (b - a) / 2);
        long mid = lo + (hi - lo) / 2;
        if (mid * mid == n)
        {
          return mid;
        }
        else if (mid * mid < n)
        {
          lo = mid + 1;
        }
        else
        {
          hi = mid - 1;
        }
      }

      return hi;
    }

    static void Main(string[] args)
    {
      Console.WriteLine($"abc begins with abcd: {Begins("abc", "abcd")}");
      Console.WriteLine($"abcd begins with abc: {Begins("abcd", "abc")}");
      Console.WriteLine(AddCommas("123456789123456789"));
      Console.WriteLine(AddCommas("12345678912345678"));
      Console.WriteLine(AddCommas("1234567891234567"));
      Console.WriteLine("Enter a sequence");
      Solve();
      Console.WriteLine("Identity fn test");
      Test();
      Console.WriteLine("Double experiment");
      DoubleExperiment();
      Console.WriteLine("ISqrt");
      for (int i = 0; i < 50;  i++)
      {
        Console.WriteLine($"{i} {ISqrt(i)}");
      }
    }
  }
}
