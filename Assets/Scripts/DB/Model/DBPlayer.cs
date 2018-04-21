
using System;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;
using Maticsoft.DBUtility;//Please add references

namespace Maticsoft
{
    /// <summary>
    /// 类to3d_nomode_players。
    /// </summary>
    [Serializable]
    public partial class DBPlayer
    {
        public DBPlayer()
        { }
        #region Model
        private string _id;
        private string _playerid;
        private string _playername;
        private int _direction;
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
        public string playerID
        {
            set { _playerid = value; }
            get { return _playerid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string playerName
        {
            set { _playername = value; }
            get { return _playername; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int direction
        {
            set { _direction = value; }
            get { return _direction; }
        }
        #endregion Model


        #region  Method

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public DBPlayer(string ID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,playerID,playerName,direction ");
            strSql.Append(" FROM to3d_nomode_players ");
            strSql.Append(" where playerID=@ID ");
            //strSql.Append(" where ID=@ID ");
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
                if (ds.Tables[0].Rows[0]["playerID"] != null)
                {
                    this.playerID = ds.Tables[0].Rows[0]["playerID"].ToString();
                }
                if (ds.Tables[0].Rows[0]["playerName"] != null)
                {
                    this.playerName = ds.Tables[0].Rows[0]["playerName"].ToString();
                }
                if (ds.Tables[0].Rows[0]["direction"] != null && ds.Tables[0].Rows[0]["direction"].ToString() != "")
                {
                    this.direction = int.Parse(ds.Tables[0].Rows[0]["direction"].ToString());
                }
            }
        }

        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string ID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from to3d_nomode_players");
            strSql.Append(" where playerID=@ID ");

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
            strSql.Append("insert into to3d_nomode_players (");
            strSql.Append("ID,playerID,playerName,direction)");
            strSql.Append(" values (");
            strSql.Append("@ID,@playerID,@playerName,@direction)");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@ID", MySqlDbType.VarChar,50),
                    new MySqlParameter("@playerID", MySqlDbType.VarChar,50),
                    new MySqlParameter("@playerName", MySqlDbType.VarChar,50),
                    new MySqlParameter("@direction", MySqlDbType.Int32,11)};
            parameters[0].Value = ID;
            parameters[1].Value = playerID;
            parameters[2].Value = playerName;
            parameters[3].Value = direction;

            DbHelperMySQL.ExecuteSql(strSql.ToString(), parameters);
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update to3d_nomode_players set ");
            strSql.Append("playerID=@playerID,");
            strSql.Append("playerName=@playerName,");
            strSql.Append("direction=@direction");
            strSql.Append(" where ID=@ID ");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@playerID", MySqlDbType.VarChar,50),
                    new MySqlParameter("@playerName", MySqlDbType.VarChar,50),
                    new MySqlParameter("@direction", MySqlDbType.Int32,11),
                    new MySqlParameter("@ID", MySqlDbType.VarChar,50)};
            parameters[0].Value = playerID;
            parameters[1].Value = playerName;
            parameters[2].Value = direction;
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
            strSql.Append("delete from to3d_nomode_players ");
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
            strSql.Append("select ID,playerID,playerName,direction ");
            strSql.Append(" FROM to3d_nomode_players ");
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
                if (ds.Tables[0].Rows[0]["playerID"] != null)
                {
                    this.playerID = ds.Tables[0].Rows[0]["playerID"].ToString();
                }
                if (ds.Tables[0].Rows[0]["playerName"] != null)
                {
                    this.playerName = ds.Tables[0].Rows[0]["playerName"].ToString();
                }
                if (ds.Tables[0].Rows[0]["direction"] != null && ds.Tables[0].Rows[0]["direction"].ToString() != "")
                {
                    this.direction = int.Parse(ds.Tables[0].Rows[0]["direction"].ToString());
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
            strSql.Append(" FROM to3d_nomode_players ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return DbHelperMySQL.Query(strSql.ToString());
        }

        #endregion  Method
    }
}


