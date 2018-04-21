using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using MySql.Data.MySqlClient;
using Maticsoft.DBUtility;//Please add references
namespace Maticsoft
{
    /// <summary>
    /// 类DBTrack。
    /// </summary>
    [Serializable]
    public partial class DBTrack
    {
        public DBTrack()
        { }
        #region Model
        private int _index;
        private string _id;
        private string _roundid;
        private string _touchdown_p1;
        private string _touchdown_p2;
        private decimal? _maxspeed;
        private decimal? _maxrotatespeed;
        private int? _xrotatespeed;
        private int? _yrotatespeed;
        private int? _direction;
        private decimal? _overnetheight;
        private string _tracestring;
        private decimal? _shadowlenght;
        private decimal? _shadowwidth;
        private decimal? _shadowangle;
        private DateTime _createtime;
        private int? _trackresult;
        private string _ownerid;
        /// <summary>
        /// auto_increment
        /// </summary>
        public int Index
        {
            set { _index = value; }
            get { return _index; }
        }
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
        public string roundID
        {
            set { _roundid = value; }
            get { return _roundid; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string touchdown_p1
        {
            set { _touchdown_p1 = value; }
            get { return _touchdown_p1; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string touchdown_p2
        {
            set { _touchdown_p2 = value; }
            get { return _touchdown_p2; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? maxSpeed
        {
            set { _maxspeed = value; }
            get { return _maxspeed; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? maxRotateSpeed
        {
            set { _maxrotatespeed = value; }
            get { return _maxrotatespeed; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? xRotateSpeed
        {
            set { _xrotatespeed = value; }
            get { return _xrotatespeed; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? yRotateSpeed
        {
            set { _yrotatespeed = value; }
            get { return _yrotatespeed; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? direction
        {
            set { _direction = value; }
            get { return _direction; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? overNetHeight
        {
            set { _overnetheight = value; }
            get { return _overnetheight; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string traceString
        {
            set { _tracestring = value; }
            get { return _tracestring; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? shadowLenght
        {
            set { _shadowlenght = value; }
            get { return _shadowlenght; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? shadowWidth
        {
            set { _shadowwidth = value; }
            get { return _shadowwidth; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? shadowAngle
        {
            set { _shadowangle = value; }
            get { return _shadowangle; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime createtime
        {
            set { _createtime = value; }
            get { return _createtime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? trackResult
        {
            set { _trackresult = value; }
            get { return _trackresult; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string ownerID
        {
            set { _ownerid = value; }
            get { return _ownerid; }
        }
        #endregion Model


        #region  Method

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public DBTrack(string ID)
        {
            //StringBuilder strSql = new StringBuilder();
            ////strSql.Append("select Index,ID,roundID,touchdown_p1,touchdown_p2,maxSpeed,maxRotateSpeed,xRotateSpeed,yRotateSpeed,direction,overNetHeight,traceString,shadowLenght,shadowWidth,shadowAngle,createtime,trackResult,ownerID ");
            //strSql.Append("select * ");
            //strSql.Append(" FROM to3d_nomode_track ");
            //strSql.Append(" where ID=@ID ");
            ////strSql.Append(" where Index=@Index ");
            //MySqlParameter[] parameters = {
            // new MySqlParameter("@ID", MySqlDbType.VarChar,50)};
            ////new MySqlParameter("@Index", MySqlDbType.Int32)};
            //parameters[0].Value = ID;

            //DataSet ds = DbHelperMySQL.Query(strSql.ToString(), parameters);
            DataSet ds = DbHelperMySQL.Query("select * from to3d_nomode_track where ID = \"" + ID + "\"");
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["Index"] != null && ds.Tables[0].Rows[0]["Index"].ToString() != "")
                {
                    this.Index = int.Parse(ds.Tables[0].Rows[0]["Index"].ToString());
                }
                if (ds.Tables[0].Rows[0]["ID"] != null)
                {
                    this.ID = ds.Tables[0].Rows[0]["ID"].ToString();
                }
                if (ds.Tables[0].Rows[0]["roundID"] != null)
                {
                    this.roundID = ds.Tables[0].Rows[0]["roundID"].ToString();
                }
                if (ds.Tables[0].Rows[0]["touchdown_p1"] != null)
                {
                    this.touchdown_p1 = ds.Tables[0].Rows[0]["touchdown_p1"].ToString();
                }
                if (ds.Tables[0].Rows[0]["touchdown_p2"] != null)
                {
                    this.touchdown_p2 = ds.Tables[0].Rows[0]["touchdown_p2"].ToString();
                }
                if (ds.Tables[0].Rows[0]["maxSpeed"] != null && ds.Tables[0].Rows[0]["maxSpeed"].ToString() != "")
                {
                    this.maxSpeed = decimal.Parse(ds.Tables[0].Rows[0]["maxSpeed"].ToString());
                }
                if (ds.Tables[0].Rows[0]["maxRotateSpeed"] != null && ds.Tables[0].Rows[0]["maxRotateSpeed"].ToString() != "")
                {
                    this.maxRotateSpeed = decimal.Parse(ds.Tables[0].Rows[0]["maxRotateSpeed"].ToString());
                }
                if (ds.Tables[0].Rows[0]["xRotateSpeed"] != null && ds.Tables[0].Rows[0]["xRotateSpeed"].ToString() != "")
                {
                    this.xRotateSpeed = int.Parse(ds.Tables[0].Rows[0]["xRotateSpeed"].ToString());
                }
                if (ds.Tables[0].Rows[0]["yRotateSpeed"] != null && ds.Tables[0].Rows[0]["yRotateSpeed"].ToString() != "")
                {
                    this.yRotateSpeed = int.Parse(ds.Tables[0].Rows[0]["yRotateSpeed"].ToString());
                }
                if (ds.Tables[0].Rows[0]["direction"] != null && ds.Tables[0].Rows[0]["direction"].ToString() != "")
                {
                    this.direction = int.Parse(ds.Tables[0].Rows[0]["direction"].ToString());
                }
                if (ds.Tables[0].Rows[0]["overNetHeight"] != null && ds.Tables[0].Rows[0]["overNetHeight"].ToString() != "")
                {
                    this.overNetHeight = decimal.Parse(ds.Tables[0].Rows[0]["overNetHeight"].ToString());
                }
                if (ds.Tables[0].Rows[0]["traceString"] != null)
                {
                    this.traceString = ds.Tables[0].Rows[0]["traceString"].ToString();
                }
                if (ds.Tables[0].Rows[0]["shadowLenght"] != null && ds.Tables[0].Rows[0]["shadowLenght"].ToString() != "")
                {
                    this.shadowLenght = decimal.Parse(ds.Tables[0].Rows[0]["shadowLenght"].ToString());
                }
                if (ds.Tables[0].Rows[0]["shadowWidth"] != null && ds.Tables[0].Rows[0]["shadowWidth"].ToString() != "")
                {
                    this.shadowWidth = decimal.Parse(ds.Tables[0].Rows[0]["shadowWidth"].ToString());
                }
                if (ds.Tables[0].Rows[0]["shadowAngle"] != null && ds.Tables[0].Rows[0]["shadowAngle"].ToString() != "")
                {
                    this.shadowAngle = decimal.Parse(ds.Tables[0].Rows[0]["shadowAngle"].ToString());
                }
                if (ds.Tables[0].Rows[0]["createtime"] != null && ds.Tables[0].Rows[0]["createtime"].ToString() != "")
                {
                    this.createtime = DateTime.Parse(ds.Tables[0].Rows[0]["createtime"].ToString());
                }
                if (ds.Tables[0].Rows[0]["trackResult"] != null && ds.Tables[0].Rows[0]["trackResult"].ToString() != "")
                {
                    this.trackResult = int.Parse(ds.Tables[0].Rows[0]["trackResult"].ToString());
                }
                if (ds.Tables[0].Rows[0]["ownerID"] != null)
                {
                    this.ownerID = ds.Tables[0].Rows[0]["ownerID"].ToString();
                }
            }
        }
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int Index)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from to3d_nomode_track");
            strSql.Append(" where Index=@Index ");

            MySqlParameter[] parameters = {
                    new MySqlParameter("@Index", MySqlDbType.Int32)};
            parameters[0].Value = Index;

            return DbHelperMySQL.Exists(strSql.ToString(), parameters);
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public void Add()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into to3d_nomode_track (");
            strSql.Append("ID,roundID,touchdown_p1,touchdown_p2,maxSpeed,maxRotateSpeed,xRotateSpeed,yRotateSpeed,direction,overNetHeight,traceString,shadowLenght,shadowWidth,shadowAngle,createtime,trackResult,ownerID)");
            strSql.Append(" values (");
            strSql.Append("@ID,@roundID,@touchdown_p1,@touchdown_p2,@maxSpeed,@maxRotateSpeed,@xRotateSpeed,@yRotateSpeed,@direction,@overNetHeight,@traceString,@shadowLenght,@shadowWidth,@shadowAngle,@createtime,@trackResult,@ownerID)");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@ID", MySqlDbType.VarChar,50),
                    new MySqlParameter("@roundID", MySqlDbType.VarChar,50),
                    new MySqlParameter("@touchdown_p1", MySqlDbType.VarChar,50),
                    new MySqlParameter("@touchdown_p2", MySqlDbType.VarChar,50),
                    new MySqlParameter("@maxSpeed", MySqlDbType.Float),
                    new MySqlParameter("@maxRotateSpeed", MySqlDbType.Float),
                    new MySqlParameter("@xRotateSpeed", MySqlDbType.Int32,11),
                    new MySqlParameter("@yRotateSpeed", MySqlDbType.Int32,11),
                    new MySqlParameter("@direction", MySqlDbType.Int32,11),
                    new MySqlParameter("@overNetHeight", MySqlDbType.Float),
                    new MySqlParameter("@traceString", MySqlDbType.LongText),
                    new MySqlParameter("@shadowLenght", MySqlDbType.Float),
                    new MySqlParameter("@shadowWidth", MySqlDbType.Float),
                    new MySqlParameter("@shadowAngle", MySqlDbType.Float),
                    new MySqlParameter("@createtime", MySqlDbType.DateTime),
                    new MySqlParameter("@trackResult", MySqlDbType.Int32,11),
                    new MySqlParameter("@ownerID", MySqlDbType.VarChar,50)};
            parameters[0].Value = ID;
            parameters[1].Value = roundID;
            parameters[2].Value = touchdown_p1;
            parameters[3].Value = touchdown_p2;
            parameters[4].Value = maxSpeed;
            parameters[5].Value = maxRotateSpeed;
            parameters[6].Value = xRotateSpeed;
            parameters[7].Value = yRotateSpeed;
            parameters[8].Value = direction;
            parameters[9].Value = overNetHeight;
            parameters[10].Value = traceString;
            parameters[11].Value = shadowLenght;
            parameters[12].Value = shadowWidth;
            parameters[13].Value = shadowAngle;
            parameters[14].Value = createtime;
            parameters[15].Value = trackResult;
            parameters[16].Value = ownerID;

            DbHelperMySQL.ExecuteSql(strSql.ToString(), parameters);
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update to3d_nomode_track set ");
            strSql.Append("ID=@ID,");
            strSql.Append("roundID=@roundID,");
            strSql.Append("touchdown_p1=@touchdown_p1,");
            strSql.Append("touchdown_p2=@touchdown_p2,");
            strSql.Append("maxSpeed=@maxSpeed,");
            strSql.Append("maxRotateSpeed=@maxRotateSpeed,");
            strSql.Append("xRotateSpeed=@xRotateSpeed,");
            strSql.Append("yRotateSpeed=@yRotateSpeed,");
            strSql.Append("direction=@direction,");
            strSql.Append("overNetHeight=@overNetHeight,");
            strSql.Append("traceString=@traceString,");
            strSql.Append("shadowLenght=@shadowLenght,");
            strSql.Append("shadowWidth=@shadowWidth,");
            strSql.Append("shadowAngle=@shadowAngle,");
            strSql.Append("createtime=@createtime,");
            strSql.Append("trackResult=@trackResult,");
            strSql.Append("ownerID=@ownerID");
            strSql.Append(" where Index=@Index ");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@ID", MySqlDbType.VarChar,50),
                    new MySqlParameter("@roundID", MySqlDbType.VarChar,50),
                    new MySqlParameter("@touchdown_p1", MySqlDbType.VarChar,50),
                    new MySqlParameter("@touchdown_p2", MySqlDbType.VarChar,50),
                    new MySqlParameter("@maxSpeed", MySqlDbType.Float),
                    new MySqlParameter("@maxRotateSpeed", MySqlDbType.Float),
                    new MySqlParameter("@xRotateSpeed", MySqlDbType.Int32,11),
                    new MySqlParameter("@yRotateSpeed", MySqlDbType.Int32,11),
                    new MySqlParameter("@direction", MySqlDbType.Int32,11),
                    new MySqlParameter("@overNetHeight", MySqlDbType.Float),
                    new MySqlParameter("@traceString", MySqlDbType.LongText),
                    new MySqlParameter("@shadowLenght", MySqlDbType.Float),
                    new MySqlParameter("@shadowWidth", MySqlDbType.Float),
                    new MySqlParameter("@shadowAngle", MySqlDbType.Float),
                    new MySqlParameter("@createtime", MySqlDbType.DateTime),
                    new MySqlParameter("@trackResult", MySqlDbType.Int32,11),
                    new MySqlParameter("@ownerID", MySqlDbType.VarChar,50),
                    new MySqlParameter("@Index", MySqlDbType.Int32,11)};
            parameters[0].Value = ID;
            parameters[1].Value = roundID;
            parameters[2].Value = touchdown_p1;
            parameters[3].Value = touchdown_p2;
            parameters[4].Value = maxSpeed;
            parameters[5].Value = maxRotateSpeed;
            parameters[6].Value = xRotateSpeed;
            parameters[7].Value = yRotateSpeed;
            parameters[8].Value = direction;
            parameters[9].Value = overNetHeight;
            parameters[10].Value = traceString;
            parameters[11].Value = shadowLenght;
            parameters[12].Value = shadowWidth;
            parameters[13].Value = shadowAngle;
            parameters[14].Value = createtime;
            parameters[15].Value = trackResult;
            parameters[16].Value = ownerID;
            parameters[17].Value = Index;

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
        public bool Delete(int Index)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from to3d_nomode_track ");
            strSql.Append(" where Index=@Index ");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@Index", MySqlDbType.Int32)};
            parameters[0].Value = Index;

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
        public void GetModel(int Index)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select Index,ID,roundID,touchdown_p1,touchdown_p2,maxSpeed,maxRotateSpeed,xRotateSpeed,yRotateSpeed,direction,overNetHeight,traceString,shadowLenght,shadowWidth,shadowAngle,createtime,trackResult,ownerID ");
            strSql.Append(" FROM to3d_nomode_track ");
            strSql.Append(" where Index=@Index ");
            MySqlParameter[] parameters = {
                    new MySqlParameter("@Index", MySqlDbType.Int32)};
            parameters[0].Value = Index;

            DataSet ds = DbHelperMySQL.Query(strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["Index"] != null && ds.Tables[0].Rows[0]["Index"].ToString() != "")
                {
                    this.Index = int.Parse(ds.Tables[0].Rows[0]["Index"].ToString());
                }
                if (ds.Tables[0].Rows[0]["ID"] != null)
                {
                    this.ID = ds.Tables[0].Rows[0]["ID"].ToString();
                }
                if (ds.Tables[0].Rows[0]["roundID"] != null)
                {
                    this.roundID = ds.Tables[0].Rows[0]["roundID"].ToString();
                }
                if (ds.Tables[0].Rows[0]["touchdown_p1"] != null)
                {
                    this.touchdown_p1 = ds.Tables[0].Rows[0]["touchdown_p1"].ToString();
                }
                if (ds.Tables[0].Rows[0]["touchdown_p2"] != null)
                {
                    this.touchdown_p2 = ds.Tables[0].Rows[0]["touchdown_p2"].ToString();
                }
                if (ds.Tables[0].Rows[0]["maxSpeed"] != null && ds.Tables[0].Rows[0]["maxSpeed"].ToString() != "")
                {
                    this.maxSpeed = decimal.Parse(ds.Tables[0].Rows[0]["maxSpeed"].ToString());
                }
                if (ds.Tables[0].Rows[0]["maxRotateSpeed"] != null && ds.Tables[0].Rows[0]["maxRotateSpeed"].ToString() != "")
                {
                    this.maxRotateSpeed = decimal.Parse(ds.Tables[0].Rows[0]["maxRotateSpeed"].ToString());
                }
                if (ds.Tables[0].Rows[0]["xRotateSpeed"] != null && ds.Tables[0].Rows[0]["xRotateSpeed"].ToString() != "")
                {
                    this.xRotateSpeed = int.Parse(ds.Tables[0].Rows[0]["xRotateSpeed"].ToString());
                }
                if (ds.Tables[0].Rows[0]["yRotateSpeed"] != null && ds.Tables[0].Rows[0]["yRotateSpeed"].ToString() != "")
                {
                    this.yRotateSpeed = int.Parse(ds.Tables[0].Rows[0]["yRotateSpeed"].ToString());
                }
                if (ds.Tables[0].Rows[0]["direction"] != null && ds.Tables[0].Rows[0]["direction"].ToString() != "")
                {
                    this.direction = int.Parse(ds.Tables[0].Rows[0]["direction"].ToString());
                }
                if (ds.Tables[0].Rows[0]["overNetHeight"] != null && ds.Tables[0].Rows[0]["overNetHeight"].ToString() != "")
                {
                    this.overNetHeight = decimal.Parse(ds.Tables[0].Rows[0]["overNetHeight"].ToString());
                }
                if (ds.Tables[0].Rows[0]["traceString"] != null)
                {
                    this.traceString = ds.Tables[0].Rows[0]["traceString"].ToString();
                }
                if (ds.Tables[0].Rows[0]["shadowLenght"] != null && ds.Tables[0].Rows[0]["shadowLenght"].ToString() != "")
                {
                    this.shadowLenght = decimal.Parse(ds.Tables[0].Rows[0]["shadowLenght"].ToString());
                }
                if (ds.Tables[0].Rows[0]["shadowWidth"] != null && ds.Tables[0].Rows[0]["shadowWidth"].ToString() != "")
                {
                    this.shadowWidth = decimal.Parse(ds.Tables[0].Rows[0]["shadowWidth"].ToString());
                }
                if (ds.Tables[0].Rows[0]["shadowAngle"] != null && ds.Tables[0].Rows[0]["shadowAngle"].ToString() != "")
                {
                    this.shadowAngle = decimal.Parse(ds.Tables[0].Rows[0]["shadowAngle"].ToString());
                }
                if (ds.Tables[0].Rows[0]["createtime"] != null && ds.Tables[0].Rows[0]["createtime"].ToString() != "")
                {
                    this.createtime = DateTime.Parse(ds.Tables[0].Rows[0]["createtime"].ToString());
                }
                if (ds.Tables[0].Rows[0]["trackResult"] != null && ds.Tables[0].Rows[0]["trackResult"].ToString() != "")
                {
                    this.trackResult = int.Parse(ds.Tables[0].Rows[0]["trackResult"].ToString());
                }
                if (ds.Tables[0].Rows[0]["ownerID"] != null)
                {
                    this.ownerID = ds.Tables[0].Rows[0]["ownerID"].ToString();
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
            strSql.Append(" FROM to3d_nomode_track ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return DbHelperMySQL.Query(strSql.ToString());
        }

        #endregion  Method
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public DBTrack(int ID)
        {
            //DataSet ds = DbHelperMySQL.Query(strSql.ToString(), parameters);

            //StringBuilder strSql = new StringBuilder();
            //strSql.Append("select * from to3d_nomode_track");
            //strSql.Append(" where Index=@Index ");

            //MySqlParameter[] parameters = {
            //    new MySqlParameter("@Index", MySqlDbType.Int32)};
            //parameters[0].Value = ID;
            //DataSet ds = DbHelperMySQL.Query(strSql.ToString(), parameters);
            DataSet ds = DbHelperMySQL.Query( "select * from to3d_nomode_track where `Index` = " + ID);
            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["Index"] != null && ds.Tables[0].Rows[0]["Index"].ToString() != "")
                {
                    this.Index = int.Parse(ds.Tables[0].Rows[0]["Index"].ToString());
                }
                if (ds.Tables[0].Rows[0]["ID"] != null)
                {
                    this.ID = ds.Tables[0].Rows[0]["ID"].ToString();
                }
                if (ds.Tables[0].Rows[0]["roundID"] != null)
                {
                    this.roundID = ds.Tables[0].Rows[0]["roundID"].ToString();
                }
                if (ds.Tables[0].Rows[0]["touchdown_p1"] != null)
                {
                    this.touchdown_p1 = ds.Tables[0].Rows[0]["touchdown_p1"].ToString();
                }
                if (ds.Tables[0].Rows[0]["touchdown_p2"] != null)
                {
                    this.touchdown_p2 = ds.Tables[0].Rows[0]["touchdown_p2"].ToString();
                }
                if (ds.Tables[0].Rows[0]["maxSpeed"] != null && ds.Tables[0].Rows[0]["maxSpeed"].ToString() != "")
                {
                    this.maxSpeed = decimal.Parse(ds.Tables[0].Rows[0]["maxSpeed"].ToString());
                }
                if (ds.Tables[0].Rows[0]["maxRotateSpeed"] != null && ds.Tables[0].Rows[0]["maxRotateSpeed"].ToString() != "")
                {
                    this.maxRotateSpeed = decimal.Parse(ds.Tables[0].Rows[0]["maxRotateSpeed"].ToString());
                }
                if (ds.Tables[0].Rows[0]["xRotateSpeed"] != null && ds.Tables[0].Rows[0]["xRotateSpeed"].ToString() != "")
                {
                    this.xRotateSpeed = int.Parse(ds.Tables[0].Rows[0]["xRotateSpeed"].ToString());
                }
                if (ds.Tables[0].Rows[0]["yRotateSpeed"] != null && ds.Tables[0].Rows[0]["yRotateSpeed"].ToString() != "")
                {
                    this.yRotateSpeed = int.Parse(ds.Tables[0].Rows[0]["yRotateSpeed"].ToString());
                }
                if (ds.Tables[0].Rows[0]["direction"] != null && ds.Tables[0].Rows[0]["direction"].ToString() != "")
                {
                    this.direction = int.Parse(ds.Tables[0].Rows[0]["direction"].ToString());
                }
                if (ds.Tables[0].Rows[0]["overNetHeight"] != null && ds.Tables[0].Rows[0]["overNetHeight"].ToString() != "")
                {
                    this.overNetHeight = decimal.Parse(ds.Tables[0].Rows[0]["overNetHeight"].ToString());
                }
                if (ds.Tables[0].Rows[0]["traceString"] != null)
                {
                    this.traceString = ds.Tables[0].Rows[0]["traceString"].ToString();
                }
                if (ds.Tables[0].Rows[0]["shadowLenght"] != null && ds.Tables[0].Rows[0]["shadowLenght"].ToString() != "")
                {
                    this.shadowLenght = decimal.Parse(ds.Tables[0].Rows[0]["shadowLenght"].ToString());
                }
                if (ds.Tables[0].Rows[0]["shadowWidth"] != null && ds.Tables[0].Rows[0]["shadowWidth"].ToString() != "")
                {
                    this.shadowWidth = decimal.Parse(ds.Tables[0].Rows[0]["shadowWidth"].ToString());
                }
                if (ds.Tables[0].Rows[0]["shadowAngle"] != null && ds.Tables[0].Rows[0]["shadowAngle"].ToString() != "")
                {
                    this.shadowAngle = decimal.Parse(ds.Tables[0].Rows[0]["shadowAngle"].ToString());
                }
                if (ds.Tables[0].Rows[0]["createtime"] != null && ds.Tables[0].Rows[0]["createtime"].ToString() != "")
                {
                    this.createtime = DateTime.Parse(ds.Tables[0].Rows[0]["createtime"].ToString());
                }
                if (ds.Tables[0].Rows[0]["trackResult"] != null && ds.Tables[0].Rows[0]["trackResult"].ToString() != "")
                {
                    this.trackResult = int.Parse(ds.Tables[0].Rows[0]["trackResult"].ToString());
                }
                if (ds.Tables[0].Rows[0]["ownerID"] != null)
                {
                    this.ownerID = ds.Tables[0].Rows[0]["ownerID"].ToString();
                }
            }
        }

        public List<DBTrack> Fill(DataSet ds)
        {
            List<DBTrack> list = new List<DBTrack>();
            int row = ds.Tables[0].Rows.Count;
            if (row > 0)
            {
                for (int i = 0; i < row; i++)
                {
                    DBTrack track = new DBTrack();
                    if (ds.Tables[0].Rows[i]["Index"] != null && ds.Tables[0].Rows[i]["Index"].ToString() != "")
                    {
                        track.Index = int.Parse(ds.Tables[0].Rows[i]["Index"].ToString());
                    }
                    if (ds.Tables[0].Rows[i]["ID"] != null)
                    {
                        track.ID = ds.Tables[0].Rows[i]["ID"].ToString();
                    }
                    if (ds.Tables[0].Rows[i]["roundID"] != null)
                    {
                        track.roundID = ds.Tables[0].Rows[i]["roundID"].ToString();
                    }
                    if (ds.Tables[0].Rows[i]["touchdown_p1"] != null)
                    {
                        track.touchdown_p1 = ds.Tables[0].Rows[i]["touchdown_p1"].ToString();
                    }
                    if (ds.Tables[0].Rows[i]["touchdown_p2"] != null)
                    {
                        track.touchdown_p2 = ds.Tables[0].Rows[i]["touchdown_p2"].ToString();
                    }
                    if (ds.Tables[0].Rows[i]["maxSpeed"] != null && ds.Tables[0].Rows[i]["maxSpeed"].ToString() != "")
                    {
                        track.maxSpeed = decimal.Parse(ds.Tables[0].Rows[i]["maxSpeed"].ToString());
                    }
                    if (ds.Tables[0].Rows[i]["maxRotateSpeed"] != null && ds.Tables[0].Rows[i]["maxRotateSpeed"].ToString() != "")
                    {
                        track.maxRotateSpeed = decimal.Parse(ds.Tables[0].Rows[i]["maxRotateSpeed"].ToString());
                    }
                    if (ds.Tables[0].Rows[i]["xRotateSpeed"] != null && ds.Tables[0].Rows[i]["xRotateSpeed"].ToString() != "")
                    {
                        track.xRotateSpeed = int.Parse(ds.Tables[0].Rows[i]["xRotateSpeed"].ToString());
                    }
                    if (ds.Tables[0].Rows[i]["yRotateSpeed"] != null && ds.Tables[0].Rows[i]["yRotateSpeed"].ToString() != "")
                    {
                        track.yRotateSpeed = int.Parse(ds.Tables[0].Rows[i]["yRotateSpeed"].ToString());
                    }
                    if (ds.Tables[0].Rows[i]["direction"] != null && ds.Tables[0].Rows[i]["direction"].ToString() != "")
                    {
                        track.direction = int.Parse(ds.Tables[0].Rows[i]["direction"].ToString());
                    }
                    if (ds.Tables[0].Rows[i]["overNetHeight"] != null && ds.Tables[0].Rows[i]["overNetHeight"].ToString() != "")
                    {
                        track.overNetHeight = decimal.Parse(ds.Tables[0].Rows[i]["overNetHeight"].ToString());
                    }
                    if (ds.Tables[0].Rows[i]["traceString"] != null)
                    {
                        track.traceString = ds.Tables[0].Rows[i]["traceString"].ToString();
                    }
                    if (ds.Tables[0].Rows[i]["shadowLenght"] != null && ds.Tables[0].Rows[i]["shadowLenght"].ToString() != "")
                    {
                        track.shadowLenght = decimal.Parse(ds.Tables[0].Rows[i]["shadowLenght"].ToString());
                    }
                    if (ds.Tables[0].Rows[i]["shadowWidth"] != null && ds.Tables[0].Rows[i]["shadowWidth"].ToString() != "")
                    {
                        track.shadowWidth = decimal.Parse(ds.Tables[0].Rows[i]["shadowWidth"].ToString());
                    }
                    if (ds.Tables[0].Rows[i]["shadowAngle"] != null && ds.Tables[0].Rows[i]["shadowAngle"].ToString() != "")
                    {
                        track.shadowAngle = decimal.Parse(ds.Tables[0].Rows[i]["shadowAngle"].ToString());
                    }
                    if (ds.Tables[0].Rows[i]["createtime"] != null && ds.Tables[0].Rows[i]["createtime"].ToString() != "")
                    {
                        track.createtime = DateTime.Parse(ds.Tables[0].Rows[i]["createtime"].ToString());
                    }
                    if (ds.Tables[0].Rows[i]["trackResult"] != null && ds.Tables[0].Rows[i]["trackResult"].ToString() != "")
                    {
                        track.trackResult = int.Parse(ds.Tables[0].Rows[i]["trackResult"].ToString());
                    }
                    if (ds.Tables[0].Rows[i]["ownerID"] != null)
                    {
                        track.ownerID = ds.Tables[0].Rows[i]["ownerID"].ToString();
                    }
                    list.Add(track);
                }

            }
            list.Sort(compare);
            return list;
        }

        int compare(DBTrack a, DBTrack b)
        {
            return a.Index.CompareTo(b.Index);
        }
    }
}

