using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Impostor.Api.Innersloth.Maps.Tasks;
using Impostor.Api.Innersloth.Maps.Vents;

namespace Impostor.Api.Innersloth.Maps
{
    public class MiraData : IMapData
    {
        private readonly IReadOnlyDictionary<int, IVent> _vents;
        private readonly IReadOnlyDictionary<int, ITask> _tasks;

        internal MiraData()
        {
            var vents = new[]
            {
                new MiraVent(this, MiraVent.Ids.Balcony, new Vector2(23.77f, -1.94f), left: MiraVent.Ids.Medbay, right: MiraVent.Ids.Cafeteria),
                new MiraVent(this, MiraVent.Ids.Cafeteria, new Vector2(23.9f, 7.18f), left: MiraVent.Ids.Admin, right: MiraVent.Ids.Balcony),
                new MiraVent(this, MiraVent.Ids.Reactor, new Vector2(0.4800001f, 10.697f), left: MiraVent.Ids.Laboratory, center: MiraVent.Ids.Decontamination, right: MiraVent.Ids.Launchpad),
                new MiraVent(this, MiraVent.Ids.Laboratory, new Vector2(11.606f, 13.816f), left: MiraVent.Ids.Reactor, center: MiraVent.Ids.Decontamination, right: MiraVent.Ids.Office),
                new MiraVent(this, MiraVent.Ids.Office, new Vector2(13.28f, 20.13f), left: MiraVent.Ids.Laboratory, center: MiraVent.Ids.Admin, right: MiraVent.Ids.Greenhouse),
                new MiraVent(this, MiraVent.Ids.Admin, new Vector2(22.39f, 17.23f), left: MiraVent.Ids.Greenhouse, center: MiraVent.Ids.Cafeteria, right: MiraVent.Ids.Office),
                new MiraVent(this, MiraVent.Ids.Greenhouse, new Vector2(17.85f, 25.23f), left: MiraVent.Ids.Admin, right: MiraVent.Ids.Office),
                new MiraVent(this, MiraVent.Ids.Medbay, new Vector2(15.41f, -1.82f), left: MiraVent.Ids.Balcony, right: MiraVent.Ids.LockerRoom),
                new MiraVent(this, MiraVent.Ids.Decontamination, new Vector2(6.83f, 3.145f), left: MiraVent.Ids.Reactor, center: MiraVent.Ids.LockerRoom, right: MiraVent.Ids.Laboratory),
                new MiraVent(this, MiraVent.Ids.LockerRoom, new Vector2(4.29f, 0.5299997f), left: MiraVent.Ids.Medbay, center: MiraVent.Ids.Launchpad, right: MiraVent.Ids.Decontamination),
                new MiraVent(this, MiraVent.Ids.Launchpad, new Vector2(-6.18f, 3.56f), left: MiraVent.Ids.Reactor, right: MiraVent.Ids.LockerRoom),
            };

            Vents = vents.ToDictionary(x => x.Id, x => x).AsReadOnly();
            _vents = vents.ToDictionary(x => (int)x.Id, x => (IVent)x).AsReadOnly();

            var tasks = new[]
            {
                new MiraTask(MiraTask.Ids.HallwayFixWiring, TaskTypes.FixWiring, TaskCategories.CommonTask),
                new MiraTask(MiraTask.Ids.AdminEnterIDCode, TaskTypes.EnterIdCode, TaskCategories.CommonTask),
                new MiraTask(MiraTask.Ids.MedbaySubmitScan, TaskTypes.SubmitScan, TaskCategories.LongTask, true),
                new MiraTask(MiraTask.Ids.BalconyClearAsteroids, TaskTypes.ClearAsteroids, TaskCategories.LongTask),
                new MiraTask(MiraTask.Ids.ElectricalDivertPowerToAdmin, TaskTypes.DivertPower, TaskCategories.ShortTask),
                new MiraTask(MiraTask.Ids.ElectricalDivertPowerToCafeteria, TaskTypes.DivertPower, TaskCategories.ShortTask),
                new MiraTask(MiraTask.Ids.ElectricalDivertPowerToCommunications, TaskTypes.DivertPower, TaskCategories.ShortTask),
                new MiraTask(MiraTask.Ids.ElectricalDivertPowerToLaunchpad, TaskTypes.DivertPower, TaskCategories.ShortTask),
                new MiraTask(MiraTask.Ids.ElectricalDivertPowerToMedbay, TaskTypes.DivertPower, TaskCategories.ShortTask),
                new MiraTask(MiraTask.Ids.ElectricalDivertPowerToOffice, TaskTypes.DivertPower, TaskCategories.ShortTask),
                new MiraTask(MiraTask.Ids.StorageWaterPlants, TaskTypes.WaterPlants, TaskCategories.LongTask),
                new MiraTask(MiraTask.Ids.ReactorStartReactor, TaskTypes.StartReactor, TaskCategories.LongTask),
                new MiraTask(MiraTask.Ids.ElectricalDivertPowerToGreenhouse, TaskTypes.DivertPower, TaskCategories.ShortTask),
                new MiraTask(MiraTask.Ids.AdminChartCourse, TaskTypes.ChartCourse, TaskCategories.ShortTask),
                new MiraTask(MiraTask.Ids.GreenhouseCleanO2Filter, TaskTypes.Filter, TaskCategories.ShortTask),
                new MiraTask(MiraTask.Ids.LaunchpadFuelEngines, TaskTypes.FuelEngines, TaskCategories.ShortTask),
                new MiraTask(MiraTask.Ids.LaboratoryAssembleArtifact, TaskTypes.AssembleArtifact, TaskCategories.ShortTask),
                new MiraTask(MiraTask.Ids.LaboratorySortSamples, TaskTypes.SortSamples, TaskCategories.ShortTask),
                new MiraTask(MiraTask.Ids.AdminPrimeShields, TaskTypes.PrimeShields, TaskCategories.ShortTask),
                new MiraTask(MiraTask.Ids.CafeteriaEmptyGarbage, TaskTypes.EmptyGarbage, TaskCategories.ShortTask),
                new MiraTask(MiraTask.Ids.BalconyMeasureWeather, TaskTypes.MeasureWeather, TaskCategories.ShortTask),
                new MiraTask(MiraTask.Ids.ElectricalDivertPowerToLaboratory, TaskTypes.DivertPower, TaskCategories.ShortTask),
                new MiraTask(MiraTask.Ids.CafeteriaBuyBeverage, TaskTypes.BuyBeverage, TaskCategories.ShortTask),
                new MiraTask(MiraTask.Ids.OfficeProcessData, TaskTypes.ProcessData, TaskCategories.ShortTask),
                new MiraTask(MiraTask.Ids.LaunchpadRunDiagnostics, TaskTypes.RunDiagnostics, TaskCategories.LongTask),
                new MiraTask(MiraTask.Ids.ReactorUnlockManifolds, TaskTypes.UnlockManifolds, TaskCategories.ShortTask),
                new MiraTask(MiraTask.Ids.CleanVent, TaskTypes.VentCleaning, TaskCategories.ShortTask),
            };

            Tasks = tasks.ToDictionary(x => x.Id, x => x).AsReadOnly();
            _tasks = tasks.ToDictionary(x => (int)x.Id, x => (ITask)x).AsReadOnly();
        }

        public IReadOnlyDictionary<MiraVent.Ids, MiraVent> Vents { get; }

        IReadOnlyDictionary<int, IVent> IMapData.Vents => _vents;

        public IReadOnlyDictionary<MiraTask.Ids, MiraTask> Tasks { get; }

        IReadOnlyDictionary<int, ITask> IMapData.Tasks => _tasks;
    }
}
