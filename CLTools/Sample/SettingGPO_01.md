
# Windows Updateの自動更新を「無効」

```powershell
$gpo_disAutoUpdate = @((New-LocalGPO -Path "Software\Policies\Microsoft\Windows\WindowsUpdate\AU" -Name "NoAutoUpdate" -Value 1 -Type REG_DWORD),
    (New-LocalGPO -Path "Software\Policies\Microsoft\Windows\WindowsUpdate\AU" -Name "**del.AUOptions" -Value " " -Type REG_SZ),
    (New-LocalGPO -Path "Software\Policies\Microsoft\Windows\WindowsUpdate\AU" -Name "**del.AutomaticMaintenanceEnabled" -Value " " -Type REG_SZ),
    (New-LocalGPO -Path "Software\Policies\Microsoft\Windows\WindowsUpdate\AU" -Name "**del.ScheduledInstallDay" -Value " " -Type REG_SZ),
    (New-LocalGPO -Path "Software\Policies\Microsoft\Windows\WindowsUpdate\AU" -Name "**del.ScheduledInstallTime" -Value " " -Type REG_SZ),
    (New-LocalGPO -Path "Software\Policies\Microsoft\Windows\WindowsUpdate\AU" -Name "**del.ScheduledInstallEveryWeek" -Value " " -Type REG_SZ),
    (New-LocalGPO -Path "Software\Policies\Microsoft\Windows\WindowsUpdate\AU" -Name "**del.ScheduledInstallFirstWeek" -Value " " -Type REG_SZ),
    (New-LocalGPO -Path "Software\Policies\Microsoft\Windows\WindowsUpdate\AU" -Name "**del.ScheduledInstallSecondWeek" -Value " " -Type REG_SZ),
    (New-LocalGPO -Path "Software\Policies\Microsoft\Windows\WindowsUpdate\AU" -Name "**del.ScheduledInstallThirdWeek" -Value " " -Type REG_SZ),
    (New-LocalGPO -Path "Software\Policies\Microsoft\Windows\WindowsUpdate\AU" -Name "**del.ScheduledInstallFourthWeek" -Value " " -Type REG_SZ),
    (New-LocalGPO -Path "Software\Policies\Microsoft\Windows\WindowsUpdate\AU" -Name "**del.AllowMUUpdateService" -Value " " -Type REG_SZ))

Set-LocalGPO -Machine -GroupPolicyObject $gpo_disAutoUpdate
```

