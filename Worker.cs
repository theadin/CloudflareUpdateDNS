namespace CloudflareUpdateDNS;

public class Worker : BackgroundService
{
    static readonly HttpClient client = new HttpClient();

    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _config;

    public Worker(IConfiguration config, ILogger<Worker> logger)
    {
        _logger = logger;
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var zoneId = _config.GetValue<string>("ZoneId")!;
        var token = _config.GetValue<string>("Token")!;
        var domains = _config.GetValue<string>("Domains")!.Split(',');
        var domainsIds = _config.GetValue<string>("DomainIds")!.Split(',');

        //var bubu = await Cloudflare.GetDomains(zoneId, token);

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }


            string currentIP = await GetPublicIPAddressAsync();
            Console.WriteLine($"currentIP: {currentIP}");

            if (!Cloudflare.InitialCheck)
            {
                Cloudflare.InitialCheck = true;

                await UpdateIpsForAllDomains(zoneId, token, domains, domainsIds, currentIP);

            }
            else
            {
                if (Cloudflare.IPInCloudflare != currentIP)
                    await UpdateIpsForAllDomains(zoneId, token, domains, domainsIds, currentIP);

            }
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }

    private static async Task UpdateIpsForAllDomains(string zoneId, string token, string[] domains, string[] domainsIds, string currentIP)
    {
        for (int i = 0; i < domains.Length; i++)
        {
            var domain = domains[i];
            var domainId = domainsIds[i];
            var domainDetails = await Cloudflare.GetDomainDetails(zoneId, token, domainId);
            var ipDefinedInCloudflare = domainDetails?.result.content;
            if (domainDetails != null && ipDefinedInCloudflare != currentIP)
                await Cloudflare.UpdateDomainIP(zoneId, token, domainId, domain, currentIP);
        }
    }

    public static async Task<string> GetPublicIPAddressAsync()
    {
        var httpClient = new HttpClient();
        var response = await httpClient.GetAsync("http://icanhazip.com");
        var content = await response.Content.ReadAsStringAsync();
        var ipAddress = content.Trim();
        return ipAddress;
    }
}
