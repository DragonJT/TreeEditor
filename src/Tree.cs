using Raylib_cs;

interface ILayout
{
    void DrawSpace();
    void DrawText(string text, Color color);
    void DrawText(string text, Color color, string errorMsg);
}

class TextLayout : ILayout
{
    public string text = "";

    public void DrawSpace()
    {
        text += ' ';
    }

    public void DrawText(string text, Color color)
    {
        this.text += text;
    }

    public void DrawText(string text, Color color, string errorMsg)
    {
        this.text += text;
    }
}

class ErrorMsg(Rectangle rect, string msg)
{
    public Rectangle rect= rect;
    public string msg = msg;
}

class DrawLayout : ILayout
{
    readonly int spacing = 6;
    readonly int fontSize = 35;
    readonly int lineSize = 50;
    readonly int border = 20;
    readonly int indentSize = 60;
    readonly int spaceSize = 18;
    int x;
    int y;
    List<ErrorMsg> errorMsgs = [];

    public DrawLayout()
    {
        x = border;
        y = border;
    }

    public void NewLine()
    {
        y += lineSize;
        x = border;
    }

    public void DrawSpace()
    {
        x += spaceSize;
    }
    
    public void DrawText(string text, Color color, string errorMsg)
    {
        Raylib.DrawText(text, x, y, fontSize, color);
        var length = Raylib.MeasureText(text, fontSize) + spacing;
        errorMsgs.Add(new ErrorMsg(new Rectangle(x, y, length, fontSize), errorMsg));
        Raylib.DrawLineEx(new (x, y+fontSize), new (x+length, y+fontSize), 3, Color.Red);
        x += length;
    }

    public void DrawText(string text, Color color)
    {
        Raylib.DrawText(text, x, y, fontSize, color);
        x += Raylib.MeasureText(text, fontSize) + spacing;
    }

    public void DrawIndent(int indent, bool selected)
    {
        if (selected)
        {
            Raylib.DrawRectangle(0, y, Raylib.GetScreenWidth(), fontSize, new Color(0.2f,0.2f,0.2f));
        }
        for(var i = 0; i < indent; i++)
        {
            Raylib.DrawRectangle(x, y, 4, fontSize, new Color(0,0,0,0.3f));
            x += indentSize;
        }
        Raylib.DrawRectangle(x, y, 4, fontSize, new Color(0,0,0,0.3f));
    }

    public void DrawErrorMsgs()
    {
        var mousePos = Raylib.GetMousePosition();
        foreach(var e in errorMsgs)
        {
            if(Raylib.CheckCollisionPointRec(mousePos, e.rect))
            {
                var width = Raylib.MeasureText(e.msg, fontSize) + 10;
                var r = new Rectangle(e.rect.X + 5, e.rect.Y, width, lineSize);
                Raylib.DrawRectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height, Color.Black); 
                Raylib.DrawRectangleLinesEx(r, 2, Color.Red);
                Raylib.DrawText(e.msg, (int)r.X, (int)r.Y, fontSize, Color.White);
            }
        }
    }
}

class Tree
{
    public Tree Parent { get; private set; }
    public ILineTree LineTree {get; private set;}
    public readonly List<Tree> children = [];

    public bool IsRoot => Parent == null;

    public Tree()
    {
        LineTree = new Empty();
    }

    string GetCode()
    {
        var layout = new TextLayout();
        LineTree.Draw(layout);
        return layout.text;
    }

    void TryParse(string code)
    {
        if(Parent == null)
        {
            var tokens = new Tokens([..new Tokenizer(code).GetTokens()]);
            LineTree = new Parser(tokens).Parse();
            ConnectVariables.Connect(this);
        }
        else if(Parent.LineTree is IParser parser)
        {
            var tokens = new Tokens([..new Tokenizer(code).GetTokens()]);
            LineTree = parser.Parse(tokens);
            ConnectVariables.Connect(this);
        }
    }

    public bool Backspace()
    {
        var code = GetCode();
        if(code.Length > 0)
        {
            code = code[..^1];
            TryParse(code);
            return true;
        }
        return false;
    }

    public void InsertCode(string text)
    {
        var code = GetCode();
        code += text;
        TryParse(code);
    }

    public void Draw(DrawLayout layout, int indent)
    {
        layout.DrawIndent(indent, Program.selected == this);
        LineTree.Draw(layout);
        layout.NewLine();
        foreach(var c in children)
        {
            c.Draw(layout, indent + 1);
        }
    }

    public Tree AddChild()
    {
        var tree = new Tree();
        AddChild(tree, 0);
        return tree;
    }

    public void AddChild(Tree child, int index)
    {
        children.Insert(index, child);
        child.Parent = this;
    }

    public void SetParentAfter(Tree newParent, Tree after)
    {
        Parent.children.Remove(this);
        newParent.AddChild(this, after.Index() + 1);
    }

    public int Index()
    {
        if(Parent == null)
        {
            return 0;
        }
        return Parent.children.IndexOf(this);
    }

    public Tree GetChild(int index)
    {
        return children[index];
    }

    public void Delete()
    {
        if (Parent != null)
        {
            Parent.children.Remove(this);
        }
    }

    public Tree LastChild()
    {
        if(children.Count > 0)
        {
            return children[^1].LastChild();
        }
        return this;
    }

    public Tree Next(bool goIntoChildren = true)
    {
        if(goIntoChildren && children.Count > 0)
        {
            return children[0];
        }
        else if(Parent != null)
        {
            var id = Index() + 1;
            if(id < Parent.children.Count)
            {
                return Parent.children[id];
            }
            else
            {
                return Parent.Next(false);
            }
        }
        else
        {
            return null;
        }
    }

    public Tree AddNextChild()
    {
        var tree = new Tree();
        if(LineTree is IParser)
        {
            AddChild(tree, 0);
            return tree;
        }
        else
        {
            Parent.AddChild(tree, Index()+1);
            return tree;
        }
    }
}