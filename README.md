# Software Development Skills

A small .NET console project for deliberate software engineering practice with an AI tutor. Generate an exercise, solve it yourself, push your attempt, and request feedback against the learning outcome.

## Getting started

Install the .NET 10 SDK, clone this repository, then run:

```sh
dotnet build development.csproj
dotnet run --project development.csproj
```

The console currently contains an unregistered exercise template. Register runnable exercises in Program.cs when generating a lesson. Each registered module runs once.

## Working with an agent

Give your agent access to this repository and ask it to read [AGENTS.md](AGENTS.md). Repository access and file edits depend on the agent's tools; these instructions do not install an agent or run anything automatically.

Example requests:

- “Give me an intermediate SRP refactor with a small, realistic scenario.”
- “Give me a subtle bug to investigate. Describe the symptoms without revealing the cause.”
- “Create a rate limiting exercise with a local fake API.”
- “Give me a design-from-scratch exercise about retries and partial failure.”
- “Hint only.”
- “Review my pushed attempt on exercise/001-order-processing. Do not change my code.”
- “Re-review my latest revision.”
- “Finish this exercise and record what I learned.”

If your chat tool does not automatically load AGENTS.md, explicitly ask it to read that file at the start of a new conversation.

## Exercise cycle

1. Create an exercise branch from main.
2. Agent adds the brief and starter code, validates it, and commits the starter.
3. Record the starter commit in exercises/current/brief.md in a subsequent metadata commit.
4. Pull the branch, work without coding assistance unless requested, commit and push.
5. Ask for review; revise as needed.
6. Record the outcome in learning/progress.md.
7. Preserve the learning record on main before deleting the exercise branch.

See [the workflow](docs/exercise-workflow.md) for branch and completion details and [the review guide](docs/review-guide.md) for assessment criteria.

## Layout

- Interfaces/ — runnable learning contracts.
- Modules/ — exercises grouped by subject; the initial template is Modules/SOLID/SRP/Exercise.cs.
- exercises/current/ — the active branch's brief and review.
- exercises/templates/ — reusable brief and review templates.
- docs/ — workflow, review criteria, and optional learning resources.
- learning/ — editable learner profile and durable progress.

Keep the existing layout small. Add folders, test projects, async contracts, or dependencies when an exercise needs them.

## Learning approach

Practice refactoring, debugging, design, testing, and reliability. Difficulty comes from guidance, scope, discovery, and constraints. Reviews assess behaviour and reasoning; adding classes or naming a design pattern is not proof of improvement.

For independent practice, keep inline AI completion disabled if that supports your learning. Hints and full solutions are always opt-in.

## Sharing

Fork this repository and edit learning/profile.md for your goals. The supplied profile reflects the initial learning focus; it is not a skill assessment. Only commit information you are comfortable making public. No reuse license has been selected yet; the repository owner should choose one before advertising it as a reusable template.
