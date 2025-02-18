using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using ZeroElectric.Vinculum;

namespace _2_fort_cs;

public class Campaign
{
    [JsonInclude] public double Money = 1000;
    [JsonInclude] public double Battles;
    [JsonInclude] public int Level;
    [JsonInclude] public List<FloorTileTemplate> Inventory = new List<FloorTileTemplate>();
    [JsonInclude] public Fort Fort1 = new Fort();
    [JsonInclude] public Fort Fort2 = new Fort();
    [JsonInclude] public Fort Fort3 = new Fort();
    [JsonInclude] public int SelectedFort;
    private int _selectedLevel = -1;
    private string _outcomeText = "";

    public void Save()
    {
        string jsonString = JsonSerializer.Serialize(this);
        //Console.WriteLine($"JSON campaign looks like: {jsonString}");
        File.WriteAllText(Directory.GetCurrentDirectory() + "/campaign.sav", jsonString);
    }

    public static Campaign Load()
    {
        string jsonString = File.ReadAllText(Directory.GetCurrentDirectory() + "/campaign.sav");
        return JsonSerializer.Deserialize<Campaign>(jsonString) ?? throw new NullReferenceException("Failed to deserialize campaign save file");
    }

    public void Start()
    {
        _selectedLevel = -1;
        Program.CurrentScene = Scene.Campaign;
        Screen.RegenerateBackground();
        Save();
    }
    
    public void Update()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_FOUR) && Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT)) // ye ole moneyhacks
        {
            Money += 1000;
        }
        
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Raylib.BLACK);
        Screen.DrawBackground(Raylib.DARKBROWN);
        
        Raylib.DrawTextEx(Resources.Font, _outcomeText, new Vector2(Screen.HCenter-590, Screen.VCenter+100), 12, 1, Raylib.WHITE);

        Raylib.DrawTextEx(Resources.Font, $"Bug Dollars: ${Money}", new Vector2(Screen.HCenter-590, Screen.VCenter+50), 12, 1, Raylib.WHITE);

        
        if (RayGui.GuiButton(new Rectangle(Screen.HCenter-600,   Screen.VCenter-300, 200, 40), "Edit Fort") != 0) EditorScene.Start(Fort1);
        // if (RayGui.GuiButton(new Rectangle(20,  100, 200, 40), "Edit Fort 2") != 0) EditorScene.Start(Fort2);
        // if (RayGui.GuiButton(new Rectangle(20,  150, 200, 40), "Edit Fort 3") != 0) EditorScene.Start(Fort3);
        if (Level >= 15 && GUI.ButtonWide(Screen.HCenter+300, Screen.VCenter-300, Resources.CampaignLevels[15].Name)) _selectedLevel = 15;
        if (Level >= 14 && GUI.ButtonWide(Screen.HCenter+300, Screen.VCenter-260, Resources.CampaignLevels[14].Name)) _selectedLevel = 14;
        if (Level >= 13 && GUI.ButtonWide(Screen.HCenter+300, Screen.VCenter-220, Resources.CampaignLevels[13].Name)) _selectedLevel = 13;
        if (Level >= 12 && GUI.ButtonWide(Screen.HCenter+300, Screen.VCenter-180, Resources.CampaignLevels[12].Name)) _selectedLevel = 12;
        if (Level >= 11 && GUI.ButtonWide(Screen.HCenter+300, Screen.VCenter-140, Resources.CampaignLevels[11].Name)) _selectedLevel = 11;
        if (Level >= 10 && GUI.ButtonWide(Screen.HCenter+300, Screen.VCenter-100, Resources.CampaignLevels[10].Name)) _selectedLevel = 10;
        if (Level >=  9 && GUI.ButtonWide(Screen.HCenter+300, Screen.VCenter- 60, Resources.CampaignLevels[ 9].Name)) _selectedLevel =  9;
        if (Level >=  8 && GUI.ButtonWide(Screen.HCenter+300, Screen.VCenter- 20, Resources.CampaignLevels[ 8].Name)) _selectedLevel =  8;
        if (Level >=  7 && GUI.ButtonWide(Screen.HCenter+300, Screen.VCenter+ 20, Resources.CampaignLevels[ 7].Name)) _selectedLevel =  7;
        if (Level >=  6 && GUI.ButtonWide(Screen.HCenter+300, Screen.VCenter+ 60, Resources.CampaignLevels[ 6].Name)) _selectedLevel =  6;
        if (Level >=  5 && GUI.ButtonWide(Screen.HCenter+300, Screen.VCenter+100, Resources.CampaignLevels[ 5].Name)) _selectedLevel =  5;
        if (Level >=  4 && GUI.ButtonWide(Screen.HCenter+300, Screen.VCenter+140, Resources.CampaignLevels[ 4].Name)) _selectedLevel =  4;
        if (Level >=  3 && GUI.ButtonWide(Screen.HCenter+300, Screen.VCenter+180, Resources.CampaignLevels[ 3].Name)) _selectedLevel =  3;
        if (Level >=  2 && GUI.ButtonWide(Screen.HCenter+300, Screen.VCenter+220, Resources.CampaignLevels[ 2].Name)) _selectedLevel =  2;
        if (Level >=  1 && GUI.ButtonWide(Screen.HCenter+300, Screen.VCenter+260, Resources.CampaignLevels[ 1].Name)) _selectedLevel =  1;
        if (Level >=  0 && GUI.ButtonWide(Screen.HCenter+300, Screen.VCenter+300, Resources.CampaignLevels[ 0].Name)) _selectedLevel =  0;

        if (_selectedLevel != -1)
        {
            Raylib.DrawTextEx(Resources.Font, $"{Resources.CampaignLevels[_selectedLevel].Name}\nTravel cost: {_selectedLevel * 250} \nPrize: {(_selectedLevel+1) * 1000}", new Vector2(Screen.HCenter+160, Screen.VCenter+100), 12, 1, Raylib.WHITE);
            if (Money >= _selectedLevel * 250 && RayGui.GuiButton(new Rectangle(Screen.HCenter+160, Screen.VCenter+200, 200, 40), "To Battle!") != 0)
            {
                Money -= _selectedLevel * 250;
                BattleScene.CustomBattle = false;
                BattleScene.Start(Fort1, Resources.CampaignLevels[_selectedLevel]);
            }
        }
        
        if (RayGui.GuiButton(new Rectangle(Screen.HCenter-600, Screen.VCenter+250, 280, 50), "Quit") != 0)
        {
            MenuScene.Start();
        }
        
        Raylib.EndDrawing();
    }
    
    public void ReportBattleOutcome(bool win)
    {
        Battles++;
        if (win)
        {
            _outcomeText = $"You destroyed {Resources.CampaignLevels[_selectedLevel].Name}!\n${1000 * (_selectedLevel + 1)} prize earned.";
            Money += 1000 * (_selectedLevel + 1);
            if (Level == _selectedLevel)
            {
                Level++;
                _outcomeText += $"\nNew level and structure unlocked!";
                if (Level == 8)
                {
                    _outcomeText = "The capitol has fallen, you are the new bug emperor! \nCongratulations!";
                }
            }
        }
        else
        {
            _outcomeText = "You were beaten back...";
        }
    }
}