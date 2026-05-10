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
  * [ ] Variable assignments
  * [ ] Function calls
  * [ ] Module names & importing
  * [ ] Type definitions
    * [X] Structs
    * [X] Aliases
    * [ ] Enums
  * [ ] Functions on types (traits)
  * [ ] Expressions
    * [ ] Arithmetic/logic operators (`-` `+` `/` `*` `%` `==` `!=` `<` `>` `<=` `>=`)
    * [ ] Unary operators (`-` `+` `!`)
    * [X] Parentheses (`(expr)`)
    * [X] Primitives (numbers, bool, str)
* [ ] Semantic analysis
  * TODO
* [ ] Execution
  * TODO