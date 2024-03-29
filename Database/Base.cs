﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.CRUD;
using System.Data.SqlClient;

namespace Database
{
    public class Base : IBase // IBase implementa a classe base com todos os seus métodos.Conceito de herança aplicado.
    {
        private string connectionString = ConfigurationManager.AppSettings["MYSQL"];
        public int Key 
        {
            get
            {
                foreach(PropertyInfo pi in this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)) 
                { 
                    OpcoesBase opcoes = (OpcoesBase)pi.GetCustomAttribute(typeof(OpcoesBase));
                    if(opcoes != null && opcoes.ChavePrimaria)
                    {
                        return Convert.ToInt32(pi.GetValue(this));
                    }
                }
                return 0;
            }
        }

        public List<IBase> Buscar()
        {
           var lista = new List<IBase>();   
           using(MySqlConnection connection = new MySqlConnection(connectionString))
           {
                List<string> where = new List<string>();
                string chaveprimaria = string.Empty;
                foreach(PropertyInfo pi in this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    OpcoesBase opcoesBase = (OpcoesBase)pi.GetCustomAttribute(typeof(OpcoesBase));
                    if (opcoesBase != null)
                    {
                        if (opcoesBase.ChavePrimaria)
                        {
                            chaveprimaria = pi.Name + "=" + pi.GetValue(this);
                        }
                        if(pi.GetValue(this) != null) 
                        {
                            if(tipoPropriedade(pi)=="varchar(255)" || tipoPropriedade(pi) == "datetime")
                            {
                                where.Add(pi.Name + "='" + pi.GetValue(this) + "'");
                            }
                            else
                            {
                                where.Add(pi.Name + "=" + pi.GetValue(this));
                            }
                        }
                    }
                }
                string sql;
                if(Key == 0)
                {
                    sql = "select * from " + this.GetType().Name + "s ";
                    if(where.Count > 0) 
                    {
                        sql += "where " + string.Join(" or ", where.ToArray());
                    }
                }
                else
                {
                    sql = "select * from " + this.GetType().Name + "s where " + chaveprimaria;
                }
                MySqlCommand mySql = new MySqlCommand(sql, connection);
                mySql.Connection.Open();
                MySqlDataReader reader = mySql.ExecuteReader();
                while(reader.Read()) 
                { 
                    var obj = (IBase)Activator.CreateInstance(this.GetType());
                    foreach(PropertyInfo info in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)) 
                    { 
                        OpcoesBase opcoes = (OpcoesBase)info.GetCustomAttribute(typeof(OpcoesBase));
                        if(opcoes != null)
                        {
                            info.SetValue(obj, reader[info.Name]);
                        }
                        lista.Add(obj);
                    }
                }
                mySql.Connection.Close();
           }
           return lista;
        }
        private string tipoPropriedade(PropertyInfo pi)
        {
            switch (pi.PropertyType.Name)
            {
                case "Int32":
                    return "int";
                case "Int64":
                    return "bigint";
                case "double":
                    return "decimal(9,2)";
                case "Datetime":
                    return "datetime";
                default:
                    return "varchar(255)";
            }
        }

        public void CriarTabela()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                List<string> campos = new List<string>();
                string chavePrimaria = "";
                foreach(PropertyInfo pi in this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    OpcoesBase opcoesBase = (OpcoesBase)pi.GetCustomAttribute(typeof(OpcoesBase));
                    if(opcoesBase != null && opcoesBase.UsaBD)
                    {
                        if (opcoesBase.ChavePrimaria)
                        {
                            chavePrimaria = pi.Name + " int auto_increment primary key";
                        }
                        else
                        {
                            campos.Add(pi.Name + " " + tipoPropriedade(pi) + " ");
                        }
                    }
                }
                string sql = "create table if not exists " + this.GetType().Name + "s (";
                sql += chavePrimaria + ", ";
                sql += string.Join(", ", campos.ToArray());
                sql += ");";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
        }

        public void Excluir()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql="delete from "+this.GetType().Name+"s where id="+this.Key + ";";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
        }
        public void Salvar()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                List<string> campos = new List<string>();
                List<string> valores = new List<string>();
                foreach(PropertyInfo pi in this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    OpcoesBase opcoes = (OpcoesBase)pi.GetCustomAttribute(typeof(OpcoesBase));
                    if(opcoes != null && opcoes.UsaBD && !opcoes.ChavePrimaria)
                    {
                        if(this.Key == 0)
                        {
                            campos.Add(pi.Name);
                            valores.Add("'" + pi.GetValue(this) + "'"); 
                        }
                        else
                        {
                            valores.Add(" " + pi.Name + "='" + pi.GetValue(this) + "'");
                        }
                    }
                }
                string sql = "";
                if(this.Key == 0)
                {
                    sql = "insert into " + this.GetType().Name + "s (";
                    sql += string.Join(", ", campos.ToArray()) + ")";
                    sql += " values (" + string.Join(", ", valores.ToArray()) + ")";
                }
                else
                {
                    sql = "update " + this.GetType().Name + "s set " + string.Join(", ", valores.ToArray()) + " where id=" + this.Key;
                }
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
       }    }

        public List<IBase> Todos()
        {
            var lista = new List<IBase>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = "select * from " + this.GetType().Name + "s ";
                MySqlCommand mySql = new MySqlCommand(sql, connection);
                mySql.Connection.Open();
                MySqlDataReader reader = mySql.ExecuteReader();
                while (reader.Read())
                {
                    var obj = (IBase)Activator.CreateInstance(this.GetType());
                    foreach (PropertyInfo info in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        OpcoesBase opcoes = (OpcoesBase)info.GetCustomAttribute(typeof(OpcoesBase));
                        if (opcoes != null)
                        {
                            info.SetValue(obj, reader[info.Name]);
                        }
                        lista.Add(obj);
                    }
                }
                mySql.Connection.Close();
            }
            return lista;
        }
    }
}
