namespace Impostor.Api.Innersloth.Maps.Tasks
{
    public class AirshipTask : ITask
    {
        internal AirshipTask(Ids id, TaskTypes type, TaskCategories category, bool isVisual = false)
        {
            Id = id;
            Name = id.ToString();
            Type = type;
            Category = category;
            IsVisual = isVisual;
        }

        public enum Ids
        {
            ElectricalFixWiring = 0,
            MeetingRoomEnterIDCode = 1,
            ElectricalCalibrateDistributor = 2,
            ElectricalResetBreakers = 3,
            VaultRoomDownloadData = 4,
            BrigDownloadData = 5,
            CargoBayDownloadData = 6,
            GapRoomDownloadData = 7,
            RecordsDownloadData = 8,
            CargoBayUnlockSafe = 9,
            VentilationStartFans = 10,
            MainHallEmptyGarbage = 11,
            MedicalEmptyGarbage = 12,
            KitchenEmptyGarbage = 13,
            MainHallDevelopPhotos = 14,
            CargoBayFuelEngines = 15,
            SecurityRewindTapes = 16,
            LoungeEmptyGarbage = 17,
            ShowersEmptyGarbage = 18,
            VaultRoomPolishRuby = 19,
            CockpitStabilizeSteering = 20,
            ArmoryDownloadData = 21,
            CockpitDownloadData = 22,
            CommsDownloadData = 23,
            MedicalDownloadData = 24,
            ViewingDeckDownloadData = 25,
            ElectricalDivertPowerToArmory = 26,
            ElectricalDivertPowerToCockpit = 27,
            ElectricalDivertPowerToGapRoom = 28,
            ElectricalDivertPowerToMainHall = 29,
            ElectricalDivertPowerToMeetingRoom = 30,
            ElectricalDivertPowerToShowers = 31,
            ElectricalDivertPowerToEngine = 32,
            ShowersPickUpTowels = 33,
            LoungeCleanToilet = 34,
            VaultRoomDressMannequin = 35,
            RecordsSortRecords = 36,
            ArmoryPutAwayPistols = 37,
            ArmoryPutAwayRifles = 38,
            MainHallDecontaminate = 39,
            KitchenMakeBurger = 40,
            ShowersFixShower = 41,
            CleanVent = 42,
        }

        public Ids Id { get; }

        int ITask.Id => (int)Id;

        public string Name { get; }

        public TaskTypes Type { get; }

        public TaskCategories Category { get; }

        public bool IsVisual { get; }
    }
}
