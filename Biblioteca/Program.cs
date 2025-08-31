using Biblioteca.Models;
using Biblioteca.Repositorios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(opciones =>
{
    opciones.IdleTimeout = System.TimeSpan.FromMinutes(60);
    opciones.Cookie.HttpOnly = true;
    opciones.Cookie.IsEssential = true;
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IConexion, ConexionDAO>();
builder.Services.AddScoped<IRepositorioUsuario, UsuarioDAO>();
builder.Services.AddScoped<IUsuarioRepository<Usuario>, UsuarioDAO>(); 
builder.Services.AddScoped<IServicioAutenticacion, ServicioAutenticacion>();
builder.Services.AddScoped<IServicioSesion, ServicioSesion>();
builder.Services.AddScoped<IRecursos, RecursosDAO>();
builder.Services.AddScoped<IReservas, ReservaDAO>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

//app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Acceder}/{id?}");

app.Run();
