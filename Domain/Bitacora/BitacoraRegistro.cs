using System;

namespace Domain
{
    public class BitacoraRegistro
    {
        public int Id { get; set; }
        public int? IdUsuario { get; set; }
        public string IdentificadorUsuario { get; set; }
        public string Modulo { get; set; }
        public string Accion { get; set; }
        public string Nivel { get; set; }
        public string Descripcion { get; set; }
        public string Equipo { get; set; }
        public DateTime Fecha { get; set; }

        public AuditoriaMemento SaveToMemento()
        {
            return new AuditoriaMemento("Bitacora", Id, new System.Collections.Generic.Dictionary<string, object>
            {
                { "Id", Id },
                { "IdUsuario", IdUsuario },
                { "IdentificadorUsuario", IdentificadorUsuario },
                { "Modulo", Modulo },
                { "Accion", Accion },
                { "Nivel", Nivel },
                { "Descripcion", Descripcion },
                { "Equipo", Equipo },
                { "Fecha", Fecha }
            });
        }
    }
}
