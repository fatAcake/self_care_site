# PowerShell скрипт для помощи в получении шрифтов ActayWide
# Этот скрипт поможет скачать и разместить шрифты

$fontsDir = "public\fonts"
$currentDir = Get-Location

# Проверяем существование папки fonts
if (-not (Test-Path $fontsDir)) {
    New-Item -ItemType Directory -Path $fontsDir -Force
    Write-Host "Создана папка $fontsDir" -ForegroundColor Green
}

Write-Host "`n=== Инструкции по получению шрифтов ActayWide ===" -ForegroundColor Cyan
Write-Host "`nШрифт ActayWide является коммерческим и требует покупки или использования бесплатной версии."
Write-Host "`nДоступные варианты:"
Write-Host "1. Бесплатная версия: https://pixelsurplus.com/products/actay-free-geometric-grotesque-fonts" -ForegroundColor Yellow
Write-Host "2. Полная версия: https://www.myfonts.com/products/actay-wide-complete-package-437897/" -ForegroundColor Yellow
Write-Host "`nПосле скачивания:"
Write-Host "- Если у вас TTF файлы, конвертируйте их в WOFF/WOFF2 используя: https://cloudconvert.com/ttf-to-woff2"
Write-Host "- Разместите файлы в папке: $fontsDir"
Write-Host "`nТребуемые файлы:"
Write-Host "  - ActayWide-Regular.woff2"
Write-Host "  - ActayWide-Regular.woff"
Write-Host "  - ActayWide-Bold.woff2"
Write-Host "  - ActayWide-Bold.woff"
Write-Host "`n=== Проверка наличия файлов ===" -ForegroundColor Cyan

$requiredFiles = @(
    "ActayWide-Regular.woff2",
    "ActayWide-Regular.woff",
    "ActayWide-Bold.woff2",
    "ActayWide-Bold.woff"
)

$allFound = $true
foreach ($file in $requiredFiles) {
    $filePath = Join-Path $fontsDir $file
    if (Test-Path $filePath) {
        Write-Host "✓ $file найден" -ForegroundColor Green
    } else {
        Write-Host "✗ $file отсутствует" -ForegroundColor Red
        $allFound = $false
    }
}

if ($allFound) {
    Write-Host "`n✓ Все файлы шрифтов на месте!" -ForegroundColor Green
} else {
    Write-Host "`n⚠ Некоторые файлы отсутствуют. Следуйте инструкциям выше." -ForegroundColor Yellow
}

