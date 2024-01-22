# Features

| Status | Feature | Description |
|:------:|:--------|:------------|
||||
|**>>>**|**Types:**||
||||
| ☑ | `bool` | `true`, `maybe`, `false` |
| ☑ | `int` | `69`, `∞`, `NaN` |
| ☑ | `float` | `12.57`, `12.57f`, `69f`, `∞f`, `NaNf` |
| ☑ | `char` | `'S'`, `‹X›` |
| ☑ | `range` | `(0:10)`, `(0:10:2)`, `(0.5:20.5:0.5)` |
| ☑ | `string` | `"Foo"`, `«Bar»` |
| ☑ | `list<t>` or `t[]` | `[12, 57, 69, 420]`, `["SEx", "is", "cool"]`, `[[1, 2], [3, 4]]` |
| ☐ | `dict<k,v>` or `{k:v}` | `{ x1: y1, x2: y2... }` |
| ☐ | `record` → `{ f1: t1... }` | `{ name = "John Doe", age = 34 }` |
| ☑ | `action` or `function<void, t...>` | `(foo:string, bar:int)->void => {...}` |
| ☑ | `func<out, t...>` | `(x:int, y:int)->int => x + y`, `(x:string)->bool => {...}` |
||||
|**>>>**|**Unary Operations:**||
||||
| ☑ | Identity | `+number => number` |
| ☑ | Negation | `-number => number` |
| ☑ | Bitwise Complement | `~int => int` |
| ☑ | Bitwise Complement | `~bool => bool` |
| ☑ | Boolean Complement | `!bool => bool` |
||||
|**>>>**|**Binary Operations:**||
||||
| ☑ | Addition | `number + number => number` |
| ☑ | Char Addition | `char + number => char` |
| ☑ | Subtraction | `number - number => number` |
| ☑ | Multiplication | `number * number => number` |
| ☑ | Division | `number / number => float` |
| ☑ | Modulo | `number % number => float` |
| ☑ | String Concatenation | `string + any => string` |
| ☑ | String Multiplication | `string * int => string` |
| ☑ | List Concatenation | `t[] + t[] => t[]` |
| ☑ | List Multiplication | `t[] * int => t[]` |
||||
|**>>>**|**Cancelled:**||
||||
| ☒ | ~~==  or~~ | ~~Being able to do: `x == "a" or "b"`~~ |
| ☒ | ~~and ==~~ | ~~Being able to do: `x and y == "b"`~~ |

- `☐` > Planned
- `☑` > Done
- `☒` > Cancelled

<!-- &#9744; &#9745; &#9746; &check; &cross; &starf; -->
