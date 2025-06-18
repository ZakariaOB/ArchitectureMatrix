namespace MachineMonitoringWebAPI.Constants
{
    public static class ApiRoutes
    {
        public const string Root = "api";

        public const string Base = Root + "/";

        public static class Machines
        {
            public const string GetAll = Base + "machines";

            public const string Get = Base + "machine/{id}";

            public const string Delete = Base + "machine/{id}";

            public const string GetTotalProduction = Base + "machine/totalproduction";

            public const string GetForDashboard = Base + "machine/dashboard/{id}";
        }
    }
}
