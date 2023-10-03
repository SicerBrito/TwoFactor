# Documentación 📄

- ## Autenticación en dos pasos o autenticación de dos factores o 2FA
    ### Que es?
    Es un método de seguridad que requiere dos formas distintas de verificar la identidad de un usuario antes de permitirle el acceso a una cuenta o sistema.

    El objetivo de la autenticación en dos pasos es agregar una capa adicional de seguridad a las cuentas en línea, ya que incluso si alguien conoce o roba la contraseña, no podrá acceder a la cuenta sin también poseer el segundo factor de autenticación.

- ## Que fue lo que hice?
    - Comence creando la entidad Usuario donde se va a almacenar el token secreto para la validación de los usuarios.
    
    <img src="Img/1.png" alt="EntidadUsuario" style="width: 1000px;">

    - Despues realice un metodo generico llamado FindFirst en Dominio/Interfaces/IGenericRepository que va a buscar la primera entidad en el repositorio que cumpla con la condición específica que tenemos en el repositorio.

    <img src="Img/2.png" alt="EntidadUsuario" style="width: 1000px;">

    - La condición específica a la que nos refererimos se encuentra dentro de nuestro Aplicacion/UsurarioRepository donde declaramos un método llamado GetByIdAsync que toma un parámetro id de tipo long que va a retonar, que va a buscar al usuario identificado con ese Id para pasarle el TokenSecreto 

- ## Migraciones
    dotnet ef migrations add InitialCreate --project ./Persistencia/ --startup-project ./API/ --output-dir ./Data/Migrations/

    dotnet ef database update --project ./Persistencia/ --startup-project ./API/  
