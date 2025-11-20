namespace HexagonalMachineMonitoring.Core.Domain.Models.Rules
{
    public static class TemperatureExceededRule
    {
        public static bool IsExceeded(
                TemperatureReading reading, double threshold)
            => reading.Value > threshold;
    }
}
