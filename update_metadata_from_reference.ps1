# PowerShell script to update metadata from reference files
# Updates existing .level files with metadata from reference _4m_info.xml files
# Also renames files to remove _campain suffix

$levelsDir = "Assets\Resources\Levels"
$referenceDir = "$levelsDir\reference"
$backupDir = "backup_metadata_update_$(Get-Date -Format 'yyyyMMdd_HHmmss')"

Write-Host "Starting metadata update from reference files..." -ForegroundColor Green

# Create backup
Write-Host "Creating backup in $backupDir..." -ForegroundColor Yellow
New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
Get-ChildItem "$levelsDir\*.level" | Copy-Item -Destination $backupDir

# Get all reference info files
$infoFiles = Get-ChildItem "$referenceDir\*_4m_info.xml"

foreach ($infoFile in $infoFiles) {
    # Extract level name from filename (remove _4m_info.xml)
    $levelName = $infoFile.BaseName -replace "_4m_info$", ""
    
    # Find corresponding .level file (with _campain suffix)
    $oldLevelFile = "$levelsDir\${levelName}_campain.level"
    $newLevelFile = "$levelsDir\${levelName}.level"
    
    if (Test-Path $oldLevelFile) {
        Write-Host "Processing: $levelName" -ForegroundColor Cyan
        
        # Read reference info XML
        [xml]$infoXml = Get-Content $infoFile.FullName
        $levelInfo = $infoXml.BlockBallEvolution_LevelInfo
        
        # Read existing level file
        [xml]$levelXml = Get-Content $oldLevelFile
        
        # Extract medal times (4 Medal elements with time attributes)
        $medalTimes = $levelInfo.Medal | ForEach-Object { $_.time }
        if ($medalTimes.Count -ne 4) {
            Write-Warning "Expected 4 medal times for $levelName, found $($medalTimes.Count)"
            continue
        }
        
        # Update MetaData section
        $metaData = $levelXml.BlockBallLevel.MetaData
        $metaData.LevelName = $levelInfo.LevelName
        $metaData.Creator = $levelInfo.Creator
        $metaData.TextureSet = $levelInfo.TextureSet.id
        $metaData.Skybox = $levelInfo.SkyBox
        $metaData.Music = $levelInfo.Music
        $metaData.Thumbnail = $levelInfo.Thumbnail
        
        # Update TimeTiers with medal times (1st=Mad_Genius, 2nd=Grand_Master, 3rd=Journeyman, 4th=Apprentice)
        $timeTiers = $metaData.TimeTiers
        $timeTiers.Mad_Genius_Time = $medalTimes[0]
        $timeTiers.Grand_Master_Time = $medalTimes[1] 
        $timeTiers.Journeyman_Time = $medalTimes[2]
        $timeTiers.Apprentice_Time = $medalTimes[3]
        
        # Save to new filename (without _campain)
        $levelXml.Save($newLevelFile)
        
        # Remove old file if different name
        if ($oldLevelFile -ne $newLevelFile) {
            Remove-Item $oldLevelFile
            Write-Host "  Renamed: ${levelName}_campain.level -> ${levelName}.level" -ForegroundColor Green
        }
        
        Write-Host "  Updated metadata for: $levelName" -ForegroundColor Green
        Write-Host "    LevelName: $($levelInfo.LevelName)" -ForegroundColor Gray
        Write-Host "    Creator: $($levelInfo.Creator)" -ForegroundColor Gray
        Write-Host "    Medal Times: $($medalTimes -join ', ')" -ForegroundColor Gray
    }
    else {
        Write-Warning "Level file not found: $oldLevelFile"
    }
}

Write-Host "`nMetadata update completed!" -ForegroundColor Green
Write-Host "Backup created in: $backupDir" -ForegroundColor Yellow
Write-Host "`nUpdated files:" -ForegroundColor Cyan
Get-ChildItem "$levelsDir\*.level" | Where-Object { $_.Name -notmatch "_campain" } | ForEach-Object { 
    Write-Host "  $($_.Name)" -ForegroundColor White
}
