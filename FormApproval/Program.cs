using FormApproval.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;

// App startup:
// - Configures Blazor (interactive server components).
// - Registers demo services (in-memory repo and current user stub).
// - Enables antiforgery protection and static files.
// - Maps the Razor Components app.
var builder = WebApplication.CreateBuilder(args);

// Blazor Web App services (server-side interactivity).
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// App services (DI registrations).
builder.Services.AddSingleton<IFormRepository, InMemoryFormRepository>(); // in-memory persistence
builder.Services.AddSingleton<CurrentUserStub>();                         // mock user/role
builder.Services.AddSingleton<ICurrentUser>(sp => sp.GetRequiredService<CurrentUserStub>());
builder.Services.AddSingleton<FormServices>(); // placeholder for future app services

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Anti-forgery support required by .NET 8 form posts (pairs with EditForm.FormName).
app.UseAntiforgery();

// Map the component app and enable interactive server render mode.
app.MapRazorComponents<FormApproval.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();