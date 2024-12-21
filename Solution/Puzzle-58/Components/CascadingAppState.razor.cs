using Microsoft.AspNetCore.Components;

namespace Puzzle_58.Components;

// Add properties here that you want to share across all clients
public partial class CascadingAppState : ComponentBase
{
	private readonly IServiceProvider _serviceProvider;
	public CascadingAppState(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	[Parameter]
	public RenderFragment ChildContent { get; set; }

	public object ClientState { get; private set; }

	private Client client = Client.None;
	public Client CurrentClient
	{
		get => client;
		set
		{
			if (client != value)
			{
				client = value;
				// Instantiate the AppState for the selected client
				string typeName = $"Puzzle_58.Clients.{client}.AppState{client}";
				var type = GetClientType(typeName);
				if (type != null)
				{
					ClientState = ActivatorUtilities.CreateInstance(_serviceProvider, type);
					StateHasChanged();
				}
			}
		}
	}

	public Type GetClientType(string typeName)
	{
		var assembly = typeof(CascadingAppState).Assembly;
		return assembly.GetType(typeName);
	}
}