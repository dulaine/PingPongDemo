using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;
using Maticsoft.DBUtility;//Please add references
namespace Maticsoft
{
    /// <summary>
    /// 类DBround。
    /// </summary>
    [Serializable]
    public partial class DBround
    {
        public DBround()
        { }
        #region Model
        private string _id;
        private string _scoreid;
        private string _serveid;
        private DateTime _createtime;
        /// <summary>
        /// 
        /// </summary>
        public string ID
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string scoreID
        {
            set { _scoreid = value; }
            get { return _scoreid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string serveID
        {
            set { _serveid = value; }
            get { return _serveid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime createtime
        {
            set { _createtime = value; }
            get { return _createtime; }
        }
        #endregion Model


        #region  Method

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public DBround(string ID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,scoreID,serveID,createtime ");
            strSql.Append(" FROM to3d_nomode_round ");
            strSql.Append(" where ID=@ID ");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@ID", MySqlDbType.VarChar)};
            parameters[0].Value = ID;

            DataSet ds = DbHelperMySQL.Query(strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["ID"] != null)
                {
                    this.ID = ds.Tables[0].Rows[0]["ID"].ToString();
                }
                if (ds.Tables[0].Rows[0]["scoreID"] != null)
                {
                    this.scoreID = ds.Tables[0].Rows[0]["scoreID"].ToString();
                }
                if (ds.Tables[0].Rows[0]["serveID"] != null)
                {
                    this.serveID = ds.Tables[0].Rows[0]["serveID"].ToString();
                }
                if (ds.Tables[0].Rows[0]["createtime"] != null && ds.Tables[0].Rows[0]["createtime"].ToString() != "")
                {
                    this.createtime = DateTime.Parse(ds.Tables[0].Rows[0]["createtime"].ToString());
                }
            }
        }

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string ID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from to3d_nomode_round");
            strSql.Append(" where ID=@ID ");

            MySqlParameter[] parameters = {
                    new MySqlParameter("@ID", MySqlDbType.VarChar)};
            parameters[0].Value = ID;

            return DbHelperMySQL.Exists(strSql.ToString(), parameters);
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into to3d_nomode_round (");
            strSql.Append("ID,scoreID,serveID,createtime)");
            strSql.Append(" values (");
            strSql.Append("@ID,@scoreID,@serveID,@createtime)");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@ID", MySqlDbType.VarChar,50),
                    new MySqlParameter("@scoreID", MySqlDbType.VarChar,50),
                    new MySqlParameter("@serveID", MySqlDbType.VarChar,50),
                    new MySqlParameter("@createtime", MySqlDbType.DateTime)};
            parameters[0].Value = ID;
            parameters[1].Value = scoreID;
            parameters[2].Value = serveID;
            parameters[3].Value = createtime;

            DbHelperMySQL.ExecuteSql(strSql.ToString(), parameters);
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update to3d_nomode_round set ");
            strSql.Append("scoreID=@scoreID,");
            strSql.Append("serveID=@serveID,");
            strSql.Append("createtime=@createtime");
            strSql.Append(" where ID=@ID ");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@scoreID", MySqlDbType.VarChar,50),
                    new MySqlParameter("@serveID", MySqlDbType.VarChar,50),
                    new MySqlParameter("@createtime", MySqlDbType.DateTime),
                    new MySqlParameter("@ID", MySqlDbType.VarChar,50)};
            parameters[0].Value = scoreID;
            parameters[1].Value = serveID;
            parameters[2].Value = createtime;
            parameters[3].Value = ID;

            int rows = DbHelperMySQL.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string ID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from to3d_nomode_round ");
            strSql.Append(" where ID=@ID ");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@ID", MySqlDbType.VarChar)};
            parameters[0].Value = ID;

            int rows = DbHelperMySQL.ExecuteSql(strSql.ToString(), parameters);
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public void GetModel(string ID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,scoreID,serveID,createtime ");
            strSql.Append(" FROM to3d_nomode_round ");
            strSql.Append(" where ID=@ID ");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@ID", MySqlDbType.VarChar)};
            parameters[0].Value = ID;

            DataSet ds = DbHelperMySQL.Query(strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["ID"] != null)
                {
                    this.ID = ds.Tables[0].Rows[0]["ID"].ToString();
                }
                if (ds.Tables[0].Rows[0]["scoreID"] != null)
                {
                    this.scoreID = ds.Tables[0].Rows[0]["scoreID"].ToString();
                }
                if (ds.Tables[0].Rows[0]["serveID"] != null)
                {
                    this.serveID = ds.Tables[0].Rows[0]["serveID"].ToString();
                }
                if (ds.Tables[0].Rows[0]["createtime"] != null && ds.Tables[0].Rows[0]["createtime"].ToString() != "")
                {
                    this.createtime = DateTime.Parse(ds.Tables[0].Rows[0]["createtime"].ToString());
                }
            }
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * ");
            strSql.Append(" FROM to3d_nomode_round ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return DbHelperMySQL.Query(strSql.ToString());
        }




        #endregion  Method

        public List<DBround> Fill(DataSet ds)
        {
            List<DBround> list = new List<DBround>();
            int row = ds.Tables[0].Rows.Count;
            if (row > 0)
            {
                for (int i = 0; i < row; i++)
                {
                    DBround round = new DBround();

                    if (ds.Tables[0].Rows[i]["ID"] != null)
                    {
                        round.ID = ds.Tables[0].Rows[i]["ID"].ToString();
                    }
                    if (ds.Tables[0].Rows[i]["scoreID"] != null)
                    {
                        round.scoreID = ds.Tables[0].Rows[i]["scoreID"].ToString();
                    }
                    if (ds.Tables[0].Rows[i]["serveID"] != null)
                    {
                        round.serveID = ds.Tables[0].Rows[i]["serveID"].ToString();
                    }
                    if (ds.Tables[0].Rows[i]["createtime"] != null &&
                        ds.Tables[0].Rows[i]["createtime"].ToString() != "")
                    {
                        round.createtime = DateTime.Parse(ds.Tables[0].Rows[i]["createtime"].ToString());
                    }

                    list.Add(round);
                }

            }

            return list;
        }

        public DataSet GetLatest()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from  to3d_nomode_round where createtime =");
            strSql.Append("(select max(createtime) as createtime ");
            strSql.Append(" FROM to3d_nomode_round)");

            return DbHelperMySQL.Query(strSql.ToString());
        }
    }
}

