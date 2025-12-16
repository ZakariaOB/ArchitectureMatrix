[![.NET Core](https://github.com/ardalis/CleanArchitecture/workflows/.NET%20Core/badge.svg)](https://github.com/ardalis/CleanArchitecture/actions)
[![publish Ardalis.CleanArchitecture Template to nuget](https://github.com/ardalis/CleanArchitecture/actions/workflows/publish.yml/badge.svg)](https://github.com/ardalis/CleanArchitecture/actions/workflows/publish.yml)
[![Ardalis.CleanArchitecture.Template on NuGet](https://img.shields.io/nuget/v/Ardalis.CleanArchitecture.Template?label=Ardalis.CleanArchitecture.Template)](https://www.nuget.org/packages/Ardalis.CleanArchitecture.Template/)

<a href="https://twitter.com/intent/follow?screen_name=ardalis">
    <img src="https://img.shields.io/twitter/follow/ardalis.svg?label=Follow%20@ardalis" alt="Follow @ardalis" />
</a> &nbsp; <a href="https://twitter.com/intent/follow?screen_name=nimblepros">
    <img src="https://img.shields.io/twitter/follow/nimblepros.svg?label=Follow%20@nimblepros" alt="Follow @nimblepros" />
</a>

<p>

![Alt](https://repobeats.axiom.co/api/embed/be5094dd306ba53b8f4fc0b43c9de5d8ca23a608.svg "Repobeats analytics image")

</p>

# Clean Architecture

A starting point for Clean Architecture with ASP.NET Core. [Clean Architecture](https://8thlight.com/blog/uncle-bob/2012/08/13/the-clean-architecture.html) is just the latest in a series of names for the same loosely-coupled, dependency-inverted architecture. You will also find it named [hexagonal](https://alistair.cockburn.us/hexagonal-architecture), [ports-and-adapters](http://www.dossier-andreas.net/software_architecture/ports_and_adapters.html), or [onion architecture](http://jeffreypalermo.com/blog/the-onion-architecture-part-1/).

Learn more about Clean Architecture and this template in [NimblePros' Introducing Clean Architecture course](https://academy.nimblepros.com/p/learn-clean-architecture). Use code ARDALIS to save 20%.

This architecture is used in the [DDD Fundamentals course](https://www.pluralsight.com/courses/fundamentals-domain-driven-design) by [Steve Smith](https://ardalis.com) and [Julie Lerman](https://thedatafarm.com/).

:school: Contact Steve's company, [NimblePros](https://nimblepros.com/), for Clean Architecture or DDD training and/or implementation assistance for your team.

## Take the Course!

[Learn about how to implement Clean Architecture](https://academy.nimblepros.com/p/intro-to-clean-architecture) from [NimblePros](https://nimblepros.com) trainers [Sarah "sadukie" Dutkiewicz](https://blog.nimblepros.com/author/sadukie/) and [Steve "ardalis" Smith](https://blog.nimblepros.com/author/ardalis/).

## Table Of Contents

- [Clean Architecture](#clean-architecture)
  - [Troubleshooting Chrome Errors](#troubleshooting-chrome-errors)
  - [Table Of Contents](#table-of-contents)
  - [Give a Star! :star:](#give-a-star-star)
  - [Versions](#versions)
  - [Learn More](#learn-more)
- [Getting Started](#getting-started)
  - [Using the dotnet CLI template](#using-the-dotnet-cli-template)
  - [What about Controllers and Razor Pages?](#what-about-controllers-and-razor-pages)
    - [Add Ardalis.ApiEndpoints](#add-ardalisapiendpoints)
    - [Add Controllers](#add-controllers)
    - [Add Razor Pages](#add-razor-pages)
  - [Using the GitHub Repository](#using-the-github-repository)
  - [Running Migrations](#running-migrations)
- [Goals](#goals)
  - [History and Shameless Plug Section](#history-and-shameless-plug-section)
- [Design Decisions and Dependencies](#design-decisions-and-dependencies)
  - [Where To Validate](#where-to-validate)
  - [The Core Project](#the-core-project)
  - [The Use Cases Project](#the-use-cases-project)
  - [The Infrastructure Project](#the-infrastructure-project)
  - [The Web Project](#the-web-project)
  - [The SharedKernel Project](#the-sharedkernel-project)
  - [The Test Projects](#the-test-projects)
- [Patterns Used](#patterns-used)
  - [Domain Events](#domain-events)
  - [Related Projects](#related-projects)

## Give a Star! :star:

If you like or are using this project to learn or start your solution, please give it a star. Thanks!

Or if you're feeling really generous, we now support GitHub sponsorships - see the button above.

## Sponsors

I'm please to announce that [Amazon AWS's FOSS fund](https://github.com/aws/dotnet-foss) has chosen to award a 12-month sponsorship to this project. Thank you, and thanks to all of my other past and current sponsors!

## Troubleshooting Chrome Errors

By default the site uses HTTPS and expects you to have a self-signed developer certificate for localhost use. If you get an error with Chrome [see this answer](https://stackoverflow.com/a/31900210/13729) for mitigation instructions.

## Versions

The main branch is now using **.NET 9**. This corresponds with NuGet package version 10.x. Previous versions are available - see our [Releases](https://github.com/ardalis/CleanArchitecture/releases).

## Learn More

- [Live Stream Recordings Working on Clean Architecture](https://www.youtube.com/c/Ardalis/search?query=clean%20architecture)
- [DotNetRocks Podcast Discussion with Steve "ardalis" Smith](https://player.fm/series/net-rocks/clean-architecture-with-steve-smith)
- [Fritz and Friends Streaming Discussion with Steve "ardalis" Smith](https://www.youtube.com/watch?v=k8cZUW4MS3I)

# Getting Started

To use this template, there are a few options:

- Install using `dotnet new` (recommended)
- Download this Repository (and modify as needed)

## Using the dotnet CLI template

First, install the template from [NuGet (https://www.nuget.org/packages/Ardalis.CleanArchitecture.Template/)](https://www.nuget.org/packages/Ardalis.CleanArchitecture.Template/):

```powershell
dotnet new install Ardalis.CleanArchitecture.Template
```

You can see available options by running the command with the `-?` option:

```powershell
dotnet new clean-arch -?
ASP.NET Clean Architecture Solution (C#)
Author: Steve Smith @ardalis, Erik Dahl

Usage:
  dotnet new clean-arch [options] [template options]

Options:
  -n, --name <name>       The name for the output being created. If no name is specified, the name of the output
                          directory is used.
  -o, --output <output>   Location to place the generated output.
  --dry-run               Displays a summary of what would happen if the given command line were run if it would result
                          in a template creation.
  --force                 Forces content to be generated even if it would change existing files.
  --no-update-check       Disables checking for the template package updates when instantiating a template.
  --project <project>     The project that should be used for context evaluation.
  -lang, --language <C#>  Specifies the template language to instantiate.
  --type <project>        Specifies the template type to instantiate.

Template options:
  -as, --aspire  Include .NET Aspire.
                 Type: bool
                 Default: false
```

You should see the template in the list of templates from `dotnet new list` after this installs successfully. Look for "ASP.NET Clean Architecture Solution" with Short Name of "clean-arch".

Navigate to the parent directory in which you'd like the solution's folder to be created.

Run this command to create the solution structure in a subfolder name `Your.ProjectName`:

```
dotnet new clean-arch -o Your.ProjectName
```

The `Your.ProjectName` directory and solution file will be created, and inside that will be all of your new solution contents, properly namespaced and ready to run/test!

Example:
![powershell screenshot showing steps](https://user-images.githubusercontent.com/782127/101661723-9fd28e80-3a16-11eb-8be4-f9195d825ad6.png)

Thanks [@dahlsailrunner](https://github.com/dahlsailrunner) for your help getting this working!

**Known Issues**: 

- Don't include hyphens in the name. See [#201](https://github.com/ardalis/CleanArchitecture/issues/201).
- Don't use 'Ardalis' as your namespace (conflicts with dependencies).

## What about Controllers and Razor Pages?

As of version 9, this solution template only includes support for API Endpoints using the FastEndpoints library. If you want to use my ApiEndpoints library, Razor Pages, and/or Controllers you can use the last template that included them, [version 7.1](https://www.nuget.org/packages/Ardalis.CleanArchitecture.Template/7.1.0). Alternately, they're easily added to this template after installation.

### Add Ardalis.ApiEndpoints

To use [Ardalis.ApiEndpoints](https://www.nuget.org/packages/Ardalis.ApiEndpoints) instead of (or in addition to) [FastEndpoints](https://fast-endpoints.com/), just add the reference and use the base classes from the documentation.

```powershell
dotnet add package Ardalis.ApiEndpoints
```

### Add Controllers

You'll need to add support for controllers to the Program.cs file. You need:

```csharp
builder.Services.AddControllers(); // ControllersWithView if you need Views

// and

app.MapControllers();
```

Once these are in place, you should be able to create a Controllers folder and (optionally) a Views folder and everything should work as expected. Personally I find Razor Pages to be much better than Controllers and Views so if you haven't fully investigated Razor Pages you might want to do so right about now before you choose Views.

### Add Razor Pages

You'll need to add support for Razor Pages to the Program.cs file. You need:

```csharp
builder.Services.AddRazorPages();

// and

app.MapRazorPages();
```

Then you just add a Pages folder in the root of the project and go from there.

## Using the GitHub Repository

To get started based on this repository, you need to get a copy locally. You have three options: fork, clone, or download. Most of the time, you probably just want to download.

You should **download the repository**, unblock the zip file, and extract it to a new folder if you just want to play with the project or you wish to use it as the starting point for an application.

You should **fork this repository** only if you plan on submitting a pull request. Or if you'd like to keep a copy of a snapshot of the repository in your own GitHub account.

You should **clone this repository** if you're one of the contributors and you have commit access to it. Otherwise you probably want one of the other options.

## Running Migrations

You shouldn't need to do this to use this template, but if you want migrations set up properly in the Infrastructure project, you need to specify that project name when you run the migrations command.

In Visual Studio, open the Package Manager Console, and run `Add-Migration InitialMigrationName -StartupProject Your.ProjectName.Web -Context AppDbContext -Project Your.ProjectName.Infrastructure`.

In a terminal with the CLI, the command is similar. Run this from the Web project directory:

```powershell
dotnet ef migrations add MIGRATIONNAME -c AppDbContext -p ../Your.ProjectName.Infrastructure/Your.ProjectName.Infrastructure.csproj -s Your.ProjectName.Web.csproj -o Data/Migrations
```

To use SqlServer, change `options.UseSqlite(connectionString));` to `options.UseSqlServer(connectionString));` in the `Your.ProjectName.Infrastructure.StartupSetup` file. Also remember to replace the `SqliteConnection` with `DefaultConnection` in the `Your.ProjectName.Web.Program` file, which points to your Database Server.

To update the database use this command from the Web project folder (replace `MachineMonitoringClean10` with your project's name):

```powershell
dotnet ef database update -c AppDbContext -p ../MachineMonitoringClean10.Infrastructure/MachineMonitoringClean10.Infrastructure.csproj -s MachineMonitoringClean10.Web.csproj
```

# Goals

The goal of this repository is to provide a basic solution structure that can be used to build Domain-Driven Design (DDD)-based or simply well-factored, SOLID applications using .NET Core. Learn more about these topics here:

- [SOLID Principles for C# Developers](https://www.pluralsight.com/courses/csharp-solid-principles)
- [Domain-Driven Design Fundamentals](https://www.pluralsight.com/courses/fundamentals-domain-driven-design)
- [Refactoring to SOLID C# Code](https://www.pluralsight.com/courses/refactoring-solid-c-sharp-code)

If you're used to building applications as single-project or as a set of projects that follow the traditional UI -> Business Layer -> Data Access Layer "N-Tier" architecture, I recommend you check out these two courses (ideally before DDD Fundamentals):

- [Creating N-Tier Applications in C#, Part 1](https://www.pluralsight.com/courses/n-tier-apps-part1)
- [Creating N-Tier Applications in C#, Part 2](https://www.pluralsight.com/courses/n-tier-csharp-part2)

Steve Smith also maintains Microsoft's reference application, eShopOnWeb, and its associated free eBook. Check them out here:

- [eShopOnWeb on GitHub](https://github.com/nimblepros/eShopOnWeb) (now supported by [NimblePros](https://nimblepros.com))
- [Architecting Modern Web Applications with ASP.NET Core and Microsoft Azure](https://aka.ms/webappebook) (eBook)

Note that the goal of this project and repository is **not** to provide a sample or reference application. It's meant to just be a template, but with enough pieces in place to show you where things belong as you set up your actual solution. Instead of useless "Class1.cs" there are a few real classes in place. Delete them as soon as you understand why they're there and where you should put your own, similar files. There *is* a sample application in the `/sample` folder, if you're looking for that.

## History and Shameless Plug Section

I've used this starter kit to teach the basics of ASP.NET Core using Domain-Driven Design concepts and patterns for some time now (starting when ASP.NET Core was still in pre-release). Typically I teach a one- or two-day hands-on workshop ahead of events like DevIntersection, or private on-site workshops for companies looking to bring their teams up to speed with the latest development technologies and techniques. Feel free to [contact me](https://nimblepros.com/) if you'd like information about upcoming workshops.

# Design Decisions and Dependencies

The goal of this solution template is to provide a fairly bare-bones starter kit for new projects. It does not include every possible framework, tool, or feature that a particular enterprise application might benefit from. Its choices of technology for things like data access are rooted in what is the most common, accessible technology for most business software developers using Microsoft's technology stack. It doesn't (currently) include extensive support for things like logging, monitoring, or analytics, though these can all be added easily. Below is a list of the technology dependencies it includes, and why they were chosen. Most of these can easily be swapped out for your technology of choice, since the nature of this architecture is to support modularity and encapsulation.

## Where To Validate

Validation of user input is a requirement of all software applications. The question is, where does it make sense to implement it in a concise and elegant manner? This solution template includes 4 separate projects, each of which might be responsible for performing validation as well as enforcing business invariants (which, given validation should already have occurred, are usually modeled as exceptions).

- [When to Validate and When to Throw Exceptions](https://www.youtube.com/watch?v=dpPcnAT7n7M)

The domain model itself should generally rely on object-oriented design to ensure it is always in a consistent state. It leverages encapsulation and limits public state mutation access to achieve this, and it assumes that any arguments passed to it have already been validated, so null or other improper values yield exceptions, not validation results, in most cases.

The use cases / application project includes the set of all commands and queries the system supports. It's frequently responsible for validating its own command and query objects. This is most easily done using a [chain of responsibility pattern](https://deviq.com/design-patterns/chain-of-responsibility-pattern) via MediatR behaviors or some other pipeline.

The Web project includes all API endpoints, which include their own request and response types, following the [REPR pattern](https://deviq.com/design-patterns/repr-design-pattern). The FastEndpoints library includes built-in support for validation using FluentValidation on the request types. This is a natural place to perform input validation as well.

Having validation occur both within the API endpoints and then again at the use case level may be considered redundant. There are tradeoffs to adding essentially the same validation in two places, one for API requests and another for messages sent to Use Case handlers. Following defensive coding, it often makes sense to add validation in both places, as the overhead is minimal and the peace of mind of mind and greater application robustness is often worth it.

## The Core Project

The Core project is the center of the Clean Architecture design, and all other project dependencies should point toward it. As such, it has very few external dependencies. The Core project should include the Domain Model including things like:

- Entities
- Aggregates
- Value Objects
- Domain Events
- Domain Event Handlers
- Domain Services
- Specifications
- Interfaces
- DTOs (sometimes)

You can learn more about these patterns and how to apply them here:

- [DDD Fundamentals](https://www.pluralsight.com/courses/fundamentals-domain-driven-design)
- [DDD Concepts](https://deviq.com/domain-driven-design/ddd-overview)

## The Use Cases Project

An optional project, I've included it because many folks were demanding it and it's easier to remove than to add later. This is also often referred to as the *Application* or *Application Services* layer. The Use Cases project is organized following CQRS into Commands and Queries (I considered having folders for `Commands` and `Queries` but felt it added little - the folders per actual *command* or *query* is sufficient without extra nesting). Commands mutate the domain model and thus should always use Repository abstractions for their data access (Repositories are how one fetches and persists domain model types). Queries are readonly, and thus **do not need to use the repository pattern**, but instead can use whatever query service or approach is most convenient.

Since the Use Cases project is set up to depend on Core and does not depend on Infrastructure, there will still need to be abstractions defined for its data access. And it *can* use things like specifications, which can sometimes help encapsulate query logic as well as result type mapping. But it doesn't *have* to use repository/specification - it can just issue a SQL query or call a stored procedure if that's the most efficient way to get the data.

Although this is an optional project to include (without it, your API endpoints would just work directly with the domain model or query services), it does provide a nice UI-ignorant place to add automated tests, and lends itself toward applying policies for cross-cutting concerns using a Chain of Responsibility pattern around the message handlers (for things like validation, caching, auth, logging, timing, etc.). The template includes an example of this for logging, which is located in the [SharedKernel NuGet package](https://github.com/ardalis/Ardalis.SharedKernel/blob/main/src/Ardalis.SharedKernel/LoggingBehavior.cs).

## The Infrastructure Project

Most of your application's dependencies on external resources should be implemented in classes defined in the Infrastructure project. These classes should implement interfaces defined in Core. If you have a very large project with many dependencies, it may make sense to have multiple Infrastructure projects (e.g. Infrastructure.Data), but for most projects one Infrastructure project with folders works fine. The template includes data access and domain event implementations, but you would also add things like email providers, file access, web api clients, etc. to this project so they're not adding coupling to your Core or UI projects.

## The Web Project

The entry point of the application is the ASP.NET Core web project (or possibly the AspireHost project, which in turn loads the Web project). This is actually a console application, with a `public static void Main` method in `Program.cs`. It leverages FastEndpoints and the REPR pattern to organize its API endpoints.

## The SharedKernel Package

A [Shared Kernel](https://deviq.com/domain-driven-design/shared-kernel) is used to share common elements between bounded contexts. It's a DDD term but many organizations leverage "common" projects or packages for things that are useful to share between several applications.

I recommend creating a separate SharedKernel project and solution if you will require sharing code between multiple [bounded contexts](https://ardalis.com/encapsulation-boundaries-large-and-small/) (see [DDD Fundamentals](https://www.pluralsight.com/courses/domain-driven-design-fundamentals)). I further recommend this be published as a NuGet package (most likely privately within your organization) and referenced as a NuGet dependency by those projects that require it.

Previously a project for SharedKernel was included in this project. However, for the above reasons I've made it a separate package, [Ardalis.SharedKernel](https://github.com/ardalis/Ardalis.SharedKernel), which **you should replace with your own when you use this template**.

If you want to see another [example of a SharedKernel package, the one I use in my updated Pluralsight DDD course is on NuGet here](https://www.nuget.org/packages/PluralsightDdd.SharedKernel/).

## The Test Projects

Test projects could be organized based on the kind of test (unit, functional, integration, performance, etc.) or by the project they are testing (Core, Infrastructure, Web), or both. For this simple starter kit, the test projects are organized based on the kind of test, with unit, functional and integration test projects existing in this solution. Functional tests are a special kind of [integration test](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0) that perform [subcutaneous testing](https://martinfowler.com/bliki/SubcutaneousTest.html) of the APIs of the Web project, without actually hosting a real website or going over the network. I've created a bunch of [test helpers](https://github.com/ardalis/HttpClientTestExtensions) to make these kinds of tests shorter and easier to maintain.

# Patterns Used

This solution template has code built in to support a few common patterns, especially Domain-Driven Design patterns. Here is a brief overview of how a few of them work.

## Domain Events

Domain events are a great pattern for decoupling a trigger for an operation from its implementation. This is especially useful from within domain entities since the handlers of the events can have dependencies while the entities themselves typically do not. In the sample, you can see this in action with the `ToDoItem.MarkComplete()` method. The following sequence diagram demonstrates how the event and its handler are used when an item is marked complete through a web API endpoint.

![Domain Event Sequence Diagram](https://user-images.githubusercontent.com/782127/75702680-216ce300-5c73-11ea-9187-ec656192ad3b.png)

## Machine Monitoring - Clean Architecture (.NET 10)

## ?? Overview

This project demonstrates **Clean Architecture** principles using a **Machine Monitoring** domain as a real-world example. The implementation showcases how Clean Architecture provides superior maintainability, testability, and flexibility compared to traditional layered architectures.

---

## ??? Project Structure

```
MachineMonitoringClean10/
?
??? src/
?   ??? MachineMonitoringClean10.Core/              # Enterprise Business Rules
?   ?   ??? MachineAggregate/                        # Machine domain model
?   ?   ?   ??? Machine.cs                           # ? Aggregate Root
?   ?   ?   ??? MachineId.cs                         # Value Object
?   ?   ?   ??? MachineName.cs                       # Value Object
?   ?   ?   ??? MachineStatus.cs                     # SmartEnum
?   ?   ?   ??? MachineProduction.cs                 # Value Object
?   ?   ?   ??? Events/                              # Domain Events
?   ?   ?   ??? Handlers/                            # Event Handlers
?   ?   ?   ??? Specifications/                      # Query Specifications
?   ?   ??? ContributorAggregate/                    # Example aggregate
?   ?
?   ??? MachineMonitoringClean10.UseCases/          # Application Business Rules
?   ?   ??? Machines/
?   ?       ??? Create/                              # Create Machine use case
?   ?       ??? Get/                                 # Get Machine use case
?   ?       ??? List/                                # List Machines use case
?   ?       ??? AddProduction/                       # ? Record Production use case
?   ?       ??? UpdateStatus/                        # Update Status use case
?   ?       ??? Delete/                              # Delete Machine use case
?   ?       ??? MachineDTO.cs                        # Data Transfer Object
?   ?
?   ??? MachineMonitoringClean10.Infrastructure/    # External Concerns
?   ?   ??? Data/
?   ?   ?   ??? AppDbContext.cs                      # EF Core DbContext
?   ?   ?   ??? Config/                              # EF Configurations
?   ?   ?   ?   ??? MachineConfiguration.cs          # Machine mapping
?   ?   ?   ??? Queries/                             # Optimized read queries
?   ?   ?       ??? ListMachinesQueryService.cs      # CQRS read model
?   ?   ??? Email/                                   # Email infrastructure
?   ?
?   ??? MachineMonitoringClean10.Web/               # Presentation
?       ??? Machines/                                # Machine API endpoints
?       ?   ??? Create.cs                            # POST /Machines
?       ?   ??? GetById.cs                           # GET /Machines/{id}
?       ?   ??? List.cs                              # GET /Machines
?       ?   ??? AddProduction.cs                     # ? POST /Machines/{id}/Production
?       ?   ??? UpdateStatus.cs                      # PUT /Machines/{id}/Status
?       ?   ??? Delete.cs                            # DELETE /Machines/{id}
?       ??? Program.cs                               # Application entry point
?
??? docs/
    ??? CLEAN_ARCHITECTURE_GUIDE.md                  # Complete architecture guide
    ??? MACHINE_MONITORING_USE_CASE.md               # ? Use case documentation
```

---

## ?? Key Features

### **Machine Monitoring Domain**

- ? **Create Machines** with validation
- ? **Track Machine Status** (Inactive, Active, Maintenance, Decommissioned)
- ? **Record Production** (only active machines)
- ? **Production History** with date ranges
- ? **Business Rules Enforcement** at domain level
- ? **Domain Events** for notifications and side effects

### **Clean Architecture Patterns**

- ? **Value Objects**: `MachineName`, `MachineId`, `MachineProduction`
- ? **SmartEnums**: `MachineStatus` with behavior
- ? **Domain Events**: `MachineCreatedEvent`, `MachineProductionAddedEvent`
- ? **Specifications**: `MachineByIdSpec`, `MachinesByStatusSpec`
- ? **CQRS**: Separate commands (write) and queries (read)
- ? **Repository Pattern**: Abstract data access
- ? **Mediator Pattern**: Decoupled message handling

---

## ?? Getting Started

### Prerequisites

- .NET 10 SDK
- SQL Server (or SQLite for local development)
- Visual Studio 2022 / VS Code / Rider

### Running the Application

#### Option 1: With .NET Aspire (Recommended)

```bash
# Run the Aspire orchestrator
cd src/MachineMonitoringClean10.AspireHost
dotnet run
```

Aspire will:
- Provision SQL Server container automatically
- Configure connection strings
- Provide dashboard at https://localhost:15888

#### Option 2: Standalone Web API

```bash
# Run the Web project directly (uses SQLite)
cd src/MachineMonitoringClean10.Web
dotnet run
```

API will be available at:
- https://localhost:5001
- Swagger UI: https://localhost:5001/swagger

---

## ?? API Endpoints

### **Machines**

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/Machines` | List all machines |
| `GET` | `/Machines/{id}` | Get machine by ID |
| `POST` | `/Machines` | Create new machine |
| `POST` | `/Machines/{id}/Production` | **Record production** ? |
| `PUT` | `/Machines/{id}/Status` | Update machine status |
| `DELETE` | `/Machines/{id}` | Delete machine |

### Example: Record Production

```bash
POST /Machines/1/Production
Content-Type: application/json

{
  "quantity": 100,
  "producedAt": "2024-01-15T10:30:00Z",
  "batchNumber": "BATCH-001"
}
```

**Business Rule**: Machine must be in **Active** status to record production.

---

## ?? Why This Example?

The **Machine Production Recording** use case perfectly demonstrates Clean Architecture strengths:

### 1. **Business Rules in Domain** ?

```csharp
// ? Traditional Layered: Rules in controller
if (machine.Status != "Active") 
  return BadRequest();

// ? Clean Architecture: Rules in domain
public Machine AddProduction(...)
{
  if (!Status.CanRecordProduction)
    throw new InvalidOperationException("Machine must be Active");
  // ...
}
```

### 2. **Testability** ?

```csharp
// Pure unit test - no database, no HTTP
[Fact]
public void AddProduction_WhenInactive_ThrowsException()
{
  var machine = new Machine(MachineName.From("CNC-001"));
  
  Assert.Throws<InvalidOperationException>(() =>
    machine.AddProduction(100, DateTime.UtcNow));
}
```

### 3. **Multiple Interfaces** ?

Business rules enforced consistently across:
- REST API ?
- gRPC (easy to add) ?
- CLI (easy to add) ?
- SignalR (easy to add) ?

### 4. **CQRS Optimization** ?

```csharp
// Write: Go through domain (rules enforced)
machine.AddProduction(...);

// Read: Bypass domain (optimized SQL)
dbContext.Machines.Select(m => new MachineDTO(...))
```

---

## ?? Testing

### Run All Tests

```bash
dotnet test
```

### Test Coverage

- **Unit Tests**: Pure domain logic (no dependencies)
- **Integration Tests**: Use case handlers (minimal mocking)
- **Functional Tests**: End-to-end API tests

Example unit test:

```csharp
[Fact]
public void Machine_AddProduction_WhenActive_AddsSuccessfully()
{
  // Arrange
  var machine = new Machine(MachineName.From("CNC-001"));
  machine.Activate();

  // Act
  machine.AddProduction(100, DateTime.UtcNow, "BATCH-001");

  // Assert
  Assert.Equal(100, machine.GetTotalProduction());
}
```

---

## ?? Documentation

### Essential Reading

1. **[CLEAN_ARCHITECTURE_GUIDE.md](./CLEAN_ARCHITECTURE_GUIDE.md)**
   - Complete architecture explanation
   - Clean vs. Hexagonal vs. Onion comparison
   - Code examples with detailed comments
   - When to use Clean Architecture

2. **[MACHINE_MONITORING_USE_CASE.md](./MACHINE_MONITORING_USE_CASE.md)** ?
   - Step-by-step use case walkthrough
   - Shows Clean Architecture strengths
   - Side-by-side code comparisons
   - Testing strategies

### Layer-Specific READMEs

- [Core README](./src/MachineMonitoringClean10.Core/README.md) - Domain model patterns
- [UseCases README](./src/MachineMonitoringClean10.UseCases/README.md) - Application logic
- [Infrastructure README](./src/MachineMonitoringClean10.Infrastructure/README.md) - External concerns

---

## ?? Learning Path

### Beginner

1. Read `CLEAN_ARCHITECTURE_GUIDE.md`
2. Explore `Machine` entity in Core
3. Trace a single use case (e.g., `AddProduction`)
4. Run the application and test APIs

### Intermediate

1. Study domain events and handlers
2. Understand CQRS implementation
3. Explore specifications pattern
4. Write unit tests for domain logic

### Advanced

1. Compare with Layered/Onion/Hexagonal implementations
2. Add new use cases following existing patterns
3. Implement integration tests
4. Extend with new aggregates

---

## ?? Key Concepts Demonstrated

| Concept | File/Location | Benefit |
|---------|---------------|---------|
| **Aggregate Root** | `Machine.cs` | Consistency boundary |
| **Value Objects** | `MachineName.cs`, `MachineProduction.cs` | Type safety, validation |
| **SmartEnum** | `MachineStatus.cs` | Business rules with enums |
| **Domain Events** | `MachineProductionAddedEvent.cs` | Loose coupling |
| **Specifications** | `MachineByIdSpec.cs` | Reusable queries |
| **CQRS** | `AddProductionHandler` vs `ListMachinesQueryService` | Performance optimization |
| **Repository** | `IRepository<Machine>` | Data access abstraction |
| **Use Cases** | `Machines/AddProduction/` | Application orchestration |

---

## ?? Architecture Comparison

See [CLEAN_ARCHITECTURE_GUIDE.md](./CLEAN_ARCHITECTURE_GUIDE.md#-clean-vs-hexagonal-vs-onion-whats-the-difference) for detailed comparison.

**Quick Summary**:

| Feature | Layered | Onion | Hexagonal | **Clean** |
|---------|---------|-------|-----------|-----------|
| Business Rules | ? Scattered | ?? Mixed | ? Centralized | ? **Pure Domain** |
| Use Cases Layer | ? No | ? No | ?? Implicit | ? **Explicit** |
| CQRS | ? No | ? No | ?? Manual | ? **Built-in** |
| Value Objects | ? No | ?? Optional | ?? Optional | ? **Core Pattern** |
| Domain Events | ? No | ? No | ?? Optional | ? **First-class** |

---

## ??? Technology Stack

- **.NET 10** - Latest framework features
- **C# 14** - Primary constructors, collection expressions
- **EF Core** - ORM for data access
- **FastEndpoints** - Minimal API framework
- **MediatR** - Mediator pattern implementation
- **Vogen** - Value object code generation
- **SmartEnum** - Rich enum types
- **Ardalis.Specification** - Repository pattern with specifications
- **.NET Aspire** - Orchestration and observability

---

## ?? Clean Architecture Benefits

### ? **Independence**
- Framework agnostic (swap EF Core, ASP.NET, etc.)
- Database agnostic (SQL Server, PostgreSQL, etc.)

### ? **Testability**
- Pure domain logic (no infrastructure)
- Fast unit tests (no database/HTTP)

### ? **Maintainability**
- Business rules centralized
- Clear boundaries between layers

### ? **Flexibility**
- Add new interfaces without code duplication
- Change infrastructure without touching domain

### ? **Scalability**
- CQRS enables read/write optimization
- Domain events enable async processing

---

## ?? Quick Reference

### Create a Machine

```bash
POST /Machines
{
  "name": "CNC Machine 001",
  "description": "High-precision CNC machine"
}
```

### Activate Machine

```bash
PUT /Machines/1/Status
{
  "status": "Active"
}
```

### Record Production (? Key Use Case)

```bash
POST /Machines/1/Production
{
  "quantity": 150,
  "producedAt": "2024-01-15T14:30:00Z",
  "batchNumber": "BATCH-2024-001"
}
```

### Get Machine Details

```bash
GET /Machines/1

Response:
{
  "id": 1,
  "name": "CNC Machine 001",
  "status": "Active",
  "totalProduction": 150,
  "lastProductionAt": "2024-01-15T14:30:00Z"
}
```

---

## ?? Contributing

This is a reference implementation for educational purposes. Feel free to:

1. Explore the code
2. Run the application
3. Read the documentation
4. Learn the patterns
5. Apply to your own projects

---

## ?? Support

Need help understanding Clean Architecture?

- ?? Read: [CLEAN_ARCHITECTURE_GUIDE.md](./CLEAN_ARCHITECTURE_GUIDE.md)
- ?? Study: [MACHINE_MONITORING_USE_CASE.md](./MACHINE_MONITORING_USE_CASE.md)
- ?? Reference: [Clean Architecture Template](https://github.com/ardalis/CleanArchitecture)
- ?? Contact: [NimblePros](https://nimblepros.com)

---

## ?? Additional Resources

- **Book**: *Clean Architecture* by Robert C. Martin (Uncle Bob)
- **Blog**: [Ardalis.com](https://ardalis.com)
- **Template**: [Ardalis Clean Architecture](https://github.com/ardalis/CleanArchitecture)
- **Course**: [Clean Architecture with ASP.NET Core](https://www.pluralsight.com/courses/clean-architecture-asp-net-core)

---

## ? Highlights

This implementation demonstrates:

? **Rich Domain Model** - Business logic where it belongs  
? **Value Objects** - Type-safe domain concepts  
? **Domain Events** - Loose coupling at domain level  
? **CQRS** - Optimized reads and writes  
? **Testability** - Pure unit tests without mocking  
? **Framework Independence** - Core has zero dependencies  
? **Multiple Interfaces** - Same logic, different entry points  

**The key insight**: In Clean Architecture, the business rule *"only active machines can record production"* lives in **one place** (`Machine.AddProduction()`), but is enforced **everywhere** - web API, tests, future interfaces. That's the power of Clean Architecture.

---

**Ready to explore?** Start with [MACHINE_MONITORING_USE_CASE.md](./MACHINE_MONITORING_USE_CASE.md) to see Clean Architecture in action! ??

