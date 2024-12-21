using Novell.Directory.Ldap;
using Microsoft.Extensions.Configuration;
using prueba_tecnica.Services;
using System;

namespace prueba_tecnica.Services
{
    public class LdapService
    {
        private readonly IConfiguration _configuration;
        private readonly TokenService _tokenService;

        public LdapService(IConfiguration configuration, TokenService tokenService)
        {
            _configuration = configuration;
            _tokenService = tokenService;
        }

        public LdapEntry? Authenticate(string username, string password)
        {
            // Configuración del servidor LDAP desde appsettings.json
            string ldapServer = _configuration["Ldap:Server"];
            string baseDn = _configuration["Ldap:BaseDn"];
            string adminUser = _configuration["Ldap:AdminUser"];
            string adminPassword = _configuration["Ldap:AdminPassword"];

            try
            {
                using (var connection = new LdapConnection())
                {
                    // Conectarse al servidor LDAP
                    connection.Connect(ldapServer, 389);
                    connection.Bind(adminUser, adminPassword);

                    // Construir el filtro de búsqueda basado en el username
                    string searchFilter = $"(cn={username})";
                    var searchResults = connection.Search(
                        baseDn, // Base DN
                        LdapConnection.ScopeSub, // Búsqueda en subárbol
                        searchFilter, // Filtro de búsqueda
                        null, // Todos los atributos
                        false
                    );

                    // Verificar si encontramos el usuario
                    if (searchResults.HasMore())
                    {
                        var entry = searchResults.Next();

                        // Intentar hacer bind con las credenciales del usuario
                        connection.Bind(entry.Dn, password);
                        if (connection.Bound)
                        {
                            return entry; // Retorna el entry con los datos del usuario
                        }
                    }

                    return null;
                }
            }
            catch (LdapException ex)
            {
                Console.WriteLine($"Error de LDAP: {ex.Message}");
                return null;
            }
        }

        public string GenerateToken(LdapEntry ldapEntry)
        {
            // Extraer información del entry LDAP
            string username = ldapEntry.GetAttribute("uid").StringValue;
            string email = ldapEntry.GetAttribute("mail").StringValue;
            string displayName = ldapEntry.GetAttribute("cn").StringValue;
            string department = "Department";  // Este dato lo puedes agregar si está en LDAP
            string title = "Title";  // Este dato lo puedes agregar si está en LDAP

            // Generar el token usando el servicio TokenService
            return _tokenService.GenerateToken(username, email, displayName, department, title);
        }
    }
}

