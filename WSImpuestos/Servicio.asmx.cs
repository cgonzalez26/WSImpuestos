using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using WSImpuestos.dsDatosTableAdapters;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using WSImpuestos.dsDenunciasTableAdapters;
using WSImpuestos.dsEstablecimientosTableAdapters;
using System.Configuration;
using System.Data.SqlClient;

namespace WSImpuestos
{
    /// <summary>
    /// Descripción breve de Servicio
    /// </summary>
    public class Employee
    {
        public string Name { get; set; }
        public string Company { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
    }

    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class Servicio : System.Web.Services.WebService
    {
        DenunciaTableAdapter Denuncia = new DenunciaTableAdapter();
        Impuesto_AutTableAdapter Impuesto_Aut = new Impuesto_AutTableAdapter();
        Impuesto_InmTableAdapter Impuesto_Inm = new Impuesto_InmTableAdapter();
        Impuesto_TSGTableAdapter Impuesto_TSG = new Impuesto_TSGTableAdapter();
        EstablecimientosTableAdapter Establecimiento = new EstablecimientosTableAdapter();
        dsDatos dsDatos = new dsDatos();
        dsDenuncias dsDenuncias = new dsDenuncias();
        dsEstablecimientos dsEstablecimientos = new dsEstablecimientos();

        //IMPUESTOS AUTOMOTOR
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //[ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public DataSet getImpuestos_Aut()
        {
            //DataSet ds;
            this.Impuesto_Aut.Fill(this.dsDatos.Impuesto_Aut);

            return dsDatos;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string TestJSON()
        {
            Employee[] e = new Employee[2];
            e[0] = new Employee();
            e[0].Name = "Ajay Singh";
            e[0].Company = "Birlasoft Ltd.";
            e[0].Address = "LosAngeles California";
            e[0].Phone = "1204675";
            e[0].Country = "US";
            e[1] = new Employee();
            e[1].Name = "Ajay Singh";
            e[1].Company = "Birlasoft Ltd.";
            e[1].Address = "D-195 Sector Noida";
            e[1].Phone = "1204675";
            e[1].Country = "India";
            return new JavaScriptSerializer().Serialize(e);
        }

        public static object DataTableToJSON(DataTable table)
        {
            var list = new List<Dictionary<string, object>>();

            foreach (DataRow row in table.Rows)
            {
                var dict = new Dictionary<string, object>();

                foreach (DataColumn col in table.Columns)
                {
                    dict[col.ColumnName] = (Convert.ToString(row[col]));
                }
                list.Add(dict);
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            return serializer.Serialize(list);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string getImpuesto_Aut2()
        {
            DataTable dt_impuestos = new DataTable();//DataTable();
            dt_impuestos = this.Impuesto_Aut.GetData();

            Dictionary<string, object> row;
            var rows = new List<Dictionary<string, object>>();
            foreach (DataRow dr in dt_impuestos.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt_impuestos.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }

            return JsonConvert.SerializeObject(rows);
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string getImpuesto_Inm2()
        {
            DataTable dt_impuestos = new DataTable();//DataTable();
            //DataSet ds;
            //this.Impuesto_Inm.Fill(this.dsDatos.Impuesto_Inm);
            dt_impuestos = this.Impuesto_Inm.GetData();

            Dictionary<string, object> row;
            var rows = new List<Dictionary<string, object>>();
            foreach (DataRow dr in dt_impuestos.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt_impuestos.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            /*var grid = new
            {
                page = 1,
                records = dt_impuestos.Count(),
                total = dt_impuestos.Count(),

                //rows = from item in dt_impuestos
                //       select new
                //       {
                //           id = item.iId_Imp_Inm,
                //           cell = new string[]{
                //             item.sTipo_Imp,
                //             item.nMonoto_Pagar.ToString(),
                //             //item.UnitPrice.ToString("N2")
                //        }
                //    }
                rows = rows
            };*/
            
            return JsonConvert.SerializeObject(rows);
        }

        [WebMethod]
        public DataSet getImpuesto_Inm()
        {
            //DataSet ds;
            this.Impuesto_Inm.Fill(this.dsDatos.Impuesto_Inm);

            return dsDatos;
        }

        [WebMethod]
        public DataSet getImpuesto_TSG()
        {
            //DataSet ds;
            this.Impuesto_TSG.Fill(this.dsDatos.Impuesto_TSG);

            return dsDatos;
        }

        //DENUNCIA

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string getDenuncias2()
        {
            this.Denuncia.Fill(this.dsDenuncias.Denuncia);

            return JsonConvert.SerializeObject(dsDenuncias, Formatting.Indented);
        }

        [WebMethod]
        public DataSet getDenuncias()
        {
            //DataSet ds;
            this.Denuncia.Fill(this.dsDenuncias.Denuncia);

            return dsDenuncias;
        }

        [WebMethod]
        public string insertDenuncia(string sTipo_Den, int iDNI, string sMail, string sNombre, string sApellido, int iN_Calle, string sEntre_Calle, string tObservaciones, decimal dLongitud, decimal dLatitud)
        {
            //iId_Denuncia
            string result;
            try
            {
                this.Denuncia.Insert(sTipo_Den, iDNI, sMail, sNombre, sApellido, iN_Calle, sEntre_Calle, tObservaciones, dLongitud, dLatitud);
                result = "La denuncia se agregó correctamente.";
                return result;
            }
            catch (Exception ex)
            {
                result = "Error al insertar " + ex.Message.ToString();
                return result;
            }
        }

        [WebMethod]
        public string updateDenuncia(string sTipo_Den, int iDNI, string sMail, string sNombre, string sApellido, int iN_Calle, string sEntre_Calle, string tObservaciones, decimal dLongitud, decimal dLatitud, int iId_Denuncia)
        {
            string result;
            try
            {
                this.Denuncia.Update(sTipo_Den, iDNI, sMail, sNombre, sApellido, iN_Calle, sEntre_Calle, tObservaciones, dLongitud, dLatitud, iId_Denuncia);
                result = "La denuncia se actualizó correctamente.";
                return result;
            }
            catch (Exception ex)
            {
                result = "Error al actualizar el registro " + ex.Message.ToString();
                return result;
            }
        }

        [WebMethod]
        public string deleteDenuncia(int iId_Denuncia)
        {
            string result;
            try
            {
                this.Denuncia.Delete(iId_Denuncia);
                result = "La denuncia se eliminó correctamente.";
                return result;
            }
            catch (Exception ex)
            {
                result = "Error al eliminar el registro " + ex.Message.ToString();
                return result;
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string getEstablecimientos()
        {
            //DataSet ds;
            this.Establecimiento.Fill(this.dsEstablecimientos.Establecimientos);

            return JsonConvert.SerializeObject(dsEstablecimientos, Formatting.Indented);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string authenticate(string UsuarioNombre, string Password) {

            string cs = ConfigurationManager.ConnectionStrings["OranDBConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Usuarios WHERE UsuarioNombre = '" + UsuarioNombre+"'", con);
                cmd.ExecuteNonQuery();

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet dsUsuario = new DataSet();
                da.Fill(dsUsuario);
                con.Close();

                return JsonConvert.SerializeObject(dsUsuario, Formatting.Indented);

            }
        }
    }
}
