// App/Jobs/TimerSyncJob.cs   (Timer CSV — DUPLICATED parsing)
using Core.Application.UseCases;
using Microsoft.Extensions.Options;
using OnionMachineMonitoring.Core.Entities;

namespace App.Jobs;

public sealed class TimerSyncJob : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<TimerSyncJob> _log;
    private readonly TimerOptions _opts;
    public TimerSyncJob(IServiceProvider services, ILogger<TimerSyncJob> log, IOptions<TimerOptions> opts)
    { _services = services; _log = log; _opts = opts.Value; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_opts.Enabled) return;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var fromUtc = DateTime.SpecifyKind(DateTime.UtcNow.Date.AddDays(-1), DateTimeKind.Utc);
                var toUtc = DateTime.SpecifyKind(DateTime.UtcNow.Date.AddDays(1), DateTimeKind.Utc);
                var items = new List<MachineProduction>();

                // DUPLICATED parsing/normalization #2
                foreach (var line in File.ReadLines(_opts.CsvPath).Skip(1))
                {
                    var parts = line.Split(',', StringSplitOptions.TrimEntries);
                    if (parts.Length < 2) continue;
                    if (!DateTime.TryParse(parts[0], out var ts)) continue;
                    if (!decimal.TryParse(parts[1], out var amt)) continue;

                    var tsUtc = DateTime.SpecifyKind(ts, DateTimeKind.Utc);
                    if (tsUtc >= fromUtc && tsUtc < toUtc)
                    {
                        // items.Add(new MachineProduction(_opts.MachineId, tsUtc, amt));
                    }
                }

                using var scope = _services.CreateScope();
                var uc = scope.ServiceProvider.GetRequiredService<SyncMachineProductionForDateRange>();
                await uc.ExecuteAsync(_opts.MachineId, fromUtc, toUtc, items, stoppingToken);

                _log.LogInformation("Timer synced {Count} rows", items.Count);
            }
            catch (Exception ex) { _log.LogError(ex, "Timer sync failed"); }

            await Task.Delay(TimeSpan.FromSeconds(_opts.IntervalSeconds), stoppingToken);
        }
    }
}
public sealed class TimerOptions
{
    public bool Enabled { get; set; } = true;
    public int IntervalSeconds { get; set; } = 60;
    public string CsvPath { get; set; } = "data/incoming.csv";
    public Guid MachineId { get; set; } = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
}
