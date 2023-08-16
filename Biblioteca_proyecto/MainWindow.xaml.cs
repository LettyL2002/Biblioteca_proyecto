using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Biblioteca_proyecto // Cambia nombre
{
    public partial class MainWindow: Window
    {
        public MainWindow() 
        {
            InitializeComponent();
            /* Datos de prueba */
            prestamos.Add("1", new Prestamo("1", DateTime.Now, "Juan", "El principito", "Antoine de Saint-Exupéry", DateTime.Now.AddDays(7)));
            prestamos.Add("2", new Prestamo("2", DateTime.Now, "Maria", "Cien años de soledad", "Gabriel Garcia Marquez", DateTime.Now.AddDays(14)));
            DataGrid_datos.Items.Refresh(); 
        }

        readonly Dictionary < string, Prestamo > prestamos = new();
        readonly BindingList < Prestamo > prestamosBinding = new();

        private void ButtonAgregar_Click(object sender, EventArgs e) {
            if (!ValidarDatos()) return;

            string numero = textBoxNumero.Text;
            DateTime fecha = dateTimePicker1.SelectedDate.Value.ToUniversalTime();
            string nombre = textBoxNombre.Text;
            string titulo = textBoxTitulo.Text;
            string autor = textBoxAutor.Text;
            DateTime devolucion = dateTimePicker2.SelectedDate.Value.ToUniversalTime();

            Prestamo prestamo = new(numero, fecha, nombre, titulo, autor, devolucion);
            prestamos.Add(numero, prestamo);
            prestamosBinding.Add(prestamo);

            if (MessageBox.Show("¿Desea agregar otro prestamo?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                LimpiarDatos();
            } else if (MessageBox.Show("¿Seguro que desea eliminar este préstamo?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.No) {

                LimpiarDatos();

            }

        }

        /* Funciones */

        private bool ValidarDatos() {
            if (!int.TryParse(textBoxNumero.Text, out int numero)) {
                MessageBox.Show("El número debe ser un valor numérico");
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBoxNombre.Text)) {
                MessageBox.Show("El nombre no puede estar vacío");
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBoxTitulo.Text)) {
                MessageBox.Show("El título no puede estar vacío");
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBoxAutor.Text)) {
                MessageBox.Show("El autor no puede estar vacío");
                return false;
            }

            if (dateTimePicker2.SelectedDate < DateTime.Now) {
                MessageBox.Show("La fecha de devolución debe ser posterior a la fecha actual");
                return false;
            }

            if (prestamos.ContainsKey(textBoxNumero.Text)) {
                MessageBox.Show("Ya existe un préstamo con ese número");
                return false;
            }

            return true;
        }

        private void LimpiarDatos() {
            textBoxNumero.Clear();
            dateTimePicker1 = null;
            textBoxNombre.Clear();
            textBoxTitulo.Clear();
            textBoxAutor.Clear();
            dateTimePicker2 = null;
        }

        private void ButtonBuscar_Click(object sender, EventArgs e)
        {
            string numero = textBoxBuscar.Text.Trim();

            if (string.IsNullOrEmpty(numero))
            {
                MessageBox.Show("Debe ingresar un número de préstamo");
                return;
            }

            if (!prestamos.ContainsKey(numero))
            {
                MessageBox.Show("El número de préstamo ingresado no existe");
                return;
            }

            Prestamo prestamo = prestamos[numero];

            int index = prestamosBinding.IndexOf(prestamo);

            if (index >= 0)
            {
                DataGrid_datos.SelectedItem = prestamosBinding[index];
            }
            else
            {
                MessageBox.Show("Préstamo no encontrado en la lista");
            }

        }

        private void ButtonModificar_Click(object sender, EventArgs e) {
            string numero = textBoxModificar.Text;

            if (prestamos.ContainsKey(numero)) {
                Prestamo prestamo = prestamos[numero];

                MessageBox.Show("Préstamo modificado");
            } else {
                MessageBox.Show("No se encontró préstamo para modificar");
            }
        }

        private void ButtonEliminar_Click(object sender, EventArgs e) {
            string numero = textBoxEliminar.Text; 

            if (prestamos.ContainsKey(numero)) {
                Prestamo prestamo = prestamos[numero];

                if (prestamo.Devolucion > DateTime.Now) {
                    MessageBox.Show("No se puede eliminar un préstamo activo");
                    return;
                }

                if (MessageBox.Show("¿Seguro que desea eliminar este préstamo?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                    prestamos.Remove(numero);
                    prestamosBinding.Remove(prestamo);
                    MessageBox.Show("Préstamo eliminado");
                } else if (MessageBox.Show("¿Seguro que desea eliminar este préstamo?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.No) {
                    LimpiarDatos();

                }

            } else {
                MessageBox.Show("No se encontró préstamo para eliminar");
            }
        }

        private void ButtonGuardar_Click(object sender, EventArgs e) {
            try {
                using(StreamWriter writer = new StreamWriter("prestamos.txt")) {
                    foreach(Prestamo prestamo in prestamosBinding) {
                        writer.WriteLine(prestamo.ToString());
                    }
                }

                MessageBox.Show("Datos guardados en el archivo prestamos.txt");
            } catch (Exception ex) {
                MessageBox.Show("Error al guardar datos: " + ex.Message);
            }
        }

        class Prestamo 
        {
            public string Numero 
            {
                get;
                set;
            }
            public DateTime Fecha 
            {
                get;
                set;
            }
            public string Nombre 
            {
                get;
                set;
            }
            public string Titulo 
            {
                get;
                set;
            }
            public string Autor 
            {
                get;
                set;
            }
            public DateTime Devolucion 
            {
                get;
                set;
            }

            public Prestamo(string numero, DateTime fecha, string nombre, string titulo, string autor, DateTime devolucion) {
                Numero = numero;
                Fecha = fecha;
                Nombre = nombre;
                Titulo = titulo;
                Autor = autor;
                Devolucion = devolucion;
            }

            public override string ToString() 
            {
                return Numero + "|" + Fecha.ToShortDateString() + "|" + Nombre + "|" + Titulo + "|" + Autor + "|" + Devolucion.ToShortDateString();
            }
        }

    }
}