# Review guide

Judge the exercise against its stated outcomes and constraints, not an imagined production system.

| Dimension | Evidence to look for |
| --- | --- |
| Behaviour | Required outcomes preserved; intended defects fixed; relevant edge cases considered |
| Responsibility | Independent reasons to change separated; orchestration remains understandable |
| Abstraction | Functions describe coherent operations at a consistent level; useful names and boundaries |
| Design tradeoffs | Complexity fits the problem; dependencies and patterns have concrete purposes |
| Verification | Meaningful examples or tests cover the important behaviour and regression |
| Explanation | Learner can explain decisions, assumptions, and remaining limitations |

For SRP, identify who or what drives a change. Ask whether notification, validation, storage, or orchestration rules can change independently. Splitting methods alone does not establish independent responsibilities, and small cohesive classes do not require further decomposition.

For debugging, assess reproduction, root-cause reasoning, repair, and regression protection. For reliability, assess explicit failure assumptions and behaviour under retries, time, cancellation, concurrency, or partial success only where in scope.

Use qualitative outcomes:
- Achieved: evidence supports the primary objective.
- Partly achieved: useful progress with a specific remaining gap.
- Not yet achieved: primary objective remains unmet.
- Not assessed: evidence or execution unavailable.

Separate correctness defects from design gaps and optional preferences. Prioritize the most useful one or two findings with file locations, impact, and a question or direction. Avoid a full replacement implementation unless requested.

Passing tests is evidence about tested behaviour, not proof of sound design. Likewise, a sound design does not excuse broken required behaviour. Acknowledge assistance and distinguish observed implementation from reasoning the learner has not yet explained.
