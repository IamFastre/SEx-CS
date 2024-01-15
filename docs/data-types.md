# Data Types

In SEx, there are a handful of built-in data types/structures
here is a list of them:

- [`void`](#voids)
- [`null`](#nulls)
- [`bool`](#booleans)
- [`int`](#integers)
- [`float`](#floats)
- [`range`](#ranges)
- [`char`](#chars)
- [`string`](#strings)
- [`list`](#lists)
- [`function`](#functions) & [`action`](#actions)

<!----------------------------------------------------------------------->
## Voids

May only be used as a return type for `function`s and always for `action`s.

**- Syntax:**

```dart
• -> void // only as return type
```

**- Example:**

```dart
>>> Print2DList(lst:any[][]) -> void: {
    for line in lst: {
        for elem in line: {
            if elem == null: {
                errors.Add("null element found")
                // a new line after is necessary so it doesn't try to parse a return expression
                return
            } else 
                Print($"{elem} ", false)
        }
        Print()
    }
}
```

<!----------------------------------------------------------------------->
## Nulls

It carrier nothing basically, used to nullify a value so the app knows it's still not for use,
it's also not meant to be dereferenced in normal circumstances.
It's like locking the variable usage so it throws errors when used in places not meant to be used.

_Note:_ `null` can't be used as a declaration or return type.

**- Syntax:**

```dart
• null
```

**- Example:**

```dart
# input :string = null
# isGay :bool   = null
# isMale:bool   = null

Print(input) // SymbolError: Dereference of a null reference
Print(isGay) // SymbolError: Dereference of a null reference


Print("What are you?")
Print("(m/f) ", false)
input = Read().toLower()

if input == "m":
    isMale = true
else if input == "f":
    isMale = false
else: {
    Print("Uhh, I'm just gonna assume")
    isMale = maybe
}

input = null // you can re-nullify any type whenever

Print("Are you gay?")
Print("(y/n) ", false)
input = Read().toLower()

if input == "y":
    isGay = true
else if input == "n":
    isGay = false
else: {
    Print($"Okay {isMale ? "mr." : "ms."} secrets, I'm just gonna assume")
    isGay = maybe
}

Print(isGay ? "Oh ma gaaad!! same bestiiieee!!!" : "Oh, me too!")
```

**- Conversions:**

- `null -> string` (only literal)

<!----------------------------------------------------------------------->
## Booleans

It represents a boolean value that is either `true`, `false` or `maybe`. To further explain `maybe`,
it's just the interpreter choosing `true` or `false` based on pseudo-randomness.

**- Syntax:**

```dart
• true
• false
• maybe
```

**- Example:**

```dart
# doCoinFlip:bool = true
# haveACoin :bool = true

>>> CoinFlip(count:int) -> void: {
    if (count <= 0):
        Print("Have you thought it through?")
    else:
        for i in (1:count):
            Print(maybe ? "Heads!" : "Tails")
}

if doCoinFlip && haveACoin:
    CoinFlip(12)
else if !haveACoin:
    Print("get a coin, pal")
```

**- Conversions:**

- `bool -> int`
- `bool -> float`
- `bool -> number`
- `bool -> string`

**- Operations:**

- **Unary:**
- `~`: bitwise NOT
- `!`: boolean NOT
- **Binary:**
- `|`: bitwise OR
- `&`: bitwise AND
- `||`: boolean OR
- `&&`: boolean AND

<!----------------------------------------------------------------------->
## Integers

They simply carry integral number types.

**- Syntax:**

```dart
• 69
• +12
• -57
• ∞
• NaN
```

**- Example:**

```dart
# bitchesCount:int = 0
```

**- Conversions:**

- `int -> float`
- `int -> number`
- `int -> char`
- `int -> string`

**- Operations:**

- **Unary:**
- `~`: bitwise NOT
- `|`: bitwise OR
- `&`: bitwise AND
- `+`: identity
- `-`: negation
- **Binary:**
- `+`: addition
- `-`: subtraction
- `*`: multiplication
- `/`: division
- `%`: modulo
- `**`: power
- `>`: greater
- `<`: lesser
- `>=`: greater or equal
- `<=`: lesser or equal

<!----------------------------------------------------------------------->
## Floats

They simply carry floating point number types.

**- Syntax:**

```dart
• +12f
• -12.57
• ∞f
• NaNf
```

**- Example:**

```dart
# ppLengthCM:float = 2.5f
```

**- Conversions:**

- `float -> int` (floored)
- `float -> number`
- `float -> char` (floored)
- `float -> string`

**- Operations:**

- **Unary:**
- `+`: identity
- `-`: negation
- **Binary:**
- `+`: addition
- `-`: subtraction
- `*`: multiplication
- `/`: division
- `%`: modulo
- `**`: power
- `>`: greater
- `<`: lesser
- `>=`: greater or equal
- `<=`: lesser or equal

<!----------------------------------------------------------------------->
## Ranges

They are iterable values that have a start and end value in addition to a step value.
They are parenthesized unless used to index like: `v[i]`.

_Note:_ using operator `in` with ranges disregards the step value.

**- Syntax:**

```dart
• (0:5)
• (0:100:2)
• (-50:50:0.5)
• (50:0:-1)
```

**- Example:**

```dart
>>> StupidWayToKnowIfANumberIsSmallerThanOrEqualToPI(x:number) -> bool:
    x in (-∞:3.14)

StupidWayToKnowIfANumberIsSmallerThanOrEqualToPI(2) // output: true
StupidWayToKnowIfANumberIsSmallerThanOrEqualToPI(5) // output: false
```

**- Conversions:**

- `range -> int[]` (floored)
- `range -> float[]`
- `range -> number[]`
- `range -> string`

**- Operations:**

- `in`: inclusion

<!----------------------------------------------------------------------->
## Chars

They represent a single character, it's treated kind of like a string but kind of like an integer too.

**- Syntax:**

```dart
• 'S'
• '\u0045'
• ‹x›
```

**- Example:**

```dart
>>> GetAlphabets() -> string: {
    # alphabets:string = ""
    # c:char = 'a'

    while c <= 'z':
        alphabets += c++

    return alphabets
}

GetAlphabets() // output: "abcdefghijklmnopqrstuvwxyz"
```

**- Conversions:**

- `char -> int`
- `char -> float` (floored)
- `char -> number` (floored)
- `char -> string`

**- Operations:**

- **Binary:**
- `+`: addition
- `-`: subtraction
- `>`: greater
- `<`: lesser
- `>=`: greater or equal
- `<=`: lesser or equal

<!----------------------------------------------------------------------->
## Strings

They are iterable values that represent a string of characters.

**- Syntax:**

```dart
• "SEx"
• "\u0069\x73"
• «cool!»
• $"69 minus 57 is {69-57}"
• $«Logged in as {username}»
```

**- Example:**

```dart
# first:string = "John"
# last :string = "Doe"

# getUsername:function<string> = () -> string 
    => $"{first.toLower()}_{last.toLower()}"
```

**- Conversions:**

- `string -> char[]`
- `string -> string[]`

**- Operations:**

- **Binary:**
- `+`: string concatenation
- `*`: string multiplication
- `in`: inclusion

<!----------------------------------------------------------------------->
## Lists

They are iterable data structures that contain a homogeneous list of values.

**- Syntax:**

```dart
• ["SEx", "is", "cool!"]
• [12, 0.57f, 69]
• [[1, 2]
   [3, 4]]
```

**- Example:**

```dart
>>> MakeGrid(dim:int) -> int[][]: {
    # grid:int[][] = []
    for i in (1:dim): {

        # line:int[] = []
        for j in (1:dim):
            line.Add((i*j)->int)

        grid.Add(line)
    }

    return grid
}

MakeGrid(5)
// output:
// [[1, 2, 3, 4, 5],
//  [2, 3, 4, 5, 6],
//  [3, 4, 5, 6, 7],
//  [4, 5, 6, 7, 8],
//  [5, 6, 7, 8, 9],
//  [6, 7, 8, 9, 10]]
```

**- Conversions:**

- `list -> string`

**- Operations:**

- **Binary:**
- `+`: list concatenation
- `*`: list multiplication
- `in`: inclusion

<!----------------------------------------------------------------------->
## Functions

They're:
> "a sequence of program instructions that performs a specific task, packaged as a unit."
>
> \- Wikipedia

But additionally they're stored as values that can be assigned to variables and passed to other functions here.

**- Syntax:**

```dart
• Named Functions:
• >>> func() -> t: { ... }
• >>> func() -> t: expression
• Anonymous Functions:
• # func = ()->t => { ... }
• # func = ()->t => expression
• Constant Functions:
• >>>* func() -> t: { ... }
• #* func = ()->t => { ... }
```

**- Example:**

```dart
>>> Fibonacci(n:int) -> int: {
    if n == 0 || n == 1:
        return n
    return Fibonacci(n-1) + Fibonacci(n-2)
}

Fibonacci(12) // output: 144
```

**- Conversions:**

- `function -> string`

## Actions

They're basically [function](#functions)s that always return void; procedures in other words.

**- Syntax:**

```dart
• Named Actions:
• >>> act(): { ... }
• >>> act(): expression
• Anonymous Actions:
• # act = ()->void => { ... }
• # act = ()->void => expression
• Constant Actions:
• >>>* act(): { ... }
• #* act = ()->void => { ... }
```

**- Example:**

```dart

>>> HelloTo(name:string): {
    Print($"Hello there {name}")
}

# input:string = Read()
HelloTo(input)
```

**- Conversions:**

- `action -> string`

<!-- 
## Type

**- Syntax:**

```dart
• 
```

**- Example:**

```dart
```

**- Conversions:**

- `type -> string`

**- Operations:**
-->
