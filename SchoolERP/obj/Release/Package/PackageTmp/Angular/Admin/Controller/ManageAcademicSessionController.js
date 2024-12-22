(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageAcademicSessionController', ManageAcademicSessionController);

    ManageAcademicSessionController.$inject = ["$scope", "adminService", "ConstantData", "Factory", "loadDataService"];

    function ManageAcademicSessionController($scope, adminService, ConstantData, Factory, loadDataService) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[1];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.AcademicSession = {};

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

        $scope.getAcademicSessionList = function () {
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

        $scope.$watch('formAcademicSession.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.SaveAcademicSession = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var AcademicSessionModel = {
                    AcademicSessionId: $scope.AcademicSession.AcademicSessionId,
                    ClassId: $scope.AcademicSession.ClassId,
                    Status: $scope.AcademicSession.Status,
                    SessionName: $scope.AcademicSession.SessionName,
                    AcademicSessionNo: $scope.AcademicSession.AcademicSessionNo,
                    SessionStartDate: loadDataService.getDateTime($scope.AcademicSession.SessionStartDate),
                    SessionEndDate: loadDataService.getDateTime($scope.AcademicSession.SessionEndDate),
                }
                $scope.dataLoading = true;
                adminService.saveAcademicSession(AcademicSessionModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (AcademicSessionModel.AcademicSessionId > 0)
                                toastr.success("Academic Session Updated Successfully.");
                            else
                                toastr.success("New AcademicSession Created Successfully.");
                            $('#modal_Student_Charge').modal('hide');
                            $scope.getAcademicSessionList();
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

        $scope.AcademicSession = {};
        $scope.AcademicSession.AcademicSessionId = null;
        $scope.AcademicSession.ClassId = null;
        $scope.AcademicSession.SessionName = null;
        $scope.AcademicSession.Status = null;

        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.AcademicSession.AcademicSessionId = null;
            $scope.AcademicSession.ClassId = $scope.ClassId;
            $scope.AcademicSession.SessionName = null;
            $scope.AcademicSession.SessionStartDate = null;
            $scope.AcademicSession.SessionEndDate = null;
            //$scope.AcademicSession.Status = null;
            $scope.formAcademicSession.$setPristine();
            $scope.formAcademicSession.$setUntouched();
        }

        $scope.newAcademicSession = function () {
            $scope.resetForm();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editAcademicSession = function (AcademicSessionModel) {
            $scope.resetForm();
            $scope.AcademicSession.AcademicSessionId = AcademicSessionModel.AcademicSessionId;
            $scope.AcademicSession.ClassId = AcademicSessionModel.ClassId;
            $scope.AcademicSession.SessionName = AcademicSessionModel.SessionName;
            $scope.AcademicSession.SessionStartDate = new Date(AcademicSessionModel.SessionStartDate);
            if ($scope.AcademicSession.SessionEndDate)
                $scope.AcademicSession.SessionEndDate = new Date(AcademicSessionModel.SessionEndDate);
            $scope.AcademicSession.Status = AcademicSessionModel.Status;
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (AcademicSessionId) {
            $scope.AcademicSessionId = AcademicSessionId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteAcademicSession = function () {
            $scope.dataLoading = true;
            adminService.deleteAcademicSession($scope.AcademicSessionId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("Academic Session deleted successfully.", "Deleted Successfully.")
                        $scope.AcademicSessionId = null;
                        $('#modal_confirmation').modal('hide');
                        $scope.getAcademicSessionList();
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

