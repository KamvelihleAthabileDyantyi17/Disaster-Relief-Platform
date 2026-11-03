$url = "https://localhost:7185/" # Update with your local app URL if different (check your launchSettings.json)
$concurrentUsers = 5
$requestsPerUser = 10

Write-Host "Starting Load Test against $url with $concurrentUsers concurrent users..." -ForegroundColor Cyan

$jobs = 1..$concurrentUsers | ForEach-Object {
    Start-Job -ScriptBlock {
        param($targetUrl, $count)
        $sw = [System.Diagnostics.Stopwatch]::StartNew()
        $successCount = 0

        for ($i = 0; $i -lt $count; $i++) {
            try {
                $response = Invoke-WebRequest -Uri $targetUrl -UseBasicParsing -TimeoutSec 5
                if ($response.StatusCode -eq 200) {
                    $successCount++
                }
            }
            catch {
                # Log or ignore individual hiccups during load
            }
        }
        $sw.Stop()
        [PSCustomObject]@{
            UserThread = $id
            SuccessfulRequests = $successCount

            DurationMs = $sw.ElapsedMilliseconds
        }
    } -ArgumentList $_, $requestsPerUser
}

# Wait for all jobs to complete and gather results
$results = $jobs | Wait-Job | Receive-Job
$jobs | Remove-Job

Write-Host "Load Test Complete!" -ForegroundColor Green
$results | Format-Table -AutoSize