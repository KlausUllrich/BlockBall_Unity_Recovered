# Force update campaign_definition.xml by reading all reference files

$levelsDir = "Assets\Resources\Levels"
$referenceDir = "$levelsDir\reference"
$campaignFile = "$levelsDir\campaign_definition.xml"

Write-Host "Force updating campaign_definition.xml..." -ForegroundColor Green

# Read all reference info files and organize by difficulty
$levelsByDifficulty = @{}

# Get all reference info files
$infoFiles = Get-ChildItem "$referenceDir\*_4m_info.xml"

foreach ($infoFile in $infoFiles) {
    $levelName = $infoFile.BaseName -replace "_4m_info$", ""
    $levelFile = "$levelsDir\${levelName}.level"
    
    # Only process if the level file exists
    if (Test-Path $levelFile) {
        [xml]$infoXml = Get-Content $infoFile.FullName
        $levelInfo = $infoXml.BlockBallEvolution_LevelInfo
        
        $difficulty = $levelInfo.Difficulty
        $displayName = $levelInfo.LevelName
        
        # Map "genius" to "hard"
        if ($difficulty -eq "genius") { $difficulty = "hard" }
        
        if (-not $levelsByDifficulty.ContainsKey($difficulty)) {
            $levelsByDifficulty[$difficulty] = @()
        }
        
        $levelsByDifficulty[$difficulty] += @{
            FileName = $levelName
            DisplayName = $displayName
        }
        
        Write-Host "  $levelName -> $difficulty ($displayName)" -ForegroundColor Cyan
    }
}

# Create the new XML content
$xmlLines = @()
$xmlLines += '<?xml version="1.0" encoding="utf-8"?>'
$xmlLines += '<CampaignDefinition xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">'

# Process each difficulty level in order
$difficulties = @("beginner", "easy", "intermediate", "advanced", "hard", "bonus")

foreach ($difficulty in $difficulties) {
    $displayName = (Get-Culture).TextInfo.ToTitleCase($difficulty)
    $xmlLines += "  <LevelSet Name=`"$difficulty`" DisplayName=`"$displayName`">"
    
    if ($levelsByDifficulty.ContainsKey($difficulty) -and $levelsByDifficulty[$difficulty].Count -gt 0) {
        foreach ($level in $levelsByDifficulty[$difficulty]) {
            $xmlLines += "    <Level FileName=`"$($level.FileName)`" DisplayName=`"$($level.DisplayName)`" />"
        }
    } else {
        $xmlLines += "    <!-- No levels in this category -->"
    }
    
    $xmlLines += "  </LevelSet>"
}

$xmlLines += '</CampaignDefinition>'

# Try to force close any file handles and write
try {
    # Create backup first
    $backupFile = "${campaignFile}.backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
    if (Test-Path $campaignFile) {
        Copy-Item $campaignFile $backupFile -Force
        Write-Host "Backup created: $backupFile" -ForegroundColor Yellow
    }
    
    # Force write the new content
    $xmlContent = $xmlLines -join "`r`n"
    [System.IO.File]::WriteAllText($campaignFile, $xmlContent, [System.Text.Encoding]::UTF8)
    
    Write-Host "Campaign definition updated successfully!" -ForegroundColor Green
    
    # Show summary
    Write-Host "`nSummary by difficulty:" -ForegroundColor Cyan
    foreach ($difficulty in $difficulties) {
        if ($levelsByDifficulty.ContainsKey($difficulty)) {
            $count = $levelsByDifficulty[$difficulty].Count
            Write-Host "  $difficulty`: $count levels" -ForegroundColor White
        } else {
            Write-Host "  $difficulty`: 0 levels" -ForegroundColor Gray
        }
    }
    
} catch {
    Write-Error "Failed to update file: $($_.Exception.Message)"
    Write-Host "XML content that would be written:" -ForegroundColor Yellow
    $xmlLines | ForEach-Object { Write-Host $_ -ForegroundColor Gray }
}
