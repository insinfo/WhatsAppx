using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Reflection;
using System.Diagnostics;

namespace WhatsAppx
{
    public class DBHandlerMySQL
    {
        public string Server { get; set; } = "localhost";//"127.0.0.1" localhost
        public string Port { get; set; } = "3306";
        public string User { get; set; } = "root";//"sisadmin";//"postgres"
        public string Password { get; set; } = "";//"Ins257257"
        public string Database { get; set; } = "whatsappx";
        private MySqlConnection Connection; //MySQL
        private MySqlDataAdapter mAdapter;
        private DataSet mDataSet; //MySQL

        public DBHandlerMySQL Connect()
        {
            //define o dataset
            mDataSet = new DataSet();
            //define string de conexao e cria a conexao
            Connection = new MySqlConnection("persistsecurityinfo=False;server=" +
                Server+";database="+ Database+";uid="+ User+";pwd="+Password+ ";SslMode=none"); 
            return this;
        }

        //Inserir registros
        public void InsertObj(object valueObject,string tableNameIn =null)
        {
            try
            {
                //abre a conexao
                Connection.Open();
                //string cmdInserir = $"Insert Into Noticias(nome,email,idade) values('{0}','{1}',{2})";
                var tableName = tableNameIn == null ? valueObject.GetType().Name.ToLower()+ "s"  : tableNameIn;
                string sql = $"Insert Into {tableName} (";
                PropertyInfo[] properties = valueObject.GetType().GetProperties();
                var values = "";
                foreach (var property in properties)
                {
                    //Console.WriteLine(property.Name);
                    sql += "" + property.Name.ToLower() + ",";
                    values += "'" + property.GetValue(valueObject).ToString().Replace("'", "\"") + "',";
                }
                sql = sql.Remove(sql.Length - 1);
                values = values.Remove(values.Length - 1);
                sql += ") values (" + values + ");";

                 Console.WriteLine(sql);
                //Verifica se a conexão está aberta
                if (Connection.State == ConnectionState.Open)
                {
                    //Se estiver aberta insere os dados na BD
                    MySqlCommand commS = new MySqlCommand(sql, Connection);
                    commS.BeginExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                //throw ex;
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                //throw ex;
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Connection.Close();
            }
        }

        public void UpdateObject(object valueObject,int id, string tableNameIn = null)
        {
            try
            {
                //abre a conexao
                Connection.Open();
                //string cmdInserir = $"Insert Into Noticias(nome,email,idade) values('{0}','{1}',{2})";
                var tableName = tableNameIn == null ? valueObject.GetType().Name.ToLower() + "s" : tableNameIn;
                string sql = $"UPDATE {tableName} SET ";
                PropertyInfo[] properties = valueObject.GetType().GetProperties();

                foreach (var property in properties)
                {
                    var prop = property.Name.ToLower();
                    var val = property.GetValue(valueObject).ToString();

                    if(prop == "enviou")
                    {
                        Debug.WriteLine(val);
                        val = val == "True" ? "1" : "2";
                        Debug.WriteLine(val);
                    }

                    if (prop != "id") {
                        sql += "" + prop + "='" + val + "',";
                    }
                }
                sql = sql.Remove(sql.Length - 1);                
                sql += " WHERE id='"+ id.ToString() +"'";

                Console.WriteLine(sql);
                
                //Se estiver aberta insere os dados na BD
                MySqlCommand commS = new MySqlCommand(sql, Connection);
                //commS.BeginExecuteNonQuery();
                var reseut =commS.ExecuteNonQuery();
                Console.WriteLine(reseut.ToString());
            }
            catch (MySqlException ex)
            {
                //throw ex;
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                //throw ex;
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Connection.Close();
            }
        }

        public List<Contato> GetContatos(string device)
        {
            try
            {
                var query = @"SELECT * FROM contatos WHERE device='"+device+"' and enviou='0'";
                Connection.Open();
                //verificva se a conexão esta aberta
                if (Connection.State == ConnectionState.Open)
                {
                    //cria um adapter usando a instrução SQL para acessar a tabela contatos
                    mAdapter = new MySqlDataAdapter(query, Connection);
                    //preenche o dataset via adapter
                    mAdapter.Fill(mDataSet, "contatos");
                    var dtsource = mDataSet.Tables["contatos"];
                    List<Contato> list = new List<Contato> ();
                    foreach (DataRow row in dtsource.Rows)
                    {
                        bool enviou = row["enviou"].ToString() == "1" ? true : false;
                        int id = Convert.ToInt32(row["id"]);
                        list.Add(new Contato (){Id=id, Nome= row["nome"].ToString().Trim(), Telefone= row["telefone"].ToString().Trim(), Device = row["device"].ToString(), Enviou = enviou });
                    }
                    return list;
                }
            }
            catch (MySqlException ex)
            {
                //throw ex;
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                //throw ex;
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Connection.Close();
            }
            return null;
        }

        public List<string> GetDevices()
        {
            try
            {
                var query = @"SELECT DISTINCT device FROM `contatos`";
                Connection.Open();
                //verificva se a conexão esta aberta
                if (Connection.State == ConnectionState.Open)
                {
                    //cria um adapter usando a instrução SQL para acessar a tabela contatos
                    mAdapter = new MySqlDataAdapter(query, Connection);
                    //preenche o dataset via adapter
                    mAdapter.Fill(mDataSet, "devices");
                    var dtsource = mDataSet.Tables["devices"];
                    List<string> list = new List<string>();
                    foreach (DataRow row in dtsource.Rows)
                    {                       
                        list.Add(row["device"].ToString());
                    }
                    return list;
                }
            }
            catch (MySqlException ex)
            {
                //throw ex;
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                //throw ex;
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Connection.Close();
            }
            return null;
        }

        public Mensagem GetLastMensagem()
        {
            try
            {
                var query = @"SELECT * FROM mensagens where publicada = 1 ORDER BY id desc limit 1";
                Connection.Open();
                //verificva se a conexão esta aberta
                if (Connection.State == ConnectionState.Open)
                {
                    //cria um adapter usando a instrução SQL para acessar a tabela contatos
                    mAdapter = new MySqlDataAdapter(query, Connection);
                    //preenche o dataset via adapter
                    mAdapter.Fill(mDataSet, "mensagens");
                    var dtsource = mDataSet.Tables["mensagens"];
                    if (dtsource.Rows.Count > 0) {
                        Mensagem mensagem = new Mensagem();
                        mensagem.Media = dtsource.Rows[0]["media"].ToString().Trim();
                        mensagem.Texto = dtsource.Rows[0]["texto"].ToString().Trim();
                        mensagem.Publicada = dtsource.Rows[0]["publicada"].ToString().Trim();
                        //Debug.WriteLine(mensagem.Texto);
                        return mensagem;
                    }  
                }
            }
            catch (MySqlException ex)
            {
                //throw ex;
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                //throw ex;
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Connection.Close();
            }
            return null;
        }
    }
}
