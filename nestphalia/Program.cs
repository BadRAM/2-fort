﻿using System.Diagnostics;
using System.Numerics;
using ZeroElectric.Vinculum;
using static ZeroElectric.Vinculum.Raylib;
namespace nestphalia;

public enum Scene
{
	Intro,
	MainMenu,
	Campaign,
	Editor,
	CustomBattleSetup,
	Battle,
	PostBattle
}

static class Program
{
	private static bool Paused;
	public static bool Muted;
	public static Scene CurrentScene;
	public static Campaign Campaign;
	
    public static void Main()
    {
	    SetWindowMinSize(1200, 600);
	    
	    SetWindowState(ConfigFlags.FLAG_WINDOW_RESIZABLE);
	    
        InitWindow(1200, 600, "2-fort");
        SetTargetFPS(60);
        SetExitKey(KeyboardKey.KEY_NULL);

        InitAudioDevice();      
        
        Screen.UpdateBounds();
        
        // Load a texture from the resources directory
        if (!Directory.Exists(Directory.GetCurrentDirectory() + "/forts/"))
        {
	        Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/forts/");
        }
        Resources.Load();
        Assets.Load();
        Screen.Initialize();
        GUI.Initialize();
        
        
        MenuScene.Start();
        while (!WindowShouldClose())
        {
	        Time.UpdateTime();

	        if (IsWindowResized())
	        {
		        Screen.UpdateBounds();
	        }

	        switch (CurrentScene)
	        {
		        case Scene.Intro:
			        IntroScene.Update();
			        break;
		        case Scene.MainMenu:
			        MenuScene.Update();
			        break;
		        case Scene.Campaign:
			        Campaign.Update();
			        break;
		        case Scene.Editor:
			        EditorScene.Update();
			        break;
		        case Scene.CustomBattleSetup:
			        CustomBattleMenu.Update();
			        break;		       
		        case Scene.Battle:
			        BattleScene.Update();
			        break;
		        case Scene.PostBattle:
			        PostBattleScene.Update();
			        break;
	        }


	        SetMusicVolume(Resources.MusicPlaying, MathF.Min((float)Time.Scaled, 0.5f));
	        UpdateMusicStream(Resources.MusicPlaying);
        }

        Console.WriteLine("Quitting time!");
        
	    Resources.Unload();
        
	    CloseAudioDevice();     
        //CloseWindow();
    }
}