using Microsoft.AspNetCore.Components;

namespace Puzzle_58.Clients.Acme;

public partial class AppStateAcme : ComponentBase
{
	[Parameter]
	public RenderFragment ChildContent { get; set; }

	public string AcmeMessage { get; set; } = string.Empty;

}
