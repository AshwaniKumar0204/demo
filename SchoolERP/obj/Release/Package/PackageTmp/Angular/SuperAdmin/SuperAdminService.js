(function () {
    "use strict";

    angular.module("app")
        .service("superAdminService", superAdminService);

    superAdminService.$inject = ["$http"];

    function superAdminService($http) {
        this.checkSysytemLogin = function () {
            return $http.get("/api/systemLogin/checkSysytemLogin");
        }

        this.logoutSystemLogin = function () {
            return $http.get("/api/systemLogin/Logout");
        }

        //========================================Staff==========================================================
        this.getStaffList = function (Status, DepartmentId) {
            if (Status == null || Status == undefined)
                Status = 0;
            if (DepartmentId == null || DepartmentId == undefined)
                DepartmentId = 0;
            return $http.get("/api/Staff/StaffList", { params: { Status: Status, DepartmentId: DepartmentId } });
        }

        //-------------------------------------Academic Session--------------------------------------------------
        this.getAcademicSessionList = function (SessionStatus) {
            return $http.get("/api/AcademicSession/AcademicSessionList", { params: { SessionStatus: SessionStatus } });
        }

        this.getFeeDueReport = function (SectionId, UptoMonthNo, AcademicSessionId) {
            if (SectionId == null || SectionId == undefined)
                SectionId = 0;
            if (UptoMonthNo == null || UptoMonthNo == undefined)
                UptoMonthNo = 0;
            if (AcademicSessionId == null || AcademicSessionId == undefined)
                AcademicSessionId = 0;
            return $http.get("/api/report/FeeDueReport", { params: { SectionId: SectionId, UptoMonthNo: UptoMonthNo, AcademicSessionId: AcademicSessionId } });
        }

        this.getFeeDueReportExcel = function (SectionId, UptoMonthNo, AcademicSessionId) {
            if (SectionId == null || SectionId == undefined)
                SectionId = 0;
            if (UptoMonthNo == null || UptoMonthNo == undefined)
                UptoMonthNo = 0;
            if (AcademicSessionId == null || AcademicSessionId == undefined)
                AcademicSessionId = 0;
            return $http.get("/api/report/FeeDueReportExcel", { params: { SectionId: SectionId, UptoMonthNo: UptoMonthNo, AcademicSessionId: AcademicSessionId } });
        }

        //------------------------------------Month----------------------------------------
        this.getMonthList = function () {
            return $http.get("/api/month/monthlist");
        }

        this.getClassList = function (ClassStatus) {
            return $http.get("/api/class/classlist", { params: { ClassStatus: ClassStatus } });
        }

        this.getSectionList = function (SectionStatus, ClassId) {
            if (ClassId == null)
                ClassId = 0;
            return $http.get("/api/section/SectionList2", { params: { SectionStatus: SectionStatus, ClassId: ClassId } });
        }

        //----------------------------------------------------Exam Role Section----------------------------------------------------
        this.getAllRoleSectionList = function (RoleId) {
            return $http.get("/api/RoleSection/AllRoleSectionList", { params: { RoleId: RoleId } });
        }

        this.saveRoleSection = function (RoleModel) {
            var response = $http({
                method: "post",
                url: "/api/RoleSection/SaveRoleSection",
                data: JSON.stringify(RoleModel),
                dataType: "json",
            });
            return response;
        };

        //-------------------------Manage Task Master---------------------------------------
        this.getTaskMasterListForMenu = function () {
            return $http.get("/api/Menu/TaskMasterListForMenu");
        }
        this.getMenuList = function () {
            return $http.get("/api/Menu/MenuList");
        }

        this.deleteMenu = function (MenuId) {
            return $http.get("/api/Menu/deleteMenu", { params: { MenuId: MenuId } });
        }

        this.menuUp = function (MenuId) {
            return $http.get("/api/Menu/MenuUp", { params: { MenuId: MenuId } });
        }

        this.menuDown = function (MenuId) {
            return $http.get("/api/Menu/MenuDown", { params: { MenuId: MenuId } });
        }

        this.saveMenu = function (MenuModel) {
            var response = $http({
                method: "post",
                url: "/api/Menu/saveMenu",
                data: JSON.stringify(MenuModel),
                dataType: "json",
            });
            return response;
        };

        //-------------------------Manage Role Task---------------------------------------
        this.getAllRoleTaskList = function (Status, RoleId) {
            return $http.get("/api/RoleTask/AllRoleTaskList", { params: { Status: Status, RoleId: RoleId } });
        }

        this.saveRoleTask = function (RoleModel) {
            var response = $http({
                method: "post",
                url: "/api/RoleTask/saveRoleTask",
                data: JSON.stringify(RoleModel),
                dataType: "json",
            });
            return response;
        };


        //-------------------------Manage Emplyee Type---------------------------------------
        this.getEmployeeTypeList = function (Status) {
            return $http.get("/api/EmployeeType/EmployeeTypeList", { params: { Status: Status } });
        }

        this.deleteEmployeeType = function (EmployeeTypeId) {
            return $http.get("/api/EmployeeType/deleteEmployeeType", { params: { EmployeeTypeId: EmployeeTypeId } });
        }

        this.saveEmployeeType = function (EmployeeTypeModel) {
            var response = $http({
                method: "post",
                url: "/api/EmployeeType/saveEmployeeType",
                data: JSON.stringify(EmployeeTypeModel),
                dataType: "json",
            });
            return response;
        };

        //-------------------------Manage Task Group---------------------------------------
        this.getTaskGroupList = function () {
            return $http.get("/api/TaskGroup/TaskGroupList");
        }

        this.deleteTaskGroup = function (TaskGroupId) {
            return $http.get("/api/TaskGroup/deleteTaskGroup", { params: { TaskGroupId: TaskGroupId } });
        }

        this.saveTaskGroup = function (TaskGroupModel) {
            var response = $http({
                method: "post",
                url: "/api/TaskGroup/saveTaskGroup",
                data: JSON.stringify(TaskGroupModel),
                dataType: "json",
            });
            return response;
        };

        //-------------------------Manage Task Master---------------------------------------
        this.getTaskMasterList = function (Status) {
            return $http.get("/api/TaskMaster/TaskMasterList", { params: { Status: Status } });
        }

        this.deleteTaskMaster = function (TaskMasterId) {
            return $http.get("/api/TaskMaster/deleteTaskMaster", { params: { TaskMasterId: TaskMasterId } });
        }

        this.saveTaskMaster = function (TaskMasterModel) {
            var response = $http({
                method: "post",
                url: "/api/TaskMaster/saveTaskMaster",
                data: JSON.stringify(TaskMasterModel),
                dataType: "json",
            });
            return response;
        };

        //-------------------------Manage Role---------------------------------------
        this.getRoleList = function (Status) {
            return $http.get("/api/Role/RoleList", { params: { Status: Status } });
        }

        this.deleteRole = function (RoleId) {
            return $http.get("/api/Role/deleteRole", { params: { RoleId: RoleId } });
        }

        this.saveRole = function (RoleModel) {
            var response = $http({
                method: "post",
                url: "/api/Role/saveRole",
                data: JSON.stringify(RoleModel),
                dataType: "json",
            });
            return response;
        };

        //-------------------------Manage School---------------------------------------
        this.getSchoolList = function (Status) {
            return $http.get("/api/School/SchoolList", { params: { Status: Status } });
        }

        this.deleteSchool = function (SchoolId) {
            return $http.get("/api/School/deleteSchool", { params: { SchoolId: SchoolId } });
        }

        this.getSchoolLogo = function (SchoolId) {
            return $http.get("/api/School/SchoolLogo", { params: { SchoolId: SchoolId } });
        }

        this.saveSchool = function (SchoolModel) {
            var response = $http({
                method: "post",
                url: "/api/School/saveSchool",
                data: JSON.stringify(SchoolModel),
                dataType: "json",
            });
            return response;
        };

        //-------------------------Manage Employee---------------------------------------
        this.getEmployeeList = function (Status) {
            return $http.get("/api/Employee/EmployeeList", { params: { Status: Status } });
        }

        this.deleteEmployee = function (EmployeeId) {
            return $http.get("/api/Employee/deleteEmployee", { params: { EmployeeId: EmployeeId } });
        }

        this.saveEmployee = function (EmployeeModel) {
            var response = $http({
                method: "post",
                url: "/api/Employee/saveEmployee",
                data: JSON.stringify(EmployeeModel),
                dataType: "json",
            });
            return response;
        };

        //-------------------------Manage System Login---------------------------------------
        this.getSystemLoginList = function (Status, EmployeeId) {
            return $http.get("/api/SystemLogin/SystemLoginList", { params: { Status: Status, EmployeeId: EmployeeId } });
        }

        this.deleteSystemLogin = function (LoginId) {
            return $http.get("/api/SystemLogin/deleteSystemLogin", { params: { LoginId: LoginId } });
        }

        this.saveSystemLogin = function (SystemLoginModel) {
            var response = $http({
                method: "post",
                url: "/api/SystemLogin/saveSystemLogin",
                data: JSON.stringify(SystemLoginModel),
                dataType: "json",
            });
            return response;
        };
    }
}());