using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using U3_Topicos_Actividad_1.ViewModels;

namespace U3_Topicos_Actividad_1.Models
{
    class IngredienteModel
    {
        public string Nombre { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public Categorias Categoria { get; set; }
        public string Imagen { get; set; } = "";
        public string Proveedor { get; set; } = "";
        public int Cantidad { get; set; }
        public Medidas Medida { get; set; }
        public DateTime Caducidad { get; set; }
    }
}
