using System;
using System.Collections.Generic;
using MapLib;
using MapLib.Attributes;
using MapLib.Configuration;
using MapLib.Extensions;

namespace MapLib.Examples
{
    // Classes de exemplo
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public Address? Address { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Age { get; set; }
        public AddressDto? Address { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }

    public class Address
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
    }

    public class AddressDto
    {
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
    }

    // Exemplo com atributos
    public class Product
    {
        public int Id { get; set; }
        
        [MapTo("ProductName")]
        public string Name { get; set; } = string.Empty;
        
        public decimal Price { get; set; }
        
        [IgnoreMap]
        public string InternalCode { get; set; } = string.Empty;
    }

    public class ProductDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    // Profile de mapeamento customizado
    public class UserMappingProfile : MappingProfile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => CalculateAge(src.BirthDate)));
        }

        private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== MapLib - Exemplos de Uso ===\n");

            // Exemplo 1: Mapeamento básico por convenção
            Console.WriteLine("1. Mapeamento Básico por Convenção:");
            BasicMappingExample();
            Console.WriteLine();

            // Exemplo 2: Mapeamento com atributos
            Console.WriteLine("2. Mapeamento com Atributos:");
            AttributeMappingExample();
            Console.WriteLine();

            // Exemplo 3: Mapeamento com configuração fluente
            Console.WriteLine("3. Mapeamento com Configuração Fluente:");
            FluentConfigurationExample();
            Console.WriteLine();

            // Exemplo 4: Mapeamento de coleções
            Console.WriteLine("4. Mapeamento de Coleções:");
            CollectionMappingExample();
            Console.WriteLine();

            // Exemplo 5: Mapeamento de objetos aninhados
            Console.WriteLine("5. Mapeamento de Objetos Aninhados:");
            NestedObjectMappingExample();
            Console.WriteLine();

            Console.WriteLine("Pressione qualquer tecla para sair...");
            Console.ReadKey();
        }

        static void BasicMappingExample()
        {
            var config = new MapperConfiguration();
            var mapper = config.CreateMapper();

            var address = new Address
            {
                Street = "Rua das Flores, 123",
                City = "São Paulo",
                ZipCode = "01234-567"
            };

            var addressDto = mapper.Map<Address, AddressDto>(address);

            Console.WriteLine($"  Endereço: {addressDto.Street}, {addressDto.City} - {addressDto.ZipCode}");
        }

        static void AttributeMappingExample()
        {
            var config = new MapperConfiguration();
            var mapper = config.CreateMapper();

            var product = new Product
            {
                Id = 1,
                Name = "Notebook",
                Price = 3500.00m,
                InternalCode = "INTERNAL-123" // Será ignorado
            };

            var productDto = mapper.Map<Product, ProductDto>(product);

            Console.WriteLine($"  Produto: {productDto.ProductName} - R$ {productDto.Price:F2}");
            Console.WriteLine($"  (InternalCode foi ignorado no mapeamento)");
        }

        static void FluentConfigurationExample()
        {
            var config = new MapperConfiguration();
            config.AddProfile<UserMappingProfile>();
            var mapper = config.CreateMapper();

            var user = new User
            {
                Id = 1,
                FirstName = "João",
                LastName = "Silva",
                Email = "joao.silva@example.com",
                BirthDate = new DateTime(1990, 5, 15)
            };

            var userDto = mapper.Map<User, UserDto>(user);

            Console.WriteLine($"  Nome Completo: {userDto.FullName}");
            Console.WriteLine($"  Email: {userDto.Email}");
            Console.WriteLine($"  Idade: {userDto.Age} anos");
        }

        static void CollectionMappingExample()
        {
            var config = new MapperConfiguration();
            config.AddProfile<UserMappingProfile>();
            var mapper = config.CreateMapper();

            var users = new List<User>
            {
                new User { Id = 1, FirstName = "Maria", LastName = "Santos", Email = "maria@example.com", BirthDate = new DateTime(1985, 3, 20) },
                new User { Id = 2, FirstName = "Pedro", LastName = "Oliveira", Email = "pedro@example.com", BirthDate = new DateTime(1992, 8, 10) },
                new User { Id = 3, FirstName = "Ana", LastName = "Costa", Email = "ana@example.com", BirthDate = new DateTime(1988, 12, 5) }
            };

            var userDtos = users.MapList<User, UserDto>(mapper);

            Console.WriteLine($"  Total de usuários mapeados: {userDtos.Count}");
            foreach (var dto in userDtos)
            {
                Console.WriteLine($"    - {dto.FullName} ({dto.Age} anos)");
            }
        }

        static void NestedObjectMappingExample()
        {
            var config = new MapperConfiguration();
            config.AddProfile<UserMappingProfile>();
            var mapper = config.CreateMapper();

            var user = new User
            {
                Id = 1,
                FirstName = "Carlos",
                LastName = "Ferreira",
                Email = "carlos@example.com",
                BirthDate = new DateTime(1995, 7, 25),
                Address = new Address
                {
                    Street = "Av. Paulista, 1000",
                    City = "São Paulo",
                    ZipCode = "01310-100"
                },
                Roles = new List<string> { "Admin", "User", "Manager" }
            };

            var userDto = mapper.Map<User, UserDto>(user);

            Console.WriteLine($"  Usuário: {userDto.FullName}");
            Console.WriteLine($"  Endereço: {userDto.Address?.Street}, {userDto.Address?.City}");
            Console.WriteLine($"  Roles: {string.Join(", ", userDto.Roles)}");
        }
    }
}
