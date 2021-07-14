namespace Impostor.Api.Innersloth.Maps.Tasks
{
    public class MiraTask : ITask
    {
        internal MiraTask(Ids id, TaskTypes type, TaskCategories category, bool isVisual = false)
        {
            Id = id;
            Name = id.ToString();
            Type = type;
            Category = category;
            IsVisual = isVisual;
        }

        public enum Ids
        {
            HallwayFixWiring = 0,
            AdminEnterIDCode = 1,
            MedbaySubmitScan = 2,
            BalconyClearAsteroids = 3,
            ElectricalDivertPowerToAdmin = 4,
            ElectricalDivertPowerToCafeteria = 5,
            ElectricalDivertPowerToCommunications = 6,
            ElectricalDivertPowerToLaunchpad = 7,
            ElectricalDivertPowerToMedbay = 8,
            ElectricalDivertPowerToOffice = 9,
            StorageWaterPlants = 10,
            ReactorStartReactor = 11,
            ElectricalDivertPowerToGreenhouse = 12,
            AdminChartCourse = 13,
            GreenhouseCleanO2Filter = 14,
            LaunchpadFuelEngines = 15,
            LaboratoryAssembleArtifact = 16,
            LaboratorySortSamples = 17,
            AdminPrimeShields = 18,
            CafeteriaEmptyGarbage = 19,
            BalconyMeasureWeather = 20,
            ElectricalDivertPowerToLaboratory = 21,
            CafeteriaBuyBeverage = 22,
            OfficeProcessData = 23,
            LaunchpadRunDiagnostics = 24,
            ReactorUnlockManifolds = 25,
            CleanVent = 26,
        }

        public Ids Id { get; }

        int ITask.Id => (int)Id;

        public string Name { get; }

        public TaskTypes Type { get; }

        public TaskCategories Category { get; }

        public bool IsVisual { get; }
    }
}
