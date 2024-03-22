using ProyectoCRUD.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoCRUD.Controllers
{
    public class ContactoController : Controller
    {
        //Creo la cadena de conexion en WebConfig..
        private static string conexion = ConfigurationManager.ConnectionStrings["cadena"].ToString();

        //Creo la lista de Contactos
        private static List<Contacto> lista = new List<Contacto>();
        // GET: Contacto
        public ActionResult Inicio()
        {
            //Creo una lista nueva..
            lista = new List<Contacto>();

            //Creo la conexion con la base de datos.
            using (SqlConnection oconexion = new SqlConnection(conexion))
            {
                SqlCommand comando = new SqlCommand("SELECT * FROM CONTACTO", oconexion);
                comando.CommandType = CommandType.Text;
                oconexion.Open();

                //Le doy la orden de que proceda a ejecutar la instruccion (Select * from contacto) 
                //El executereader hace que lea todo lo que contiene la tabla CONTACTO.
                using (SqlDataReader datareader = comando.ExecuteReader())
                {
                    while (datareader.Read())
                    {
                        Contacto nuevoContacto = new Contacto();
                        nuevoContacto.IdContacto = Convert.ToInt32(datareader["IdContacto"]);
                        nuevoContacto.Nombres = datareader["Nombres"].ToString();
                        nuevoContacto.Apellidos = datareader["Apellidos"].ToString();
                        nuevoContacto.Telefono = datareader["Telefono"].ToString();
                        nuevoContacto.Correo = datareader["Correo"].ToString();

                        lista.Add(nuevoContacto);
                    }
                }
            }
            //A nuestra vista le paso por parametro la lista de contactos.
            return View(lista);
        }
        //Tipo Get devuelve la vista.
        [HttpGet]
        public ActionResult Registrar()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Editar(int? idcontacto)
        {
            if(idcontacto == null)
                return RedirectToAction("Inicio", "Contacto");

            Contacto contacto = lista.Where(c => c.IdContacto == idcontacto).FirstOrDefault();
            //Le doy a la vista el elemento contacto para que trabaje con el
            return View(contacto);
        }

        [HttpGet]
        public ActionResult Eliminar(int? idcontacto)
        {
            if (idcontacto == null)
                return RedirectToAction("Inicio", "Contacto");

            Contacto contacto = lista.Where(c => c.IdContacto == idcontacto).FirstOrDefault();
            //Le doy a la vista el elemento contacto para que trabaje con el
            return View(contacto);
        }

        //Tipo Post hace una accion con el formulario que ya esta en la vista (en este caso Registrar) 
        [HttpPost]
        public ActionResult Registrar(Contacto contacto)
        {
            using (SqlConnection oconexion = new SqlConnection(conexion))
            {
                SqlCommand comando = new SqlCommand("SP_Registrar", oconexion);
                comando.Parameters.AddWithValue("Nombres", contacto.Nombres);
                comando.Parameters.AddWithValue("Apellidos", contacto.Apellidos);
                comando.Parameters.AddWithValue("Telefono", contacto.Telefono);
                comando.Parameters.AddWithValue("Correo", contacto.Correo);

                comando.CommandType = CommandType.StoredProcedure;
                oconexion.Open();
            //El ExecuteNonQuery se encarga de hacer la ejecucion de todo el comando(en este caso el registro)
                comando.ExecuteNonQuery();

               
            }
            //NO direcciono a la misma vista,Si no que, la redirecciono al formulario con la lista de contactos.
            return RedirectToAction("Inicio","Contacto");
        }

        [HttpPost]
        public ActionResult Editar(Contacto contacto)
        {
            using (SqlConnection oconexion = new SqlConnection(conexion))
            {
                SqlCommand comando = new SqlCommand("SP_Editar", oconexion);
                comando.Parameters.AddWithValue("IdContacto", contacto.IdContacto);
                comando.Parameters.AddWithValue("Nombres", contacto.Nombres);
                comando.Parameters.AddWithValue("Apellidos", contacto.Apellidos);
                comando.Parameters.AddWithValue("Telefono", contacto.Telefono);
                comando.Parameters.AddWithValue("Correo", contacto.Correo);

                comando.CommandType = CommandType.StoredProcedure;
                oconexion.Open();
                comando.ExecuteNonQuery();


            }
            return RedirectToAction("Inicio", "Contacto");
        }


        [HttpPost]
        public ActionResult Eliminar(string IdContacto)
        {
            using (SqlConnection oconexion = new SqlConnection(conexion))
            {
                SqlCommand comando = new SqlCommand("SP_Eliminar", oconexion);
                comando.Parameters.AddWithValue("IdContacto", IdContacto);
                comando.CommandType = CommandType.StoredProcedure;
                oconexion.Open();
                comando.ExecuteNonQuery();


            }
            return RedirectToAction("Inicio", "Contacto");
        }
    }
}