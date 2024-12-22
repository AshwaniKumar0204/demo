(function () {
    'use strict';

    angular.
        module('app').
        config(['$routeProvider',
            function config($routeProvider) {
                $routeProvider.when("/", {
                    templateUrl: "/superadmin/index",
                    controller: "DashboardController",
                }).when("/Index", {
                    templateUrl: "/superadmin/index",
                    controller: "DashboardController",
                }).when("/FeeDueReport", {
                    templateUrl: "/superadmin/FeeDueReport",
                    controller: "FeeDueReportController",
                }).when("/ManageTaskGroup", {
                    templateUrl: "/superadmin/ManageTaskGroup",
                    controller: "ManageTaskGroupController",
                }).when("/EmployeeType", {
                    templateUrl: "/superadmin/EmployeeType",
                    controller: "EmployeeTypeController",
                }).when("/ManageSchool", {
                    templateUrl: "/superadmin/ManageSchool",
                    controller: "ManageSchoolController",
                }).when("/ManageEmployee", {
                    templateUrl: "/superadmin/ManageEmployee",
                    controller: "ManageEmployeeController",
                }).when("/ManageSysytemLogin", {
                    templateUrl: "/superadmin/ManageSysytemLogin",
                    controller: "ManageSystemLoginController",
                }).when("/ManageTaskMaster", {
                    templateUrl: "/superadmin/ManageTaskMaster",
                    controller: "ManageTaskMasterController",
                }).when("/ManageRole", {
                    templateUrl: "/superadmin/ManageRole",
                    controller: "ManageRoleController",
                }).when("/ManageRoleTask", {
                    templateUrl: "/superadmin/ManageRoleTask",
                    controller: "ManageRoleTaskController",
                }).when("/ManageMenu", {
                    templateUrl: "/superadmin/ManageMenu",
                    controller: "ManageMenuController",
                }).when("/ManageRoleSection", {
                    templateUrl: "/superadmin/ManageRoleSection",
                    controller: "ManageRoleSectionController",
                }).when("/ChangePassword", {
                    templateUrl: "/superadmin/ChangePassword",
                    //controller: "ChangePasswordController",
                }).when("/InvalidUrl", {
                    templateUrl: "/site/InvalidUrl",
                }).otherwise({
                    redirectTo: "/InvalidUrl"
                });
            }
        ]);
}());