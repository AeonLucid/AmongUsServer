using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Impostor.Api.Innersloth.Maps.Tasks;
using Impostor.Api.Innersloth.Maps.Vents;

namespace Impostor.Api.Innersloth.Maps
{
    public class SkeldData : IMapData
    {
        private readonly IReadOnlyDictionary<int, IVent> _vents;
        private readonly IReadOnlyDictionary<int, ITask> _tasks;

        internal SkeldData()
        {
            var vents = new[]
            {
                new SkeldVent(this, SkeldVent.Ids.Admin, new Vector2(2.544f, -9.955201f), left: SkeldVent.Ids.Cafeteria, right: SkeldVent.Ids.RightHallway),
                new SkeldVent(this, SkeldVent.Ids.RightHallway, new Vector2(9.384f, -6.438f), left: SkeldVent.Ids.Admin, right: SkeldVent.Ids.Cafeteria),
                new SkeldVent(this, SkeldVent.Ids.Cafeteria, new Vector2(4.2588f, -0.276f), left: SkeldVent.Ids.Admin, right: SkeldVent.Ids.RightHallway),
                new SkeldVent(this, SkeldVent.Ids.Electrical, new Vector2(-9.7764f, -8.034f), left: SkeldVent.Ids.Security, right: SkeldVent.Ids.Medbay),
                new SkeldVent(this, SkeldVent.Ids.UpperEngine, new Vector2(-15.288f, 2.52f), left: SkeldVent.Ids.UpperReactor),
                new SkeldVent(this, SkeldVent.Ids.Security, new Vector2(-12.534f, -6.9492f), left: SkeldVent.Ids.Medbay, right: SkeldVent.Ids.Electrical),
                new SkeldVent(this, SkeldVent.Ids.Medbay, new Vector2(-10.608f, -4.176f), left: SkeldVent.Ids.Security, right: SkeldVent.Ids.Electrical),
                new SkeldVent(this, SkeldVent.Ids.Weapons, new Vector2(8.820001f, 3.324f), right: SkeldVent.Ids.UpperNavigation),
                new SkeldVent(this, SkeldVent.Ids.LowerReactor, new Vector2(-20.796f, -6.9528f), left: SkeldVent.Ids.LowerEngine),
                new SkeldVent(this, SkeldVent.Ids.LowerEngine, new Vector2(-15.2508f, -13.656f), left: SkeldVent.Ids.LowerReactor),
                new SkeldVent(this, SkeldVent.Ids.Shields, new Vector2(9.5232f, -14.3376f), left: SkeldVent.Ids.LowerNavigation),
                new SkeldVent(this, SkeldVent.Ids.UpperReactor, new Vector2(-21.876f, -3.0516f), left: SkeldVent.Ids.UpperEngine),
                new SkeldVent(this, SkeldVent.Ids.UpperNavigation, new Vector2(16.008f, -3.168f), right: SkeldVent.Ids.Weapons),
                new SkeldVent(this, SkeldVent.Ids.LowerNavigation, new Vector2(16.008f, -6.384f), right: SkeldVent.Ids.Shields),
            };

            Vents = vents.ToDictionary(x => x.Id, x => x).AsReadOnly();
            _vents = vents.ToDictionary(x => (int)x.Id, x => (IVent)x).AsReadOnly();

            var tasks = new[]
            {
                new SkeldTask(SkeldTask.Ids.AdminSwipeCard, TaskTypes.SwipeCard, TaskCategories.CommonTask),
                new SkeldTask(SkeldTask.Ids.ElectricalFixWiring, TaskTypes.FixWiring, TaskCategories.CommonTask),
                new SkeldTask(SkeldTask.Ids.WeaponsClearAsteroids, TaskTypes.ClearAsteroids, TaskCategories.LongTask, true),
                new SkeldTask(SkeldTask.Ids.EnginesAlignEngineOutput, TaskTypes.AlignEngineOutput, TaskCategories.ShortTask),
                new SkeldTask(SkeldTask.Ids.MedbaySubmitScan, TaskTypes.SubmitScan, TaskCategories.LongTask, true),
                new SkeldTask(SkeldTask.Ids.MedbayInspectSample, TaskTypes.InspectSample, TaskCategories.LongTask),
                new SkeldTask(SkeldTask.Ids.StorageFuelEngines, TaskTypes.FuelEngines, TaskCategories.LongTask),
                new SkeldTask(SkeldTask.Ids.ReactorStartReactor, TaskTypes.StartReactor, TaskCategories.LongTask),
                new SkeldTask(SkeldTask.Ids.O2EmptyChute, TaskTypes.EmptyChute, TaskCategories.LongTask, true),
                new SkeldTask(SkeldTask.Ids.CafeteriaEmptyGarbage, TaskTypes.EmptyGarbage, TaskCategories.LongTask, true),
                new SkeldTask(SkeldTask.Ids.CommunicationsDownloadData, TaskTypes.UploadData, TaskCategories.ShortTask),
                new SkeldTask(SkeldTask.Ids.ElectricalCalibrateDistributor, TaskTypes.CalibrateDistributor, TaskCategories.ShortTask),
                new SkeldTask(SkeldTask.Ids.NavigationChartCourse, TaskTypes.ChartCourse, TaskCategories.ShortTask),
                new SkeldTask(SkeldTask.Ids.O2CleanO2Filter, TaskTypes.Filter, TaskCategories.ShortTask),
                new SkeldTask(SkeldTask.Ids.ReactorUnlockManifolds, TaskTypes.UnlockManifolds, TaskCategories.ShortTask),
                new SkeldTask(SkeldTask.Ids.ElectricalDownloadData, TaskTypes.UploadData, TaskCategories.ShortTask),
                new SkeldTask(SkeldTask.Ids.NavigationStabilizeSteering, TaskTypes.StabilizeSteering, TaskCategories.ShortTask),
                new SkeldTask(SkeldTask.Ids.WeaponsDownloadData, TaskTypes.UploadData, TaskCategories.ShortTask),
                new SkeldTask(SkeldTask.Ids.ShieldsPrimeShields, TaskTypes.PrimeShields, TaskCategories.ShortTask, true),
                new SkeldTask(SkeldTask.Ids.CafeteriaDownloadData, TaskTypes.UploadData, TaskCategories.ShortTask),
                new SkeldTask(SkeldTask.Ids.NavigationDownloadData, TaskTypes.UploadData, TaskCategories.ShortTask),
                new SkeldTask(SkeldTask.Ids.ElectricalDivertPowerToShields, TaskTypes.DivertPower, TaskCategories.ShortTask),
                new SkeldTask(SkeldTask.Ids.ElectricalDivertPowerToWeapons, TaskTypes.DivertPower, TaskCategories.ShortTask),
                new SkeldTask(SkeldTask.Ids.ElectricalDivertPowerToCommunications, TaskTypes.DivertPower, TaskCategories.ShortTask),
                new SkeldTask(SkeldTask.Ids.ElectricalDivertPowerToUpperEngine, TaskTypes.DivertPower, TaskCategories.ShortTask),
                new SkeldTask(SkeldTask.Ids.ElectricalDivertPowerToO2, TaskTypes.DivertPower, TaskCategories.ShortTask),
                new SkeldTask(SkeldTask.Ids.ElectricalDivertPowerToNavigation, TaskTypes.DivertPower, TaskCategories.ShortTask),
                new SkeldTask(SkeldTask.Ids.ElectricalDivertPowerToLowerEngine, TaskTypes.DivertPower, TaskCategories.ShortTask),
                new SkeldTask(SkeldTask.Ids.ElectricalDivertPowerToSecurity, TaskTypes.DivertPower, TaskCategories.ShortTask),
                new SkeldTask(SkeldTask.Ids.CleanVent, TaskTypes.VentCleaning, TaskCategories.ShortTask),
            };

            Tasks = tasks.ToDictionary(x => x.Id, x => x).AsReadOnly();
            _tasks = tasks.ToDictionary(x => (int)x.Id, x => (ITask)x).AsReadOnly();
        }

        public IReadOnlyDictionary<SkeldVent.Ids, SkeldVent> Vents { get; }

        IReadOnlyDictionary<int, IVent> IMapData.Vents => _vents;

        public IReadOnlyDictionary<SkeldTask.Ids, SkeldTask> Tasks { get; }

        IReadOnlyDictionary<int, ITask> IMapData.Tasks => _tasks;
    }
}
