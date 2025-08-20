# PowerShell Authentication Example
$baseUrl = "http://localhost:5062"
$loginUrl = "$baseUrl/api/auth/login"

# Login credentials
$loginData = @{
    usernameOrEmail = "admin@storepos.com"
    password = "Admin123!"
} | ConvertTo-Json

# Login request
$response = Invoke-RestMethod -Uri $loginUrl -Method POST -Body $loginData -ContentType "application/json"

if ($response.success) {
    $accessToken = $response.data.accessToken
    $refreshToken = $response.data.refreshToken
    
    Write-Host "Login successful!" -ForegroundColor Green
    Write-Host "Access Token: $accessToken"
    
    # Store tokens for future requests
    $headers = @{
        "Authorization" = "Bearer $accessToken"
        "Content-Type" = "application/json"
    }
    
    # Example: Get products with authentication
    $productsUrl = "$baseUrl/api/products"
    $products = Invoke-RestMethod -Uri $productsUrl -Method GET -Headers $headers
    Write-Host "Products retrieved: $($products.Count)"
} else {
    Write-Host "Login failed: $($response.message)" -ForegroundColor Red
}
