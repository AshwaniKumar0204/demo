(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageSemesterController', ManageSemesterController);

    ManageSemesterController.$inject = ["$scope", "adminService", "ConstantData", "Factory"];

    function ManageSemesterController($scope, adminService, ConstantData, Factory) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.Semester = {};

        $scope.getStatusList = function () {
            $scope.dataLoading = true;
            adminService.getStatusList()
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.StatusList = response.data.StatusList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getStatusList();

        $scope.getSemesterList = function () {
            if ($scope.ClassId == null) {
                $scope.SemesterList = [];
                return;
            }
            var obj = {
                ClassId: $scope.ClassId
            }
            $scope.dataLoading = true;
            adminService.getSemesterList(obj)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.SemesterList = response.data.SemesterList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getSemesterList();

        $scope.getClassList = function () {
            $scope.dataLoading = true;
            adminService.getClassList(1)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ClassList = response.data.ClassList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                });
        }, function (err) {
            toastr.error("Some error occured while fetching data");
            $scope.dataLoading = false;
        }
        $scope.getClassList();

        $scope.$watch('formSemester.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.SaveSemester = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var SemesterModel = {
                    SemesterId: $scope.Semester.SemesterId,
                    ClassId: $scope.Semester.ClassId,
                    Status: $scope.Semester.Status,
                    SemesterName: $scope.Semester.SemesterName,
                    SemesterNo: $scope.Semester.SemesterNo,
                }
                $scope.dataLoading = true;
                adminService.saveSemester(SemesterModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (SemesterModel.SemesterId > 0)
                                toastr.success("Semester Updated Successfully.");
                            else
                                toastr.success("New Semester Created Successfully.");
                            $('#modal_Student_Charge').modal('hide');
                            $scope.getSemesterList();
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

        $scope.Semester = {};
        $scope.Semester.SemesterId = null;
        $scope.Semester.ClassId = null;
        $scope.Semester.SemesterName = null;
        $scope.Semester.Status = null;

        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.Semester.SemesterId = null;
            $scope.Semester.ClassId = $scope.ClassId;
            $scope.Semester.SemesterName = null;
            $scope.Semester.SemesterNo = null;
            //$scope.Semester.Status = null;
            $scope.formSemester.$setPristine();
            $scope.formSemester.$setUntouched();
        }

        $scope.newSemester = function () {
            if ($scope.ClassId > 0) {
                $scope.resetForm();
                $('#modal_Student_Charge').modal('show');
            } else {
                toastr.error("Class is required !!");
            }
        }

        $scope.editSemester = function (SemesterModel) {
            $scope.resetForm();
            $scope.Semester.SemesterId = SemesterModel.SemesterId;
            $scope.Semester.ClassId = SemesterModel.ClassId;
            $scope.Semester.SemesterName = SemesterModel.SemesterName;
            $scope.Semester.SemesterNo = SemesterModel.SemesterNo;
            $scope.Semester.Status = SemesterModel.Status;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (SemesterId) {
            $scope.SemesterId = SemesterId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteSemester = function () {
            $scope.dataLoading = true;
            adminService.deleteSemester($scope.SemesterId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Semester deleted successfully.", "Deleted Successfully.")
                        $scope.SemesterId = null;
                        $('#modal_confirmation').modal('hide');
                        $scope.getSemesterList();
                    } else {
                        toastr.error(response.data.Message);
                        $scope.dataLoading = false;
                    }
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
    }

})();

