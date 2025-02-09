using System.Text.Json.Serialization;

namespace _2_fort_cs;

public class Fort
{
    [JsonInclude] public string Name = "Fort";
    [JsonInclude] public string Comment = "It's a fort!";
    [JsonInclude] public string[] Board = new string[20 * 20];
    
    public void Load()
    {
        for (int x = 0; x < 20; x++)
        {
            for (int y = 0; y < 20; y++)
            {
                World.SetTile(Assets.GetTileByName(Board[x+y*20]), x+1,y+1);
            }
        }
    }
}