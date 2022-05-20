using Quiz.DO;
using Quiz.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.DAL
{
    public class UserDAL
    {
        private static SqlParameter[] CmdParms;
        public static int DoCheckUserMobileActive(string MobileNo)
        {
            string query = "SELECT status FROM tbl_UserDetails where DeleteFlag =0 and MobileNo='"+ MobileNo + "' ";
            object objCandidateID = SQLHelper.ExecuteScalar(query);
            return SQLHelper.ConvertInt(objCandidateID);
        }
        public static int DoInsertLoginOTP(string MobileNo, string OTP)
        {
            int nResult = 0;
            try
            {
                SqlParameter[] CmdParms = new SqlParameter[3];
                CmdParms[0] = new SqlParameter("@MobileNo", MobileNo);
                CmdParms[1] = new SqlParameter("@OTP", OTP);
                CmdParms[2] = new SqlParameter("@LoginDate", System.DateTime.Now);
                
                string CommandText = "INSERT INTO tbl_LoginOTPDetails (MobileNo, OTP, LoginDate, DeleteFlag) VALUES (@MobileNo, @OTP, @LoginDate, 0); SELECT SCOPE_IDENTITY();";
                nResult = Convert.ToInt32(SQLHelper.ExecuteScalar(CommandText, CmdParms));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return nResult;
        }
        public static LoginDO DoGetUserOTPDetails(string LoginAutoID)
        {
            SqlConnection sqlConn = SQLHelper.ExecuteReaderConnection();
            try
            {
                LoginDO obj = new LoginDO();
                string CommandText = "SELECT * from tbl_LoginOTPDetails l";
                CommandText += " where  LoginAutoID=" + LoginAutoID + " ;";
                SqlDataReader dr = SQLHelper.ExecuteReader(CommandText, sqlConn);
                while (dr.Read())
                {                    
                    obj.LoginAutoID = SQLHelper.ConvertInt(dr["LoginAutoID"].ToString());
                    obj.MobileNo = dr["MobileNo"].ToString();
                    obj.OTP = dr["OTP"].ToString();
                    obj.LoginDate = SQLHelper.ConvertDateTime(dr["LoginDate"].ToString());
                    obj.LoginDate = SQLHelper.ConvertDateTime(dr["LoginDate"].ToString());
                    //obj.UserRole = SQLHelper.ConvertInt(dr["UserRole"].ToString());
                }
                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                SQLHelper.ExecuteReaderConnectionClose(sqlConn);
            }
        }
        
        //User Details
        public static List<UserDO> DoGetUserDetails(string CandidateMobile,string search)
        {
            List<UserDO> lstDO = new List<UserDO>();
            SqlConnection sqlConn = SQLHelper.ExecuteReaderConnection();
            try
            {
                string CommandText = "select u.*,s.*,q.TopicName from tbl_UserDetails u";
                CommandText += " left outer join tbl_ScoreDetails s on s.UserAutoID=u.UserAutoID and s.DeleteFlag=0";
                CommandText += " left outer join tbl_QuestionDetails q on s.QuestionAutoID=q.QuestionAutoID";
                CommandText += " where u.DeleteFlag =0 ";
                if (CandidateMobile != null & CandidateMobile != "0") CommandText += " and u.MobileNo ='" + CandidateMobile + "'";
                if (search != null & search != "") CommandText += " and (u.UserRole like '%" + search + "%' or u.CreatedDate like '%" + search + "%' or u.EmailID like '%" + search + "%' or u.FullName like '%" + search + "%' or u.MobileNo like '%" + search + "%' or u.RollNo like '%" + search + "%' or u.Department like '%" + search + "%' )";
                SqlDataReader dr = SQLHelper.ExecuteReader(CommandText, sqlConn);
                while (dr.Read())
                {
                    UserDO obj = new UserDO();
                    obj.UserAutoID = SQLHelper.ConvertInt(dr["UserAutoID"].ToString());
                    obj.EmailID = dr["EmailID"].ToString();
                    obj.FullName = dr["FullName"].ToString();
                    obj.MobileNo = dr["MobileNo"].ToString();
                    obj.UserRole = dr["UserRole"].ToString();
                    obj.RollNo = dr["RollNo"].ToString();
                    obj.Department = dr["Department"].ToString();
                    obj.Status = dr["status"].ToString();
                    obj.Topic = dr["TopicName"].ToString();
                    int duration = SQLHelper.ConvertInt(dr["duration"].ToString());

                    string strDuration = "0";
                    if (duration > 0) {
                        strDuration = duration / 60 + " : " + duration % 60;
                    }
                    obj.Duration = strDuration;
                    int NoQuestion = SQLHelper.ConvertInt(dr["NoQuestion"].ToString());
                    int Correct = SQLHelper.ConvertInt(dr["Correct"].ToString());
                    obj.Result = "";
                    if (NoQuestion!=0)
                    obj.Result = Correct + "/" + NoQuestion;

                   obj.CreatedDate = SQLHelper.SqlDateDisplay(dr["CreatedDate"].ToString());
                    lstDO.Add(obj);
                }
                return lstDO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                SQLHelper.ExecuteReaderConnectionClose(sqlConn);
            }
        }
        public static int DoGetCandidateID()
        {
            string query = "SELECT ISNULL(MAX(CandidateID), 0)+1 FROM tbl_UserDetails; ";
            object objCandidateID = SQLHelper.ExecuteScalar(query);
            return Convert.ToInt32(objCandidateID);
        }
        public static int DoInsertUserDetails(UserDO objDO)
        {
           try
            {
                int nResult = 0;
                int strCandidateID = 0;
                string CommandText = string.Empty;
                if (objDO.UserAutoID == 0) strCandidateID = DoGetCandidateID();

                CmdParms = new SqlParameter[11];
                CmdParms[0] = new SqlParameter("@UserAutoID", objDO.UserAutoID);
                CmdParms[1] = new SqlParameter("@Identifier", SQLHelper.RandomString(20));
                CmdParms[2] = new SqlParameter("@CandidateID", strCandidateID);
                CmdParms[3] = new SqlParameter("@FullName", objDO.FullName);
                CmdParms[4] = new SqlParameter("@MobileNo", objDO.MobileNo);
                CmdParms[5] = new SqlParameter("@EmailID", objDO.EmailID);
                CmdParms[6] = new SqlParameter("@UserRole", "2");
                CmdParms[7] = new SqlParameter("@CreatedDate", System.DateTime.Now);
                CmdParms[8] = new SqlParameter("@UpdatedDate", System.DateTime.Now);
                CmdParms[9] = new SqlParameter("@RollNo", objDO.RollNo);
                CmdParms[10] = new SqlParameter("@Department", objDO.Department);

                if (objDO.UserAutoID == 0)
                {
                    CommandText = "Insert into tbl_UserDetails(Department,RollNo,Identifier,CandidateID,FullName,MobileNo,EmailID,UserRole,CreatedDate,UpdatedDate,DeleteFlag)" +
                    "Values(@Department,@RollNo,@Identifier,@CandidateID,@FullName,@MobileNo,@EmailID,@UserRole,@CreatedDate,@UpdatedDate,0);SELECT SCOPE_IDENTITY();";
                }
                else CommandText = " Update tbl_UserDetails set Department=@Department,RollNo=@RollNo,FullName=@FullName,MobileNo=@MobileNo,EmailID=@EmailID,UpdatedDate=@UpdatedDate where UserAutoID=" + objDO.UserAutoID;
                
                nResult = SQLHelper.ConvertInt(SQLHelper.ExecuteScalar(CommandText, CmdParms));
                return nResult;
            }
            catch (Exception exMe)
            {
                throw exMe;
            }
        }
        public static int DoDeleteUserDetails(int UserAutoID)
        {
            string query = " Update tbl_UserDetails set DeleteFlag=1 where UserAutoID=" + UserAutoID+" ;";
            object objCandidateID = SQLHelper.ExecuteScalar(query);
            return Convert.ToInt32(objCandidateID);
        }
        public static int DoUpdateStatusUserDetails(int UserAutoID,int status)
        {
            string query = " Update tbl_UserDetails set status="+ status + " where UserAutoID=" + UserAutoID + " ;";
            object objCandidateID = SQLHelper.ExecuteScalar(query);
            return Convert.ToInt32(objCandidateID);
        }
        //Question Details
        public static List<QuestionDO> DoGetQuestionDetails(string search)
        {
            List<QuestionDO> lstDO = new List<QuestionDO>();
            SqlConnection sqlConn = SQLHelper.ExecuteReaderConnection();
            try
            {
                string CommandText = "select * from tbl_QuestionDetails where DeleteFlag =0 ";
                if (search != null & search != "") CommandText += " and (TopicName like '%" + search + "%' or CreatedDate like '%" + search + "%' )";
                SqlDataReader dr = SQLHelper.ExecuteReader(CommandText, sqlConn);
                while (dr.Read())
                {
                    QuestionDO obj = new QuestionDO();
                    obj.QuestionAutoID = SQLHelper.ConvertInt(dr["QuestionAutoID"].ToString());
                    obj.TopicName = dr["TopicName"].ToString();
                    obj.CreatedDate = SQLHelper.SqlDateDisplay(dr["CreatedDate"].ToString());
                    lstDO.Add(obj);
                }
                return lstDO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                SQLHelper.ExecuteReaderConnectionClose(sqlConn);
            }
        }
        public static int DoInsertQuestionDetails(QuestionDO objDO)
        {
            try
            {
                int nResult = 0;
                string CommandText = string.Empty;
                CmdParms = new SqlParameter[7];
                CmdParms[0] = new SqlParameter("@QuestionAutoID", objDO.QuestionAutoID);
                CmdParms[1] = new SqlParameter("@Identifier", SQLHelper.RandomString(20));
                CmdParms[2] = new SqlParameter("@TopicName", objDO.TopicName);
                CmdParms[3] = new SqlParameter("@FileName", objDO.FileName);
                CmdParms[4] = new SqlParameter("@FilePath", objDO.FilePath);
                CmdParms[5] = new SqlParameter("@CreatedDate", System.DateTime.Now);
                CmdParms[6] = new SqlParameter("@UpdatedDate", System.DateTime.Now);

                if (objDO.QuestionAutoID == 0)
                {
                    CommandText = "Insert into tbl_QuestionDetails(Identifier,TopicName,FileName,FilePath,CreatedDate,UpdatedDate,DeleteFlag)" +
                    "Values(@Identifier,@TopicName,@FileName,@FilePath,@CreatedDate,@UpdatedDate,0);SELECT SCOPE_IDENTITY();";
                }
                else CommandText = " Update tbl_QuestionDetails set TopicName=@TopicName,UpdatedDate=@UpdatedDate where QuestionAutoID=" + objDO.QuestionAutoID;

                nResult = SQLHelper.ConvertInt(SQLHelper.ExecuteScalar(CommandText, CmdParms));
                return nResult;
            }
            catch (Exception exMe)
            {
                throw exMe;
            }
        }
        public static int DoDeleteQuestionDetails(int QuestionAutoID)
        {
            string query = " Update tbl_QuestionDetails set DeleteFlag=1 where QuestionAutoID=" + QuestionAutoID + " ;";
            object objCandidateID = SQLHelper.ExecuteScalar(query);
            return Convert.ToInt32(objCandidateID);
        }
        public static DataTable DoGetDataFromExcel(string filepath, string SheetName)
        {
            DataTable dTable = new DataTable();
            try
            {
                if (File.Exists(filepath))
                {
                    string OledbConnstring = @"Provider=Microsoft.ACE.OLEDB.12.0;  Data Source=" + filepath + ";Extended Properties=\"Excel 12.0;HDR=YES;\"";
                    string strSQL = "SELECT * FROM [" + SheetName + "$]";
                    OleDbConnection OledbConn = new OleDbConnection(OledbConnstring);
                    OledbConn.Open();
                    OleDbCommand OledbComm = new OleDbCommand(strSQL, OledbConn);
                    OleDbDataAdapter DAP = new OleDbDataAdapter(OledbComm);
                    try
                    {
                        DAP.Fill(dTable);
                    }
                    catch (Exception ex)
                    {
                        DAP.Dispose();
                        OledbComm.Dispose();
                        OledbConn.Close();
                        OledbConn.Dispose();
                    }
                    DAP.Dispose();
                    OledbComm.Dispose();
                    OledbConn.Close();
                    OledbConn.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dTable;
        }
        //User Mapping Details
        public static List<MappingDO> DoGetUserMappingDetails(string CandidateID, string QuestionID, string search)
        {
            List<MappingDO> lstDO = new List<MappingDO>();
            SqlConnection sqlConn = SQLHelper.ExecuteReaderConnection();
            try
            {
                string CommandText = "select * from tbl_UserTopicMapping where DeleteFlag =0 ";
                if (search != null & search != "") CommandText += " and (TopicName like '%" + search + "%' or CreatedDate like '%" + search + "%' )";
                SqlDataReader dr = SQLHelper.ExecuteReader(CommandText, sqlConn);
                while (dr.Read())
                {
                    MappingDO obj = new MappingDO();
                    obj.UserTopicMappingAutoID = SQLHelper.ConvertInt(dr["UserTopicMappingAutoID"].ToString());
                    obj.QuestionAutoID = SQLHelper.ConvertInt(dr["QuestionAutoID"].ToString());
                    obj.UserAutoID = SQLHelper.ConvertInt(dr["UserAutoID"].ToString());
                    obj.Status = SQLHelper.ConvertInt(dr["Status"].ToString());
                    obj.CreatedDate = SQLHelper.SqlDateDisplay(dr["CreatedDate"].ToString());
                    obj.StartTime = dr["StartTime"].ToString();
                    obj.EndTime = dr["EndTime"].ToString();
                    lstDO.Add(obj);
                }
                return lstDO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                SQLHelper.ExecuteReaderConnectionClose(sqlConn);
            }
        }
        public static int DoInsertUserMappingDetails(string QuestionAutoID, string UserAutoID)
        {
            try
            {
                string CommandText = "Update tbl_UserDetails set status=0 where UserAutoID=" + UserAutoID + ";Update tbl_ScoreDetails set DeleteFlag=1 where UserAutoID=" + UserAutoID + ";";
                CommandText += " Insert into tbl_ScoreDetails(QuestionAutoID , UserAutoID) Values(" + QuestionAutoID + " , 	" + UserAutoID + " ); SELECT SCOPE_IDENTITY();";

                SQLHelper.ExecuteScalar(CommandText);
                return 1;
            }
            catch (Exception exMe)
            {
                throw exMe;
            }
        }
        public static int DoUpdateScoredetails(string UserAutoID,int NoQuestion,int Correct,int Worng, int duration, string Answer)
        {
            try
            {
                string CommandText = "Update tbl_ScoreDetails set NoQuestion="+ NoQuestion + ",Correct=" + Correct + ",Worng=" + Worng + ",duration=" + duration + ",Answer='" + Answer + "' where DeleteFlag=0 and UserAutoID=" + UserAutoID + ";";

                SQLHelper.ExecuteScalar(CommandText);
                return 1;
            }
            catch (Exception exMe)
            {
                throw exMe;
            }
        }
        public static int DoDeleteUserMappingDetails(int UserTopicMappingAutoID)
        {
            string query = " Update tbl_UserTopicMapping set DeleteFlag=1 where UserTopicMappingAutoID=" + UserTopicMappingAutoID + " ;";
            object objCandidateID = SQLHelper.ExecuteScalar(query);
            return Convert.ToInt32(objCandidateID);
        }
    }
}
