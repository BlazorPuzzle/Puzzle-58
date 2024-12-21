# Blazor Puzzle #58

## Multi-Tenant State

YouTube Video: https://youtu.be/A9_2rwenKkU

Blazor Puzzle Home Page: https://blazorpuzzle.com

### The Challenge:

In this demo, we have created an architecture that lets us customize the experience for our different clients. In the real world, the client would be decided based on the login or some other method, but we simply have buttons on the main screen to switch between clients.

We want to use an application state component for each client, but we don't want to cascade it around the entire app, as we have done in previous episodes and on BlazorTrain.

Instead, we want to create a state component for each client, and be able to access it as a scoped parameter in all the pages in each client. 

How can we provide customized state **components** for each client and maintain that state across client pages?

> :point_up: **Note**: We do not want a myriad of scoped services. We want components.

### The Solution:

Step one is to rewrite *CascadingAppState.cs* to support dynamically creating client state components based on their class name when the CurrentClient changes:

```C#
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
```

The result of this is to add the client state component as a property of `CascadingAppState`, which is a scoped component accessible everywhere.

The `ClientState` property is specified as an object.

The `GetClientType` method will come in handy during step 3, and is used in the `CurrentClient` setter.

Step 2 is to cast the `ClientState` property appropriately when accessing it. For example:

*Acme.razor*:

```c#
@using Components
<h3>Acme Page</h3>

@ClientState.AcmeMessage

@code {

    [CascadingParameter]
    public CascadingAppState AppState { get; set; }

    AppStateAcme ClientState => (AppStateAcme)AppState.ClientState;
}
```

Step 3 is to streamline the way that we instantiate the client components we use for pages. For this we will use `<DynamicComponent>`. Here is *ConfigPage.razor*:

```c#
@page "/configpage"

@if (AppState.CurrentClient == Client.None)
{
    <h3>Please select a client</h3>
}
else
{
    <DynamicComponent Type="@AppState.GetClientType($"Puzzle_58.Clients.{AppState.CurrentClient}.Config{AppState.CurrentClient}")" />
}

@code {

    [CascadingParameter]
    public CascadingAppState AppState { get; set; }
}
```

We can instantiate the client-specific component using the `DynamicComponent`, which gets it's type directly from the `GetClientType` method of the `CascadingAppState` component.

Boom!
