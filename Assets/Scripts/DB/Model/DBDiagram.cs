using System;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;
using Maticsoft.DBUtility;//Please add references
namespace Maticsoft
{
    /// <summary>
    /// 类DBDiagram。
    /// </summary>
    [Serializable]
    public partial class DBDiagram
    {
        public DBDiagram()
        { }
        #region Model
        private string _id;
        private string _imgurl;
        private byte[] _img;
        private int _imgtype;
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
        public string imgurl
        {
            set { _imgurl = value; }
            get { return _imgurl; }
        }
        /// <summary>
        /// 
        /// </summary>
        public byte[] img
        {
            set { _img = value; }
            get { return _img; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int imgType
        {
            set { _imgtype = value; }
            get { return _imgtype; }
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
        public DBDiagram(string ID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ID,imgurl,img,imgType,createtime ");
            strSql.Append(" FROM to3d_nomode_diagram ");
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
                if (ds.Tables[0].Rows[0]["imgurl"] != null)
                {
                    this.imgurl = ds.Tables[0].Rows[0]["imgurl"].ToString();
                }
                this.img = (byte[])ds.Tables[0].Rows[0]["img"];
                if (ds.Tables[0].Rows[0]["imgType"] != null && ds.Tables[0].Rows[0]["imgType"].ToString() != "")
                {
                    this.imgType = int.Parse(ds.Tables[0].Rows[0]["imgType"].ToString());
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
            strSql.Append("select count(1) from to3d_nomode_diagram");
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
            strSql.Append("insert into to3d_nomode_diagram (");
            strSql.Append("ID,imgurl,img,imgType,createtime)");
            strSql.Append(" values (");
            strSql.Append("@ID,@imgurl,@img,@imgType,@createtime)");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@ID", MySqlDbType.VarChar,50),
                    new MySqlParameter("@imgurl", MySqlDbType.VarChar,50),
                    new MySqlParameter("@img", MySqlDbType.LongBlob),
                    new MySqlParameter("@imgType", MySqlDbType.Int32,11),
                    new MySqlParameter("@createtime", MySqlDbType.DateTime)};
            parameters[0].Value = ID;
            parameters[1].Value = imgurl;
            parameters[2].Value = img;
            parameters[3].Value = imgType;
            parameters[4].Value = createtime;

            DbHelperMySQL.ExecuteSql(strSql.ToString(), parameters);
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update to3d_nomode_diagram set ");
            strSql.Append("imgurl=@imgurl,");
            strSql.Append("img=@img,");
            strSql.Append("imgType=@imgType,");
            strSql.Append("createtime=@createtime");
            strSql.Append(" where ID=@ID ");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@imgurl", MySqlDbType.VarChar,50),
                    new MySqlParameter("@img", MySqlDbType.LongBlob),
                    new MySqlParameter("@imgType", MySqlDbType.Int32,11),
                    new MySqlParameter("@createtime", MySqlDbType.DateTime),
                    new MySqlParameter("@ID", MySqlDbType.VarChar,50)};
            parameters[0].Value = imgurl;
            parameters[1].Value = img;
            parameters[2].Value = imgType;
            parameters[3].Value = createtime;
            parameters[4].Value = ID;

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
            strSql.Append("delete from to3d_nomode_diagram ");
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
            strSql.Append("select ID,imgurl,img,imgType,createtime ");
            strSql.Append(" FROM to3d_nomode_diagram ");
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
                if (ds.Tables[0].Rows[0]["imgurl"] != null)
                {
                    this.imgurl = ds.Tables[0].Rows[0]["imgurl"].ToString();
                }
                //this.img=ds.Tables[0].Rows[0]["img"].ToString();
                if (ds.Tables[0].Rows[0]["imgType"] != null && ds.Tables[0].Rows[0]["imgType"].ToString() != "")
                {
                    this.imgType = int.Parse(ds.Tables[0].Rows[0]["imgType"].ToString());
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
            strSql.Append(" FROM to3d_nomode_diagram ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return DbHelperMySQL.Query(strSql.ToString());
        }

        #endregion  Method
    }
}

