using FormApproval.Services;

var builder = WebApplication.CreateBuilder(args);

// Blazor Web App services
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// App services
builder.Services.AddSingleton<IFormRepository, InMemoryFormRepository>();
builder.Services.AddSingleton<CurrentUserStub>();
builder.Services.AddSingleton<ICurrentUser>(sp => sp.GetRequiredService<CurrentUserStub>());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// ✅ Add this line for anti-forgery support
app.UseAntiforgery();

app.MapRazorComponents<FormApproval.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();