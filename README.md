# Prosaic

This is a small language, which is intentionally dull and
simple. It features a static type system, which is similar
to TypeScript's, but is much simpler.

### Example
```
main(args: Str[]) {
    var x = 2;
    var y = "Hello";
    ret 0;
}
```

### Implementation
* [X] Lexer
* [ ] Parser
  * [X] Function definitions
  * [X] Variable definitions
  * [X] Variable assignments
  * [X] Function calls
  * [ ] Module names & importing
  * [ ] Type definitions
    * [X] Structs
    * [X] Aliases
    * [ ] Enums
    * [ ] Inline anonymous structs
  * [ ] Functions on types (traits)
  * [X] Expressions
    * [X] Arithmetic/logic operators (`-` `+` `/` `*` `%` `==` `!=` `<` `>` `<=` `>=`)
    * [X] Unary operators (`-` `+` `!`)
    * [X] Parentheses (`(expr)`)
    * [X] Primitives (numbers, bool, str)
* [ ] Semantic analysis
  * TODO
* [ ] Execution
  * TODO