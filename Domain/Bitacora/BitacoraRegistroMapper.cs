namespace Domain
{
    public static class BitacoraRegistroMapper
    {
        public static BitacoraRegistro Mapear(IBitacoraEvento bitacora)
        {
            if (bitacora == null)
            {
                return null;
            }

            return new BitacoraRegistro
            {
                Id = bitacora.Id,
                IdUsuario = bitacora.IdUsuario,
                IdentificadorUsuario = bitacora.IdentificadorUsuario,
                Modulo = bitacora.Modulo.ToString(),
                Accion = bitacora.Accion.ToString(),
                Nivel = bitacora.Nivel.ToString(),
                Descripcion = bitacora.Descripcion,
                Equipo = bitacora.Equipo,
                Fecha = bitacora.Fecha
            };
        }
    }
}
