
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Files_M3;

public static class ApprovedCustomersLoader
{
    /*

    // The .csproj file needs to include the following ItemGroup element to copy the Config folder to the output directory

    <ItemGroup>
    <!-- Include all files in the Config folder and copy them to the output directory -->
    <Content Include="Config\**\*">
        <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
    </ItemGroup>

    */

    private static readonly string ConfigFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Config", "ApprovedCustomers.json");

    public static List<ApprovedCustomer> LoadApprovedNames()
    {
        if (!File.Exists(ConfigFilePath))
        {
            throw new FileNotFoundException($"Configuration file not found: {ConfigFilePath}");
        }

        var json = File.ReadAllText(ConfigFilePath);
        var config = JsonSerializer.Deserialize<ApprovedCustomersConfig>(json);
        return config?.ApprovedNames ?? new List<ApprovedCustomer>();
    }

    private class ApprovedCustomersConfig
    {
        public List<ApprovedCustomer> ApprovedNames { get; set; } = new List<ApprovedCustomer>();
    }

    public class ApprovedCustomer
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}

