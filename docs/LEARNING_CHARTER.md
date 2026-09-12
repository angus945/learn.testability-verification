# learn.testability-verification — Learning Charter

> 用純 .NET 小型遊戲，練習以 DDD / Clean Architecture 建立「可控制、可觀察、可驗證」的系統，並逐步導入 `crafty-racoon/module.verification`。
>
> 本文件是跨對話的學習基準。後續討論若沒有明確要求改變方向，應以本文件為準，不任意提前導入後續階段能力，也不為了展示框架而增加不必要抽象。

## Testability 與測試策略

本專案的主要學習目標是 **Testability Architecture**，不是 TDD、Unit Testing 方法論，也不以測試覆蓋率為主要成果。

因此後續實作預設採用：

```text
正常設計與實作 Production Code
↓
辨識不可控制、不可觀察或難以隔離的部分
↓
建立必要的 Testability Seam
↓
以少量 Test / Verification 證明該 Seam 有效
```

而不是：

```text
先寫 Test
↓
Red
↓
實作
↓
Green
↓
Refactor
```

除非某個階段明確以 TDD 為學習主題，否則不要使用 TDD 作為預設教學流程。

### 測試在本專案中的角色

Test 的主要用途是：

- 驗證 Architecture Boundary 是否成立。
- 驗證 Dependency 是否真的可以被替換或控制。
- 驗證 Observation Surface 是否足以從外部判斷系統狀態。
- 驗證 Testability Seam 是否有效。
- 驗證 Verification Infrastructure 本身的行為。

Test 不需要：

- 對每個 method 建立完整測試組。
- 為所有邊界條件追求高 coverage。
- 為簡單 Value Object 強制執行 Red / Green / Refactor。
- 為了「可測」而替所有 class 建立 interface。
- 讓測試結構反過來主導 Domain Model。

對單純且天然可測的 Domain Code，可以正常完成 Production implementation，再以少量測試確認核心規則即可。

### 優先研究的 Testability 問題

後續學習應優先關注：

```text
Controllability
Observability
Determinism
Isolation
Stable Entry Point
Stable Observation Surface
Failure Explainability
External Verification
```

例如：

```text
Random.Shared
```

造成結果不可重現時，應研究如何建立 controllable randomness seam。

```text
Console.WriteLine(...)
```

成為唯一知道 gameplay 結果的方法時，應研究 observability。

Test 必須直接存取 Aggregate internal state 才能驗證結果時，應研究 stable observation surface。

Test 需要直接修改 Domain internal state 才能建立 scenario 時，應研究 controllability 與 production entry point。

### Production Code 優先

本專案預設從真實產品設計角度實作 Tiny Arena。

Testability 必須服務 Production Architecture，而不是 Production Architecture 服務 Test。

應避免：

```text
Test-only setter
Test-only constructor
Test-only command
ForceWin()
SetHealthForTest()
TeleportForTest()
直接暴露 internal collection 給測試
```

如果 Test 很難寫，首先應問：

> 這代表系統缺少合理的 controllability / observability boundary，還是只是這個測試沒有必要？

不要立即增加 test-specific API。

### 測試數量不是進度指標

本專案不以：

```text
Test Count
Code Coverage
Mock Count
Assertion Count
```

作為完成度判準。

更重要的判準是：

> 系統是否能透過正常 Application Entry Point 被控制，並透過穩定、被動的 Observation Surface 被外部驗證。

最終目標是：

```text
External Controller
        ↓
Normal Application Entry Point
        ↓
Application / Domain
        ↓
Committed Result
        ↓
Facts / Snapshots
        ↓
Invariant / Oracle
        ↓
Diagnostics / Evidence
```

而不是：

```text
Test
↓
直接操作 Internal Object
↓
大量 Assert
```

### 對 AI 助教的額外要求

後續教學不得因為本專案與 Testability 有關，就預設採用 TDD。

AI 應優先：

1. 先協助正常完成目前的 Production Design。
2. 指出其中實際存在的 Testability Problem。
3. 解釋該問題屬於 Controllability、Observability、Determinism、Isolation 或其他哪一類。
4. 只有在需要證明設計有效時，才加入最小必要 Test。
5. 不為了增加 Test 而增加 abstraction。
6. 不追求測試完整度，除非該完整度本身就是當前學習目標。

如果 Production Code 本身已經天然容易測試，就不需要額外建立 Testability Infrastructure。

---

# 1. 專案定位

Repository：

```text
learn.testability-verification
```

用途：

```text
Learning Project
```

不是 reusable framework，也不是 `module.*`。

目標：

1. 練習 Testability Architecture，而不只是 Unit Testing。
2. 練習 DDD / Clean Architecture 下的 dependency boundary。
3. 練習如何讓系統具備：
   - Controllability
   - Observability
   - Deterministic seams
   - Runtime invariant verification
   - External oracle verification
   - Failure diagnostics
   - Evidence collection
4. 練習正確採用 `module.verification`，而不是讓 verification framework 反過來主導產品架構。
5. 保持純 .NET，不依賴 Unity。

---

# 2. 核心學習原則

整個專案遵守下列順序：

```text
Pure Domain
    ↓
Dependency Control
    ↓
Controllability
    ↓
Observability
    ↓
State Observation
    ↓
Invariant Verification
    ↓
Runtime Control
    ↓
Oracle Verification
    ↓
Diagnostics / Evidence
```

最重要的原則：

> 先讓產品本身具備良好的可測試性，再導入 verification module。

禁止採取：

```text
Install verification framework
↓
讓遊戲邏輯圍繞 verification API 設計
```

正確方向：

```text
產品本身有正常 Application Entry Point
↓
產品本身可以 deterministic testing
↓
產品本身可以被動觀察
↓
verification adapter 接上去
```

---

# 3. 遊戲題目

專案使用一個很小的純 .NET 回合制格子戰鬥遊戲：

```text
Tiny Arena
```

基礎規則：

| 項目         | 規格                                  |
| ------------ | ------------------------------------- |
| 地圖         | 5 × 5                                 |
| 玩家         | 1                                     |
| 敵人         | 2 個 Slime                            |
| 玩家行動     | Move / Attack / Wait                  |
| 敵人回合     | 玩家完成合法 Action 後依固定順序執行  |
| Damage       | 初期固定，之後透過 Random abstraction |
| 勝利條件     | 所有敵人死亡                          |
| 失敗條件     | 玩家死亡                              |
| Presentation | Console                               |
| Persistence  | 初期 InMemory                         |
| Runtime      | 單執行緒                              |
| Unity        | 不使用                                |
| Network      | 不使用                                |
| Database     | 不使用                                |

遊戲只是教學載體。

不要把功能複雜度當成學習目標。

---

# 4. Repository 結構

預期結構：

```text
learn.testability-verification/
│
├─ src/
│  ├─ TinyArena.Domain/
│  ├─ TinyArena.Application/
│  ├─ TinyArena.Infrastructure/
│  ├─ TinyArena.Verification/
│  └─ TinyArena.Console/
│
├─ tests/
│  ├─ TinyArena.Domain.Tests/
│  ├─ TinyArena.Application.Tests/
│  ├─ TinyArena.Verification.Tests/
│  └─ TinyArena.AcceptanceTests/
│
├─ modules/
│  └─ module.verification/
│
├─ docs/
│  └─ LEARNING_CHARTER.md
│
└─ TinyArena.sln
```

---

# 5. module.verification 引入方式

`crafty-racoon/module.verification` 使用 Git submodule：

```text
modules/module.verification
```

概念：

```text
learn.testability-verification
└─ modules/
   └─ module.verification/
```

主專案負責：

- 決定使用哪一個 module.verification commit。
- 決定哪些 verification modules 被實際引用。
- 建立 TinyArena 專屬 adapter。
- 建立 Composition Root。
- 不修改 verification module 來迎合 TinyArena。

禁止：

```text
src/TinyArena.Domain/module.verification
src/TinyArena.Application/module.verification
```

也不要把 submodule 放進任何單一 bounded context 內。

---

# 6. Clean Architecture Dependency Rule

基本方向：

```text
TinyArena.Console
        │
        ├───────────────┐
        ↓               ↓
TinyArena.Application   TinyArena.Verification
        │               │
        ↓               │
TinyArena.Domain ←──────┘
        ↑
        │
TinyArena.Infrastructure
```

更精確的 compile dependency：

```text
TinyArena.Domain
    → 不依賴其他 TinyArena assembly

TinyArena.Application
    → TinyArena.Domain

TinyArena.Infrastructure
    → TinyArena.Application
    → TinyArena.Domain

TinyArena.Verification
    → TinyArena.Application
    → TinyArena.Domain
    → Module.Verification.*

TinyArena.Console
    → TinyArena.Application
    → TinyArena.Infrastructure
    → TinyArena.Verification
```

Composition Root 位於最外層 host。

---

# 7. Domain 原則

Domain 只表達遊戲規則。

預期核心模型：

```text
GameSession                 Aggregate Root

Actor
├─ Player
└─ Enemy

Position                    Value Object
Health                      Value Object

GameStatus
├─ Running
├─ Won
└─ Lost
```

典型行為：

```text
GameSession.MovePlayer(...)
GameSession.Attack(...)
GameSession.Wait(...)
GameSession.ExecuteEnemyTurn(...)
```

Domain 不負責：

- logging
- persistence
- JSON
- Console
- test framework
- verification framework
- random implementation
- system clock
- service locator
- global singleton

---

# 8. Testability 約束

下列規則整個專案期間固定成立。

## 8.1 不可控依賴

Domain / Application 禁止直接使用：

```text
DateTime.Now
DateTime.UtcNow
Random.Shared
Guid.NewGuid()
Thread.Sleep
Environment.*
Console.*
static global service
Service Locator
```

若真的需要，先建立 stable abstraction。

---

## 8.2 Domain Tests

理想 Domain Test：

```text
Arrange Aggregate
↓
Invoke Domain Behavior
↓
Assert Domain State
```

Domain Test 原則上不應依賴：

```text
Mock ILogger
Mock Repository
Mock IServiceProvider
Mock EventBus
```

若大量出現，代表 Domain boundary 需要重新檢查。

---

## 8.3 Test 不得建立第二條產品路徑

禁止 test-only 操作：

```text
TeleportPlayer()
SetEnemyHealth(0)
ForceWin()
SkipValidation()
SetInternalState(...)
```

Acceptance Test 必須盡量經過正式 Application Entry Point。

---

# 9. Verification Module 的角色

目前 `module.verification` 中的重要能力分工如下。

## SystemFact

描述：

```text
已經發生的事實
```

例如：

```text
PlayerMoved
DamageApplied
ActorDefeated
BattleWon
```

不是 command：

```text
ApplyDamage      X
DamageApplied    O
```

具體 fact 由真正擁有語意的產品 module 定義。

---

## SystemFact.Observability

負責：

```text
Fact
↓
Observation Hub
↓
Observer
```

Observer 必須 passive。

Observer failure 不得影響產品正常流程。

---

## StateSnapshot

負責：

```text
Host-published immutable observation
```

核心原則：

```text
Read != Capture
```

Host 在 consistency boundary 主動 capture / publish。

Reader 只讀已發布 snapshot。

Snapshot 不是：

```text
Save Game
Replay State
Restore Point
Mutable DTO
```

---

## Invariant

負責：

```text
Committed Context
↓
Evaluate
↓
InvariantResult
```

Invariant：

- 不修改 gameplay。
- 不執行修復。
- 不替代 Domain validation。
- 不等同 operation failure。

---

## RuntimeControl

負責：

```text
Operation Admission
Handle
Completion
Result Lookup
```

它不負責真正的 gameplay execution。

正確：

```text
RuntimeControl
↓
TinyArena Adapter
↓
Application Use Case
↓
Domain
```

錯誤：

```text
RuntimeControl
↓
直接改 Aggregate
```

---

## Oracle

負責：

```text
Immutable Observation
↓
Expectation Evaluation
↓
Verdict
```

Oracle：

- 不送 command。
- 不控制 runtime。
- 不 capture state。
- 不保存 runtime history。

尤其必須區分：

```text
Operation Outcome
```

與：

```text
Test Verdict
```

---

## Diagnostics

描述：

```text
發生了什麼問題
```

不決定：

```text
是否 retry
是否 stop
是否 throw
是否 test failed
```

Execution Policy 由 host 決定。

---

## Evidence

保存：

```text
references + metadata
```

不複製 authoritative payload。

Host 自己負責：

```text
JSON
File
Database
CI Artifact
```

---

# 10. 學習階段

---

# Stage 00 — Architecture Skeleton

## 目標

建立：

```text
TinyArena.Domain
TinyArena.Application
TinyArena.Infrastructure
TinyArena.Console

TinyArena.Domain.Tests
TinyArena.Application.Tests
```

尚不引入 verification module。

## 學習重點

- Clean Architecture dependency direction
- Assembly boundary
- Composition Root
- ProjectReference 管理

## 完成條件

核心測試可在沒有以下項目的情況執行：

```text
Console
File System
Unity
Network
Database
Clock
Thread
```

## 不要提前做

- SystemFact
- Snapshot
- RuntimeControl
- Oracle
- Evidence

---

# Stage 01 — Pure Domain

## 實作

完成：

```text
GameSession
Actor
Player
Enemy
Position
Health
GameStatus
```

以及：

```text
Move
Attack
Wait
EnemyTurn
Win
Lose
```

## 測試

至少包含：

```text
Player cannot move outside board
Player cannot move into occupied cell
Attack reduces health
Dead actor cannot act
All enemies dead → Won
Player dead → Lost
```

## 學習重點

- Aggregate
- Entity
- Value Object
- Invariant inside Domain Model
- Pure unit testing

注意：

這裡的「Domain invariant」是產品規則。

它和後面的 `Module.Verification.Invariant` 是不同層次。

---

# Stage 02 — Application & Test Seams

## 實作 Use Cases

```text
StartGame
SubmitPlayerAction
GetGameState
```

建立 ports：

```text
IGameSessionRepository
IRandomSource
```

Infrastructure：

```text
InMemoryGameSessionRepository
SystemRandomSource
```

Tests：

```text
FakeGameSessionRepository
SequenceRandomSource
```

## 學習重點

- Application boundary
- Ports and Adapters
- Dependency Inversion
- Test Double
- deterministic input

## 完成條件

Application tests 可以完全控制：

```text
Initial State
Input
Randomness
Persistence
```

---

# Stage 03 — System Facts

## 引入

```text
Module.Verification.SystemFact
Module.Verification.SystemFact.Observability
```

## 建立 Facts

例如：

```text
PlayerMoved
DamageApplied
ActorDefeated
TurnCompleted
BattleWon
BattleLost
```

## 正確流程

```text
Application Use Case
↓
Domain Behavior
↓
Commit
↓
Publish Facts
↓
Return
```

重要：

```text
Commit before Publish
```

避免發布尚未正式成立的事實。

## 學習重點

- passive observability
- fact vs event vs command
- post-commit observation
- producer / observer decoupling

## 完成條件

Acceptance Test 能觀察 gameplay fact，而不需要讀 Logger。

---

# Stage 04 — State Snapshot

## 引入

```text
Module.Verification.StateSnapshot
```

## 建立

```text
BattleSnapshot
├─ Turn
├─ Status
├─ Player
│  ├─ Position
│  └─ Health
└─ Enemies[]
```

Snapshot 必須 immutable。

## 流程

```text
Commit
↓
Create BattleSnapshot
↓
Publish Snapshot
```

## 學習重點

- observation boundary
- consistency boundary
- black-box state verification
- immutable projection

## Acceptance Test 方向

避免：

```text
repository.Get()
↓
直接檢查 Aggregate
```

改成：

```text
Execute Operation
↓
Read Snapshot
↓
Assert Observation
```

## 不要做

Snapshot 不得拿來：

```text
Restore
Replay
Save Game
```

---

# Stage 05 — Verification Invariants

## 引入

```text
Module.Verification.Invariant
```

## 建立規則

例如：

```text
HealthRangeInvariant
ActorPositionInvariant
ActorOverlapInvariant
WonStateInvariant
LostStateInvariant
```

## 流程

```text
Committed Snapshot
↓
InvariantRegistry
↓
InvariantResult
```

## 學習重點

- product validation vs external verification
- passive runtime assertions
- post-condition checking
- invariant registry

## 必須理解

```text
Operation Success
```

與：

```text
Invariant Verification Success
```

不是同一件事。

例如：

```text
Attack Operation = Success
Invariant = Failed
```

代表產品接受並完成操作，但 committed state 違反應有條件。

---

# Stage 06 — Runtime Control

## 引入

```text
Module.Verification.RuntimeControl
```

## 建立 Adapter

```text
TinyArenaRuntimeAdapter
```

方向：

```text
Test
↓
RuntimeControl
↓
TinyArenaRuntimeAdapter
↓
Application Use Case
↓
Domain
```

## 學習重點

- controllability
- production entry point reuse
- operation lifecycle
- avoiding test-only execution paths

## 禁止

```text
RuntimeControl
↓
Domain internal mutation
```

---

# Stage 07 — Oracle

## 引入

```text
Module.Verification.Oracle
```

## 建立 Observation Context

例如：

```text
BattleObservation
├─ LatestSnapshot
├─ Facts
├─ OperationResult
└─ InvariantResults
```

## 建立 Oracles

例如：

```text
PlayerAtOracle
EnemyDeadOracle
GameWonOracle
FactOccurredOracle
InvariantCleanOracle
```

## 流程

```text
Execute Scenario
↓
Collect Immutable Observation
↓
Evaluate Oracles
↓
Verdict
```

## 學習重點

- expectation language
- operation outcome vs test verdict
- black-box scenario verification
- passive evaluation

---

# Stage 08 — Diagnostics & Evidence

## 引入

```text
Module.Verification.Diagnostics
Module.Verification.Evidence
```

## Diagnostics

讓失敗可以回答：

```text
What failed?
Where?
With what values?
Under which operation / observation?
```

例如：

```text
Invariant:
    actor.health.range

Diagnostic:
    Actor health is outside legal range.

Location:
    actor:slime01.health

Properties:
    health = -3
    maxHealth = 10
```

## Evidence

組成：

```text
EvidenceBundle
├─ operation-stream reference
├─ fact-stream reference
├─ snapshot reference
├─ invariant-report reference
├─ oracle-report reference
└─ diagnostic reference
```

Host 可以自行實作：

```text
JsonEvidenceWriter
```

例如：

```text
artifacts/
└─ scenario-001/
   ├─ evidence.json
   ├─ operations.json
   ├─ facts.json
   ├─ snapshots.json
   └─ diagnostics.json
```

## 學習重點

- failure explainability
- diagnostics vs execution policy
- evidence manifest
- authoritative data ownership

---

# 11. 最終 Verification Pipeline

目標結構：

```text
                    ┌─────────────────┐
                    │ Console / Tests │
                    └────────┬────────┘
                             │
                  ┌──────────▼──────────┐
                  │   Runtime Control   │
                  └──────────┬──────────┘
                             │
                     normal entry point
                             │
                  ┌──────────▼──────────┐
                  │    Application      │
                  └──────────┬──────────┘
                             │
                  ┌──────────▼──────────┐
                  │       Domain        │
                  └──────────┬──────────┘
                             │
                           Commit
                             │
          ┌──────────────────┼──────────────────┐
          ↓                  ↓                  ↓
     System Facts        Snapshot            Result
          │                  │
          │                  ↓
          │              Invariant
          │                  │
          └────────┬─────────┘
                   ↓
               Observation
                   │
                   ↓
                 Oracle
                   │
                   ↓
                Verdict
                   │
          ┌────────┴────────┐
          ↓                 ↓
     Diagnostics         Evidence
```

這是學習目標，不是第一天就應該建立的架構。

---

# 12. 固定禁止事項

除非有明確學習目的，不要做：

```text
MediatR
CQRS Framework
Event Sourcing
Message Broker
Database
Network API
Unity
Generic Repository Framework
Generic Result Framework
Custom DI Framework
Reflection-based Auto Registration
Source Generator
Plugin System
Replay System
Simulation Framework
ECS
```

原因：

這些東西會讓 Testability 主題失焦。

如果後續需要導入，必須先回答：

> 它正在解決哪個目前實際存在的問題？

---

# 13. 不追求的事情

本專案不以這些為目標：

```text
完整遊戲
漂亮 Console UI
高效能
多人連線
Persistence
Production deployment
高度 generic abstraction
Reusable TinyArena framework
```

TinyArena 可以寫得很小。

核心是 architecture learning。

---

# 14. 每個階段的開發方式

每個 Stage 都遵守：

```text
1. 先定義目前階段要解決的問題
2. 寫最小測試
3. 建最小 production code
4. 完成階段需求
5. Refactor
6. 檢查 dependency direction
7. 確認沒有偷用後續階段能力
8. Commit
```

不要：

```text
先把最終架構全部 scaffold 出來
```

---

# 15. 建議 Git Branch / Tag

可使用：

```text
stage/00-architecture
stage/01-domain
stage/02-application-seams
stage/03-system-facts
stage/04-state-snapshot
stage/05-invariants
stage/06-runtime-control
stage/07-oracles
stage/08-evidence-diagnostics
stage/09-final
```

也可以在完成後打 tag：

```text
stage-00
stage-01
...
```

目的不是 Git Flow，而是方便比較每一次架構演化。

---

# 16. 學習對話規則

後續開新 ChatGPT 對話時，可以附上本文件，並要求：

> 依 `LEARNING_CHARTER.md` 繼續 `learn.testability-verification`。不要提前導入尚未進入的 Stage，也不要為了「最佳實踐」額外加入文件沒有要求的 framework。先確認目前 Stage、目前完成項目，再教下一個最小步驟。

回答時應優先說明：

1. 現在位於哪個 Stage。
2. 這一步正在練什麼 testability concept。
3. 為什麼需要這個 abstraction。
4. 如果沒有它，測試會遇到什麼問題。
5. Production dependency direction。
6. Test dependency direction。
7. 完成這一步的驗收條件。
8. 哪些東西現在刻意不做。

---

# 17. 對 AI 助教的固定要求

在這個專案中，AI 應遵守：

## 不要過度設計

如果：

```text
一個 interface
一個 concrete implementation
一個 fake
```

足以教清楚概念，就不要引入 framework。

---

## 不要一次給完整最終答案

教學應偏向：

```text
小步驟
↓
實作
↓
測試
↓
理解
↓
下一步
```

而不是直接生成整套最終 repository。

---

## 每個 abstraction 都必須有理由

不能只因為：

```text
Clean Architecture 都這樣寫
```

就加入 abstraction。

必須指出具體問題，例如：

```text
Random.Shared 導致測試不可重現
↓
所以建立 IRandomSource
```

---

## Verification 不能污染 Domain

預設：

```text
TinyArena.Domain
```

不知道：

```text
Oracle
RuntimeControl
Evidence
Diagnostics
StateSnapshot Channel
```

若真的需要 Domain 實作 shared factual contract，應先明確說明語意所有權與 dependency trade-off。

---

## 優先 production-like testing

Acceptance Test 優先：

```text
正常 Application Entry Point
+
公開 Verification Surface
```

而不是：

```text
直接取得 Aggregate
+
修改 internal state
```

---

# 18. 完成整個專案後應能回答的問題

完成後，應該能清楚回答：

### Testability

- Testability 和 Unit Testing 有什麼不同？
- Controllability 是什麼？
- Observability 是什麼？
- 為什麼 deterministic seam 很重要？
- 為什麼 Test API 不應建立第二條產品執行路徑？

### DDD / Clean Architecture

- Domain validation 和 verification invariant 有什麼不同？
- Application boundary 在哪？
- 為什麼 RuntimeControl 要回到正常 Use Case？
- Snapshot 為什麼不應直接暴露 Aggregate？
- Fact 的語意應由誰擁有？

### Verification

- SystemFact 和 Command 有什麼不同？
- Snapshot 和 Replay State 有什麼不同？
- Operation Outcome 和 Oracle Verdict 有什麼不同？
- Invariant failure 和 operation failure 有什麼不同？
- Diagnostics 和 Exception Policy 有什麼不同？
- Evidence reference 和 authoritative payload 有什麼不同？

如果這些問題只能靠背 API 回答，代表學習還沒有完成。

---

# 19. 最終判準

這個專案成功的判準不是：

```text
使用了多少 module.verification API
```

而是：

> 即使拿掉 `module.verification`，TinyArena 本身仍然具有乾淨的 Domain、清楚的 Application Boundary，以及良好的 dependency seams。

然後：

> `module.verification` 能透過外部 adapter，對正常產品路徑提供控制、觀察、驗證與 evidence，而不需要侵入核心 gameplay。

這才是本專案真正要練的 Testability Architecture。
