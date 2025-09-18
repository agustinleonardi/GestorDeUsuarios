using System;
using System.Runtime.Serialization;

namespace GestorDeUsuarios.Domain.Entities;

public class Usuario
{
    public int Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public DateTime FechaCreacion { get; private set; }

    //Constructor para logica
    public Usuario(int id, string nombre, string email, DateTime fechaCreacion)
    {
        Id = id;
        Nombre = nombre;
        Email = email;
        FechaCreacion = fechaCreacion;
    }

    //Constructor para EF 
    protected Usuario() { }
}