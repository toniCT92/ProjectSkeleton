# AI Usage Disclosure

## Tools Used

- **Claude Sonnet 4.7** (Anthropic) — used for overall design discussions, architecture planning, and approach decisions
- **ChatGPT** (OpenAI) — used for code improvement suggestions and small refactoring ideas

## How I Used Them

I used Claude Sonnet 4.7 as a planning partner — discussing the overall architecture (class hierarchy, separation between rendering, input, game logic and persistence), the choice of features to demonstrate (LINQ, pattern matching, custom exceptions, IDisposable, generics, inheritance), and the file structure of the project. I described what each component should do, reviewed the suggestions, and adapted them to my own style before committing.

ChatGPT was used for smaller code improvement suggestions during development — refining specific functions, asking about idiomatic C# patterns, and getting feedback on edge cases (such as the 180 degree turn prevention in the snake direction logic).

All code was reviewed and understood before being committed. I asked clarifying questions about every concept (pattern matching syntax, LINQ chain composition, abstract classes, IDisposable lifecycle, custom exception design) before moving on.

## Fully AI-Generated Regions

No fully AI-generated C# source code regions. All `.cs` files in the repository were written and reviewed by me, so no `// AI-generated` / `// end AI-generated` markers are required in the source files.

The `README.md` documentation file was drafted with Claude Sonnet 4.7's help, then reviewed and adjusted by me. The `AI_USAGE.md` file you are reading was also drafted with Claude's assistance.

## Notes

The AI assistants explained C# and .NET concepts that informed the design decisions — the move-accumulator pattern for frame-rate-independent timing, tuple patterns in switch expressions, JSON serialization with `System.Text.Json`, the use of `using var` for automatic IDisposable disposal, and the difference between abstract classes and interfaces. The implementation decisions and structure were guided by my own judgement informed by these explanations.
