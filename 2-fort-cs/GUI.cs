using System.Numerics;
using ZeroElectric.Vinculum;
using static ZeroElectric.Vinculum.Raylib;

namespace _2_fort_cs;

public static class GUI
{
    private static Texture _buttonWideTexture;
    private static Texture _buttonNarrowTexture;
    public static Font Font;

    public static void Initialize()
    {
        _buttonWideTexture = Resources.GetTextureByName("button_wide");
        _buttonNarrowTexture = Resources.GetTextureByName("button_narrow");
    }

    public static void DrawTextCentered(int x, int y, string text, float size = 12, Color? color = null)
    {
        Color c = color ?? new Color(255, 255, 255, 255);
        
        Vector2 pos = new Vector2((int)(x-MeasureTextEx(Resources.Font, text, size, size/12).X/2), (int)(y-size/2));
        
        DrawTextEx(Resources.Font, text, pos, size, size/12, c);
    }

    public static void DrawTextLeft(int x, int y, string text, float size = 12, Color? color = null)
    {
        Color c = color ?? new Color(255, 255, 255, 255);
        
        DrawTextEx(Resources.Font, text, new Vector2(x,y), size, size/12, c);
    }
    
    public static bool ButtonWide(int x, int y, string text, bool enabled = true)
    {
        bool hover = CheckCollisionPointRec(GetMousePosition(), new Rectangle(x, y, 300, 40));
        bool press = !enabled || (hover && IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT));
        
        Rectangle subSprite = new Rectangle(0, !press ? !hover ? 0 : 40 : 80, 300, 40);
        DrawTextureRec(_buttonWideTexture, subSprite, new Vector2(x,y), WHITE);
        DrawTextCentered(x+150, y+20, text);
        
        return enabled && hover && IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT);
    }
    
    public static bool ButtonNarrow(int x, int y, string text, bool enabled = true)
    {
        bool hover = CheckCollisionPointRec(GetMousePosition(), new Rectangle(x, y, 100, 40));
        bool press = !enabled || (hover && IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT));
        
        Rectangle subSprite = new Rectangle(0, !press ? !hover ? 0 : 40 : 80, 100, 40);
        DrawTextureRec(_buttonNarrowTexture, subSprite, new Vector2(x,y), WHITE);
        DrawTextCentered(x+50, y+20, text);
        
        return enabled && hover && IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT);
    }
}