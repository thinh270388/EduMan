using EduManModel.Dtos;

namespace EduManModel
{
    public partial class DataProcess<T>
    {
        public Dictionary<Type, string> UrlGetAll = new()
        {
            {typeof(DtoClass),"http://bacai.ddns.net/api/EduMan/Class/GetAll" },
            {typeof(DtoClassDiscipline),"http://bacai.ddns.net/api/EduMan/ClassDiscipline/GetAll" },
            {typeof(DtoClassDistribute),"http://bacai.ddns.net/api/EduMan/ClassDistribute/GetAll" },
            {typeof(DtoDiscipline),"http://bacai.ddns.net/api/EduMan/Discipline/GetAll" },
            {typeof(DtoDisciplineGroup),"http://bacai.ddns.net/api/EduMan/DisciplineGroup/GetAll" },
            {typeof(DtoDisciplineType),"http://bacai.ddns.net/api/EduMan/DisciplineType/GetAll" },
            {typeof(DtoDroppedOut),"http://bacai.ddns.net/api/EduMan/DroppedOut/GetAll" },
            {typeof(DtoFunct),"http://bacai.ddns.net/api/EduMan/Funct/GetAll" },
            {typeof(DtoGrade),"http://bacai.ddns.net/api/EduMan/Grade/GetAll" },
            {typeof(DtoGroupUser),"http://bacai.ddns.net/api/EduMan/GroupUser/GetAll" },
            {typeof(DtoLevel),"http://bacai.ddns.net/api/EduMan/Level/GetAll" },
            {typeof(DtoRoleAssign),"http://bacai.ddns.net/api/EduMan/RoleAssign/GetAll" },
            {typeof(DtoStartWeek),"http://bacai.ddns.net/api/EduMan/StartWeek/GetAll" },
            {typeof(DtoStudent),"http://bacai.ddns.net/api/EduMan/Student/GetAll" },
            {typeof(DtoStudentDiscipline),"http://bacai.ddns.net/api/EduMan/StudentDiscipline/GetAll" },
            {typeof(DtoStudentDistribute),"http://bacai.ddns.net/api/EduMan/StudentDistribute/GetAll" },
            {typeof(DtoTeacher),"http://bacai.ddns.net/api/EduMan/Teacher/GetAll" },
            {typeof(DtoUserInfo),"http://bacai.ddns.net/api/EduMan/UserInfo/GetAll" },
            {typeof(DtoWeekly),"http://bacai.ddns.net/api/EduMan/Weekly/GetAll" }
        };
        public Dictionary<Type, string> UrlGetOne = new()
        {
            {typeof(DtoClass),"http://bacai.ddns.net/api/EduMan/Class/GetOne" },
            {typeof(DtoClassDiscipline),"http://bacai.ddns.net/api/EduMan/ClassDiscipline/GetOne" },
            {typeof(DtoClassDistribute),"http://bacai.ddns.net/api/EduMan/ClassDistribute/GetOne" },
            {typeof(DtoDiscipline),"http://bacai.ddns.net/api/EduMan/Discipline/GetOne" },
            {typeof(DtoDisciplineGroup),"http://bacai.ddns.net/api/EduMan/DisciplineGroup/GetOne" },
            {typeof(DtoDisciplineType),"http://bacai.ddns.net/api/EduMan/DisciplineType/GetOne" },
            {typeof(DtoDroppedOut),"http://bacai.ddns.net/api/EduMan/DroppedOut/GetOne" },
            {typeof(DtoFunct),"http://bacai.ddns.net/api/EduMan/Funct/GetOne" },
            {typeof(DtoGrade),"http://bacai.ddns.net/api/EduMan/Grade/GetOne" },
            {typeof(DtoGroupUser),"http://bacai.ddns.net/api/EduMan/GroupUser/GetOne" },
            {typeof(DtoLevel),"http://bacai.ddns.net/api/EduMan/Level/GetOne" },
            {typeof(DtoRoleAssign),"http://bacai.ddns.net/api/EduMan/RoleAssign/GetOne" },
            {typeof(DtoStartWeek),"http://bacai.ddns.net/api/EduMan/StartWeek/GetOne" },
            {typeof(DtoStudent),"http://bacai.ddns.net/api/EduMan/Student/GetOne" },
            {typeof(DtoStudentDiscipline),"http://bacai.ddns.net/api/EduMan/StudentDiscipline/GetOne" },
            {typeof(DtoStudentDistribute),"http://bacai.ddns.net/api/EduMan/StudentDistribute/GetOne" },
            {typeof(DtoTeacher),"http://bacai.ddns.net/api/EduMan/Teacher/GetOne" },
            {typeof(DtoUserInfo),"http://bacai.ddns.net/api/EduMan/UserInfo/GetOne" },
            {typeof(DtoWeekly),"http://bacai.ddns.net/api/EduMan/Weekly/GetOne" }
        };
        public Dictionary<Type, string> UrlFind = new()
        {
            {typeof(DtoClass),"http://bacai.ddns.net/api/EduMan/Class/Find" },
            {typeof(DtoClassDiscipline),"http://bacai.ddns.net/api/EduMan/ClassDiscipline/Find" },
            {typeof(DtoClassDistribute),"http://bacai.ddns.net/api/EduMan/ClassDistribute/Find" },
            {typeof(DtoDiscipline),"http://bacai.ddns.net/api/EduMan/Discipline/Find" },
            {typeof(DtoDisciplineGroup),"http://bacai.ddns.net/api/EduMan/DisciplineGroup/Find" },
            {typeof(DtoDisciplineType),"http://bacai.ddns.net/api/EduMan/DisciplineType/Find" },
            {typeof(DtoDroppedOut),"http://bacai.ddns.net/api/EduMan/DroppedOut/Find" },
            {typeof(DtoFunct),"http://bacai.ddns.net/api/EduMan/Funct/Find" },
            {typeof(DtoGrade),"http://bacai.ddns.net/api/EduMan/Grade/Find" },
            {typeof(DtoGroupUser),"http://bacai.ddns.net/api/EduMan/GroupUser/Find" },
            {typeof(DtoLevel),"http://bacai.ddns.net/api/EduMan/Level/Find" },
            {typeof(DtoRoleAssign),"http://bacai.ddns.net/api/EduMan/RoleAssign/Find" },
            {typeof(DtoStartWeek),"http://bacai.ddns.net/api/EduMan/StartWeek/Find" },
            {typeof(DtoStudent),"http://bacai.ddns.net/api/EduMan/Student/Find" },
            {typeof(DtoStudentDiscipline),"http://bacai.ddns.net/api/EduMan/StudentDiscipline/Find" },
            {typeof(DtoStudentDistribute),"http://bacai.ddns.net/api/EduMan/StudentDistribute/Find" },
            {typeof(DtoTeacher),"http://bacai.ddns.net/api/EduMan/Teacher/Find" },
            {typeof(DtoUserInfo),"http://bacai.ddns.net/api/EduMan/UserInfo/Find" },
            {typeof(DtoWeekly),"http://bacai.ddns.net/api/EduMan/Weekly/Find" }
        };
        public Dictionary<Type, string> UrlAdd = new()
        {
            {typeof(DtoClass),"http://bacai.ddns.net/api/EduMan/Class/Add" },
            {typeof(DtoClassDiscipline),"http://bacai.ddns.net/api/EduMan/ClassDiscipline/Add" },
            {typeof(DtoClassDistribute),"http://bacai.ddns.net/api/EduMan/ClassDistribute/Add" },
            {typeof(DtoDiscipline),"http://bacai.ddns.net/api/EduMan/Discipline/Add" },
            {typeof(DtoDisciplineGroup),"http://bacai.ddns.net/api/EduMan/DisciplineGroup/Add" },
            {typeof(DtoDisciplineType),"http://bacai.ddns.net/api/EduMan/DisciplineType/Add" },
            {typeof(DtoDroppedOut),"http://bacai.ddns.net/api/EduMan/DroppedOut/Add" },
            {typeof(DtoFunct),"http://bacai.ddns.net/api/EduMan/Funct/Add" },
            {typeof(DtoGrade),"http://bacai.ddns.net/api/EduMan/Grade/Add" },
            {typeof(DtoGroupUser),"http://bacai.ddns.net/api/EduMan/GroupUser/Add" },
            {typeof(DtoLevel),"http://bacai.ddns.net/api/EduMan/Level/Add" },
            {typeof(DtoRoleAssign),"http://bacai.ddns.net/api/EduMan/RoleAssign/Add" },
            {typeof(DtoStartWeek),"http://bacai.ddns.net/api/EduMan/StartWeek/Add" },
            {typeof(DtoStudent),"http://bacai.ddns.net/api/EduMan/Student/Add" },
            {typeof(DtoStudentDiscipline),"http://bacai.ddns.net/api/EduMan/StudentDiscipline/Add" },
            {typeof(DtoStudentDistribute),"http://bacai.ddns.net/api/EduMan/StudentDistribute/Add" },
            {typeof(DtoTeacher),"http://bacai.ddns.net/api/EduMan/Teacher/Add" },
            {typeof(DtoUserInfo),"http://bacai.ddns.net/api/EduMan/UserInfo/Add" },
            {typeof(DtoWeekly),"http://bacai.ddns.net/api/EduMan/Weekly/Add" }
        };
        public Dictionary<Type, string> UrlUpdate = new()
        {
            {typeof(DtoClass),"http://bacai.ddns.net/api/EduMan/Class/Update" },
            {typeof(DtoClassDiscipline),"http://bacai.ddns.net/api/EduMan/ClassDiscipline/Update" },
            {typeof(DtoClassDistribute),"http://bacai.ddns.net/api/EduMan/ClassDistribute/Update" },
            {typeof(DtoDiscipline),"http://bacai.ddns.net/api/EduMan/Discipline/Update" },
            {typeof(DtoDisciplineGroup),"http://bacai.ddns.net/api/EduMan/DisciplineGroup/Update" },
            {typeof(DtoDisciplineType),"http://bacai.ddns.net/api/EduMan/DisciplineType/Update" },
            {typeof(DtoDroppedOut),"http://bacai.ddns.net/api/EduMan/DroppedOut/Update" },
            {typeof(DtoFunct),"http://bacai.ddns.net/api/EduMan/Funct/Update" },
            {typeof(DtoGrade),"http://bacai.ddns.net/api/EduMan/Grade/Update" },
            {typeof(DtoGroupUser),"http://bacai.ddns.net/api/EduMan/GroupUser/Update" },
            {typeof(DtoLevel),"http://bacai.ddns.net/api/EduMan/Level/Update" },
            {typeof(DtoRoleAssign),"http://bacai.ddns.net/api/EduMan/RoleAssign/Update" },
            {typeof(DtoStartWeek),"http://bacai.ddns.net/api/EduMan/StartWeek/Update" },
            {typeof(DtoStudent),"http://bacai.ddns.net/api/EduMan/Student/Update" },
            {typeof(DtoStudentDiscipline),"http://bacai.ddns.net/api/EduMan/StudentDiscipline/Update" },
            {typeof(DtoStudentDistribute),"http://bacai.ddns.net/api/EduMan/StudentDistribute/Update" },
            {typeof(DtoTeacher),"http://bacai.ddns.net/api/EduMan/Teacher/Update" },
            {typeof(DtoUserInfo),"http://bacai.ddns.net/api/EduMan/UserInfo/Update" },
            {typeof(DtoWeekly),"http://bacai.ddns.net/api/EduMan/Weekly/Update" }
        };
        public Dictionary<Type, string> UrlDelete = new()
        {
            {typeof(DtoClass),"http://bacai.ddns.net/api/EduMan/Class/Delete" },
            {typeof(DtoClassDiscipline),"http://bacai.ddns.net/api/EduMan/ClassDiscipline/Delete" },
            {typeof(DtoClassDistribute),"http://bacai.ddns.net/api/EduMan/ClassDistribute/Delete" },
            {typeof(DtoDiscipline),"http://bacai.ddns.net/api/EduMan/Discipline/Delete" },
            {typeof(DtoDisciplineGroup),"http://bacai.ddns.net/api/EduMan/DisciplineGroup/Delete" },
            {typeof(DtoDisciplineType),"http://bacai.ddns.net/api/EduMan/DisciplineType/Delete" },
            {typeof(DtoDroppedOut),"http://bacai.ddns.net/api/EduMan/DroppedOut/Delete" },
            {typeof(DtoFunct),"http://bacai.ddns.net/api/EduMan/Funct/Delete" },
            {typeof(DtoGrade),"http://bacai.ddns.net/api/EduMan/Grade/Delete" },
            {typeof(DtoGroupUser),"http://bacai.ddns.net/api/EduMan/GroupUser/Delete" },
            {typeof(DtoLevel),"http://bacai.ddns.net/api/EduMan/Level/Delete" },
            {typeof(DtoRoleAssign),"http://bacai.ddns.net/api/EduMan/RoleAssign/Delete" },
            {typeof(DtoStartWeek),"http://bacai.ddns.net/api/EduMan/StartWeek/Delete" },
            {typeof(DtoStudent),"http://bacai.ddns.net/api/EduMan/Student/Delete" },
            {typeof(DtoStudentDiscipline),"http://bacai.ddns.net/api/EduMan/StudentDiscipline/Delete" },
            {typeof(DtoStudentDistribute),"http://bacai.ddns.net/api/EduMan/StudentDistribute/Delete" },
            {typeof(DtoTeacher),"http://bacai.ddns.net/api/EduMan/Teacher/Delete" },
            {typeof(DtoUserInfo),"http://bacai.ddns.net/api/EduMan/UserInfo/Delete" },
            {typeof(DtoWeekly),"http://bacai.ddns.net/api/EduMan/Weekly/Delete" }
        };
    }
}
