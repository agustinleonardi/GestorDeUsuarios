using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
using Microsoft.VisualBasic;

namespace GestorDeUsuarios.Domain.Entities;

public class Domicilio
{
    public int Id { get; private set; }
    public int UsuarioID { get; private set; }
    public string Calle { get; set; } = string.Empty;
    public string Numero { get; private set; } = string.Empty;
    public string Provincia { get; private set; } = string.Empty;
    public string Ciudad { get; private set; } = string.Empty;
    public DateTime FechaCreacion { get; private set; }

    public Domicilio(int id, int usuarioId, string calle, string numero, string provincia, string ciudad, DateTime fechaCreacion)
    {
        Id = id;
        UsuarioID = usuarioId;
        Calle = calle;
        Numero = numero;
        Provincia = provincia;
        Ciudad = ciudad;
        FechaCreacion = fechaCreacion;
    }

    protected Domicilio() { }

}