
# Windows Updateの自動更新を「無効」

```powershell
$gpo_disAutoUpdate = @(
    (New-LocalGPO -Path "Software\Policies\Microsoft\Windows\WindowsUpdate\AU" -Name "NoAutoUpdate" -Value 1 -Type REG_DWORD),
    (New-LocalGPO -Path "Software\Policies\Microsoft\Windows\WindowsUpdate\AU" -Name "**del.AUOptions" -Value " " -Type REG_SZ),
    (New-LocalGPO -Path "Software\Policies\Microsoft\Windows\WindowsUpdate\AU" -Name "**del.AutomaticMaintenanceEnabled" -Value " " -Type REG_SZ),
    (New-LocalGPO -Path "Software\Policies\Microsoft\Windows\WindowsUpdate\AU" -Name "**del.ScheduledInstallDay" -Value " " -Type REG_SZ),
    (New-LocalGPO -Path "Software\Policies\Microsoft\Windows\WindowsUpdate\AU" -Name "**del.ScheduledInstallTime" -Value " " -Type REG_SZ),
    (New-LocalGPO -Path "Software\Policies\Microsoft\Windows\WindowsUpdate\AU" -Name "**del.ScheduledInstallEveryWeek" -Value " " -Type REG_SZ),
    (New-LocalGPO -Path "Software\Policies\Microsoft\Windows\WindowsUpdate\AU" -Name "**del.ScheduledInstallFirstWeek" -Value " " -Type REG_SZ),
    (New-LocalGPO -Path "Software\Policies\Microsoft\Windows\WindowsUpdate\AU" -Name "**del.ScheduledInstallSecondWeek" -Value " " -Type REG_SZ),
    (New-LocalGPO -Path "Software\Policies\Microsoft\Windows\WindowsUpdate\AU" -Name "**del.ScheduledInstallThirdWeek" -Value " " -Type REG_SZ),
    (New-LocalGPO -Path "Software\Policies\Microsoft\Windows\WindowsUpdate\AU" -Name "**del.ScheduledInstallFourthWeek" -Value " " -Type REG_SZ),
    (New-LocalGPO -Path "Software\Policies\Microsoft\Windows\WindowsUpdate\AU" -Name "**del.AllowMUUpdateService" -Value " " -Type REG_SZ)
)

Set-LocalGPO -Machine -GroupPolicyObject $gpo_disAutoUpdate
```

# リモートデスクトップサービスユーザーに対してリモートデスクトップサービスセッションを1つに制限する 「無効」

```powershell
$gpo_disSingleSessionPerUser = @(
    (New-LocalGPO -Path "SOFTWARE\Policies\Microsoft\Windows NT\Terminal Services" -Name "fSingleSessionPerUser" -Value 0 -Type REG_DWORD)
)

Set-LocalGPO -Machine -GroupPolicyObject $gpo_disSingleSessionPerUser
```

# Windows Defender SmartScreenを構成 「無効」
「Microsort Edge」「エクスプローラー」の両方を無効

```powershell
$gpo_disSmartScreen = @(
    (New-LocalGPO -Path "Software\Policies\Microsoft\MicrosoftEdge\PhishingFilter" -Name "EnabledV9" -Value 0 -Type REG_DWORD),
    (New-LocalGPO -Path "Software\Policies\Microsoft\Windows\System" -Name "EnableSmartScreen" -Value 0 -Type REG_DWORD),
    (New-LocalGPO -Path "Software\Policies\Microsoft\Windows\System" -Name "**del.ShellSmartScreenLevel" -Value " " -Type REG_SZ)
)

Set-LocalGPO -Machine -GroupPolicyObject $gpo_disSmartScreen
```

# Windows PowerShell - スクリプトの実行を有効にする を「有効 / すべてのスクリプトを許可する」

```powershell
$gpo_enEnableScripts_machine = @(
    (New-LocalGPO -Path "Software\Policies\Microsoft\Windows\PowerShell" -Name "EnableScripts" -Value 1 -Type REG_DWORD),
    (New-LocalGPO -Path "Software\Policies\Microsoft\Windows\PowerShell" -Name "ExecutionPolicy" -Value "Unrestricted" -Type REG_SZ)
)
$gpo_enEnableScripts_user = @(
    (New-LocalGPO -Path "Software\Policies\Microsoft\Windows\PowerShell" -Name "EnableScripts" -Value 1 -Type REG_DWORD),
    (New-LocalGPO -Path "Software\Policies\Microsoft\Windows\PowerShell" -Name "ExecutionPolicy" -Value "Unrestricted" -Type REG_SZ)
)

Set-LocalGPO -Machine -GroupPolicyObject $gpo_enEnableScripts_machine
Set-LocalGPO -User -GroupPolicyObject $gpo_enEnableScripts_user
```

# 電源の管理 - スリープの設定 - スリープ時にスタンバイ状態(S1-S3)を許可する 「無効」

```powershell
$gpo_disSleepS1S3 = @(
    (New-LocalGPO -Path "Software\Policies\Microsoft\Power\PowerSettings\abfc2519-3608-4c2a-94ea-171b0ed546ab" -Name "ACSettingIndex" -Value 0 -Type REG_DWORD),
    (New-LocalGPO -Path "Software\Policies\Microsoft\Power\PowerSettings\abfc2519-3608-4c2a-94ea-171b0ed546ab" -Name "DCSettingIndex" -Value 0 -Type REG_DWORD)
)

Set-LocalGPO -Machine -GroupPolicyObject $gpo_disSleepS1S3
```

# システム - ログオン
## コンピューターの起動及びログオンで常にネットワークを待つ 「有効」
## ユーザーの簡易切り替えのエントリポイントを非表示にする 「有効」
## 初回サインインのアニメーションを表示する 「無効」

```powershell
$gpo_systemLogonPolicy = @(
    (New-LocalGPO -Path "Software\Policies\Microsoft\Windows NT\CurrentVersion\Winlogon" -Name "SyncForegroundPolicy" -Value 1 -Type REG_DWORD),
    (New-LocalGPO -Path "Software\Microsoft\Windows\CurrentVersion\Policies\System" -Name "HideFastUserSwitching" -Value 1 -Type REG_DWORD),
    (New-LocalGPO -Path "Software\Microsoft\Windows\CurrentVersion\Policies\System" -Name "EnableFirstLogonAnimation" -Value 0 -Type REG_DWORD)
)
Set-LocalGPO -Machine -GroupPolicyObject $gpo_systemLogonPolicy
```



