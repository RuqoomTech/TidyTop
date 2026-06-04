using TidyTop.Core.Models;

namespace TidyTop.Core.Services;

public static class DefaultSmartBoxFactory
{
    public static DesktopLayout CreateDefaultLayout()
    {
        return new DesktopLayout
        {
            Name = "Default",
            SmartBoxes = new List<SmartBox>
            {
                CreateOfficeBox(),
                CreateDevelopmentBox(),
                CreateWebAndCommunicationBox(),
                CreateGamesBox(),
                CreateFilesBox(),
                CreateOtherBox()
            }
        };
    }

    private static SmartBox CreateOfficeBox()
    {
        return CreateRuleBasedBox(
            "Office & Documents",
            "📊",
            "#2F855A",
            28,
            96,
            new[] { ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".pdf", ".odt", ".ods", ".odp" },
            new[] { "word", "excel", "powerpoint", "office", "acrobat", "reader", "document", "spreadsheet", "presentation" });
    }

    private static SmartBox CreateDevelopmentBox()
    {
        return CreateRuleBasedBox(
            "Development",
            "🛠️",
            "#2563EB",
            374,
            96,
            new[] { ".cs", ".js", ".jsx", ".ts", ".tsx", ".py", ".java", ".cpp", ".h", ".json", ".md", ".sln", ".csproj" },
            new[] { "visual studio", "code", "vscode", "rider", "intellij", "github", "git", "repo", "project" });
    }

    private static SmartBox CreateWebAndCommunicationBox()
    {
        return CreateRuleBasedBox(
            "Web & Communication",
            "💬",
            "#0E7490",
            720,
            96,
            new[] { ".url", ".html", ".htm" },
            new[] { "chrome", "edge", "firefox", "browser", "discord", "telegram", "whatsapp", "zoom", "teams", "slack", "outlook" });
    }

    private static SmartBox CreateGamesBox()
    {
        return CreateRuleBasedBox(
            "Games",
            "🎮",
            "#7E22CE",
            28,
            390,
            Array.Empty<string>(),
            new[] { "steam", "epic games", "game", "games", "minecraft", "riot", "blizzard", "gog", "origin", "ubisoft" });
    }

    private static SmartBox CreateFilesBox()
    {
        var box = CreateRuleBasedBox(
            "Files & Folders",
            "📁",
            "#C2410C",
            374,
            390,
            new[] { ".txt", ".rtf", ".zip", ".rar", ".7z", ".png", ".jpg", ".jpeg", ".gif", ".mp4", ".mov", ".avi" },
            new[] { "archive", "folder", "download", "file" });

        box.Rules.Add(new SmartBoxRule { Kind = SmartBoxRuleKind.ItemType, Value = nameof(DesktopItemType.Folder) });
        return box;
    }

    private static SmartBox CreateOtherBox()
    {
        return new SmartBox
        {
            Title = "Other / Unboxed",
            Emoji = "✨",
            AccentColor = "#475569",
            Behavior = SmartBoxBehavior.CatchAll,
            IsSystemBox = true,
            X = 720,
            Y = 390,
            Width = 328,
            Height = 276
        };
    }

    private static SmartBox CreateRuleBasedBox(
        string title,
        string emoji,
        string accentColor,
        int x,
        int y,
        IEnumerable<string> extensions,
        IEnumerable<string> keywords)
    {
        var rules = new List<SmartBoxRule>();
        rules.AddRange(extensions.Select(extension => new SmartBoxRule
        {
            Kind = SmartBoxRuleKind.Extension,
            Value = extension
        }));
        rules.AddRange(keywords.Select(keyword => new SmartBoxRule
        {
            Kind = SmartBoxRuleKind.NameContains,
            Value = keyword
        }));

        return new SmartBox
        {
            Title = title,
            Emoji = emoji,
            AccentColor = accentColor,
            Behavior = SmartBoxBehavior.RuleBased,
            IsSystemBox = true,
            X = x,
            Y = y,
            Width = 328,
            Height = 276,
            Rules = rules
        };
    }
}
