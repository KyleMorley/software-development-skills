# Software Development Skills

Small C# exercises with an AI coach: pull, solve, push, review.

## Run

Install the .NET 10 SDK:

```sh
dotnet build development.csproj
dotnet run --project development.csproj
```

Register runnable exercises in Program.cs. Each module runs once.

## Use the coach

Ask the agent to read [AGENTS.md](AGENTS.md), then say:

- “Give me an SRP refactor on a new branch.”
- “Give me a subtle bug to investigate.”
- “Hint only.”
- “Review my SRP changes on exercise/order-processing.”
- “Add this learning resource: <link>.”

Requirements and reviews stay in chat. Tell the coach which exercise to review; it uses code and Git history. No current-exercise documents or automatic logs.

## Reference

- [Learning preferences](learning/profile.md)
- [Review guide](docs/review-guide.md)
- [Learning resources](docs/resources.md)

Markdown is limited to 80 lines and 600 words per file, enforced in GitHub Actions. New Markdown files require an explicit request and an intentional allowlist change.
