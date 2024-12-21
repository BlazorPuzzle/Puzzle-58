using Microsoft.AspNetCore.Components;

namespace Puzzle_58.Clients.Contoso;

public partial class AppStateContoso : ComponentBase
{
	[Parameter]
	public RenderFragment ChildContent { get; set; }

	public string ContosoMessage { get; set; } = string.Empty;

}
