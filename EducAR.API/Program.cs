using System.Text;
using EducAR.API.Data;
using EducAR.API.Helpers;
using EducAR.API.Repositories;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services;
using EducAR.API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using EducAR.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = builder.Configuration["Jwt:Issuer"],
            ValidAudience            = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Inyección de dependencias
builder.Services.AddScoped<JwtHelper>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IDocenteRepository, DocenteRepository>();
builder.Services.AddScoped<IDocenteService, DocenteService>();
builder.Services.AddScoped<ITutorRepository, TutorRepository>();
builder.Services.AddScoped<ITutorService, TutorService>();
builder.Services.AddScoped<IAlumnoRepository, AlumnoRepository>();
builder.Services.AddScoped<IAlumnoService, AlumnoService>();
builder.Services.AddScoped<ICicloLectivoRepository, CicloLectivoRepository>();
builder.Services.AddScoped<ICicloLectivoService, CicloLectivoService>();
builder.Services.AddScoped<IMateriaRepository, MateriaRepository>();
builder.Services.AddScoped<IMateriaService, MateriaService>();
builder.Services.AddScoped<ICursoRepository, CursoRepository>();
builder.Services.AddScoped<ICursoService, CursoService>();
builder.Services.AddScoped<IPeriodoEvaluacionRepository, PeriodoEvaluacionRepository>();
builder.Services.AddScoped<IPeriodoEvaluacionService, PeriodoEvaluacionService>();
builder.Services.AddScoped<IAsistenciaRepository, AsistenciaRepository>();
builder.Services.AddScoped<IAsistenciaService, AsistenciaService>();
builder.Services.AddScoped<ICalificacionRepository, CalificacionRepository>();
builder.Services.AddScoped<ICalificacionService, CalificacionService>();
builder.Services.AddScoped<IEvaluacionService, EvaluacionService>();
builder.Services.AddScoped<IBoletinRepository, BoletinRepository>();
builder.Services.AddScoped<IBoletinService, BoletinService>();
builder.Services.AddScoped<IMatriculaRepository, MatriculaRepository>();
builder.Services.AddScoped<IMatriculaService, MatriculaService>();
builder.Services.AddScoped<IMensajeRepository, MensajeRepository>();
builder.Services.AddScoped<IMensajeService, MensajeService>();
builder.Services.AddScoped<IEmailNotificacionesService, EmailNotificacionesService>();
builder.Services.AddScoped<IAlumnoTutorRepository, AlumnoTutorRepository>();
builder.Services.AddScoped<IAlumnoTutorService, AlumnoTutorService>();
builder.Services.AddScoped<IDocenteMateriaCursoRepository, DocenteMateriaCursoRepository>();
builder.Services.AddScoped<IDocenteMateriaCursoService, DocenteMateriaCursoService>();
builder.Services.AddScoped<IEscuelaRepository, EscuelaRepository>();
builder.Services.AddScoped<IEscuelaService, EscuelaService>();
builder.Services.AddScoped<IEscuelaRepository, EscuelaRepository>();
builder.Services.AddScoped<IEscuelaService, EscuelaService>();
builder.Services.AddScoped<ITelefonoRepository, TelefonoRepository>();
builder.Services.AddScoped<ITelefonoService, TelefonoService>();

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("EducAR_Policy", policy =>
    {
        policy
            .AllowAnyOrigin()    // cualquier origen (app Android, emulador, dispositivo)
            .AllowAnyMethod()    // GET, POST, PUT, PATCH, DELETE
            .AllowAnyHeader();   // Authorization, Content-Type, etc.
    });
});
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.Http,
        Scheme       = "bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,
        Description  = "Ingresá el token JWT (sin la palabra Bearer)"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
       [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseCors("EducAR_Policy");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
