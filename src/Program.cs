using Raylib_cs;

static class Program
{
    public static Tree selected;

    static bool IsKeyPressed(KeyboardKey key)
    {
        return Raylib.IsKeyPressed(key) || Raylib.IsKeyPressedRepeat(key);
    }

    static void Main()
    {
        Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
        Raylib.InitWindow(1000, 800, "TreeEditor");
        Raylib.MaximizeWindow();
        Tree root = new(true);
        selected = root.AddChild();

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(new Color(0.1f, 0.1f, 0.1f));

            root.Draw();

            string str = "";
            int key = Raylib.GetCharPressed();
            while(key > 0)
            {
                str += (char)key;
                key = Raylib.GetCharPressed();
            }
            if(str.Length > 0)
            {
                selected.InsertCode(str);
            }
            if (Raylib.IsKeyDown(KeyboardKey.LeftControl))
            {
            }
            else
            {
                if (IsKeyPressed(KeyboardKey.Up))
                {
                    var id = selected.Index();
                    if(id > 0)
                    {
                        selected = selected.Parent.GetChild(id - 1).LastChild();
                    }
                    else if(id == 0 && !selected.IsRoot)
                    {
                        selected = selected.Parent;
                    }
                }
                if (IsKeyPressed(KeyboardKey.Down))
                {
                    var next = selected.Next();
                    if(next != null)
                    {
                        selected = next;
                    }
                }
                if (IsKeyPressed(KeyboardKey.Enter))
                {
                    selected = selected.AddNextChild();
                }
                if (IsKeyPressed(KeyboardKey.Backspace))
                {
                    if(!selected.Backspace())
                    {
                        var oldParent = selected.Parent;
                        var newParent = oldParent.Parent;
                        if (!oldParent.IsRoot)
                        {
                            selected.SetParentAfter(newParent, oldParent);
                        }
                        else
                        {
                            var i = selected.Index();
                            if (i > 0)
                            {
                                selected.Delete();
                                selected = oldParent.GetChild(i-1).LastChild();
                            }
                        }
                    }
                }
            }
            
            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }
}