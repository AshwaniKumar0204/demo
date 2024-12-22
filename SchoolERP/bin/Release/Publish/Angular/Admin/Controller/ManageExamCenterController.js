(function () {
    'use strict';

    angular
        .module('app')
        .controller('ManageExamCenterController', ManageExamCenterController);

    ManageExamCenterController.$inject = ["$scope", "adminService", "ConstantData", "Factory", "loadDataService"];

    function ManageExamCenterController($scope, adminService, ConstantData, Factory, loadDataService) {
        $scope.checkSysytemLogin();
        $scope.currentPage = 1;
        $scope.sizeOptions = Factory.SizeOptions;
        $scope.pageSize = $scope.sizeOptions[0];
        $scope.sort = function (key) {
            $scope.sortKey = key;
            $scope.reverse = !$scope.reverse;
        };
        $scope.StatusList = ConstantData.Status;
        $scope.StatusDetail = ConstantData.StatusDetail;
        $scope.ExamCenter = {};

        $scope.getExamCenterList = function () {
            $scope.dataLoading = true;
            adminService.getExamCenterList(0)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        $scope.ExamCenterList = response.data.ExamCenterList;
                    } else {
                        toastr.error(response.data.Message);
                    }
                    $scope.dataLoading = false;
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
        }
        $scope.getExamCenterList();

        $scope.$watch('formExamCenter.$valid', function (value) {
            $scope.IsFormValid = value;
        })

        $scope.saveExamCenter = function () {
            $scope.IsSubmitted = true;
            if ($scope.IsFormValid) {
                var ExamCenterModel = {
                    ExamCenterId: $scope.ExamCenter.ExamCenterId,
                    ExamCenterName: $scope.ExamCenter.ExamCenterName,
                    ReportingTime: $scope.ExamCenter.ReportingTime,
                    ExamTime: $scope.ExamCenter.ExamTime,
                    ExamDate: loadDataService.getDateTime($scope.ExamCenter.ExamDate),
                    Exam: $scope.ExamCenter.Exam,
                    Status: $scope.ExamCenter.Status,
                }

                $scope.dataLoading = true;
                adminService.saveExamCenter(ExamCenterModel)
                    .then(function (response) {
                        if (response.data.Message == 'Success') {
                            if (ExamCenterModel.ExamCenterId > 0)
                                toastr.success("Exam Center Updated Successfully.");
                            else
                                toastr.success("New Exam Center Created Successfully.");
                            $scope.resetForm();
                            $('#modal_Student_Charge').modal('hide');
                            $scope.getExamCenterList();
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

        $scope.resetForm = function () {
            $scope.IsSubmitted = false;
            $scope.ExamCenter = {};
            $scope.ExamCenter.ExamDate = new Date();
            $scope.ExamCenter.Status = null;
            $scope.formExamCenter.$setPristine();
            $scope.formExamCenter.$setUntouched();
        }

        $scope.newExamCenter = function () {
            $scope.resetForm();
            $('#modal_Student_Charge').modal('show');
        }

        $scope.editExamCenter = function (ExamCenterModel) {
            $scope.resetForm();
            $scope.ExamCenter = ExamCenterModel;
            $scope.ExamCenter.ExamDate = new Date(ExamCenterModel.ExamDate);
            $('#modal_Student_Charge').modal('show');
        }

        $scope.deleteConfirmation = function (ExamCenterId) {
            $scope.ExamCenterId = ExamCenterId;
            $('#modal_confirmation').modal('show');
        }

        $scope.deleteExamCenter = function () {
            $scope.dataLoading = true;
            adminService.deleteExamCenter($scope.ExamCenterId)
                .then(function (response) {
                    if (response.data.Message == 'Success') {
                        toastr.success("One Exam Center deleted successfully.", "Deleted Successfully.")
                        $scope.ExamCenterId = null;
                        $('#modal_confirmation').modal('hide');
                        $scope.getExamCenterList();
                    } else {
                        toastr.error(response.data.Message);
                        $scope.dataLoading = false;
                    }
                }, function (err) {
                    toastr.error("Some error occured while fetching data");
                    $scope.dataLoading = false;
                })
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
    }
})();
