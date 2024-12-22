(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageClassSubjectController', ManageClassSubjectController);

    ManageClassSubjectController.$inject = ["$scope", "adminService", "ConstantData", "Factory"];

    function ManageClassSubjectController($scope, adminService, ConstantData, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.SubjectTypeList = ConstantData.SubjectType;
        $scope.ClassSubjectStatusList = ConstantData.ClassSubjectStatus;
        $scope.ClassId = null;
        $scope.SectionId = null;

        $scope.getSubjectList = function () {
            $scope.dataLoading = true;
            adminService.getSubjectList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.SubjectList = response.data.SubjectList;
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
        $scope.getSubjectList();

        $scope.getClassList = function () {
            $scope.dataLoading = true;
            adminService.getClassList(1)
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

        $scope.changeClass = function () {
            $scope.SectionList = [];
            angular.forEach($scope.MainSectionList, function (section, key) {
                if (section.ClassId == $scope.ClassId) {
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

        $scope.getAcademicSessionList = function () {
            $scope.dataLoading = true;
            adminService.getAcademicSessionList(1)
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

        $scope.getClassSubjectList = function () {
            if ($scope.SectionId == null || $scope.AcademicSessionId == null) {
                $scope.ClassSubjectList = [];
                return;
            }
            $scope.dataLoading = true;
            adminService.getClassSubjectList($scope.SectionId, 0, $scope.AcademicSessionId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ClassSubjectList = response.data.ClassSubjectList;
                        $scope.calculateTotal();
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.$watch('formClassSubject.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.openEditConfirmation = function () {
            if ($scope.ClassSubject.ClassSubjectId > 0) {
                $('#modal_Student_Charge').modal('hide');
                $('#modal_confirmation_edit').modal('show');
            } else {
                $scope.saveClassSubject(false);
            }
        }

        $scope.saveClassSubject = function (IsChangeInOtherSections) {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                // Is Subject is already exist
                var ClassSubjectModel = {
                    AcademicSessionId: $scope.AcademicSessionId,
                    SectionId: $scope.SectionId,
                    TheoryPeriods: $scope.ClassSubject.TheoryPeriods,
                    SubjectId: $scope.ClassSubject.SubjectId,
                    ClassSubjectStatus: $scope.ClassSubject.ClassSubjectStatus,
                    ClassSubjectId: $scope.ClassSubject.ClassSubjectId,
                    SubjectType: $scope.ClassSubject.SubjectType,
                    PracticalPeriods: $scope.ClassSubject.PracticalPeriods,
                    IsIncluded: $scope.ClassSubject.IsIncluded,
                    IsChangeInOtherSections: IsChangeInOtherSections,
                    ParentClassSubjectId: $scope.ClassSubject.ParentClassSubjectId,
                }

                $scope.dataLoading = true;
                adminService.saveClassSubject(ClassSubjectModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (ClassSubjectModel.ClassSubjectId > 0)
                                toastr.success("Class Subject Updated Successfully.");
                            else
                                toastr.success("New Class Subject Created Successfully.");
                            $('#modal_Student_Charge').modal('hide');
                            $('#modal_confirmation_edit').modal('hide');
                            $scope.getClassSubjectList();
                        } else {
                            toastr.error(response.data.Message);
                            $scope.dataLoading = false;
                        }
                    }, function (err) {
                        toastr.error("Some error occured while fetching data");
                        $scope.dataLoading = false;
                    })

            } else {
                toastr.error("Fill all fields!", "Validation Error");
            }
        }

        $scope.ClassSubject = {};
        $scope.ClassSubject.SubjectType = null;
        $scope.ClassSubject.SubjectId = null;
        $scope.ClassSubject.ClassSubjectStatus = null;
        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.ClassSubject.ClassSubjectId = null;
            $scope.ClassSubject.SubjectId = null;
            $scope.ClassSubject.AcademicSessionId = null;
            $scope.formClassSubject.$setPristine();
            $scope.formClassSubject.$setUntouched();
        }

        $scope.newClassSubject = function () {
            $scope.resetForm();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editClassSubject = function (ClassSubjectModel) {
            $scope.resetForm();
            $scope.ClassSubject = ClassSubjectModel;
            $scope.ClassSubject.PracticalPeriods = parseInt($scope.ClassSubject.PracticalPeriods);
            $scope.ClassSubject.TheoryPeriods = parseInt($scope.ClassSubject.TheoryPeriods);
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (ClassSubjectId) {
            $scope.ClassSubjectId = ClassSubjectId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteClassSubject = function () {
            $scope.dataLoading = true;
            adminService.deleteClassSubject($scope.ClassSubjectId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Class Subject deleted successfully.", "Deleted Successfully.")
                        $scope.ClassSubjectId = null;
                        $('#modal_confirmation').modal('hide');
                        $scope.getClassSubjectList();
                    } else {
                        toastr.error(response.data.Message);
                        $scope.dataLoading = false;
                    }
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.changePositionUp = function (ClassSubjectId) {
            $scope.dataLoading = true;
            adminService.changePositionUp(ClassSubjectId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.getClassSubjectList();
                    } else {
                        toastr.error(response.data.Message);
                        $scope.dataLoading = false;
                    }
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.changePositionDown = function (ClassSubjectId) {
            $scope.dataLoading = true;
            adminService.changePositionDown(ClassSubjectId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.getClassSubjectList();
                    } else {
                        toastr.error(response.data.Message);
                        $scope.dataLoading = false;
                    }
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.copyClassSubjects = function () {
            $scope.dataLoading = true;
            adminService.copyClassSubjects($scope.SectionId, $scope.AcademicSessionId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Copied Successfully.");
                        $('#modal_confirmation_copy').modal('hide');
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }

        $scope.calculateTotal = function () {
            $scope.TotalTheoryPeriods = 0;
            $scope.TotalPracticalPeriods = 0;
            $scope.ParentClassSubjectList = [];
            angular.forEach($scope.ClassSubjectList, function (classSubject, key) {
                if (classSubject.IsIncluded) {
                    $scope.TotalPracticalPeriods += classSubject.PracticalPeriods;
                    $scope.TotalTheoryPeriods += classSubject.TheoryPeriods;
                }
                if (classSubject.ParentClassSubjectId == null)
                    $scope.ParentClassSubjectList.push(classSubject);
            })
        }


    }
})();
