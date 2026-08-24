using System;
using Abstractions;

namespace Domain
{
    public class Bitacora : IBitacoraEvento
    {
        public int Id { get; set; }
        public int? IdUsuario { get; set; }
        public string IdentificadorUsuario { get; set; }
        public BitacoraModulo Modulo { get; set; }
        public BitacoraAccion Accion { get; set; }
        public BitacoraNivel Nivel { get; set; }
        public string Descripcion { get; set; }
        public string Equipo { get; set; }
        public DateTime Fecha { get; set; }
    }
}
