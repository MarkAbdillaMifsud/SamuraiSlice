# Samurai Slice! - Prototype Branch

**Status:** archived. Do not merge to `main`.

This branch holds the disposable Week 2 prototype spike — a throwaway Unity sketch built to answer three questions before any production code went near the project: does swipe-versus-Linecast slice detection feel right, what is the spawn-cadence sweet spot, and at what bomb probability does tension start to outweigh frustration?

## What was prototyped

- **Swipe detection.** `QuickSwipe.cs` captures mouse positions on drag and runs a `Physics2D.Linecast` between the last two points each frame. Anything tagged `Ingredient` that intersects gets destroyed and logs `+10`. Verdict: the feel holds — no mesh-splitting required, the paired pre-split sprite-half flyout sells the slice convincingly.
- **Spawn cadence.** `QuickSpawner.cs` instantiates circle-sprite ingredients at the bottom of the screen with a random upward-and-sideways `Rigidbody2D.AddForce`. The Inspector-exposed `spawnInterval` was tuned live.
- **Bomb hazard.** A red circle prefab with a configurable spawn weight. Slicing it logs `GAME OVER` and nothing else — the production version's screen-shake, vignette, and silence-punch are out of scope here.

## Findings — one-line summary

Read `prototype_findings.md` for the full write-up. The headline: Linecast-based slice detection is the right approach for this scope; the spawn-cadence sweet spot sits in the 0.7–1.0s range for early-game feel; and bomb tension begins to register meaningfully somewhere around 5–8% spawn weight in the late-game phase. Those numbers now live in the ScriptableObject curves on `main` as the starting point for Week 9–10 balancing.

## Running it

Open in **Unity 6000.4.0f1** (URP 2D). Open `Assets/_Sandbox/Sandbox.unity` and press Play. Mouse-drag to slice. Tune `spawnInterval` and the bomb-weight field on the spawner GameObject in the Inspector to feel your way around the parameter space.

There is no Android build configuration on this branch. The prototype was developed and validated against desktop mouse input only — touch parity was deferred to the proper Input System integration on `main`.

## Caveats

- Tag-driven collision logic depends on prefab tags being set in the Editor. A missing `Ingredient` or `Bomb` tag silently falls into the wrong branch and the bug presents as "nothing happens when I slice." If anything behaves strangely, check the prefab tags first — and prefer `CompareTag()` over string `==` in any code you lift from here.
- No object pooling. At sustained high spawn rates the allocation churn is visible in the Profiler. This is one of the things `main` fixes.
- No persistence, no scoring, no menus, no pause. None of that was the question this branch was asked to answer.

## Do not

- Merge this branch to `main`.
- Cherry-pick code from here into production scripts. The values are right; the code is not.
- Treat anything here as the source of truth. `main` and the Technical Documentation are the canon.

