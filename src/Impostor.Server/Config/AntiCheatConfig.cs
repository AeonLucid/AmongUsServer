﻿namespace Impostor.Server.Config
{
    public class AntiCheatConfig
    {
        public const string Section = "AntiCheat";

        public bool BanIpFromGame { get; set; } = true;
        public bool DisableAntiCheat { get; set; } = false;
        
    }
}
