using System;

namespace Domain
{
    public class AuditoriaRegistro
    {
        public int Id { get; set; }
        public string Entidad { get; set; }
        public int IdEntidad { get; set; }
        public string Accion { get; set; }
        public int? IdUsuarioActor { get; set; }
        public string IdentificadorUsuarioActor { get; set; }
        public DateTime FechaEvento { get; set; }
        public string EstadoAnteriorJson { get; set; }
        public string EstadoNuevoJson { get; set; }
        public string CambiosJson { get; set; }
    }
}
