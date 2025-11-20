# MapFast - Biblioteca de Mapeamento de DTOs para C#

[![.NET Standard 2.1](https://img.shields.io/badge/.NET%20Standard-2.1-blue)](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)
[![NuGet](https://img.shields.io/nuget/v/MapFast.svg)](https://www.nuget.org/packages/MapFast/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

**MapFast** é uma biblioteca leve e eficiente para mapeamento automático entre classes e DTOs (Data Transfer Objects) em C#. Inspirada nas melhores práticas de bibliotecas como AutoMapper, a MapFast oferece uma solução simples e poderosa para transferência de dados entre camadas da sua aplicação.

## 📋 Índice

- [Características](#-características)
- [Instalação](#-instalação)
- [Início Rápido](#-início-rápido)
- [Guia de Uso](#-guia-de-uso)
  - [Mapeamento Básico](#mapeamento-básico)
  - [Mapeamento com Atributos](#mapeamento-com-atributos)
  - [Configuração Fluente](#configuração-fluente)
  - [Mapeamento de Coleções](#mapeamento-de-coleções)
  - [Objetos Aninhados](#objetos-aninhados)
- [API Reference](#-api-reference)
- [Melhores Práticas](#-melhores-práticas)
- [Exemplos Avançados](#-exemplos-avançados)

## ✨ Características

- **Mapeamento por Convenção**: Mapeia automaticamente propriedades com o mesmo nome
- **Atributos Declarativos**: Use `[MapTo]`, `[MapFrom]` e `[IgnoreMap]` para controle fino
- **API Fluente**: Configure mapeamentos complexos com uma sintaxe elegante
- **Alta Performance**: Usa Expression Trees compiladas com cache para máxima eficiência
- **Objetos Aninhados**: Suporte completo para mapeamento recursivo
- **Coleções**: Mapeia `List<T>`, `Array`, `IEnumerable<T>` automaticamente
- **Conversão de Tipos**: Conversão automática entre tipos compatíveis
- **Detecção de Referências Circulares**: Previne loops infinitos em grafos de objetos
- **Null-Safe**: Tratamento seguro de valores nulos
- **.NET Standard 2.1**: Compatível com .NET Core 3.0+, .NET 5+, .NET Framework 4.7.2+

## 📦 Instalação

### Via NuGet
```bash
dotnet add package MapFast
```

### Via Package Manager Console
```powershell
Install-Package MapFast
```

### Build Manual
```bash
git clone https://github.com/seu-usuario/MapLib.git
cd MapLib/MapLib
dotnet build
```

## 🚀 Início Rápido

```csharp
using MapLib;

// 1. Criar configuração
var config = new MapperConfiguration();
var mapper = config.CreateMapper();

// 2. Definir classes
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

// 3. Mapear!
var user = new User { Id = 1, Name = "João Silva", Email = "joao@example.com" };
var userDto = mapper.Map<User, UserDto>(user);

Console.WriteLine($"{userDto.Name} - {userDto.Email}");
// Output: João Silva - joao@example.com
```

## 📖 Guia de Uso

### Mapeamento Básico

O mapeamento por convenção funciona automaticamente para propriedades com o mesmo nome:

```csharp
var config = new MapperConfiguration();
var mapper = config.CreateMapper();

var source = new Product { Id = 1, Name = "Notebook", Price = 3500.00m };
var destination = mapper.Map<Product, ProductDto>(source);
```

### Mapeamento com Atributos

Use atributos para controlar o mapeamento de forma declarativa:

#### `[MapTo]` - Mapear para propriedade com nome diferente

```csharp
public class Product
{
    public int Id { get; set; }
    
    [MapTo("ProductName")]  // Mapeia para ProductDto.ProductName
    public string Name { get; set; }
}

public class ProductDto
{
    public int Id { get; set; }
    public string ProductName { get; set; }
}
```

#### `[IgnoreMap]` - Ignorar propriedade no mapeamento

```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    [IgnoreMap]  // Não será mapeado
    public string InternalCode { get; set; }
}
```

#### `[MapFrom]` - Especificar propriedade de origem

```csharp
public class UserDto
{
    public int Id { get; set; }
    
    [MapFrom("Name")]  // Mapeia de User.Name
    public string UserName { get; set; }
}
```

### Configuração Fluente

Para mapeamentos mais complexos, use Profiles com API fluente:

```csharp
using MapLib.Configuration;

public class UserProfile : MappingProfile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            // Combinar propriedades
            .ForMember(dest => dest.FullName, 
                opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            
            // Calcular valores
            .ForMember(dest => dest.Age, 
                opt => opt.MapFrom(src => CalculateAge(src.BirthDate)))
            
            // Ignorar propriedades
            .Ignore(dest => dest.InternalId);
    }
    
    private int CalculateAge(DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age)) age--;
        return age;
    }
}

// Usar o profile
var config = new MapperConfiguration();
config.AddProfile<UserProfile>();
var mapper = config.CreateMapper();
```

### Mapeamento de Coleções

MapLib mapeia coleções automaticamente:

```csharp
using MapLib.Extensions;

var users = new List<User>
{
    new User { Id = 1, Name = "Maria" },
    new User { Id = 2, Name = "João" },
    new User { Id = 3, Name = "Ana" }
};

// Método 1: Extension method
var userDtos = users.MapList<User, UserDto>(mapper);

// Método 2: Mapeamento direto
var userDtos2 = mapper.Map<List<User>, List<UserDto>>(users);

// Arrays também funcionam
User[] userArray = users.ToArray();
UserDto[] dtoArray = mapper.Map<User[], UserDto[]>(userArray);
```

### Objetos Aninhados

Mapeamento recursivo de objetos complexos:

```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Address Address { get; set; }
    public List<Order> Orders { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public AddressDto Address { get; set; }
    public List<OrderDto> Orders { get; set; }
}

// MapLib mapeia recursivamente todos os objetos aninhados
var userDto = mapper.Map<User, UserDto>(user);
```

## 📚 API Reference

### MapperConfiguration

Classe principal para configurar o mapeador.

```csharp
var config = new MapperConfiguration();

// Adicionar profile
config.AddProfile<MyProfile>();
config.AddProfile(new MyProfile());

// Criar mapper
IMapper mapper = config.CreateMapper();

// Validar configuração (futuro)
config.AssertConfigurationIsValid();
```

### IMapper

Interface principal para realizar mapeamentos.

```csharp
// Criar novo objeto de destino
TDestination Map<TSource, TDestination>(TSource source);

// Mapear para objeto existente
TDestination Map<TSource, TDestination>(TSource source, TDestination destination);
```

### MappingProfile

Classe base para criar perfis de mapeamento.

```csharp
public class MyProfile : MappingProfile
{
    public MyProfile()
    {
        CreateMap<Source, Destination>()
            .ForMember(dest => dest.Property, opt => opt.MapFrom(src => src.OtherProperty))
            .Ignore(dest => dest.IgnoredProperty)
            .ConvertUsing(src => new Destination { /* custom logic */ });
    }
}
```

### IMappingExpression

Interface fluente para configurar mapeamentos.

```csharp
CreateMap<Source, Destination>()
    // Mapear membro específico
    .ForMember(dest => dest.DestProperty, opt => opt.MapFrom(src => src.SourceProperty))
    
    // Ignorar membro
    .Ignore(dest => dest.IgnoredProperty)
    
    // Usar conversor customizado
    .ConvertUsing(src => new Destination { /* ... */ });
```

### Atributos

#### `[MapTo(string propertyName)]`
Especifica o nome da propriedade de destino.

#### `[MapFrom(string propertyName)]`
Especifica o nome da propriedade de origem.

#### `[IgnoreMap]`
Marca a propriedade para ser ignorada no mapeamento.

### Extension Methods

```csharp
using MapLib.Extensions;

// Mapear lista
List<TDestination> MapList<TSource, TDestination>(
    this IEnumerable<TSource> source, 
    IMapper mapper);

// Mapear objeto genérico
TDestination MapTo<TDestination>(this object source, IMapper mapper);
```

## 💡 Melhores Práticas

### 1. Configure Uma Vez, Use Sempre

```csharp
// ❌ Não faça isso
public UserDto GetUser(int id)
{
    var config = new MapperConfiguration();  // Cria nova config a cada chamada
    var mapper = config.CreateMapper();
    return mapper.Map<User, UserDto>(user);
}

// ✅ Faça isso
public class UserService
{
    private readonly IMapper _mapper;
    
    public UserService(IMapper mapper)  // Injete via DI
    {
        _mapper = mapper;
    }
    
    public UserDto GetUser(int id)
    {
        return _mapper.Map<User, UserDto>(user);
    }
}
```

### 2. Organize com Profiles

```csharp
// ✅ Bom: Agrupe mapeamentos relacionados
public class UserMappingProfile : MappingProfile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<User, UserSummaryDto>();
        CreateMap<UserCreateRequest, User>();
    }
}

public class OrderMappingProfile : MappingProfile
{
    public OrderMappingProfile()
    {
        CreateMap<Order, OrderDto>();
        CreateMap<OrderItem, OrderItemDto>();
    }
}
```

### 3. Mantenha DTOs Simples

```csharp
// ✅ Bom: DTO flat e simples
public class UserDto
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
}

// ❌ Evite: Lógica de negócio em DTOs
public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public void ValidateEmail() { /* ... */ }  // Não!
    public decimal CalculateDiscount() { /* ... */ }  // Não!
}
```

### 4. Use Mapeamento Simples

```csharp
// ✅ Bom: Mapeamento direto
CreateMap<User, UserDto>()
    .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

// ❌ Evite: Lógica complexa no mapeamento
CreateMap<Order, OrderDto>()
    .ForMember(dest => dest.Total, opt => opt.MapFrom(src => 
    {
        // Muita lógica aqui...
        var subtotal = src.Items.Sum(x => x.Price * x.Quantity);
        var discount = src.DiscountPercentage / 100m * subtotal;
        var tax = (subtotal - discount) * 0.15m;
        return subtotal - discount + tax + src.ShippingCost;
    }));
// Melhor: Calcule isso no modelo de domínio
```

## 🎯 Exemplos Avançados

### Conversor Customizado

```csharp
public class TemperatureProfile : MappingProfile
{
    public TemperatureProfile()
    {
        CreateMap<Temperature, TemperatureDto>()
            .ConvertUsing(src => new TemperatureDto 
            { 
                Fahrenheit = src.Celsius * 9 / 5 + 32 
            });
    }
}
```

### Mapeamento Condicional com Lógica Customizada

```csharp
public class UserProfile : MappingProfile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => 
                string.IsNullOrEmpty(src.NickName) ? src.FullName : src.NickName))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => 
                src.IsActive ? "Ativo" : "Inativo"));
    }
}
```

### Mapeamento Bidirecional

```csharp
public class ProductProfile : MappingProfile
{
    public ProductProfile()
    {
        // Produto -> DTO
        CreateMap<Product, ProductDto>();
        
        // DTO -> Produto
        CreateMap<ProductDto, Product>();
    }
}
```

## 🔧 Integração com ASP.NET Core

```csharp
// Startup.cs ou Program.cs
public void ConfigureServices(IServiceCollection services)
{
    // Criar configuração global
    var mapperConfig = new MapperConfiguration();
    mapperConfig.AddProfile<UserProfile>();
    mapperConfig.AddProfile<OrderProfile>();
    
    // Registrar como singleton
    var mapper = mapperConfig.CreateMapper();
    services.AddSingleton<IMapper>(mapper);
}

// Controller
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _repository;
    
    public UsersController(IMapper mapper, IUserRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }
    
    [HttpGet("{id}")]
    public ActionResult<UserDto> GetUser(int id)
    {
        var user = _repository.GetById(id);
        if (user == null) return NotFound();
        
        return _mapper.Map<User, UserDto>(user);
    }
    
    [HttpPost]
    public ActionResult<UserDto> CreateUser(CreateUserRequest request)
    {
        var user = _mapper.Map<CreateUserRequest, User>(request);
        _repository.Add(user);
        
        return CreatedAtAction(nameof(GetUser), 
            new { id = user.Id }, 
            _mapper.Map<User, UserDto>(user));
    }
}
```

## 📝 Licença

Este projeto está licenciado sob a licença MIT - veja o arquivo [LICENSE](LICENSE) para detalhes.

## 🤝 Contribuindo

Contribuições são bem-vindas! Sinta-se à vontade para abrir issues e pull requests.

## 📧 Suporte

Para questões e suporte, abra uma issue no [GitHub](https://github.com/seu-usuario/MapLib/issues).

---

**MapFast** - Mapeamento de DTOs simples, rápido e eficiente para C# 🚀

[![NuGet](https://img.shields.io/nuget/v/MapFast.svg)](https://www.nuget.org/packages/MapFast/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/MapFast.svg)](https://www.nuget.org/packages/MapFast/)
