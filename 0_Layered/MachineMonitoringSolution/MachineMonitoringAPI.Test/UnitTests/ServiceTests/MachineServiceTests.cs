using MachineMonitoring.Repository.DataContext;
using MachineMonitoring.Repository.Repositories;
using MachineMonitoring.Repository.Repositories.ApiMachine;
using MachineMonitoringRepository.Models;
using MachineMonitoringService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace MachineMonitoring.Tests.UnitTests.ServiceTests;

/// <summary>
/// DEMONSTRATES: How Easy It Is To Make Mistakes in Layered Architecture
/// 
/// EDUCATIONAL GOAL:
/// These tests prove that layered architecture makes it EASY for developers to
/// accidentally write infrastructure-coupled code without realizing it.
/// 
/// THE TRAP:
/// 1. Repository returns IQueryable<Machine> (explicitly leaking infrastructure)
/// 2. Developer innocently writes: .Where(m => EF.Functions.Like(...))
/// 3. Code compiles ✓ and works ✓ with EF repository
/// 4. Developer thinks: "I'm using abstractions properly!"
/// 5. Reality: Service is now MARRIED to Entity Framework
/// 6. Switching to API repository → BREAKS with InvalidOperationException
/// 
/// WHY THIS JUSTIFIES BETTER ARCHITECTURE:
/// In Onion/Hexagonal architecture, this mistake is IMPOSSIBLE because:
/// - Repository must return materialized data (Task<IEnumerable<T>>)
/// - Infrastructure details cannot leak through the boundary
/// - Using EF.Functions in service = compile error
/// </summary>
public class MachineServiceTests
{
    private static MachineMonitoringContext CreateInMemoryContext()
    {
        // SQLite In-Memory supports EF.Functions.Like() (real SQL database)
        // Unlike EF In-Memory provider which is just a collection
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<MachineMonitoringContext>()
            .UseSqlite(connection)
            .Options;

        var context = new MachineMonitoringContext(options);
        context.Database.EnsureCreated(); // Create schema
        
        context.Machines.AddRange(
            new Machine { Name = "Cutter" },
            new Machine { Name = "Printer" },
            new Machine { Name = "Folder" }
        );
        context.SaveChanges();

        return context;
    }

    /// <summary>
    /// TEST 1: The Successful Case (That Hides The Problem)
    /// 
    /// WHAT HAPPENS:
    /// - Repository returns IQueryable<Machine> from DbContext
    /// - Service calls .Where(m => EF.Functions.Like(...))
    /// - LINQ detects IQueryable → uses EF query provider
    /// - EF.Functions.Like() is translated to SQL: WHERE Name LIKE 'C%'
    /// - Query executes on database
    /// - Test PASSES ✓
    /// 
    /// THE ILLUSION:
    /// Developer believes they wrote proper abstracted code because:
    /// ✓ Using interface (IMachineRepository)
    /// ✓ Test passes
    /// ✓ Code review passes
    /// 
    /// THE REALITY:
    /// Service is TIGHTLY COUPLED to Entity Framework
    /// EF.Functions.Like() ONLY works with EF's IQueryable provider
    /// The interface explicitly exposes infrastructure
    /// 
    /// THE PROBLEM:
    /// In layered architecture, this mistake is EASY and UNDETECTABLE
    /// until you try to swap implementations (see Test 2)
    /// </summary>
    [Fact]
    public void EfRepository_Should_Work_With_IQueryable()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repo = new MachineRepository(context);
        var service = new MachineService(repo, null, null);

        // Act
        var result = service.GetMachineNamesStartingWith('C');

        // Assert
        Assert.Contains("Cutter", result);
    }

    /// </summary>
    [Fact]
    public void ApiRepository_Should_Fail_Because_It_Cannot_Handle_EF_Functions()
    {
        // Arrange
        var context = CreateInMemoryContext();
        var repo = new ApiMachineRepository();
        var service = new MachineService(repo, null, null);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = service.GetMachineNamesStartingWith('C').ToList();
        });
    }
}
