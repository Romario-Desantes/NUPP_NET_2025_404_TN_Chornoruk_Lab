# Test script for REST API - Lab 4
$baseUrl = "http://localhost:5000/api"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  REST API Testing - Lab 4" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 1. Create Circle
Write-Host "1. Creating Circle (POST /api/figures)" -ForegroundColor Yellow
$circle = @{
    name = "Test Circle"
    color = "chervonyi"
    type = "Circle"
    radius = 15.5
} | ConvertTo-Json

$circleResponse = Invoke-RestMethod -Uri "$baseUrl/figures" -Method Post -Body $circle -ContentType "application/json"
Write-Host "  Created: $($circleResponse.name) (ID: $($circleResponse.id))" -ForegroundColor Green
$circleId = $circleResponse.id
Write-Host ""

# 2. Create Rectangle
Write-Host "2. Creating Rectangle (POST /api/figures)" -ForegroundColor Yellow
$rectangle = @{
    name = "Test Rectangle"
    color = "synii"
    type = "Rectangle"
    width = 20.0
    height = 10.0
} | ConvertTo-Json

$rectangleResponse = Invoke-RestMethod -Uri "$baseUrl/figures" -Method Post -Body $rectangle -ContentType "application/json"
Write-Host "  Created: $($rectangleResponse.name) (ID: $($rectangleResponse.id))" -ForegroundColor Green
$rectangleId = $rectangleResponse.id
Write-Host ""

# 3. Create Square
Write-Host "3. Creating Square (POST /api/figures)" -ForegroundColor Yellow
$square = @{
    name = "Test Square"
    color = "zelenyi"
    type = "Square"
    width = 12.0
} | ConvertTo-Json

$squareResponse = Invoke-RestMethod -Uri "$baseUrl/figures" -Method Post -Body $square -ContentType "application/json"
Write-Host "  Created: $($squareResponse.name) (ID: $($squareResponse.id))" -ForegroundColor Green
Write-Host ""

# 4. Create Triangle
Write-Host "4. Creating Triangle (POST /api/figures)" -ForegroundColor Yellow
$triangle = @{
    name = "Test Triangle"
    color = "zhovtyi"
    type = "Triangle"
    sideA = 5.0
    sideB = 6.0
    sideC = 7.0
} | ConvertTo-Json

$triangleResponse = Invoke-RestMethod -Uri "$baseUrl/figures" -Method Post -Body $triangle -ContentType "application/json"
Write-Host "  Created: $($triangleResponse.name) (ID: $($triangleResponse.id))" -ForegroundColor Green
Write-Host ""

# 5. Get All Figures
Write-Host "5. Getting All Figures (GET /api/figures)" -ForegroundColor Yellow
$allFigures = Invoke-RestMethod -Uri "$baseUrl/figures" -Method Get
Write-Host "  Found figures: $($allFigures.Count)" -ForegroundColor Green
foreach ($fig in $allFigures | Select-Object -First 5) {
    $areaRounded = [math]::Round($fig.area, 2)
    Write-Host "    - $($fig.name) - $($fig.type) (Area: $areaRounded)" -ForegroundColor White
}
Write-Host ""

# 6. Get Figure by ID
Write-Host "6. Getting Figure by ID (GET /api/figures/{id})" -ForegroundColor Yellow
$figure = Invoke-RestMethod -Uri "$baseUrl/figures/$circleId" -Method Get
Write-Host "  Retrieved: $($figure.name)" -ForegroundColor Green
Write-Host "    Type: $($figure.type)" -ForegroundColor White
Write-Host "    Color: $($figure.color)" -ForegroundColor White
$areaRounded = [math]::Round($figure.area, 2)
$perimeterRounded = [math]::Round($figure.perimeter, 2)
Write-Host "    Area: $areaRounded" -ForegroundColor White
Write-Host "    Perimeter: $perimeterRounded" -ForegroundColor White
Write-Host ""

# 7. Update Figure
Write-Host "7. Updating Figure (PUT /api/figures/{id})" -ForegroundColor Yellow
$updateData = @{
    id = $rectangleId
    name = "Updated Rectangle"
    color = "fioletovyi"
} | ConvertTo-Json

$updatedFigure = Invoke-RestMethod -Uri "$baseUrl/figures/$rectangleId" -Method Put -Body $updateData -ContentType "application/json"
Write-Host "  Updated: $($updatedFigure.name)" -ForegroundColor Green
Write-Host "    New color: $($updatedFigure.color)" -ForegroundColor White
Write-Host ""

# 8. Pagination
Write-Host "8. Pagination (GET /api/figures/page)" -ForegroundColor Yellow
$pageUrl = "$baseUrl/figures/page" + "?page=1" + "&" + "pageSize=2"
$page1 = Invoke-RestMethod -Uri $pageUrl -Method Get
Write-Host "  Page 1 (2 items):" -ForegroundColor Green
foreach ($fig in $page1) {
    Write-Host "    - $($fig.name)" -ForegroundColor White
}
Write-Host ""

# 9. Create Collection
Write-Host "9. Creating Collection (POST /api/collections)" -ForegroundColor Yellow
$collection = @{
    name = "Test Collection"
    description = "Collection for API testing"
} | ConvertTo-Json

$collectionResponse = Invoke-RestMethod -Uri "$baseUrl/collections" -Method Post -Body $collection -ContentType "application/json"
Write-Host "  Created collection: $($collectionResponse.name) (ID: $($collectionResponse.id))" -ForegroundColor Green
$collectionId = $collectionResponse.id
Write-Host ""

# 10. Get All Collections
Write-Host "10. Getting All Collections (GET /api/collections)" -ForegroundColor Yellow
$allCollections = Invoke-RestMethod -Uri "$baseUrl/collections" -Method Get
Write-Host "  Found collections: $($allCollections.Count)" -ForegroundColor Green
foreach ($col in $allCollections) {
    Write-Host "    - $($col.name) - Figures: $($col.figureCount)" -ForegroundColor White
}
Write-Host ""

# 11. Update Collection
Write-Host "11. Updating Collection (PUT /api/collections/{id})" -ForegroundColor Yellow
$updateCollection = @{
    id = $collectionId
    name = "Updated Collection"
    description = "Updated description"
} | ConvertTo-Json

$updatedCollection = Invoke-RestMethod -Uri "$baseUrl/collections/$collectionId" -Method Put -Body $updateCollection -ContentType "application/json"
Write-Host "  Updated: $($updatedCollection.name)" -ForegroundColor Green
Write-Host ""

# 12. Delete Figure
Write-Host "12. Deleting Figure (DELETE /api/figures/{id})" -ForegroundColor Yellow
try {
    Invoke-RestMethod -Uri "$baseUrl/figures/$circleId" -Method Delete
    Write-Host "  Figure deleted successfully" -ForegroundColor Green
} catch {
    Write-Host "  Error deleting figure" -ForegroundColor Red
}
Write-Host ""

# 13. Verify Deletion (404)
Write-Host "13. Verifying Deletion (expecting 404)" -ForegroundColor Yellow
try {
    Invoke-RestMethod -Uri "$baseUrl/figures/$circleId" -Method Get
    Write-Host "  Figure still exists" -ForegroundColor Red
} catch {
    Write-Host "  Got 404 - figure deleted successfully" -ForegroundColor Green
}
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Testing Completed" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
