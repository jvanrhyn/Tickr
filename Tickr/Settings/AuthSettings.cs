﻿namespace Tickr.Settings
{
    public record AuthSettings
    {
        public string Domain { get; set; }
        public string Audience { get; set; }
    }
}