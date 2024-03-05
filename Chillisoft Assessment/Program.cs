using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

class Program
{
    static void Main(string[] args)
    {
        // Get the solution directory path
        string solutionDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;
        // Combine the solution directory path with the 'files' directory
        string filesDirectory = Path.Combine(solutionDirectory, "files");

        // Construct paths for users.txt and menus.txt files
        string usersFile = Path.Combine(filesDirectory, "users.txt");
        string menusFile = Path.Combine(filesDirectory, "menus.txt");

        try
        {
            // Read user and menu data from the provided files.
            var usersData = ReadUsersFile(usersFile);
            var menusData = ReadMenusFile(menusFile);

            // Generate the output JSON structure based on the data read.
            var outputData = GenerateOutput(usersData, menusData);

            // Serialize the output data to JSON using System.Text.Json.
            var options = new JsonSerializerOptions
            {
                WriteIndented = true  // Set the option for indented formatting.
            };
            string jsonOutput = JsonSerializer.Serialize(outputData, options);
            //Console.WriteLine(jsonOutput);  // Output the serialized JSON.

            // Save the JSON output to a file named "output.json" in the 'files' directory
            string outputFilePath = Path.Combine(filesDirectory, "output.json");
            File.WriteAllText(outputFilePath, jsonOutput);
            Console.WriteLine($"Output saved to: {outputFilePath}");
        }
        catch (Exception e)
        {
            // Handle exceptions and display error message
            Console.WriteLine($"Error: {e.Message}");
        }
    }

    // Read user data from the provided file and return a list of tuples containing user names and permissions.
    static List<(string, char[])> ReadUsersFile(string usersFile)
    {
        var usersData = new List<(string, char[])>();
        foreach (string line in File.ReadAllLines(usersFile))
        {
            var parts = line.Trim().Split(' ');
            string userName = parts[0];
            char[] permissions = (parts[1] + parts[2]).ToCharArray();
            usersData.Add((userName, permissions));
        }
        return usersData;
    }

    // Read menu data from the provided file and return a dictionary containing menu IDs and names.
    static Dictionary<int, string> ReadMenusFile(string menusFile)
    {
        var menusData = new Dictionary<int, string>();
        foreach (string line in File.ReadAllLines(menusFile))
        {
            var parts = line.Trim().Split(", ");
            int menuId = int.Parse(parts[0]);
            string menuName = parts[1];
            menusData.Add(menuId, menuName);
        }
        return menusData;
    }

    // Generate the output JSON structure based on user permissions and menu data.
    static List<User> GenerateOutput(List<(string, char[])> usersData, Dictionary<int, string> menusData)
    {
        var output = new List<User>();

        foreach (var user in usersData)
        {
            string userName = user.Item1;
            char[] permissions = user.Item2;
            
            var userMenuItems = new List<string>();
            // Iterate through each permission and map it to the corresponding menu item
            for (int i = 0; i < permissions.Length; i++)
            {
                if (permissions[i] == 'Y' && menusData.ContainsKey(i + 1))
                {
                    // Add the corresponding menu item to the user's menu items
                    userMenuItems.Add(menusData[i + 1]);
                }
            }

            // Create a new User object with the userName and populated userMenuItems
            output.Add(new User { UserName = userName, MenuItems = userMenuItems });
        }

        return output;
    }
}

