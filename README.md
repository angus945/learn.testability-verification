# learn.testability-verification

使用純 .NET 小型遊戲，練習 **Testability Architecture**、**DDD** 與 **Clean Architecture**。

本專案會逐步導入 [`crafty-racoon/workspace.verification`](https://github.com/crafty-racoon/workspace.verification)，練習如何讓系統具備：

* Controllability
* Observability
* Deterministic Testing
* State Snapshot
* Runtime Invariant Verification
* Runtime Control
* Oracle Verification
* Diagnostics
* Evidence Collection

## 練習題目

專案使用一個簡單的回合制格子戰鬥遊戲：

```text
Tiny Arena
```

基本規則：

* 5 × 5 地圖
* 1 名玩家
* 2 名敵人
* 玩家可以 Move / Attack / Wait
* 玩家行動後敵人依固定順序行動
* 擊敗所有敵人即勝利
* 玩家死亡即失敗

遊戲本身只是學習載體，重點在架構與可測試性。

## 技術環境

```text
.NET
C#
xUnit
DDD
Clean Architecture
```

不使用 Unity。

## 專案結構

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
│  └─ workspace.verification/
│
├─ docs/
│  └─ LEARNING_CHARTER.md
│
└─ TinyArena.sln
```

## workspace.verification

`workspace.verification` 以 Git Submodule 引入：

```bash
git submodule add https://github.com/crafty-racoon/workspace.verification.git modules/workspace.verification
```

Clone 專案時：

```bash
git clone --recurse-submodules <repository-url>
```

如果已經 Clone：

```bash
git submodule update --init --recursive
```

## 學習階段

```text
Stage 00  Architecture Skeleton
Stage 01  Pure Domain
Stage 02  Application & Test Seams
Stage 03  System Facts
Stage 04  State Snapshot
Stage 05  Verification Invariants
Stage 06  Runtime Control
Stage 07  Oracle
Stage 08  Diagnostics & Evidence
```

學習方向：

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

## 核心原則

本專案不是為了展示 `workspace.verification` API。

目標是先建立一個本身具有良好可測試性的系統，再讓 verification 能力從外部接入。

因此：

```text
Domain
```

不應依賴：

```text
Oracle
RuntimeControl
Diagnostics
Evidence
StateSnapshot
```

Verification 應盡量透過正式 Application Entry Point 控制系統，並透過公開 observation surface 驗證結果。

## 學習文件

完整學習方向與架構約束請參考：

```text
docs/LEARNING_CHARTER.md
```

後續實作應以該文件作為主要學習基準。
