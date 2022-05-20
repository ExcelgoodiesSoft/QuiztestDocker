using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quiz.DO
{
    public class UserDO
    {
        public int UserAutoID { get; set; }
        public string MobileNo { get; set; }
        public string EmailID { get; set; }
        public string FullName { get; set; }
        public string UserRole { get; set; }
        public string RollNo { get; set; }
        public string Department { get; set; }
        public string Status { get; set; }
        public string Topic { get; set; }
        public string Duration { get; set; }
        public string Result { get; set; }
        public string CreatedDate { get; set; }
    }
    public class LoginDO
    {
        public int LoginAutoID { get; set; }
        public string MobileNo { get; set; }
        public string OTP { get; set; }
        public DateTime LoginDate { get; set; }
        public int DeleteFlag { get; set; }
        public int UserRole { get; set; }
    }
    public class QuestionDO
    {
        public int QuestionAutoID { get; set; }
        public string TopicName { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string CreatedDate { get; set; }
    }
    public class JsonArrayListDO
    {
        public int id { get; set; }
        public int type { get; set; }
        public string question { get; set; }
        public List<string> choices { get; set; }
        public List<int> corrects { get; set; }
    }
    public class MappingDO
    {
        public int UserTopicMappingAutoID { get; set; }
        public int QuestionAutoID { get; set; }
        public int UserAutoID { get; set; }
        public int Status { get; set; }
        public string CreatedDate { get; set; }
        public int DeleteFlag { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

    }
}
