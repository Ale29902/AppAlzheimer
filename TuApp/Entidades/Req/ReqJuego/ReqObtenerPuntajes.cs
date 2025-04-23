using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuApp.Entidades
{
    public class ReqObtenerPuntajes
    {
        public int idPaciente { get; set; }
        public int idCuidador { get; set; }
        public int idJuego { get; set; }
    }
}