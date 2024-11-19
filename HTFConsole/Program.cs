
using Involved.HTF.Common;
using Newtonsoft.Json;
using System.Drawing;
using System.Text.Json.Nodes;
using System.Text;
using static System.Net.WebRequestMethods;
using static System.Collections.Specialized.BitVector32;
using Newtonsoft.Json.Linq;
using System.Net.Http.Json;
using Involved.HTF.Common.Dto;
using System.Linq;

HackTheFutureClient hackTheFutureClient = new HackTheFutureClient();

string token = await hackTheFutureClient.Login("spacefunk", "e8c679d4-ad47-4b60-9143-d753e4fdc634");

string url = "https://app-htf-2024.azurewebsites.net/api";
Console.WriteLine(token);


using (HttpClient client = new HttpClient())
{
    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    var responseStart = await client.GetAsync($"{url}/a/easy/start");

    {
        var response = await client.GetAsync($"{url}/a/easy/puzzle");

        if (response.IsSuccessStatusCode)
        {
            var jsonstring = await response.Content.ReadAsStringAsync();
            var convertedstring = JsonConvert.DeserializeObject<CommandResponse>(jsonstring);
            Console.WriteLine($"{convertedstring.Commands}");
            var result = CalculateDepth(convertedstring.Commands);
            Console.WriteLine(result);
            var postResponse = await client.PostAsJsonAsync($"{url}/a/easy/puzzle", result);
            if (postResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"{await postResponse.Content.ReadAsStringAsync()}");
            }
            else
            {
                Console.WriteLine($"{await postResponse.Content.ReadAsStringAsync()}");
            }
        }
        else
        {
            Console.WriteLine($"Failed to get data: {response.StatusCode}");
        }
    }


    object CalculateDepth(string convertedstring)
    {
        var depth = 0;
        var distance = 0;
        var totaldepth = 0;

        string[] commands = convertedstring.Split(',');

        foreach (var command in commands)
        {
            string[] parts = command.Trim().Split(' ');
            string direction = parts[0];
            int value = int.Parse(parts[1]);

            switch (direction)
            {
                case "Down":
                    depth += value;
                    break;
                case "Up":
                    depth -= value;
                    break;
                case "Forward":
                    distance += value;
                    totaldepth += depth * value;
                    break;
            }
        }
        

        return totaldepth * distance;
    }
}










using (HttpClient client = new HttpClient())
{
    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    var responseStart = await client.GetAsync($"{url}/a/medium/start");

    {
        var response = await client.GetAsync($"{url}/a/medium/puzzle");
        

        if (response.IsSuccessStatusCode)
        {
            string jsonstring = await response.Content.ReadAsStringAsync();
            var deserializedObjects = JsonConvert.DeserializeObject<BattleTeamsDto>(jsonstring);
            WinningTeamDto result = BattleSimulator(deserializedObjects);

            Console.WriteLine(result.remainingHealth);
            Console.WriteLine(result.winningTeam);

            var postResponse = await client.PostAsJsonAsync($"{url}/a/medium/puzzle", result);
            if (postResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"{await postResponse.Content.ReadAsStringAsync()}");
            }
            else
            {
                Console.WriteLine($"{await postResponse.Content.ReadAsStringAsync()}");
            }
        }
        else
        {
            Console.WriteLine($"Failed to get data: {response.StatusCode}");
        }
    }
}

WinningTeamDto BattleSimulator(BattleTeamsDto Teams)
{
    var teamA = Teams.TeamA;
    var teamB = Teams.TeamB;

    while(teamA.Count != 0 && teamB.Count != 0)
    {
        TeamMember firstAttacker = GetFirstAttacker(teamA[0], teamB[0]);
        TeamMember secondAttacker = firstAttacker == teamA[0] ? teamB[0] : teamA[0];

        TeamMember LostSoldier = InitiateBattle(firstAttacker, secondAttacker);

        if (teamA.Contains(LostSoldier))
        {
            teamA.Remove(LostSoldier);
        }
        else
        {
            teamB.Remove(LostSoldier);
        }
    }

    WinningTeamDto winningTeam = CheckWhoWon(teamA, teamB);
    return winningTeam;
}

WinningTeamDto CheckWhoWon(List<TeamMember> teamA, List<TeamMember> teamB)
{
    if (teamA.Any())
    {
        List<int> LeftoverHealth = teamA.Select(x => x.Health).ToList();
        WinningTeamDto winningTeam = new WinningTeamDto()
        {
            winningTeam = "TeamA",
            remainingHealth = LeftoverHealth.Sum()
        };

        return winningTeam;
    }
    else
    {
        List<int> LeftoverHealth = teamB.Select(x => x.Health).ToList();
        WinningTeamDto winningTeam = new WinningTeamDto()
        {
            winningTeam = "TeamB",
            remainingHealth = LeftoverHealth.Sum()
        };

        return winningTeam;
    }
}

TeamMember GetFirstAttacker(TeamMember teamMember1, TeamMember teamMember2)
{
    return teamMember1.Speed >= teamMember2.Speed ? teamMember1 : teamMember2;
}

TeamMember InitiateBattle(TeamMember firstAttacker, TeamMember secondAttacker)
{
    while(firstAttacker.Health > 0 && secondAttacker.Health > 0)
    {
        if(firstAttacker.Health > 0) 
        { 
        AttackOpponent(firstAttacker,secondAttacker);
        }

        if(secondAttacker.Health > 0)
        {
            AttackOpponent(secondAttacker,firstAttacker);
        }
    }

    if(firstAttacker.Health <= 0)
    {
        return firstAttacker;
    }
    else
    {
        return secondAttacker;
    }
}

void AttackOpponent(TeamMember attacker, TeamMember defender)
{
    var attackDamage = attacker.Strength;
    defender.Health -= attackDamage;
}









Dictionary<string, string> symbolToLetter = new Dictionary<string, string>
    {
        {"∆", "A"}, {"⍟", "B"}, {"◊", "C"}, {"Ψ", "D"}, {"Σ", "E"},
        {"ϕ", "F"}, {"Ω", "G"}, {"λ", "H"}, {"ζ", "I"}, {"Ϭ", "J"},
        {"ↄ", "K"}, {"◯", "L"}, {"⧖", "M"}, {"⊗", "N"}, {"⊕", "O"},
        {"∇", "P"}, {"⟁", "Q"}, {"⎍", "R"}, {"φ", "S"}, {"✦", "T"},
        {"⨅", "U"}, {"ᚦ", "V"}, {"ϡ", "W"}, {"⍾", "X"}, {"⍝", "Y"},
        {"≈", "Z"}
};

using (HttpClient client2 = new HttpClient())
{
    client2.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    var responseStart = await client2.GetAsync($"{url}/b/easy/start");

    {
        var response = await client2.GetAsync($"{url}/b/easy/puzzle");

        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            var convertedString = JsonConvert.DeserializeObject<MessageResponse>(jsonString);

            Console.WriteLine($"Received commands: {convertedString.AlienMessage}");

            // Decoding the alien message
            var result = ExtractAlienMessage(convertedString.AlienMessage);
            Console.WriteLine($"Decoded message: {result}");

            var postResponse = await client2.PostAsJsonAsync($"{url}/b/easy/puzzle", result);
            if (postResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"{await postResponse.Content.ReadAsStringAsync()}");
            }
            else
            {
                Console.WriteLine($"{await postResponse.Content.ReadAsStringAsync()}");
            }
        }

        else
        {
            Console.WriteLine($"Failed to get data: {response.StatusCode}");
        }
    }

    object ExtractAlienMessage(string convertedString)
    {
        var decodedMessage = new StringBuilder();

        if (string.IsNullOrEmpty(convertedString))
        {
            Console.WriteLine("Received an empty or null string to decode.");
            return string.Empty;
        }

        foreach (var symbol in convertedString)
        {
            string decodedChar = symbolToLetter.ContainsKey(symbol.ToString())
                ? symbolToLetter[symbol.ToString()]
                : symbol.ToString();
            decodedMessage.Append(decodedChar);
        }

        return decodedMessage.ToString();
    }
}






using (HttpClient client = new HttpClient())
{
    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    var responseStart = await client.GetAsync($"{url}/b/hard/start");

    {
        var response = await client.GetAsync($"{url}/b/hard/puzzle");

        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            var convertedString = JsonConvert.DeserializeObject<MazeResponse>(jsonString);

            var mazeResult = convertMaze(convertedString.maze);
            PrintMaze(mazeResult);

            int steps = CalculateSteps(mazeResult);
            Console.WriteLine(steps);
            var postResponse = await client.PostAsJsonAsync($"{url}/b/hard/puzzle", steps);
            if (postResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"{await postResponse.Content.ReadAsStringAsync()}");
            }
            else
            {
                Console.WriteLine($"{await postResponse.Content.ReadAsStringAsync()}");
            }
        }

        else
        {
            Console.WriteLine($"Failed to get data: {response.StatusCode}");
        }
    }

}

static void PrintMaze(string[,] maze)
{
    int rows = maze.GetLength(0);
    int cols = maze.GetLength(1);

    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            Console.Write(maze[i, j] + " ");
        }
        Console.WriteLine();
    }
}



string[,] convertMaze(string[][] maze)
{
    int rows = maze.Length;
    int cols = maze[0].Length;

    string[,] mazeArray = new string[rows, cols];

    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            mazeArray[i, j] = maze[i][j];
        }
    }

    return mazeArray;
}

 static int CalculateSteps(string[,] maze)
{
    int rows = maze.GetLength(0);
    int cols = maze.GetLength(1);

    (int, int) start = (-1, -1);
    (int, int) end = (-1, -1);

    // Find start ('S') and end ('E') positions
    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            if (maze[i, j] == "S")
            {
                start = (i, j);
            }
            else if (maze[i, j] == "E")
            {
                end = (i, j);
            }
        }
    }

    if (start == (-1, -1) || end == (-1, -1))
    {
        throw new Exception("Maze must contain both 'S' (start) and 'E' (end).");
    }

    // BFS to calculate shortest path
    return BFS(maze, start, end);
}



static int BFS(string[,] maze, (int row, int col) start, (int row, int col) end)
{
    int[] dRow = { -1, 1, 0, 0 }; // Row adjustments
    int[] dCol = { 0, 0, -1, 1 }; // Column adjustments

    int rows = maze.GetLength(0);
    int cols = maze.GetLength(1);

    bool[,] visited = new bool[rows, cols];
    Queue<(int row, int col, int steps)> queue = new Queue<(int, int, int)>();

    // Start BFS from 'S'
    queue.Enqueue((start.row, start.col, 0));
    visited[start.row, start.col] = true;

    while (queue.Count > 0)
    {
        var (currentRow, currentCol, steps) = queue.Dequeue();

        // If we reach the end ('E'), return the steps
        if ((currentRow, currentCol) == end)
        {
            return steps;
        }

        // Explore all four possible directions
        for (int i = 0; i < 4; i++)
        {
            int newRow = currentRow + dRow[i];
            int newCol = currentCol + dCol[i];

            // Check bounds and whether the cell is valid
            if (newRow >= 0 && newRow < rows &&
                newCol >= 0 && newCol < cols &&
                !visited[newRow, newCol] &&
                maze[newRow, newCol] != "#" &&
                maze[newRow, newCol] != "B")
            {
                queue.Enqueue((newRow, newCol, steps + 1));
                visited[newRow, newCol] = true;
            }
        }
    }

    // If we cannot reach 'E'
    return -1;
}


public class MazeResponse
{
    public string[][] maze { get; set; }
}


public class MessageResponse
{
    public string AlienMessage { get; set; }
}

public class CommandResponse
{
    public string Commands { get; set; }
}

