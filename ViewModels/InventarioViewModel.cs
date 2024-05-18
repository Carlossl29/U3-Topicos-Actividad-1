using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using U3_Topicos_Actividad_1.Models;

namespace U3_Topicos_Actividad_1.ViewModels
{
    public enum Categorias { Vegetales, Carnes, Frutas, Condimentos }
    public enum Medidas { kg, g, l, ml, unidad}
    public enum Filtros { Todos, CantidadAscendente, CantidadDescendente, Caducados, Vigentes, PorAgotarse, PorCaducar}
    class InventarioViewModel : INotifyPropertyChanged
    {
        private Filtros filtroseleccionado;
        public ICommand VerListaCommand { get; set; }
        public ICommand VerInicioCommand { get; set; }
        public ICommand VerAgregarCommand { get; set; }
        public ICommand VerEditarCommand { get; set; }
        public ICommand VerEliminarCommand { get; set; }
        public ICommand VerDetallesCommand { get; set; }
        public ICommand AgregarCommand { get; set; }
        public ICommand EditarCommand { get; set; }
        public ICommand EliminarCommand { get; set; }
        public ICommand BuscarCommand { get; set; }
        public Categorias Categoria { get; set; }
        public IngredienteModel Ingrediente { get; set; } = null!;
        public List<IngredienteModel> Ingredientes { get; set; } = new();
        public ObservableCollection<IngredienteModel> ListaIngredientes { get; set; } = new();
        public string Error { get; set; } = "";
        public string Busqueda { get; set; } = "";
        public Filtros FiltroSeleccionado
        {
            get
            {
                return filtroseleccionado;
            }
            set
            {
                filtroseleccionado = value;
                ActualizarLista();
            }
        }
        public IEnumerable<Medidas> ListaMedidas
        {
            get
            {
                return Enum.GetValues(typeof(Medidas)).Cast<Medidas>();
            }
        }
        public IEnumerable<Filtros> ListaFiltros
        {
            get
            {
                return Enum.GetValues(typeof(Filtros)).Cast<Filtros>(); 
            }
        }
        public ObservableCollection<IngredienteModel> Lista { get; set; } = new();

        public InventarioViewModel()
        {
            VerListaCommand = new Command<Categorias>(VerLista);
            VerInicioCommand = new Command(VerInicio);
            VerAgregarCommand = new Command(VerAgregar);
            AgregarCommand = new Command(Agregar);
            BuscarCommand = new Command(Buscar);
            VerEditarCommand = new Command(VerEditar);
            EditarCommand = new Command(Editar);
            VerDetallesCommand = new Command<IngredienteModel>(VerDetalles);
            VerEliminarCommand = new Command(VerEliminar);
            EliminarCommand = new Command(Eliminar);
            Cargar();
        }

        private void Eliminar()
        {
            if (Ingrediente != null)
            {
                Ingredientes.Remove(Ingrediente);
                ActualizarLista();
                VerLista(Categoria);
                Guardar();
            }
        }

        private void VerEliminar()
        {
            Shell.Current.GoToAsync("//Eliminar");
        }

        private void VerDetalles(IngredienteModel ingrediente)
        {
            Ingrediente = ingrediente;
            Actualizar(nameof(Ingrediente));
            Shell.Current.GoToAsync("//Detalles");
        }

        private void Editar()
        {
            Error = "";
            if (Ingrediente != null)
            {
                if (string.IsNullOrWhiteSpace(Ingrediente.Nombre))
                {
                    Error += "Ingrese un nombre.\n";
                }
                if (string.IsNullOrWhiteSpace(Ingrediente.Descripcion))
                {
                    Error += "Ingrese una descripción.\n";
                }
                if (string.IsNullOrWhiteSpace(Ingrediente.Imagen) || !Ingrediente.Imagen.StartsWith("https:") || !Ingrediente.Imagen.EndsWith(".jpg"))
                {
                    Error += "Ingrese una imagen en formato jpg.\n";
                }
                if (Ingrediente.Caducidad.Date < DateTime.Now.Date)
                {
                    Error += "Ingrese una fecha válida.\n";
                }
                if (string.IsNullOrWhiteSpace(Ingrediente.Proveedor))
                {
                    Error += "Ingrese un proveedor.\n";
                }
                if (Ingrediente.Cantidad == 0)
                {
                    Error += "Ingrese una cantidad válida.\n";
                }

                Actualizar(nameof(Error));

                if (Error == "")
                {
                    Ingredientes[indiceIngrediente] = Ingrediente;
                    Guardar();
                    VerDetalles(Ingrediente);
                }
            }
        }

        int indiceIngrediente;
        private void VerEditar()
        {
            IngredienteModel clon = new()
            {
                Categoria = Ingrediente.Categoria,
                Caducidad = Ingrediente.Caducidad,
                Descripcion = Ingrediente.Descripcion,
                Cantidad = Ingrediente.Cantidad,
                Imagen = Ingrediente.Imagen,
                Medida = Ingrediente.Medida,
                Nombre = Ingrediente.Nombre,
                Proveedor = Ingrediente.Proveedor
            };

            indiceIngrediente = Ingredientes.IndexOf(Ingrediente);
            Ingrediente = clon;
            Error = "";
            Actualizar(nameof(Ingrediente));
            Actualizar(nameof(Error));
            Shell.Current.GoToAsync("//Editar");
        }

        private void Buscar()
        {

            if (string.IsNullOrWhiteSpace(Busqueda))
            {
                ActualizarLista();
            }
            else
            {
                FiltroSeleccionado = Filtros.Todos;
                Actualizar(nameof(FiltroSeleccionado));
                List<IngredienteModel> ingredientes;
                int numero;
                if (int.TryParse(Busqueda, out numero))
                {
                    ingredientes = Ingredientes.Where(x => x.Cantidad == numero).ToList();
                    ListaIngredientes.Clear();
                    foreach (var ingrediente in ingredientes)
                    {
                        ListaIngredientes.Add(ingrediente);
                    }
                }
                else
                {
                    ingredientes = Ingredientes.Where(x => x.Descripcion.ToLower().Contains(Busqueda.ToLower()) || x.Nombre.ToLower().Contains(Busqueda.ToLower()) || x.Proveedor.ToLower().Contains(Busqueda.ToLower())).ToList();
                    ListaIngredientes.Clear();
                    foreach (var ingrediente in ingredientes)
                    {
                        ListaIngredientes.Add(ingrediente);
                    }
                }
                
            }

        }

        private void Agregar()
        {
            Error = "";
            if (Ingrediente != null)
            {
                if (string.IsNullOrWhiteSpace(Ingrediente.Nombre))
                {
                    Error += "Ingrese un nombre.\n";
                }
                if (string.IsNullOrWhiteSpace(Ingrediente.Descripcion))
                {
                    Error += "Ingrese una descripción.\n";
                }
                if (string.IsNullOrWhiteSpace(Ingrediente.Imagen) || !Ingrediente.Imagen.StartsWith("https:") || !Ingrediente.Imagen.EndsWith(".jpg"))
                {
                    Error += "Ingrese una imagen en formato jpg.\n";
                }
                if (Ingrediente.Caducidad.Date < DateTime.Now.Date)
                {
                    Error += "Ingrese una fecha válida.\n";
                }
                if (string.IsNullOrWhiteSpace(Ingrediente.Proveedor))
                {
                    Error += "Ingrese un proveedor.\n";
                }
                if (Ingrediente.Cantidad == 0)
                {
                    Error += "Ingrese una cantidad válida.\n";
                }

                Actualizar(nameof(Error));

                if (Error == "")
                {
                    Ingredientes.Add(Ingrediente);
                    Guardar();
                    VerLista(Categoria);
                }
            }
        }

        private void VerAgregar()
        {
            Ingrediente = new()
            {
                Categoria = Categoria,
                Caducidad = DateTime.Now,
            };
            Actualizar(nameof(Ingrediente));
            Shell.Current.GoToAsync("//Agregar");
        }

        private void VerLista(Categorias categoria)
        {
            Categoria = categoria;
            FiltroSeleccionado = Filtros.Todos;
            Busqueda = "";
            Actualizar(nameof(Categoria));
            Actualizar(nameof(FiltroSeleccionado));
            Actualizar(nameof(Busqueda));
            Shell.Current.GoToAsync("//Lista");
        }

        private void VerInicio()
        {
            Shell.Current.GoToAsync("//Inicio");
        }

        private void ActualizarLista()
        {
            if (FiltroSeleccionado == Filtros.Todos)
            {
                ListaIngredientes.Clear();
                var ingredientesfiltro = Ingredientes.Where(x=>x.Categoria == Categoria).OrderBy(x=>x.Nombre).ToList();   
                foreach (var ingrediente in ingredientesfiltro)
                {
                    ListaIngredientes.Add(ingrediente);
                }
            }
            if (FiltroSeleccionado == Filtros.CantidadDescendente)
            {
                ListaIngredientes.Clear();
                var ingredientesfiltro = Ingredientes.Where(x => x.Categoria == Categoria).OrderByDescending(x=>x.Cantidad).ToList();
                foreach (var ingrediente in ingredientesfiltro)
                {
                    ListaIngredientes.Add(ingrediente);
                }
            }
            if (FiltroSeleccionado == Filtros.CantidadAscendente)
            {
                ListaIngredientes.Clear();
                var ingredientesfiltro = Ingredientes.Where(x => x.Categoria == Categoria).OrderBy(x => x.Cantidad).ToList();
                foreach (var ingrediente in ingredientesfiltro)
                {
                    ListaIngredientes.Add(ingrediente);
                }
            }
            if (FiltroSeleccionado == Filtros.Caducados)
            {
                ListaIngredientes.Clear();
                var ingredientesfiltro = Ingredientes.Where(x => x.Categoria == Categoria && x.Caducidad.Date < DateTime.Now.Date).ToList();
                foreach (var ingrediente in ingredientesfiltro)
                {
                    ListaIngredientes.Add(ingrediente);
                }
            }
            if (FiltroSeleccionado == Filtros.Vigentes)
            {
                ListaIngredientes.Clear();
                var ingredientesfiltro = Ingredientes.Where(x => x.Categoria == Categoria && x.Caducidad.Date >= DateTime.Now.Date).ToList();
                foreach (var ingrediente in ingredientesfiltro)
                {
                    ListaIngredientes.Add(ingrediente);
                }
            }
            if (FiltroSeleccionado == Filtros.PorAgotarse)
            {
                ListaIngredientes.Clear();
                var ingredientesfiltro = Ingredientes.Where(x => x.Cantidad < 5).ToList();
                foreach (var ingrediente in ingredientesfiltro)
                {
                    ListaIngredientes.Add(ingrediente);
                }
            }
            if (FiltroSeleccionado == Filtros.PorCaducar)
            {
                ListaIngredientes.Clear();
                var ingredientesfiltro = Ingredientes.Where(x => x.Caducidad.Date < DateTime.Now.AddDays(2).Date).ToList();
                foreach (var ingrediente in ingredientesfiltro)
                {
                    ListaIngredientes.Add(ingrediente);
                }
            }
        }

        private void Guardar()
        {
            var ruta = FileSystem.AppDataDirectory;
            File.WriteAllText(ruta + "/inventario.json", JsonSerializer.Serialize(Ingredientes));
        }

        private void Cargar()
        {
            var ruta = FileSystem.AppDataDirectory;
            if (File.Exists(ruta + "/inventario.json"))
            {
                var datos = JsonSerializer.Deserialize<List<IngredienteModel>>(File.ReadAllText(ruta + "/inventario.json"));
                if (datos != null)
                {
                    Ingredientes.AddRange(datos);
                }
            }
        }

        private void Actualizar(string nombre)
        {
            PropertyChanged?.Invoke(this, new(nombre));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
