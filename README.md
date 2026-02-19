# Substrate

A primitives library for systems that can't afford to be slow.

## Features

- **Zero-allocation hot paths**: No `new`, no boxing where it matters
- **Hardware-adaptive SIMD**: Runs at your CPU's vector width
- **Bounded memory**: Global budget with automatic pressure relief
- **Lock-free concurrency**: MPSC queues, epoch-based reclamation
- **Typed IDs**: Compile-time safety, runtime int performance

## Quick Start

```csharp
using Substrate.Engine;

using var substrate = new Substrate();
var nodes = substrate.CreateTable("nodes");
var classIds = nodes.AddColumn<int>("class_id");
var hashes = nodes.AddColumn<uint>("hash");

int row = nodes.AddRow();
classIds[row] = 42;
hashes[row] = 0xDEADBEEF;
```

## Building

```bash
dotnet build
dotnet run --project examples/Substrate.Examples
dotnet test
```

## License
MIT
