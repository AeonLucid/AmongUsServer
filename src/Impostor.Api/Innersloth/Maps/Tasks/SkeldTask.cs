namespace Impostor.Api.Innersloth.Maps.Tasks
{
    public class SkeldTask : ITask
    {
        internal SkeldTask(Ids id, TaskTypes type, TaskCategories category, bool isVisual = false)
        {
            Id = id;
            Name = id.ToString();
            Type = type;
            Category = category;
            IsVisual = isVisual;
        }

        public enum Ids
        {
            AdminSwipeCard = 0,
            ElectricalFixWiring = 1,
            WeaponsClearAsteroids = 2,
            EnginesAlignEngineOutput = 3,
            MedbaySubmitScan = 4,
            MedbayInspectSample = 5,
            StorageFuelEngines = 6,
            ReactorStartReactor = 7,
            O2EmptyChute = 8,
            CafeteriaEmptyGarbage = 9,
            CommunicationsDownloadData = 10,
            ElectricalCalibrateDistributor = 11,
            NavigationChartCourse = 12,
            O2CleanO2Filter = 13,
            ReactorUnlockManifolds = 14,
            ElectricalDownloadData = 15,
            NavigationStabilizeSteering = 16,
            WeaponsDownloadData = 17,
            ShieldsPrimeShields = 18,
            CafeteriaDownloadData = 19,
            NavigationDownloadData = 20,
            ElectricalDivertPowerToShields = 21,
            ElectricalDivertPowerToWeapons = 22,
            ElectricalDivertPowerToCommunications = 23,
            ElectricalDivertPowerToUpperEngine = 24,
            ElectricalDivertPowerToO2 = 25,
            ElectricalDivertPowerToNavigation = 26,
            ElectricalDivertPowerToLowerEngine = 27,
            ElectricalDivertPowerToSecurity = 28,
            CleanVent = 29,
        }

        public Ids Id { get; }

        int ITask.Id => (int)Id;

        public string Name { get; }

        public TaskTypes Type { get; }

        public TaskCategories Category { get; }

        public bool IsVisual { get; }
    }
}
