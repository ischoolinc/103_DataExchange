using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Xml;
namespace JH.TC.DataExchange._103
{
    class custStudentRecord
    {
        //Student
        //public static string[] requiredFields = new string[] { "id" };
        public string AccountType { get; private set; }
        public DateTime? Birthday { get; private set; }
        public string BirthPlace { get; private set; }
        public string Comment { get; private set; }
        public string EnglishName { get; private set; }
        public string EnrollmentCategory { get; private set; }
        public string Gender { get; private set; }
        public string ID { get; private set; }
        public string IDNumber { get; private set; }
        public string Name { get; private set; }
        public string Nationality { get; private set; }
        public string RefClassID { get; private set; }
        public string SALoginName { get; private set; }
        public int? SeatNo { get; private set; }
        public string StudentNumber { get; private set; }

        //new atti below
        public string SMSPhone { get; private set; }
        public string MallingAddress { get; private set; }
        public string MallingAddressZipCode { get; private set; }
        public string MallingAddressCounty { get; private set; }
        public string MallingAddressTown { get; private set; }
        public string MallingAddressDetailAddress { get; private set; }
        public string ContactPhone { get; private set; }
        public string OtherPhone1 { get; private set; }
        public string OtherPhone2 { get; private set; }
        public string OtherPhone3 { get; private set; }
        public string PermanentAddress { get; private set; }
        public string PermanentAddressZipCode { get; private set; }
        public string PermanentAddressCounty { get; private set; }
        public string PermanentAddressTown { get; private set; }
        public string PermanentAddressDetailAddress { get; private set; }
        public string PermanentPhone { get; private set; }
        public string DiplomaNumber { get; private set; }
        public string FatherName { get; private set; }
        public string FatherNationality { get; private set; }
        public string FatherIDNumber { get; private set; }
        public string FatherLiving { get; private set; }
        public string FatherJob { get; private set; }
        public string FatherEducationDegree { get; private set; }
        public string MotherName { get; private set; }
        public string MotherNationality { get; private set; }
        public string MotherIDNumber { get; private set; }
        public string MotherLiving { get; private set; }
        public string MotherJob { get; private set; }
        public string MotherEducationDegree { get; private set; }
        public string CustodianName { get; private set; }
        public string CustodianNationality { get; private set; }
        public string CustodianIDNumber { get; private set; }
        public string CustodianLiving { get; private set; }
        public string CustodianRelationship { get; private set; }
        public string CustodianJob { get; private set; }
        public string CustodianEducationDegree { get; private set; }
        public string BeforeEnrollmentSchool { get; private set; }
        public string BeforeEnrollmentSchoolLocation { get; private set; }
        public string BeforeEnrollmentClassName { get; private set; }
        public string BeforeEnrollmentSeatNo { get; private set; }
        public string BeforeEnrollmentMemo { get; private set; }
        public string BeforeEnrollmentGraduateSchoolYear { get; private set; }

        //from other tables
        public string ClassName { get; private set; }
        public string ClassGradeYear { get; private set; }
        public string RefDeptID { get; private set; }
        public string DeptName { get; private set; } // in dept table

        public enum StudentStatus
        {
            一般 = 0,
            延修 = 1,
            畢業或離校 = 2,
            休學 = 3,
            輟學 = 4,
            刪除 = 5,
            轉出 = 6,
            退學 = 7,
        }
        public custStudentRecord(DataRow row)
        {
            //foreach (string field in requiredFields)
            //{
            //    if (string.IsNullOrEmpty(row["id"].ToString()))
            //        throw new ArgumentNullException("student id not allow be null");
            //}

            #region setup a Student
            if (row.Table.Columns.Contains("id"))
                this.ID = "" + row["id"];
            if (row.Table.Columns.Contains("name"))
                this.Name = "" + row["name"];
            if (row.Table.Columns.Contains("english_name"))
                this.EnglishName = "" + row["english_name"];
            if (row.Table.Columns.Contains("birthdate"))
            {
                DateTime tmp;
                this.Birthday = DateTime.TryParse("" + row["birthdate"], out tmp) ? (DateTime?)tmp : null;
            }
            if (row.Table.Columns.Contains("id_number"))
                this.IDNumber = "" + row["id_number"];
            if (row.Table.Columns.Contains("ref_class_id"))
                this.RefClassID = "" + row["ref_class_id"];
            if (row.Table.Columns.Contains("class_name"))
                this.ClassName = "" + row["class_name"];
            this.ClassName = this.ClassName.Substring(this.ClassName.Length >= 2 ? this.ClassName.Length - 2 : 0);

            if (row.Table.Columns.Contains("class_grade_year"))
                this.ClassGradeYear = "" + row["class_grade_year"];
            if (row.Table.Columns.Contains("class_ref_dept_id"))
                this.RefDeptID = "" + row["class_ref_dept_id"];//in table "class"'s ref_dept_id
            if (row.Table.Columns.Contains("dept_name"))
                this.DeptName = "" + row["dept_name"];

            if (row.Table.Columns.Contains("birth_place"))
                this.BirthPlace = "" + row["birth_place"];
            if (row.Table.Columns.Contains("student_number"))
                this.StudentNumber = "" + row["student_number"];
            if (row.Table.Columns.Contains("seat_no"))
            {
                int tmp_seatno;
                this.SeatNo = int.TryParse("" + row["seat_no"], out tmp_seatno) ? (int?)tmp_seatno : null;
            }
            if (row.Table.Columns.Contains("sa_login_name"))
                this.SALoginName = "" + row["sa_login_name"];
            if (row.Table.Columns.Contains("account_type"))
                this.AccountType = "" + row["account_type"];
            if (row.Table.Columns.Contains("gender"))
                this.Gender = mapGender("" + row["gender"]);
            if (row.Table.Columns.Contains("nationality"))
                this.Nationality = "" + row["nationality"];
            if (row.Table.Columns.Contains("comment"))
                this.Comment = "" + row["comment"];
            if (row.Table.Columns.Contains("sms_phone"))
                this.SMSPhone = "" + row["sms_phone"];
            if (row.Table.Columns.Contains("contact_phone"))
                this.ContactPhone = "" + row["contact_phone"];
            if (row.Table.Columns.Contains("permanent_phone"))
                this.PermanentPhone = "" + row["permanent_phone"];
            //<PhoneList><PhoneNumber>0937177460</PhoneNumber><PhoneNumber/><PhoneNumber/></PhoneList>
            if (row.Table.Columns.Contains("other_phones"))
            {
                XmlDocument xmlDoc = new XmlDocument();
                loadXml(xmlDoc, "" + row["other_phones"]);
                string tmp = "";
                if (parseXml(xmlDoc, "PhoneList/PhoneNumber", out tmp, 0))
                    this.OtherPhone1 = tmp;
                if (parseXml(xmlDoc, "PhoneList/PhoneNumber", out tmp, 1))
                    this.OtherPhone2 = tmp;
                if (parseXml(xmlDoc, "PhoneList/PhoneNumber", out tmp, 2))
                    this.OtherPhone3 = tmp;
            }

            //<AddressList><Address><ZipCode>304</ZipCode><County>新竹縣</County><Town>新豐鄉</Town><DetailAddress>瑞興村3鄰46之2號</DetailAddress></Address></AddressList>
            if (row.Table.Columns.Contains("mailing_address"))
            {
                XmlDocument xmlDoc = new XmlDocument();
                loadXml(xmlDoc, "" + row["mailing_address"]);
                string tmp = "";
                if (parseXml(xmlDoc, "AddressList/Address/ZipCode", out tmp))
                    this.MallingAddressZipCode = tmp;
                if (parseXml(xmlDoc, "AddressList/Address/County", out tmp))
                    this.MallingAddressCounty = tmp;
                if (parseXml(xmlDoc, "AddressList/Address/Town", out tmp))
                    this.MallingAddressTown = tmp;
                if (parseXml(xmlDoc, "AddressList/Address/DetailAddress", out tmp))
                    this.MallingAddressDetailAddress = tmp;
                if (!string.IsNullOrWhiteSpace(this.MallingAddressDetailAddress))
                    this.MallingAddress = "[" + this.MallingAddressZipCode + "]" + MallingAddressCounty + MallingAddressTown + MallingAddressDetailAddress;
            }
            if (row.Table.Columns.Contains("permanent_address"))
            {
                XmlDocument xmlDoc = new XmlDocument();
                loadXml(xmlDoc, "" + row["permanent_address"]);
                string tmp = "";
                if (parseXml(xmlDoc, "AddressList/Address/ZipCode", out tmp))
                    this.PermanentAddressZipCode = tmp;
                if (parseXml(xmlDoc, "AddressList/Address/County", out tmp))
                    this.PermanentAddressCounty = tmp;
                if (parseXml(xmlDoc, "AddressList/Address/Town", out tmp))
                    this.PermanentAddressTown = tmp;
                if (parseXml(xmlDoc, "AddressList/Address/DetailAddress", out tmp))
                    this.PermanentAddressDetailAddress = tmp;
                if (!string.IsNullOrWhiteSpace(this.PermanentAddressDetailAddress))
                    this.PermanentAddress = "[" + this.PermanentAddressZipCode + "]" + PermanentAddressCounty + PermanentAddressTown + PermanentAddressDetailAddress;
            }
            if (row.Table.Columns.Contains("diploma_number"))
            {
                XmlDocument xmlDoc = new XmlDocument();
                loadXml(xmlDoc, "" + row["diploma_number"]);
                string tmp = "";
                if (parseXml(xmlDoc, "DiplomaNumber", out tmp))
                    this.DiplomaNumber = tmp;
            }
            #region Father
            if (row.Table.Columns.Contains("father_name"))
                this.FatherName = "" + row["father_name"];
            if (row.Table.Columns.Contains("father_nationality"))
                this.FatherNationality = "" + row["father_nationality"];
            if (row.Table.Columns.Contains("father_id_number"))
                this.FatherIDNumber = "" + row["father_id_number"];
            if (row.Table.Columns.Contains("father_living"))
                this.FatherLiving = mapParentLiving("" + row["father_living"]);
            if (row.Table.Columns.Contains("father_other_info"))
            {
                XmlDocument xmlDoc = new XmlDocument();
                loadXml(xmlDoc, "" + row["father_other_info"]);
                string tmp = "";
                if (parseXml(xmlDoc, "FatherOtherInfo/FatherJob", out tmp))
                    this.FatherJob = tmp;
                if (parseXml(xmlDoc, "FatherOtherInfo/FatherEducationDegree", out tmp))
                    this.FatherEducationDegree = tmp;
            }
            #endregion

            #region Mother
            if (row.Table.Columns.Contains("mother_name"))
                this.MotherName = "" + row["mother_name"];
            if (row.Table.Columns.Contains("mother_nationality"))
                this.MotherNationality = "" + row["mother_nationality"];
            if (row.Table.Columns.Contains("mother_id_number"))
                this.MotherIDNumber = "" + row["mother_id_number"];
            if (row.Table.Columns.Contains("mother_living"))
                this.MotherLiving = mapParentLiving("" + row["mother_living"]);
            if (row.Table.Columns.Contains("mother_other_info"))
            {
                XmlDocument xmlDoc = new XmlDocument();
                loadXml(xmlDoc, "" + row["mother_other_info"]);
                string tmp = "";
                if (parseXml(xmlDoc, "MotherOtherInfo/MotherJob", out tmp))
                    this.MotherJob = tmp;
                if (parseXml(xmlDoc, "MotherOtherInfo/MotherEducationDegree", out tmp))
                    this.MotherEducationDegree = tmp;
            }
            #endregion

            #region Custodian
            if (row.Table.Columns.Contains("custodian_name"))
                this.CustodianName = "" + row["custodian_name"];
            if (row.Table.Columns.Contains("custodian_nationality"))
                this.CustodianNationality = "" + row["custodian_nationality"];
            if (row.Table.Columns.Contains("custodian_id_number"))
                this.CustodianIDNumber = "" + row["custodian_id_number"];
            if (row.Table.Columns.Contains("custodian_living"))
                this.CustodianLiving = mapParentLiving("" + row["custodian_living"]);
            if (row.Table.Columns.Contains("custodian_relationship"))
                this.CustodianRelationship = "" + row["custodian_relationship"];
            if (row.Table.Columns.Contains("custodian_other_info"))
            {
                XmlDocument xmlDoc = new XmlDocument();
                loadXml(xmlDoc, "" + row["custodian_other_info"]);
                string tmp = "";
                if (parseXml(xmlDoc, "CustodianOtherInfo/Job", out tmp))
                    this.CustodianJob = tmp;
                if (parseXml(xmlDoc, "CustodianOtherInfo/EducationDegree", out tmp))
                    this.CustodianEducationDegree = tmp;
            }
            #endregion

            #region BeforeEnrollment
            //<BeforeEnrollment><School>1</School><SchoolLocation>2</SchoolLocation><ClassName>3</ClassName><SeatNo>4</SeatNo><Memo>5</Memo><GraduateSchoolYear>6</GraduateSchoolYear></BeforeEnrollment>
            if (row.Table.Columns.Contains("before_enrollment"))
            {
                XmlDocument xmlDoc = new XmlDocument();
                loadXml(xmlDoc, "" + row["before_enrollment"]);
                string tmp = "";
                if (parseXml(xmlDoc, "BeforeEnrollment/School", out tmp))
                    this.BeforeEnrollmentSchool = tmp;
                if (parseXml(xmlDoc, "BeforeEnrollment/SchoolLocation", out tmp))
                    this.BeforeEnrollmentSchoolLocation = tmp;
                if (parseXml(xmlDoc, "BeforeEnrollment/ClassName", out tmp))
                    this.BeforeEnrollmentClassName = tmp;
                if (parseXml(xmlDoc, "BeforeEnrollment/SeatNo", out tmp))
                    this.BeforeEnrollmentSeatNo = tmp;
                if (parseXml(xmlDoc, "BeforeEnrollment/Memo", out tmp))
                    this.BeforeEnrollmentMemo = tmp;
                if (parseXml(xmlDoc, "BeforeEnrollment/GraduateSchoolYear", out tmp))
                    this.BeforeEnrollmentGraduateSchoolYear = tmp;

            }
            #endregion

            #endregion
        }
        public static void loadXml(XmlDocument xmlDoc, string xml)
        {
            xmlDoc.LoadXml("<A>" + xml + "</A>");
        }
        public static bool parseXml(XmlDocument xmlDoc, string xpath, out string output, int elementIndex = 0)
        {
            output = "";
            List<string> r = new List<string>();
            try
            {
                var elements = xmlDoc.SelectNodes("A/" + xpath);
                for (int i = 0; i < elements.Count; i++)
                {
                    XmlNode element = elements[i];
                    if (i == elementIndex)
                    {
                        output = element.InnerText;
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {

                return false;
            }
        }
        public static string mapGender(string gender)
        {
            switch (gender)
            {
                case "1":
                    return "1";
                case "0":
                    return "2";
                default:
                    return "";
            }
        }
        public static string mapParentLiving(string living)
        {
            switch (living)
            {
                case "true":
                    return "存";
                case "false":
                    return "歿";
                default:
                    return "";
            }
        }
    }
}
