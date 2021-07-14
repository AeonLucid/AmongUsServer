using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Impostor.Api.Innersloth.Maps.Tasks;
using Impostor.Api.Innersloth.Maps.Vents;

namespace Impostor.Api.Innersloth.Maps
{
    public class AirshipData : IMapData
    {
        private readonly IReadOnlyDictionary<int, IVent> _vents;
        private readonly IReadOnlyDictionary<int, ITask> _tasks;

        internal AirshipData()
        {
            var vents = new[]
            {
                new AirshipVent(this, AirshipVent.Ids.Vault, new Vector2(-12.6322f, 8.4735f), left: AirshipVent.Ids.Cockpit),
                new AirshipVent(this, AirshipVent.Ids.Cockpit, new Vector2(-22.099f, -1.512f), left: AirshipVent.Ids.Vault, right: AirshipVent.Ids.ViewingDeck),
                new AirshipVent(this, AirshipVent.Ids.ViewingDeck, new Vector2(-15.659f, -11.6991f), left: AirshipVent.Ids.Cockpit),
                new AirshipVent(this, AirshipVent.Ids.EngineRoom, new Vector2(0.203f, -2.5361f), left: AirshipVent.Ids.Kitchen, right: AirshipVent.Ids.MainHallBottom),
                new AirshipVent(this, AirshipVent.Ids.Kitchen, new Vector2(-2.6019f, -9.338f), left: AirshipVent.Ids.EngineRoom, right: AirshipVent.Ids.MainHallBottom),
                new AirshipVent(this, AirshipVent.Ids.MainHallBottom, new Vector2(7.021f, -3.730999f), left: AirshipVent.Ids.EngineRoom, right: AirshipVent.Ids.Kitchen),
                new AirshipVent(this, AirshipVent.Ids.GapRight, new Vector2(9.814f, 3.206f), left: AirshipVent.Ids.MainHallTop, right: AirshipVent.Ids.GapLeft),
                new AirshipVent(this, AirshipVent.Ids.GapLeft, new Vector2(12.663f, 5.922f), left: AirshipVent.Ids.MainHallTop, right: AirshipVent.Ids.GapRight),
                new AirshipVent(this, AirshipVent.Ids.MainHallTop, new Vector2(3.605f, 6.923f), left: AirshipVent.Ids.GapLeft, right: AirshipVent.Ids.GapRight),
                new AirshipVent(this, AirshipVent.Ids.Showers, new Vector2(23.9869f, -1.386f), left: AirshipVent.Ids.Records, right: AirshipVent.Ids.CargoBay),
                new AirshipVent(this, AirshipVent.Ids.Records, new Vector2(23.2799f, 8.259998f), left: AirshipVent.Ids.Showers, right: AirshipVent.Ids.CargoBay),
                new AirshipVent(this, AirshipVent.Ids.CargoBay, new Vector2(30.4409f, -3.577f), left: AirshipVent.Ids.Showers, right: AirshipVent.Ids.Records),
            };

            Vents = vents.ToDictionary(x => x.Id, x => x).AsReadOnly();
            _vents = vents.ToDictionary(x => (int)x.Id, x => (IVent)x).AsReadOnly();

            var tasks = new[]
            {
                new AirshipTask(AirshipTask.Ids.ElectricalFixWiring, TaskTypes.FixWiring, TaskCategories.CommonTask),
                new AirshipTask(AirshipTask.Ids.MeetingRoomEnterIDCode, TaskTypes.EnterIdCode, TaskCategories.CommonTask),
                new AirshipTask(AirshipTask.Ids.ElectricalCalibrateDistributor, TaskTypes.CalibrateDistributor, TaskCategories.LongTask),
                new AirshipTask(AirshipTask.Ids.ElectricalResetBreakers, TaskTypes.ResetBreakers, TaskCategories.LongTask),
                new AirshipTask(AirshipTask.Ids.VaultRoomDownloadData, TaskTypes.UploadData, TaskCategories.LongTask),
                new AirshipTask(AirshipTask.Ids.BrigDownloadData, TaskTypes.UploadData, TaskCategories.LongTask),
                new AirshipTask(AirshipTask.Ids.CargoBayDownloadData, TaskTypes.UploadData, TaskCategories.LongTask),
                new AirshipTask(AirshipTask.Ids.GapRoomDownloadData, TaskTypes.UploadData, TaskCategories.LongTask),
                new AirshipTask(AirshipTask.Ids.RecordsDownloadData, TaskTypes.UploadData, TaskCategories.LongTask),
                new AirshipTask(AirshipTask.Ids.CargoBayUnlockSafe, TaskTypes.UnlockSafe, TaskCategories.LongTask),
                new AirshipTask(AirshipTask.Ids.VentilationStartFans, TaskTypes.StartFans, TaskCategories.LongTask),
                new AirshipTask(AirshipTask.Ids.MainHallEmptyGarbage, TaskTypes.EmptyGarbage, TaskCategories.LongTask),
                new AirshipTask(AirshipTask.Ids.MedicalEmptyGarbage, TaskTypes.EmptyGarbage, TaskCategories.LongTask),
                new AirshipTask(AirshipTask.Ids.KitchenEmptyGarbage, TaskTypes.EmptyGarbage, TaskCategories.LongTask),
                new AirshipTask(AirshipTask.Ids.MainHallDevelopPhotos, TaskTypes.DevelopPhotos, TaskCategories.LongTask),
                new AirshipTask(AirshipTask.Ids.CargoBayFuelEngines, TaskTypes.FuelEngines, TaskCategories.LongTask),
                new AirshipTask(AirshipTask.Ids.SecurityRewindTapes, TaskTypes.RewindTapes, TaskCategories.LongTask),
                new AirshipTask(AirshipTask.Ids.LoungeEmptyGarbage, TaskTypes.EmptyGarbage, TaskCategories.LongTask),
                new AirshipTask(AirshipTask.Ids.ShowersEmptyGarbage, TaskTypes.EmptyGarbage, TaskCategories.LongTask),
                new AirshipTask(AirshipTask.Ids.VaultRoomPolishRuby, TaskTypes.PolishRuby, TaskCategories.ShortTask),
                new AirshipTask(AirshipTask.Ids.CockpitStabilizeSteering, TaskTypes.StabilizeSteering, TaskCategories.ShortTask),
                new AirshipTask(AirshipTask.Ids.ArmoryDownloadData, TaskTypes.UploadData, TaskCategories.LongTask),
                new AirshipTask(AirshipTask.Ids.CockpitDownloadData, TaskTypes.UploadData, TaskCategories.LongTask),
                new AirshipTask(AirshipTask.Ids.CommsDownloadData, TaskTypes.UploadData, TaskCategories.LongTask),
                new AirshipTask(AirshipTask.Ids.MedicalDownloadData, TaskTypes.UploadData, TaskCategories.LongTask),
                new AirshipTask(AirshipTask.Ids.ViewingDeckDownloadData, TaskTypes.UploadData, TaskCategories.LongTask),
                new AirshipTask(AirshipTask.Ids.ElectricalDivertPowerToArmory, TaskTypes.DivertPower, TaskCategories.ShortTask),
                new AirshipTask(AirshipTask.Ids.ElectricalDivertPowerToCockpit, TaskTypes.DivertPower, TaskCategories.ShortTask),
                new AirshipTask(AirshipTask.Ids.ElectricalDivertPowerToGapRoom, TaskTypes.DivertPower, TaskCategories.ShortTask),
                new AirshipTask(AirshipTask.Ids.ElectricalDivertPowerToMainHall, TaskTypes.DivertPower, TaskCategories.ShortTask),
                new AirshipTask(AirshipTask.Ids.ElectricalDivertPowerToMeetingRoom, TaskTypes.DivertPower, TaskCategories.ShortTask),
                new AirshipTask(AirshipTask.Ids.ElectricalDivertPowerToShowers, TaskTypes.DivertPower, TaskCategories.ShortTask),
                new AirshipTask(AirshipTask.Ids.ElectricalDivertPowerToEngine, TaskTypes.DivertPower, TaskCategories.ShortTask),
                new AirshipTask(AirshipTask.Ids.ShowersPickUpTowels, TaskTypes.PickUpTowels, TaskCategories.ShortTask),
                new AirshipTask(AirshipTask.Ids.LoungeCleanToilet, TaskTypes.CleanToilet, TaskCategories.ShortTask),
                new AirshipTask(AirshipTask.Ids.VaultRoomDressMannequin, TaskTypes.DressMannequin, TaskCategories.ShortTask),
                new AirshipTask(AirshipTask.Ids.RecordsSortRecords, TaskTypes.SortRecords, TaskCategories.ShortTask),
                new AirshipTask(AirshipTask.Ids.ArmoryPutAwayPistols, TaskTypes.PutAwayPistols, TaskCategories.ShortTask),
                new AirshipTask(AirshipTask.Ids.ArmoryPutAwayRifles, TaskTypes.PutAwayRifles, TaskCategories.ShortTask),
                new AirshipTask(AirshipTask.Ids.MainHallDecontaminate, TaskTypes.Decontaminate, TaskCategories.ShortTask),
                new AirshipTask(AirshipTask.Ids.KitchenMakeBurger, TaskTypes.MakeBurger, TaskCategories.ShortTask),
                new AirshipTask(AirshipTask.Ids.ShowersFixShower, TaskTypes.FixShower, TaskCategories.ShortTask),
                new AirshipTask(AirshipTask.Ids.CleanVent, TaskTypes.VentCleaning, TaskCategories.ShortTask),
            };

            Tasks = tasks.ToDictionary(x => x.Id, x => x).AsReadOnly();
            _tasks = tasks.ToDictionary(x => (int)x.Id, x => (ITask)x).AsReadOnly();
        }

        public IReadOnlyDictionary<AirshipVent.Ids, AirshipVent> Vents { get; }

        IReadOnlyDictionary<int, IVent> IMapData.Vents => _vents;

        public IReadOnlyDictionary<AirshipTask.Ids, AirshipTask> Tasks { get; }

        IReadOnlyDictionary<int, ITask> IMapData.Tasks => _tasks;
    }
}
