# ArchitectureMatrix

Side‑by‑side samples of **Layered**, **Onion**, **Hexagonal (Ports & Adapters)**, and a starter **Clean** slice for the same tiny domain: `Machine` / `MachineProduction`.

## Repo Map
```
ArchitectureMatrix.sln
0_Layered/
1_Onion/
2_Hexagonal/
  HexagonalMachineMonitoring.Core/
  HexagonalMachineMonitoring.Adapters/
  HexagonalMachineMonitoring.Api/
3_Clean/
  MachineMonitoring.Clean/
```

## Quick Start
```bash
dotnet build ArchitectureMatrix.sln
dotnet run --project 2_Hexagonal/HexagonalMachineMonitoring.Api/HexagonalMachineMonitoring.Api.csproj
```
Swagger exposes **POST /hex/sync**. Configure inputs in `2_Hexagonal/HexagonalMachineMonitoring.Api/appsettings.json`:
```json
{ "Hex": { "Sources": ["Ef", "Http", "Csv"] } }
```

## Hexagonal vs Onion — wins
| Concern | Onion (common) | Hexagonal (by design) |
|---|---|---|
| Rules/Normalization | Duplicated in repos/services | **One policy port** used by all |
| Multiple sources | Pick one repo or brittle composite | Use case consumes **list of sources** |
| Side‑effects | Publishers/loggers leak inward | **Ports**: swap sink/event adapters via DI |
| Drivers | HTTP‑centric services | HTTP/CLI/Queue = inbound adapters |
| Boundaries | Tech types creep into core | Core owns ports; no tech leakage |
| Testing | Heavy infra mocks | Fake ports; pure core tests |

## Main points
- Toggle `Hex:Sources` → call `/hex/sync` → behavior changes, **core unchanged**.  
- Edit `DefaultNormalizationPolicy` → all sources obey; **no adapter edits**.  
- Swap `LogEventPort` ↔ `BusEventPort` or `InMemorySink` ↔ `FileSink` in DI → side‑effects change; **core untouched**.

## Structure Conventions
- **Core**: `Domain`, `Ports/Inbound`, `Ports/Outbound`, `UseCases` (owns interfaces).  
- **Adapters**: `Outbound/Sources|Sink|Events|Policies` (implement ports).  
- **Api**: DI/composition + HTTP inbound adapter.
