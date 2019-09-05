﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Management.Automation;
using CLTools.Class;

namespace CLTools.Cmdlet
{
    [Cmdlet(VerbsCommon.Set, "DisplayResolution")]
    public class SetDisplayResolution : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0), Alias("X")]
        public int Width { get; set; }
        [Parameter(Mandatory = true, Position = 1), Alias("Y")]
        public int Height { get; set; }
        [Parameter(Mandatory = true)]
        public int[] DisplayNumbers { get; set; }
        [Parameter]
        public SwitchParameter Reload { get; set; }

        protected override void ProcessRecord()
        {
            string[] _DisplayNumbers = DisplayNumbers.Select(x => string.Format("{0:00}", x)).ToArray();

            MonitorRegistry mr = new MonitorRegistry();
            mr.CheckRegMonitor();
            mr.ChangeRegResolution(_DisplayNumbers, Width, Height);
            
            if(mr.IsChanged && Reload)
            {
                new ChangeStatePNPDevice("Display").Reload();
            }
        }
    }
}