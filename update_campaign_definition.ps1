# PowerShell script to update campaign_definition.xml based on difficulty from reference files

$levelsDir = "Assets\Resources\Levels"
$referenceDir = "$levelsDir\reference"
$campaignFile = "$levelsDir\campaign_definition.xml"

Write-Host "Updating campaign_definition.xml based on difficulty levels..." -ForegroundColor Green

# Initialize level sets
$levelSets = @{
    "beginner" = @()
    "easy" = @()
    "intermediate" = @()
    "advanced" = @()
    "hard" = @()
    "bonus" = @()
}

# Get all current .level files (excluding backups and subdirectories)
$levelFiles = Get-ChildItem "$levelsDir\*.level" | Where-Object { $_.Directory.Name -eq "Levels" }

Write-Host "Found $($levelFiles.Count) level files to process:" -ForegroundColor Cyan
$levelFiles | ForEach-Object { Write-Host "  $($_.BaseName)" -ForegroundColor Gray }

foreach ($levelFile in $levelFiles) {
    $levelName = $levelFile.BaseName
    $infoFile = "$referenceDir\${levelName}_4m_info.xml"
    
    if (Test-Path $infoFile) {
        # Read reference info XML
        [xml]$infoXml = Get-Content $infoFile
        $levelInfo = $infoXml.BlockBallEvolution_LevelInfo
        
        $difficulty = $levelInfo.Difficulty
        $displayName = $levelInfo.LevelName
        
        # Add to appropriate level set
        if ($levelSets.ContainsKey($difficulty)) {
            $levelSets[$difficulty] += @{
                FileName = $levelName
                DisplayName = $displayName
            }
            Write-Host "  $levelName -> $difficulty ($displayName)" -ForegroundColor Green
        } else {
            Write-Warning "Unknown difficulty '$difficulty' for level $levelName, adding to 'intermediate'"
            $levelSets["intermediate"] += @{
                FileName = $levelName
                DisplayName = $displayName
            }
        }
    } else {
        Write-Warning "No reference info file found for $levelName, skipping"
    }
}

# Generate new campaign definition XML
$xmlContent = @"
<?xml version="1.0" encoding="utf-8"?>
<CampaignDefinition xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
"@

foreach ($setName in @("beginner", "easy", "intermediate", "advanced", "hard", "bonus")) {
    $displayName = (Get-Culture).TextInfo.ToTitleCase($setName)
    $xmlContent += "`n  <LevelSet Name=`"$setName`" DisplayName=`"$displayName`">"
    
    if ($levelSets[$setName].Count -gt 0) {
        foreach ($level in $levelSets[$setName]) {
            $xmlContent += "`n    <Level FileName=`"$($level.FileName)`" DisplayName=`"$($level.DisplayName)`" />"
        }
    } else {
        $xmlContent += "`n    <!-- No levels in this category -->"
    }
    
    $xmlContent += "`n  </LevelSet>"
}

$xmlContent += "`n</CampaignDefinition>"

# Backup original file
$backupFile = "$campaignFile.backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
Copy-Item $campaignFile $backupFile
Write-Host "Original file backed up to: $backupFile" -ForegroundColor Yellow

# Write new campaign definition
$xmlContent | Set-Content $campaignFile -Encoding UTF8

Write-Host "`nCampaign definition updated successfully!" -ForegroundColor Green
Write-Host "Summary by difficulty:" -ForegroundColor Cyan
foreach ($setName in @("beginner", "easy", "intermediate", "advanced", "hard", "bonus")) {
    $count = $levelSets[$setName].Count
    Write-Host "  $setName`: $count levels" -ForegroundColor White
}
