using EduManModel.Dtos;

namespace EduManModel
{
    public partial class DataProcess<T>
    {
        public Dictionary<Type, string> UrlGetAll = new()
        {
            {typeof(DtoClass),"api/EduMan/Class/GetAll" },
            {typeof(DtoClassDiscipline),"api/EduMan/ClassDiscipline/GetAll" },
            {typeof(DtoClassDistribute),"api/EduMan/ClassDistribute/GetAll" },
            {typeof(DtoDiscipline),"api/EduMan/Discipline/GetAll" },
            {typeof(DtoDisciplineGroup),"api/EduMan/DisciplineGroup/GetAll" },
            {typeof(DtoDisciplineType),"api/EduMan/DisciplineType/GetAll" },
            {typeof(DtoDroppedOut),"api/EduMan/DroppedOut/GetAll" },
            {typeof(DtoFunct),"api/EduMan/Funct/GetAll" },
            {typeof(DtoGrade),"api/EduMan/Grade/GetAll" },
            {typeof(DtoGroupUser),"api/EduMan/GroupUser/GetAll" },
            {typeof(DtoLevel),"api/EduMan/Level/GetAll" },
            {typeof(DtoRoleAssign),"api/EduMan/RoleAssign/GetAll" },
            {typeof(DtoStartWeek),"api/EduMan/StartWeek/GetAll" },
            {typeof(DtoStudent),"api/EduMan/Student/GetAll" },
            {typeof(DtoStudentDiscipline),"api/EduMan/StudentDiscipline/GetAll" },
            {typeof(DtoStudentDistribute),"api/EduMan/StudentDistribute/GetAll" },
            {typeof(DtoTeacher),"api/EduMan/Teacher/GetAll" },
            {typeof(DtoUserInfo),"api/EduMan/UserInfo/GetAll" },
            {typeof(DtoWeekly),"api/EduMan/Weekly/GetAll" }
        };
        public Dictionary<Type, string> UrlGetOne = new()
        {
            {typeof(DtoClass),"api/EduMan/Class/GetOne" },
            {typeof(DtoClassDiscipline),"api/EduMan/ClassDiscipline/GetOne" },
            {typeof(DtoClassDistribute),"api/EduMan/ClassDistribute/GetOne" },
            {typeof(DtoDiscipline),"api/EduMan/Discipline/GetOne" },
            {typeof(DtoDisciplineGroup),"api/EduMan/DisciplineGroup/GetOne" },
            {typeof(DtoDisciplineType),"api/EduMan/DisciplineType/GetOne" },
            {typeof(DtoDroppedOut),"api/EduMan/DroppedOut/GetOne" },
            {typeof(DtoFunct),"api/EduMan/Funct/GetOne" },
            {typeof(DtoGrade),"api/EduMan/Grade/GetOne" },
            {typeof(DtoGroupUser),"api/EduMan/GroupUser/GetOne" },
            {typeof(DtoLevel),"api/EduMan/Level/GetOne" },
            {typeof(DtoRoleAssign),"api/EduMan/RoleAssign/GetOne" },
            {typeof(DtoStartWeek),"api/EduMan/StartWeek/GetOne" },
            {typeof(DtoStudent),"api/EduMan/Student/GetOne" },
            {typeof(DtoStudentDiscipline),"api/EduMan/StudentDiscipline/GetOne" },
            {typeof(DtoStudentDistribute),"api/EduMan/StudentDistribute/GetOne" },
            {typeof(DtoTeacher),"api/EduMan/Teacher/GetOne" },
            {typeof(DtoUserInfo),"api/EduMan/UserInfo/GetOne" },
            {typeof(DtoWeekly),"api/EduMan/Weekly/GetOne" }
        };
        public Dictionary<Type, string> UrlFind = new()
        {
            {typeof(DtoClass),"api/EduMan/Class/Find" },
            {typeof(DtoClassDiscipline),"api/EduMan/ClassDiscipline/Find" },
            {typeof(DtoClassDistribute),"api/EduMan/ClassDistribute/Find" },
            {typeof(DtoDiscipline),"api/EduMan/Discipline/Find" },
            {typeof(DtoDisciplineGroup),"api/EduMan/DisciplineGroup/Find" },
            {typeof(DtoDisciplineType),"api/EduMan/DisciplineType/Find" },
            {typeof(DtoDroppedOut),"api/EduMan/DroppedOut/Find" },
            {typeof(DtoFunct),"api/EduMan/Funct/Find" },
            {typeof(DtoGrade),"api/EduMan/Grade/Find" },
            {typeof(DtoGroupUser),"api/EduMan/GroupUser/Find" },
            {typeof(DtoLevel),"api/EduMan/Level/Find" },
            {typeof(DtoRoleAssign),"api/EduMan/RoleAssign/Find" },
            {typeof(DtoStartWeek),"api/EduMan/StartWeek/Find" },
            {typeof(DtoStudent),"api/EduMan/Student/Find" },
            {typeof(DtoStudentDiscipline),"api/EduMan/StudentDiscipline/Find" },
            {typeof(DtoStudentDistribute),"api/EduMan/StudentDistribute/Find" },
            {typeof(DtoTeacher),"api/EduMan/Teacher/Find" },
            {typeof(DtoUserInfo),"api/EduMan/UserInfo/Find" },
            {typeof(DtoWeekly),"api/EduMan/Weekly/Find" }
        };
        public Dictionary<Type, string> UrlAdd = new()
        {
            {typeof(DtoClass),"api/EduMan/Class/Add" },
            {typeof(DtoClassDiscipline),"api/EduMan/ClassDiscipline/Add" },
            {typeof(DtoClassDistribute),"api/EduMan/ClassDistribute/Add" },
            {typeof(DtoDiscipline),"api/EduMan/Discipline/Add" },
            {typeof(DtoDisciplineGroup),"api/EduMan/DisciplineGroup/Add" },
            {typeof(DtoDisciplineType),"api/EduMan/DisciplineType/Add" },
            {typeof(DtoDroppedOut),"api/EduMan/DroppedOut/Add" },
            {typeof(DtoFunct),"api/EduMan/Funct/Add" },
            {typeof(DtoGrade),"api/EduMan/Grade/Add" },
            {typeof(DtoGroupUser),"api/EduMan/GroupUser/Add" },
            {typeof(DtoLevel),"api/EduMan/Level/Add" },
            {typeof(DtoRoleAssign),"api/EduMan/RoleAssign/Add" },
            {typeof(DtoStartWeek),"api/EduMan/StartWeek/Add" },
            {typeof(DtoStudent),"api/EduMan/Student/Add" },
            {typeof(DtoStudentDiscipline),"api/EduMan/StudentDiscipline/Add" },
            {typeof(DtoStudentDistribute),"api/EduMan/StudentDistribute/Add" },
            {typeof(DtoTeacher),"api/EduMan/Teacher/Add" },
            {typeof(DtoUserInfo),"api/EduMan/UserInfo/Add" },
            {typeof(DtoWeekly),"api/EduMan/Weekly/Add" }
        };
        public Dictionary<Type, string> UrlUpdate = new()
        {
            {typeof(DtoClass),"api/EduMan/Class/Update" },
            {typeof(DtoClassDiscipline),"api/EduMan/ClassDiscipline/Update" },
            {typeof(DtoClassDistribute),"api/EduMan/ClassDistribute/Update" },
            {typeof(DtoDiscipline),"api/EduMan/Discipline/Update" },
            {typeof(DtoDisciplineGroup),"api/EduMan/DisciplineGroup/Update" },
            {typeof(DtoDisciplineType),"api/EduMan/DisciplineType/Update" },
            {typeof(DtoDroppedOut),"api/EduMan/DroppedOut/Update" },
            {typeof(DtoFunct),"api/EduMan/Funct/Update" },
            {typeof(DtoGrade),"api/EduMan/Grade/Update" },
            {typeof(DtoGroupUser),"api/EduMan/GroupUser/Update" },
            {typeof(DtoLevel),"api/EduMan/Level/Update" },
            {typeof(DtoRoleAssign),"api/EduMan/RoleAssign/Update" },
            {typeof(DtoStartWeek),"api/EduMan/StartWeek/Update" },
            {typeof(DtoStudent),"api/EduMan/Student/Update" },
            {typeof(DtoStudentDiscipline),"api/EduMan/StudentDiscipline/Update" },
            {typeof(DtoStudentDistribute),"api/EduMan/StudentDistribute/Update" },
            {typeof(DtoTeacher),"api/EduMan/Teacher/Update" },
            {typeof(DtoUserInfo),"api/EduMan/UserInfo/Update" },
            {typeof(DtoWeekly),"api/EduMan/Weekly/Update" }
        };
        public Dictionary<Type, string> UrlDelete = new()
        {
            {typeof(DtoClass),"api/EduMan/Class/Delete" },
            {typeof(DtoClassDiscipline),"api/EduMan/ClassDiscipline/Delete" },
            {typeof(DtoClassDistribute),"api/EduMan/ClassDistribute/Delete" },
            {typeof(DtoDiscipline),"api/EduMan/Discipline/Delete" },
            {typeof(DtoDisciplineGroup),"api/EduMan/DisciplineGroup/Delete" },
            {typeof(DtoDisciplineType),"api/EduMan/DisciplineType/Delete" },
            {typeof(DtoDroppedOut),"api/EduMan/DroppedOut/Delete" },
            {typeof(DtoFunct),"api/EduMan/Funct/Delete" },
            {typeof(DtoGrade),"api/EduMan/Grade/Delete" },
            {typeof(DtoGroupUser),"api/EduMan/GroupUser/Delete" },
            {typeof(DtoLevel),"api/EduMan/Level/Delete" },
            {typeof(DtoRoleAssign),"api/EduMan/RoleAssign/Delete" },
            {typeof(DtoStartWeek),"api/EduMan/StartWeek/Delete" },
            {typeof(DtoStudent),"api/EduMan/Student/Delete" },
            {typeof(DtoStudentDiscipline),"api/EduMan/StudentDiscipline/Delete" },
            {typeof(DtoStudentDistribute),"api/EduMan/StudentDistribute/Delete" },
            {typeof(DtoTeacher),"api/EduMan/Teacher/Delete" },
            {typeof(DtoUserInfo),"api/EduMan/UserInfo/Delete" },
            {typeof(DtoWeekly),"api/EduMan/Weekly/Delete" }
        };
    }
}
