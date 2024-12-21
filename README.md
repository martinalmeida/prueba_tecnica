# ASP.NET Core
ASP.NET Core es un marco de trabajo multiplataforma y de código abierto para crear aplicaciones modernas basadas en la nube y conectadas a Internet, como aplicaciones web, aplicaciones de IoT y backends móviles. Las aplicaciones de ASP.NET Core se ejecutan en [.NET](https://dot.net), un entorno de ejecución de aplicaciones gratuito, multiplataforma y de código abierto. Fue diseñado para proporcionar un marco de trabajo de desarrollo optimizado para aplicaciones que se implementan en la nube o se ejecutan en las instalaciones. Consta de componentes modulares con una sobrecarga mínima, por lo que conserva la flexibilidad mientras crea sus soluciones. Puede desarrollar y ejecutar sus aplicaciones de ASP.NET Core en varias plataformas en Windows, Mac y Linux. [Obtenga más información sobre ASP.NET Core](https://learn.microsoft.com/aspnet/core/).

## Empezar

Siga las instrucciones de [Introducción](https://learn.microsoft.com/aspnet/core/getting-started).

Consulte también la [página de inicio de .NET para ver versiones lanzadas de .NET, guías de introducción y recursos de aprendizaje](https://www.microsoft.com/net).

## Requisitos previos

Antes de comenzar, asegúrese de tener lo siguiente instalado:

1. **.NET SDK**: Descargue e instale el SDK de [.NET](https://dotnet.microsoft.com/download).
2. **Editor de código**: Use Visual Studio, Visual Studio Code o cualquier editor que prefiera para trabajar con C# y ASP.NET Core.
3. **Servidor LDAP**: Si está utilizando un servidor LDAP simulado o un servidor real, asegúrese de tener acceso y las credenciales necesarias para autenticar usuarios. Si no tiene un servidor LDAP a mano, puede usar un servicio simulado para pruebas.

    Para probar el sistema LDAP, puede utilizar la herramienta [Filestash LDAP Test Tool](https://www.filestash.app/ldap-test-tool.html?host=ldap%3A%2F%2Fldap.forumsys.com). Este servicio proporciona un servidor LDAP de prueba con acceso a través de las credenciales predeterminadas:
    
    - **Usuario**: `uid=newton,dc=example,dc=com`
    - **Contraseña**: `password`

    **Nota**: Asegúrese de modificar las configuraciones LDAP en el archivo de configuración de su aplicación para que coincidan con las credenciales y la URL del servidor LDAP que está utilizando.

## Configuración de la Aplicación

1. **Configurar la cadena de conexión LDAP**:

   En el archivo `appsettings.json`, asegúrese de tener la siguiente configuración de LDAP:

    ```json
    {
      "Ldap": {
        "Server": "ldap://ldap.forumsys.com",  // Reemplace con la URL de su servidor LDAP
        "BaseDn": "dc=example,dc=com",  // Asegúrese de que esta base DN sea correcta
        "AdminUser": "cn=read-only-admin,dc=example,dc=com",  // Usuario administrador
        "AdminPassword": "password"  // Contraseña del administrador
      }
    }
    ```

2. **Modificar el filtro LDAP según el atributo del nombre de usuario**:

   En el código de autenticación LDAP, ajuste el filtro de búsqueda para usar el atributo adecuado de acuerdo con su servidor LDAP. Si está utilizando `uid` para el nombre de usuario, mantenga el filtro como sigue:

    ```csharp
    string searchFilter = $"(uid={username})";
    ```

   Si su servidor LDAP usa otro atributo como `cn`, modifique el filtro en consecuencia:

    ```csharp
    string searchFilter = $"(cn={username})";
    ```

3. **Probar la autenticación**:

   El método `Authenticate` que se utiliza en el backend realiza una búsqueda del usuario en el servidor LDAP y luego verifica la contraseña. Asegúrese de que las credenciales enviadas sean correctas y coincidan con las almacenadas en el servidor LDAP.

   Aquí hay un ejemplo de cómo usar este método en un controlador:

    ```csharp
    [HttpPost("authenticate")]
    public IActionResult Authenticate([FromBody] LoginRequest request)
    {
        var ldapEntry = _ldapService.Authenticate(request.Username, request.Password);
        if (ldapEntry != null)
        {
            return Ok(new { Message = "Autenticación exitosa." });
        }
        return Unauthorized(new { Message = "Credenciales inválidas." });
    }
    ```

4. **Verificación de la respuesta 401**:

   Si recibe un error **401 - Unauthorized**, es posible que las credenciales sean incorrectas o que el DN utilizado para hacer el `Bind` no sea válido. Asegúrese de que el atributo y el formato del nombre de usuario (`uid`, `cn`, etc.) sean correctos según el servidor LDAP.

   Si el servidor LDAP simulado (como el de Filestash) está funcionando correctamente, pero sigue obteniendo 401, intente validar los detalles de la conexión en el código, como el DN y el filtro de búsqueda.

## Herramientas

Para probar la conexión LDAP, puede usar la herramienta de prueba LDAP de Filestash:

- [Filestash LDAP Test Tool](https://www.filestash.app/ldap-test-tool.html?host=ldap%3A%2F%2Fldap.forumsys.com)

**Credenciales para pruebas:**
- Usuario: `uid=newton,dc=example,dc=com`
- Contraseña: `password`

Esto le permitirá verificar si la conexión y las credenciales LDAP son correctas antes de implementarlas en su aplicación.
