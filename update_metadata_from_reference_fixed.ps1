# PowerShell script to update metadata from reference files (FIXED VERSION)
# Updates existing .level files with metadata from reference _4m_info.xml files
# Handles InternalName -> LevelName element rename

$levelsDir = "Assets\Resources\Levels"
$referenceDir = "$levelsDir\reference"

Write-Host "Starting metadata update from reference files (FIXED)..." -ForegroundColor Green

# Get all reference info files
$infoFiles = Get-ChildItem "$referenceDir\*_4m_info.xml"

foreach ($infoFile in $infoFiles) {
    # Extract level name from filename (remove _4m_info.xml)
    $levelName = $infoFile.BaseName -replace "_4m_info$", ""
    
    # Find corresponding .level file (should now exist without _campain suffix)
    $levelFile = "$levelsDir\${levelName}.level"
    
    if (Test-Path $levelFile) {
        Write-Host "Processing: $levelName" -ForegroundColor Cyan
        
        # Read reference info XML
        [xml]$infoXml = Get-Content $infoFile.FullName
        $levelInfo = $infoXml.BlockBallEvolution_LevelInfo
        
        # Read existing level file content as text for manual XML editing
        $levelContent = Get-Content $levelFile -Raw
        
        # Extract medal times (4 Medal elements with time attributes)
        $medalTimes = $levelInfo.Medal | ForEach-Object { $_.time }
        if ($medalTimes.Count -ne 4) {
            Write-Warning "Expected 4 medal times for $levelName, found $($medalTimes.Count)"
            continue
        }
        
        # Replace InternalName element with LevelName
        $levelContent = $levelContent -replace '<InternalName>.*?</InternalName>', "<LevelName>$($levelInfo.LevelName)</LevelName>"
        
        # Update other metadata fields
        $levelContent = $levelContent -replace '<Creator>.*?</Creator>', "<Creator>$($levelInfo.Creator)</Creator>"
        $levelContent = $levelContent -replace '<TextureSet>.*?</TextureSet>', "<TextureSet>$($levelInfo.TextureSet.id)</TextureSet>"
        $levelContent = $levelContent -replace '<Skybox>.*?</Skybox>', "<Skybox>$($levelInfo.SkyBox)</Skybox>"
        $levelContent = $levelContent -replace '<Music>.*?</Music>', "<Music>$($levelInfo.Music)</Music>"
        
        # Update TimeTiers with medal times (1st=Mad_Genius, 2nd=Grand_Master, 3rd=Journeyman, 4th=Apprentice)
        $timeTiersPattern = '<TimeTiers[^>]*\/>'
        $newTimeTiers = "<TimeTiers Mad_Genius_Time=`"$($medalTimes[0])`" Grand_Master_Time=`"$($medalTimes[1])`" Journeyman_Time=`"$($medalTimes[2])`" Apprentice_Time=`"$($medalTimes[3])`" />"
        $levelContent = $levelContent -replace $timeTiersPattern, $newTimeTiers
        
        # Save updated content
        $levelContent | Set-Content $levelFile -Encoding UTF8
        
        Write-Host "  Updated metadata for: $levelName" -ForegroundColor Green
        Write-Host "    LevelName: $($levelInfo.LevelName)" -ForegroundColor Gray
        Write-Host "    Creator: $($levelInfo.Creator)" -ForegroundColor Gray
        Write-Host "    TextureSet: $($levelInfo.TextureSet.id)" -ForegroundColor Gray
        Write-Host "    Skybox: $($levelInfo.SkyBox)" -ForegroundColor Gray
        Write-Host "    Music: $($levelInfo.Music)" -ForegroundColor Gray
        Write-Host "    Medal Times: $($medalTimes -join ', ')" -ForegroundColor Gray
    }
    else {
        Write-Warning "Level file not found: $levelFile"
    }
}

Write-Host "`nMetadata update completed!" -ForegroundColor Green

# Verify one file to show the result
$sampleFile = "$levelsDir\ancient_key.level"
if (Test-Path $sampleFile) {
    Write-Host "`nSample result (ancient_key.level MetaData section):" -ForegroundColor Yellow
    $content = Get-Content $sampleFile
    $inMetaData = $false
    foreach ($line in $content) {
        if ($line -match '<MetaData>') { $inMetaData = $true }
        if ($inMetaData) { 
            Write-Host "  $line" -ForegroundColor White
        }
        if ($line -match '</MetaData>') { 
            $inMetaData = $false
            break
        }
    }
}
