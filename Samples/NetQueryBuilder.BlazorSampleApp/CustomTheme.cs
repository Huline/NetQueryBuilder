using MudBlazor;

namespace NetQueryBuilder.BlazorSampleApp;

public static class CustomTheme
{
    public static MudTheme DefaultTheme => new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#1E88E5",
            Secondary = "#FF4081",
            AppbarBackground = "#1E88E5",
            Background = "#F5F5F5",
            DrawerBackground = "#FFF",
            DrawerText = "rgba(0,0,0, 0.7)",
            Success = "#007E33"
        },
        PaletteDark = new PaletteDark
        {
            Primary = "#2196F3",
            Secondary = "#FF4081",
            Background = "#121212",
            AppbarBackground = "#1E1E1E",
            DrawerBackground = "#1E1E1E",
            Surface = "#1E1E1E",
            DrawerText = "rgba(255,255,255, 0.8)",
            Success = "#00C851"
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "6px"
        },
        Typography = new Typography
        {
            Default = new Default
            {
                FontFamily = new[] { "Roboto", "Helvetica", "Arial", "sans-serif" },
                FontSize = "0.875rem",
                FontWeight = 400,
                LineHeight = 1.43,
                LetterSpacing = ".01071em"
            },
            H1 = new H1
            {
                FontSize = "2rem",
                FontWeight = 500
            },
            H6 = new H6
            {
                FontSize = "1.125rem",
                FontWeight = 500
            }
        }
    };
}