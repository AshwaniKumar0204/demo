(function () {
    'use strict';

    angular
        .module('app')
        .controller('AdmissionListController', AdmissionListController);

    AdmissionListController.$inject = ['$scope', 'adminService', 'Factory', 'ConstantData', '$location'];

    function AdmissionListController($scope, adminService, Factory, ConstantData, $location) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[0];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.GenderList = ConstantData.Gender;
        $scope.CategoryList = ConstantData.Category;
        $scope.BloodGroupList = ConstantData.BloodGroup;
        $scope.ClassId = 0;
        $scope.ExcelStudentSelectionModel = {};


        $scope.getAcademicSessionList = function () {
            if ($scope.ClassId == 0) {
                $scope.AcademicSessionList = [];
                $scope.AcademicSessionId = null;
                return;
            }
            var obj = {
                ClassId: $scope.ClassId
            }
            $scope.dataLoading = true;
            adminService.getAcademicSessionList(obj)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.AcademicSessionList = response.data.AcademicSessionList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getAcademicSessionList();

        $scope.getClassList = function () {
            $scope.dataLoading = true;
            adminService.getClassList(ConstantData.ClassStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ClassList = response.data.ClassList;
                        var index = { ClassId: 0, ClassName: '--All Courses--' };
                        $scope.ClassList.unshift(index);
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getClassList();

        $scope.getAdmissionFromList = function () {
            $scope.dataLoading = true;
            adminService.getAdmissionFromList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.AdmissionFromList = response.data.AdmissionFromList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getAdmissionFromList();

        $scope.changeClass = function () {
            $scope.getAcademicSessionList();
            $scope.SectionList = [];
            angular.forEach($scope.MainSectionList, function (section, key) {
                if (section.ClassId == $scope.ClassId) {
                    $scope.SectionList.push(section);
                }
            })
            var index = { SectionId: 0, SectionName: '--All Branch--' };
            $scope.SectionList.unshift(index);
            $scope.SectionId = 0;
            //$scope.filterList();
        }

        $scope.filterList = function () {
            $scope.AdmissionList = [];
            angular.forEach($scope.MainAdmissionList, function (admission, key) {
                if ($scope.ClassId == 0) {
                    $scope.AdmissionList.push(admission);
                } else if ($scope.ClassId == admission.ClassId) {
                    if ($scope.SectionId == 0) {
                        $scope.AdmissionList.push(admission);
                    } else if (admission.SectionId == $scope.SectionId) {
                        $scope.AdmissionList.push(admission);
                    }
                }
            });
        }

        $scope.getSectionList = function () {
            $scope.dataLoading = true;
            adminService.getSectionList(ConstantData.SectionStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.MainSectionList = response.data.SectionList;
                        $scope.changeClass();
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getSectionList();

        $scope.getAdmissionList = function () {
            //if ($scope.ClassId == 0) {
            //    toastr.error("Course is required!!");
            //    return;
            //}
            //if ($scope.AcademicSessionId == null) {
            //    toastr.error("Session is required!!");
            //    return;
            //}
            $scope.dataLoading = true;
            adminService.getAdmissionList($scope.AcademicSessionId, $scope.ClassId, $scope.SectionId, $scope.AdmissionFrom)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.AdmissionList = response.data.AdmissionList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.viewStudentPhoto = function (PhotoUrl) {
            $scope.PhotoUrl = PhotoUrl;
            $('#modal_photo').modal('show');
        }

        $scope.editStudentDetail = function (admissionId) {
            sessionStorage.setItem('editAdmissionId', admissionId);
            sessionStorage.setItem('backURL', 'AdmissionList');
            $location.path('/StudentDetailUpdate');
        }

        $scope.exportToExcel = function () {
            window.open("/admin/ExcelAdmissionList?AcademicSessionId=" + $scope.AcademicSessionId + "&ClassId=" + $scope.ClassId + "&SectionId=" + $scope.SectionId);
        }

        $scope.tablesToExcel = function (tables, wsnames, wbname, appname) {
            var initailPage = $scope.pageSize;
            $scope.pageSize = $scope.sizeOptions[$scope.sizeOptions.length - 1];
            $scope.IsExporting = true;
            setTimeout(function () {
                tablesToExcel(tables, wsnames, wbname, appname);
            }, 500)
            setTimeout(function () {
                $scope.IsExporting = false;
                $scope.pageSize = initailPage;
                $scope.$apply();
            }, 1000)
        }

        $scope.checkedAll = function () {
            if ($scope.IsCheckedAll) {
                $scope.IsSLNo = true;
                $scope.IsHouseName = true;
                $scope.IsSessionName = true;
                $scope.IsFullName = true;
                $scope.IsAdmissionNo = true;
                $scope.IsClassName = true;
                $scope.IsSectionName = true;
                $scope.IsRollNo = true;
                $scope.IsDateOfBirth = true;
                $scope.IsGender = true;
                $scope.IsFatherName = true;
                $scope.IsMotherName = true;
                $scope.IsFatherOccupation = true;
                $scope.IsFatherAadhaarNo = true;
                $scope.IsCategory = true;
                $scope.IsBloodGroup = true;
                $scope.IsCorrespondenceAddressLine1 = true;
                $scope.IsCorrespondenceAddressLine2 = true;
                $scope.IsCorrespondenceCityName = true;
                $scope.IsCorrespondenceStateName = true;
                $scope.IsCorrespondecePinCode = true;
                $scope.IsPermanentAddressLine1 = true;
                $scope.IsPermanentAddressLine2 = true;
                $scope.IsPermanentCityName = true;
                $scope.IsPermanentStateName = true;
                $scope.IsPermanentPinCode = true;
                $scope.IsMobileNo = true;
                $scope.IsAlternateNo = true;
                $scope.IsEmail = true;
                $scope.IsAadhaarNo = true;
                $scope.IsFamilyAnnualIncome = true;
                $scope.IsAdmissionDate = true;
                $scope.IsPreviousSchoolName = true;
                $scope.IsPreviousBoardName = true;
                $scope.IsLastClassAttend = true;
                $scope.IsMediumOfInstruction = true;
                $scope.IsPreviousSchoolTCNo = true;
                $scope.IsPreviousSchoolTCIssueDate = true;
                $scope.IsHeight = true;
                $scope.IsWeight = true;
                $scope.IsJoiningDate = true;
                $scope.IsRemarks = true;
            } else {
                $scope.IsSLNo = false;
                $scope.IsHouseName = false;
                $scope.IsSessionName = false;
                $scope.IsFullName = false;
                $scope.IsAdmissionNo = false;
                $scope.IsClassName = false;
                $scope.IsSectionName = false;
                $scope.IsRollNo = false;
                $scope.IsDateOfBirth = false;
                $scope.IsGender = false;
                $scope.IsFatherName = false;
                $scope.IsFatherOccupation = false;
                $scope.IsFatherAadhaarNo = false;
                $scope.IsCategory = false;
                $scope.IsBloodGroup = false;
                $scope.IsCorrespondenceAddressLine1 = false;
                $scope.IsCorrespondenceAddressLine2 = false;
                $scope.IsCorrespondenceCityName = false;
                $scope.IsCorrespondenceStateName = false;
                $scope.IsCorrespondecePinCode = false;
                $scope.IsPermanentAddressLine1 = false;
                $scope.IsPermanentAddressLine2 = false;
                $scope.IsPermanentCityName = false;
                $scope.IsPermanentStateName = false;
                $scope.IsPermanentPinCode = false;
                $scope.IsMobileNo = false;
                $scope.IsAlternateNo = false;
                $scope.IsEmail = false;
                $scope.IsAadhaarNo = false;
                $scope.IsFamilyAnnualIncome = false;
                $scope.IsAdmissionDate = false;
                $scope.IsPreviousSchoolName = false;
                $scope.IsPreviousBoardName = false;
                $scope.IsLastClassAttend = false;
                $scope.IsMediumOfInstruction = false;
                $scope.IsPreviousSchoolTCNo = false;
                $scope.IsPreviousSchoolTCIssueDate = false;
                $scope.IsHeight = false;
                $scope.IsWeight = false;
                $scope.IsJoiningDate = false;
                $scope.IsRemarks = false;
            }
        }
    }
})();
