using System.Collections.Generic;
using DAL;
using Domain;

namespace Repository
{
    public class LanguageRepository
    {
        private readonly IdiomaDataMapper _idiomaDataMapper;

        public LanguageRepository()
            : this(new IdiomaDataMapper())
        {
        }

        public LanguageRepository(IdiomaDataMapper idiomaDataMapper)
        {
            _idiomaDataMapper = idiomaDataMapper;
        }

        public int Crear(Idioma idioma, int? idUsuarioResponsable)
        {
            return _idiomaDataMapper.Crear(idioma, idUsuarioResponsable);
        }

        public bool Actualizar(Idioma idioma, int? idUsuarioResponsable, string motivo)
        {
            return _idiomaDataMapper.Actualizar(idioma, idUsuarioResponsable, motivo);
        }

        public Idioma ObtenerPorId(int id)
        {
            return _idiomaDataMapper.ObtenerPorId(id);
        }

        public Idioma ObtenerDefault()
        {
            return _idiomaDataMapper.ObtenerDefault();
        }

        public List<Idioma> Listar(bool soloActivos)
        {
            return _idiomaDataMapper.Listar(soloActivos);
        }
    }
}
