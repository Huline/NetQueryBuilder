using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor;

namespace NetQueryBuilder.Blazor.Tests;

public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class MudTestLayout : ComponentBase
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<MudProvidersComponent>(0);
        builder.CloseComponent();
    }
}

// Composant wrapper qui inclut les providers MudBlazor
public class MudProvidersComponent : ComponentBase
{
    [Parameter] public RenderFragment ChildContent { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<MudThemeProvider>(0);
        builder.CloseComponent();

        builder.OpenComponent<MudDialogProvider>(1);
        builder.CloseComponent();

        builder.OpenComponent<MudSnackbarProvider>(2);
        builder.CloseComponent();

        builder.OpenComponent<MudPopoverProvider>(3);
        builder.CloseComponent();

        builder.AddContent(4, ChildContent);
    }
}