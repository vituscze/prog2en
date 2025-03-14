using System;

namespace ConsoleApp
{
  internal class Program
  {
    // Programming 2 Tutorial //

    // How to pass the tutorial
    //
    // * Attendance strongly recommended, but not mandatory
    // * Homework
    //   - Assigned every week, starting next week
    //   - 70% (i.e. 70 points) required to pass the tutorial
    //   - Submitted via ReCodEx (https://recodex.mff.cuni.cz/)
    //     * Check README for useful links (new user guide, student guide)
    //   - Do not copy code from other students or the internet
    // * Semestral project
    //   - Solve a nontrivial problem in C#
    //   - Can be implementation of an algorithm, a computer game, etc
    //   - Before you start working, submit a proposal via email and wait for confirmation
    //     * Description of the problem and specification of the solution
    //     * Doesn't have to be long, 1-2 paragraphs is usually enough
    //   - To submit a project, you need to provide:
    //     * Source code
    //     * Test data (sample inputs and outputs, if applicable)
    //     * User documentation (explain how to use the program to your users)
    //     * Developer documentation (explain how your programs works to other developers)
    //   - Deadlines are on the course page (https://ksvi.mff.cuni.cz/~dingle/2024-5/prog_2/programming_2.html)

    // Study materials
    //
    // * GitHub repository
    // * Discord server (optional)
    // * Recordings
    //   - Posted every week to the Discord server
    //   - Also available on request

    // Main differences from Python:
    //
    // * (statically) typed
    // * indentation doesn't matter, curly braces instead
    // * more focus on objects and classes
    // * compiled into intermediate code, JIT; Python is interpreted
    //
    // Empty project (w/o top level statements) isn't an empty file
    // (unlike in Python). We have the Program class with a static method
    // named Main. This is where the program execution begins (i.e. when
    // you launch the program, it will start by calling the Main method).
    //
    // We won't be using the top-level statements feature of more recent C#
    // version here (mainly because it doesn't work in ReCodEx, but there
    // are projects where having a class with the Main method makes more sense).
    //
    // Quick type overview:
    // * int (32 bit signed integer)
    // * uint (32 bit unsigned integer)
    // * long (64 bit signed integer)
    // * ulong (64 bit unsigned integer)
    // * float (32 bit floating point number)
    // * double (64 bit floating point number)
    // * char (16 bit code unit of UTF16 - it's complicated)
    // * string (sequence of chars)
    // * bool (true/false)
    // * void (special return type of methods that don't return any values)
    //
    // Arithmetic operators are very similar to Python. One of the main differences
    // is that there is only a single division operator (/). It either does integer
    // division (if the operands are integers) or floating point division (if at least
    // one operand is a floating point number).
    //
    // Instead of not, and, or, C# has the operators !, &&, ||.

    // Multiples //
    //
    // Solve Project Euler's Problem 1 in C#:
    //   Find the sum of all the multiples of 3 or 5 below 1000.
    static int Euler1()
    {
      int sum = 0;

      for (int i = 0; i < 1000; i++)
      {
        if (i % 3 == 0 || i % 5 == 0)
        {
          sum += i;
        }
      }

      return sum;
    }

    // Fibonacci Sum //
    //
    // Solve Project Euler's Problem 2 in C#:
    //   By considering the terms in the Fibonacci sequence whose values do not exceed
    //   four million, find the sum of the even-valued terms.
    static int Euler2()
    {
      int a = 0;
      int b = 1;
      int sum = 0;

      while (a < 4_000_000)
      {
        if (a % 2 == 0)
        {
          sum += a;
        }

        (a, b) = (b, a + b);
      }

      return sum;
    }

    // Prime Factor //
    //
    // Solve Project Euler's Problem 3 in C#:
    //   What is the largest prime factor of the number 600851475143?
    static long Euler3()
    {
      long n = 600851475143;
      long factor = 1;

      for (long i = 2; i * i <= n; i++)
      {
        while (n % i == 0)
        {
          factor = i;
          n /= i;
        }
      }

      if (n != 1)
      {
        factor = n;
      }

      return factor;
    }

    // Largest Palindrome Product //
    //
    // Solve Project Euler's Problem 4 in C#:
    //   Find the largest palindrome made from the product of two 3-digit numbers.
    static int Euler4()
    {
      int largest = 0;

      for (int a = 100; a < 1000; a++)
      {
        for (int b = a; b < 1000; b++)
        {
          int product = a * b;
          string productStr = product.ToString();
          // Alternatively, we can use the C# version of f-strings:
          // string productStr = $"{product}";

          bool isPalindrome = true;
          for (int i = 0; i < productStr.Length / 2; i++)
          {
            if (productStr[i] != productStr[^(i + 1)])
            {
              isPalindrome = false;
              break;
            }
          }

          if (isPalindrome && product > largest)
          {
            largest = product;
          }
        }
      }

      return largest;
    }

    // Same as above but without using strings.
    static int Euler4Alt()
    {
      int largest = 0;

      for (int a = 100; a < 1000; a++)
      {
        for (int b = a; b < 1000; b++)
        {
          int product = a * b;
          int productRev = 0;

          while (product != 0)
          {
            productRev *= 10;
            productRev += product % 10;
            product /= 10;
          }

          // Restore the original product.
          product = a * b;
          if (product == productRev && product > largest)
          {
            largest = product;
          }
        }
      }

      return largest;
    }

    // Last Digits //
    //
    // Write a program that computes and prints the last 5 digits of the integer 2^1000.
    static long LastDigits()
    {
      long n = 2;
      long exp = 1000;
      long result = 1;

      while (exp > 0)
      {
        if (exp % 2 == 1)
        {
          result = (result * n) % 100000;
        }

        n = (n * n) % 100000;
        exp /= 2;
      }

      return result;
    }

    static void Main(string[] args)
    {
      Console.WriteLine(Euler1());
      Console.WriteLine(Euler2());
      Console.WriteLine(Euler3());
      Console.WriteLine(Euler4());
      Console.WriteLine(Euler4Alt());
      Console.WriteLine(LastDigits());
    }
  }
}
