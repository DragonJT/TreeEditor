using Raylib_cs;
using System.Diagnostics;

class CompileAndRunC
{
    public static void Run(string ccode)
    {
        string buildDir = "build";
        string cFile = "main.c";
        string binary = "program";

        Directory.CreateDirectory(buildDir);

        File.WriteAllText(Path.Combine(buildDir, cFile), ccode);

        Run("gcc", $"{buildDir}/{cFile} -o {buildDir}/{binary}");
        Run($"./{buildDir}/{binary}", "");
    }

    static void Run(string file, string args)
    {
        var p = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = file,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            }
        };

        p.Start();
        Console.Write(p.StandardOutput.ReadToEnd());
        Console.Error.Write(p.StandardError.ReadToEnd());
        p.WaitForExit();
    }
}


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
                if (IsKeyPressed(KeyboardKey.R))
                {
                    var ccode = "#include<stdio.h>\n" + root.ToC();
                    CompileAndRunC.Run(ccode);
                }
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