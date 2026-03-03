$loginBody = @{
    email = "mostafa.business97@gmail.com"
    password = "246810"
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "http://localhost:5100/api/auth/login" -Method Post -Body $loginBody -ContentType "application/json"
$token = $response.Token

if (-not $token) {
    Write-Host "Failed to login"
    exit
}

try {
    $dashboardResponse = Invoke-RestMethod -Uri "http://localhost:5100/api/dashboard" -Method Get -Headers @{ "Authorization" = "Bearer $token" }
    Write-Host "Success!"
    $dashboardResponse | ConvertTo-Json -Depth 10
} catch {
    Write-Host "Failed!"
    Write-Host $_.Exception.Message
    $stream = $_.Exception.Response.GetResponseStream()
    $reader = New-Object System.IO.StreamReader($stream)
    $responseBody = $reader.ReadToEnd()
    Write-Host $responseBody
}
