# Quick notes about code quality

An important part of coding isn't just writing code that works (i.e. does what we want it to), but also making sure that your code can be understood (and modified) in the future.

## Why?

Most code needs to be changed at some point: a bug is found, requirements change, libraries are updated, etc. When that happens you want the process of modification to be as painless as possible.

Obviously, this is less of an issue for solutions to ReCodEx exercises, or even the current semestral projects, given the smaller scope of these. However, it's a good idea to get into the proper habits sooner rather than later.

## What?

* **Readability**
  * Your code should be easy to read and understand.
  * Use descriptive names for variables, functions and classes.
* **Simplicity**
  * Prefer simpler code to more complex one.
  * Don't overengineer your solutions.
* **Maintainability**
  * Write code that is easy to extend or update.
  * Break your code into smaller units (functions, classes, modules).
  * Don't repeat yourself!
* **Consistency**
  * Stick to a single coding style (naming conventions, indentation, max line length, etc).
  * In the future, you'll probably be asked to stick to some general code style for a given language (such as PEP 8 for Python), but here it's sufficient to just keep it consistent.
* **Correctness**
  * Your code should work correctly even in edge cases.
  * Think about what happens when your program is given some unexpected input.
* **Documentation**
  * If you need to write complicated code, make sure to add comments that explain it.
  * Your comments should explain *why* something is done (or sometimes also *how*), it shouldn't try to explain *what* the code is doing -- that should be obvious from the code itself.
  * If there are some hidden assumptions or invariants, explain them!

The above is perfectly sufficient for ReCodEx assignments, but for your semestral project, you might also want to think about the following:

* **Testing**
  * Write automated tests (unit tests, integration tests, etc).
  * After you change your code, tests can be used to check that you didn't introduce new bugs!

* **Version control**
  * You should be using a version control system such as Git.
  * Commit often and write meaningful commit messages.

## Examples

Here are some examples of what to avoid and how to fix it.

### Global variables

```cs
static public Dictionary<string, string> configuration = new();

...

static void ResetLanguage()
{
    configuration["language"] = "english";
}
```

Global variables (here as a publicly acessible static variable) make your code hard to debug and reason about -- a global variable could be changed from *anywhere* in your program. You should try to replace them with instance variables or function parameters. Global *constants* are fine (see below).

### Magic numbers

```cs
if (connections < 30)
{
    StartConnection();
}
```

What is the number 30 doing here? You could explain it in a comment but an even easier solution is to just actually name it:

```cs
const int MAX_CONNECTIONS = 30;

...

if (connections < MAX_CONNECTIONS)
{
    StartConnection();
}
```

In general, you should avoid putting hardcoded values in your code.

### Code duplication

```cs
if (userType == User.ADMIN)
{
    Console.WriteLine("Hello admin!");
}
else if (userType == User.GUEST)
{
    Console.WriteLine("Hello guest!");
}
```

If you notice doing the same thing again and again, you should probably refactor your code.

```cs
Console.WriteLine($"Hello {UserTypeString(userType)}!");
```

### Trying to do too much

If your class or function ends up trying to do too much at once, it's probably a good idea to split it into multiples. Don't make your classes and functions responsible for multiple things at once.

For example, if you're implementing the Dijkstra's algorithm, your function *shouldn't* also be priting the shortest path to the console! Leave that to someone else.

### Don't comment out old code

That's what version control is for! If you need to go back to an older version of your code, you can just use `git checkout`.

### Useless comments

```cs
// Increase i by 2 in each iteration.
for (int i = 0; i < results.Count; i += 2)
{
    ...
}
```

This comment doesn't really convey any new information. Any programer can figure out what the following line is doing.

```cs
// Only check the values at even positions.
for (int i = 0; i < results.Count; i += 2)
{
    ...
}
```

This is a bit better, but still tries to tell us what the code is doing rather than why we're doing it!

```cs
// The previous step guarantees that the result is at an even position;
// we don't need to check anything else.
for (int i = 0; i < results.Count; i += 2)
{
    ...
}
```

### Inconsistent code style

```cs
for (int current_value=1; current_value <= 20;current_value++)
{
int currentValueSquared=current_value * current_value;
    Console.WriteLine( currentValueSquared );
}
```

Don't use different naming conventions without reason (e.g. both camelCase and underscore_names in the above example). Keep the indentation consistent. Keep the spacing consistent -- generally a good idea to surround operators with whitespace, add a space after `;` in a for loop, etc.
