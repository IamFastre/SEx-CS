# Features

| Status | Feature | Description |
|:------:|:--------|:------------|
||||
|**>>>**|**Types:**||
||||
|&#9745;| `bool` | `true`, `maybe`, `false` |
|&#9745;| `int` | `69`, `∞`, `NaN` |
|&#9745;| `float` | `12.57`, `12.57f`, `69f`, `∞f`, `NaNf` |
|&#9745;| `char` | `'S'`, `‹X›` |
|&#9745;| `range` | `(0:10)`, `(0:10:2)`, `(0.5:20.5:0.5)` |
|&#9745;| `string` | `"Foo"`, `«Bar»` |
|&#9745;| `list<t>` \| `t[]` | `[12, 57, 69, 420]`, `["SEx", "is", "cool"]`, `[[1, 2], [3, 4]]` |
|&#9744;| `record` → `{ field1:t1... }` | `{ name = "John Doe", age = 34 }` |
|&#9745;| `action` \| `function<void, t...>` | `(foo:string, bar:int)->void => {...}` |
|&#9745;| `function<out, t...>` | `(x:int, y:int)->int => x + y`, `(x:string)->bool => {...}` |
||||
|**>>>**|**Unary Operations:**||
||||
|&#9745;| Identity | `+number => number` |
|&#9745;| Negation | `-number => number` |
|&#9745;| Bitwise Complement | `~int => int` |
|&#9745;| Bitwise Complement | `~bool => bool` |
|&#9745;| Boolean Complement | `!bool => bool` |
||||
|**>>>**|**Binary Operations:**||
||||
|&#9745;| Addition | `number + number => number` |
|&#9745;| Char Addition | `char + number => char` |
|&#9745;| Subtraction | `number - number => number` |
|&#9745;| Multiplication | `number * number => number` |
|&#9745;| Division | `number / number => float` |
|&#9745;| Modulo | `number % number => float` |
|&#9745;| String Concatenation | `string + any => string` |
|&#9745;| String Multiplication | `string * int => string` |
|&#9745;| List Concatenation | `t[] + t[] => t[]` |
|&#9745;| List Multiplication | `t[] * int => t[]` |
||||
|**>>>**|**Cancelled:**||
||||
|&#9746;| ~~==  or~~ | ~~Being able to do: `x == "a" or "b"`~~ |
|&#9746;| ~~and ==~~ | ~~Being able to do: `x and y == "b"`~~ |

- &#9744; > Planned
- &#9745; > Done
- &#9746; > Cancelled

<!-- &#9744; &#9745; &#9746; &check; &cross; &starf; -->
