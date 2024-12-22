(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageTeacherController', ManageTeacherController);

    ManageTeacherController.$inject = ["$scope", "adminService", "ConstantData", "Factory", "loadDataService"];

    function ManageTeacherController($scope, adminService, ConstantData, Factory, loadDataService) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.SubjectId = null;
        $scope.TeacherSubject = {};
        $scope.TeacherSubject.SubjectId = null;
        $scope.TeacherSubject.ClassId = null;
        $scope.TeacherSubject.SectionId = null;
        $scope.TeacherSubject.TeacherSubjectStatus = null;
        $scope.TeacherStatusList = ConstantData.TeacherStatus;
        $scope.GenderList = ConstantData.Gender;
        $scope.TeacherSubjectStatusList = ConstantData.TeacherSubjectStatus;
        $scope.TeacherPeriod = {};
        $scope.TeacherPeriod.PeriodNo = null;

        $scope.getClassList = function () {
            $scope.dataLoading = true;
            adminService.getClassList(ConstantData.ClassStatus[1].Key)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ClassList = response.data.ClassList;
                        var index = { ClassId: null, ClassName: '--Select Class--' };
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

        $scope.getClassSubjectList = function () {
            if (!$scope.IsUpdate)
                $scope.TeacherSubject.SubjectId = null;
            if ($scope.TeacherSubject.SectionId == null) {
                $scope.SubjectList = [];
                var index = { SubjectId: null, SubjectName: '--Select Subject--' };
                $scope.SubjectList.unshift(index);
                return;
            }
            $scope.dataLoading = true;
            adminService.getClassSubjectList($scope.TeacherSubject.SectionId, 1,3)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.SubjectList = response.data.ClassSubjectList;
                        var index = { SubjectId: null, SubjectName: '--Select Subject--' };
                        $scope.SubjectList.unshift(index);
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getClassSubjectList();

        $scope.changeClass = function () {
            $scope.SectionList = [];
            angular.forEach($scope.MainSectionList, function (section, key) {
                if (section.ClassId == $scope.TeacherSubject.ClassId) {
                    $scope.SectionList.push(section);
                }
            })
            var index = { SectionId: null, SectionName: '--Select Section--' };
            $scope.SectionList.unshift(index);
            $scope.SectionId = null;
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

        $scope.getPeriodList = function () {
            $scope.dataLoading = true;
            adminService.getPeriodList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.PeriodList = response.data.PeriodList;
                        var index = { PeriodNo: null, PeriodName: '--Select Period--' };
                        $scope.PeriodList.unshift(index);
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getPeriodList();

        $scope.getTeacherList = function () {
            $scope.dataLoading = true;
            adminService.getTeacherList(0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.TeacherList = response.data.TeacherList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getTeacherList();


        $scope.$watch('formTeacher.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.editTeacherSubject = function (TeacherSubject, index) {
            $scope.TeacherSubject.ClassId = TeacherSubject.ClassId;
            $scope.TeacherSubject.TeacherSubjectId = TeacherSubject.TeacherSubjectId;
            $scope.TeacherSubject.SectionId = TeacherSubject.SectionId;
            $scope.TeacherSubject.SubjectId = TeacherSubject.SubjectId;
            $scope.TeacherSubject.NoOfPeriods = TeacherSubject.NoOfPeriods;
            $scope.TeacherSubject.PracticalPeriods = TeacherSubject.PracticalPeriods;
            $scope.TeacherSubject.TeacherSubjectStatus = TeacherSubject.TeacherSubjectStatus;
            $scope.TeacherSubject.IsIncluded = TeacherSubject.IsIncluded;
            $scope.IsUpdate = true;
            $scope.TeacherSubjectIndex = index;
            $scope.changeClass();
            $scope.getClassSubjectList();
        }
        $scope.addTeacherSubject = function () {
            if ($scope.TeacherSubject.SectionId == null) {
                toastr.error("Section is required !!");
                return;
            } if ($scope.TeacherSubject.SubjectId == null) {
                toastr.error("Subject is required !!");
                return;
            }

            if ($scope.TeacherSubject.TeacherSubjectStatus == null) {
                toastr.error("Status is required !!");
                return;
            }
            var i = 0, SelectedSubject = {}, SelectedClass = {}, SelectedSection = {};
            //Get Selected Subject Information
            for (i = 0; i < $scope.SubjectList.length; i++) {
                if ($scope.TeacherSubject.SubjectId == $scope.SubjectList[i].SubjectId) {
                    SelectedSubject = $scope.SubjectList[i];
                    break;
                }
            }

            //Get Selected Class Information
            for (i = 0; i < $scope.ClassList.length; i++) {
                if ($scope.TeacherSubject.ClassId == $scope.ClassList[i].ClassId) {
                    SelectedClass = $scope.ClassList[i];
                    break;
                }
            }
            //Get Selected Section Information
            for (i = 0; i < $scope.SectionList.length; i++) {
                if ($scope.TeacherSubject.SectionId == $scope.SectionList[i].SectionId) {
                    SelectedSection = $scope.SectionList[i];
                    break;
                }
            }

            //Check Wheather Selected Class or Subject is already added or not
            if (!$scope.IsUpdate) {
                for (i = 0; i < $scope.Teacher.TeacherSubjectList.length; i++) {
                    var teacherSubject = $scope.Teacher.TeacherSubjectList[i];
                    if ($scope.TeacherSubject.SubjectId == teacherSubject.SubjectId && $scope.TeacherSubject.SectionId == teacherSubject.SectionId) {
                        toastr.error("This subject is already added.");
                        return;
                    }
                }
                var TeacherSubject = {
                    TeacherSubjectId: 0,
                    SubjectId: SelectedSubject.SubjectId,
                    SubjectName: SelectedSubject.SubjectName,
                    SubjectCode: SelectedSubject.SubjectCode,
                    ClassId: SelectedClass.ClassId,
                    ClassName: SelectedClass.ClassName,
                    SectionId: SelectedSection.SectionId,
                    SectionName: SelectedSection.SectionName,
                    NoOfPeriods: $scope.TeacherSubject.NoOfPeriods,
                    PracticalPeriods: $scope.TeacherSubject.PracticalPeriods,
                    TeacherSubjectStatus: $scope.TeacherSubject.TeacherSubjectStatus,
                    IsIncluded: $scope.TeacherSubject.IsIncluded,
                }
                $scope.Teacher.TeacherSubjectList.push(TeacherSubject);
            } else {
                //for (i = 0; i < $scope.Teacher.TeacherSubjectList.length; i++) {
                var teacherSubject = $scope.Teacher.TeacherSubjectList[$scope.TeacherSubjectIndex];
                //if ($scope.TeacherSubject.TeacherSubjectId == teacherSubject.TeacherSubjectId) {
                teacherSubject.ClassName = SelectedClass.ClassName;
                teacherSubject.SectionName = SelectedSection.SectionName;
                teacherSubject.SubjectName = SelectedSubject.SubjectName;
                teacherSubject.SubjectCode = SelectedSubject.SubjectCode;
                teacherSubject.ClassId = $scope.TeacherSubject.ClassId;
                teacherSubject.TeacherSubjectId = $scope.TeacherSubject.TeacherSubjectId;
                teacherSubject.SectionId = $scope.TeacherSubject.SectionId;
                teacherSubject.SubjectId = $scope.TeacherSubject.SubjectId;
                teacherSubject.NoOfPeriods = $scope.TeacherSubject.NoOfPeriods;
                teacherSubject.PracticalPeriods = $scope.TeacherSubject.PracticalPeriods;
                teacherSubject.TeacherSubjectStatus = $scope.TeacherSubject.TeacherSubjectStatus;
                teacherSubject.IsIncluded = $scope.TeacherSubject.IsIncluded;
                //break;
                //}
                //}
            }
            $scope.IsUpdate = false;
            $scope.TeacherSubject.TeacherSubjectId = null;
            $scope.TeacherSubject.SubjectId = null;
            //$scope.TeacherSubject.ClassId = null;
            //$scope.TeacherSubject.Section = null;
            //$scope.TeacherSubject.TeacherSubjectStatus = null;
            $scope.TeacherSubject.PracticalPeriods = null;
            $scope.TeacherSubject.NoOfPeriods = null;
            //$scope.TeacherSubject.IsIncluded = false;
        }

        $scope.deleteTeacherSubject = function (index, TeacherSubject) {
            if (TeacherSubject.TeacherSubjectId > 0) {
                $scope.dataLoading = true;
                adminService.deleteTeacherSubject(TeacherSubject.TeacherSubjectId)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("One Subject deleted successfully.", "Deleted Successfully.")
                            $scope.Teacher.TeacherSubjectList.splice(index, 1);
                            setTimeout(function () { $('#modal_Student_Charge').modal('show') }, 1000);
                        } else {
                            toastr.error(response.data.Message);
                        }
                        $scope.dataLoading = false;
                    }, function (err) {
                        toastr.error("Some error occured while fetching data");
                        $scope.dataLoading = false;
                    })
            }
            else {
                $scope.Teacher.TeacherSubjectList.splice(index, 1);
                setTimeout(function () { $('#modal_Student_Charge').modal('show') }, 1000);
            }
        }

        $scope.addTeacherPeriod = function () {
            if (!$scope.IsUpdate) {
                if ($scope.TeacherPeriod.PeriodNo == null) {
                    toastr.error("Selected Period is invalid !!");
                    return;
                }
                var i = 0, SelectedPeriod = {};
                for (i = 0; i < $scope.Teacher.TeacherPeriodList.length; i++) {
                    var teacherPeriod = $scope.Teacher.TeacherPeriodList[i];
                    if ($scope.TeacherPeriod.PeriodNo == teacherPeriod.PeriodNo) {
                        toastr.error("This Period is already added.");
                        return;
                    }
                }
                for (i = 0; i < $scope.PeriodList.length; i++) {
                    if ($scope.TeacherPeriod.PeriodNo == $scope.PeriodList[i].PeriodNo) {
                        SelectedPeriod = $scope.PeriodList[i];
                        break;
                    }
                }
                var TeacherPeriod = {
                    TeacherPeriodId: 0,
                    PeriodNo: SelectedPeriod.PeriodNo,
                    PeriodName: SelectedPeriod.PeriodName,
                }
                $scope.Teacher.TeacherPeriodList.push(TeacherPeriod);
            }
            $scope.TeacherPeriod = [];
            $scope.TeacherPeriod.PeriodNo = null;
        }

        $scope.deleteTeacherPeriod = function (index, TeacherPeriod) {
            if (TeacherPeriod.TeacherPeriodId > 0) {
                $scope.dataLoading = true;
                adminService.deleteTeacherPeriod(TeacherPeriod.TeacherPeriodId)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            toastr.success("One Period deleted successfully.", "Deleted Successfully.")
                            $scope.Teacher.TeacherPeriodList.splice(index, 1);
                            setTimeout(function () { $('#modal_Student_Charge').modal('show') }, 1000);
                        } else {
                            toastr.error(response.data.Message);
                        }
                        $scope.dataLoading = false;
                    }, function (err) {
                        toastr.error("Some error occured while fetching data");
                        $scope.dataLoading = false;
                    })
            }
            else {
                $scope.Teacher.TeacherPeriodList.splice(index, 1);
                setTimeout(function () { $('#modal_Student_Charge').modal('show') }, 1000);
            }
        }

        $scope.getDepartmentList = function () {
            $scope.dataLoading = true;
            adminService.getDepartmentList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.DepartmentList = response.data.DepartmentList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getDepartmentList();

        $scope.saveTeacher = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var TeacherModel = {
                    DepartmentId: $scope.Teacher.DepartmentId,
                    StaffId: $scope.Teacher.StaffId,
                    TeacherId: $scope.Teacher.TeacherId,
                    TeacherStatus: $scope.Teacher.TeacherStatus,
                    TeacherName: $scope.Teacher.TeacherName,
                    IsPartTimeTeacher: $scope.Teacher.IsPartTimeTeacher,
                    Arrangement: $scope.Teacher.Arrangement,
                    TeacherCode: $scope.Teacher.TeacherCode,
                    Gender: $scope.Teacher.Gender,
                    FatherName: $scope.Teacher.FatherName,
                    MobileNo: $scope.Teacher.MobileNo,
                    AlternateNo: $scope.Teacher.AlternateNo,
                    CurrentAddress: $scope.Teacher.CurrentAddress,
                    PermanentAddress: $scope.Teacher.PermanentAddress,
                    Email: $scope.Teacher.Email,
                    Qualification: $scope.Teacher.Qualification,
                    JoinDate: loadDataService.getDateTime($scope.Teacher.JoinDate),
                    TeacherSubjectList: $scope.Teacher.TeacherSubjectList,
                    TeacherPeriodList: $scope.Teacher.TeacherPeriodList,
                }

                $scope.dataLoading = true;
                adminService.saveTeacher(TeacherModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (TeacherModel.TeacherId > 0)
                                toastr.success("Teacher Updated Successfully.");
                            else
                                toastr.success("New Teacher Created Successfully.");
                            $scope.resetForm();
                            $scope.TeacherList = response.data.TeacherList;
                            $('#modal_Student_Charge').modal('hide');
                        } else {
                            toastr.error(response.data.Message);
                        }
                        $scope.dataLoading = false;
                    }, function (err) {
                        toastr.error("Some error occured while fetching data");
                        $scope.dataLoading = false;
                    })

            } else {
                toastr.error("Fill all fields!", "Validation Error");
            }
        }

        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.Teacher = {};
            $scope.Teacher.TeacherSubjectList = [];
            $scope.Teacher.TeacherPeriodList = [];
            $scope.Teacher.TeacherStatus = null;
            $scope.Teacher.Gender = null;
            $scope.Teacher.JoinDate = new Date();
            $scope.formTeacher.$setPristine();
            $scope.formTeacher.$setUntouched();
        }

        $scope.newTeacher = function () {
            $scope.resetForm();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editTeacher = function (TeacherModel) {
            $scope.resetForm();
            $scope.Teacher = TeacherModel;
            $scope.Teacher.MobileNo = parseInt(TeacherModel.MobileNo);
            $scope.Teacher.AlternateNo = parseInt(TeacherModel.AlternateNo);
            $scope.Teacher.JoinDate = new Date(TeacherModel.JoinDate);
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (Record, param1, param2) {
            $scope.Record = Record;
            if (Record == 1) {
                $scope.TeacherId = param1;
            } else if (Record == 2) {
                $scope.param1 = param1;
                $scope.param2 = param2;
            } else if (Record == 3) {
                $scope.param1 = param1;
                $scope.param2 = param2;
            }
            $('#modal_Student_Charge').modal('hide');
            $('#modal_confirmation').modal('show');
            //setTimeout(function () {
            //}, 1000)
        }

        $scope.deleteRecord = function () {
            if ($scope.Record == 1) {
                $scope.deleteTeacher();
            } else if ($scope.Record == 2) {
                $scope.deleteTeacherSubject($scope.param1, $scope.param2);
            } else if ($scope.Record == 3) {
                $scope.deleteTeacherPeriod($scope.param1, $scope.param2);
            }
            $scope.param1 = null;
            $scope.param2 = null;
            $scope.Record = null;
            $('#modal_confirmation').modal('hide');
        }
        $scope.deleteTeacher = function () {
            $scope.dataLoading = true;
            adminService.deleteTeacher($scope.TeacherId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Teacher deleted successfully.", "Deleted Successfully.")
                        $scope.TeacherList = response.data.TeacherList;
                        $scope.TeacherId = null;
                        $('#modal_confirmation').modal('hide');
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
    }
})();
