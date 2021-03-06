﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using System.IO;
using CLTools.Serialize;

namespace CLTools.Cmdlet.GPO
{
    [Cmdlet(VerbsData.Import, "LocalGPO")]
    public class ImportLocalGPO : PSCmdlet
    {
        [Parameter]
        public SwitchParameter IgnoreMachine { get; set; }
        [Parameter]
        public SwitchParameter IgnoreUser { get; set; }
        [Parameter(Position = 0), Alias("Path")]
        public string ImportFilePath { get; set; }
        [Parameter]
        public string TargetPolFile { get; set; }

        private string _currentDirectory = null;

        protected override void BeginProcessing()
        {
            //  カレントディレクトリカレントディレクトリの一時変更
            _currentDirectory = System.Environment.CurrentDirectory;
            System.Environment.CurrentDirectory = this.SessionState.Path.CurrentFileSystemLocation.Path;
        }

        protected override void ProcessRecord()
        {
            Functions.CheckAdmin();

            if (!string.IsNullOrEmpty(ImportFilePath))
            {
                Class.GPO.GroupPolicy groupPolicy = DataSerializer.Deserialize<Class.GPO.GroupPolicy>(ImportFilePath);

                if (!string.IsNullOrEmpty(TargetPolFile))
                {
                    if (groupPolicy.Machine != null && groupPolicy.Machine.Count > 0)
                    {
                        Class.GPO.PolFile pol = new Class.GPO.PolFile();
                        foreach (Class.GPO.GroupPolicyObject gpo in groupPolicy.Machine)
                        {
                            pol.Entries[gpo.Path + "\\" + gpo.Name] = gpo.ConvertToPolEntry();
                        }
                        if (!Directory.Exists(Path.GetDirectoryName(TargetPolFile)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(TargetPolFile));
                        }
                        pol.Save(TargetPolFile);
                    }
                }
                else
                {
                    if (!IgnoreMachine && groupPolicy.Machine != null && groupPolicy.Machine.Count > 0)
                    {
                        Class.GPO.PolFile pol = new Class.GPO.PolFile();
                        foreach (Class.GPO.GroupPolicyObject gpo in groupPolicy.Machine)
                        {
                            pol.Entries[gpo.Path + "\\" + gpo.Name] = gpo.ConvertToPolEntry();
                        }
                        if (!Directory.Exists(Path.GetDirectoryName(Class.GPO.Item.MACHINE_POL_PATH)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(Class.GPO.Item.MACHINE_POL_PATH));
                        }
                        pol.Save(Class.GPO.Item.MACHINE_POL_PATH);
                    }
                    if (!IgnoreUser && groupPolicy.User != null && groupPolicy.User.Count > 0)
                    {
                        Class.GPO.PolFile pol = new Class.GPO.PolFile();
                        foreach (Class.GPO.GroupPolicyObject gpo in groupPolicy.User)
                        {
                            pol.Entries[gpo.Path + "\\" + gpo.Name] = gpo.ConvertToPolEntry();
                        }
                        if (!Directory.Exists(Path.GetDirectoryName(Class.GPO.Item.USER_POL_PATH)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(Class.GPO.Item.USER_POL_PATH));
                        }
                        pol.Save(Class.GPO.Item.USER_POL_PATH);
                    }
                }
            }
        }

        protected override void EndProcessing()
        {
            //  カレントディレクトリを戻す
            System.Environment.CurrentDirectory = _currentDirectory;
        }
    }
}
