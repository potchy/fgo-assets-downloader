using FgoAssetsDownloader;
using Flurl;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

IConfigurationRoot configurationRoot = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var settings = configurationRoot.Get<Settings>();

using var httpClient = new HttpClient();

string baseDownloadDirectory = args.FirstOrDefault();
if (string.IsNullOrWhiteSpace(baseDownloadDirectory))
    baseDownloadDirectory = "Download";

Directory.CreateDirectory(baseDownloadDirectory);

async Task DownloadServants()
{
    var servants = await httpClient.GetFromJsonAsync<Servant[]>(settings.ServantsUri);

    await Parallel.ForEachAsync(servants, async delegate (Servant servant, CancellationToken cancellationToken)
    {
        string servantDownloadDirectory = Path.Combine(baseDownloadDirectory, "Servants", $"{servant.CollectionNo:D3} {PathUtils.SanitizeFileName(servant.Name)}");
        string ascensionDownloadDirectory = Path.Combine(servantDownloadDirectory, "Ascensions");

        foreach (Url ascensionUrl in servant.ExtraAssets.CharaGraph.Ascension.Values)
        {
            await Download(ascensionUrl, ascensionDownloadDirectory);
        }

        if (servant.ExtraAssets.CharaGraph.Costume is not null)
        {
            string costumeDownloadDirectory = Path.Combine(servantDownloadDirectory, "Costumes");

            foreach (Url costumeUrl in servant.ExtraAssets.CharaGraph.Costume.Values)
            {
                await Download(costumeUrl, costumeDownloadDirectory);
            }
        }
    });
}

async Task DownloadCraftEssences()
{
    var craftEssences = await httpClient.GetFromJsonAsync<CraftEssence[]>(settings.CraftEssencesUri);
    string craftEssenceDownloadDirectory = Path.Combine(baseDownloadDirectory, "Craft Essences");

    await Parallel.ForEachAsync(craftEssences, async delegate (CraftEssence craftEssence, CancellationToken cancellationToken)
    {
        Url craftEssenceUrl = craftEssence.ExtraAssets.CharaGraph.Equip.Values.Single();
        string craftEssenceFileName = craftEssenceUrl.PathSegments.Last();
        string craftEssenceExtension = Path.GetExtension(craftEssenceFileName);
        craftEssenceFileName = $"{craftEssence.CollectionNo:D4} {PathUtils.SanitizeFileName(craftEssence.Name)}{craftEssenceExtension}";

        await Download(craftEssenceUrl, craftEssenceDownloadDirectory, craftEssenceFileName);
    });
}

async Task Download(Url resourceUrl, string downloadDirectory, string fileName = null)
{
    if (string.IsNullOrWhiteSpace(fileName))
        fileName = resourceUrl.PathSegments.Last();

    string filePath = Path.Combine(downloadDirectory, fileName);
    if (File.Exists(filePath))
        return;

    Console.WriteLine($"Downloading {filePath}...");
    Directory.CreateDirectory(downloadDirectory);

    string temporaryFilePath = filePath + ".tmp";
    using (Stream fileStream = File.Create(temporaryFilePath))
    {
        try
        {
            using Stream downloadStream = await httpClient.GetStreamAsync(resourceUrl);
            await downloadStream.CopyToAsync(fileStream);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    File.Move(temporaryFilePath, filePath);
}

await Task.WhenAll(DownloadServants(), DownloadCraftEssences());